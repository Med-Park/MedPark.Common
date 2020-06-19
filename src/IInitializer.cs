using System.Threading.Tasks;

namespace MedPark.Common
{
    public interface IInitializer
    {
        Task InitializeAsync();
    }
}