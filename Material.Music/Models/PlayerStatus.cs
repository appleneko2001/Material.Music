using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Material.Music.Core.Containers;
using Material.Music.ViewModels;

namespace Material.Music.Models
{
    public class PlayerStatus : JsonDataContainer<PlayerStatus>
    {
        [JsonIgnore]
        public override string JsonSaveSubdir => "";

        [JsonIgnore]
        public override string JsonSaveFilename => "PlayerStatus.json"; 
        public PlayerStatus()
        {
            SetTargetSaves(this); 
        }

        public int Volume { get; set; } = 50;

        public bool Muted { get; set; } = false;

        public bool Shuffle { get; set; } = false;

        public int RepeatMode { get; set; } = PlayerContext.Repeat_NoRepeat;

        public string PreviousMediaId { get; set; } = null;

        public string PlaylistId { get; set; } = null;

        // Window sizes
        
        public double WindowWidth { get; set; }

        public double WindowHeight { get; set; }
    }
}
