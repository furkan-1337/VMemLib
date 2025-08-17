using System.Diagnostics;
using System.Text;
using VMemLib.definitions;

namespace VMemLib.core;

public interface ivmem
{
    // Properties
    Process Process { get; }
    nint Handle { get; }
    nint BaseAddress { get; }
    types.ProcessAccessFlags Access { get; set; }

    // Methods
    types.InitializeResult Initialize(int processId, types.ProcessAccessFlags access);
    types.InitializeResult Initialize(string processName, types.ProcessAccessFlags access);

    byte[] ReadBytes(nint address, int size);
    bool ReadBytes(nint address, ref byte[] buffer);
    T Read<T>(nint address) where T : struct;
    void Write<T>(nint address, object value) where T : struct;
    float[] Matrix<T>(nint address, int size) where T : struct;
    string ReadAsString(nint address, int maxLength = 255, Encoding? encoding = null);
    nint GetModuleAddress(string module);
    void SetContext(Process process, nint handle, nint baseAddress, types.ProcessAccessFlags access);
}