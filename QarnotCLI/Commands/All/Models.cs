namespace QarnotCLI;

public record AllModel(
    bool Delete,
    bool List,
    bool Abort
) : GlobalModel;
