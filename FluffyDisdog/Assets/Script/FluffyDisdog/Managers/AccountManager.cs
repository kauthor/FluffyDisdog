namespace FluffyDisdog
{
    public class AccountManager : CustomSingleton<AccountManager>
    {
        private int gold;

        public int Gold => gold;

        protected override void Awake()
        {
            base.Awake();
            gold = 1000; //임시
        }

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