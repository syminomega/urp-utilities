# Changelog

## [11.0.4-preview.1] — 2026-10-08 — Unity 6000.6 / URP 17.6

- Fix compilation after the removal of the legacy `Execute` render pass API; use Render Graph on newer Unity versions.
- Retain the legacy execution path for older Unity versions.
- Verified script compilation with Unity 6000.6.4f1 / URP 17.6.0.

## Render Graph update — Unity 6000.3 / URP 17

- Add Render Graph and Renderer List rendering while preserving material overrides, pass selection and depth control.
- Guard against invalid color textures, renderer lists and missing depth attachments.
- Tested with Unity 6000.3.

## Original — Legacy URP

- Introduce custom multi-pass rendering and Rendering Layer filtering, with editor tools for multi-object editing and explicit layer application.
- Use the legacy `ScriptableRenderPass` execution path; the original package targets Unity 2021.3 / URP 11.
