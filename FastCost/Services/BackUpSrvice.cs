using System.Globalization;
using FastCost.Core.Services;

namespace FastCost.Services
{
    internal class BackUpSrvice : IBackUpSrvice
    {
        private readonly IAllCostsService _allCostsService;

        public BackUpSrvice(IAllCostsService allCostsService)
        {
            _allCostsService = allCostsService;
        }

        public async Task ExportData()
        {
            var costs = await _allCostsService.LoadCostsBackUp();

            var filePath = Path.Combine(FileSystem.AppDataDirectory, "costsBackUp.csv");

            var lines = new List<string> { "Id,Value,Description,Date,CategoryId" };
            foreach (var cost in costs)
            {
                var description = cost.Description?.Replace("\"", "\"\"") ?? string.Empty;
                var value = cost.Value.ToString(CultureInfo.InvariantCulture);
                var date = cost.Date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                lines.Add($"{cost.Id},{value},\"{description}\",{date},{cost.CategoryId}");
            }

            await File.WriteAllLinesAsync(filePath, lines);

            if (!File.Exists(filePath))
            {
                await Shell.Current.DisplayAlertAsync("Failure", $"Error writing to file. File path: {filePath}", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Success", $"Costs data exported to file. File path: {filePath}", "OK");
            }
        }
    }
}
