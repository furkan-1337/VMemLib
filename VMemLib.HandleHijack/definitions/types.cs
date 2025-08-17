using System.Runtime.InteropServices;

namespace VMemLib.HandleHijack.definitions;

public class types
{
        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GENERIC_MAPPING
        {
            public int GenericRead;
            public int GenericWrite;
            public int GenericExecute;
            public int GenericAll;
        }

        [Flags]
        public enum NTSTATUS : uint
        {
            // Success
            Success = 0x00000000,

            //Errors
            InfoLengthMismatch = 0xc0000004,
            AccessViolation = 0xC0000005
        }

        // used in NtQuerySystemInformation API call
        [Flags]
        public enum SYSTEM_INFORMATION_CLASS : uint
        {
            SystemHandleInformation = 0x10
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_HANDLE_TABLE_ENTRY_INFO
        {
            public int UniqueProcessId;
            public Byte ObjectType;
            public Byte HandleFlags;
            public UInt16 HandleValue;
            public UIntPtr ObjectPointer;
            public IntPtr AccessMask;
        }
        
        public enum Privilege : int
        {
            SeCreateTokenPrivilege = 1,
            SeAssignPrimaryTokenPrivilege = 2,
            SeLockMemoryPrivilege = 3,
            SeIncreaseQuotaPrivilege = 4,
            SeUnsolicitedInputPrivilege = 5,
            SeMachineAccountPrivilege = 6,
            SeTcbPrivilege = 7,
            SeSecurityPrivilege = 8,
            SeTakeOwnershipPrivilege = 9,
            SeLoadDriverPrivilege = 10,
            SeSystemProfilePrivilege = 11,
            SeSystemtimePrivilege = 12,
            SeProfileSingleProcessPrivilege = 13,
            SeIncreaseBasePriorityPrivilege = 14,
            SeCreatePagefilePrivilege = 15,
            SeCreatePermanentPrivilege = 16,
            SeBackupPrivilege = 17,
            SeRestorePrivilege = 18,
            SeShutdownPrivilege = 19,
            SeDebugPrivilege = 20,
            SeAuditPrivilege = 21,
            SeSystemEnvironmentPrivilege = 22,
            SeChangeNotifyPrivilege = 23,
            SeRemoteShutdownPrivilege = 24,
            SeUndockPrivilege = 25,
            SeSyncAgentPrivilege = 26,
            SeEnableDelegationPrivilege = 27,
            SeManageVolumePrivilege = 28,
            SeImpersonatePrivilege = 29,
            SeCreateGlobalPrivilege = 30,
            SeTrustedCredManAccessPrivilege = 31,
            SeRelabelPrivilege = 32,
            SeIncreaseWorkingSetPrivilege = 33,
            SeTimeZonePrivilege = 34,
            SeCreateSymbolicLinkPrivilege = 35
        }

        [Flags]
        public enum DuplicateOptions : uint
        {
            DUPLICATE_CLOSE_SOURCE = 0x00000001,
            DUPLICATE_SAME_ACCESS = 0x00000002
        }

        [Flags]
        public enum OBJECT_INFORMATION_CLASS
        {
            ObjectBasicInformation = 0,
            ObjectNameInformation = 1,
            ObjectTypeInformation = 2,
            ObjectAllTypesInformation = 3,
            ObjectHandleInformation = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_TYPE_INFORMATION
        { // Information Class 2
            public UNICODE_STRING Name;
            public int ObjectCount;
            public int HandleCount;
            public int Reserved1;
            public int Reserved2;
            public int Reserved3;
            public int Reserved4;
            public int PeakObjectCount;
            public int PeakHandleCount;
            public int Reserved5;
            public int Reserved6;
            public int Reserved7;
            public int Reserved8;
            public int InvalidAttributes;
            public GENERIC_MAPPING GenericMapping;
            public int ValidAccess;
            public byte Unknown;
            public byte MaintainHandleDatabase;
            public int PoolType;
            public int PagedPoolUsage;
            public int NonPagedPoolUsage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SafeFileHandle
        {
            public UNICODE_STRING Name;
        }
}