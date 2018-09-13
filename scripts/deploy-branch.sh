#!/bin/bash

set -e

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "master" ]; then
  echo "Deploying CI..."
  
  export Version=$(cat version)-beta-$(date +%Y%m%d%H%M%S)
  dotnet build -c Release
  dotnet pack -c Release

  for nupkg in ./src/Tars.Net.*/bin/Release/Tars.Net.*.nupkg; do dotnet nuget push $nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json; done
  
else
  echo "Skipping CI deploy"
fi