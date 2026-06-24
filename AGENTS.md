# Agent Instructions

Follow the canonical repository policy in `AI_POLICY.md`.

For Unity work, use MCP for Unity by default unless it is clearly unnecessary for the specific task.

For Unity visual or layout tasks:

- inspect once
- edit once
- verify once
- if still off, do at most one correction pass
- then stop

Before handing off Unity C# changes, run:

```sh
python3 tools/ai-guardrails/check_ai_guardrails.py
```

Use `AI_CONTEXT.ignore` as the canonical context-pruning policy.
