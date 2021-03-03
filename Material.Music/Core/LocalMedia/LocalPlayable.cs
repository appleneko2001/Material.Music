using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Material.Dialog;
using Material.Music.Appleneko2001;
using Material.Music.Core.Engine;
using Material.Music.Core.MediaInfo;
using Material.Music.Core.Logging;

namespace Material.Music.Core.LocalMedia
{
    public class LocalPlayable : PlayableBase
    {
        #region Constructor
        public LocalPlayable(FileInfo file) : this(file.FullName)
        {

        }

        /// <summary>
        /// Create object and initializes stream for reading file
        /// </summary>
        /// <param name="filePath">Target file source, it should be existed otherwise will return Exception.</param>
        public LocalPlayable(string filePath)
        {
            m_Path = filePath;
            Load();
        }
        /// <summary>
        /// Create instance on safe way
        /// </summary>
        /// <param name="filePath">The track file path (can be absolute or relative)</param>
        /// <param name="result">Out a instance, otherwise null if fail</param>
        /// <returns>Return true if everything is ok.</returns>
        public static bool TryCreate(string filePath, out LocalPlayable result)
        {
            var header = Analyzer.Analyze(filePath);
            if (header == AudioHeaderData.Unsupported || header == AudioHeaderData.Error)
            {
                result = null;
                return false;
            }
            result = new LocalPlayable(filePath);
            return true;
        }
        #endregion
        /// <summary>
        /// Get track information while instantiate and analyze file header. 
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            try
            {
                if (!File.Exists(m_Path))
                    throw new FileNotFoundException(string.Format("The file is not available or it doesn't exist", m_Path), m_Path);
                MediaFormat = Analyzer.Analyze(m_Path);
                if (MediaFormat == AudioHeaderData.Unsupported)
                    throw new InvalidDataException(m_Path);
                else if (MediaFormat == AudioHeaderData.Error)
                    throw Analyzer.GetLastError();
                m_Stream = new BassFileStream(new FileStream(m_Path, FileMode.Open));
                {
                    m_Hash = Utils.CalculateMurmur3Hash(m_Stream.ReadStream);
                    TrackInfo = new BassTagMediaInfo(this);
                }
                m_Stream.CloseStream();
                if (string.IsNullOrEmpty(TrackInfo.Title)) // Take the file name instead if we can't get the title info from tags
                {
                    m_FileName = Path.GetFileNameWithoutExtension(m_Path); 
                }
                Ready = true;
                return true;
            }
            catch (Exception e)
            {
                // File could be used by another process, give it a file name if can
                if (File.Exists(m_Path))
                {
                    m_FileName = Path.GetFileNameWithoutExtension(m_Path);
                }
                else
                {
                    SetCorruptedState(true, e);
                }
                //Logger.LogError(e, true);
                //ExceptMessage.PrintConsole(3, $"An error occurred when loading media: {e.Message}\r\n{e.StackTrace}\r\nMedia will unavailable until media is returned to online state (Is ready to playback).");
            }
            Ready = false;
            return false;
        }

        #region Overrides
        #region Properties
        public override string Title => TrackInfo?.Title ?? m_FileName;
        public override bool Ready => File.Exists(m_Path);

        public override string DurationString => Duration.ToString(@"hh\:mm\:ss");
        public override bool IsLocalMedia => true;
        #endregion
        /// <summary>
        /// Try to know file are readable or it is playable by host. 
        /// </summary>
        public override void CheckStatus() => Load();

        /// <summary>
        /// Get track file hash. Used for identity of file.
        /// </summary>
        /// <returns></returns>
        public override string GetObjectHash() => m_Hash;

        public override string Identicator => m_Hash;

        /// <summary>
        /// Get tunnel object with access to this file for Bass host.
        /// </summary>
        /// <returns></returns>
        public override BassFileStream GetBassStream()
        {
            if (m_Stream == null || m_Stream.IsDisposed())
            {
                m_Stream = new BassFileStream(new FileStream(m_Path, FileMode.Open));
                if (m_Stream.IsDisposed())
                    throw new ArgumentNullException(null, "Stream not readable, cannot provide stream to read media.");
            }
            return m_Stream;
        }
        /// <summary>
        /// Get file absolute path.
        /// </summary>
        /// <returns>An absolute path to this file</returns>
        public override string GetMediaPath() => m_Path;

        /// <summary>
        /// Close the stream tunnel. Pass true argument to fully clear this instance.
        /// </summary>
        /// <param name="completelyDispose"></param>
        public override void Close(bool completelyDispose = false)
        {
            if (m_Stream != null)
            {
                m_Stream.CloseStream();
                m_Stream = null;
            }
            if (completelyDispose)
            {
                m_Stream = null;
                MediaFormat = AudioHeaderData.Unsupported;
                Duration = TimeSpan.Zero;
                TrackInfo = null;
                m_Path = null;
                m_Hash = null;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region Private members
        private string m_FileName;
        private string m_Path;
        private string m_Hash;
        private BassFileStream m_Stream;
        #endregion


    }

}
