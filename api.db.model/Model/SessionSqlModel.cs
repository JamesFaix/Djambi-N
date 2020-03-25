using System;
using System.ComponentModel.DataAnnotations;

namespace Apex.Api.Db.Model
{
    public class SessionSqlModel
    {
        [Required]
        public int Id { get; set; }

        [Required] 
        public string Token { get; set; }

        [Required]
        public UserSqlModel User { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime ExpiresOn { get; set; }
    }
}
