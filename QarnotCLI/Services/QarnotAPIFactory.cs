using QarnotSDK;

namespace QarnotCLI;

public interface IQarnotAPIFactory
{
    public Connection Create(GlobalModel model);
}

public class QarnotAPIFactory : IQarnotAPIFactory
{
    public Connection Create(GlobalModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Token))
        {
            throw new Exception("There should be a token");
        }

        HttpClientHandler? handler = null;
        if (model.UnsafeSsl)
        {
            handler = new UnsafeClientHandler();
        }
        else if (!string.IsNullOrWhiteSpace(model.CustomSslCertificat))
        {
            handler = new CustomCAClientHandler(model.CustomSslCertificat);
        }

        return new Connection(
            uri: string.IsNullOrWhiteSpace(model.ApiUri) ? "https://api.qarnot.com" : model.ApiUri,
            storageUri: string.IsNullOrWhiteSpace(model.StorageUri) ? null : model.StorageUri,
            token: model.Token,
            httpClientHandler: handler,
            forceStoragePathStyle: model.ForcePathStyle,
            sanitizeBucketPaths: !model.DisableBucketPathsSanitization,
            showBucketWarnings: false
        )
        {
            StorageAccessKey = model.AccountEmail,
            S3HttpClientFactory = model.StorageUnsafeSsl ? new UnsafeS3HttpClientFactory() : null,
        };
    }
}
