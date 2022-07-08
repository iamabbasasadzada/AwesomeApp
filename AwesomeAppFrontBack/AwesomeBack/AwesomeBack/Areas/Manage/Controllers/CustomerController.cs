using AwesomeBack.DAL;
using AwesomeBack.Models;
using AwesomeBack.Utilies.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AwesomeBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CustomerController(AppDbContext context , IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize]

        public IActionResult Index(int page=1)
        {

            return View(_context.Customers.ToPagedList(page,2));
        }
        [Authorize]

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (_context.Customers.FirstOrDefault(c => c.FullName == customer.FullName) != null)
            {
                ModelState.AddModelError("FullName", "This FullName already in Db");
                return View();
            }
            if (customer.Photo == null)
            {
                ModelState.AddModelError("Photo", "You must choose file");
                return View();
            }
            if (!customer.Photo.CheckSize(500000))
            {
                ModelState.AddModelError("Photo", "File must be lower than 500000 kb");
                return View();
            }
            if (!customer.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "File must be image");
                return View();
            }
            string savePath = Path.Combine(_env.WebRootPath, "assets", "images");
            customer.Image = await customer.Photo.SaveFileAsync(savePath);
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            Customer customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> Edit(Customer customer)
        {
            Customer customerToUpdate = _context.Customers.FirstOrDefault(c => c.Id == customer.Id);
            if (customerToUpdate == null) return NotFound();
            if (!ModelState.IsValid) return BadRequest();
            if(customerToUpdate.FullName != customer.FullName)
            {
                if (_context.Customers.FirstOrDefault(c => c.FullName == customer.FullName) != null)
                {
                    ModelState.AddModelError("FullName", "This FullName already in Db");
                    return View();
                }
            }
            if(customer.Photo != null)
            {
                if (!customer.Photo.CheckSize(500000))
                {
                    ModelState.AddModelError("Photo", "File must be lower than 500000 kb");
                    return View();
                }
                if (!customer.Photo.CheckType("image/"))
                {
                    ModelState.AddModelError("Photo", "File must be image");
                    return View();
                }
                string savePath = Path.Combine(_env.WebRootPath, "assets", "images");
                customer.Image = await customer.Photo.SaveFileAsync(savePath);
                if (System.IO.File.Exists(Path.Combine(savePath, customerToUpdate.Image)))
                {
                    System.IO.File.Delete(Path.Combine(savePath, customerToUpdate.Image));
                }
                customerToUpdate.Image = customer.Image;
            }
            customerToUpdate.FullName = customer.FullName;
            customerToUpdate.Position = customer.Position;
            customerToUpdate.Desc = customer.Desc;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public IActionResult Delete(int id)
        {
            Customer customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();
            string path = Path.Combine(_env.WebRootPath, "assets", "images");
            if (System.IO.File.Exists(Path.Combine(path, customer.Image)))
            {
                System.IO.File.Delete(Path.Combine(path, customer.Image));
            }
            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
