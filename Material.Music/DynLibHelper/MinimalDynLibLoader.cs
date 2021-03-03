using System;
using System.Runtime.InteropServices;

namespace Material.Music.DynLibHelper
{
    public class MinimalDynLibLoader : IDisposable
    {
        private static ILoaderBackend _backend;

        static MinimalDynLibLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _backend = new WindowsLoaderBackend();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _backend = new UnixLoaderBackend();
            }
        }

        private IntPtr libPtr;
        
        public MinimalDynLibLoader(string libPath)
        {
            if (_backend is null)
                throw new NotSupportedException("Backend is not initialized. Could not supported on your platform.");

            libPtr = _backend.Load(libPath);

            if (libPtr == IntPtr.Zero)
                throw new NullReferenceException($"Library {libPath} cannot be loaded.");
        }

        public TDelegate GetDelegate<TDelegate>(string funcName)
        {
            var addr = _backend.GetFuncPointer(libPtr, funcName);
            return Marshal.GetDelegateForFunctionPointer<TDelegate>(addr);
        }

        private void ReleaseUnmanagedResources()
        {
            _backend.Free(libPtr);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~MinimalDynLibLoader()
        {
            ReleaseUnmanagedResources();
        }
    }
}