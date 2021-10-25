namespace QarnotCLI.Test
{
    public static class HttpJobObject
    {
        public const string JobResponseBody = @"{
            ""errors"": [],
            ""status"": {
            },
            ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
            ""name"": ""job_name"",
            ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
            ""profile"": ""docker-batch"",
            ""state"": ""Success"",
            ""creationDate"": ""2019-11-08T10:54:11Z"",
            ""endDate"": ""0001-01-01T00:00:00Z"",
        }";

        public const string JobsListBodies = @"{
            ""Token"": ""the-token"",
            ""NextToken"": ""next-token"",
            ""IsTruncated"": false,
            ""Data"": [{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Success"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            },{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Success"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            },{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Success"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            }]
        }";

        public const string ActiveJobsListBodiesWithPaging = @"{
            ""Token"": ""the-token"",
            ""NextToken"": ""next-token"",
            ""IsTruncated"": false,
            ""Data"": [{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Active"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            },{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Active"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            },{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Active"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            }]
        }";

        public const string ActiveJobsListBodies = @"[{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Active"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            },{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Active"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            },{
                ""errors"": [],
                ""status"": {
                },
                ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""name"": ""job_name"",
                ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
                ""profile"": ""docker-batch"",
                ""state"": ""Active"",
                ""creationDate"": ""2019-11-08T10:54:11Z"",
                ""endDate"": ""0001-01-01T00:00:00Z"",
            }]";
    }
}
