# VMemLib [![NuGet](https://img.shields.io/nuget/v/VMemLib.svg)](https://www.nuget.org/packages/VMemLib/)

**VMemLib** is a C# library for reading and writing memory of target processes. It provides safe and flexible access to process memory, supports reading and writing various data types, and includes helper functions for matrix and world-to-screen calculations.  

> ⚠️ **For educational purposes only. Do not use on software you do not own or have permission to access.**

> 
## Features
- Open and manage target processes
- Read and write memory
- Read strings and structs from memory
- Matrix / WorldToScreen transformations
- Get module base addresses
- Advanced access using handle hijacking (`vmem_h`)

## Key Classes

### `vmem_base`
- Base class for memory operations.
- Provides methods like:
  - `Initialize`
  - `Read<T>`
  - `Write<T>`
  - `ReadBytes`
  - `ReadAsString`
  - `GetModuleAddress`
- Includes helper methods for matrix and WorldToScreen transformations.

### `vmem`
- Inherits from `vmem_base`.
- Implements standard process memory access via OpenProcess.

### `vmem_h` [![NuGet](https://img.shields.io/nuget/v/VMemLib.HandleHijack.svg)](https://www.nuget.org/packages/VMemLib.HandleHijack/)
- Inherits from `vmem_base`.
- Uses handle hijacking to obtain enhanced access to target processes.

## Usage Example

```csharp
var memory = new vmem();
var result = memory.Initialize("notepad", types.ProcessAccessFlags.All);

if(result == types.InitializeResult.Ok)
{
    var value = memory.Read<int>(memory.BaseAddress + 0x1234);
    memory.Write(memory.BaseAddress + 0x1234, 42);
}
