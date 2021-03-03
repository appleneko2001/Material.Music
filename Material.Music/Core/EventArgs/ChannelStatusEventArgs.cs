using Material.Music.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Material.Music.Core.Engine;
using Material.Music.Core.Enums;

namespace Material.Music.Core.EventArgs
{
    public class ChannelStatusEventArgs : System.EventArgs
    {
        public PlayableBase Playable;
        public BassChannel Channel;
        public ChannelStatusEnums Status;
    }
}
