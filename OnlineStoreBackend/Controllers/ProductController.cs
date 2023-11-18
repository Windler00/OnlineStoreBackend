using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Dto.Product;
using OnlineStoreBackend.Models;
using System.Security.Claims;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("createproduct")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<ActionResult> CreateProduct(ProductDto request)
        {
            using (DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Name.Contains(request.Name));
                if (product != null)
                {
                    return Conflict(new { message = "Product alredy exist" });
                }
                if (request.Name.Length < 5)
                {
                    return BadRequest(new { message = "Minimum product name length 5" });
                }
                if (request.Description.Length < 5)
                {
                    return BadRequest(new { message = "Minimum product description length 5" });
                }
                Product newProduct = new Product();
                newProduct.Name = request.Name;
                newProduct.Description = request.Description;
                newProduct.Price = request.Price;
                newProduct.Quantity = request.Quantity;
                var requestctx = _httpContextAccessor.HttpContext.Request;
                var serverUrl = $"{requestctx.Scheme}://{requestctx.Host.Value}";
                newProduct.ImageUrl = serverUrl + $"/default/ProductImage.jpg";
                db.Products.Add(newProduct);
                db.SaveChanges();
                product = db.Products.FirstOrDefault(p => p.Name.Contains(request.Name));
                return Ok(new { message = "Product added", product = product});
            }
        }
        [HttpPost("uploadimage")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<ActionResult> UploadImage(IFormFile file, int id)
        {
            if (file != null && file.Length > 0)
            {
                using (DataContext db = new DataContext())
                {
                    var product = db.Products.FirstOrDefault(p => p.Id == id);
                    if (product == null)
                    {
                        return BadRequest(new { message = "Product not found" });
                    }
                    var oldFileName = Path.GetFileName(product.ImageUrl);
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "productimages", oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    string fileExtension = Path.GetExtension(file.FileName);
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "productimages", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var request = _httpContextAccessor.HttpContext.Request;
                    var serverUrl = $"{request.Scheme}://{request.Host.Value}";
                    product.ImageUrl = serverUrl + $"/productimages/{fileName}";
                    db.SaveChanges();
                    return Ok(new { message = "Product image uploaded", imageUrl = product.ImageUrl });
                }
            }
            else
            {
                return BadRequest(new { message = "Something went wrong product image is not loaded" });
            }
        }
        [HttpDelete("deleteproduct")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<ActionResult> DeleteProduct(ProductDto request)
        {
            using(DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Name == request.Name);
                if (product == null)
                {
                    return BadRequest(new { message = "Product not found" });
                }
                db.Products.Remove(product);
                db.SaveChanges();
                return Ok(new { message = "Product deleted" });
            }
        }
        [HttpPatch("changeproduct")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<ActionResult> ChangeProduct (ChangeProductDto request)
        {
            using(DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(u => u.Id == request.Id);
                if(product == null)
                {
                    return BadRequest(new { message = "Product not found"});
                }
                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.Quantity = request.Quantity;
                db.SaveChanges();
                return Ok(new {message = "Product changed" });
            }
        }
        [HttpGet("getproduct")]
        public async Task<ActionResult> GetProduct(int id)
        {
            using(DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return BadRequest(new { message = "Product not found" });
                }
                return Ok(product);
            }
        }
        [HttpGet("getproducts")]
        public async Task<ActionResult> GetProducts(int page = 1, int pageSize = 10)
        {
            using(DataContext db = new DataContext())
            {
                var totalCount = db.Products.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var products = db.Products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Products = products
                };

                return Ok(result);
            }
        }
    }
}