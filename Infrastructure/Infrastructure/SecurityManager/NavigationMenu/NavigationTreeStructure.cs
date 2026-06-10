using Application.Common.Services.SecurityManager;
using System.Text.Json;

namespace Infrastructure.SecurityManager.NavigationMenu;






public class JsonStructureItem
{
    public string? URL { get; set; }
    public string? Name { get; set; }
    public bool IsModule { get; set; }
    public List<JsonStructureItem> Children { get; set; } = new List<JsonStructureItem>();
}

public static class NavigationTreeStructure
{

    public static readonly string JsonStructure = """
    [
        {
            "URL": "#",
            "Name": "Irányítópultok",
            "IsModule": true,
            "Children": [
                {
                    "URL": "/Dashboards/DefaultDashboard",
                    "Name": "Alapértelmezett",
                    "IsModule": false
                }
            ]
        },
        {
            "URL": "#",
            "Name": "Értékesítés",
            "IsModule": true,
            "Children": [
                {
                    "URL": "/CustomerGroups/CustomerGroupList",
                    "Name": "Ügyfélcsoport",
                    "IsModule": false
                },
                {
                    "URL": "/CustomerCategories/CustomerCategoryList",
                    "Name": "Ügyfélkategória",
                    "IsModule": false
                },
                {
                    "URL": "/Customers/CustomerList",
                    "Name": "Ügyfél",
                    "IsModule": false
                },
                {
                    "URL": "/CustomerContacts/CustomerContactList",
                    "Name": "Ügyfél kapcsolattartó",
                    "IsModule": false
                },
                {
                    "URL": "/SalesOrders/SalesOrderList",
                    "Name": "Értékesítési rendelés",
                    "IsModule": false
                },
                {
                    "URL": "/SalesReports/SalesReportList",
                    "Name": "Értékesítési riport",
                    "IsModule": false
                }
            ]
        },
        {
            "URL": "#",
            "Name": "Beszerzés",
            "IsModule": true,
            "Children": [
                {
                    "URL": "/VendorGroups/VendorGroupList",
                    "Name": "Szállítócsoport",
                    "IsModule": false
                },
                {
                    "URL": "/VendorCategories/VendorCategoryList",
                    "Name": "Szállítókategória",
                    "IsModule": false
                },
                {
                    "URL": "/Vendors/VendorList",
                    "Name": "Szállító",
                    "IsModule": false
                },
                {
                    "URL": "/VendorContacts/VendorContactList",
                    "Name": "Szállító kapcsolattartó",
                    "IsModule": false
                },
                {
                    "URL": "/PurchaseOrders/PurchaseOrderList",
                    "Name": "Beszerzési rendelés",
                    "IsModule": false
                },
                {
                    "URL": "/PurchaseReports/PurchaseReportList",
                    "Name": "Beszerzési riport",
                    "IsModule": false
                }
            ]
        },
        {
            "URL": "#",
            "Name": "Készletkezelés",
            "IsModule": true,
            "Children": [
                {
                    "URL": "/UnitMeasures/UnitMeasureList",
                    "Name": "Mértékegység",
                    "IsModule": false
                },
                {
                    "URL": "/ProductGroups/ProductGroupList",
                    "Name": "Termékcsoport",
                    "IsModule": false
                },
                {
                    "URL": "/Products/ProductList",
                    "Name": "Termék",
                    "IsModule": false
                },
                {
                    "URL": "/Warehouses/WarehouseList",
                    "Name": "Raktár",
                    "IsModule": false
                },
                {
                    "URL": "/DeliveryOrders/DeliveryOrderList",
                    "Name": "Kiszállítási megbízás",
                    "IsModule": false
                },
                {
                    "URL": "/SalesReturns/SalesReturnList",
                    "Name": "Értékesítési visszáru",
                    "IsModule": false
                },
                {
                    "URL": "/GoodsReceives/GoodsReceiveList",
                    "Name": "Árubevételezés",
                    "IsModule": false
                },
                {
                    "URL": "/PurchaseReturns/PurchaseReturnList",
                    "Name": "Beszerzési visszáru",
                    "IsModule": false
                },
                {
                    "URL": "/TransferOuts/TransferOutList",
                    "Name": "Készletkivét",
                    "IsModule": false
                },
                {
                    "URL": "/TransferIns/TransferInList",
                    "Name": "Készletbetét",
                    "IsModule": false
                },
                {
                    "URL": "/PositiveAdjustments/PositiveAdjustmentList",
                    "Name": "Pozitív korrekció",
                    "IsModule": false
                },
                {
                    "URL": "/NegativeAdjustments/NegativeAdjustmentList",
                    "Name": "Negatív korrekció",
                    "IsModule": false
                },
                {
                    "URL": "/Scrappings/ScrappingList",
                    "Name": "Selejtezés",
                    "IsModule": false
                },
                {
                    "URL": "/StockCounts/StockCountList",
                    "Name": "Leltár",
                    "IsModule": false
                },
                {
                    "URL": "/TransactionReports/TransactionReportList",
                    "Name": "Tranzakciós riport",
                    "IsModule": false
                },
                {
                    "URL": "/StockReports/StockReportList",
                    "Name": "Készletriport",
                    "IsModule": false
                },
                {
                    "URL": "/MovementReports/MovementReportList",
                    "Name": "Mozgási riportok",
                    "IsModule": false
                }
            ]
        },
        {
            "URL": "#",
            "Name": "Segédeszközök",
            "IsModule": true,
            "Children": [
                {
                    "URL": "/Todos/TodoList",
                    "Name": "Teendő",
                    "IsModule": false
                },
                {
                    "URL": "/TodoItems/TodoItemList",
                    "Name": "Teendő elem",
                    "IsModule": false
                }
            ]
        },
        {
            "URL": "#",
            "Name": "Felhasználókezelés",
            "IsModule": true,
            "Children": [
                {
                    "URL": "/Users/UserList",
                    "Name": "Felhasználók",
                    "IsModule": false
                },
                {
                    "URL": "/Roles/RoleList",
                    "Name": "Szerepkörök",
                    "IsModule": false
                }
            ]
        },
        {
            "URL": "#",
            "Name": "Profilok",
            "IsModule": true,
            "Children": [
                {
                    "URL": "/Profiles/MyProfile",
                    "Name": "Profilom",
                    "IsModule": false
                }
            ]
        },
        {
            "URL": "#",
            "Name": "Beállítások",
            "IsModule": true,
            "Children": [
                {
                    "URL": "/Companies/MyCompany",
                    "Name": "Cégem",
                    "IsModule": false
                },
                {
                    "URL": "/Taxs/TaxList",
                    "Name": "Adók",
                    "IsModule": false
                },
                {
                    "URL": "/NumberSequences/NumberSequenceList",
                    "Name": "Számsorozatok",
                    "IsModule": false
                }
            ]
        }
    ]
    """;

    public static List<MenuNavigationTreeNodeDto> GetCompleteMenuNavigationTreeNode()
    {
        var json = JsonStructure;

        var menus = JsonSerializer.Deserialize<List<JsonStructureItem>>(json);

        List<MenuNavigationTreeNodeDto> nodes = new List<MenuNavigationTreeNodeDto>();

        var index = 1;
        void AddNodes(List<JsonStructureItem> menuItems, string? parentId = null)
        {
            foreach (var item in menuItems)
            {
                var nodeId = index.ToString();
                if (item.IsModule)
                {
                    nodes.Add(new MenuNavigationTreeNodeDto(nodeId, item.Name ?? "", param_hasChild: true, param_expanded: false));
                }
                else
                {
                    nodes.Add(new MenuNavigationTreeNodeDto(nodeId, item.Name ?? "", parentId, item.URL));
                }

                index++;

                if (item.Children != null && item.Children.Count > 0)
                {
                    AddNodes(item.Children, nodeId);
                }
            }
        }

        if (menus != null) AddNodes(menus);

        return nodes;
    }

    public static string GetFirstSegmentFromUrlPath(string? path)
    {
        var result = string.Empty;
        if (path != null && path.Contains("/"))
        {
            string[] parts = path.Split("/");
            if (parts.Length > 2)
            {
                result = parts[1];
            }
        }
        return result;
    }

    public static List<string> GetCompleteFirstMenuNavigationSegment()
    {
        var json = JsonStructure;
        var menus = JsonSerializer.Deserialize<List<JsonStructureItem>>(json);
        var result = new List<string>();

        if (menus != null)
        {
            foreach (var item in menus)
            {
                ProcessMenuItem(item, result);
            }
        }

        return result;
    }

    private static void ProcessMenuItem(JsonStructureItem item, List<string> result)
    {
        if (!string.IsNullOrEmpty(item.URL) && item.URL != "#")
        {
            var segment = GetFirstSegmentFromUrlPath(item.URL);
            if (!string.IsNullOrEmpty(segment) && !result.Contains(segment))
            {
                result.Add(segment);
            }
        }

        if (item.Children != null)
        {
            foreach (var child in item.Children)
            {
                ProcessMenuItem(child, result);
            }
        }
    }


}

