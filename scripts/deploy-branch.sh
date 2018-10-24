#!/bin/bash

set -e

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "develop" ]; then
  echo "Deploying CI..."
  
  export Version=$(cat version)-beta-$(date +%Y%m%d%H%M%S)
  dotnet build -c Release
  dotnet pack -c Release

  for nupkg in ./src/Tars.Net.*/bin/Release/Tars.Net.*.nupkg; do dotnet nuget push $nupkg -k $MYNUGET_KEY -s https://www.myget.org/F/tarsnet; done
  
else
  echo "Skipping CI deploy"
fi