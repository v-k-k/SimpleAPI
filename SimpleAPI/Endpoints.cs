using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAPI
{
    public static class Endpoints
    {
        public static readonly string auth = "http://localhost:8000/token";
        public static readonly string me = "http://localhost:8000/users/me";
        public static readonly string info = "http://localhost:8000/";
        public static readonly string send = "http://localhost:8000/send";
        public static readonly string update = "http://localhost:8000/update";
        public static readonly string delete = "http://localhost:8000/delete/";
        public static readonly string createCookie = "http://localhost:8000/cookie-and-object/";
    }
}
