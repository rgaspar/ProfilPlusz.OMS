using Application.Common.Services.ExcelImport;
using Application.Features.PriceListManager.Commands;
using Application.Features.PriceListManager.Queries;
using ASPNET.BackEnd.Common.Base;
using ASPNET.BackEnd.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPNET.BackEnd.Controllers;

[Route("api/[controller]")]
public class PriceListController(ISender sender, ExcelImportService excelImportService) : BaseApiController(sender)
{
    [Authorize]
    [HttpPost("CreatePriceList")]
    public async Task<ActionResult<ApiSuccessResult<CreatePriceListResult>>> CreatePriceListAsync(
        [FromBody] CreatePriceListRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreatePriceListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreatePriceListAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("UpdatePriceList")]
    public async Task<ActionResult<ApiSuccessResult<UpdatePriceListResult>>> UpdatePriceListAsync(
        [FromBody] UpdatePriceListRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdatePriceListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdatePriceListAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("DeletePriceList")]
    public async Task<ActionResult<ApiSuccessResult<DeletePriceListResult>>> DeletePriceListAsync(
        [FromBody] DeletePriceListRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeletePriceListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeletePriceListAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetPriceList")]
    public async Task<ActionResult<ApiSuccessResult<GetPriceListResult>>> GetPriceListAsync(
        CancellationToken cancellationToken,
        [FromQuery] bool isDeleted = false,
        [FromQuery] string? productId = null)
    {
        var request = new GetPriceListRequest { IsDeleted = isDeleted, ProductId = productId };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPriceListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPriceListAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("ImportPriceListFromExcel")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> ImportPriceListFromExcelAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Nincs feltöltött fájl.");

        if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Csak .xlsx fájl fogadható el.");

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("A fájl mérete nem haladhatja meg a 10 MB-ot.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        using var stream = file.OpenReadStream();

        var request = new ImportPriceListFromExcelRequest
        {
            ExcelStream = stream,
            CreatedById = userId
        };

        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<ImportPriceListFromExcelResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"{response.SuccessCount} árlistasor sikeresen importálva.",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetPriceListImportTemplate")]
    public IActionResult GetPriceListImportTemplate()
    {
        var headers = new[]
        {
            "Árucikk szám", "Vevőkód", "ÁFA", "Nettó ár", "Bruttó ár",
            "Mennyiségi kedvezmény", "Kedvezménytől", "Kedvezményig"
        };

        var bytes = excelImportService.GenerateTemplate(headers);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "arlista-import-template.xlsx");
    }
}
