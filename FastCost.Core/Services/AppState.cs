namespace FastCost.Core.Services
{
    public static class AppState
    {
        private static int _dataVersion = 1;

        // Global version of data. Incremented whenever a cost is added, edited or deleted.
        public static int DataVersion => _dataVersion;

        public static void NotifyDataChanged() => Interlocked.Increment(ref _dataVersion);
    }
}
