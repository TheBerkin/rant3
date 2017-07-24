Packages are archives of resources, such as programs and tables, that can be packed into a single file with the extension `.rantpkg`.

By loading a package into Rant, all of the programs and tables contained within it become available for use by the associated `RantEngine`.

## Authoring packages

A package is created by placing all of the desired resources into a common folder along with a `rantpkg.json` file containing
information that tells the packer about metadata, dependencies, and where to place the built file.

The following resource filetypes are currently supported:

* `.rant`
* `.rantpgm`
* `.table`

### The package metadata file

The required `rantpkg.json` file contains a JSON object with the following information:

```json
{
  "title": "Your Package Name",
  "id": "YourPackageId",
  "version": "1.0.0",
  "tags": ["rantionary", "official", "dictionary"],
  "authors": ["Nicholas Fleck", "Andrew Rogers"],
  "description": "Description of your package",
  "out": "relative/save/path",
  "dependencies": [
      {
          "id": "DependencyId",
          "version": "1.0.0",
          "allow-newer": true
      }
  ]
}
```

### Packing

To create the package file, use the Rant Command Line Tools (included in the main repository) to run the following command in the package content folder:

```
rct pack
```

You can override the output path specified in `rantpkg.json` by using the `-out` parameter.

```
rct pack -out "alternate/save/path"
```


## Loading packages

To load a package, use the `RantEngine.LoadPackage` method.

```csharp
var rant = new RantEngine();
rant.LoadPackage("Rantionary.rantpkg");
```

Afterwards, you may access your packed patterns by passing their relative path in the package into the `RantEngine.DoPackaged` method.
No file extension is required.

```csharp
var output = rant.DoPackaged("dialogue/greetings/hello");
```