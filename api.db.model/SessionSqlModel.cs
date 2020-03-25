using System;

namespace Apex.Api.Db.Model
{
    public class SessionSqlModel
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public UserSqlModel User { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
