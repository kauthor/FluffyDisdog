using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace FluffyDisdog.UI
{
    public class DamageFontPart:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float duration = 1.0f;


        public async UniTask Show(int amount,Transform position, Action<DamageFontPart> onLifecycleEnd)
        {
            damageText.SetText($"{amount}");
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