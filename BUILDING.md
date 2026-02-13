# Building XRNeckSafer (Windows)

This repository is a Windows Visual Studio solution containing:
- `XR_APILAYER_NOVENDOR_XRNeckSafer` (C++ OpenXR API layer DLL)
- `XRNeckSaferApp` (.NET Framework 4.7.2 WinForms app)
- `Installer` (WiX-based MSI project)

## Prerequisites

1. **Windows 10/11**
2. **Visual Studio 2022** (17.x, Community/Pro/Enterprise)
   - Install workloads:
     - **Desktop development with C++**
     - **.NET desktop development**
   - Individual components:
     - MSVC v142 or newer C++ x64/x86 build tools
     - Windows 10/11 SDK
3. **.NET Framework 4.7.2 Developer Pack**
4. **NuGet CLI** (for `packages.config` restore) or Visual Studio NuGet restore
5. **WiX Toolset** (for installer build only)
   - Needed if you build `Installer/Installer.wixproj`

## Restore dependencies

This repo uses legacy `packages.config` in both managed and native projects.
The `packages/` folder is intentionally not committed.

From a **Developer Command Prompt for VS 2022** at repo root:

```bat
nuget restore XRNeckSafer.sln
```

If `nuget` is not on PATH, use Visual Studio's package restore (open solution and restore).

## Build from command line

From a **Developer Command Prompt for VS 2022**:

```bat
msbuild XRNeckSafer.sln /m /p:Configuration=Release /p:Platform=x64
```

For app-only + native layer (skip installer):

```bat
msbuild XRNeckSafer.sln /m /p:Configuration=Release /p:Platform=x64 /t:XR_APILAYER_NOVENDOR_XRNeckSafer;XRNeckSaferApp
```

## Build in Visual Studio

1. Open `XRNeckSafer.sln`
2. Restore NuGet packages
3. Select `Release | x64`
4. Build solution

## Register/unregister implicit OpenXR API layer (testing)

### Option A: Use bundled scripts
Run as Administrator:
- Register: `XR_APILAYER_NOVENDOR_XRNeckSafer\Install-XR_APILAYER_NOVENDOR_XRNeckSafer.ps1`
- Unregister: `XR_APILAYER_NOVENDOR_XRNeckSafer\Uninstall-XR_APILAYER_NOVENDOR_XRNeckSafer.ps1`

### Option B: Manual registry
The implicit API layer key is:
`HKLM\Software\Khronos\OpenXR\1\ApiLayers\Implicit`

Add/remove a `REG_DWORD` value whose name is the full path to
`XR_APILAYER_NOVENDOR_XRNeckSafer.json` and value `0` (enabled).

After registration changes, restart your OpenXR runtime and target app.

## Runtime validation notes

- Start `XRNeckSaferApp.exe`
- In app menu: **OpenXR → Show active OpenXR API Layers**
- Verify `XR_APILAYER_NOVENDOR_XRNeckSafer` is listed
- Start an OpenXR title and use center/offset buttons as configured
