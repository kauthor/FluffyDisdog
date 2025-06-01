using FluffyDisdog;
using UnityEngine;

namespace Script.FluffyDisdog.Managers
{
    public class StageSeedGroup
    {
        private System.Random minorSeed;
        private System.Random storeSeed;

        public StageSeedGroup(int bases)
        {
            minorSeed = new System.Random(bases);
            storeSeed = new System.Random(bases+1);
        }
        
        public int GetNextMinorSeed() => minorSeed.Next();
        public int GetNextStoreSeed() => storeSeed.Next();
    }
    public class SeedManager:CustomSingleton<SeedManager>
    {

        [SerializeField] private int testSeed = -1;


        private System.Random baseSeed;
        private int currentStage = 0;
        private int currentStageSeed;
        private StageSeedGroup stageGroup;

        void InitGame()
        {
            GenerateBaseLevelSeed();
        }

        void GenerateBaseLevelSeed()
        {
            var randSeed = testSeed >=0 ? new System.Random(testSeed): new System.Random();
            int seed = randSeed.Next();


            baseSeed = new System.Random(seed);
        }


        public int UpdateStage(int stage = 1)
        {
            if(baseSeed == null || stage <=1)
                InitGame();

            currentStageSeed = baseSeed.Next();
            stageGroup = new StageSeedGroup(currentStageSeed);
            return currentStage;
        }


        public int GetMinor()
        {
            if (stageGroup == null)
                return 0;

            return stageGroup.GetNextMinorSeed();
        }

        public int GetStoreSeed()
        {
            if (stageGroup == null)
                return 0;

            return stageGroup.GetNextStoreSeed();
        }
    }
}