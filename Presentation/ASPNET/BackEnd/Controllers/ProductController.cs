using Application.Common.Services.ExcelImport;
using Application.Features.ProductManager.Commands;
using Application.Features.ProductManager.Queries;
using ASPNET.BackEnd.Common.Base;
using ASPNET.BackEnd.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ASPNET.BackEnd.Controllers;

[Route("api/[controller]")]
public class ProductController : BaseApiController
{
    private readonly ExcelImportService _excelImportService;

    public ProductController(ISender sender, ExcelImportService excelImportService) : base(sender)
    {
        _excelImportService = excelImportService;
    }

    [Authorize]
    [HttpPost("CreateProduct")]
    public async Task<ActionResult<ApiSuccessResult<CreateProductResult>>> CreateProductAsync([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        string json = """
                        {
            	"number": "PTA-TEST-001",
            	"name": "Progress Proterminal Cream Alumínium L Profil 10 mm",
            	"factoryName": "Terminal Cream 10",
            	"description": "Teszt termék leírás webshophoz",
            	"unitPrice": 12.5,
            	"physical": true,
            	"unitMeasureId": "e3eab9a5-b861-4a45-8a4e-b43104560efc",
            	"productGroupId": "7ac53101-93fc-4abf-b3e0-b43104560f1a",
            	"manufacturer": "Progress",
            	"manufacturerNumber": "PTA 10-SL02",
            	"ean": "8031893208893",
            	"brandId": "439e0a5b-b0ef-48b6-b0b8-b43104560f26",
            	"colorId": "3df00e8d-145c-41e9-a7de-b43104560f3f",
            	"salesUnitQuantity": 1,
            	"minimumSalesQuantity": 1,
            	"orderQuantityStep": 20,
            	"packageQuantity": 20,
            	"isStockProduct": true,
            	"warningStock": 65,
            	"minimumStock": 40,
            	"hasSerialNumber": false,
            	"weight": 0.03,
            	"image1Url": "https://example.com/image1.jpg",
            	"image2Url": "https://example.com/image2.jpg",
            	"image3Url": "https://example.com/image3.jpg",
            	"videoUrl": "https://example.com/video.mp4",
            	"pdfUrl": "https://example.com/datasheet.pdf",
            	"purchaseCurrency": 1,
            	"salesCurrency": 2,
            	"status": 1,
            	"createdById": "USER_ID"
            }
            """;

        var obj = JsonSerializer.Deserialize<CreateProductRequest>(
    json,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });


        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateProductResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateProductAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("UpdateProduct")]
    public async Task<ActionResult<ApiSuccessResult<UpdateProductResult>>> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateProductResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateProductAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("DeleteProduct")]
    public async Task<ActionResult<ApiSuccessResult<DeleteProductResult>>> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteProductResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteProductAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpPost("ImportProductsFromExcel")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> ImportProductsFromExcelAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Nincs feltöltött fájl.");

        if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Csak .xlsx fájl fogadható el.");

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("A fájl mérete nem haladhatja meg a 10 MB-ot.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        using var stream = file.OpenReadStream();

        var request = new ImportProductsFromExcelRequest
        {
            ExcelStream = stream,
            CreatedById = userId
        };

        var response = await _sender.Send(request, cancellationToken);

        if (response.ErrorReportBytes.Length > 0)
        {
            return File(
                response.ErrorReportBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"import-jelentes-{DateTime.Now:yyyyMMdd-HHmmss}.xlsx");
        }

        var overwriteNote = response.OverwriteCount > 0
            ? $" ({response.OverwriteCount} felülírva)"
            : string.Empty;

        return Ok(new ApiSuccessResult<ImportProductsFromExcelResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"{response.SuccessCount} termék sikeresen importálva{overwriteNote}.",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetProductImportTemplate")]
    public IActionResult GetProductImportTemplate()
    {
        var headers = new[]
        {
            "Szám", "Név", "GyárNeve", "Leírás",
            "Egységár", "Mértékegység", "Termékcsoport", "Fizikai",
            "Gyártó", "GyártóiSzám", "EAN",
            "BeszerzésiPénznem", "ÉrtékesítésiPénznem", "Státusz"
        };

        var bytes = _excelImportService.GenerateTemplate(headers);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "termek-import-template.xlsx");
    }

    [Authorize]
    [HttpGet("GetProductList")]
    public async Task<ActionResult<ApiSuccessResult<GetProductListResult>>> GetProductListAsync(
        CancellationToken cancellationToken,
        [FromQuery] bool isDeleted = false
        )
    {
        var request = new GetProductListRequest { IsDeleted = isDeleted };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetProductListResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetProductListAsync)}",
            Content = response
        });
    }


}


