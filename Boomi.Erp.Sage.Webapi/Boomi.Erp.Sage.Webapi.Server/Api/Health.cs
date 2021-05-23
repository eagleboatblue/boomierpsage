using System.Web.Http;

namespace Boomi.Erp.Sage.Webapi.Server.Api
{
    public class HealthController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}
