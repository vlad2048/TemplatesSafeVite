# Create a Server project with Saturn and enable Fable.Remoting

## Create Shared & Server project
```ps1
dotnet new classlib -o src\Shared -lang "F#" -f netstandard2.0
dotnet sln add src\Shared\Shared.fsproj
# rename Library.fs to Shared.fs

dotnet new console -o src\Server -lang F#
dotnet sln add src\Server\Server.fsproj

# make sure both projects are setup for Paket:
#   - <Import Project="..\..\.paket\Paket.Restore.targets" />
#   - paket.references

dotnet paket add FSharp.Core --project src\Server\Server.fsproj
```

## Install Saturn
```ps1
dotnet paket add Saturn --project src\Server\Server.fsproj
dotnet paket add Fable.Remoting.Giraffe --project src\Server\Server.fsproj
dotnet paket add Fable.Remoting.Client --project src\Client\Client.fsproj
```