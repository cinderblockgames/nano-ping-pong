#!/bin/sh

# run the apps in the background
dotnet /app/listener/NanoPingPong.dll &
dotnet /app/web/NanoPingPong.Web.dll &

# tail the output log in the foreground to keep the container running
tail -f /run/logs/output.log
