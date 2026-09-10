namespace FluffyDisdog.UI.Part
{
    public enum SelectableUiType
    {
        NONE=0,
        Card,
        Pack,
        Relic,
        Gold,
        Upgrade
    }
    public interface ISelectableUiPart
    {
        public abstract SelectableUiType Type { get; }
    }
}