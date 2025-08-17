using System.Runtime.InteropServices;

namespace VMemLib.utils;

public static class conversion
{
    public static float[] ToFloats(this byte[] bytes)
    {
        if (bytes.Length % 4 != 0) throw new ArgumentException();
        float[] floats = new float[bytes.Length / 4];
        for (int i = 0; i < floats.Length; i++) floats[i] = BitConverter.ToSingle(bytes, i * 4);
        return floats;
    }

    public static T AsStruct<T>(this byte[] bytes) where T : struct
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)); }
        finally { handle.Free(); }
    }

    public static byte[] AsBytes(this object obj)
    {
        int length = Marshal.SizeOf(obj);
        byte[] array = new byte[length];
        IntPtr pointer = Marshal.AllocHGlobal(length);
        Marshal.StructureToPtr(obj, pointer, true);
        Marshal.Copy(pointer, array, 0, length);
        Marshal.FreeHGlobal(pointer);
        return array;
    }
}