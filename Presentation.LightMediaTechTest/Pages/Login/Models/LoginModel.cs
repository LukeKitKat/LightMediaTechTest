using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Pages.Login.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(255)]
        public string LoginUserName = string.Empty;

        [Required]
        [MaxLength(255)]
        public string LoginPassword = string.Empty;
    }
}
