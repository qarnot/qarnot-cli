using System.CommandLine;

namespace QarnotCLI;

public readonly record struct Example(
    string Title,
    string[] CommandLines,
    bool IsError = false,
    bool IgnoreTest = false
);

public class CommandWithExamples : Command
{
    private readonly List<Example> _examples = new();
    public IReadOnlyList<Example> Examples { get => _examples; }

    public CommandWithExamples(string name, string description)
        : base(name, description)
    {
    }

    public void Add(Example example)
    {
        AddExample(example);
    }

    public void AddExample(Example example)
    {
        _examples.Add(example);
    }

    public void Add(IEnumerable<Example> examples)
    {
        AddExamples(examples);
    }

    public void AddExamples(IEnumerable<Example> examples)
    {
        _examples.AddRange(examples);
    }
}
