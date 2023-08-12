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
        public async Task<ActionResult> CreateProduct(ProductDto request)
        {
            using (DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Name.Contains(request.Name));
                if (product != null)
                {
                    return Conflict("Product alredy exist");
                }
                Product newProduct = new Product();
                newProduct.Name = request.Name;
                newProduct.Description = request.Description;
                db.Products.Add(newProduct);
                db.SaveChanges();
                return Ok("Product added");
            }
        }
        [HttpDelete("deleteproduct")]
        public async Task<ActionResult> DeleteProduct(ProductDto request)
        {
            using(DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Name == request.Name);
                if (product == null)
                {
                    return BadRequest("Product not found");
                }
                db.Products.Remove(product);
                db.SaveChanges();
                return Ok("Product deleted");
            }
        }
        [HttpPatch("changeproduct")]
        public async Task<ActionResult> ChangeProduct (ProductDto request)
        {
            using(DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(u => u.Name == request.Name);
                if(product == null)
                {
                    return BadRequest("Product not found");
                }
                product.Name = request.Name;
                product.Description = request.Description;
                db.SaveChanges();
                return Ok("Product changed");
            }
        }
        [HttpGet("getproduct")]
        public async Task<ActionResult> GetProduct(ProductDto request)
        {
            using(DataContext db = new DataContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Name.Contains(request.Name));
                if (product == null)
                {
                    return BadRequest("Product not found");
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
                    .Skip(request.First)
                    .Take(request.Last)
                    .ToList();
                return Ok(products);
            }
        }
    }
}