using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Sessions")]
    public class SessionSqlModel
    {
        [Required]
        public int Id { get; set; }

        [Required] 
        public string Token { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public UserSqlModel User { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime ExpiresOn { get; set; }
    }
}
