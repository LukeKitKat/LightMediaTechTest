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
    [Table("Events")]
    [Index(nameof(Id), Name = "IX_Events_Id", IsUnique = true)]
    [PrimaryKey(nameof(Id))]
    public class Event : EntityBase
    {
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        [MaxLength(255)]
        public string? EventName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(80)")]
        [MaxLength(80)]
        public string? EventShortDescription { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string? EventFullDescription { get; set; }

        [Key]
        [Required]
        [ForeignKey(nameof(EventCatagory))]
        public int? EventCatagoryId { get; set; }
        public virtual EventCatagory? EventCatagory {get;set;}

        [Required]
        public string? EventLocation { get; set; }

        [Required]
        public DateTime? EventDateTime { get; set; }
                
        public DateTime? PublishedDateTime { get; set; }

        public string? EventPicture { get; set; }

        public bool AcceptingBookings { get; set; }

        public virtual ICollection<EventUser> EventUsers { get; set; } = new HashSet<EventUser>()!;
    }
}
