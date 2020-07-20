# Quick Start!

#### Qarnot Documentation : [Documentation](https://computing.qarnot.com/developers/overview/qarnot-computing-home)
#### Qarnot API : [Api Console](https://console.qarnot.com)

## Install

#### from the binary 

* Go to [the CLI releases](https://github.com/qarnot/qarnot-cli/releases)
* download your OS release binary 
* you can rename your release binary to `qarnot` on mac and linux, and move it in your binary file to use it as in the examples below

#### Rebuild from the source code

```bash
git clone https://github.com/qarnot/qarnot-cli.git qarnot-cli
cd qarnot-cli
dotnet publish -c Release -r ubuntu-x64 /p:PublishSingleFile=true -o ./your/dest/folder/bin
```
__use the correct runtime identifier depending on your platform [runtime identifiers](https://docs.microsoft.com/fr-fr/dotnet/core/rid-catalog)__

## Set your connection information
Your connection information are in : [https://account.qarnot.com/compute](https://account.qarnot.com/compute)  
Launch (with your token): 
```bash
qarnot config -t 0123456789__myAPIToken__9876543210
```

If you need to set a specific custom storage or compute uri : 
```bash
qarnot config -t 0123456789__myAPIToken__9876543210 -u https://api.qarnot.com -s https://storage.qarnot.com -f true
```

#### Environment variable used to change the configuration

| Name | usage |
| - | - |
| QARNOT_LOCAL_PATH | Specify a custom configuration file path | 
| QARNOT_DEFAULT_PATH | Specify a global configuration file path check if the QARNOT_LOCAL_PATH is not found | 
| QARNOT_CLIENT_TOKEN | Use your api connection token without storing it | 
| QARNOT_CLUSTER_URL | Use a custom qarnot api url | 
| QARNOT_STORAGE_URL | Use a custom qarnot storage url | 
| QARNOT_ACCOUNT_EMAIL | Specify your qarnot email account | 
| QARNOT_USE_STORAGE_PATH_STYLE | force the storage path style, it must be "true" or "false" | 

## Create a Qarnot Resource

#### Directly on the command line 

##### Create an "hello world" Task
```bash
qarnot task create  --name "Task name" --profile docker-batch --instance 4 --constants "DOCKER_CMD=echo hello world" 
```

```bash
Uuid : fb6e883c-f136-49cf-84b2-bd2f73bfdb60
Message : New task create, state : Submitted
```

##### Create a bucket
```bash
qarnot bucket create -n Bucket_name --files test_file.txt test_file2.txt 
```
```bash
Uuid : Bucket_name
Message : New bucket create,
```


#### With the help of a configuration file 
(not available for Buckets)
```bash
qarnot task create -f CreateTask.json --shortname use_a_shortname_different_to_the_file_shortname 
```
```bash
create task
Uuid : fb6e883c-f136-49cf-84b2-bd2f73bfdb60
Message : New task create, state : Submitted
```


[configuration file models](fileCreate.md)

## Manage the Qarnot Resources

### List resources

List all the pool
```bash
qarnot pool list --tags PoolList
```

```bash
 ------------------------------------------------------------------------------------------------------------
 | Name    | State           | Uuid                                 | Shortname  | Profile      | NodeCount |
 ------------------------------------------------------------------------------------------------------------
 | poolN°1 | Closed          | 6de206a7-5170-4c3e-8e77-da15c7ad81b4 | id-1       | docker-batch | 2         |
 ------------------------------------------------------------------------------------------------------------
 | poolN°2 | FullyDispatched | abeec2bc-4137-43ba-b1fb-8ec2fe850b95 | id-2       | docker-batch | 10        |
 ------------------------------------------------------------------------------------------------------------

```

### Get resources details

Get all the information of a job 
```bash
qarnot job info --id 77951c5b-7c63-4525-a270-c62bbe0ca476
```
```json
[
  {
    "Connection": {
        ...
    },
    "Uuid": "77951c5b-7c63-4525-a270-c62bbe0ca476",
    "Name": "name",
    "Shortname": "77951c5b-7c63-4525-a270-c62bbe0ca476",
    "PoolUuid": "00000000-0000-0000-0000-000000000000",
    "State": "Active",
    "CreationDate": "2020-02-28T08:55:45Z",
    "LastModified": "2020-02-28T08:55:45Z",
    "UseDependencies": false,
    "MaximumWallTime": "00:00:00",
    "Pool": null
  }
]
```

Get all the information of a job in XML Format
```bash
qarnot job info --id 77951c5b-7c63-4525-a270-c62bbe0ca476 --format XML
```
```xml
<Information>
  <Values>
    <Connection>
        ...
    </Connection>
    <Uuid>77951c5b-7c63-4525-a270-c62bbe0ca476</Uuid>
    <Name>name</Name>
    <Shortname>77951c5b-7c63-4525-a270-c62bbe0ca476</Shortname>
    <PoolUuid>00000000-0000-0000-0000-000000000000</PoolUuid>
    <State>Active</State>
    <CreationDate>2020-02-28T08:55:45Z</CreationDate>
    <LastModified>2020-02-28T08:55:45Z</LastModified>
    <UseDependencies>false</UseDependencies>
    <MaximumWallTime>00:00:00</MaximumWallTime>
    <Pool />
  </Values>
</Information>
```

### Abort a resource

```bash
qarnot job abort --name "name"
```
```bash
 ---------------------------------------------------------- 
 | Uuid                                 | Message         |
 ---------------------------------------------------------- 
 | 77951c5b-7c63-4525-a270-c62bbe0ca476 | Job terminate   |
 ---------------------------------------------------------- 

 Count: 1

```

### Delete a resource

delete all the jobs

```bash
qarnot job delete --all
```
```bash
 ------------------------------------------------------- 
 | Uuid                                 | Message      |
 ------------------------------------------------------- 
 | 6e40bac4-c2dc-4812-be48-171e36b5e271 | Job delete   |
 ------------------------------------------------------- 
 | 20f22eea-de5a-496f-a0f8-952cab173375 | Job delete   |
 ------------------------------------------------------- 
 | 543c72ce-3e32-415f-8cf0-0567fd5638fb | Job delete   |
 ------------------------------------------------------- 
 | 77951c5b-7c63-4525-a270-c62bbe0ca476 | Job delete   |
 ------------------------------------------------------- 

 Count: 4
```

## Manage your account

```bash
qarnot account
```
```bash
AccountInformation : 
    Email : my.mail@adress.com
    MaxInstances : 50
    Quota Bucket Number : 5/10
    Quota Bytes Bucket : 10443/107342
    Quota Task Count : 1/50
    Quota Running Task : 0/50
    Quota Total Pool : 6/50
    Quota Running Pool : 4/50
```
