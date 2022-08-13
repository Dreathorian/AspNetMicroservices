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
}
