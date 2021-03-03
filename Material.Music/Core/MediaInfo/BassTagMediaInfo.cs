using Material.Music.Appleneko2001;
using ManagedBass;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Material.Music.Core.Interfaces;
using Material.Music.ManagedBassFix.Tags;
using Material.Music.ViewModels;

namespace Material.Music.Core.MediaInfo
{
    public class BassTagMediaInfo : ViewModelBase, IMediaInfo
    {
        #region Constructor
        public BassTagMediaInfo()
        {

        }

        public BassTagMediaInfo(IPlayable media)
        {
            var channelId = Bass.CreateStream(StreamSystem.NoBuffer, BassFlags.Default, media.GetBassStream().GetBassFileController());
            if (channelId != 0)
            { 
                ReadTags(channelId);
            }
            Bass.StreamFree(channelId);
        }

        public BassTagMediaInfo(int channelId)
        {
            ReadTags(channelId);
        }
        #endregion

        #region Public methods
        #endregion

        #region Properties
        public string TrackId { get => m_TrackId; set { m_TrackId = value; NotifyUpdate(); } }
        public string Artist { get => m_Artist; set { m_Artist = value; NotifyUpdate(); } }
        public string AlbumArtist { get => m_ArtistAlbum; set { m_ArtistAlbum = value; NotifyUpdate(); } }
        public string Album { get => m_Album; set { m_Album = value; NotifyUpdate(); } }
        public string Year { get => m_Year; set { m_Year = value; NotifyUpdate(); } }
        public string Genre { get => m_Genre; set { m_Genre = value; NotifyUpdate(); } }
        public string Title { get => m_Title; set { m_Title = value; NotifyUpdate(); } }
        public string TagProvider { get => m_TagProvider; set { m_TagProvider = value; NotifyUpdate(); } }

        #endregion

        #region Private members
        private readonly PlayableBase m_ThisPlayable;
        private string m_TrackId;
        private string m_Artist;
        private string m_ArtistAlbum;
        private string m_Album;
        private string m_Year;
        private string m_Genre;
        private string m_Title;
        private string m_TagProvider;
        #endregion

        #region Private methods
        private void NotifyUpdate([CallerMemberName] string memberName = null)
        {
            this.OnPropertyChanged(memberName);
        }

        private void ReadTags(int channelId)
        {
            var BassTags = BassTagsLibrary.Instance;
            
            Artist = BassTags.Read(channelId, "%ARTI");
            Title = BassTags.Read(channelId, "%TITL");
            Album = BassTags.Read(channelId, "%ALBM");
            Genre = BassTags.Read(channelId, "%GNRE");
            AlbumArtist = BassTags.Read(channelId, "%AART");
        }
        #endregion
         
        public override string ToString()
        {
            var album = Album ?? "Unknown Album";
            var artist = Artist ?? "Unknown Artist";
            return $"{album} - {artist}";
        }
    }
}
