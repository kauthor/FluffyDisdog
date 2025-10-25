using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class BinaryGoldPrefab:MonoBehaviour
    {
        [SerializeField] private Sprite[] binarySprites;

        [SerializeField] private Image RenderArea;

        public void SetBinary(int num)
        {
            int index = 0;
            if (num <= 9 && num >= 0)
            {
                index = num;
            }
            
            RenderArea.sprite = binarySprites[index];
        }
    }
}