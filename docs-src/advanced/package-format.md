Packages are binary files containing serialized Rant resources.
Resources can be stored in an uncompressed format, or compressed using DEFLATE.

## Data Types

All types are little-endian unless otherwise specified.

All strings are encoded in UTF-8 unless otherwise specified.

- **bool**: 8-bit boolean value: 0x1 = true, 0x0 = false
- **byte**: 8-bit unsigned integer
- **short**: 16-bit signed integer
- **ushort**: 16-bit unsigned integer
- **int**: 32-bit signed integer
- **float**: 32-bit float
- **double**: 64-bit float
- **string**: String: `int` containing length in bytes followed by sequence of characters
- **string[]**: String array: `int` containing item count followed by sequence of `string` items

## Header
```c
struct RantPackageHeader
{
	int Magic = 0x4E465250; // "NFRP"
	int FormatVersion;
	bool Compress; // Indicates resource data is compressed using DEFLATE
	string PackageTitle;
	string PackageId;
	string PackageDescription;
	string[] PackageTags;
	string[] PackageAuthors;
	int PackageVerMajor;
	int PackageVerMinor;
	int PackageVerRevision;
	int DependencyCount;
	RantPackageDependency[DependencyCount] Dependencies;
	int ResourceCount;
}

struct RantPackageDependency
{
	string ID;
	int DepVerMajor;
	int DepVerMinor;
	int DepVerRevision;
	bool AllowNewer;
}
```

## Resource chunks
Following the file header is a sequence of serialized resources.
Each resource is prefixed with 4 bytes that identify the resource type.
It is up to the resource serializer to indicate how large the data chunk is.

### Type codes

|Type Code|Hex|Description|
|---|---|---|
|dic2|0x64696332|Dictionary table|
|prog|0x70726F67|Rant program|