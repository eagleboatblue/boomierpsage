using System;
using System.Threading.Tasks;

namespace Boomi.Erp.Sage.Webapi.Data.Abstract
{
    public interface IOrderLinesRepository
    {
        Task<bool> ExistsAsync(string code, bool isService);
    }
}
