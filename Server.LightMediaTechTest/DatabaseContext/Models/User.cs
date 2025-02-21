using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.DatabaseContext.Models
{
    [Table("Users")]
    [Index(nameof(Id), Name = "IX_Users_Id", IsUnique = true)]
    [PrimaryKey(nameof(Id))]
    public class User : EntityBase
    {
        #region Display Properties

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        [MaxLength(255)]
        public string? AccountName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        [MaxLength(255)]
        public string DisplayName { get; set; } = "_TEMP";

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        [MaxLength(255)]
        public string? Email { get; set; }

        [Required]
        public DateTime? DoB { get; set; }

        [Key]
        [ForeignKey(nameof(UserRole))]
        public int UserRoleId { get; set; }
        public virtual UserRole? UserRoles { get; set; }
        #endregion

        #region System Properties

        public string PasswordHash { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public DateTime? LastLogin { get; set; }

        public string? LastLoginLocation { get; set; } = string.Empty;


        public virtual ICollection<EventUser> EventUsers { get; set; } = [];

        #endregion
    }
}
