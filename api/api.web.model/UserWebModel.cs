using System;

namespace Apex.Api.Web.Model
{
    public enum PrivilegeDto
    {
        EditUsers = 1,
        EditPendingGames = 2,
        OpenParticipation = 3,
        ViewGames = 4,
        Snapshots = 5
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PrivilegeDto[] Privileges { get; set; }
    }

    public class CreateUserRequestDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class CreationSourceDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Time { get; set; }
    }
}
