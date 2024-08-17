using System.Collections.Generic;

namespace Zermos_Web.Models;

public class ProductData
{
    public string id { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string domain { get; set; }
    public object logo { get; set; }
}

public class InfowijsCustomerProductsModel
{
    public List<ProductData> data { get; set; }
}
