<div align="center">
  <img src="RayStudio.png" alt="RayStudio Logo" width="120"/>

  <h1>NetDashboard</h1>

</div>

<p><strong>Network & Mail Diagnostics Toolkit for Windows</strong></p>

[![CI](https://github.com/9t29zhmwdh-coder/NetDashboard/actions/workflows/ci.yml/badge.svg)](https://github.com/9t29zhmwdh-coder/NetDashboard/actions) ![Microsoft | M365](https://img.shields.io/badge/Microsoft-M365-0078d4?logo=microsoft&logoColor=white) ![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey?logo=windows&logoColor=0078d4) ![C#](https://img.shields.io/badge/C%23-239120?logo=dotnet&logoColor=white) ![AI | Claude Code](https://img.shields.io/badge/AI-Claude_Code-black?logo=anthropic&logoColor=white) ![AI | Copilot](https://img.shields.io/badge/AI-Copilot-black?logo=github&logoColor=white) [![Release](https://img.shields.io/github/v/release/9t29zhmwdh-coder/NetDashboard?color=3F8E7E)](https://github.com/9t29zhmwdh-coder/NetDashboard/releases) [![License](https://img.shields.io/github/license/9t29zhmwdh-coder/NetDashboard?color=lightgrey)](LICENSE)

[🇩🇪 Deutsche Version](README.de.md)

A compact Windows desktop app (C#, WPF, .NET 8) that combines DNS diagnostics, mail server discovery, network connectivity checks, and system information in a single dark-themed interface; with full trilingual UI.

Built for M365-connected infrastructure. Validates DNS and Exchange Online connectivity requirements aligned with [Microsoft 365 network connectivity principles](https://learn.microsoft.com/en-us/microsoft-365/enterprise/microsoft-365-network-connectivity-principles).

> **How it runs:** NetDashboard is a native Windows desktop app (WPF), not a server and not a browser tool. It opens its own window like any installed program.

![NetDashboard](screenshot.png)

---

> 💾 **Download:** [Windows Installer](https://github.com/9t29zhmwdh-coder/NetDashboard/releases/latest) (see release assets for `NetDashboard-Setup-*.exe`): requires the [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0), not code-signed (Windows SmartScreen will warn on first run). Or build from source, see Getting Started below.

---

> 🌱 New here? → [Step-by-step guide for beginners](GETTING_STARTED.md)

---

**In practice:** you get a single dark-themed window that answers "why can't this domain send or receive mail" in a few clicks: paste an address, get the mail server, and cross-check DNS, SPF, DKIM, and DMARC across three public resolvers at once.

## Features

| Tab | Function |
|-----|----------|
| **Mail Check** | Enter any email address → auto-discover IMAP & SMTP server via live DNS lookup |
| **DNS Check** | Query A, AAAA, MX, TXT, NS, CNAME, SOA, PTR and ALL records , parallel across up to 3 resolvers |
| **Connection** | Ping, port test and traceroute to any host |
| **My System** | IP configuration, DNS servers, flush DNS cache, ARP table, routing table, public IP |

**DNS Resolvers:** Google (8.8.8.8) · Cloudflare (1.1.1.1) · Quad9 (9.9.9.9)

---

## Microsoft 365 / Exchange Online Use Cases

NetDashboard is particularly useful in Microsoft 365 and Exchange Online environments:

| Scenario | How |
|----------|-----|
| **Verify M365 mail setup** | Query MX records for `yourdomain.com` ; confirms routing to Exchange Online |
| **SPF / DKIM / DMARC audit** | TXT record lookup shows SPF policy, DKIM selectors, and DMARC enforcement |
| **Auto-discover validation** | Check CNAME `autodiscover.yourdomain.com` points to `autodiscover.outlook.com` |
| **Teams / SIP SRV records** | Use DNS Check → ALL to inspect Teams Direct Routing SRV records |
| **MX priority check** | Verify MX priority order for hybrid Exchange routing |
| **Spam filter bypass** | Inspect all TXT records to confirm 3rd-party filter inclusion in SPF |
| **Connector troubleshooting** | Trace SMTP banner and port availability to Exchange Online IPs |

---

## Languages / Sprachen / Langues

Switch language directly in the app, no restart required.

🇩🇪 Deutsch &nbsp;|&nbsp; 🇬🇧 English &nbsp;|&nbsp; 🇫🇷 Français

---

## Requirements

- Windows 10 / 11
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Getting Started

```bash
# Build from source
dotnet build NetDashboard.csproj --configuration Release

# Run
dotnet run --project NetDashboard.csproj
```

---

## Uninstall / Cleanup

Delete the build output folder (`bin/`, `obj/`), or remove the installed copy via Windows Settings → Apps if you packaged it yourself. NetDashboard does not write to the registry and has no persisted settings file: closing the app leaves nothing else behind.

---

## Tech Stack

- **Language:** C# 12
- **Framework:** WPF / .NET 8
- **UI:** Dark theme, MVVM pattern
- **DNS:** System.Net.Dns + raw UDP resolver
- **No external dependencies**: fully offline after launch

---

**Author:** [Rafael Yilmaz](https://github.com/9t29zhmwdh-coder) · **Status:** Active · ![version](https://img.shields.io/github/v/release/9t29zhmwdh-coder/NetDashboard?color=6b7280&style=flat-square) · **License:** MIT
