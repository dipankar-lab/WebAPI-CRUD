using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace WebApp.Controllers
{
    public class ProductsController : Controller
    {
        Uri baseAddress;
        IConfiguration _config;
        HttpClient client;
        public ProductsController(IConfiguration config)
        {
            _config = config;
            baseAddress = new Uri(_config["ApiAddress"]);
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> model = new List<Product>();
            var response = client.GetAsync(client.BaseAddress + "/product/GetProducts").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                model = JsonSerializer.Deserialize<IEnumerable<Product>>(data);
            }
            return View(model);
        }

        private IEnumerable<Category> GetCategories()
        {
            IEnumerable<Category> model = new List<Category>();
            var response = client.GetAsync(client.BaseAddress + "/Category").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                model = JsonSerializer.Deserialize<IEnumerable<Category>>(data);
            }
            return model;
        }

        public IActionResult Create()
        {
            ViewBag.Categories = GetCategories();
            return View();
        }



        [HttpPost]
        public IActionResult Create(Product model)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                string data = JsonSerializer.Serialize(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = client.PostAsync(client.BaseAddress + "/Product/AddProduct",content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Categories = GetCategories();
            return View();
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Categories = GetCategories();
            Product model = GetProductsById(id);

            return View("Create", model);
        }

        private Product GetProductsById(int id)
        {
            var model = new Product();
            var response = client.GetAsync(client.BaseAddress + "/product/GetProducts"+id).Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                model = JsonSerializer.Deserialize<Product>(data);
            }
            return model;
        }

        [HttpPost]
        public IActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                string data = JsonSerializer.Serialize(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = client.PutAsync(client.BaseAddress + "/Product/Update"+model.Id, content).Result;
                return RedirectToAction("Index");
            }
            ViewBag.Categories = GetCategories();
            return View("Create", model);
        }

        public IActionResult Delete(int id)
        {
            var response = client.DeleteAsync(client.BaseAddress + "/product/Delete" + id).Result;
            return RedirectToAction("Index");
        }
    }
}
