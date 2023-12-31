﻿using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SimpleEcommerceV2.Main.Domain.Commands;
using SimpleEcommerceV2.Main.Domain.InOut.Requests;
using SimpleEcommerceV2.Main.Domain.Mappings;
using SimpleEcommerceV2.Main.Domain.Models;
using SimpleEcommerceV2.Main.Domain.Repositories.Contracts;
using SimpleEcommerceV2.Repositories.Contracts;

namespace SimpleEcommerceV2.Main.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/product")]
    public sealed class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IReadEntityRepository<ProductEntity> _readRepository;
        private readonly IStockReadRepository _stockReadRepository;
        public ProductController
        (
            IMediator mediator, 
            IReadEntityRepository<ProductEntity> readRepository,
            IStockReadRepository stockReadRepository,
            IHttpContextAccessor context
        )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
            _stockReadRepository = stockReadRepository ?? throw new ArgumentNullException(nameof(stockReadRepository));
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var products = await _readRepository.GetAllAsync(cancellationToken);
            var productsAsArrays = products as ProductEntity[] ?? products.ToArray();
            if (!productsAsArrays.Any())
                return NoContent();

            return Ok(productsAsArrays.MapToResponse());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var product = await _readRepository.GetByIdAsync(id, cancellationToken);
            if (product is null)
                return NotFound();

            return Ok(product.MapToResponse());
        }


        [HttpGet("{id:int}/stock")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetStockByProductIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var stock = await _stockReadRepository.GetByProductIdAsync(id, cancellationToken);
            if (stock is null)
                return NotFound();

            return Ok(stock.MapToResponse());
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddAsync([FromBody] ProductRequest productRequest, CancellationToken cancellationToken)
        {
            var createdProduct = await _mediator.Send(productRequest.MapToRegisterCommand(), cancellationToken);
            return Created(Request.GetDisplayUrl(),createdProduct);
        }

        [HttpPost("{id:int}/stock")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddAsync([FromRoute] int id, [FromBody] StockRequest stock, CancellationToken cancellationToken)
        {
            var createdStock = await _mediator.Send(stock.MapToRegisterProductStockCommand(id), cancellationToken);
            return Created(Request.GetDisplayUrl(), createdStock);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromRoute]int id, [FromBody] ProductRequest productRequest, CancellationToken cancellationToken)
        {
            var product = await _readRepository.GetByIdAsync(id, cancellationToken);
            if (product is null)
                return NotFound();
            
            var updatedProduct = await _mediator.Send(productRequest.MapToUpdateCommand(id), cancellationToken);
            return Ok(updatedProduct);
        }
        
        [HttpPut("{id:int}/stock")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromRoute]int id, [FromBody] StockRequest stock, CancellationToken cancellationToken)
        {
            var product = await _readRepository.GetByIdAsync(id, cancellationToken);
            if (product is null)
                return NotFound();
            
            var updatedStock = await _mediator.Send(stock.MapToUpdateProductStockCommand(id), cancellationToken);
            return Ok(updatedStock);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var product = await _readRepository.GetByIdAsync(id, cancellationToken);
            if (product is null)
                return NotFound();
            
            await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
            return Ok();
        }
    }
}
