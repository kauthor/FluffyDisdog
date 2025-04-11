using System.ComponentModel;

namespace FluffyDisdog.Data
{
    public enum ToolAdditionalOption
    {
        [Description("그런 거 없다.")]
        None=0,
        [Description("파괴 실패 시 확률로 균열.")]
        ChangeCrackWhenFail=1,
        [Description("파괴 실패 시 확률로 전염.")]
        ChangeFlagueWhenFail=2,
        [Description("인접 랜덤타일 전염.")]
        ChangeFlagueRandomNearTile=3
    }
}