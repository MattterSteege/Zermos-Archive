#nullable enable
using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class Error
    {
        public int? status { get; set; }
        public string? title { get; set; }
    }

    public class InfowijsAccessTokenModel
    {
        public List<Error>? errors { get; set; }
        public string? data { get; set; }
    }
}