# Add Fable.Solid

## Install dependencies
```ps1
yarn add solid-js
yarn add -D vite-plugin-solid

# Implicitely depends on Fable.Solid
dotnet paket add Feliz.JSX.Solid --project src\Client\Client.fsproj
```

## Use Solid Vite plugin (./vite.config.js)
```js
import solidPlugin from 'vite-plugin-solid';

export default defineConfig({
    plugins: [
        solidPlugin()
    ],
    // ...
}
```

## Run
```ps1
dotnet run --project src\Server\Server.fsproj

dotnet fable watch src\Client\Client.fsproj -o src\Client\output -s -e .jsx --run yarn run vite --host

# Or
dotnet fable src\Client\Client.fsproj -o src\Client\output -s -e .jsx
yarn run vite --host
```