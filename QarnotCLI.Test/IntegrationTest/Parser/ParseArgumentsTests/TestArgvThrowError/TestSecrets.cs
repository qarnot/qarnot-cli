using NUnit.Framework;

namespace QarnotCLI.Test;

[TestFixture]
public class TestSecretsMissingArguments
{
    [SetUp]
    public void Init()
    {
        CLILogsTest.SurchargeLogs();
    }

    [TestCase("secrets", "create", TestName = "Create requires key and value")]
    [TestCase("secrets", "create", "key", TestName = "Create requires value")]
    [TestCase("secrets", "get", TestName = "Get requires value")]
    [TestCase("secrets", "delete", TestName = "Delete requires key")]
    [TestCase("secrets", "update", TestName = "Update requires key and value")]
    [TestCase("secrets", "update", "key", TestName = "Update requires value")]
    public void SecretsCommandRequiredArguments(params string[] argv)
    {
        var parser = new CommandLineParser(
            new OptionConverter(new JsonDeserializer()),
            new CommandLine.Parser(),
            new ParserUsage(),
            new VerbFormater()
        );

        var ex = Assert.Throws<ParseException>(() => parser.Parse(argv));
        Assert.That(ex, Is.Not.Null);
    }
}
