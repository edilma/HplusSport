using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HPlusSport.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HplusSport.API.Models;
using Microsoft.EntityFrameworkCore;
using HplusSport.API.Classes;

namespace HplusSport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        // Method 1 -> get the products in a list
        //[HttpGet]   
        //public IEnumerable<Product>GetAllProducts() 
        //{
        //    return _context.Products.ToArray();
        //}


        // Method 2 - Use an IactionResult
        //[HttpGet]
        //public IActionResult GetAllProducts()
        //{
        //    return Ok(_context.Products.ToArray());
        //}

        // Give the id a data type.  If the data is incorrect the response is gona be a 404
        //[HttpGet("{id:int}")]
        //public IActionResult GetProduct(int id)
        //{
        //    var product = _context.Products.Find(id);
        //    if (product== null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(product);

     //  }


    [HttpGet]
        //to get all products GetAllProducts().  With Size and pagination (QueryParameters...)
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;

            if (queryParameters.MinPrice != null &&
                queryParameters.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price >= queryParameters.MinPrice.Value &&
                    p.Price <= queryParameters.MaxPrice.Value);
            }

            //To find an specific product sku
            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku);
            }

            // To find a specific product searching in the name and sku 


            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                products = products.Where(p => p.Name.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
                                            p.Sku.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }

            // To find a specific product name

            if ((!string.IsNullOrEmpty(queryParameters.Name)))
            {
                products = products.Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }


            // To sort the products
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }
            
            // To do the pagination 
            products = products
            .Skip(queryParameters.Size * (queryParameters.Page - 1))
            .Take(queryParameters.Size);


            return Ok(await products.ToArrayAsync());
        }

        // Give the id a data type.  If the data is incorrect the response is gona be a 404
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);

        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new  { id = product.Id },
                product);
        }


    }
}
