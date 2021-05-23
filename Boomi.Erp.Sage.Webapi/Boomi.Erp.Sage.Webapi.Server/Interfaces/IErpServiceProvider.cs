using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Models.Common;
using Boomi.Erp.Sage.Webapi.Models.Orders;

namespace Boomi.Erp.Sage.Webapi.Server.Interfaces
{
    public interface IErpServiceProvider
    {
        Task<SageResponse> CreateOrder(Metadata metadata, Order order);
    }
}
