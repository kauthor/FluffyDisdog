namespace FluffyDisdog.UI
{
    public class UILoadingPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.Loading;

        public static UILoadingPopup NormalLoadStart()
        {
            var pop = PopupManager.I.GetPopup(PopupType.Loading);
            if (pop is UILoadingPopup stg)
            {
                stg.gameObject.SetActive(true);
                return stg;
            }

            return null;
        }
    }
}