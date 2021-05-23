using System;
using System.Threading.Tasks;

namespace Boomi.Erp.Sage.Webapi.Data.Abstract
{
    public interface IUsersRepository
    {
        Task<bool> HasAccessAsync(string company, string email);
    }
}
