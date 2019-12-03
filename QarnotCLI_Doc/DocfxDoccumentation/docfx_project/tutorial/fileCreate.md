#### [Return](tutorial.md)

##### Tasks
```bash
cat CreateTask.json
{
  "Name": null,
  "Shortname": null,
  "InstanceCount": 0,
  "Range": null,
  "Profile": null,
  "Tags": [],
  "Constants": [],
  "Ressources": [],
  "Result": null,
  "Dependents": [],
  "JobUuid": null,
  "PoolUuid": null,
}
```

##### Pools
```bash
cat CreatePool.json
{
  "Name": null,
  "Shortname": null,
  "Profile": null,
  "InstanceCount": 0,
  "Tags": [],
  "Constants": [],
  "Ressources": [],
  "IsElastic": false,
  "ElasticMinimumTotalNodes": 0,
  "ElasticMaximumTotalNodes": 0,
  "ElasticMinimumIdlingNodes": 0,
  "ElasticResizePeriod": 0,
  "ElasticResizeFactor": 0.0,
  "ElasticMinimumIdlingTime": 0
}
```

##### Jobs
```bash
cat CreateJob.json
{
  "Name": null,
  "Shortname": null,
  "PoolUuid": null,
  "IsDependents": false,
  "Tags": [],
}
```
