<div align="center">
  <img src="RayStudio.png" alt="RayStudio Logo" width="120"/>

  <h1>NetDashboard: Erste Schritte</h1>
</div>

[English](GETTING_STARTED.md) · [Zurück zur README](README.de.md)

---

### Windows

NetDashboard ist eine native Windows-Desktop-App (WPF) ohne fertigen Installer: du baust sie aus dem Quellcode. Das funktioniert nur unter Windows; es gibt keine Linux- oder macOS-Version, da WPF dort nicht läuft.

**1. Terminal öffnen**

Auf den **Start-Button** (unten links) klicken oder die Windows-Taste drücken, `PowerShell` eintippen und in den Ergebnissen **Windows PowerShell** anklicken. Ein blaues Fenster mit Text öffnet sich: das ist dein Terminal für die nächsten Schritte (nach jedem Befehl Enter drücken).

<!-- TODO: Screenshot vom Startmenü mit "PowerShell"-Suche -->

**2. Prüfen, ob .NET schon installiert ist**

```powershell
dotnet --version
```

- Erscheint eine Versionsnummer wie `8.0.xxx` → weiter mit Schritt 3.
- Erscheint eine rote Fehlermeldung wie `Die Benennung "dotnet" wurde nicht als Name eines Cmdlets ... erkannt` → .NET ist nicht installiert. Öffne [dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0) und lade das **.NET 8.0 SDK** für Windows herunter (nicht "Runtime": zum Bauen aus Quellcode wird das SDK gebraucht). Installer ausführen, durchklicken, fertig. Danach dieses PowerShell-Fenster schliessen und neu öffnen (Schritt 1 wiederholen), damit Windows den neuen Befehl kennt.

**3. NetDashboard herunterladen**

Kein Git nötig. Öffne [github.com/9t29zhmwdh-coder/NetDashboard](https://github.com/9t29zhmwdh-coder/NetDashboard) im Browser, klicke auf den grünen **"Code"**-Button, dann **"Download ZIP"**. Die Datei landet im Ordner "Downloads". Rechtsklick auf die ZIP-Datei → **"Alle extrahieren"** → einen Ordner auswählen (z. B. den Desktop) → "Extrahieren".

<!-- TODO: Screenshot vom grünen "Code"-Button mit "Download ZIP" -->

**4. Bauen und starten**

Im PowerShell-Fenster in den entpackten Ordner wechseln. Falls er z. B. `NetDashboard-main` auf dem Desktop heisst:

```powershell
cd Desktop\NetDashboard-main
dotnet build NetDashboard.csproj --configuration Release
dotnet run --project NetDashboard.csproj
```

Der erste Build dauert einen Moment; danach öffnet sich das NetDashboard-Fenster automatisch.

<!-- TODO: Screenshot vom laufenden NetDashboard-Fenster -->

**5. Wieder deinstallieren**

Einfach den entpackten Ordner löschen (`bin/` und `obj/` darin enthalten die Build-Ausgabe). NetDashboard schreibt nicht in die Windows-Registry und hat keine Einstellungsdatei, es bleibt also nichts zurück.

### Troubleshooting

| Meldung / Problem | Was tun |
|---|---|
| `dotnet` wird auch nach der Installation nicht erkannt | PowerShell-Fenster schliessen und neu öffnen: Windows liest installierte Pfade nur bei einem frischen Start neu ein |
| Windows zeigt "Windows hat Ihren PC geschützt" (SmartScreen) bei selbst gebauter `.exe` | Normal bei unsignierten kleinen Tools; auf "Weitere Informationen" → "Trotzdem ausführen" klicken |
| `cd Desktop\NetDashboard-main` funktioniert nicht | Tatsächlichen Ordnernamen im Explorer prüfen: GitHub hängt beim ZIP-Download meist `-main` an |
| PowerShell meldet, dass die Skriptausführung deaktiviert ist | Betrifft nur `.ps1`-Skripte, nicht `dotnet run`, falls es doch vorkommt: `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned` ausführen und mit "J" bestätigen |

Hakt's trotzdem? Ein [Issue auf GitHub](https://github.com/9t29zhmwdh-coder/NetDashboard/issues) eröffnen.
