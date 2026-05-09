namespace FastCost.Core.Services
{
    public static class AppState
    {
        // Global version of data. Incremented whenever a cost is added, edited or deleted.
        public static int DataVersion { get; private set; } = 1;

        public static void NotifyDataChanged()
        {
            DataVersion++;
        }
    }
}
