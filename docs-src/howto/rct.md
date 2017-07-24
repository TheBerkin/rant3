Rant includes a command-line utility, rct.exe, to simplify the process of building packages and programs.

## Commands

RCT includes the following commands:

### pack

The "pack" command is used to build packages. See the [packages](./packages) page for a guide on how to set up your package files for building.

```
Usage: rct pack [-out ...] [-version ...] [--no-compress] [path]

Parameters:
  -out                           Output path for package.
  -version                       Overrides the package version string in rantpkg.json.
  --no-compress                  Indicates that the package content should not be compressed.
```

### build

The "build" command compiles .rant pattern files into .rantpgm files.

```
Usage: rct build path

Parameters:
(None)
```

### fdocs

Generates a Rant function reference document in Markdown format.

```
Usage: rct fdocs [-out ...]

Parameters:
  -out                           Indicates the output path for the generated file.
```