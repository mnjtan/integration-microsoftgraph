using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Integration.MicrosoftGraph.Service.Controllers
{
  public abstract class BaseController : Controller
  {
    protected readonly ILogger logger;

    protected BaseController(ILoggerFactory factory)
    {
      logger = factory.CreateLogger(this.GetType().Name);
    }
  }
}
