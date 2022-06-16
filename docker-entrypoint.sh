#!/bin/sh

# run the apps in the background
dotnet /app/listener/NanoPingPong.dll &
dotnet /app/web/NanoPingPong.Web.dll
