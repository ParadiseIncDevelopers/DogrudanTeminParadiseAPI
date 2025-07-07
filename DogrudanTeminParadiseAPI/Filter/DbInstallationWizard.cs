using DogrudanTeminParadiseAPI.Models;
using MongoDB.Driver;
using System.Text.Json;

namespace DogrudanTeminParadiseAPI.Filter
{
    public class DbInstallationWizard
    {
        private readonly IConfiguration _cfg;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;

        public DbInstallationWizard(IServiceProvider services)
        {
            _cfg = services.GetRequiredService<IConfiguration>();
            var connectionString = _cfg["MongoAPI"];
            var dbName = _cfg["MongoDBName"];
            _client = new MongoClient(connectionString);
            _db = _client.GetDatabase(dbName);
        }

        public async Task RunAsync()
        {
            // 1) Koleksiyon isimleri
            var collections = new[]
            {
            "AdminUsers", "SuperAdmin", "Users", "Entreprises",
            "ProductItems", "Products", "AdministrationUnits",
            "SubAdministrationUnits", "ThreeSubAdministrationUnits",
            "ProcurementEntries", "Titles", "ProcurementListItems",
            "Units", "OfferLetters", "Categories", "BudgetItems",
            "MarketResearchJuries", "InspectionAcceptanceJuries",
            "InspectionAcceptanceCertificates",
            "AdditionalInspectionAcceptanceCertificates",
            "ApproximateCostJuries", "ProcurementEntryEditors", "InspectionAcceptanceNotes",
            "DecisionNumbers"
        };

            // 2) Eksik koleksiyonları yarat
            var existing = await _db.ListCollectionNames().ToListAsync();
            foreach (var name in collections)
            {
                if (!existing.Contains(name))
                    await _db.CreateCollectionAsync(name);
            }

            // 3) Titles koleksiyonunu seed et
            await SeedIfEmptyAsync<Title>("Titles", "Titles.json");

            // 4) Units koleksiyonunu seed et
            await SeedIfEmptyAsync<Unit>("Units", "Units.json");
        }

        private async Task SeedIfEmptyAsync<T>(string collName, string jsonFile)
        {
            var coll = _db.GetCollection<T>(collName);
            var count = await coll.EstimatedDocumentCountAsync();
            if (count == 0)
            {
                var json = await File.ReadAllTextAsync(jsonFile);
                var list = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (list != null && list.Count > 0)
                    await coll.InsertManyAsync(list);

                // JSON dosyasını sil (isteğe bağlı)
                File.Delete(jsonFile);
            }
        }
    }
}
