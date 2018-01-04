using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace MVC
{
    public static class VariaveisGlobal
    {
        public static HttpClient WebApiClient = new HttpClient();

        static VariaveisGlobal()
        {
            WebApiClient.BaseAddress = new Uri("http://localhost:56440/api/");
            WebApiClient.DefaultRequestHeaders.Clear();
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}