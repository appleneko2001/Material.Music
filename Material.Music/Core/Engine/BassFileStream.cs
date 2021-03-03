using ManagedBass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Material.Music.Core.Engine
{
    /// <summary>
    /// An simple bass BASS_FILEPROCS class function implements with FileStream base, can be used on local storage only
    /// </summary>
    public class BassFileStream : BassStream, TagLib.File.IFileAbstraction
    {
        /// <summary>
        /// Create a BassFileStream object, and start FileStream with read-only mode
        /// </summary>
        /// <param name="path">File source</param>
        public BassFileStream(Stream stream)
        {
            bass_fs = new FileProcedures() { Close = BassFileClose, Length = BassFileLength, Read = BassFileRead, Seek = BassFileSeek };//new BASS_FILEPROCS(BassFileClose, BassFileLength, BassFileRead, BassFileSeek);

            ReadStream = stream;
        }

        public FileProcedures GetBassFileController() => bass_fs;

        private FileProcedures bass_fs;

        public Stream ReadStream { get; }

        /// <summary>
        /// Do not use it.
        /// </summary>
        public Stream WriteStream => Stream.Null; // read-only stream

        public string Name => "MediaStream";

        private void BassFileClose(IntPtr user) => ReadStream.Close();
        private long BassFileLength(IntPtr user) => ReadStream.Length;
        private int BassFileRead(IntPtr buffer, int length, IntPtr user)
        {
            try
            {
                if (ReadStream.CanRead)
                {
                    byte[] data = new byte[length];
                    int bytesread = ReadStream.Read(data, 0, length);
                    if(bytesread > 0)
                        Marshal.Copy(data, 0, buffer, bytesread);
                    return bytesread;
                }
                else
                {
                    // Returns empty data.
                    Marshal.Copy(Array.Empty<byte>(), 0, buffer, 0);
                    return 0;
                }
            }
            catch (ObjectDisposedException)
            {
                Marshal.Copy(Array.Empty<byte>(), 0, buffer, 0);
                return 0;
            }
        }
        private bool BassFileSeek(long offset, IntPtr user)
        {
            try
            {
                long pos = ReadStream.Seek(offset, SeekOrigin.Begin);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool IsDisposed() => !ReadStream.CanRead;
        public void CloseStream() => ReadStream.Close();
        public void CloseStream(Stream stream) => CloseStream();
    }
}
