using System.Net;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IProductRepository repository;
    private readonly ILogger<CatalogController> logger;

    public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<Product>>> GetProducts() => Ok(await repository.GetAll().ToListAsync());

    [HttpGet("{id:length(24)}", Name = "GetProduct")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> GetProductById(string id)
    {
        var product = await repository.GetById(id);

        if (product != null) return Ok(product);

        logger.LogError("Product with id: {Id}, not found", id);
        return NotFound();
    }

    [HttpGet("[action]/{category}", Name = nameof(GetProductByCategory))]
    [ProducesResponseType(typeof(Product[]), (int)HttpStatusCode.OK)]
    public async Task<List<Product>> GetProductByCategory(string category) => await repository.GetByCategory(category).ToListAsync();

    [HttpPost]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        await repository.Create(product);
        return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product) => Ok(await repository.Update(product));

    [HttpDelete("{id:length(24)}", Name = nameof(DeleteProduct))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteProduct(string id) => Ok(await repository.Delete(id));
}
