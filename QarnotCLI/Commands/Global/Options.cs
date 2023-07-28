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
        TokenOpt = new Option<string>(
            name: "--token",
            description: "Connection API token"
        );
        UnsafeSslOpt = new Option<bool?>(
            name: "--unsafe-ssl",
            description: "Don't check the ssl certificate"
        );
        CustomSslCertificateOpt = new Option<string>(
            name: "--api-ca-certificate",
            description: "Path to your custom SSL certificate"
        );
        VerboseOpt = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Set the max verbose messages"
        );
        QuietOpt = new Option<bool>(
            aliases: new[] { "--quiet", "-q" },
            description: "Set no verbose messages"
        );
        NoColorOpt = new Option<bool>(
            name: "--no-color",
            description: "Remove the color on the console"
        );
        FormatOpt = new Option<string>(
            name: "--format",
            description: "Change the result format (one of TABLE (default) or JSON)"
        ).FromAmong("TABLE", "JSON");
        HumanReadableOpt = new Option<bool>(
            aliases: new[] { "-h", "--human-readable" },
            description: "Print sizes in human readable format (e.g. 1K, 234M, etc.)"
        );

        ConnectionConfiguration = connectionConfiguration;
    }
}

public static class GlobalOptionsExtension
{
    public static Command AddGlobalOptions(this Command cmd, GlobalOptions options)
    {
        cmd.AddGlobalOption(options.TokenOpt);
        cmd.AddGlobalOption(options.UnsafeSslOpt);
        cmd.AddGlobalOption(options.CustomSslCertificateOpt);
        cmd.AddGlobalOption(options.VerboseOpt);
        cmd.AddGlobalOption(options.QuietOpt);
        cmd.AddGlobalOption(options.NoColorOpt);
        cmd.AddGlobalOption(options.FormatOpt);
        cmd.AddGlobalOption(options.HumanReadableOpt);

        return cmd;
    }
}
