using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HPlusSport.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HplusSport.API.Models;

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


        [HttpGet]   
        public IEnumerable<Product>GetAllProducts() 
        {
            return _context.Products.ToArray();
        }
    }
}
