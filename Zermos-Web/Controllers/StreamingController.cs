#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers;

public class StreamingController : Controller
{

    [HttpGet("/stream")]
    public async Task<IActionResult> Stream()
    {
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");
        Response.Headers.Add("X-Accel-Buffering", "no");
        Response.Headers.Add("Transfer-Encoding", "chunked");
        
        // Flush the headers to ensure the browser starts receiving the response
        await Response.Body.FlushAsync();
    
        // Write an initial message to the client. This will be shown as a message from the server
        var writer = new StreamWriter(Response.Body, Encoding.UTF8, bufferSize: 10240, leaveOpen: true);
    
        await writer.WriteLineAsync("<!--" + await ViewRenderer.RenderViewToStringAsync("_loading", null, ControllerContext)); // return the loading.cs PartialView
        await writer.WriteLineAsync(); // Separation between events
        await writer.FlushAsync();
    
        await Task.Delay(1000);
    
        //replace all the html with the new html
        await writer.WriteLineAsync("-->" + await ViewRenderer.RenderViewToStringAsync("Index", null, ControllerContext));
        await writer.WriteLineAsync(); // Separation between events
        await writer.FlushAsync();
        
        //close the stream
        await writer.DisposeAsync();
        return Ok();
    }
}
#endif