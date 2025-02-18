using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.DatabaseManager.Models
{
    [Table("EventAttendees")]
    [Index(nameof(Id), Name = "IX_EventAttendees_Id", IsUnique = true)]
    [PrimaryKey(nameof(Id))]
    public class EventUser : EntityBase
    {
        [Key]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        [Key]
        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        public virtual Event? Event { get; set; }
    }
}
