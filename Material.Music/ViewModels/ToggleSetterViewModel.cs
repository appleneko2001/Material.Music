using System;
using Material.Icons;
using Material.Music.Core.Interfaces;
using Material.Styles;

namespace Material.Music.ViewModels
{
    public class ToggleSetterViewModel : BaseSetterViewModel , IHasClickable
    {
        public ToggleSetterViewModel(string? title, string? caption = null, 
            MaterialIconKind? icon = null, string saverId = null, bool defaultValue = false, Func<bool, bool> canClick = null,
            Action<bool> onValueChanged = null) : base(title, caption, icon)
        {
            _saverId = saverId;
            SetterValue = defaultValue;
            OnValuePropertyChanged = onValueChanged;

            if (canClick == null)
            {
                OnCanClickCalled = canClick;
            }
        }

        private string? _saverId;
        
        private bool _setterValue;
        public bool SetterValue
        {
            get => _setterValue;
            set { _setterValue = value; OnPropertyChanged(); OnValuePropertyChanged?.Invoke(value);}
        }

        public Action<bool> OnValuePropertyChanged;
        public Func<bool, bool> OnCanClickCalled;

        public bool CanClick => OnCanClickCalled?.Invoke(_setterValue) ?? true;

        public void OnClicked()
        {
            if(!CanClick)
                return;
            SetterValue = !SetterValue;
        }
    }
}