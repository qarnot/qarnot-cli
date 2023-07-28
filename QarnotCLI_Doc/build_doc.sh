#!/bin/bash

version="2.75.1" # last stable version when writing this

if [ $# -ne 0 ]
then
    version=$1
fi

set -e

expected_cur_dir=$(readlink -f $(dirname ${BASH_SOURCE[0]}))
cd $expected_cur_dir

pushd ..  # go back to repo toplevel

pushd QarnotCLI
mkdir -p ubuntu/debug
dotnet publish -c Debug -r ubuntu-x64 /p:PublishSingleFile=true -o ./bin/ubuntu/debug --use-current-runtime --self-contained;
popd  # back to toplevel

pushd QarnotCLI_Doc
if ! [ -d DocfxDocumentation/docfx_project/man ]
then
    mkdir -p DocfxDocumentation/docfx_project/man
    pushd CreateDoc/
    dotnet run
    cp manMarkDown/* ../DocfxDocumentation/docfx_project/man
    popd  # back to QarnotCLI_Doc
fi

pushd DocfxDocumentation
# N.B.: docfx update as a dotnet tool is available only from version 2.60.0 so if force with previous version it will not work
# N.B.B: it is not possible to downgrade an existing version of a dotnet tool
# So if versionning is not working, do not force version and use latest release of the tool
dotnet tool update -g docfx --version $version || dotnet tool update -g docfx
docfx="$(find / -name docfx  2> /dev/null | grep tools/docfx)"
$docfx metadata docfx_project/docfx.json
$docfx build docfx_project/docfx.json
pushd docfx_project/_site
cp images/Q\ -\ white.svg logo.svg && cp images/Q\ -\ black.png favicon.ico

popd  # back to DocfsDocumentation
popd  # back to QarnotCLI_Doc
popd  # back to toplevel
popd  # back to script directory

# back in toplevel directory
