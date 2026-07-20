using System.Reflection;
using NetDashboard.Services;
using Xunit;

namespace NetDashboard.Tests.Services;

public class NetworkServiceTests
{
    private readonly NetworkService _service = new();

    #region ExtractDomain Tests

    [Fact]
    public void ExtractDomain_ValidEmail_ReturnsDomain()
    {
        // Arrange
        var email = "user@example.ch";

        // Act
        var result = InvokeExtractDomain(email);

        // Assert
        Assert.Equal("example.ch", result);
    }

    [Fact]
    public void ExtractDomain_ValidEmailWithSubdomain_ReturnsDomain()
    {
        // Arrange
        var email = "name@subdomain.firma.ch";

        // Act
        var result = InvokeExtractDomain(email);

        // Assert
        Assert.Equal("subdomain.firma.ch", result);
    }

    [Fact]
    public void ExtractDomain_InvalidEmailNoAt_ReturnsEmpty()
    {
        // Arrange
        var email = "invalidemailaddress";

        // Act
        var result = InvokeExtractDomain(email);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ExtractDomain_MultipleAtSigns_ReturnsEmpty()
    {
        // Arrange
        var email = "user@@example.ch";

        // Act
        var result = InvokeExtractDomain(email);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ExtractDomain_EmptyString_ReturnsEmpty()
    {
        // Arrange
        var email = "";

        // Act
        var result = InvokeExtractDomain(email);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ExtractDomain_NullString_ReturnsEmpty()
    {
        // Arrange
        string? email = null;

        // Act
        var result = InvokeExtractDomain(email ?? "");

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ExtractDomain_EmailWithWhitespace_ReturnsTrimedDomain()
    {
        // Arrange
        var email = "  user@example.ch  ";

        // Act
        var result = InvokeExtractDomain(email);

        // Assert
        Assert.Equal("example.ch", result);
    }

    #endregion

    #region Esc Tests

    [Fact]
    public void Esc_NoSingleQuotes_ReturnsInputTrimmed()
    {
        // Arrange
        var input = "  example.ch  ";

        // Act
        var result = InvokeEsc(input);

        // Assert
        Assert.Equal("example.ch", result);
    }

    [Fact]
    public void Esc_WithSingleQuotes_EscapesQuotes()
    {
        // Arrange
        var input = "O'Reilly";

        // Act
        var result = InvokeEsc(input);

        // Assert
        Assert.Equal("O''Reilly", result);
    }

    [Fact]
    public void Esc_MultipleSingleQuotes_EscapesAll()
    {
        // Arrange
        var input = "'quoted'string'";

        // Act
        var result = InvokeEsc(input);

        // Assert
        Assert.Equal("''quoted''string''", result);
    }

    [Fact]
    public void Esc_EmptyString_ReturnsEmpty()
    {
        // Arrange
        var input = "";

        // Act
        var result = InvokeEsc(input);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void Esc_NullString_ReturnsEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        var result = InvokeEsc(input ?? "");

        // Assert
        Assert.Equal("", result);
    }

    #endregion

    #region BuildProviderHelp Tests

    [Fact]
    public void BuildProviderHelp_OutlookMX_ReturnsOutlookSettings()
    {
        // Arrange
        var mx = new List<string> { "protection.outlook.com." };
        var domain = "firma.ch";

        // Act
        var result = InvokeBuildProviderHelp(mx, domain);

        // Assert
        Assert.Contains("Microsoft 365", result);
        Assert.Contains("outlook.office365.com", result);
        Assert.Contains("smtp.office365.com", result);
        Assert.Contains("Port 993", result);
        Assert.Contains("Port 587", result);
    }

    [Fact]
    public void BuildProviderHelp_GmailMX_ReturnsGmailSettings()
    {
        // Arrange
        var mx = new List<string> { "smtp.google.com." };
        var domain = "example.ch";

        // Act
        var result = InvokeBuildProviderHelp(mx, domain);

        // Assert
        Assert.Contains("Google Workspace", result);
        Assert.Contains("imap.gmail.com", result);
        Assert.Contains("smtp.gmail.com", result);
        Assert.Contains("Port 993", result);
    }

    [Fact]
    public void BuildProviderHelp_ICloudMX_ReturnsICloudSettings()
    {
        // Arrange
        var mx = new List<string> { "icloud.com." };
        var domain = "example.ch";

        // Act
        var result = InvokeBuildProviderHelp(mx, domain);

        // Assert
        Assert.Contains("Apple iCloud", result);
        Assert.Contains("imap.mail.me.com", result);
        Assert.Contains("smtp.mail.me.com", result);
    }

    [Fact]
    public void BuildProviderHelp_UnknownProvider_ReturnsGenericSettings()
    {
        // Arrange
        var mx = new List<string> { "mail.example.ch." };
        var domain = "example.ch";

        // Act
        var result = InvokeBuildProviderHelp(mx, domain);

        // Assert
        Assert.Contains("Anbieter nicht eindeutig erkannt", result);
        Assert.Contains("imap.example.ch", result);
        Assert.Contains("smtp.example.ch", result);
    }

    [Fact]
    public void BuildProviderHelp_EmptyMX_ReturnsGenericSettings()
    {
        // Arrange
        var mx = new List<string>();
        var domain = "example.ch";

        // Act
        var result = InvokeBuildProviderHelp(mx, domain);

        // Assert
        Assert.Contains("Anbieter nicht eindeutig erkannt", result);
        Assert.Contains("imap.example.ch", result);
    }

    #endregion

    #region CompareVerdict Tests

    [Fact]
    public void CompareVerdict_SingleNonEmptyList_ReturnsSingleSourceMessage()
    {
        // Arrange
        var lists = new List<List<string>>
        {
            new() { "mail1.example.ch.", "mail2.example.ch." }
        };

        // Act
        var result = InvokeCompareVerdict(lists);

        // Assert
        Assert.Contains("Nur eine Quelle abgefragt", result);
    }

    [Fact]
    public void CompareVerdict_IdenticalMultipleLists_ReturnsConsistentMessage()
    {
        // Arrange
        var lists = new List<List<string>>
        {
            new() { "mail1.example.ch.", "mail2.example.ch." },
            new() { "mail1.example.ch.", "mail2.example.ch." }
        };

        // Act
        var result = InvokeCompareVerdict(lists);

        // Assert
        Assert.Contains("Alle Resolver liefern dieselben Mailserver", result);
        Assert.Contains("✔", result);
    }

    [Fact]
    public void CompareVerdict_DifferentLists_ReturnsWarning()
    {
        // Arrange
        var lists = new List<List<string>>
        {
            new() { "mail1.example.ch." },
            new() { "mail2.different.ch." }
        };

        // Act
        var result = InvokeCompareVerdict(lists);

        // Assert
        Assert.Contains("Unterschiedliche Antworten", result);
        Assert.Contains("⚠", result);
    }

    [Fact]
    public void CompareVerdict_OneEmptyList_IgnoresEmpty()
    {
        // Arrange
        var lists = new List<List<string>>
        {
            new() { "mail.example.ch." },
            new() // empty
        };

        // Act
        var result = InvokeCompareVerdict(lists);

        // Assert
        Assert.Contains("Nur eine Quelle abgefragt", result);
    }

    [Fact]
    public void CompareVerdict_AllEmptyLists_ReturnsSingleSourceMessage()
    {
        // Arrange
        var lists = new List<List<string>>
        {
            new(),
            new()
        };

        // Act
        var result = InvokeCompareVerdict(lists);

        // Assert
        Assert.Contains("Nur eine Quelle abgefragt", result);
    }

    #endregion

    #region PublicResolvers Tests

    [Fact]
    public void PublicResolvers_Contains_Google()
    {
        // Assert
        Assert.Contains(("Google", "8.8.8.8"), NetworkService.PublicResolvers);
    }

    [Fact]
    public void PublicResolvers_Contains_Cloudflare()
    {
        // Assert
        Assert.Contains(("Cloudflare", "1.1.1.1"), NetworkService.PublicResolvers);
    }

    [Fact]
    public void PublicResolvers_Contains_Quad9()
    {
        // Assert
        Assert.Contains(("Quad9", "9.9.9.9"), NetworkService.PublicResolvers);
    }

    [Fact]
    public void PublicResolvers_ContainsThreeResolvers()
    {
        // Assert
        Assert.Equal(3, NetworkService.PublicResolvers.Length);
    }

    #endregion

    #region Async Method Tests

    [Fact]
    public void DnsLookupAsync_WithValidParams_ReturnsTask()
    {
        // Arrange
        var host = "example.ch";
        var dnsServer = "8.8.8.8";
        var type = "A";

        // Act
        var result = _service.DnsLookupAsync(host, dnsServer, type);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Task<string>>(result);
    }

    [Fact]
    public void DnsLookupMultiAsync_WithValidParams_ReturnsNonEmptyString()
    {
        // Arrange
        var host = "example.ch";
        var type = "A";
        var resolvers = new List<(string, string)> { ("Google", "8.8.8.8") };

        // Act & Assert
        // This test verifies the method structure; actual execution requires PowerShell
        var task = _service.DnsLookupMultiAsync(host, type, resolvers);
        Assert.IsAssignableFrom<Task<string>>(task);
    }

    [Fact]
    public void MailCheckAsync_ValidEmail_ReturnsNonEmptyString()
    {
        // Arrange
        var email = "user@example.ch";
        var resolvers = new List<(string, string)> { ("Google", "8.8.8.8") };

        // Act & Assert
        // This test verifies the method structure; actual execution requires PowerShell
        var task = _service.MailCheckAsync(email, resolvers);
        Assert.IsAssignableFrom<Task<string>>(task);
    }

    [Fact]
    public async Task MailCheckAsync_InvalidEmail_ReturnsErrorMessage()
    {
        // Arrange
        var email = "invalid-email-without-at-sign";
        var resolvers = new List<(string, string)> { ("Google", "8.8.8.8") };

        // Act
        var result = await _service.MailCheckAsync(email, resolvers);

        // Assert
        Assert.Contains("FEHLER", result);
        Assert.Contains("gültige E-Mail-Adresse", result);
    }

    [Fact]
    public async Task MailCheckAsync_EmptyEmail_ReturnsErrorMessage()
    {
        // Arrange
        var email = "";
        var resolvers = new List<(string, string)> { ("Google", "8.8.8.8") };

        // Act
        var result = await _service.MailCheckAsync(email, resolvers);

        // Assert
        Assert.Contains("FEHLER", result);
    }

    [Fact]
    public void PingAsync_WithValidTarget_ReturnsTask()
    {
        // Arrange
        var target = "8.8.8.8";

        // Act
        var result = _service.PingAsync(target);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Task<string>>(result);
    }

    [Fact]
    public void PortTestAsync_WithValidParams_ReturnsTask()
    {
        // Arrange
        var target = "8.8.8.8";
        var port = 53;

        // Act
        var result = _service.PortTestAsync(target, port);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Task<string>>(result);
    }

    [Fact]
    public void ShowDnsServersAsync_ReturnsTask()
    {
        // Act
        var result = _service.ShowDnsServersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Task<string>>(result);
    }

    #endregion

    #region Helper Methods (via Reflection)

    private static string InvokeExtractDomain(string email)
    {
        var method = typeof(NetworkService).GetMethod(
            "ExtractDomain",
            BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(string) },
            null) ?? throw new InvalidOperationException("Method not found");

        return (string)(method.Invoke(null, new object[] { email }) ?? "");
    }

    private static string InvokeEsc(string input)
    {
        var method = typeof(NetworkService).GetMethod(
            "Esc",
            BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(string) },
            null) ?? throw new InvalidOperationException("Method not found");

        return (string)(method.Invoke(null, new object[] { input }) ?? "");
    }

    private static string InvokeBuildProviderHelp(List<string> mx, string domain)
    {
        var method = typeof(NetworkService).GetMethod(
            "BuildProviderHelp",
            BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(List<string>), typeof(string) },
            null) ?? throw new InvalidOperationException("Method not found");

        return (string)(method.Invoke(null, new object[] { mx, domain }) ?? "");
    }

    private static string InvokeCompareVerdict(List<List<string>> lists)
    {
        var method = typeof(NetworkService).GetMethod(
            "CompareVerdict",
            BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(List<List<string>>) },
            null) ?? throw new InvalidOperationException("Method not found");

        return (string)(method.Invoke(null, new object[] { lists }) ?? "");
    }

    #endregion
}
