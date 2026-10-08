# Universal Render Pipeline Utilities

Custom multi-pass rendering and Rendering Layer tools for URP.

## Changes from the original version

- Add Rendering Layer inspectors, multi-object editing and explicit application to an object or its children.
- Add Unity 6 / URP 17 Render Graph support with resource validity checks, retaining the legacy rendering path.
- Fix Unity 6000.6 compatibility after the removal of legacy render pass APIs.

See [CHANGELOG.md](CHANGELOG.md) for the three compatibility milestones.

## How to install

### Method A

1. Download this repository manually.
2. Copy the unzipped folder into the project's `Packages` folder, not `Assets`.

### Method B

1. Open the Package Manager window in Unity.
2. Select `Add package from git URL`.
3. Set the URL to `https://github.com/syminomega/urp-utilities.git#11.0` and apply.
