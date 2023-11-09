﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Models;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

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

                var product = db.Products.FirstOrDefault(p => p.Id == ProductId);
                if(product == null)
                {
                    return BadRequest("Product not exist");
                }
                if(Quantity < product.Quantity)
                {
                    return BadRequest("Unacceptable quantity");
                }
                BasketItem basketItem = new BasketItem();
                basketItem.Product = product;
                basketItem.Quantity = Quantity;
                basketItem.User = user;
                db.BasketItems.Add(basketItem);
                db.SaveChanges();
                return Ok("Product added to basket");
            }
        }
        [HttpGet("getproducts")]
        [Authorize]
        public async Task<ActionResult> GetProducts()
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


                return Ok(basketList);
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
                    return BadRequest("Product not exist");
                }
                if(user.Id == basketItem.UserId)
                {
                    db.BasketItems.Remove(basketItem);
                    db.SaveChanges();
                    return Ok("Product removed from basket");
                }
                else
                {
                    return BadRequest("Product not exist");
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
                    return BadRequest("Product not exist");
                }
                if (Quantity < basketItem.Quantity)
                {
                    return BadRequest("Unacceptable quantity");
                }
                if (user.Id == basketItem.UserId)
                {
                    basketItem.Quantity = Quantity;
                    db.SaveChanges();
                    return Ok("Product removed from basket");
                }
                else
                {
                    return BadRequest("Product not exist");
                }
            }
        }

    }
}
