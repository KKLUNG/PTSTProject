using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSDProject.Models
{
    [Table("CMSUser")]
    public class CMSUser
    {
        [Key]
        public string UserId { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
    }
}
