using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using VMemLib.definitions;
using VMemLib.utils;

namespace VMemLib.core;

public abstract class vmem_base : ivmem
{
    public Process Process { get; private set; }
    public nint Handle { get; private set; }
    public nint BaseAddress { get; private set; }

    private types.ProcessAccessFlags _access;
    public types.ProcessAccessFlags Access
    {
        get => _access;
        set
        {
            _access = value;
            CanRead = _access.HasFlag(types.ProcessAccessFlags.VirtualMemoryRead);
            CanWrite = _access.HasFlag(types.ProcessAccessFlags.VirtualMemoryWrite);
            CanOperate = _access.HasFlag(types.ProcessAccessFlags.VirtualMemoryOperation);
        }
    }

    private bool CanRead { get; set; }
    private bool CanWrite { get; set; }
    private bool CanOperate { get; set; }

    public abstract types.InitializeResult Initialize(int processId, types.ProcessAccessFlags access);
    public types.InitializeResult Initialize(string processName, types.ProcessAccessFlags access)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
            return types.InitializeResult.ProcessNotFound;
        else
            return Initialize(processes.FirstOrDefault().Id, access);
    }
    
    private static nint BytesRead = nint.Zero;
    private static nint BytesWritten = nint.Zero;
    public byte[] ReadBytes(nint address, int size)
    {
        byte[] buffer = new byte[size];
        pinvokes.ReadProcessMemory(Handle, address, buffer, buffer.Length, out BytesRead);
        return buffer;
    }
    
    public bool ReadBytes(nint address, ref byte[] buffer)
    {
        return pinvokes.ReadProcessMemory(Handle, address, buffer, buffer.Length, out BytesRead);
    }
    
    public T Read<T>(nint address) where T : struct
    {
        if(!CanRead)
            throw new InvalidOperationException("You do not have the required access to read from this process.");
        byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
        pinvokes.ReadProcessMemory(Handle, address, buffer, buffer.Length, out BytesRead);
        return buffer.AsStruct<T>();
    }
    
    public void Write<T>(nint address, object value) where T : struct
    {
        if(!CanWrite)
            throw new InvalidOperationException("You do not have the required access to write from this process.");

        byte[] buffer = value.AsBytes();
        pinvokes.WriteProcessMemory(Handle, address, buffer, buffer.Length, out BytesWritten);
    }
    
    public float[] Matrix<T>(nint address, int size) where T : struct
    {
        byte[] buffer = new byte[Marshal.SizeOf(typeof(T)) * size];
        pinvokes.ReadProcessMemory(Handle, address, buffer, buffer.Length, out BytesRead);
        return buffer.ToFloats();
    }
    
    public string ReadAsString(nint address, int maxLength = 255, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        byte[] bytes = ReadBytes(address, maxLength);
        if (bytes == null || bytes.Length == 0) 
            return string.Empty;
        
        string text = encoding.GetString(bytes);

        int nullIndex = text.IndexOf('\0');
        if (nullIndex >= 0)
            text = text[..nullIndex];

        return text;
    }
    
    public static bool WorldToScreen(Vector3 target, out Vector2 pos, float[] viewmatrix, int width, int height)
    {
        //Matrix-vector Product, multiplying world(eye) coordinates by projection matrix = clipCoords
        pos = new Vector2(0, 0);
        Vector4 clipCoords = new Vector4()
        {
            X = target.X * viewmatrix[0] + target.Y * viewmatrix[1] + target.Z * viewmatrix[2] + viewmatrix[3],
            Y = target.X * viewmatrix[4] + target.Y * viewmatrix[5] + target.Z * viewmatrix[6] + viewmatrix[7],
            Z = target.X * viewmatrix[8] + target.Y * viewmatrix[9] + target.Z * viewmatrix[10] + viewmatrix[11],
            W = target.X * viewmatrix[12] + target.Y * viewmatrix[13] + target.Z * viewmatrix[14] + viewmatrix[15]
        };

        if (clipCoords.W < 0.1f)
            return false;

        //perspective division, dividing by clip.W = Normalized Device Coordinates
        Vector3 NDC;
        NDC.X = clipCoords.X / clipCoords.W;
        NDC.Y = clipCoords.Y / clipCoords.W;
        NDC.Z = clipCoords.Z / clipCoords.W;
        
        Vector2 display = new Vector2(width, height);
        pos.X = (display.X / 2 * NDC.X) + (NDC.X + display.X / 2);
        pos.Y = -(display.Y / 2 * NDC.Y) + (NDC.Y + display.Y / 2);
        return true;
    }
    
    public nint GetModuleAddress(string module)
    {
        foreach (ProcessModule process_module in Process.Modules)
        {
            if (process_module.ModuleName == module)
                return process_module.BaseAddress;
        }

        return nint.Zero;
    }
    
    /// <summary>
    /// Sets the internal context of the VMem object for a target process.
    /// This includes the process reference, handle, base address, and access flags.
    /// This method is typically called after successfully opening a process handle.
    /// </summary>
    /// <param name="process">The target <see cref="Process"/> object.</param>
    /// <param name="handle">The native handle returned by <see cref="pinvokes.OpenProcess"/>.</param>
    /// <param name="baseAddress">The base address of the process's main module.</param>
    /// <param name="access">The desired <see cref="types.ProcessAccessFlags"/> for this process.</param>
    /// <remarks>
    /// Do not call this method directly unless you have successfully obtained a valid process handle.
    /// Use <see cref="Initialize"/> for safe and validated initialization.
    /// </remarks>
    public void SetContext(Process process, nint handle, nint baseAddress, types.ProcessAccessFlags access)
    {
        Process = process;
        Handle = handle;
        BaseAddress = baseAddress;
        Access = access;
    }
}