using ManagedBass;
using System;
using System.Collections.Generic;
using System.Text;
using Material.Music.Core.Enums;
using Material.Music.Core.EventArgs;

namespace Material.Music.Core.Engine
{
    public sealed class BassChannel
    {
        public static BassChannel Null => new BassChannel(StreamId_Null);
        
        private BassEngine engine;

        public const int StreamId_Null = 0;
        
        private float m_Volume; 

        public float Volume
        {
            get => m_Volume;
            set {
                m_Volume = value;
                setVolume(m_Volume, m_Muted);
            }
        }

        private bool m_Muted;

        public bool Muted
        {
            get => m_Muted;
            set {
                m_Muted = value;
                setVolume(m_Volume, m_Muted);
                sendStatusEvent(m_Muted ? PlayStatusEnums.Muted : PlayStatusEnums.Unmuted);
            }
        }

        public int StreamId { get; private set; }

        public BassChannel(int id)
        {
            StreamId = id;
            engine = BassEngine.GetInstance();
            
            if(id != 0)
                Bass.ChannelSetSync(StreamId, SyncFlags.End, 0, (a, b, c, d) => onSeekReachedEnd()); 
        }

        public void Close()
        {
            if (StreamId == StreamId_Null)
                return;
            removeStreamIdBass();
            GC.SuppressFinalize(this);
        }

        public void Pause()
        {
            Utils.AssertIfFail(Bass.ChannelPause(StreamId));
            sendStatusEvent(PlayStatusEnums.Pause);
        }

        public void Resume()
        {
            Utils.AssertIfFail(Bass.ChannelPlay(StreamId));
            sendStatusEvent(PlayStatusEnums.Play);
        }

        /// <summary>
        /// For close handle file use Close() instead. This method will reset position only.
        /// </summary>
        public void Stop()
        {
            Pause();
            Seek(0.0);
            sendStatusEvent(PlayStatusEnums.Stop);
        }

        public void Restart()
        {
            Seek(0.0);
            Resume();
        }

        public PlaybackState PlaybackState => Bass.ChannelIsActive(StreamId);

        public double GetDuration()
        {
            var length = Bass.ChannelGetLength(StreamId, PositionFlags.Bytes);
            return Bass.ChannelBytes2Seconds(StreamId, length);
        }

        public double GetPosition()
        {
            var position = Bass.ChannelGetPosition(StreamId, PositionFlags.Bytes);
            return Bass.ChannelBytes2Seconds(StreamId, position);
        }

        public void Seek(double pos)
        {
            var position = Bass.ChannelSeconds2Bytes(StreamId, pos);
            Bass.ChannelSetPosition(StreamId, position);
        }

        private void setVolume(float v, bool m)
        {
            Bass.ChannelSetAttribute(StreamId, ChannelAttribute.Volume, v * (m ? 0f : 1f));
        }

        private void removeStreamIdBass()
        {
            Utils.AssertIfFail(Bass.StreamFree(StreamId));  
            engine.CallbackFromChannel(this, new ChannelStatusEventArgs { Status = ChannelStatusEnums.Unloaded }); 
            StreamId = default;
            Volume = default;
        }

        private void onSeekReachedEnd()
        {
            Pause();
            Seek(0.0);
            sendStatusEvent(PlayStatusEnums.ReachedEnd);
        }

        private void sendStatusEvent(PlayStatusEnums status) => engine.CallbackFromChannel(this, new PlayStatusEventArgs() { Status = status });
    }
}
