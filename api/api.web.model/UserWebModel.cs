using System;

namespace Apex.Api.Web.Model
{
    public enum Privilege
    {
        EditUsers = 1,
        EditPendingGames = 2,
        OpenParticipation = 3,
        ViewGames = 4,
        Snapshots = 5
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Privilege[] Privileges { get; set; }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class CreationSource
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Time { get; set; }
    }
}
