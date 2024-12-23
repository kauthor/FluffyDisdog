using System;
using Sirenix.Utilities;

namespace FluffyDisdog
{
    public class IntReactiveFluffyProperty
    {

        private int value;
        public int Value => value;

        private event Action<int> onValueChanged;
        

        public IntReactiveFluffyProperty()
        {
            value = 0;
            onValueChanged = null;
        }

        public void Subscribe(Action<int> cb)
        {
            onValueChanged -= cb;
            onValueChanged += cb;
        }

        public void ChangeValueByDelta(int delta)
        {
            value += delta;
            onValueChanged?.Invoke(value);
        }

        public void ChangeValue(int val)
        {
            value = val;
            onValueChanged?.Invoke(val);
        }
    }
}