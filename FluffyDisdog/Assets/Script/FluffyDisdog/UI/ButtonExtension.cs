
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class ButtonExtension:Button
    {
        private Action OnHoveredAction;
        private Action OnUnhoveredAction;

        private bool hovered = false;

        protected override void Awake()
        {
            base.Awake();
            hovered = false;
            OnHoveredAction = null;
            OnUnhoveredAction = null;
        }

        public void BindHoveredHandler(Action hoveredAction, Action unhoveredAction)
        {
            OnHoveredAction = hoveredAction;
            OnUnhoveredAction = unhoveredAction;
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if ((state == SelectionState.Highlighted || state == SelectionState.Selected)
                && !hovered)
            {
                OnHoveredAction?.Invoke();
                hovered = true;
            }
            else if (hovered)
            {
                OnUnhoveredAction?.Invoke();
                hovered = false;
            }
        }
    }
}