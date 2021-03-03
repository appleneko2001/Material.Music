using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Material.Music.Core.MediaInfo
{
    public class TaglibMediaInfo : ReactiveObject, IMediaInfo
    {
        #region Constructor
        /// <summary>
        /// Parse data from Taglib Audio Tag
        /// </summary>
        /// <param name="tag">Instance Taglib tag object</param>
        public TaglibMediaInfo(TagLib.Tag tag, PlayableBase playable)
        {
            m_ThisPlayable = playable;
            if (tag is null)
                throw new ArgumentNullException(nameof(tag));

            Parse(tag);
            NotifyUpdate();
        }
        /// <summary>
        /// Just create a empty tag container, you can fill infos later with other source.
        /// </summary>
        public TaglibMediaInfo()
        {

        }
        #endregion

        public void ApplyMetadata(TagLib.Tag metadata) => Parse(metadata);

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

        #region Private methods
        private void Parse(TagLib.Tag tag)
        { 
            Artist = tag.FirstPerformer.ReturnNullIfEmptyAndDeleteNewLines();
            AlbumArtist = tag.FirstAlbumArtist.ReturnNullIfEmptyAndDeleteNewLines();
            Album = tag.Album.ReturnNullIfEmptyAndDeleteNewLines();
            Year = (tag.Year == 0) ? null : tag.Year.ToString((IFormatProvider)null); // Do not use "0" or similar thing if we don't know, just keep it "null"
            Genre = tag.FirstGenre.ReturnNullIfEmptyAndDeleteNewLines();
            Title = tag.Title.ReturnNullIfEmptyAndDeleteNewLines();
            if (tag.Track != 0)
                TrackId = tag.Track.ToString();
        } 
        private void NotifyUpdate([CallerMemberName] string memberName = null)
        {
            this.RaisePropertyChanged(memberName);
        }
        #endregion
        #region Private members
        private readonly PlayableBase m_ThisPlayable;
        private long? songId, albumImageId;
        private string m_TrackId;
        private string m_Artist;
        private string m_ArtistAlbum;
        private string m_Album;
        private string m_Year;
        private string m_Genre;
        private string m_Title;
        private string m_TagProvider;
        #endregion

        public override string ToString()
        {
            string album = Album ?? "Unknown Album";
            string artist = Artist ?? "Unknown Artist";
            return $"{album} - {artist}";
        }
    }
}
