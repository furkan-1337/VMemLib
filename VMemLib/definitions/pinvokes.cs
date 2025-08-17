using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace VMemLib.definitions;

public class pinvokes
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nint OpenProcess(types.ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SuppressUnmanagedCodeSecurity]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(nint hObject);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int dwSize, out nint lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out nint lpNumberOfBytesRead);
}