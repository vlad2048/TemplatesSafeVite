# Run
```ps1
# setup
yarn

# server
dotnet run --project src\Server\Server.fsproj

# client
dotnet fable watch src\Client\Client.fsproj -o src\Client\output -s --run yarn run vite --host

# client separated
dotnet fable src\Client\Client.fsproj -o src\Client\output -s
yarn run vite --host
```
