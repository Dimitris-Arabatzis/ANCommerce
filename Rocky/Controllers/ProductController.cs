using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky_DataAccess;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = prodRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRepo.GetAll(includeProperies: "Category,ApplicationType");

            return View(objList);
        }

        public IActionResult Upsert(int? id)
        {
            //IEnumerable <SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            //ViewBag.CategoryDropDown = CategoryDropDown;

            //Product product = new Product();

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetAllDropdownList(WebConstants.CategoryName),
                ApplicationTypeList = _prodRepo.GetAllDropdownList(WebConstants.ApplicationTypeName)
            };

            if ( id == null )
            {
                //Create
                return View(productVM);
            }
            else
            {
                productVM.Product = _prodRepo.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {

                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WebConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using(var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extension;

                    _prodRepo.Add(productVM.Product);
                    
                }
                else
                {
                    //Updating
                    var objFromDb = _prodRepo.FirstOrDefault(x => x.Id == productVM.Product.Id,isTracking:false);

                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WebConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload,objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;

                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _prodRepo.Update(productVM.Product);
                }
                _prodRepo.Save();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _prodRepo.GetAllDropdownList(WebConstants.CategoryName);
            productVM.ApplicationTypeList = _prodRepo.GetAllDropdownList(WebConstants.ApplicationTypeName);

            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product product = _prodRepo.FirstOrDefault(x => x.Id == id,includeProperies: "Category,ApplicationType");
            //product.Category = _db.Category.Find(product.CategoryId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _prodRepo.Find(id.GetValueOrDefault());

            if(obj == null)
            {
                return NotFound();
            }

            var imageToDelete = _webHostEnvironment.WebRootPath + WebConstants.ImagePath + obj.Image;
            if (System.IO.File.Exists(imageToDelete))
            {
                System.IO.File.Delete(imageToDelete);
            }
            

            _prodRepo.Remove(obj);
            _prodRepo.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Duplicate(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _prodRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            

            var existingImage = _webHostEnvironment.WebRootPath + WebConstants.ImagePath + obj.Image;
            var newImageName = Guid.NewGuid() + Path.GetExtension(existingImage);
            var newImage = _webHostEnvironment.WebRootPath + WebConstants.ImagePath + newImageName;
            if (System.IO.File.Exists(existingImage))
            {
                System.IO.File.Copy(existingImage, newImage);
            }
            obj.Image = newImageName;
            obj.Id = default;

            _prodRepo.Add(obj);
            _prodRepo.Save();
            return RedirectToAction("Index");
        }
    }
}
