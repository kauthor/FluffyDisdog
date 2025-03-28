namespace FluffyDisdog
{
    public class AccountManager : CustomSingleton<AccountManager>
    {
        private int gold;

        public int Gold => gold;

        public bool GoldConsume(int amount)
        {
            if (amount > gold)
                return false;
            gold -= amount;
            return true;
        }
        
    }
}