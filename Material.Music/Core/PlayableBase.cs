using ManagedBass;
using System;
using System.Collections.Generic;
using System.Text;
using Material.Music.Appleneko2001;
using Material.Music.Core.Engine;
using Material.Music.Core.Interfaces;
using Material.Music.Core.MediaInfo;
using Material.Music.ViewModels;

namespace Material.Music.Core
{
    public abstract class PlayableBase : ViewModelBase, IPlayable
    {
        /// <summary>
        /// Do not override it if you try to implementing an class that supports online streaming or from online resource.
        /// </summary>
        public virtual bool IsLocalMedia { get; internal set; } = false;


        public virtual bool Ready { get; internal set; } = false;

        /// <summary>
        /// Replace it by override. By default it should be known after get the file header and send it to Analyzer.Analyze(). 
        /// </summary>
        public virtual AudioHeaderData MediaFormat { get; internal set; } = AudioHeaderData.Unsupported;

        /// <summary>
        /// Track duration. It could be known if file are mp3 container and get from TagLib.
        /// </summary>
        public virtual TimeSpan Duration { get; internal set; } = TimeSpan.Zero;

        public virtual string DurationString { get; internal set; } = "00:00:00";

        /// <summary>
        /// Track information (Title, artist, album etc.). It could be known if file are mp3 container and get from TagLib. Other types I didn't tried.
        /// </summary>
        public virtual IMediaInfo TrackInfo { get; internal set; } = null;

        /// <summary>
        /// Replace it by override. By default it looks like "public override string Title => TrackInfo.Title"
        /// </summary>
        public virtual string Title { get; internal set; } = "** Playable Base **";

        public virtual string FaultReason => m_FaultReason;

        public virtual string Identicator => "/\\Unknown/\\";

        /// <summary>
        /// Require to implement it yourself, otherwize will throw an <see cref="NotImplementedException"/>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void CheckStatus()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Require to implement it yourself, otherwize will throw an <see cref="NotImplementedException"/>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Close(bool completelyDispose = false)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Require to implement it yourself, otherwize will throw an <see cref="NotImplementedException"/>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual BassFileStream GetBassStream()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Require to implement it yourself, otherwize will throw an <see cref="NotImplementedException"/>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual string GetMediaPath()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Require to implement it yourself, otherwize will throw an <see cref="NotImplementedException"/>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual string GetObjectHash()
        {
            throw new NotImplementedException();
        }

        public virtual void SetCorruptedState(bool v, Exception errorInfo)
        {
            m_FaultReason = v ? errorInfo.Message : "";
            OnPropertyChanged(nameof(FaultReason));
        }

        private string m_FaultReason;


    }
}
