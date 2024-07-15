using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_MicroService_JobScheduler.Controllers;

// [Authorize]
public class HomeController : Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Index()
    {
        return View();
    }
}



[ApiController]
[Route("api/[controller]/[action]")]
public class JobTestController : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Job Test Controller Get");
    }

    [HttpPost]
    public IActionResult Post()
    {
        return Ok("Job Test Controller Post");
    }

    [HttpPut]
    public IActionResult Put()
    {
        return Ok("Job Test Controller Put");
    }
}


