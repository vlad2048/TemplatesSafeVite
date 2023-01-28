#       Run
[rem]:  ###
```
dotnet run
```

&nbsp;

&nbsp;

#       Package Management
[rem]:  ##################

## Paket
It's using **Paket** instead of nuget for .net packages
```ps1
# show installed packages (listed in ./paket.dependencies)
dotnet paket show-installed-packages

# update dependencies
# first: update .fsproj files to net7.0
dotnet paket update --no-install
dotnet paket update

# update Fable to v4
# dotnet tool update fable --prerelease
```

## Femto
Using **Femto** to keep JS packages (package.json) in sync with .Net packages (Paket)
```ps1
# analyze
dotnet femto src\Client
# resolve
dotnet femto --resolve src\Client
```

## Upgrade to Fable v4
- delete .\gobal.json
- change Shared/Server/Client .fsproj files to net7.0 (but not Build.fsproj)
- yarn
- update
```ps1
# udpate dotnet tools
dotnet tool update paket --prerelease
dotnet tool update fable --prerelease
dotnet tool update femto --prerelease
dotnet tool update fantomas-tool --prerelease

# update paket dependencies
dotnet paket update
```



&nbsp;

&nbsp;

#       CSS / SCSS
[rem]:  ##########

To import a .css (or .scss) file from JS and have vite.js watch it for changes,
add this line to ```src/Client/App.fs```:
```fs
Fable.Core.JsInterop.import "" "./public/base.scss"
```
And create ```src/Client/public/base.scss```

Note, this is how Fable translates the path given in F# to the path written in the .js file:
```
         base.scss		=>		                      base.scss
        /base.scss		=>		       ../../../../../base.scss
       ./base.scss		=>		                   ../base.scss
  public/base.scss		=>		               public/base.scss
 /public/base.scss		=>		../../../../../public/base.scss
./public/base.scss		=>		            ../public/base.scss
```
