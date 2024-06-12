namespace QarnotCLI;

public record SetConfigModel(
    bool Local,
    bool Show
) : GlobalModel;

public record ShowConfigModel(
    bool Global,
    bool WithoutEnv
) : GlobalModel;