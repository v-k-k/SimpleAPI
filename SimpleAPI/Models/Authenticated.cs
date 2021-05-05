using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAPI.Models
{
    class Authenticated
    {
        public string username { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public bool disabled { get; set; }
        public string hashed_password { get; set; }
    }
}
