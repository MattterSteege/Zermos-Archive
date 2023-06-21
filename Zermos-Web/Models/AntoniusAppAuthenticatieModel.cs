namespace Zermos_Web.Models
{
    public class AntoniusAppAuthenticatieModel
    {
        public AntoniusAppAuthenticatieModelData data { get; set; }
    }

    public class AntoniusAppAuthenticatieModelData
    {
        public string id { get; set; }
        public string expires_at { get; set; }
        public string user_id { get; set; }
        public string customer_product_id { get; set; }
        public string status { get; set; }
        public int community_id { get; set; }
    }
    
    public class AntoniusAppAuthenticatieModelAuthSuccess
    {
        public string data { get; set; }
    }
}