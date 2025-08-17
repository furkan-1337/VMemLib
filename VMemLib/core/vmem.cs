using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using VMemLib.definitions;

namespace VMemLib.core;

public class vmem : vmem_base
{
    public override types.InitializeResult Initialize(int processId, types.ProcessAccessFlags access)
    {
        Process vprocess;
        try {
            vprocess = Process.GetProcessById(processId);
        }
        catch (ArgumentException) {
            return types.InitializeResult.ProcessNotFound;
        }

        if (vprocess.HasExited)
            return types.InitializeResult.ProcessNotFound;

        var handle = pinvokes.OpenProcess(access, false, processId);
        if (handle == IntPtr.Zero)
            return types.InitializeResult.AccessDenied;

        SetContext(vprocess, handle, vprocess.MainModule.BaseAddress, access);
        return types.InitializeResult.Ok;
    }
}