using Rozetka.Data;

namespace Rozetka.Servises
{
    public interface IDataService
    {
    }

    public class DataService : IDataService
    {
        private readonly DataContext _context;

        public DataService(DataContext context)
        {
            _context = context;
        }

    }
}