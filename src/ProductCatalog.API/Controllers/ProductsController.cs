using Microsoft.AspNetCore.Mvc;
using ProductCatalog.API.Models;
using ProductCatalog.Application.Common.DTOs;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Products.Commands.CreateProduct;
using ProductCatalog.Application.Products.Commands.DeleteProduct;
using ProductCatalog.Application.Products.Commands.UpdateProduct;
using ProductCatalog.Application.Products.Commands.UpdateProductStock;
using ProductCatalog.Application.Queries.GetAllProducts;
using ProductCatalog.Application.Queries.GetProduct;

namespace ProductCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var products = await dispatcher.Send(new GetAllProductsQuery(), cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await dispatcher.Send(new GetProductQuery(id), cancellationToken);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Description, request.Price, request.Currency, request.Stock);
        var productId = await dispatcher.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = productId }, productId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(id, request.Name, request.Description, request.Price, request.Currency);
        await dispatcher.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);
        await dispatcher.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/stock")]
    public async Task<ActionResult> UpdateStockAsync(Guid id, [FromBody] UpdateStockRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductStockCommand(id, request.Quantity);
        await dispatcher.Send(command, cancellationToken);

        return NoContent();
    }
}