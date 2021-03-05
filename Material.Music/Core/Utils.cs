using ManagedBass;
using MurMur3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Material.Music.Core.Interfaces;

namespace Material.Music.Core
{
    public static class Utils
    {
        /// <summary>
        /// Compare file magic header
        /// </summary>
        /// <param name="origin">File magic header bytes</param>
        /// <param name="target">Target bytes for compare</param>
        /// <param name="offset_origin">Start position, default are zero</param>
        /// <param name="count">Compare length.</param>
        /// <returns>Returns true if match, otherwize will return false.</returns>
        public static bool CompareBytes(byte[] origin, byte[] target, int offset_origin = 0, int count = 1)
        {
            if (target is null || origin is null)
                return true;
            bool result = true;
            for (int i = offset_origin, t = 0; (t < target.Length && i < origin.Length && t < count); i++, t++)
            {
                if (origin[i] != target[t])
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public static string GetFullDuration(this ObservableCollection<IPlayable> collection)
        {
            if (collection is null)
                return "#!Null arguments";
            try
            {
                TimeSpan duration = TimeSpan.Zero;
                foreach (var track in collection)
                {
                    if (track.Ready)
                        duration += track.Duration;
                }
                return duration.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                return "#!" + e.Message;
            }
        }

        /// <summary>
        /// Show error code when Bass Library returned a false boolean value.
        /// </summary>
        /// <param name="bassCommand">Command from Un4seen.Bass unmanaged wrapper</param>
        /// <param name="isCritical">Is the error in the critical level? Default value is false (warning)</param>
        /// <param name="exception">Show all error but except defined in parameter exception.</param>
        public static void AssertIfFail(bool bassCommand, bool isCritical = false, Errors[] exception = null)
        {
            if (!bassCommand)
            {
                var code = Bass.LastError;
                if (exception != null)
                    if (exception.Contains(code))
                        return;
                //ExceptMessage.PopupExcept(code, isCritical);
            }
        }

        /// <summary>
        /// Calculate hash with the Murmur3 fast algorithm. For more information about this algorithm, just visit the page: http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string CalculateMurmur3Hash(Stream stream)
        {
            const int SizeForCalculateHash = 1024 * 8; // 8192 bytes will taken for calculate hash.

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanSeek)
                throw new NotSupportedException("Stream can't to seek, unable to seek old position.");

            using (var memory = new MemoryStream(new byte[SizeForCalculateHash], 0, SizeForCalculateHash, true, publiclyVisible: true))
            {
                stream.CopyTo(memory, 0, SizeForCalculateHash);
                var hash = new Murmur3HashCalculation();
                var final = hash.ComputeHash(memory.GetBuffer());
                return BitConverter.ToString(final).Replace("-", "");
            }
        }

        public static void CopyTo(this Stream origin, Stream destination, int offset = 0, int length = -1, int bufferSize = 1024, bool restorePos = true)
        {
            long destPos = destination.Position, originPos = origin.Position;
          
            byte[] buffer = new byte[bufferSize];
            int read;
            while (length > 0 && (read = origin.Read(buffer, offset, Math.Min(buffer.Length, length))) > 0){
                destination.Write(buffer, 0, read);
                length -= read;
            }
            if (restorePos)
            {
                destination.Position = destPos;
                origin.Position = originPos;
            }
        }

        public static string CalculateMurmur3Hash(byte[] data)
        {
            var hash = new Murmur3HashCalculation();
            var final = hash.ComputeHash(data);
            return BitConverter.ToString(final);
        }

        public static string ReturnNullIfEmptyAndDeleteNewLines(this string str) => 
            string.IsNullOrWhiteSpace(str) ? null : str.Replace("\n", " ").Replace("\r", " ");

        public static void OpenBrowserForVisitSite(string link)
        {
            var param = new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(param);
        }

        public static string ConvertFilterToPatterns(string[] filter)
        { 
            string result = $".{filter.First()}";
            if(filter.Length > 1)
            {
                for(int i = 1; i < filter.Length; i++)
                    result = $"{result}|.{filter[i]}";
            } 
            return result;
        }

        public static string[] EnumerateFiles(string path, string pattern)
        {
            List<string> resultList = new List<string>();
            var enumerated = Directory.EnumerateFiles(path);
            foreach (var item in pattern.Split('|'))
            {
                resultList.AddRange(enumerated.Where(s => s.EndsWith(item)));
            }
            return resultList.ToArray();
        }

        // https://github.com/ppy/osu-framework/blob/4e069ae691db268d4e7659580b6c351470cf304f/osu.Framework/Extensions/IEnumerableExtensions/EnumerableExtensions.cs
        /// <summary>
        /// Performs an action on all the items in an IEnumerable collection.
        /// </summary>
        /// <typeparam name="T">The type of the items stored in the collection.</typeparam>
        /// <param name="collection">The collection to iterate on.</param>
        /// <param name="action">The action to be performed.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) return;

            foreach (var item in collection)
                action(item);
        }

        /// <summary>
        /// Create a copy and performs an action to them.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        public static void DynamicForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) return;

            var copy = collection.ToArray();

            foreach (var item in copy)
                action(item);
        }

        public static string GetProgramParentDir()
        {
            var path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            if (path != AppDomain.CurrentDomain.BaseDirectory)
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            return path;
        }
        
        public static class WindowsServices
        {
            const int WS_EX_TRANSPARENT = 0x00000020;
            const int GWL_EXSTYLE = (-20);
            private const int WS_EX_LAYERED = 0x00080000;

            [DllImport("user32.dll")]
            static extern int GetWindowLong(IntPtr hwnd, int index);

            [DllImport("user32.dll")]
            static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

            public static void SetWindowHitTestVisible(IntPtr handle, bool value)
            {
                var extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);
                var style = value ? (extendedStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED ) : (extendedStyle & ~WS_EX_TRANSPARENT & ~WS_EX_LAYERED );
                SetWindowLong(handle, GWL_EXSTYLE, style);
            }
        }

        private static unsafe string PtrToStringUtf8(IntPtr ptr, out int size)
        {
            size = 0;

            var bytes = (byte*)ptr.ToPointer();

            if (ptr == IntPtr.Zero || bytes[0] == 0)
                return null;

            while (bytes[size] != 0)
                ++size;

            var buffer = new byte[size];
            Marshal.Copy(ptr, buffer, 0, size);

            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Returns a Unicode string from a pointer to a Utf-8 string.
        /// </summary>
        public static string PtrToStringUtf8(IntPtr ptr)
        {
            return PtrToStringUtf8(ptr, out var size);
        }
        
        public static string PlatformSpecificLibName(string name) => 
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"{name}.dll" :
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? $"lib{name}.so" :
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"lib{name}.dylib" : $"{name}";
        
        public static void CreateIfNotExistDir(string targetPath)
        {
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
        }
    }

    //https://stackoverflow.com/questions/2946954/make-listview-scrollintoview-scroll-the-item-into-the-center-of-the-listview-c
    
    public static class ItemsControlExtensions
    {
        public static void ScrollToCenterOfView(this ItemsControl itemsControl, ScrollViewer scrollViewer, int index)
        {
            Dispatcher.UIThread.InvokeAsync(() => itemsControl.TryScrollToCenterOfView(scrollViewer, index),
                DispatcherPriority.Loaded);
        }

        private static bool TryScrollToCenterOfView(this ItemsControl itemsControl, ScrollViewer scrollViewer, int index)
        {
            var container =
                itemsControl.ItemContainerGenerator.ContainerFromIndex(index);
            if (container == null) return false;

            var size = container.DesiredSize;
            var viewport = scrollViewer.Viewport;
            var centerMatrix = container.TranslatePoint(new Point(0, 0), itemsControl);

            if (centerMatrix.HasValue)
            {
                var x = centerMatrix.Value.X - viewport.Width / 2;
                var y = centerMatrix.Value.Y - viewport.Height / 2;
            
                scrollViewer.Offset = new Vector(x, y);
            }
            return true;
        }
    }
}




namespace MurMur3
{

    public static class IntHelpers
    {
        public static ulong RotateLeft(this ulong original, int bits)
        {
            return (original << bits) | (original >> (64 - bits));
        }

        public static ulong RotateRight(this ulong original, int bits)
        {
            return (original >> bits) | (original << (64 - bits));
        }

        public static ulong GetUInt64(this byte[] bb, int pos)
        {
            // Thanks iron9light provided a improved version for function GetUInt64
            // Source: http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html?showComment=1354868480919#c2111475095152723700
            return BitConverter.ToUInt64(bb, pos);
        }
    }
    public class Murmur3HashCalculation
    {
        // 128 bit output, 64 bit platform version

        public const ulong READ_SIZE = 16;
        private static ulong C1 = 0x87c37b91114253d5L;
        private static ulong C2 = 0x4cf5ad432745937fL;

        private ulong length;
        private const uint seed = 24; // if want to start with a seed, create a constructor
        private ulong h1;
        private ulong h2;

        private void MixBody(ulong k1, ulong k2)
        {
            h1 ^= MixKey1(k1);

            h1 = h1.RotateLeft(27);
            h1 += h2;
            h1 = h1 * 5 + 0x52dce729;

            h2 ^= MixKey2(k2);

            h2 = h2.RotateLeft(31);
            h2 += h1;
            h2 = h2 * 5 + 0x38495ab5;
        }
        private static ulong MixKey1(ulong k1)
        {
            k1 *= C1;
            k1 = k1.RotateLeft(31);
            k1 *= C2;
            return k1;
        }
        private static ulong MixKey2(ulong k2)
        {
            k2 *= C2;
            k2 = k2.RotateLeft(33);
            k2 *= C1;
            return k2;
        }
        private static ulong MixFinal(ulong k)
        {
            // avalanche bits

            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdL;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53L;
            k ^= k >> 33;
            return k;
        }
        public byte[] ComputeHash(byte[] bb)
        {
            if (bb is null)
                ProcessBytes(Array.Empty<byte>());
            else
                ProcessBytes(bb);
            return GetHash();
        }
        private void ProcessBytes(byte[] bb)
        {
            h1 = seed;
            this.length = 0L;

            int pos = 0;
            ulong remaining = (ulong)bb.Length;

            // read 128 bits, 16 bytes, 2 longs in eacy cycle
            while (remaining >= READ_SIZE)
            {
                ulong k1 = bb.GetUInt64(pos);
                pos += 8;

                ulong k2 = bb.GetUInt64(pos);
                pos += 8;

                length += READ_SIZE;
                remaining -= READ_SIZE;

                MixBody(k1, k2);
            }

            // if the input MOD 16 != 0
            if (remaining > 0)
                ProcessBytesRemaining(bb, remaining, pos);
        }
        private void ProcessBytesRemaining(byte[] bb, ulong remaining, int pos)
        {
            ulong k1 = 0;
            ulong k2 = 0;
            length += remaining;

            // little endian (x86) processing
            switch (remaining)
            {
                case 15:
                    k2 ^= (ulong)bb[pos + 14] << 48; // fall through
                    goto case 14;
                case 14:
                    k2 ^= (ulong)bb[pos + 13] << 40; // fall through
                    goto case 13;
                case 13:
                    k2 ^= (ulong)bb[pos + 12] << 32; // fall through
                    goto case 12;
                case 12:
                    k2 ^= (ulong)bb[pos + 11] << 24; // fall through
                    goto case 11;
                case 11:
                    k2 ^= (ulong)bb[pos + 10] << 16; // fall through
                    goto case 10;
                case 10:
                    k2 ^= (ulong)bb[pos + 9] << 8; // fall through
                    goto case 9;
                case 9:
                    k2 ^= bb[pos + 8]; // fall through
                    goto case 8;
                case 8:
                    k1 ^= bb.GetUInt64(pos);
                    break;
                case 7:
                    k1 ^= (ulong)bb[pos + 6] << 48; // fall through
                    goto case 6;
                case 6:
                    k1 ^= (ulong)bb[pos + 5] << 40; // fall through
                    goto case 5;
                case 5:
                    k1 ^= (ulong)bb[pos + 4] << 32; // fall through
                    goto case 4;
                case 4:
                    k1 ^= (ulong)bb[pos + 3] << 24; // fall through
                    goto case 3;
                case 3:
                    k1 ^= (ulong)bb[pos + 2] << 16; // fall through
                    goto case 2;
                case 2:
                    k1 ^= (ulong)bb[pos + 1] << 8; // fall through
                    goto case 1;
                case 1:
                    k1 ^= bb[pos]; // fall through
                    break;
                default:
                    throw new Exception("Something went wrong with remaining bytes calculation.");
            }

            h1 ^= MixKey1(k1);
            h2 ^= MixKey2(k2);
        }
        public byte[] GetHash()
        {
            h1 ^= length;
            h2 ^= length;

            h1 += h2;
            h2 += h1;

            h1 = MixFinal(h1);
            h2 = MixFinal(h2);

            h1 += h2;
            h2 += h1;

            var hash = new byte[READ_SIZE];

            Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
            Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);

            return hash;
        }
    }
}