using System.CommandLine;

namespace QarnotCLI;

public class GlobalOptions
{
    public Option<string> TokenOpt { get; }
    public Option<bool?> UnsafeSslOpt { get; }
    public Option<string> CustomSslCertificateOpt { get; }
    public Option<bool> VerboseOpt { get; }
    public Option<bool> QuietOpt { get; }
    public Option<bool> NoColorOpt { get; }
    public Option<string> FormatOpt { get; }
    public Option<bool> HumanReadableOpt { get; }
    public ConnectionConfiguration ConnectionConfiguration { get; }

    public GlobalOptions(ConnectionConfiguration connectionConfiguration)
    {
        TokenOpt = new Option<string>("--token")
        {
            Description = "Connection API token",
        };
        UnsafeSslOpt = new Option<bool?>("--unsafe-ssl")
        {
            Description = "Don't check the ssl certificate",
        };
        CustomSslCertificateOpt = new Option<string>("--api-ca-certificate")
        {
            Description = "Path to your custom SSL certificate",
        };
        VerboseOpt = new Option<bool>("--verbose", "-v")
        {
            Description = "Set the max verbose messages",
        };
        QuietOpt = new Option<bool>("--quiet", "-q")
        {
            Description = "Set no verbose messages",
        };
        NoColorOpt = new Option<bool>("--no-color")
        {
            Description = "Remove the color on the console",
        };
        FormatOpt = new Option<string>("--format")
        {
            Description = "Change the result format (one of TABLE (default) or JSON)",
        };
        FormatOpt.AcceptOnlyFromAmong("TABLE", "JSON");
        HumanReadableOpt = new Option<bool>("-h", "--human-readable")
        {
            Description = "Print sizes in human readable format (e.g. 1K, 234M, etc.)",
        };

        ConnectionConfiguration = connectionConfiguration;
    }
}

public static class GlobalOptionsExtension
{
    public static Command AddGlobalOptions(this Command cmd, GlobalOptions options)
    {
        AddAsGlobal(cmd, options.TokenOpt);
        AddAsGlobal(cmd, options.UnsafeSslOpt);
        AddAsGlobal(cmd, options.CustomSslCertificateOpt);
        AddAsGlobal(cmd, options.VerboseOpt);
        AddAsGlobal(cmd, options.QuietOpt);
        AddAsGlobal(cmd, options.NoColorOpt);
        AddAsGlobal(cmd, options.FormatOpt);
        AddAsGlobal(cmd, options.HumanReadableOpt);

        return cmd;
    }

    private static void AddAsGlobal(Command cmd, Option option)
    {
        option.Recursive = true;
        cmd.Add(option);
    }
}
