using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSMServer.Model
{
    public class UserFile
    {
        public string Email { get; set; }

        public IFormFile File { get; set; }
    }
}
