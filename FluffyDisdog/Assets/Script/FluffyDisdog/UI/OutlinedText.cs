using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using TMPro;
using UnityEngine;

namespace FluffyDisdog.UI
{
    public class OutlinedText:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private TextMeshProUGUI[] outlines;
        
        [SerializeField] private Color color;
        [SerializeField] private string text;
        [SerializeField] private float fontSize;

#if UNITY_EDITOR
        [Button]
        private void UpdateOnInspector()
            => Sync();
#endif

        private void Sync()
        {
            mainText.color = color;
            mainText.text = text;
            mainText.fontSize = fontSize;
            foreach (var outline in outlines)
            {
                outline.text = text;
                outline.fontSize = fontSize;
            }
        }
        
        public void SetColor(Color color)
        => mainText.color = color;

        public void SetText(string text)
        {
            this.text = text;
            Sync();
        }

        public void SetOutlineColor(Color color)
        {
            foreach (var outline in outlines)
                outline.color = color;
        }
    }
}