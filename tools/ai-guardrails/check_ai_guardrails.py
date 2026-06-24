#!/usr/bin/env python3
"""Guardrails for Unity C# changes made by AI agents.

Default mode checks only changed Git additions so existing baseline debt does
not fail routine work. Use --all for explicit project-wide debt inventory.
"""

from __future__ import annotations

import argparse
import re
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path


EXCLUDED_DIRS = (
    Path("Project/Assets/Plugins"),
    Path("Project/Assets/Tobii"),
    Path("Project/Assets/Joystick Pack"),
)


ERROR_PATTERNS = (
    ("AI001", "Do not add GameObject.Find; wire explicit references instead.", re.compile(r"\bGameObject\s*\.\s*Find\s*\(")),
    ("AI002", "Do not add Transform.Find; wire explicit references instead.", re.compile(r"\bTransform\s*\.\s*Find\s*\(")),
    ("AI003", "Do not add UnityEngine.UI.Text; use TextMeshPro or UI Toolkit.", re.compile(r"\bUnityEngine\s*\.\s*UI\s*\.\s*Text\b")),
)


WARNING_PATTERNS = (
    ("AI101", "Review GetComponent<T> usage; cache or serialize dependencies when practical.", re.compile(r"\bGetComponent\s*<")),
    ("AI102", "Review Instantiate usage; ensure ownership and lifecycle are explicit.", re.compile(r"\bInstantiate\s*\(")),
    ("AI103", "Review Destroy usage; ensure lifecycle side effects are intended.", re.compile(r"\bDestroy\s*\(")),
    ("AI104", "Review broad scene lookup usage; prefer explicit references.", re.compile(r"\b(?:FindObjectOfType|FindObjectsOfType|FindObjectsByType|FindFirstObjectByType|FindAnyObjectByType|FindGameObjectWithTag|FindGameObjectsWithTag)\s*<|\b(?:FindObjectOfType|FindObjectsOfType|FindObjectsByType|FindFirstObjectByType|FindAnyObjectByType|FindGameObjectWithTag|FindGameObjectsWithTag)\s*\(")),
)


PUBLIC_FIELD_PATTERN = re.compile(
    r"""^\s*public\s+
    (?!const\b)
    (?!static\s+readonly\b)
    (?!readonly\b)
    (?!event\b)
    [\w<>\[\],\s?.]+\s+
    [A-Za-z_]\w*
    (?:\s*=\s*[^;]+)?\s*;
    """,
    re.VERBOSE,
)


@dataclass(frozen=True)
class Finding:
    severity: str
    code: str
    path: Path
    line: int
    message: str
    text: str


def run_git(args: list[str], root: Path, check: bool = True) -> subprocess.CompletedProcess[str]:
    return subprocess.run(
        ["git", *args],
        cwd=root,
        check=check,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )


def git_root() -> Path:
    proc = subprocess.run(
        ["git", "rev-parse", "--show-toplevel"],
        check=True,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    return Path(proc.stdout.strip())


def is_excluded(path: Path) -> bool:
    return any(path == excluded or excluded in path.parents for excluded in EXCLUDED_DIRS)


def is_unity_csharp(path: Path) -> bool:
    return path.suffix == ".cs" and path.parts[:2] == ("Project", "Assets") and not is_excluded(path)


def changed_files(root: Path) -> set[Path]:
    files: set[Path] = set()
    for args in (
        ["diff", "--name-only", "--diff-filter=ACMRT", "HEAD"],
        ["diff", "--cached", "--name-only", "--diff-filter=ACMRT"],
        ["ls-files", "--others", "--exclude-standard"],
    ):
        proc = run_git(args, root, check=False)
        if proc.returncode == 0:
            files.update(Path(line) for line in proc.stdout.splitlines() if line)
    return {path for path in files if is_unity_csharp(path)}


def all_files(root: Path) -> set[Path]:
    base = root / "Project" / "Assets"
    if not base.exists():
        return set()
    return {
        path.relative_to(root)
        for path in base.rglob("*.cs")
        if is_unity_csharp(path.relative_to(root))
    }


def added_lines_from_diff(root: Path, path: Path, cached: bool) -> list[tuple[int, str]]:
    args = ["diff", "--cached" if cached else "--no-ext-diff", "--unified=0", "--", str(path)]
    if not cached:
        args = ["diff", "--no-ext-diff", "--unified=0", "--", str(path)]
    proc = run_git(args, root, check=False)
    if proc.returncode != 0:
        return []

    results: list[tuple[int, str]] = []
    new_line = 0
    for line in proc.stdout.splitlines():
        if line.startswith("@@"):
            match = re.search(r"\+(\d+)(?:,(\d+))?", line)
            if match:
                new_line = int(match.group(1))
            continue
        if line.startswith("+++") or line.startswith("---"):
            continue
        if line.startswith("+"):
            results.append((new_line, line[1:]))
            new_line += 1
        elif line.startswith("-"):
            continue
        else:
            new_line += 1
    return results


def file_lines(root: Path, path: Path) -> list[tuple[int, str]]:
    try:
        text = (root / path).read_text(encoding="utf-8")
    except UnicodeDecodeError:
        text = (root / path).read_text(encoding="utf-8", errors="replace")
    return list(enumerate(text.splitlines(), start=1))


def is_tracked(root: Path, path: Path) -> bool:
    proc = run_git(["ls-files", "--error-unmatch", str(path)], root, check=False)
    return proc.returncode == 0


def lines_to_check(root: Path, path: Path, full_file: bool) -> list[tuple[int, str]]:
    if full_file or not is_tracked(root, path):
        return file_lines(root, path)

    seen: dict[tuple[int, str], None] = {}
    for item in added_lines_from_diff(root, path, cached=False) + added_lines_from_diff(root, path, cached=True):
        seen[item] = None
    return list(seen.keys())


def file_has_unity_component(root: Path, path: Path) -> bool:
    try:
        text = (root / path).read_text(encoding="utf-8", errors="replace")
    except FileNotFoundError:
        return False
    return bool(re.search(r":\s*(?:[\w.]+\s*,\s*)*MonoBehaviour\b", text))


def scan_lines(root: Path, path: Path, lines: list[tuple[int, str]], full_file: bool) -> list[Finding]:
    findings: list[Finding] = []
    is_component = file_has_unity_component(root, path)

    for line_no, text in lines:
        stripped = text.strip()
        if not stripped or stripped.startswith("//"):
            continue

        for code, message, pattern in ERROR_PATTERNS:
            if pattern.search(text):
                findings.append(Finding("error", code, path, line_no, message, stripped))

        if is_component and PUBLIC_FIELD_PATTERN.search(text):
            findings.append(
                Finding(
                    "error",
                    "AI004",
                    path,
                    line_no,
                    "Do not add obvious mutable public fields on Unity component scripts; use [SerializeField] private fields or properties.",
                    stripped,
                )
            )

        for code, message, pattern in WARNING_PATTERNS:
            if pattern.search(text):
                findings.append(Finding("warning", code, path, line_no, message, stripped))

    return findings


def print_findings(findings: list[Finding]) -> None:
    for finding in findings:
        print(
            f"{finding.severity.upper()} {finding.code} {finding.path}:{finding.line}: {finding.message}\n"
            f"  {finding.text}"
        )


def main() -> int:
    parser = argparse.ArgumentParser(description="Check Unity C# AI guardrails.")
    parser.add_argument("--all", action="store_true", help="scan all Unity C# files as debt inventory")
    args = parser.parse_args()

    root = git_root()
    paths = all_files(root) if args.all else changed_files(root)

    findings: list[Finding] = []
    for path in sorted(paths):
        if not (root / path).exists():
            continue
        findings.extend(scan_lines(root, path, lines_to_check(root, path, args.all), args.all))

    print_findings(findings)

    errors = [finding for finding in findings if finding.severity == "error"]
    warnings = [finding for finding in findings if finding.severity == "warning"]

    mode = "full project" if args.all else "changed additions"
    print(f"AI guardrails scanned {len(paths)} Unity C# file(s) in {mode} mode: {len(errors)} error(s), {len(warnings)} warning(s).")

    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
