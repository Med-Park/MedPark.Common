using System.Threading.Tasks;

namespace MedPark.Common.Mongo
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync();
    }
}