namespace QarnotCLI;

public record GetSecretModel(
    string Key
) : GlobalModel;

public record WriteSecretModel(
    string Key,
    string Value
): GlobalModel;

public record ListSecretsModel(
    string Prefix,
    bool Recursive
) : GlobalModel;
