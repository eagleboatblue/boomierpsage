using System.Threading.Tasks;
using Boomi.Erp.Sage.Webapi.Data.Abstract;
using Boomi.Erp.Sage.Webapi.Models.Common;

namespace Boomi.Erp.Sage.Webapi.Server.Interfaces
{
    public interface IDbManager
    {
        Task<bool> HasUserAccess(DatabaseRange database, string username);

        IOrdersRepository GetOrdersRepository(DatabaseRange database);

        IOrderLinesRepository GetOrderLinesRepository(DatabaseRange database);
    }
}
