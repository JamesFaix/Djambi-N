using System;

namespace Apex.Api.Web.Model
{
    public class SessionDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    }

    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}