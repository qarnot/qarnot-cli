namespace QarnotCLI
{
    /// <summary>
    /// Api connection information objects
    /// Have a TOken, An Api url and au storage Url.
    /// </summary>
    public class APIConnectionInformation
    {
        private string token;
        private string apiUri;
        private string storageUri;

        public string Token
        {
            get { return string.IsNullOrWhiteSpace(token) ? null : token; }
            set { token = value; }
        }

        public string ApiUri
        {
            get { return string.IsNullOrWhiteSpace(apiUri) ? null : apiUri; }
            set { apiUri = value; }
        }

        public string StorageUri
        {
            get { return string.IsNullOrWhiteSpace(storageUri) ? null : storageUri; }
            set { storageUri = value; }
        }

        public string SetToken
        {
            set { token = string.IsNullOrWhiteSpace(token) ? value : token; }
        }

        public string SetApiUri
        {
            set { apiUri = string.IsNullOrWhiteSpace(apiUri) ? value : apiUri; }
        }

        public string SetStorageUri
        {
            set { storageUri = string.IsNullOrWhiteSpace(storageUri) ? value : storageUri; }
        }

        public bool IsComplete()
        {
            return !(string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(apiUri) ||
                string.IsNullOrWhiteSpace(storageUri));
        }
    }
}