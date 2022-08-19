namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class ReleasesHandler
    {
        private const string QarnotGithubOwner = "qarnot";
        private const string QarnotCliRepository = "qarnot-cli";
        private readonly GitHubClient Client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReleasesHandler"/> class.
        /// </summary>
        public ReleasesHandler()
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

        private Version GetAssemblyVersion()
        {
            return new Version(GetType().Assembly.GetName().Version.ToString());
        }

        private async Task<Version> GetNewestPublicReleaseVersion()
        {
            var publicReleases = (await GetPublicReleases()).ToList();
            publicReleases.Sort();
            publicReleases.Reverse();
            return publicReleases.FirstOrDefault();
        }

        private async Task<Version> GetOldestPublicReleaseVersion()
        {
            var publicReleases = (await GetPublicReleases()).ToList();
            publicReleases.Sort();
            return publicReleases.FirstOrDefault();
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
}
