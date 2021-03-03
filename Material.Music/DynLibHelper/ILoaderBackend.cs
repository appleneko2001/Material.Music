using System;

namespace Material.Music.DynLibHelper
{
    public interface ILoaderBackend
    {
        IntPtr Load(string libPath);
        IntPtr GetFuncPointer(IntPtr libPtr, string funcName);
        void Free(IntPtr libPtr);
    }
}