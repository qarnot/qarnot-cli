namespace QarnotCLI.Test
{
    public static class HttpPoolObject
    {
        public const string PoolResponseUuid = "f78fdff8-7081-46e1-bb2f-d9cd4e185ece";

        public const string PoolResponseName = "pool_name";

        public const string PoolsListUuid = "796a5321-0001-4a5c-2f42-54cce169dff8,796a5321-0002-4a5c-2f42-54cce169dff8,796a5321-0003-4a5c-2f42-54cce169dff8";

        public const string PoolResponseBody = @"{
            ""elasticProperty"": {},
            ""constants"": [],
            ""tags"": [],
            ""errors"": [],
            ""resourceBuckets"": [],
            ""status"": {
            },
            ""uuid"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
            ""name"": ""pool_name"",
            ""shortname"": ""f78fdff8-7081-46e1-bb2f-d9cd4e185ece"",
            ""profile"": ""docker-batch"",
            ""state"": ""Success"",
            ""instanceCount"": 3,
            ""creationDate"": ""2019-11-08T10:54:11Z"",
            ""endDate"": ""0001-01-01T00:00:00Z"",
            ""runningInstanceCount"": 3,
            ""runningCoreCount"": 16,
            ""executionTime"": ""00:00:25.1000003"",
            ""wallTime"": ""00:02:12"",
            ""credits"": 0.01
        }";

        public const string PoolsListBodies = @"[{
            ""elasticProperty"": {},
            ""constants"": [],
            ""constraints"": [],
            ""tags"": [],
            ""errors"": [],
            ""resourceBuckets"": [],
            ""status"": {},
            ""uuid"": ""796a5321-0001-4a5c-2f42-54cce169dff8"",
            ""name"": ""Pool Name Test List1"",
            ""shortname"": ""AddConstTagHandler-shortname1"",
            ""profile"": ""AddConstTagHandler-Profile"",
            ""state"": ""Success"",
            ""instanceCount"": 3,
            ""creationDate"": ""2019-11-08T10:54:11Z"",
            ""endDate"": ""0001-01-01T00:00:00Z"",
            ""runningInstanceCount"": 3,
            ""runningCoreCount"": 16,
            ""executionTime"": ""00:00:25.1000003"",
            ""wallTime"": ""00:02:12"",
            ""credits"": 0.01
        },{
            ""elasticProperty"": {},
            ""constants"": [],
            ""constraints"": [],
            ""tags"": [],
            ""errors"": [],
            ""resourceBuckets"": [],
            ""status"": {},
            ""uuid"": ""796a5321-0002-4a5c-2f42-54cce169dff8"",
            ""name"": ""Pool Name Test List2"",
            ""shortname"": ""AddConstTagHandler-shortname2"",
            ""profile"": ""AddConstTagHandler-Profile"",
            ""state"": ""Success"",
            ""instanceCount"": 3,
            ""creationDate"": ""2019-11-08T10:54:11Z"",
            ""endDate"": ""0001-01-01T00:00:00Z"",
            ""runningInstanceCount"": 3,
            ""runningCoreCount"": 16,
            ""executionTime"": ""00:00:25.1000003"",
            ""wallTime"": ""00:02:12"",
            ""credits"": 0.01
        },{
            ""elasticProperty"": {},
            ""constants"": [],
            ""constraints"": [],
            ""tags"": [],
            ""errors"": [],
            ""resourceBuckets"": [],
            ""status"": {},
            ""uuid"": ""796a5321-0003-4a5c-2f42-54cce169dff8"",
            ""name"": ""Pool Name Test List3"",
            ""shortname"": ""AddConstTagHandler-shortname3"",
            ""profile"": ""AddConstTagHandler-Profile"",
            ""state"": ""Success"",
            ""instanceCount"": 3,
            ""creationDate"": ""2019-11-08T10:54:11Z"",
            ""endDate"": ""0001-01-01T00:00:00Z"",
            ""runningInstanceCount"": 3,
            ""runningCoreCount"": 16,
            ""executionTime"": ""00:00:25.1000003"",
            ""wallTime"": ""00:02:12"",
            ""credits"": 0.01
        }]";
    }
}
