namespace Material.Music.Core.Interfaces
{
    public interface IHasClickable
    {
        bool CanClick { get; }
        void OnClicked();
    }
}