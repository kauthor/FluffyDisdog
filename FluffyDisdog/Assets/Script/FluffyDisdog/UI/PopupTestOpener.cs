using Sirenix.OdinInspector;
using UnityEngine;

namespace FluffyDisdog.UI
{
    public class PopupTestOpener: MonoBehaviour
    {
        [Button]
        private void OpenTestTreasure()
        {
            //if(Application.isPlaying)
               UITreasureSelectPopup.OpenPopup(() => { },1);
        }
    }
}