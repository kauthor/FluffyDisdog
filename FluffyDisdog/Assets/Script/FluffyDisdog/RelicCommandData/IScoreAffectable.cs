using FluffyDisdog.Data;

namespace FluffyDisdog.RelicCommandData
{
    public interface IScoreAffectable
    {
        public abstract NodeScoreType scoreType { get; }
        public abstract ToolTag toolType { get; }
        public abstract float scoreMulti { get; }
        public abstract float toolMulti { get; }
    }
}