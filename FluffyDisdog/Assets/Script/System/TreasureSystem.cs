using System;
using System.Collections.Generic;
using Script.FluffyDisdog.Managers;
using Random = UnityEngine.Random;

namespace FluffyDisdog
{
    public class Treasure
    {
        private int startPosX = -1;
        private int startPosY = -1;
        private int row = 0;
        private int column = 0;
        private int degree;
        private int amountToUnbox;
        private bool activated = false;

        public Treasure(int treasureX, int treasureY, int treasureDegree, int posX, int posY)
        {
            row = treasureX;
            column = treasureY;
            startPosX = posX;
            startPosY = posY;

            amountToUnbox = treasureX * treasureY;
            activated = false;
            degree = treasureDegree;
        }

        public void OnDiscovered()
        {
            int condition = 0;
            for (int j = 0; j < column; j++)
            {
                for (int i = 0; i < row; i++)
                {
                    var cond = TileGameManager.I.TileSet.GetNodeCondition(startPosX + i, startPosY + j);
                    if(cond<0)
                        condition++;
                }
            }

            if (condition >= amountToUnbox)
            {
                //todo : 여기에다가 팝업 등장을... 넣자.
            }
        }
    }
    public class TreasureSystem:IEventAffectable
    {
        private List<Treasure> treasureList = new List<Treasure>();
        private Dictionary<(int,int), Treasure> coordList = new Dictionary<(int,int), Treasure>();

        public void ClearAndInit()
        {
            treasureList.Clear();
            coordList.Clear();
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TileDigged, OnTileDigged, this);
        }

        private void OnTileDigged(TurnEventOptionParam param)
        {
            if (param is TileEmulatorOptionParam emul)
            {
                var target = emul.targetCoord;
                if (coordList.ContainsKey((target.Item1, target.Item2)))
                {
                    coordList[(target.Item1, target.Item2)].OnDiscovered();
                }
            }
        }

        public void TryGenerateTreasure(int treasureX, int treasureY, int treasureDegree,int posX, int posY, TileSet baseTileSet)
        {
            var tr = new Treasure(treasureX, treasureY, treasureDegree, posX, posY);
            for (int j = 0; j < treasureY; j++)
            {
                for (int i = 0; i < treasureX; i++)
                {
                    coordList.Add((i+posX, j+posY),tr);
                }
            }
        }
    }
}