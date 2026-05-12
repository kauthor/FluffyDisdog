namespace FluffyDisdog
{
    public class AccountManager : CustomSingleton<AccountManager>
    {
        private int gold;

        public int Gold => gold;

        protected override void Awake()
        {
            base.Awake();
            gold = 30; //임시
        }
        
        public void ResetGoldOnGameStart() => gold = 30;

        public bool GoldConsume(int amount)
        {
            if (amount > gold)
                return false;
            gold -= amount;
            return true;
        }

        public void AddGold(int amount)
        {
            gold += amount;
        }
        
    }
}