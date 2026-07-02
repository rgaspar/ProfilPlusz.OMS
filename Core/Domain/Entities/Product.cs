using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public string? Name { get; set; }                                           // Megnevezés Mi
    public string? Number { get; set; }                                         // Cikkszám Mi
    public string? FactoryName { get; set; }                                    // Gyári megnevezés
    public string? Description { get; set; }                                    // Termék leírás webhaz

    // mennyiségek
    public decimal? SalesUnitQuantity { get; set; }                             // Eladási mennyiségi egység
    public decimal? MinimumSalesQuantity { get; set; }                          // Minimum eladási mennyiség
    public decimal? OrderQuantityStep { get; set; }                             // Minimum rendelési mennyiség többszöröse
    public int? PackageQuantity { get; set; }                                   // Kiszerelés gyári     // Pl egy kartonban, raklapon

    // azonosítók
    public string? Ean { get; set; }                                            // Vonalkód (EAN)
    public string? ManufacturerNumber { get; set; }                             // Gyártói cikkszám

    // készlet
    public bool IsStockProduct { get; set; }                                    // Készletes termék?
    public int? WarningStock { get; set; }                                      // Figyelmeztető készlet
    public int? MinimumStock { get; set; }                                      // Minimum készlet

    // egyedi azonosítás
    public bool HasSerialNumber { get; set; }                                   // Gyári számos?        // Pl lejárati dátumos vagy egyéb cikkszámon belüli megkülönböztető

    // kategorizálás
    public string? ProductGroupId { get; set; }                                 // Termék kategória 1
    public ProductGroup? ProductGroup { get; set; }
    public string? Manufacturer { get; set; }                                   // Gyártó ( ez több okból is kell)
    public string? BrandId { get; set; }                                        // Brand
    public Brand? Brand { get; set; }

    // tulajdonságok
    public string? ColorId { get; set; }                                        // Szín
    public Color? Color { get; set; }
    public decimal? Weight { get; set; }                                        // Súly
    public decimal? Length { get; set; }                                        // Hossz

    // média
    public string? Image1Url { get; set; }                                      // Fénykép 1
    public string? Image2Url { get; set; }                                      // Fénykép 2
    public string? Image3Url { get; set; }                                      // Fénykép 3
    public string? VideoUrl { get; set; }                                       // Termék video
    public string? PdfUrl { get; set; }                                         // PDF feltöltések

    // pénznem
    public Currency PurchaseCurrency { get; set; }                               // Beszerzési pénznem
    public Currency SalesCurrency { get; set; }                                  // Eladási pénznem

    // státusz
    public ProductStatus Status { get; set; } = ProductStatus.Active;           // Termékstátusz

    // meglévő mezők
    public decimal? UnitPrice { get; set; }
    public bool Physical { get; set; } = true;
    public string? UnitMeasureId { get; set; }                                  // Eladási mennyiségi egység (ha külön törzs)
    public UnitMeasure? UnitMeasure { get; set; }
}