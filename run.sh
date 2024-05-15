#!/bin/bash

DIR="$(dirname "$(realpath "$0")")"

dotnet run --project "$DIR/Cli/Cli.csproj" -- $*
