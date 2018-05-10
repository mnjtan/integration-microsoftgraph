using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Integration.MicrosoftGraph.Service.Controllers
{
  public abstract class BaseController : Controller
  {
    public readonly ILogger logger;

    public BaseController(ILoggerFactory loggerFactory)
    {
      logger = loggerFactory.CreateLogger(this.GetType().Name);
    }
  }
}
