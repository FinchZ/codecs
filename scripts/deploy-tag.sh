#!/bin/bash

set -e

if [[ "${TRAVIS_TAG}" =~ ^v[0-9]+\.[0-9]+\.[0-9]+(-.*)?$ ]]; then
    echo "Deploying version ${TRAVIS_TAG}..."

    export Version=$(echo $TRAVIS_TAG | cut -c2-)
    export CurrentVersion=$(cat version)

    if [[ $Version != $CurrentVersion* ]]; then
      echo "Tag ${TRAVIS_TAG} is not compatible with current version ${CurrentVersion}" >&2
      exit 1
    fi

    dotnet build -c Release
    dotnet pack -c Release
    for nupkg in ./src/Tars.Net.*/bin/Release/Tars.Net.*.nupkg; do dotnet nuget push $nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json; done
else
  echo "Skipping version deploy"
fi