#!/bin/bash

dotnet restore
dotnet build
cd test/Tars.Net.Test
dotnet minicover instrument --workdir ../../ --assemblies test/**/bin/**/*.dll --sources src/**/*.cs 
dotnet minicover reset
cd ../../
for project in test/**/*.csproj; do dotnet test --no-build $project; done
cd test/Tars.Net.Test
dotnet minicover uninstrument --workdir ../../
dotnet minicover htmlreport --workdir ../../ --threshold 10
dotnet minicover report --workdir ../../ --threshold 10
cd ../../