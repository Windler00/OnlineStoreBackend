using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Dto.Category;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpPost("createcategory")]
        public async Task<ActionResult> CreateCategory(CategoryDto request)
        {
            using (DataContext db = new DataContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Name == request.Name);
                if (category == null)
                {
                    return Conflict("Category alredy exist");
                }
                Category newCategory = new Category();
                newCategory.Name = request.Name;
                newCategory.ParentId = request.ParentId;
                db.Add(newCategory);
                db.SaveChanges();
                return Ok("Category added");
            }
        }
        [HttpDelete("deletecategory")]
        public async Task<ActionResult> DeleteCategory(CategoryDto request)
        {
            using (DataContext db = new DataContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Name.Contains(request.Name));
                if (category == null)
                {
                    return BadRequest("Category not found");
                }
                db.Categories.Remove(category);
                db.SaveChanges();
                return Ok("Category deleted");
            }
        }
        [HttpGet("getcategories")]
        public async Task<ActionResult> GetCategories()
        {
            using (DataContext db = new DataContext())
            {
                var categories = db.Categories.ToList();
                return Ok(categories);
            }
        }
    }
}
