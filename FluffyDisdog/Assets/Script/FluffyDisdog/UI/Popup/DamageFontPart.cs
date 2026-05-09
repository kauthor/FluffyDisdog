using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace FluffyDisdog.UI
{
    public class DamageFontPart:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI[] damageOutlines;
        [SerializeField] private float duration = 1.0f;


        public async UniTask Show(int amount,Transform position, Action<DamageFontPart> onLifecycleEnd)
        {
            gameObject.SetActive(false);
            var str = $"{amount}";
            damageText.SetText(str);
            damageOutlines.ForEach(_ => _.SetText(str));
            gameObject.SetActive(true);
            this.transform.position = position.position;

            DOTween.Sequence()
                .Append(transform.DOMove(this.transform.position + new Vector3(15, 10, 0), 0.1f))
                .Append(transform.DOMove(this.transform.position + new Vector3(15, -10, 0), 0.1f))
                .Done();
            
            await UniTask.WaitForSeconds(duration);
            
            onLifecycleEnd?.Invoke(this);
        }
    }
}