using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.Models
{
    public class ServiceResponse
    {
        public bool Success { get; set; } = false;
        public List<string> Errors { get; set; } = [];
    }

    public class ServiceResponse<T>() : ServiceResponse
    {
        public T? Result { get; set; }
    }
}
