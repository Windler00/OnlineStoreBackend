using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Models;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/basket")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpPost("addproduct")]
        [Authorize]
        public async Task<ActionResult> AddProduct(int ProductId,int Quantity)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(email));
                if(user == null)
                {
                    return BadRequest(new { error = "User not exist" });
                }

                var product = db.Products.FirstOrDefault(p => p.Id == ProductId);
                if(product == null)
                {
                    return BadRequest(new {error = "Product not exist" });
                }
                if(product.Quantity < Quantity)
                {
                    return BadRequest(new { error = "Unacceptable quantity" });
                }
                var basketItem = db.BasketItems.FirstOrDefault(b => b.ProductId == ProductId);
                if(basketItem != null)
                {
                    return BadRequest(new { error = "Product already in basket" });
                }

                BasketItem newBasketItem = new BasketItem();
                newBasketItem.Product = product;
                newBasketItem.Quantity = Quantity;
                newBasketItem.User = user;
                db.BasketItems.Add(newBasketItem);
                db.SaveChanges();
                return Ok(new {success = "Product added to basket" });
            }
        }
        [HttpGet("getproducts")]
        [Authorize]
        public async Task<ActionResult> GetProducts(int page = 1, int pageSize = 10)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(email));

                var basketList = db.BasketItems
                    .Where(b => b.UserId == user.Id)
                    .Select(b => new
                    {
                        b.Product,
                        b.Quantity
                    }).ToList();

                var totalCount = basketList.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var products = basketList
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
        [HttpDelete("deleteproduct")]
        [Authorize]
        public async Task<ActionResult> DeleteProduct(int ProductId)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(email));

                var basketItem = db.BasketItems.FirstOrDefault(b => b.ProductId == ProductId);
                if (basketItem == null)
                {
                    return BadRequest(new {error = "Product not exist"});
                }
                if(user.Id == basketItem.UserId)
                {
                    db.BasketItems.Remove(basketItem);
                    db.SaveChanges();
                    return Ok(new { success = "Product removed from basket" });
                }
                else
                {
                    return BadRequest(new { error = "Product not exist" });
                }
            }
        }
        [HttpPost("changeproduct")]
        [Authorize]
        public async Task<ActionResult> ChangeProduct(int ProductId,int Quantity)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(email));

                var basketItem = db.BasketItems.FirstOrDefault(b => b.ProductId == ProductId);
                if (basketItem == null)
                {
                    return BadRequest(new { error = "Product not exist" });
                }
                if (Quantity < basketItem.Quantity)
                {
                    return BadRequest(new { error = "Unacceptable quantity" });
                }
                if (user.Id == basketItem.UserId)
                {
                    basketItem.Quantity = Quantity;
                    db.SaveChanges();
                    return Ok(new { success = "Product removed from basket" });
                }
                else
                {
                    return BadRequest(new { error = "Product not exist" });
                }
            }
        }

    }
}
