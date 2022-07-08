using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]

    public class DashboardController : Controller
    {
        [Authorize]

        public IActionResult Index()
        {
            return View();
        }
    }
}
