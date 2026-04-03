using FastCost.Core.Models;
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

            var lines = new List<string>();
            lines.Add("Id,Value,Description,Date,CategoryId");
            foreach (var cost in costs)
            {
                var description = cost.Description?.Replace("\"", "\"\"");
                lines.Add($"{cost.Id},{cost.Value},\"{description}\",{cost.Date},{cost.CategoryId}");
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
