using Application.Common.Services.ExcelImport;
using Application.Features.CustomerManager.Commands;
using Application.Features.CustomerManager.Queries;
using ASPNET.BackEnd.Common.Base;
using ASPNET.BackEnd.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPNET.BackEnd.Controllers;

[Route("api/[controller]")]
public class CustomerController(ISender sender, ExcelImportService excelImportService) : BaseApiController(sender)
{

    [Authorize]
    [HttpPost("CreateCustomer")]
    public async Task<ActionResult<ApiSuccessResult<CreateCustomerResult>>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateCustomerAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("UpdateCustomer")]
    public async Task<ActionResult<ApiSuccessResult<UpdateCustomerResult>>> UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateCustomerAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("DeleteCustomer")]
    public async Task<ActionResult<ApiSuccessResult<DeleteCustomerResult>>> DeleteCustomerAsync(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteCustomerAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetCustomerList")]
    public async Task<ActionResult<ApiSuccessResult<GetCustomerListResult>>> GetCustomerListAsync(
        CancellationToken cancellationToken,
        [FromQuery] bool isDeleted = false
        )
    {
        var request = new GetCustomerListRequest { IsDeleted = isDeleted };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCustomerListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCustomerListAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("ImportCustomersFromExcel")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> ImportCustomersFromExcelAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Nincs feltöltött fájl.");

        if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Csak .xlsx fájl fogadható el.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        using var stream = file.OpenReadStream();

        var request = new ImportCustomersFromExcelRequest
        {
            ExcelStream = stream,
            CreatedById = userId
        };

        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<ImportCustomersFromExcelResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"{response.SuccessCount} vevő sikeresen importálva.",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetCustomerImportTemplate")]
    public IActionResult GetCustomerImportTemplate()
    {
        var headers = new[]
        {
            "Ügyfél neve", "Ügyfélcsoport", "Ügyfélkategória", "E-mail",
            "Adószám", "Közösségi adószám", "Kapcsolattartó neve",
            "E-mail - visszaigazolás", "E-mail - számlázás", "E-mail - beszerzés",
            "Telefon", "Ország", "Irányítószám", "Város", "Utca, házszám",
            "Bankszámlaszám", "Számla típusa", "Fizetés módja", "Fizetési határidő", "Pénznem"
        };

        var bytes = excelImportService.GenerateTemplate(headers);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "vevo-import-sablon.xlsx");
    }
}


