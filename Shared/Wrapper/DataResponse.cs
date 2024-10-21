using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HPCTechMovieSite2024.Shared.Wrapper;

public class DataResponse<T> : Response
{
    public T Data {  get; set; }

    public DataResponse()
    {
        
    }

    public DataResponse(T data)
    {
        Success = true;
        Data = data;
    }
}
