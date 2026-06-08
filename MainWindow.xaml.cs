using System.Windows;
using System.Windows.Controls;
using NetDashboard.Services;

namespace NetDashboard;

public partial class MainWindow : Window
{
    private readonly NetworkService _net = new();

    public MainWindow()
    {
        InitializeComponent();
        LangSelector.SelectedIndex = 0; // Deutsch -> löst ApplyLanguage aus
    }

    // ---- Sprache ------------------------------------------------------

    private void LangSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LangSelector.SelectedItem is ComboBoxItem item && item.Tag is string lang)
            ApplyLanguage(lang);
    }

    private void ApplyLanguage(string lang)
    {
        var t = lang switch { "en" => En, "fr" => Fr, _ => De };

        Subtitle.Text = t["subtitle"];
        ResolverTitle.Text = t["resolverTitle"];
        ResolverHint.Text = t["resolverHint"];

        TabMail.Header = t["tabMail"];
        TabDns.Header = t["tabDns"];
        TabConn.Header = t["tabConn"];
        TabSys.Header = t["tabSys"];
        TabHelp.Header = t["tabHelp"];

        MailCardTitle.Text = t["mailTitle"];
        MailCardHint.Text = t["mailHint"];
        MailLabel.Text = t["mailLabel"];
        MailCheckBtn.Content = t["mailBtn"];

        DnsCardTitle.Text = t["dnsTitle"];
        DnsCardHint.Text = t["dnsHint"];
        DnsLabel.Text = t["dnsLabel"];
        BtnA.Content = t["recA"]; BtnAAAA.Content = t["recAAAA"]; BtnMX.Content = t["recMX"];
        BtnTXT.Content = t["recTXT"]; BtnNS.Content = t["recNS"]; BtnCNAME.Content = t["recCNAME"];
        BtnSOA.Content = t["recSOA"]; BtnALL.Content = t["recALL"]; BtnPTR.Content = t["recPTR"];

        ConnCardTitle.Text = t["connTitle"];
        ConnCardHint.Text = t["connHint"];
        ConnTargetLabel.Text = t["connTargetLabel"];
        PortLabel.Text = t["portLabel"];
        BtnPing.Content = t["btnPing"];
        BtnPort.Content = t["btnPort"];
        BtnTrace.Content = t["btnTrace"];

        SysCardTitle.Text = t["sysTitle"];
        SysCardHint.Text = t["sysHint"];
        BtnIpcfg.Content = t["btnIpcfg"]; BtnDnsSrv.Content = t["btnDnsSrv"];
        BtnCacheShow.Content = t["btnCacheShow"]; BtnCacheFlush.Content = t["btnCacheFlush"];
        BtnArp.Content = t["btnArp"]; BtnRoute.Content = t["btnRoute"];
        BtnPublicIp.Content = t["btnPublicIp"];

        foreach (var b in new[] { MailCopy, DnsCopy, ConnCopy, SysCopy }) b.Content = t["copy"];
        foreach (var b in new[] { MailClear, DnsClear, ConnClear, SysClear }) b.Content = t["clear"];

        HelpText.Text = t["help"];
    }

    // ---- Mail-Check ---------------------------------------------------

    private async void MailCheck_Click(object sender, RoutedEventArgs e)
    {
        if (!Require(MailEmailInput, MailOutput, "Bitte E-Mail-Adresse eingeben (z. B. name@firma.ch).")) return;
        await RunAsync(MailOutput, MailBusy, $"Mail-Check → {MailEmailInput.Text.Trim()}",
            () => _net.MailCheckAsync(MailEmailInput.Text.Trim(), SelectedResolvers()));
    }

    // ---- DNS-Check ----------------------------------------------------

    private async void DnsType_Click(object sender, RoutedEventArgs e)
    {
        if (!Require(HostInput, DnsOutput, "Bitte Domain oder IP eingeben.")) return;
        var type = (string)((Button)sender).Tag;
        await RunAsync(DnsOutput, DnsBusy, $"DNS {type} → {HostInput.Text.Trim()}",
            () => _net.DnsLookupMultiAsync(HostInput.Text.Trim(), type, SelectedResolvers()));
    }

    // ---- Verbindung ---------------------------------------------------

    private async void Ping_Click(object sender, RoutedEventArgs e)
    {
        if (!Require(TargetInput, ConnOutput, "Bitte ein Ziel (Domain oder IP) eingeben.")) return;
        await RunAsync(ConnOutput, ConnBusy, $"Ping → {TargetInput.Text.Trim()}",
            () => _net.PingAsync(TargetInput.Text.Trim()));
    }

    private async void PortTest_Click(object sender, RoutedEventArgs e)
    {
        if (!Require(TargetInput, ConnOutput, "Bitte ein Ziel (Domain oder IP) eingeben.")) return;
        if (!TryGetPort(out int port)) return;
        await RunAsync(ConnOutput, ConnBusy, $"Port-Test → {TargetInput.Text.Trim()}:{port}",
            () => _net.PortTestAsync(TargetInput.Text.Trim(), port));
    }

    private async void TraceRoute_Click(object sender, RoutedEventArgs e)
    {
        if (!Require(TargetInput, ConnOutput, "Bitte ein Ziel (Domain oder IP) eingeben.")) return;
        await RunAsync(ConnOutput, ConnBusy, $"Route → {TargetInput.Text.Trim()} (kann dauern…)",
            () => _net.TraceRouteAsync(TargetInput.Text.Trim()));
    }

    // ---- System -------------------------------------------------------

    private async void IpConfig_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(SysOutput, SysBusy, "IP-Konfiguration", () => _net.IpConfigAsync());
    private async void DnsServers_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(SysOutput, SysBusy, "Aktive DNS-Server", () => _net.ShowDnsServersAsync());
    private async void DnsCacheShow_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(SysOutput, SysBusy, "DNS-Cache", () => _net.ShowDnsCacheAsync());
    private async void DnsCacheFlush_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(SysOutput, SysBusy, "DNS-Cache leeren", () => _net.FlushDnsCacheAsync());
    private async void Arp_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(SysOutput, SysBusy, "ARP-Tabelle", () => _net.ArpTableAsync());
    private async void Route_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(SysOutput, SysBusy, "Routing-Tabelle", () => _net.RouteTableAsync());
    private async void PublicIp_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(SysOutput, SysBusy, "Öffentliche IP", () => _net.PublicIpAsync());

    // ---- Kopieren / Leeren --------------------------------------------

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var box = (TextBox)((Button)sender).Tag;
        if (string.IsNullOrEmpty(box.Text)) return;
        try { Clipboard.SetText(box.Text); }
        catch { }
    }

    private void Clear_Click(object sender, RoutedEventArgs e) =>
        ((TextBox)((Button)sender).Tag).Clear();

    // ---- Helfer -------------------------------------------------------

    private IReadOnlyList<(string Name, string Ip)> SelectedResolvers()
    {
        var list = new List<(string, string)>();
        if (ChkGoogle.IsChecked == true) list.Add(("Google", "8.8.8.8"));
        if (ChkCloudflare.IsChecked == true) list.Add(("Cloudflare", "1.1.1.1"));
        if (ChkQuad9.IsChecked == true) list.Add(("Quad9", "9.9.9.9"));
        if (list.Count == 0) list.Add(("System-Standard", ""));
        return list;
    }

    private async Task RunAsync(TextBox output, TextBlock busy, string title, Func<Task<string>> action)
    {
        AppendHeader(output, title);
        busy.Visibility = Visibility.Visible;
        try
        {
            string result = await action();
            Append(output, string.IsNullOrWhiteSpace(result) ? "(keine Ausgabe)" : result.TrimEnd());
        }
        catch (Exception ex) { Append(output, "FEHLER: " + ex.Message); }
        finally { busy.Visibility = Visibility.Collapsed; }
    }

    private void AppendHeader(TextBox output, string title)
    {
        Append(output, $"\n══════════ {title} ══════════");
        Append(output, $"[{DateTime.Now:HH:mm:ss}]");
    }

    private void Append(TextBox output, string text)
    {
        output.AppendText(text + Environment.NewLine);
        output.ScrollToEnd();
    }

    private bool Require(TextBox input, TextBox output, string msg)
    {
        if (string.IsNullOrWhiteSpace(input.Text)) { Append(output, "FEHLER: " + msg); return false; }
        return true;
    }

    private bool TryGetPort(out int port)
    {
        if (!int.TryParse(PortInput.Text.Trim(), out port) || port is < 1 or > 65535)
        {
            Append(ConnOutput, "FEHLER: Bitte einen gültigen Port (1–65535) eingeben.");
            return false;
        }
        return true;
    }

    // ---- Übersetzungen ------------------------------------------------

    private static readonly Dictionary<string, string> De = new()
    {
        ["subtitle"] = "Netzwerk- & Mail-Toolkit  ·  by RayStudio",
        ["resolverTitle"] = "DNS-Resolver für den Vergleich",
        ["resolverHint"] = "Mehrere anhaken = Kontrolle. Fällt ein Dienst aus oder antwortet falsch, erkennst du es durch den Vergleich. Gilt für Mail-Check und DNS-Check.",
        ["tabMail"] = "📧 Mail-Check",
        ["tabDns"] = "🔍 DNS-Check",
        ["tabConn"] = "🔌 Verbindung",
        ["tabSys"] = "💻 Mein System",
        ["tabHelp"] = "ℹ️ Hilfe",
        ["mailTitle"] = "Mail-Einstellungen herausfinden",
        ["mailHint"] = "Du kennst nur deine E-Mail-Adresse, aber nicht die Server-Einstellungen? Adresse eingeben und auf 'Prüfen' klicken – das Tool findet den Mailserver, erkennt den Anbieter und zeigt die passenden IMAP-/SMTP-Werte.",
        ["mailLabel"] = "E-Mail-Adresse",
        ["mailBtn"] = "✓ Prüfen",
        ["dnsTitle"] = "DNS-Einträge prüfen (für Support)",
        ["dnsHint"] = "Domain oder IP eingeben und Abfrage wählen. Läuft über alle oben ausgewählten Resolver – so siehst du sofort, ob alle dasselbe liefern.",
        ["dnsLabel"] = "Domain oder IP-Adresse",
        ["recA"] = "IPv4 (A)",
        ["recAAAA"] = "IPv6 (AAAA)",
        ["recMX"] = "Mailserver (MX)",
        ["recTXT"] = "Text (TXT)",
        ["recNS"] = "Nameserver (NS)",
        ["recCNAME"] = "Alias (CNAME)",
        ["recSOA"] = "Zonen-Info (SOA)",
        ["recALL"] = "Alle Records",
        ["recPTR"] = "Reverse (PTR)",
        ["connTitle"] = "Erreichbarkeit & Ports testen",
        ["connHint"] = "Prüft, ob ein Ziel antwortet (Ping), ob ein Port offen ist (443 = HTTPS, 993 = IMAP, 587 = SMTP) und welchen Weg die Daten nehmen (Route).",
        ["connTargetLabel"] = "Ziel (Domain oder IP)",
        ["portLabel"] = "Port",
        ["btnPing"] = "Ping (Erreichbarkeit)",
        ["btnPort"] = "Port-Test",
        ["btnTrace"] = "Route / Tracert",
        ["sysTitle"] = "Infos über deinen PC & dein Netzwerk",
        ["sysHint"] = "Keine Eingabe nötig – einfach auf eine Schaltfläche klicken.",
        ["btnIpcfg"] = "IP-Konfiguration",
        ["btnDnsSrv"] = "Aktive DNS-Server",
        ["btnCacheShow"] = "DNS-Cache anzeigen",
        ["btnCacheFlush"] = "DNS-Cache leeren",
        ["btnArp"] = "ARP-Tabelle",
        ["btnRoute"] = "Routing-Tabelle",
        ["btnPublicIp"] = "Öffentliche IP",
        ["copy"] = "📋 Kopieren",
        ["clear"] = "🗑 Leeren",
        ["help"] =
            "Warum mehrere DNS-Resolver?\n" +
            "DNS ist das „Telefonbuch\" des Internets. Es gibt mehrere öffentliche Auskünfte (Resolver): Google (8.8.8.8), Cloudflare (1.1.1.1) und Quad9 (9.9.9.9). Das Tool fragt mehrere gleichzeitig und vergleicht.\n" +
            "• Alle gleich → alles in Ordnung.\n" +
            "• Unterschiede → Umstellung läuft, ein Resolver hat ein Problem, oder ein Eintrag ist veraltet.\n" +
            "Fällt ein Resolver aus, siehst du dank der anderen trotzdem ein Ergebnis.\n\n" +
            "MX / SPF kurz erklärt:\n" +
            "MX = die Server, die E-Mails für eine Domain annehmen. SPF = legt fest, welche Server im Namen der Domain senden dürfen (Schutz vor Fälschungen).",
    };

    private static readonly Dictionary<string, string> En = new()
    {
        ["subtitle"] = "Network & Mail Toolkit  ·  by RayStudio",
        ["resolverTitle"] = "DNS resolvers for comparison",
        ["resolverHint"] = "Tick several = cross-check. If one service is down or answers incorrectly, the comparison reveals it. Applies to Mail Check and DNS Check.",
        ["tabMail"] = "📧 Mail Check",
        ["tabDns"] = "🔍 DNS Check",
        ["tabConn"] = "🔌 Connection",
        ["tabSys"] = "💻 My System",
        ["tabHelp"] = "ℹ️ Help",
        ["mailTitle"] = "Find your mail settings",
        ["mailHint"] = "You only know your email address, not the server settings? Enter the address and click 'Check' – the tool finds the mail server, detects the provider and shows the matching IMAP/SMTP values.",
        ["mailLabel"] = "Email address",
        ["mailBtn"] = "✓ Check",
        ["dnsTitle"] = "Check DNS records (for support)",
        ["dnsHint"] = "Enter a domain or IP and pick a query. It runs across all resolvers selected above – so you immediately see whether they all return the same.",
        ["dnsLabel"] = "Domain or IP address",
        ["recA"] = "IPv4 (A)",
        ["recAAAA"] = "IPv6 (AAAA)",
        ["recMX"] = "Mail server (MX)",
        ["recTXT"] = "Text (TXT)",
        ["recNS"] = "Name server (NS)",
        ["recCNAME"] = "Alias (CNAME)",
        ["recSOA"] = "Zone info (SOA)",
        ["recALL"] = "All records",
        ["recPTR"] = "Reverse (PTR)",
        ["connTitle"] = "Test reachability & ports",
        ["connHint"] = "Checks whether a target responds (Ping), whether a port is open (443 = HTTPS, 993 = IMAP, 587 = SMTP) and which path the data takes (Route).",
        ["connTargetLabel"] = "Target (domain or IP)",
        ["portLabel"] = "Port",
        ["btnPing"] = "Ping (reachability)",
        ["btnPort"] = "Port test",
        ["btnTrace"] = "Route / Tracert",
        ["sysTitle"] = "Info about your PC & network",
        ["sysHint"] = "No input needed – just click a button.",
        ["btnIpcfg"] = "IP configuration",
        ["btnDnsSrv"] = "Active DNS servers",
        ["btnCacheShow"] = "Show DNS cache",
        ["btnCacheFlush"] = "Flush DNS cache",
        ["btnArp"] = "ARP table",
        ["btnRoute"] = "Routing table",
        ["btnPublicIp"] = "Public IP",
        ["copy"] = "📋 Copy",
        ["clear"] = "🗑 Clear",
        ["help"] =
            "Why several DNS resolvers?\n" +
            "DNS is the internet's 'phone book'. There are several public resolvers: Google (8.8.8.8), Cloudflare (1.1.1.1) and Quad9 (9.9.9.9). The tool queries several at once and compares.\n" +
            "• All the same → everything is fine.\n" +
            "• Differences → a change is in progress, a resolver has a problem, or an entry is outdated.\n" +
            "If one resolver is down, you still get a result thanks to the others.\n\n" +
            "MX / SPF in short:\n" +
            "MX = the servers that accept email for a domain. SPF = defines which servers may send on behalf of the domain (protection against spoofing).",
    };

    private static readonly Dictionary<string, string> Fr = new()
    {
        ["subtitle"] = "Boîte à outils réseau & mail  ·  by RayStudio",
        ["resolverTitle"] = "Résolveurs DNS pour la comparaison",
        ["resolverHint"] = "Cochez-en plusieurs = contrôle. Si un service tombe en panne ou répond mal, la comparaison le révèle. Vaut pour Vérif. mail et Vérif. DNS.",
        ["tabMail"] = "📧 Vérif. mail",
        ["tabDns"] = "🔍 Vérif. DNS",
        ["tabConn"] = "🔌 Connexion",
        ["tabSys"] = "💻 Mon système",
        ["tabHelp"] = "ℹ️ Aide",
        ["mailTitle"] = "Trouver les paramètres de messagerie",
        ["mailHint"] = "Vous connaissez seulement votre adresse e-mail, pas les paramètres du serveur ? Saisissez l'adresse et cliquez sur « Vérifier » – l'outil trouve le serveur de messagerie, détecte le fournisseur et affiche les valeurs IMAP/SMTP adaptées.",
        ["mailLabel"] = "Adresse e-mail",
        ["mailBtn"] = "✓ Vérifier",
        ["dnsTitle"] = "Vérifier les enregistrements DNS (pour le support)",
        ["dnsHint"] = "Saisissez un domaine ou une IP et choisissez une requête. Elle s'exécute sur tous les résolveurs sélectionnés ci-dessus – vous voyez immédiatement s'ils renvoient tous la même chose.",
        ["dnsLabel"] = "Domaine ou adresse IP",
        ["recA"] = "IPv4 (A)",
        ["recAAAA"] = "IPv6 (AAAA)",
        ["recMX"] = "Serveur mail (MX)",
        ["recTXT"] = "Texte (TXT)",
        ["recNS"] = "Serveur de noms (NS)",
        ["recCNAME"] = "Alias (CNAME)",
        ["recSOA"] = "Info de zone (SOA)",
        ["recALL"] = "Tous les enregistr.",
        ["recPTR"] = "Inverse (PTR)",
        ["connTitle"] = "Tester l'accessibilité & les ports",
        ["connHint"] = "Vérifie si une cible répond (Ping), si un port est ouvert (443 = HTTPS, 993 = IMAP, 587 = SMTP) et quel chemin prennent les données (Route).",
        ["connTargetLabel"] = "Cible (domaine ou IP)",
        ["portLabel"] = "Port",
        ["btnPing"] = "Ping (accessibilité)",
        ["btnPort"] = "Test de port",
        ["btnTrace"] = "Route / Tracert",
        ["sysTitle"] = "Infos sur votre PC & réseau",
        ["sysHint"] = "Aucune saisie nécessaire – cliquez simplement sur un bouton.",
        ["btnIpcfg"] = "Configuration IP",
        ["btnDnsSrv"] = "Serveurs DNS actifs",
        ["btnCacheShow"] = "Afficher le cache DNS",
        ["btnCacheFlush"] = "Vider le cache DNS",
        ["btnArp"] = "Table ARP",
        ["btnRoute"] = "Table de routage",
        ["btnPublicIp"] = "IP publique",
        ["copy"] = "📋 Copier",
        ["clear"] = "🗑 Vider",
        ["help"] =
            "Pourquoi plusieurs résolveurs DNS ?\n" +
            "Le DNS est « l'annuaire » d'Internet. Il existe plusieurs résolveurs publics : Google (8.8.8.8), Cloudflare (1.1.1.1) et Quad9 (9.9.9.9). L'outil en interroge plusieurs en même temps et compare.\n" +
            "• Tous identiques → tout va bien.\n" +
            "• Différences → un changement est en cours, un résolveur a un problème, ou une entrée est obsolète.\n" +
            "Si un résolveur est en panne, vous obtenez quand même un résultat grâce aux autres.\n\n" +
            "MX / SPF en bref :\n" +
            "MX = les serveurs qui acceptent les e-mails d'un domaine. SPF = définit quels serveurs peuvent envoyer au nom du domaine (protection contre l'usurpation).",
    };
}