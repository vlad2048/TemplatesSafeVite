# Run
```ps1
dotnet run
```

# Run (details)
## setup
```ps1
yarn
```

## server
```ps1
dotnet run --project src\Server\Server.fsproj
```

## client combined
```ps1
dotnet fable watch src\Client\Client.fsproj -o src\Client\output -s -e .jsx --run yarn run vite --host
```

## client separated
```ps1
dotnet fable src\Client\Client.fsproj -o src\Client\output -s -e .jsx

yarn run vite --host
```
