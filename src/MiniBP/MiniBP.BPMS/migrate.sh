#!/bin/bash

if [ "$1" != "" ]; then
    name=$1
else
    name=`date +"%Y-%m-%d--%H:%M:%S"`
    echo -n "Migration name has not provided. Automatic naming activated: $name"
    echo
fi

dotnet ef migrations add "$name" --startup-project ../Api/Api.csproj --context PostgresBpmsDbContext
dotnet ef database update "$name" --context PostgresBpmsDbContext --startup-project ../Api/Api.csproj

