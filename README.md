<div align="center">
  <img src="RayStudio.png" alt="RayStudio Logo" width="120"/>

  <h1>NetDashboard</h1>
  <p><strong>Network & Mail Diagnostics Toolkit for Windows</strong></p>

  ![Windows](https://img.shields.io/badge/Windows-10%2F11-lightgrey?logo=windows)
  ![.NET](https://img.shields.io/badge/.NET-8.0-orange?logo=dotnet)
  ![Language](https://img.shields.io/badge/Sprache-DE%20%7C%20EN%20%7C%20FR-lightgrey)
  [![CI](https://github.com/9t29zhmwdh-coder/NetDashboard/actions/workflows/ci.yml/badge.svg)](https://github.com/9t29zhmwdh-coder/NetDashboard/actions/workflows/ci.yml)
</div>

> 🇩🇪 [Deutsche Version](README.de.md)

A compact Windows desktop app (C#, WPF, .NET 8) that combines DNS diagnostics, mail server discovery, network connectivity checks, and system information in a single dark-themed interface; with full trilingual UI.

Built for M365-connected infrastructure. Validates DNS and Exchange Online connectivity requirements aligned with [Microsoft 365 network connectivity principles](https://learn.microsoft.com/en-us/microsoft-365/enterprise/microsoft-365-network-connectivity-principles).

---

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

## Tech Stack

- **Language:** C# 12
- **Framework:** WPF / .NET 8
- **UI:** Dark theme, MVVM pattern
- **DNS:** System.Net.Dns + raw UDP resolver
- **No external dependencies**: fully offline after launch

---

**Author:** [Rafael Yilmaz](https://github.com/9t29zhmwdh-coder) &nbsp;·&nbsp; **Status:** Active &nbsp;·&nbsp; **Last Updated:** June 2026
