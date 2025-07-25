using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace DogrudanTeminParadiseAPI.Repositories
{
    public class GridFSRepository
    {
        private readonly GridFSBucket _bucket;

        public GridFSRepository(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(dbName);
            _bucket = new GridFSBucket(db);
        }

        public async Task<string> UploadAsync(byte[] data, string fileName)
        {
            var id = await _bucket.UploadFromBytesAsync(fileName, data);
            return id.ToString();
        }

        public async Task<byte[]> DownloadAsync(string id)
        {
            return await _bucket.DownloadAsBytesAsync(ObjectId.Parse(id));
        }

        public async Task DeleteAsync(string id)
        {
            await _bucket.DeleteAsync(ObjectId.Parse(id));
        }
    }
}
