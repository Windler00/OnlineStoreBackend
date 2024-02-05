using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineStoreBackend.Models;
using System.Security.Claims;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpPost("addproduct")]
        [Authorize]
        public async Task<ActionResult> AddProduct(int ProductId, int Quantity)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(email));

                var product = db.Products.FirstOrDefault(p => p.Id == ProductId);
                if (product == null)
                {
                    return BadRequest(new { error = "Product not exist" });
                }
                if (Quantity > product.Quantity)
                {
                    return BadRequest(new { error = "Unacceptable quantity" });
                }
                OrderItem orderItem = db.OrderItems.FirstOrDefault(o => o.ProductId == ProductId);
                if (orderItem != null)
                {
                    orderItem.Quantity = orderItem.Quantity + Quantity;
                    product.Quantity = product.Quantity - Quantity;
                    db.SaveChanges();
                    return Ok(new { success = "Product added to orders" });
                }
                OrderItem newOrderItem = new OrderItem();
                newOrderItem.Product = product;
                newOrderItem.Quantity = Quantity;
                newOrderItem.User = user;
                db.OrderItems.Add(newOrderItem);
                product.Quantity = product.Quantity - Quantity;
                db.SaveChanges();
                return Ok(new { success = "Product added to orders" });
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

                var orderList = db.OrderItems
                    .Where(b => b.UserId == user.Id)
                    .Select(b => new
                    {
                        b.Product,
                        b.Quantity
                    }).ToList();

                var totalCount = orderList.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var products = orderList
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

                var orderItem = db.OrderItems.FirstOrDefault(b => b.ProductId == ProductId);
                if (orderItem == null)
                {
                    return BadRequest(new { error = "Product not exist" });
                }
                if (user.Id == orderItem.UserId)
                {
                    db.OrderItems.Remove(orderItem);
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
        public async Task<ActionResult> ChangeProduct(int ProductId, int Quantity)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(email));

                var orderItem = db.OrderItems.FirstOrDefault(b => b.ProductId == ProductId);
                if (orderItem == null)
                {
                    return BadRequest(new { error = "Product not exist" });
                }
                if (Quantity < orderItem.Quantity)
                {
                    return BadRequest(new { error = "Unacceptable quantity" });
                }
                if (user.Id == orderItem.UserId)
                {
                    orderItem.Quantity = Quantity;
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