using Boomi.Erp.Sage.Webapi.Models.Orders;
using System.Threading.Tasks;

namespace Boomi.Erp.Sage.Webapi.Data.Abstract
{
    public interface IOrdersRepository
    {
        Task<bool> ExistsAsync(string orderNumber);

        void Create(Order order);
    }
}
