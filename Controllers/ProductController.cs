using DAL;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace WebApp.Controllers
{
    public class ProductController : Controller
    {
        IUnitOfWork _uow;
        public ProductController(IUnitOfWork uow, IUnitOfWork uow1)
        {
            _uow = uow;
        }
        public IActionResult Index()
        {
            var products = _uow.ProductRepo.GetProductWithCategories();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _uow.CategoryRepo.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product model)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                _uow.ProductRepo.Add(model);
                _uow.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _uow.CategoryRepo.GetAll();
            return View();
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Categories = _uow.CategoryRepo.GetAll();
            Product model = _uow.ProductRepo.Find(id);

            return View("Create", model);
        }

        [HttpPost]
        public IActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                _uow.ProductRepo.Update(model);
                _uow.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _uow.CategoryRepo.GetAll();
            return View("Create", model);
        }

        public IActionResult Delete(int id)
        {
            _uow.ProductRepo.Delete(id);
            _uow.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
