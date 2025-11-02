#!/bin/bash

find . -type d -name "obj" -prune -exec rm -rf {} +
find . -type d -name "bin" -prune -exec rm -rf {} +
find . -type d -name "coverage_report" -prune -exec rm -rf {} +
find . -type d -name "TestResults" -prune -exec rm -rf {} +
find . -type f -name "coverage.info" -prune -exec rm -rf {} + 
find . -type f -name "coverage.cobertura.xml" -prune -exec rm -rf {} + 
find . -type f -name "coverage.opencover.xml" -prune -exec rm -rf {} + 
find . -type f -name "coverage.json" -prune -exec rm -rf {} + 
dotnet clean
dotnet tool update dotnet-reportgenerator-globaltool
dotnet test -l trx  \
    /p:CollectCoverage=true \
    /p:CoverletOutput="../" \
    /p:MergeWith="../coverage.json" \
    /p:CoverletOutputFormat=json%2copencover%2clcov%2ccobertura 
dotnet reportgenerator -reports:./__tests__/coverage.cobertura.xml -targetdir:coverage_report
npx http-server -o coverage_report -c-1
