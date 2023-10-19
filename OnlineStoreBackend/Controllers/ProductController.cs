using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Dto.Product;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
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
            using (DataContext db = new DataContext())
            {
                if (file != null)
                {
                    var product = db.Products.FirstOrDefault(p => p.Id == id);
                    product.ImageName = file.Name;
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        product.ImageData = memoryStream.ToArray();
                    }
                    db.SaveChanges();
                    return Ok(new { message = "Image uploaded"});
                }
                else
                {
                    return BadRequest(new { message = "File is missing." });
                }
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
                db.SaveChanges();
                return Ok(new {message = "Product changed" });
            }
        }
        [HttpPost("getproduct")]
        public async Task<ActionResult> GetProduct(GetProductDto request)
        {
            using(DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Id == request.Id);
                if (product == null)
                {
                    return BadRequest(new { message = "Product not found" });
                }
                return Ok(product);
            }
        }
        [HttpPost("getproducts")]
        public async Task<ActionResult> GetProducts(GetProductsDto request)
        {
            using(DataContext db = new DataContext())
            {
                var products = db.Products
                    .OrderBy(p => p.Id)
                    .Skip(request.First)
                    .Take(request.Last)
                    .ToList();
                return Ok(products);
            }
        }
    }
}