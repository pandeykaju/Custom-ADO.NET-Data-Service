
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using CustomADODataService.Models;
using CustomADODataService.Repository;
using System.Data;

namespace CustomADODataService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        private const string ProductsCacheKey = "AllProducts";

        public ProductsController(
            ProductRepository repository,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _repository = repository;
            _cache = cache;
            _configuration = configuration;
        }

        // GET: api/products
        // Connected ADO.NET model with memory caching
        [HttpGet]
        public ActionResult<List<Product>> GetAll()
        {
            if (_cache.TryGetValue(
                ProductsCacheKey,
                out List<Product>? cachedProducts)
                && cachedProducts != null)
            {
                Response.Headers["X-Cache"] = "HIT";
                return Ok(cachedProducts);
            }

            var products = _repository.GetAllProducts();

            int expirationMinutes =
                _configuration.GetValue<int>(
                    "CacheSettings:ExpirationMinutes", 5);

            expirationMinutes = Math.Max(1, expirationMinutes);

            _cache.Set(
                ProductsCacheKey,
                products,
                TimeSpan.FromMinutes(expirationMinutes));

            Response.Headers["X-Cache"] = "MISS";

            return Ok(products);
        }

        // GET: api/products/1
        [HttpGet("{id:int}")]
        public ActionResult<Product> GetById(int id)
        {
            var product = _repository.GetProductById(id);

            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public IActionResult Create(Product product)
        {
            int id = _repository.AddProduct(product);

            product.ProductId = id;

            // Clear outdated cache
            _cache.Remove(ProductsCacheKey);

            return CreatedAtAction(
                nameof(GetById),
                new { id },
                product);
        }

        // PUT: api/products/1
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, Product product)
        {
            product.ProductId = id;

            bool updated = _repository.UpdateProduct(product);

            if (!updated)
                return NotFound("Product not found.");

            // Clear outdated cache
            _cache.Remove(ProductsCacheKey);

            return NoContent();
        }

        // DELETE: api/products/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            bool deleted = _repository.DeleteProduct(id);

            if (!deleted)
                return NotFound("Product not found.");

            // Clear outdated cache
            _cache.Remove(ProductsCacheKey);

            return NoContent();
        }

        // GET: api/products/dataset
        // Disconnected ADO.NET model
        [HttpGet("dataset")]
        public IActionResult GetProductsDataSet()
        {
            var dataSet = _repository.GetProductsDataSet();

            var productsTable = dataSet.Tables["Products"];

            if (productsTable == null)
            {
                return NotFound("Products table not found.");
            }

            var products = productsTable.Rows
                .Cast<DataRow>()
                .Select(row => new
                {
                    ProductId =
                        Convert.ToInt32(row["ProductId"]),

                    ProductName =
                        Convert.ToString(row["ProductName"]),

                    Price =
                        Convert.ToDecimal(row["Price"]),

                    Quantity =
                        Convert.ToInt32(row["Quantity"])
                })
                .ToList();

            return Ok(products);
        }
    }
}
