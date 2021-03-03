// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

using Material.Music.Core;

namespace Material.Music
{
    using ManagedBass;
    using Music.Core;
    using System;
    using System.IO;

    namespace Appleneko2001
    { 
        public enum AudioHeaderData
        {
            MPEG3, //mp3 file, but without ID3 Tag
            MPEG3_ID3v2, //regular mp3 file
            FLAC, //FLAC lossless 
            WindowsMedia, // WMA, WMV
            AppleM4A, //M4A file from apple or iTunes
            MPEG4Video, //M4A file but with MPEG4 Video header
            Waveform, //WAV file
            Unsupported, //Unsupposed type modern audio file, or is not a audio file
            Error //Error occurred when analyzing file type
        }
        public static class Analyzer
        {
            #region Header identicators
            private static readonly byte[] Header_MPEG3 = new byte[] { 0xFF, 0xFB };
            private static readonly byte[] Header_MPEG3_ID3v2 = new byte[] { 0x49, 0x44, 0x33 };
            private static readonly byte[] Header_FLAC = new byte[] { 0x66, 0x4C, 0x61, 0x43 };
            private static readonly byte[] Header_WindowsMedia = new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF };
            private static readonly byte[] Header_AppleM4A = new byte[] { 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20 };
            private static readonly byte[] Header_MP4A = new byte[] { 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32 };
            private static readonly byte[] Header_Waveform = new byte[] { 0x57, 0x41, 0x56, 0x45 };
            #endregion
            private static Exception prevError;
            public static Exception GetLastError() { return prevError; } 
            public static AudioHeaderData Analyze(string path)
            {
                if (!File.Exists(path))
                {
                    return AudioHeaderData.Unsupported;
                }
                try
                {
                    var header = new byte[32]; //A bytes group that can store header from file
                    using (var file = File.OpenRead(path))
                    {
                        file.Read(header, 0, header.Length);
                    }
                    if (Utils.CompareBytes(header, Header_MPEG3, 0, 2))
                        return AudioHeaderData.MPEG3;
                    else if (Utils.CompareBytes(header, Header_MPEG3_ID3v2, 0, 3))
                        return AudioHeaderData.MPEG3_ID3v2;
                    else if (Utils.CompareBytes(header, Header_FLAC, 0, 4))
                        return AudioHeaderData.FLAC;
                    else if (Utils.CompareBytes(header, Header_WindowsMedia, 0, 7))
                        return AudioHeaderData.WindowsMedia;
                    else if (Utils.CompareBytes(header, Header_AppleM4A, 0x00000004, 8))
                        return AudioHeaderData.AppleM4A;
                    else if (Utils.CompareBytes(header, Header_MP4A, 0x00000004, 8))
                        return AudioHeaderData.MPEG4Video;
                    else if (Utils.CompareBytes(header, Header_Waveform, 0x00000008, 4))
                        return AudioHeaderData.Waveform;
                    else
                        return AudioHeaderData.Unsupported;
                }
                catch (Exception e)
                {
                    prevError = e;
                    return AudioHeaderData.Error;
                }
            }
            public static string GetMimetypeString(AudioHeaderData type)
            {
                switch (type)
                {
                    case AudioHeaderData.MPEG4Video:
                    case AudioHeaderData.AppleM4A:
                        return "audio/mp4";
                    case AudioHeaderData.MPEG3:
                    case AudioHeaderData.MPEG3_ID3v2:
                        return "audio/mpeg";
                    case AudioHeaderData.FLAC:
                        return "audio/flac";
                    case AudioHeaderData.Waveform:
                        return "audio/x-wav";
                    case AudioHeaderData.WindowsMedia:
                        return "audio/x-ms-wma";
                    case AudioHeaderData.Unsupported:
                    case AudioHeaderData.Error:
                        return null;
                }
                return null;
            }
        
            public static TagType? GetBassTagType(AudioHeaderData type)
            {
                switch (type)
                { 
                    case AudioHeaderData.MPEG3_ID3v2:
                        return TagType.ID3v2;
                    case AudioHeaderData.MPEG4Video:
                    case AudioHeaderData.AppleM4A:
                        return TagType.MP4;
                    case AudioHeaderData.Waveform:
                        return TagType.WaveFormat;
                    case AudioHeaderData.WindowsMedia:
                        return TagType.WMA;

                    // Currently I didnt see basstags are supported vorbis comments, keep returns null
                    case AudioHeaderData.FLAC:

                    // MPEG with FF FB headers are means they doesnt have any tags.
                    case AudioHeaderData.MPEG3:

                    default:
                        return null;
                }
            }
        }
    }

}
