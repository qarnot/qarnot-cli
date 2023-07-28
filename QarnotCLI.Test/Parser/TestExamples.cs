using NUnit.Framework;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace QarnotCLI.Test;

[TestFixture]
public class TestExamples
{
    [Test]
    public async Task RunExamples()
    {
        var mock = new MockParser();
        var rootCommand = mock.Parser.Configuration.RootCommand;

        await RunAllExamples(rootCommand, mock);
    }

    private async Task RunAllExamples(Command cmd, MockParser mock)
    {
        if (cmd is CommandWithExamples withExamples)
        {
            foreach (var example in withExamples.Examples)
            {
                TestContext.Progress.WriteLine($"Trying to parse example: {example.Title}");
                foreach (var cmdline in example.CommandLines)
                {
                    var stripped = cmdline.Substring("qarnot ".Length);
                    var res = await mock.Parser.InvokeAsync(stripped);

                    if (example.IgnoreTest)
                    {
                        continue;
                    }
                    else if (example.IsError)
                    {
                        Assert.That(res, Is.Not.EqualTo(0), $"This example should have failed: {example.Title}: {stripped}");
                    }
                    else
                    {
                        Assert.That(res, Is.EqualTo(0), $"Unable to parse example: {example.Title}: {stripped}");
                    }
                }
            }
        }

        foreach (var child in cmd.Children.OfType<Command>())
        {
            await RunAllExamples(child, mock);
        }
    }
}
