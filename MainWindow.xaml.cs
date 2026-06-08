using System.Windows;
using System.Windows.Controls;
using NetDashboard.Services;

namespace NetDashboard;

public partial class MainWindow : Window
{
    private readonly NetworkService _net = new();

    public MainWindow() => InitializeComponent();

    // ---- Mail-Check ---------------------------------------------------

    private async void MailCheck_Click(object sender, RoutedEventArgs e)
    {
        if (!Require(MailEmailInput, MailOutput, "Bitte E-Mail-Adresse eingeben (z. B. name@firma.ch).")) return;
        await RunAsync(MailOutput, MailBusy, $"Mail-Check → {MailEmailInput.Text.Trim()}",
            () => _net.MailCheckAsync(MailEmailInput.Text.Trim(), SelectedResolvers()));
    }

    // ---- DNS-Check (Multi-Resolver) -----------------------------------

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

    // ---- Kopieren / Leeren (generisch über Button-Tag) ----------------

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var box = (TextBox)((Button)sender).Tag;
        if (string.IsNullOrEmpty(box.Text)) return;
        try { Clipboard.SetText(box.Text); }
        catch { /* Zwischenablage kurz belegt – ignorieren */ }
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
}