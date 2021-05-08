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
    [ApiVersion ("1.0")]
    [Route("v{v:apiVersion}/[controller]")]
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

        // Get request.  Use an id.  If the data is incorrect the response is gona be a 404
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

        //Add an item using a post request

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product);
        }

        //Update an item using a put request

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            //if the id doesn't match the product i will return bad request 
            if (id != product.Id)
            {
                return BadRequest();
            }
            //Here i will do the modification
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if the product is not there ( in the db) any longer
                if (_context.Products.Find(id) == null)
                {
                    return NotFound();
                }

                throw;
            }
            return NoContent();


            {

            }
        }

        //To delete an item

        [HttpDelete("{id}")]
        // Here I can use a Task<IActionResult> too 
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            //Look for the product
            var product = await _context.Products.FindAsync(id);

            //If the product is not there
            if (product == null)
            {
                return NotFound();
            }
            //If the product is found delete it using Remove
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }

        [HttpPost]
        [Route ("Delete")]
        public async Task<IActionResult> DeleteManyProducts ([FromQuery] int[] ids)
        { 
            //Create an empty list of deleted products
            List<Product> DeletedProducts = new List<Product>() ;
            
            // process products one by one
            for (int i = 0; i < ids.Length; i++)
            {
                //Look for the product by id
                Product myProduct = await _context.Products.FindAsync(ids[i]);
                //If they product is not found 
                if (myProduct==null)
                {
                    return NotFound();
                }
              // If the product is found we add it to the list of items to delete
                DeletedProducts.Add(myProduct);
            }
            //delete all the items required at the same time.  Using RemoveRange
            _context.Products.RemoveRange(DeletedProducts);
            await _context.SaveChangesAsync();

            return Ok(DeletedProducts);
        }



    }
}
