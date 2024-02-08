using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : Controller
    {
        [HttpGet("searchproducts")]
        public async Task<ActionResult> SearchProducts(string searchText, int page = 1, int pageSize = 10)
        {
            using (DataContext db = new DataContext())
            {
                var containsName = from p in db.Products
                                   where p.Name.Contains(searchText)
                                   select p;

                var containsDescription = from p in db.Products
                                          where p.Description.Contains(searchText)
                                          select p;
                if (containsName == null)
                {
                    return BadRequest(new { error = "Product not found" });
                }
                if (containsDescription == null)
                {
                    return BadRequest(new { error = "Product not found" });
                }

                var result = new List<Product>();
                foreach (var product in containsName)
                {
                    if (!result.Contains(product))
                    {
                        result.Add(product);
                    }
                }
                foreach (var product in containsDescription)
                {
                    if (!result.Contains(product))
                    {
                        result.Add(product);
                    }
                }

                var totalCount = result.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var products = result
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Products = products
                };

                return Ok(response);
            }
        }
    }
}
