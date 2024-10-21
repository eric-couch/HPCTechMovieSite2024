using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPCTechMovieSite2024.Shared.Wrapper;

public class Response
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }

    public Response()
    {
        Errors = new Dictionary<string, string[]>();
    }

    public Response(bool succeeded, string message)
    {
        Success = succeeded;
        Message = message;
    }

    public Response(string message)
    {
        Errors = new Dictionary<string, string[]>();
        Success = false;
        Message = message;
    }
}
