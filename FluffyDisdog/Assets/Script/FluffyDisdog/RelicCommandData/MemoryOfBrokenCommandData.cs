using FluffyDisdog.Data.RelicData;

namespace FluffyDisdog.RelicCommandData
{
    public class MemoryOfBrokenCommandData: RelicCommandData
    {
        //todo : 도구파괴 구현!
        public override RelicName relicType => RelicName.MemoryOfBroken;

        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
        }
        
        
    }
}