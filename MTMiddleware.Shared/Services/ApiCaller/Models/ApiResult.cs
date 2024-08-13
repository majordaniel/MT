using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Shared.Services;

public class ApiResult
{
    public bool IsSuccessfull { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Data { get; set; } = string.Empty;
}

//public class Errors
//{
//    [JsonProperty("$.DateofIncorporation")]
//    public List<string> DateofIncorporation { get; set; }
//}

public class Root
{
    public string type { get; set; }
    public string title { get; set; }
    public int status { get; set; }
    public string traceId { get; set; }
    //public Errors errors { get; set; }
}