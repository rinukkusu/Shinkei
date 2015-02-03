namespace Shinkei.IRC.Entities
{
    class EntConsole : IEntity
    {
        private static EntConsole _instance;
        public static EntConsole GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EntConsole();
            }

            return _instance;
        }

        public string GetName()
        {
            return "Console";
        }
    }
}
