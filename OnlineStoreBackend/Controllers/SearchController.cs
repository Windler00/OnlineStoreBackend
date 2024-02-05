using Microsoft.AspNetCore.Mvc;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : Controller
    {
        [HttpGet("searchproducts")]
        public async Task<ActionResult> SearchProducts(string searchText)
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

                var result = new List<string>();
                foreach (var product in containsName)
                {
                    if (!result.Contains(product.Name))
                    {
                        result.Add(product.Name);
                    }
                }
                foreach (var product in containsDescription)
                {
                    if (!result.Contains(product.Name))
                    {
                        result.Add(product.Name);
                    }
                }
                return Ok(result);
            }
        }
        [HttpGet("searchautocomplete")]
        public async Task<ActionResult> SearchAutocomplete(string searchText)
        {
            using (DataContext db = new DataContext())
            {
                var containsName = from p in db.Products
                                   where p.Name.Contains(searchText)
                                   select p;
                if (containsName == null)
                {
                    return BadRequest(new { error = "Product not found" });
                }
                var result = new List<string>();
                foreach (var product in containsName)
                {
                    if (!result.Contains(product.Name))
                    {
                        result.Add(product.Name);
                    }
                }
                return Ok(result);
            }
        }
    }
}
