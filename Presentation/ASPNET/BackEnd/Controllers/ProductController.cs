using Application.Features.ProductManager.Commands;
using Application.Features.ProductManager.Queries;
using ASPNET.BackEnd.Common.Base;
using ASPNET.BackEnd.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ASPNET.BackEnd.Controllers;

[Route("api/[controller]")]
public class ProductController : BaseApiController
{
    public ProductController(ISender sender) : base(sender)
    {
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


