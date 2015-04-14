
using System.Web.Http;
using log4net;


namespace WebApi.Controllers
{
    public class BaseController : ApiController
    {
          protected readonly ILog log = LogManager.GetLogger("ErrorLog");

          public BaseController()
          {
              log4net.Config.XmlConfigurator.Configure();             
          }
        
    }
}
