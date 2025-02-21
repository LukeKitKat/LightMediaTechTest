using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.DatabaseContext.Models
{
    public class EntityBase
    {
        [Key]
        [Column(TypeName = "int")]
        public int Id { get; }
    }
}
