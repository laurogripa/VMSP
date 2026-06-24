# AI Policy

This is the canonical, model-agnostic operating contract for AI and CLI agents working in this repository. Tool-specific files such as `AGENTS.md`, `.cursor/rules/unity.mdc`, and `.codex/config.toml` are thin adapters to this policy.

## Repository Boundaries

- The Unity project lives under `Project/`.
- Canonical AI policy files live at the Git root.
- Do not move Unity assets, scenes, prefabs, package files, project settings, or `.meta` files unless the task explicitly requires it.
- Preserve Unity serialization. Keep `.meta` files paired with their assets.
- Treat `Project/Assets/Plugins`, `Project/Assets/Tobii`, and `Project/Assets/Joystick Pack` as third-party or plugin-heavy code unless the task explicitly targets them.

## Change Discipline

- Keep changes scoped to the user request.
- Prefer existing project patterns over new architecture.
- Do not update Unity packages, add asmdefs, change scenes, change prefabs, or alter project settings without explicit approval.
- Do not introduce secrets or machine-specific absolute paths.
- For ordinary C# changes, prefer file edits and mechanical validation.
- For Unity serialized state, scene hierarchy, prefab references, asset import settings, or Editor-only state, inspect before mutating.

## Unity C# Rules

New or modified Unity C# code must avoid:

- `GameObject.Find`
- `Transform.Find`
- `UnityEngine.UI.Text`
- obvious mutable public fields on Unity component scripts

Use serialized private fields, explicit references, dependency injection through scene/prefab wiring, TextMeshPro, or UI Toolkit as appropriate.

The following APIs are allowed when justified, but should be treated as warnings during review:

- `GetComponent<T>`
- `Instantiate`
- `Destroy`
- broad scene lookup APIs such as `FindObjectOfType`, `FindObjectsOfType`, and `FindGameObjectWithTag`

Run `python3 tools/ai-guardrails/check_ai_guardrails.py` before handing off C# changes. Use `--all` only for explicit technical-debt inventory work.

## Validation Gates

Before finalizing a change:

- Confirm `Project/ProjectSettings/ProjectVersion.txt` remains `6000.0.25f1` unless the task explicitly approved an upgrade.
- Run targeted tests or static checks relevant to the files changed.
- Run the AI guardrail script for Unity C# changes.
- State any checks that could not be run.

## Unity MCP Usage

`MCP for Unity` is the recommended Unity MCP bridge for this repository because it integrates with the Unity Editor and supports scene, asset, script, test, tool-group, and optional Roslyn validation workflows.

For Unity work, always use MCP for Unity by default unless it is clearly unnecessary for the specific task. Routine pure text edits to non-Unity docs may skip MCP. Scene, prefab, asset, project setting, play mode, camera, UI, and gameplay validation work must use MCP when the bridge is available.

Current repository dependency, from `Project/Packages/manifest.json`:

```text
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
```

This matches the current documented install path:

```text
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
```

The package installation is working with the latest configured Git URL. Runtime server health is still unknown in this repo-only context when the Unity MCP bridge is not running. Do not change the package URL during routine AI setup. Package updates require explicit approval.

Use Unity MCP when Unity Editor state matters:

- scene hierarchy inspection
- serialized reference checks
- asset and prefab inspection
- Unity test runner tasks
- Editor status checks

Do not use Unity MCP for broad scene, prefab, package, project setting, or asset mutations without an approved task-specific plan.

Unity MCP must be local-only by default. Do not use remote MCP server mode unless authentication, network exposure, and secrets handling have been explicitly reviewed.

Keep only the `core` tool group enabled by default. Enable additional groups only for the active task:

- `testing` for Unity test runner work
- `ui` for UI Toolkit work
- `vfx` for shaders, VFX Graph, or procedural textures
- `docs` for Unity API lookup
- `profiling` only when profiling is explicitly requested

Disable extra groups after use to reduce prompt size and wrong-tool selection.

Roslyn validation is optional. Recommend it only when agents will write substantial C# through MCP or repeated compile errors occur. Do not install Roslyn DLLs automatically because that adds files under `Assets/Plugins/Roslyn/` and changes scripting define symbols.

## Iteration Caps

For Unity scene layout, visual adjustment, gameplay presentation, or MCP inspection tasks:

- Maximum 1 inspection pass before editing.
- Maximum 1 implementation pass before verification.
- Maximum 1 verification pass after editing.
- Maximum 2 total correction attempts for the same visual issue.
- After the second correction attempt, stop and report the best current result instead of continuing to tune.

A pass means:

- inspection pass: reading scene state, hierarchy, transforms, scripts, or taking one screenshot
- implementation pass: one grouped set of edits for the issue
- verification pass: one validation step such as one screenshot, one play check, or one scene inspection

Do not loop on visual polish without a new user instruction.
If the next step would be another guess-based adjustment, stop and ask for direction.
State the exact files, objects, or transforms to be changed before making Unity visual or layout edits.
Prefer one numeric correction over multiple micro-adjustments.

## Context Policy

Use `AI_CONTEXT.ignore` as the canonical context-pruning policy. Tool-specific ignore files should mirror or point to it.
