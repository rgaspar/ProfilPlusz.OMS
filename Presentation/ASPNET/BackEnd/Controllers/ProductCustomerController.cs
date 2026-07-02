using Application.Features.ProductCustomerManager.Commands;
using Application.Features.ProductCustomerManager.Queries;
using ASPNET.BackEnd.Common.Base;
using ASPNET.BackEnd.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers;

[Route("api/[controller]")]
public class ProductCustomerController(ISender sender) : BaseApiController(sender)
{
    [Authorize]
    [HttpGet("GetProductCustomerList")]
    public async Task<ActionResult<ApiSuccessResult<GetProductCustomerListResult>>> GetProductCustomerListAsync(
        CancellationToken cancellationToken,
        [FromQuery] bool isDeleted = false)
    {
        var response = await _sender.Send(new GetProductCustomerListRequest { IsDeleted = isDeleted }, cancellationToken);

        return Ok(new ApiSuccessResult<GetProductCustomerListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetProductCustomerListAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("CreateProductCustomer")]
    public async Task<ActionResult<ApiSuccessResult<CreateProductCustomerResult>>> CreateProductCustomerAsync(
        [FromBody] CreateProductCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateProductCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateProductCustomerAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("UpdateProductCustomer")]
    public async Task<ActionResult<ApiSuccessResult<UpdateProductCustomerResult>>> UpdateProductCustomerAsync(
        [FromBody] UpdateProductCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateProductCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateProductCustomerAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("DeleteProductCustomer")]
    public async Task<ActionResult<ApiSuccessResult<DeleteProductCustomerResult>>> DeleteProductCustomerAsync(
        [FromBody] DeleteProductCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteProductCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteProductCustomerAsync)}",
            Content = response
        });
    }
}
