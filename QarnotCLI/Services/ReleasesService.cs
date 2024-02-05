using Octokit;
using System.Reflection;

namespace QarnotCLI;

public interface IReleasesService
{
    Task<bool> IsActualReleaseDeprecated();
    Task<bool> DoesANewReleaseExists();
    AssemblyDetails GetAssemblyDetails();
}

public record AssemblyDetails(
    string CommitHash,
    string AssemblyName,
    Version AssemblyVersion,
    string FrameworkName,
    string BuildDate
)
{
    public override string ToString()
    {
        string frameWorkBuilder = FrameworkName.Replace(" ", String.Empty);
        // Remove milliseconds from the timestamp
        var date = BuildDate.Substring(0, BuildDate.Length - 5) + "Z";

        return String.Format("{0}-{1}-{2}-{3}-{4}", AssemblyName, AssemblyVersion, CommitHash, date, frameWorkBuilder);
    }
};

public class ReleasesService : IReleasesService
{
    private const string QarnotGithubOwner = "qarnot";
    private const string QarnotCliRepository = "qarnot-cli";
    private readonly GitHubClient Client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleasesService"/> class.
    /// </summary>
    public ReleasesService()
    {
        Client = new GitHubClient(new ProductHeaderValue($"{QarnotCliRepository}-{GetAssemblyVersion()}"));
    }

    private async Task<IReadOnlyList<Release>> GetAllReleases()
    {
        return await Client.Repository.Release.GetAll(QarnotGithubOwner, QarnotCliRepository);
    }

    private async Task<IEnumerable<Version>> GetPublicReleases()
    {
        return (await GetAllReleases())
        .Where(release =>
        {
            if (release.Draft || release.Prerelease)
            {
                return false;
            }

            return true;
        })
        .Select(release => new Version(release.TagName.TrimStart('v'))); // 'v1.15.0' is invalid format. Expects 'major.minor.patch' without 'v'
    }

    public AssemblyDetails GetAssemblyDetails() =>
        new(
            CommitHash: ThisAssembly.Git.Commit,
            AssemblyName: Assembly.GetEntryAssembly()!.GetName().Name!,
            AssemblyVersion: GetAssemblyVersion(),
            FrameworkName: System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            BuildDate: (string?)Assembly.GetExecutingAssembly()
                .GetCustomAttributesData()
                .FirstOrDefault(x => x.AttributeType.Name == "TimestampAttribute")?
                .ConstructorArguments
                .First()
                .Value ?? new DateTime().ToString()
        );

    private Version GetAssemblyVersion() =>
        new Version($"{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}");

    private async Task<Version> GetNewestPublicReleaseVersion()
    {
        var publicReleases = (await GetPublicReleases()).ToList();
        publicReleases.Sort();
        publicReleases.Reverse();
        return publicReleases.FirstOrDefault()!;
    }

    private async Task<Version> GetOldestPublicReleaseVersion()
    {
        var publicReleases = (await GetPublicReleases()).ToList();
        publicReleases.Sort();
        return publicReleases.FirstOrDefault()!;
    }

    /// <summary>
    /// Check if the current release is deprecated.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<bool> IsActualReleaseDeprecated()
    {
        var actualVersion = GetAssemblyVersion();
        var oldestVersion = await GetOldestPublicReleaseVersion();

        // no release
        if (oldestVersion == default)
        {
            return true;
        }

        // actual release version is lower than oldest supported
        if (actualVersion.CompareTo(oldestVersion) < 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if a new version has been released.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<bool> DoesANewReleaseExists()
    {
        var actualVersion = GetAssemblyVersion();
        var newestVersion = await GetNewestPublicReleaseVersion();

        if (actualVersion.CompareTo(newestVersion) < 0)
        {
            return true;
        }

        return false;
    }
}

public static class DeprecationDisclaimer
{
    public static bool ShouldIgnoreDeprecation =>
        Environment.GetEnvironmentVariable("QARNOT_IGNORE_DEPRECATION") is string s && bool.TryParse(s, out var b)
            ? b
            : false;

    public static async Task Display(IReleasesService releasesService, ILogger logger)
    {
        try
        {
            var isDeprecated = await releasesService.IsActualReleaseDeprecated();
            if (isDeprecated)
            {
                logger.Warning("This release version has been deprecated! Please update as soon as possible as we don't support this version anymore.");
            }

            var newReleaseExists = await releasesService.DoesANewReleaseExists();
            if (newReleaseExists)
            {
                logger.Warning("A release version exists! please upgrade the qarnot CLI to its last version to enjoy the last features.");
            }
        }
        catch (Exception)
        {
        }
    }
}
