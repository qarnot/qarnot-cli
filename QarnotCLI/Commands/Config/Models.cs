namespace QarnotCLI;

public record RunConfigModel(
    bool Global,
    bool Show
) : GlobalModel;
