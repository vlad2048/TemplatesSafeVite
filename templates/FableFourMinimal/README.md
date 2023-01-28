# Run
## Setup
```ps1
yarn
```

## Watch & Run
```ps1
dotnet fable watch src\Client\Client.fsproj -o src\Client\output -s --run yarn run vite --host
```

## Run separatly
```ps1
# compile F# to JS in src\Client\output (-s for source maps)
dotnet fable src\Client\Client.fsproj -o src\Client\output -s

# watch & serve src\Client\output on http://localhost:8080/
yarn run vite --host
```
