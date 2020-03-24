using System;

namespace Apex.Api.Web.Model
{
    public class Session
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}