# QarnotCli [![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/qarnot/qarnot-cli?sort=semver)](https://github.com/qarnot/qarnot-cli/releases)


QarnotCLI is the Command Line Interface to interact with the Qarnot Computing platform

### Build the CLI Binaries

Launch in `QarnotCLI` directory :

```
dotnet publish -c Release -r ubuntu-x64 --self-contained true /p:PublishSingleFile=true -o ./dest/bin
```
```
dotnet publish -c Release -r win10-x64 --self-contained true /p:PublishSingleFile=true -o ./dest/bin
```

### Build the documentation
```bash
./QarnotCLI_Doc/build_doc.sh
```

The built documentation will be in `QarnotCLI_Doc/DocfxDocumentation/docfx_project/_site/`, you can view
it for instance by calling

```bash
firefox QarnotCLI_Doc/DocfxDocumentation/docfx_project/_site/index.html
```


### Set the connection
When you use the CLI for the first time you need to set the configuration :
```bash
./qarnot config set -t [token] -u [qarnot.api] -s [storage.api] -f
./qarnot config set -t abcd -u "https://api.qarnot.com" -s "https://storage.qarnot.com"
```

### qarnot commands

> see documentation

|command|sub-command|example|
|-|-|-|
|task|create|`./qarnot task create --instance 4 --name "Task name" --profile docker-batch --tags TAG1 --constants "DOCKER_CMD=echo Hello World"`|
|task|infos|`./qarnot task infos --name "Task name"`|
|task|list|`./qarnot task list`|
|task|wait|`./qarnot task --id bb7c4f7c-d692-45d4-9c12-83b5a2b88ab3`|
|task|abort|`./qarnot task abort --all`|
|task|delete|`./qarnot task delete --tags TAG1`|
|task|carbon-facts|`./qarnot task carbon-facts -i bb7c4f7c-d692-45d4-9c12-83b5a2b88ab3 --datacenter DATACENTER_NAME`|
|pool|create|`./qarnot task create --name "Pool name" --profile docker-batch --tags TAG1 --pool-is-elastic --min-slot 3 --max-slot 5 `|
|pool|infos|`./qarnot pool infos --name "Pool name"`|
|pool|list|`./qarnot pool list`|
|pool|set|`./qarnot pool set --id bb7c4f7c-d692-45d4-9c12-83b5a2b88ab3 --min-slot 2`|
|pool|abort|`./qarnot pool abort --all`|
|pool|delete|`./qarnot pool delete --tags TAG1`|
|pool|carbon-facts|`./qarnot pool carbon-facts -i bb7c4f7c-d692-45d4-9c12-83b5a2b88ab3 --datacenter DATACENTER_NAME`|
|job|create|`./qarnot job create  --name "Job name"`|
|job|infos|`./qarnot job infos --name "Job name"`|
|job|list|`./qarnot job list`|
|job|abort|`./qarnot job abort --all`|
|job|delete|`./qarnot job delete --tags TAG1`|
|bucket|create|`./qarnot bucket create --name "mybucket"`|
|bucket|list|`./qarnot bucket list`|
|bucket|get|`./qarnot bucket get`|
|bucket|set|`./qarnot bucket set`|
|bucket|sync-from|`./qarnot bucket sync-from`|
|bucket|sync-to|`./qarnot bucket sync-to`|
|bucket|delete|`./qarnot bucket delete --tags TAG1`|
|all||`./qarnot all --list`|
|account||`./qarnot account`|
|config|set|`./qarnot config set -t abcd -u "https://api.qarnot.com" -s "https://storage.qarnot.com"`|
|config|show|`./qarnot config show`|
|secrets|create|`./qarnot secrets create "key" "value"`|
|secrets|get|`./qarnot secrets get "key"`|
|secrets|update|`./qarnot secrets update "key" "value"`|
|secrets|delete|`./qarnot secrets delete "key"`|
|secrets|list|`./qarnot secrets list [--recursive] ["prefix"]`|

### Contributing

To find some guidelines on how to add new commands, have a look at `./QarnotCLI/README.md`.
