using System;

namespace FluffyDisdog.Data
{
    [Flags]
    public enum ToolTag : uint
    {
        NONE=0,
        First=1<<0,
        Sec=1<<1,
        Third=1<<2,
        Fourth=1<<3,
        Fifth=1<<4,
        Sixth=1<<5,
        Seventh=1<<6,
        Eighth=1<<7,
        Ninth=1<<8,
        Tenth=1<<9
    }
}