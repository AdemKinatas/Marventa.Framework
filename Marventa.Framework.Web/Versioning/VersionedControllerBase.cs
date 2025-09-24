using Marventa.Framework.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Marventa.Framework.Web.Versioning;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class VersionedControllerBase : BaseController
{
}