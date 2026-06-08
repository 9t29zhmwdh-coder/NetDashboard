using System.Diagnostics;
using System.Text;

namespace NetDashboard.Services;

public class NetworkService
{
    // Öffentliche Resolver für den Vergleich
    public static readonly (string Name, string Ip)[] PublicResolvers =
    {
        ("Google", "8.8.8.8"),
        ("Cloudflare", "1.1.1.1"),
        ("Quad9", "9.9.9.9"),
    };

    // ---- Generische DNS-Abfrage (eine Quelle) -------------------------

    public Task<string> DnsLookupAsync(string host, string dnsServer, string type)
    {
        var cmd = $"Resolve-DnsName -Name '{Esc(host)}' -Type {type} -ErrorAction Stop";
        if (!string.IsNullOrWhiteSpace(dnsServer))
            cmd += $" -Server '{Esc(dnsServer)}'";
        cmd += " | Format-Table -AutoSize | Out-String -Width 220";
        return RunPowerShellAsync(cmd);
    }

    // ---- DNS-Abfrage über MEHRERE Resolver (Kontrolle) ----------------

    public async Task<string> DnsLookupMultiAsync(
        string host, string type, IReadOnlyList<(string Name, string Ip)> resolvers)
    {
        var tasks = resolvers.Select(async r =>
        {
            string res = await DnsLookupAsync(host, r.Ip, type);
            string label = string.IsNullOrEmpty(r.Ip) ? r.Name : $"{r.Name} ({r.Ip})";
            return $"────── Resolver: {label} ──────\n{res.Trim()}";
        });
        var blocks = await Task.WhenAll(tasks);

        string note = resolvers.Count > 1
            ? "\nℹ Hinweis: Zeigen alle Resolver dasselbe, ist alles in Ordnung. " +
              "Unterschiede deuten auf eine laufende DNS-Umstellung oder ein Resolver-Problem hin."
            : "";
        return string.Join("\n\n", blocks) + "\n" + note;
    }

    // ---- Mail-Check (für Laien) ---------------------------------------

    public async Task<string> MailCheckAsync(
        string email, IReadOnlyList<(string Name, string Ip)> resolvers)
    {
        string domain = ExtractDomain(email);
        if (string.IsNullOrWhiteSpace(domain))
            return "FEHLER: Bitte eine gültige E-Mail-Adresse eingeben (z. B. name@firma.ch).";

        var sb = new StringBuilder();
        sb.AppendLine($"E-Mail:  {email.Trim()}");
        sb.AppendLine($"Domain:  {domain}");
        sb.AppendLine();

        // 1) MX über alle Resolver
        sb.AppendLine("1) Mailserver (MX) – wer nimmt E-Mails für diese Domain an?");
        var tasks = resolvers.Select(async r => (r, hosts: await GetMxHostsAsync(domain, r.Ip)));
        var results = await Task.WhenAll(tasks);

        foreach (var (r, hosts) in results)
        {
            string label = string.IsNullOrEmpty(r.Ip) ? r.Name : $"{r.Name} ({r.Ip})";
            sb.AppendLine($"   {label}:");
            if (hosts.Count == 0) sb.AppendLine("      (keine MX-Einträge gefunden)");
            else foreach (var h in hosts) sb.AppendLine("      " + h);
        }
        sb.AppendLine();
        sb.AppendLine("   Kontrolle über mehrere Resolver:");
        sb.AppendLine("   " + CompareVerdict(results.Select(x => x.hosts).ToList()));
        sb.AppendLine();

        // 2) Anbieter + Einstellungen
        var firstHosts = results.Select(x => x.hosts).FirstOrDefault(h => h.Count > 0) ?? new();
        sb.AppendLine("2) Erkannter Anbieter & empfohlene Einstellungen:");
        sb.AppendLine(BuildProviderHelp(firstHosts, domain));
        sb.AppendLine();

        string ip0 = resolvers.Select(r => r.Ip).FirstOrDefault(i => !string.IsNullOrEmpty(i)) ?? "";

        // 3) SPF
        sb.AppendLine("3) E-Mail-Sicherheit (SPF):");
        string spf = await GetSpfAsync(domain, ip0);
        sb.AppendLine(string.IsNullOrWhiteSpace(spf)
            ? "   Kein SPF-Eintrag gefunden. (SPF schützt vor gefälschten Absendern – sollte gesetzt sein.)"
            : "   " + spf);
        sb.AppendLine();

        // 4) Autodiscover
        sb.AppendLine("4) Automatische Einrichtung (Autodiscover):");
        string ad = await CheckAutodiscoverAsync(domain, ip0);
        sb.AppendLine($"   autodiscover.{domain}: {ad} " +
                      "(wenn vorhanden, richtet Outlook das Konto meist von selbst ein).");
        return sb.ToString();
    }

    // ---- Verbindung / System (unverändert + erweitert) ----------------

    public Task<string> PingAsync(string target) =>
        RunPowerShellAsync($"Test-Connection -ComputerName '{Esc(target)}' -Count 4 " +
                           "| Format-Table -AutoSize | Out-String -Width 220");

    public Task<string> PortTestAsync(string target, int port) =>
        RunPowerShellAsync($"Test-NetConnection -ComputerName '{Esc(target)}' -Port {port} " +
                           "| Select-Object ComputerName,RemoteAddress,RemotePort,TcpTestSucceeded " +
                           "| Format-List | Out-String");

    public Task<string> TraceRouteAsync(string host) =>
        RunProcessAsync("tracert.exe", $"-d -h 20 {Esc(host)}");

    public Task<string> ShowDnsServersAsync() =>
        RunPowerShellAsync("Get-DnsClientServerAddress -AddressFamily IPv4 " +
                           "| Where-Object {$_.ServerAddresses} " +
                           "| Format-Table InterfaceAlias,ServerAddresses -AutoSize | Out-String -Width 220");

    public Task<string> IpConfigAsync() => RunProcessAsync("ipconfig.exe", "/all");

    public Task<string> ShowDnsCacheAsync() =>
        RunPowerShellAsync("Get-DnsClientCache | Format-Table Entry,RecordType,Data -AutoSize | Out-String -Width 220");

    public Task<string> FlushDnsCacheAsync() =>
        RunPowerShellAsync("Clear-DnsClientCache; 'DNS-Cache wurde geleert.'");

    public Task<string> ArpTableAsync() => RunProcessAsync("arp.exe", "-a");
    public Task<string> RouteTableAsync() => RunProcessAsync("route.exe", "print");

    public Task<string> PublicIpAsync() =>
        RunPowerShellAsync("try { Invoke-RestMethod -Uri 'https://api.ipify.org?format=text' -TimeoutSec 8 } " +
                           "catch { 'Konnte oeffentliche IP nicht abrufen (keine Internetverbindung?).' }");

    // ---- DNS-Helfer (ohne doppelte Anführungszeichen!) ----------------

    private async Task<List<string>> GetMxHostsAsync(string domain, string resolverIp)
    {
        string server = string.IsNullOrWhiteSpace(resolverIp) ? "" : " -Server '" + Esc(resolverIp) + "'";
        string cmd =
            "(Resolve-DnsName -Name '" + Esc(domain) + "' -Type MX" + server + " -EA SilentlyContinue " +
            "| Where-Object { $_.QueryType -eq 'MX' } | Sort-Object Preference " +
            "| ForEach-Object { [string]$_.Preference + ' ' + $_.NameExchange }) -join ';'";
        string raw = await RunPowerShellAsync(cmd);
        return raw.Replace("\r", "").Split('\n', ';')
                  .Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
    }

    private async Task<string> GetSpfAsync(string domain, string resolverIp)
    {
        string server = string.IsNullOrWhiteSpace(resolverIp) ? "" : " -Server '" + Esc(resolverIp) + "'";
        string cmd =
            "(Resolve-DnsName -Name '" + Esc(domain) + "' -Type TXT" + server + " -EA SilentlyContinue " +
            "| ForEach-Object { $_.Strings -join ' ' } | Where-Object { $_ -match 'v=spf1' }) -join ' ; '";
        return (await RunPowerShellAsync(cmd)).Trim();
    }

    private async Task<string> CheckAutodiscoverAsync(string domain, string resolverIp)
    {
        string server = string.IsNullOrWhiteSpace(resolverIp) ? "" : " -Server '" + Esc(resolverIp) + "'";
        string cmd =
            "$r = Resolve-DnsName -Name 'autodiscover." + Esc(domain) + "'" + server +
            " -EA SilentlyContinue; if ($r) { 'vorhanden' } else { 'nicht gefunden' }";
        return (await RunPowerShellAsync(cmd)).Trim();
    }

    private static string CompareVerdict(List<List<string>> lists)
    {
        var nonEmpty = lists.Where(l => l.Count > 0).ToList();
        if (nonEmpty.Count <= 1)
            return "✔ Nur eine Quelle abgefragt – für einen echten Vergleich mehrere Resolver anhaken.";
        var norm = nonEmpty.Select(l => string.Join("|", l.Select(s => s.ToLowerInvariant()).OrderBy(s => s)));
        return norm.Distinct().Count() == 1
            ? "✔ Alle Resolver liefern dieselben Mailserver – DNS ist konsistent."
            : "⚠ Unterschiedliche Antworten! Mögliche Gründe: laufende DNS-Umstellung, Zwischenspeicher (Cache) " +
              "oder ein Resolver-Problem. Später erneut prüfen.";
    }

    private static string BuildProviderHelp(List<string> mx, string domain)
    {
        string j = string.Join(" ", mx).ToLowerInvariant();

        if (j.Contains("protection.outlook") || j.Contains("outlook.com") || j.Contains("office365"))
            return
                "   ➜ Microsoft 365 / Exchange Online erkannt.\n" +
                "   • Posteingang (IMAP): outlook.office365.com   Port 993   Verschlüsselung: SSL/TLS\n" +
                "   • Postausgang (SMTP): smtp.office365.com       Port 587   Verschlüsselung: STARTTLS\n" +
                "   • Benutzername: deine komplette E-Mail-Adresse\n" +
                "   • Tipp: In Outlook 'Exchange'/automatische Einrichtung wählen – Adresse + Passwort genügen meist.";

        if (j.Contains("google") || j.Contains("googlemail"))
            return
                "   ➜ Google Workspace / Gmail erkannt.\n" +
                "   • Posteingang (IMAP): imap.gmail.com   Port 993   SSL/TLS\n" +
                "   • Postausgang (SMTP): smtp.gmail.com   Port 587   STARTTLS\n" +
                "   • Benutzername: komplette E-Mail-Adresse  (ggf. App-Passwort / 2FA nötig)";

        if (j.Contains("icloud") || j.Contains("apple"))
            return
                "   ➜ Apple iCloud Mail erkannt.\n" +
                "   • Posteingang (IMAP): imap.mail.me.com   Port 993   SSL/TLS\n" +
                "   • Postausgang (SMTP): smtp.mail.me.com   Port 587   STARTTLS\n" +
                "   • Benutzername: komplette iCloud-Adresse  (App-spezifisches Passwort nötig)";

        return
            "   ➜ Anbieter nicht eindeutig erkannt. Typische Einstellungen (beim Anbieter prüfen):\n" +
            $"   • Posteingang (IMAP): oft imap.{domain} oder mail.{domain}, Port 993 (SSL/TLS)\n" +
            $"   • Postausgang (SMTP): oft smtp.{domain} oder mail.{domain}, Port 587 (STARTTLS)\n" +
            "   • Benutzername: komplette E-Mail-Adresse\n" +
            "   • Tipp: Unterstützt der Anbieter 'Autodiscover', findet Outlook die Werte automatisch (siehe Punkt 4).";
    }

    private static string ExtractDomain(string email)
    {
        var parts = (email ?? "").Trim().Split('@');
        return parts.Length == 2 ? parts[1].Trim() : "";
    }

    // ---- Prozess-Infrastruktur ----------------------------------------

    private Task<string> RunPowerShellAsync(string command) =>
        RunProcessAsync("powershell.exe",
            $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"");

    private static Task<string> RunProcessAsync(string fileName, string arguments)
    {
        var tcs = new TaskCompletionSource<string>();
        var sb = new StringBuilder();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            },
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, e) => { if (e.Data != null) sb.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) sb.AppendLine(e.Data); };
        process.Exited += (_, _) => { tcs.TrySetResult(sb.ToString()); process.Dispose(); };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
        catch (Exception ex) { tcs.TrySetResult("FEHLER beim Start: " + ex.Message); }
        return tcs.Task;
    }

    private static string Esc(string input) => (input ?? "").Replace("'", "''").Trim();
}