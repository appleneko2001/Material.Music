// ReSharper disable IdentifierTypo
using System;
using System.Runtime.InteropServices;

namespace Material.Music.DynLibHelper
{
    public class UnixLoaderBackend : ILoaderBackend
    {
        private const string BackendLibName = "libdl";
        
        [DllImport(BackendLibName)]
        private static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(BackendLibName)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(BackendLibName)]
        private static extern IntPtr dlerror();
        
        [DllImport(BackendLibName)]
        public static extern int dlclose(IntPtr handle);
        
        public IntPtr Load(string libPath)
        {
            var retVal = dlopen(libPath, 2);
            var errPtr = dlerror();

            if (errPtr != IntPtr.Zero)
                throw new InvalidOperationException(Marshal.PtrToStringAnsi(errPtr));

            return retVal;
        }

        public IntPtr GetFuncPointer(IntPtr libPtr, string funcName) => dlsym(libPtr, funcName);

        public void Free(IntPtr libPtr) => dlclose(libPtr);
    }
}