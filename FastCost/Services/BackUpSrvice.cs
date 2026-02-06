using FastCost.Models;

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
            var allCosts = new AllCosts();
            var costs = await _allCostsService.LoadCostsBackUp();

            var filePath = Path.Combine(FileSystem.AppDataDirectory, "costsBackUp.csv");

            var lines = new List<string>();
            lines.Add("Id,Value,Description,Date,CategoryId");
            foreach (var cost in costs)
            {
                lines.Add($"{cost.Id},{cost.Value},{cost.Description},{cost.Date},{cost.CategoryId}");
            }

            File.WriteAllLines(filePath, lines);

            if (!File.Exists(filePath)) ;
                //await DisplayAlert("Failure", $"Error writing to file. File path: {filePath}", "OK");

            //await DisplayAlert("Success", $"Costs data exported to file. File path: {filePath}", "OK");
        }
    }
}
