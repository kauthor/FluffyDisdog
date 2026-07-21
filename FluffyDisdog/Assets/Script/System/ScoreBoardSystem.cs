using System.Collections.Generic;
using FluffyDisdog.Data;
using FluffyDisdog.RelicCommandData;

namespace FluffyDisdog
{
    public enum NodeScoreType
    {
        NONE=-1,
        IDLE=0,
        CRACK,
        /// 여기서...끊나?
        OB1,
        OB2,
        OB3,
        OB4,
        OB5,
        TREASURE,
        MAX
    }
    
    public class ScoreBoardSystem
    {
        private Dictionary<NodeScoreType, float> tileScoreMulti= new Dictionary<NodeScoreType, float>();
        public Dictionary<NodeScoreType, float> TileScoreMulti => tileScoreMulti;
        
        private Dictionary<ToolTag, float> toolScoreMulti= new Dictionary<ToolTag, float>();
        public Dictionary<ToolTag, float> ToolScoreMulti => toolScoreMulti;

        private Dictionary<NodeScoreType, bool> nodeBookmarked= new Dictionary<NodeScoreType, bool>();
        private List<NodeScoreType> nodeScoreBookmark = new List<NodeScoreType>();
        private int nodeBookmarkAmount = 0;
        private Dictionary<ToolTag, bool> toolBookmarked= new Dictionary<ToolTag, bool>();
        private List<ToolTag> toolTagBookmark = new List<ToolTag>();
        private int toolTagBookmarkAmount = 0;

        public ScoreBoardSystem()
        {
            for (int i = 0; i < (int)NodeScoreType.MAX; i++)
            {
                nodeScoreBookmark.Add((NodeScoreType)i);
                nodeBookmarked.Add((NodeScoreType)i, false);
            }

            for (int i = 1; i <= (int)ToolTag.Tenth; i *= 2)
            {
                toolTagBookmark.Add( (ToolTag)i);
                toolBookmarked.Add( (ToolTag)i, false);
            }       
        }

        public void BookmarkNodeScoreType(NodeScoreType type, bool bookmarkOn=true)
        {
            nodeBookmarked[type]=bookmarkOn;
            nodeScoreBookmark.Remove(type);
            nodeScoreBookmark.Insert(nodeBookmarkAmount + (bookmarkOn? 0:-1) , type);
            nodeBookmarkAmount += bookmarkOn ? 1 : -1;
        }

        public void BookmarkToolScoreType(ToolTag tag, bool bookmarkOn = true)
        {
            toolBookmarked[tag]=bookmarkOn;
            toolTagBookmark.Remove(tag);
            toolTagBookmark.Insert(toolTagBookmarkAmount + (bookmarkOn? 0:-1) , tag);
            toolTagBookmarkAmount += bookmarkOn ? 1 : -1;
        }
        
        public void Init(RelicSystem relicSystem)
        {
            tileScoreMulti.Clear();
            toolScoreMulti.Clear();
            var relics = relicSystem.currentRelicDatas;
            foreach (var relic in relics)
            {
                if (relic is IScoreAffectable aff)
                {
                    if (aff.scoreType != NodeScoreType.NONE)
                    {
                        if (!tileScoreMulti.TryAdd(aff.scoreType, aff.scoreMulti))
                        {
                            tileScoreMulti[aff.scoreType] += aff.scoreMulti;
                        }
                    }

                    if (aff.toolType != ToolTag.NONE)
                    {
                        if (!toolScoreMulti.TryAdd(aff.toolType, aff.toolMulti))
                        {
                            toolScoreMulti[aff.toolType] += aff.toolMulti;
                        }
                    }
                }
            }
        }
    }
}