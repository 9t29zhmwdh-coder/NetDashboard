<div align="center">
  <img src="RayStudio.png" alt="RayStudio Logo" width="120"/>

  <h1>NetDashboard — Getting Started</h1>
</div>

[🇩🇪 Deutsche Version](GETTING_STARTED.de.md) · [Back to README](README.md)

---

### Windows

NetDashboard is a native Windows desktop app (WPF) with no prebuilt installer yet — you build it from source. This only works on Windows; there's no Linux or macOS version, since WPF doesn't run there.

**1. Open a terminal**

Click the **Start button** (bottom left) or press the Windows key, type `PowerShell`, and click **Windows PowerShell** in the results. A blue window with text opens — that's your terminal for the next steps (press Enter after each command).

<!-- TODO: Screenshot of the Start menu with "PowerShell" search -->

**2. Check whether .NET is already installed**

```powershell
dotnet --version
```

- Shows a version number like `8.0.xxx` → continue with step 3.
- Shows a red error like `dotnet : The term 'dotnet' is not recognized as the name of a cmdlet` → .NET isn't installed. Open [dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0) and download the **.NET 8.0 SDK** for Windows (not "Runtime" — building from source needs the SDK). Run the installer, click through it, done. Close this PowerShell window and open a new one afterward (repeat step 1) so Windows picks up the new command.

**3. Download NetDashboard**

No Git needed. Open [github.com/9t29zhmwdh-coder/NetDashboard](https://github.com/9t29zhmwdh-coder/NetDashboard) in your browser, click the green **"Code"** button, then **"Download ZIP"**. The file lands in your "Downloads" folder. Right-click the ZIP → **"Extract All"** → pick a folder (e.g. your Desktop) → "Extract".

<!-- TODO: Screenshot of the green "Code" button with "Download ZIP" -->

**4. Build and run**

In PowerShell, switch into the extracted folder. If it's e.g. `NetDashboard-main` on your Desktop:

```powershell
cd Desktop\NetDashboard-main
dotnet build NetDashboard.csproj --configuration Release
dotnet run --project NetDashboard.csproj
```

The first build takes a moment; after that, the NetDashboard window opens automatically.

<!-- TODO: Screenshot of the running NetDashboard window -->

**5. Uninstalling again**

Just delete the extracted folder (`bin/` and `obj/` inside it hold the build output). NetDashboard doesn't touch the Windows registry and keeps no settings file, so nothing else is left behind.

### Troubleshooting

| Message / problem | What to do |
|---|---|
| `dotnet` still not recognized after installing | Close the PowerShell window and open a new one — Windows only re-reads the installed paths on a fresh start |
| Windows shows "Windows protected your PC" (SmartScreen) when running a self-built `.exe` | Normal for unsigned small tools; click "More info" → "Run anyway" |
| `cd Desktop\NetDashboard-main` doesn't work | Check the actual folder name in File Explorer — GitHub usually appends `-main` to the downloaded ZIP's folder name |
| PowerShell complains about script execution being disabled | Only relevant for `.ps1` scripts, not `dotnet run` — if it does come up: run `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned` and confirm with "Y" |

Still stuck? Open an [issue on GitHub](https://github.com/9t29zhmwdh-coder/NetDashboard/issues).
