namespace QarnotCLI
{
    using System;

    /// <summary>
    /// Api connection information objects
    /// Have a TOken, An Api url and au storage Url.
    /// </summary>
    public class APIConnectionInformation
    {
        private string token;
        private string apiUri;
        private string storageUri;
        private string accountEmail;
        private bool? forcePathStyle;

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

        public string AccountEmail
        {
            get { return string.IsNullOrWhiteSpace(accountEmail) ? null : accountEmail; }
            set { accountEmail = value; }
        }

        public bool ForcePathStyle
        {
            get { return forcePathStyle == null ? false : forcePathStyle.Value; }
            set { forcePathStyle = value; }
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

        public string SetAccountEmail
        {
            set { accountEmail = string.IsNullOrWhiteSpace(accountEmail) ? value : accountEmail; }
        }

        public bool? SetForcePathStyle
        {
            set { forcePathStyle = forcePathStyle.HasValue ? forcePathStyle : value; }
        }

        public bool? GetForcePathStyle
        {
            get { return forcePathStyle; }
        }

        public void SetForcePathStyleString(string force)
        {
            if (!string.IsNullOrEmpty(force))
            {
                forcePathStyle = Convert.ToBoolean(force);
            }
        }

        public bool IsComplete()
        {
            return !(string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(apiUri) ||
                string.IsNullOrWhiteSpace(storageUri));
        }

        public void Update(APIConnectionInformation connectionInformation)
        {
            SetToken = connectionInformation.Token;
            SetApiUri = connectionInformation.ApiUri;
            SetStorageUri = connectionInformation.StorageUri;
            SetForcePathStyle = connectionInformation.GetForcePathStyle;
            SetAccountEmail = connectionInformation.AccountEmail;
        }

        public override string ToString()
        {
            return "Token:" + Token + Environment.NewLine +
                "Api:" + ApiUri + Environment.NewLine +
                "Storage:" + StorageUri + Environment.NewLine +
                "AccountEmail:" + AccountEmail + Environment.NewLine +
                "ForceStoragePathStyle:" + ForcePathStyle.ToString();
        }
    }
}