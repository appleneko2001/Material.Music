using System;
using System.Runtime.InteropServices;
using ManagedBass;
using Material.Music.Core;
using Material.Music.DynLibHelper;

// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

namespace Material.Music.ManagedBassFix.Tags
{
    public class BassTagsLibrary : IDisposable
    {
        private MinimalDynLibLoader _library;
        private bool _disposed;

        private static BassTagsLibrary _instance;
        public static BassTagsLibrary Instance => _instance;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int TAGS_GetVersion();
        private TAGS_GetVersion _getVersion;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr TAGS_GetLastErrorDesc();
        private TAGS_GetLastErrorDesc _getPrevError;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr TAGS_Read(int Handle, string Format);
        private TAGS_Read _read;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr TAGS_ReadEx(int Handle, string Format, TagType TagType, int CodePage);
        private TAGS_ReadEx _readEx;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool TAGS_SetUTF8(bool Enable);
        private TAGS_SetUTF8 _setUtf8;

        public BassTagsLibrary(string libPath)
        {
            _library = new MinimalDynLibLoader(libPath);
            
            _getVersion = _library.GetDelegate<TAGS_GetVersion>("TAGS_GetVersion");
            _getPrevError = _library.GetDelegate<TAGS_GetLastErrorDesc>("TAGS_GetLastErrorDesc");
            _read = _library.GetDelegate<TAGS_Read>("TAGS_Read");
            _readEx = _library.GetDelegate<TAGS_ReadEx>("TAGS_ReadEx");
            _setUtf8 = _library.GetDelegate<TAGS_SetUTF8>("TAGS_SetUTF8");

            _instance = this;
        }

        /// <summary>
        /// Returns Tags library version.
        /// Current version (in HIBYTE) and build (in LOBYTE).
        /// </summary>
        public int GetVersion()
        {
            ThrowIfDisposed();
            return _getVersion();
        }
        
        /// <summary>
        /// For debug; the text description of the last <see cref="Read(int,string)"/>/<see cref="Read(int,string,TagType,int)"/> call.
        /// It may say something like: "ID3v2 tag: header is invalid", on poorly-added tags.
        /// An empty string is returned if there was no error.
        /// </summary>
        public string GetPrevError()
        {
            ThrowIfDisposed();
            _setUtf8(true);
            return Utils.PtrToStringUtf8(_getPrevError());
        }

        /// <summary>
        /// Reads tag values from the stream and formats them according to given format string.
        /// </summary>
        /// <param name="mediaHandle">BASS handle, obtained normally via CreateStream call, or any other handle, on which <see cref="Bass.ChannelGetTags"/> can be called.</param>
        /// <param name="format">format string.</param>
        /// <returns>
        /// Empty string when unable to properly read the tag, or when there are no supported tags.
        /// A string containing extracted values from the song tags, on success.
        /// A parser error message text, when format string is ill-formed.
        /// </returns>
        /// <remarks>
        /// Ill-formed string causes some error message to appear in the output, but don't count on it too much...
        /// If a file contains APE, ID3v1 and ID3v2 tags, the order of precedence is: APE, ID3v2, ID3v1.
        /// </remarks>
        public string Read(int mediaHandle, string format)
        {
            ThrowIfDisposed();
            _setUtf8(true);
            return Utils.PtrToStringUtf8(_read(mediaHandle, format));
        }
        
        /// <summary>
        /// Reads tag values from the stream and formats them according to given format string.
        /// </summary>
        /// <param name="mediaHandle">BASS handle, obtained normally via CreateStream call, or any other handle, on which <see cref="Bass.ChannelGetTags"/> can be called.</param>
        /// <param name="format">format string.</param>
        /// <param name="tagType">limit processing to a particular tag type, -1 = all tag types.</param>
        /// <param name="codePage">
        /// Codepage to use when reading "ISO-8859-1" tags.
        /// If an invalid/unavailable codepage is requested, then 1252 (Latin 1) will be used.
        /// 0 can be used to request Windows' default codepage.
        /// </param>
        /// <returns>
        /// Empty string when unable to properly read the tag, or when there are no supported tags.
        /// A string containing extracted values from the song tags, on success.
        /// A parser error message text, when format string is ill-formed.
        /// </returns>
        /// <remarks>See <see cref="Read(int,string)"/></remarks>
        public string Read(int mediaHandle, string format, TagType tagType, int codePage)
        {
            ThrowIfDisposed();
            _setUtf8(true);
            return Utils.PtrToStringUtf8(_readEx(mediaHandle, format, tagType, codePage));
        }

        private void ReleaseUnmanagedResources()
        {
            if (!_disposed)
            {
                _disposed = true;
                _library.Dispose();
                _instance = null;
            }
        }
        
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~BassTagsLibrary()
        {
            ReleaseUnmanagedResources();
        }
    }
}