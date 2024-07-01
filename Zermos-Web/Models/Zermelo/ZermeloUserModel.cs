using System.Collections.Generic;

namespace Zermos_Web.Models.zermeloUserModel
{


    public class Datum
    {
        public string displayName;
        public string code { get; set; }
        public List<object> roles { get; set; }
        public string firstName { get; set; }
        public string prefix { get; set; }
        public string lastName { get; set; }
    }

    public class Response
    {
        public List<Datum> data { get; set; }
    }

    public class ZermeloUserModel
    {
        public Response response { get; set; }
    }
}