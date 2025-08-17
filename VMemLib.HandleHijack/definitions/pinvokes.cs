using System.Runtime.InteropServices;
using System.Text;

namespace VMemLib.HandleHijack.definitions;

public class pinvokes
{
    [DllImport("ntdll.dll")]
    public static extern types.NTSTATUS NtQuerySystemInformation(
        types.SYSTEM_INFORMATION_CLASS SystemInformationClass,
        IntPtr SystemInformation,
        int SystemInformationLength,
        out int ReturnLength);

    [DllImport("ntdll.dll")]
    //[return: MarshalAs(UnmanagedType.Bool)]
    public static extern types.NTSTATUS NtDuplicateObject(
        IntPtr hSourceProcessHandle,
        IntPtr hSourceHandle,
        IntPtr hTargetProcessHandle,
        out IntPtr lpTargetHandle,
        VMemLib.definitions.types.ProcessAccessFlags dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
        types.DuplicateOptions dwOptions);


    [DllImport("ntdll.dll")]
    public static extern types.NTSTATUS NtQueryObject(
        IntPtr ObjectHandle,
        types.OBJECT_INFORMATION_CLASS ObjectInformationClass,
        IntPtr ObjectInformation,
        int ObjectInformationLength,
        out int ReturnLength);

    [DllImport("kernel32.dll")]
    public static extern bool QueryFullProcessImageName(
        IntPtr hprocess,
        int dwFlags,
        StringBuilder lpExeName,
        out int size);
    
    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern IntPtr RtlAdjustPrivilege(types.Privilege privilege/*int Privilege*/, bool bEnablePrivilege,
        bool IsThreadPrivilege, out bool PreviousValue);
    
    public static string GetHandleType(IntPtr handle)
    {
        NtQueryObject(handle, types.OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out int length);

        IntPtr buffer = Marshal.AllocHGlobal(length);
        try
        {
            types.NTSTATUS status = NtQueryObject(handle, types.OBJECT_INFORMATION_CLASS.ObjectTypeInformation, buffer, length, out length);
            if (status != 0) return null;

            types.OBJECT_TYPE_INFORMATION objType = Marshal.PtrToStructure<types.OBJECT_TYPE_INFORMATION>(buffer);
            return Marshal.PtrToStringUni(objType.Name.Buffer, objType.Name.Length / 2);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}