using Application.Features.ColorManager.Commands;
using Application.Features.ColorManager.Queries;
using ASPNET.BackEnd.Common.Base;
using ASPNET.BackEnd.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers;

[Route("api/[controller]")]
public class ColorController : BaseApiController
{
    public ColorController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpPost("CreateColor")]
    public async Task<ActionResult<ApiSuccessResult<CreateColorResult>>> CreateColorAsync(
        [FromBody] CreateColorRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateColorResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateColorAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("UpdateColor")]
    public async Task<ActionResult<ApiSuccessResult<UpdateColorResult>>> UpdateColorAsync(
        [FromBody] UpdateColorRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateColorResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateColorAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("DeleteColor")]
    public async Task<ActionResult<ApiSuccessResult<DeleteColorResult>>> DeleteColorAsync(
        [FromBody] DeleteColorRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteColorResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteColorAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetColorSingle")]
    public async Task<ActionResult<ApiSuccessResult<GetColorSingleResult>>> GetColorSingleAsync(
        CancellationToken cancellationToken,
        [FromQuery] string id
    )
    {
        var request = new GetColorSingleRequest
        {
            Id = id
        };

        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetColorSingleResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetColorSingleAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetColorList")]
    public async Task<ActionResult<ApiSuccessResult<GetColorListResult>>> GetColorListAsync(
        CancellationToken cancellationToken,
        [FromQuery] bool isDeleted = false
    )
    {
        var request = new GetColorListRequest
        {
            IsDeleted = isDeleted
        };

        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetColorListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetColorListAsync)}",
            Content = response
        });
    }
}