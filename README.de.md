<div align="center">
  <img src="RayStudio.png" alt="RayStudio Logo" width="120"/>
  <h1>NetDashboard</h1>
</div>

[🇬🇧 English Version](README.md)

**Netzwerk- & Mail-Toolkit für Windows — C# · WPF · .NET 8**

NetDashboard ist ein kompaktes Windows-Werkzeug, das DNS-Abfragen, E-Mail-Server-Erkennung, Netzwerkdiagnose und Systeminformationen in einer einzigen Dark-Theme-Oberfläche vereint.

![Windows](https://img.shields.io/badge/Windows-10%2F11-0078D4?logo=windows)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-green)

---

## Funktionen

| Tab | Funktion |
|---|---|
| **Mail-Check** | E-Mail-Adresse eingeben → IMAP- & SMTP-Server automatisch per DNS ermitteln |
| **DNS-Check** | Abfragen für A, AAAA, MX, TXT, NS, CNAME, SOA, PTR und ALL — parallel über bis zu 3 Resolver |
| **Verbindung** | Ping, Port-Test und Traceroute zu beliebigen Hosts |
| **Mein System** | IP-Konfiguration, DNS-Server, DNS-Cache anzeigen/leeren, ARP-Tabelle, Routing-Tabelle, öffentliche IP |

**DNS-Resolver** wählbar: Google (8.8.8.8) · Cloudflare (1.1.1.1) · Quad9 (9.9.9.9)

---

## Sprachen / Languages / Langues

Die Sprache ist direkt in der App umschaltbar — kein Neustart nötig.

🇩🇪 Deutsch &nbsp;|&nbsp; 🇬🇧 English &nbsp;|&nbsp; 🇫🇷 Français

---

## Voraussetzungen

- Windows 10 / 11
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Installation

1. .NET 8 Runtime installieren (falls nicht vorhanden)
2. Release herunterladen
3. `NetDashboard.exe` starten — kein Installer erforderlich

---

## Datenschutz

- Keine Telemetrie, keine Cloud-Verbindungen
- Alle DNS-Abfragen laufen direkt über die konfigurierten Resolver
- Keine Daten werden gespeichert oder übertragen

---

**Author:** [Rafael Yilmaz](https://github.com/9t29zhmwdh-coder) · **Status:** Active Development · **Last Updated:** Juni 2026
