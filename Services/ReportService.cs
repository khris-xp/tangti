using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class ReportService
{
    private readonly IMongoCollection<Report> _reportCollection;

    public ReportService(
        IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting
    )
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString
        );

        var mongoDatabase = mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName
        );

        _reportCollection = mongoDatabase.GetCollection<Report>(
            "report"
        );
    }

    public async Task<List<Report>> GetReportsAsync()
    {
        return await _reportCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Report?> GetReportAsync(string id)
    {
        return await _reportCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Report newReport)
    {
        await _reportCollection.InsertOneAsync(newReport);
    }

    public async Task UpdateAsync(string id, Report updateReport)
    {
        await _reportCollection.ReplaceOneAsync(x => x.Id == id, updateReport);
    }

    public async Task DeleteAsync(string id)
    {
        await _reportCollection.DeleteOneAsync(x => x.Id == id);
    }
}