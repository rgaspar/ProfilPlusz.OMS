using Application.Features.BrandManager.Commands;
using Application.Features.BrandManager.Queries;
using ASPNET.BackEnd.Common.Base;
using ASPNET.BackEnd.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers;

[Route("api/[controller]")]
public class BrandController : BaseApiController
{
    public BrandController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpPost("CreateBrand")]
    public async Task<ActionResult<ApiSuccessResult<CreateBrandResult>>> CreateBrandAsync(
        [FromBody] CreateBrandRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateBrandResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateBrandAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("UpdateBrand")]
    public async Task<ActionResult<ApiSuccessResult<UpdateBrandResult>>> UpdateBrandAsync(
        [FromBody] UpdateBrandRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateBrandResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateBrandAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("DeleteBrand")]
    public async Task<ActionResult<ApiSuccessResult<DeleteBrandResult>>> DeleteBrandAsync(
        [FromBody] DeleteBrandRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteBrandResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteBrandAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetBrandSingle")]
    public async Task<ActionResult<ApiSuccessResult<GetBrandSingleResult>>> GetBrandSingleAsync(
        CancellationToken cancellationToken,
        [FromQuery] string id
    )
    {
        var request = new GetBrandSingleRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetBrandSingleResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetBrandSingleAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetBrandList")]
    public async Task<ActionResult<ApiSuccessResult<GetBrandListResult>>> GetBrandListAsync(
        CancellationToken cancellationToken,
        [FromQuery] bool isDeleted = false
    )
    {
        var request = new GetBrandListRequest { IsDeleted = isDeleted };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetBrandListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetBrandListAsync)}",
            Content = response
        });
    }
}