# Create a Fable project from scratch
- Fable 4
- use Paket

## Create project
```ps1
# Create project
# ==============
dotnet new sln -o FromScratch
cd FromScratch
mkdir src
dotnet new classlib -o src\Client -lang "F#" -f netstandard2.0
dotnet sln add src\Client\Client.fsproj

# Use Paket
# =========
dotnet new tool-manifest
dotnet tool install paket
dotnet tool restore
dotnet paket convert-from-nuget --no-install
```

## edit .gitignore
```
bin/
obj/
paket-files/
```

## edit ./paket.dependencies
```
source https://api.nuget.org/v3/index.json
framework: netstandard2.0, net7.0

nuget FSharp.Core
```


## Install Fable
```ps1
dotnet tool install fable --prerelease
dotnet paket add Fable.Core --version ">= 4 theta"
dotnet paket add Fable.Browser.Dom
dotnet paket install
```

## Create ./package.json
```json
{
    "type": "module",
    "license": "MIT"
}
```

## Add vite.js
```ps1
yarn add -D vite

# creates:
#  - node_modules
#  - yarn.lock
yarn
```

## Create ./vite.config.js
```js
import { defineConfig } from 'vite'

export default defineConfig({
    plugins: [
    ],
    root: "./src/Client",
    server: {
        port: 8080,
        proxy: {
            '/api': 'http://localhost:5000',
        }
    },
    build: {
        outDir:"./"
    }
})
```

## Create ./src/Client/index.html
```html
<!doctype html>
<html>
<head>
  <title>FromScratch</title>
  <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
  <meta name="viewport" content="width=device-width, initial-scale=1">
</head>
<body>
    <canvas id="myCanvas"></canvas>
    <script src="output/App.js" type="module"></script>
</body>
</html>
```

## ./src/Client/App.fs
- rename ./src/Client/Library.fs to ./src/Client/App.fs
- and change the name in the .fsproj file
- and paste the content below
```f#
module App

open Fable.Core.JsInterop

let window = Browser.Dom.window

// Get our canvas context 
// As we'll see later, myCanvas is mutable hence the use of the mutable keyword
// the unbox keyword allows to make an unsafe cast. Here we assume that getElementById will return an HTMLCanvasElement 
let mutable myCanvas : Browser.Types.HTMLCanvasElement = unbox window.document.getElementById "myCanvas"  // myCanvas is defined in public/index.html

// Get the context
let ctx = myCanvas.getContext_2d()

// All these are immutables values
let w = myCanvas.width
let h = myCanvas.height
let steps = 20
let squareSize = 20

// gridWidth needs a float wo we cast tour int operation to a float using the float keyword
let gridWidth = float (steps * squareSize) 

// resize our canvas to the size of our grid
// the arrow <- indicates we're mutating a value. It's a special operator in F#.
myCanvas.width <- gridWidth
myCanvas.height <- gridWidth

// print the grid size to our debugger console
printfn "%i" steps

// prepare our canvas operations
[0..steps] // this is a list
  |> Seq.iter( fun x -> // we iter through the list using an anonymous function
      let v = float ((x) * squareSize) 
      ctx.moveTo(v, 0.)
      ctx.lineTo(v, gridWidth)
      ctx.moveTo(0., v)
      ctx.lineTo(gridWidth, v)
    ) 
ctx.strokeStyle <- !^"#ddd" // color

// draw our grid
ctx.stroke() 

// write Fable
ctx.textAlign <- "center"
ctx.fillText("Fable on Canvas", gridWidth * 0.5, gridWidth * 0.5)

printfn "done!"
```


## Run
```ps1
# compile F# to JS
dotnet fable src\Client\Client.fsproj -o src\Client\output -s

# watch and serve the JS
yarn run vite --host

# alternatively you can do it all with
dotnet fable watch src\Client\Client.fsproj -o src\Client\output -s --run yarn run vite --host
```
