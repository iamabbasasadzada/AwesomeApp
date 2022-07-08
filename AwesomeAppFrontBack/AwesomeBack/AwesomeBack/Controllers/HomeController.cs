using AwesomeBack.DAL;
using AwesomeBack.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Customer> customers = _context.Customers.OrderByDescending(c => c.Id).Take(3).ToList();
            return View(customers);
        }
    }
}
