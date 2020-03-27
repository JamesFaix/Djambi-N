using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Sessions")]
    public class SessionSqlModel
    {
        [Key]
        [Required]
        public int SessionId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public int UserId { get; set; }
        public UserSqlModel User { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime ExpiresOn { get; set; }
    }
}
