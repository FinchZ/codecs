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
dotnet minicover report --workdir ../../ --threshold 80
dotnet minicover coverallsreport --workdir ../../ --output "coveralls.json" --branch "$TRAVIS_BRANCH" --service-name "$COVERALLS_SERVICE" --service-job-id "$TRAVIS_JOB_ID" --repo-token "$COVERALLS_TOKEN"
cd ../../