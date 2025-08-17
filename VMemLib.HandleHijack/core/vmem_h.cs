using System.Diagnostics;
using System.Runtime.InteropServices;
using VMemLib.core;
using VMemLib.HandleHijack.definitions;

namespace VMemLib.HandleHijack;

public class vmem_h : vmem_base
{
    public override VMemLib.definitions.types.InitializeResult Initialize(int processId, VMemLib.definitions.types.ProcessAccessFlags access)
    {
        Process vprocess;
        try {
            vprocess = Process.GetProcessById(processId);
        }
        catch (ArgumentException) {
            return VMemLib.definitions.types.InitializeResult.ProcessNotFound;
        }

        if (vprocess.HasExited)
            return VMemLib.definitions.types.InitializeResult.ProcessNotFound;

        var handle = HijackHandle(processId, access);
        if (handle == IntPtr.Zero)
            return VMemLib.definitions.types.InitializeResult.AccessDenied;

        SetContext(vprocess, handle, vprocess.MainModule.BaseAddress, access);
        return VMemLib.definitions.types.InitializeResult.Ok;
    }
    
    public static nint HijackHandle(int processId, VMemLib.definitions.types.ProcessAccessFlags flags)
    {
        VMemLib.HandleHijack.definitions.pinvokes.RtlAdjustPrivilege(types.Privilege.SeDebugPrivilege, true, true, out bool OldPriv);
        var systemInformationBufferLength = 0x10000;
        var systemInformationBufferPtr = Marshal.AllocHGlobal(systemInformationBufferLength);
        var trueBufferLength = 0;
        types.NTSTATUS queryResult = pinvokes.NtQuerySystemInformation(types.SYSTEM_INFORMATION_CLASS.SystemHandleInformation, systemInformationBufferPtr, systemInformationBufferLength, out trueBufferLength);

        while (queryResult == types.NTSTATUS.InfoLengthMismatch)
        {
            systemInformationBufferLength = trueBufferLength;
            Marshal.FreeHGlobal(systemInformationBufferPtr);
            systemInformationBufferPtr = Marshal.AllocHGlobal(systemInformationBufferLength);
            queryResult = pinvokes.NtQuerySystemInformation(types.SYSTEM_INFORMATION_CLASS.SystemHandleInformation, systemInformationBufferPtr, systemInformationBufferLength, out trueBufferLength);
        }
        var numberOfHandles = Marshal.ReadInt64(systemInformationBufferPtr);
        var handleEntryPtr = new IntPtr((long)systemInformationBufferPtr + sizeof(long));
        Dictionary<int, List<types.SYSTEM_HANDLE_TABLE_ENTRY_INFO>> allHandles = new();
        for (var i = 0; i < numberOfHandles; i++)
        {
            var currentHandle = (types.SYSTEM_HANDLE_TABLE_ENTRY_INFO)Marshal.PtrToStructure(handleEntryPtr, typeof(types.SYSTEM_HANDLE_TABLE_ENTRY_INFO));
            handleEntryPtr = new IntPtr((long)handleEntryPtr + Marshal.SizeOf(currentHandle));

            if (!allHandles.ContainsKey(currentHandle.UniqueProcessId))
                allHandles.Add(currentHandle.UniqueProcessId, new List<types.SYSTEM_HANDLE_TABLE_ENTRY_INFO>());

            allHandles[currentHandle.UniqueProcessId].Add(currentHandle);
        }
        Marshal.FreeHGlobal(systemInformationBufferPtr);

        var hCurrentProcess = VMemLib.definitions.pinvokes.OpenProcess(VMemLib.definitions.types.ProcessAccessFlags.All, false, Process.GetCurrentProcess().Id);
        foreach (var handleInfo in allHandles[processId])
        {
            //Console.WriteLine($"Handle: 0x{handleInfo.HandleValue:X}, Type: {handleInfo.ObjectType} Name: {pinvokes.GetHandleType(handleInfo.HandleValue)}");
            //if (handleInfo.ObjectType != 0x5 && handleInfo.ObjectType != 0x7) /* Just process handles */
                //continue;
            if(handleInfo.ObjectType != 8) // wtf??
                continue;
            
            var grantedAccess = (VMemLib.definitions.types.ProcessAccessFlags)handleInfo.AccessMask;
            if (!grantedAccess.HasFlag(flags)) continue;

            var processHandle = VMemLib.definitions.pinvokes.OpenProcess(VMemLib.definitions.types.ProcessAccessFlags.DuplicateHandle, false, processId);
            var status = pinvokes.NtDuplicateObject(processHandle, handleInfo.HandleValue, hCurrentProcess, out nint DuplicateHandle, flags,
                        false,
                        types.DuplicateOptions.DUPLICATE_SAME_ACCESS);

            return DuplicateHandle;
        }
        return nint.Zero;
    }
}