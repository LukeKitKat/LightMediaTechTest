using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.DatabaseContext.Models
{
    [Table("EventCatagories")]
    [Index(nameof(Id), Name = "IX_EventCatagories_Id", IsUnique = true)]
    [PrimaryKey(nameof(Id))]
    public class EventCatagory : EntityBase
    {
        public string? CatagoryName { get; set; }

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>()!;
    }
}
