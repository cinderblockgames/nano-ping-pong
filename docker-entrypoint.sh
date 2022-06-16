#!/bin/sh

dotnet /app/listener/NanoPingPong.dll &
dotnet /app/web/NanoPingPong.Web.dll
