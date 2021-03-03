using System;
using System.Runtime.InteropServices;

namespace Material.Music.DynLibHelper
{
    public class WindowsLoaderBackend : ILoaderBackend
    {
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);
        
        [DllImport("kernel32")]
        public static extern int FreeLibrary(IntPtr module);

        public IntPtr Load(string libPath) => LoadLibrary(libPath);

        public IntPtr GetFuncPointer(IntPtr libPtr, string funcName) => GetProcAddress(libPtr, funcName);

        public void Free(IntPtr libPtr) => FreeLibrary(libPtr);
    }
}