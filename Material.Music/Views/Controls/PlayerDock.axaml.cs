using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Material.Music.ViewModels;
using Material.Music.Models;

namespace Material.Music.Views.Controls
{
    public class PlayerDock : UserControl
    {
        private PlayerContext Context;

        private bool _draggingSlider = false;
        private bool _thumbButtonUpToResume = false;

        private Slider SeekBarSlider ;

        public PlayerDock()
        {
            this.InitializeComponent();
            DataContext = Context = PlayerContext.GetInstance();

            SeekBarSlider = this.Get<Slider>("SeekBarSlider");
            SeekBarSlider.AddHandler(TemplateAppliedEvent, (a, b) => SeekBarSliderOnTemplateApplied(SeekBarSlider, b)); 
        }

        private void SeekBarSliderOnTemplateApplied(object sender, TemplateAppliedEventArgs e)
        {
            var slider = (Slider)sender;
            var thumb = GetSliderThumb(slider, e);  
            thumb.AddHandler(Thumb.DragStartedEvent, (s, e) => ThumbMoveStart(), RoutingStrategies.Tunnel);
            thumb.AddHandler(Thumb.DragCompletedEvent, (s, e) => ThumbMoveEnd(), RoutingStrategies.Tunnel); 
            
            var track = GetSliderTrack(slider, e); 
            track.AddHandler(PointerPressedEvent, (s, e)=> ThumbMoveStart(), RoutingStrategies.Tunnel);
            track.AddHandler(PointerReleasedEvent, (s, e) => ThumbMoveEnd(), RoutingStrategies.Tunnel);
        }
          
        private void ThumbMoveStart()
        {
            if (Context.MediaChannel is null)
                return;
            var channel = Context.MediaChannel;

            if (!_draggingSlider)
            {
                _thumbButtonUpToResume = channel.PlaybackState == ManagedBass.PlaybackState.Playing;
                channel.Pause();
            }
            _draggingSlider = true;
        }

        private void ThumbMoveEnd()
        {
            if (Context.MediaChannel is null)
                return;
            var channel = Context.MediaChannel;

            if (_draggingSlider)
            {
                _draggingSlider = false;
                channel?.Seek(Context.CurrentPosition);
                if (_thumbButtonUpToResume)
                    channel.Resume();
            }
        } 

        private Thumb GetSliderThumb(Slider obj, TemplateAppliedEventArgs e) => GetSliderTrack(obj, e)?.Thumb;

        private Track GetSliderTrack(Slider obj, TemplateAppliedEventArgs e) => e.NameScope.Find<Track>("PART_Track");

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
