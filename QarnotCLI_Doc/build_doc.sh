#!/bin/bash

set -e

expected_cur_dir=$(readlink -f $(dirname ${BASH_SOURCE[0]}))
cd $expected_cur_dir

pushd ..  # go back to repo toplevel

pushd QarnotCLI
mkdir -p ubuntu/debug
dotnet build -c Debug;
dotnet publish -c Debug -r ubuntu-x64 /p:PublishSingleFile=true -o ./bin/ubuntu/debug;
popd  # back to toplevel

pushd QarnotCLI_Doc
mkdir -p DocfxDoccumentation/docfx_project/man
pushd CreateDoc/
python3 createDocMarkdown.py ../../QarnotCLI/bin/ubuntu/debug/qarnot
cp manMarkDown/* ../DocfxDoccumentation/docfx_project/man
popd  # back to QarnotCLI_Doc

pushd DocfxDoccumentation
if [ ! -d docfx ]; then
    nuget install docfx.console -OutputDirectory docfx -Version 2.49.0
fi
chmod 755 docfx/docfx.console.2.49.0/tools/docfx.exe
mono docfx/docfx.console.2.49.0/tools/docfx.exe metadata docfx_project/docfx.json
mono docfx/docfx.console.2.49.0/tools/docfx.exe build docfx_project/docfx.json
pushd docfx_project/_site
cp images/Q\ -\ white.svg logo.svg && cp images/Q\ -\ black.png favicon.ico

popd  # back to DocfsDocumentation
popd  # back to QarnotCLI_Doc
popd  # back to toplevel
popd  # back to script directory

# back in toplevel directory
