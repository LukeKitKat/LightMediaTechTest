using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.DatabaseManager.Models
{
    [Table("UserRoles")]
    [Index(nameof(Id), Name = "IX_UserRoles_Id", IsUnique = true)]
    [PrimaryKey(nameof(Id))]
    public class UserRole : EntityBase
    {
        public string RoleName { get; set; } = string.Empty;
        
        public bool CanManageUsers { get; set; }
        public bool CanExportDetails { get; set; }
        public bool CanAmendEvents { get; set; }

        public virtual ICollection<User>? Users { get; set; } = new HashSet<User>()!;
    }
}
