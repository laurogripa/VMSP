# Unity MCP Setup

This repository uses `AI_POLICY.md` as the canonical operating contract. This file documents the current Unity MCP setup and the safe operating rules for agents.

## Current Dependency Audit

`Project/Packages/manifest.json` currently declares:

```json
"com.coplaydev.unity-mcp": "https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main"
```

This matches the currently documented Git URL install path for MCP for Unity:

```text
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
```

The installation is working with the latest configured Git URL. Runtime server health is unknown from repository inspection when the Unity MCP bridge is not running.

Do not change the package URL as part of AI setup. Package changes require explicit approval.

## Manual Client Configuration

Prefer the package UI when available:

```text
Window -> MCP for Unity
```

HTTP default:

```json
{
  "mcpServers": {
    "unityMCP": {
      "url": "http://localhost:8080/mcp"
    }
  }
}
```

Stdio fallback:

```json
{
  "mcpServers": {
    "unityMCP": {
      "command": "uvx",
      "args": ["--from", "mcpforunityserver", "mcp-for-unity", "--transport", "stdio"]
    }
  }
}
```

## Operating Rules

- Keep Unity MCP local-only by default.
- Do not use remote MCP server mode unless auth, network exposure, and secrets handling have been reviewed.
- Prefer MCP inspection before risky Unity edits involving scenes, prefabs, serialized references, assets, or Editor state.
- Prefer file edits for ordinary C# changes.
- Do not make broad scene, prefab, package, project setting, or asset mutations without an approved task-specific plan.

## Tool Groups

Keep only `core` enabled by default. Enable extra groups only for the current task:

- `testing` for Unity test runner tasks
- `ui` for UI Toolkit work
- `vfx` for VFX/shader work
- `docs` for Unity API lookup
- `profiling` only when profiling is explicitly requested

Disable extra groups after use.

## Roslyn Validation

Roslyn validation is optional. Enable it only if agents will write substantial C# through MCP or repeated compile errors occur.

Do not install Roslyn automatically. The installer adds DLLs under `Assets/Plugins/Roslyn/` and changes scripting define symbols.

## Manual Unity Check

If the Unity Editor is available, open:

```text
Window -> MCP for Unity
```

Confirm server/client status and the active tool groups manually. If the bridge is not running, record server health as unknown rather than failed unless a connection attempt was explicitly made and failed.

## References

- https://github.com/CoplayDev/unity-mcp
- https://coplaydev.github.io/unity-mcp/getting-started/install
- https://coplaydev.github.io/unity-mcp/guides/tool-groups
- https://coplaydev.github.io/unity-mcp/guides/roslyn
