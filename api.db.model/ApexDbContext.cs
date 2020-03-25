using Microsoft.EntityFrameworkCore;

namespace Apex.Api.Db.Model
{
    public class ApexDbContext : DbContext
    {
        public DbSet<EventSqlModel> Events { get; set; }
        public DbSet<EventKindSqlModel> EventKinds { get; set; }
        public DbSet<GameSqlModel> Games { get; set; }
        public DbSet<GameStatusSqlModel> GameStatuses { get; set; }
        public DbSet<NeutralPlayerNameSqlModel> NeutralPlayerNames { get; set; }
        public DbSet<PlayerKindSqlModel> PlayerKinds { get; set; }
        public DbSet<PlayerSqlModel> Players { get; set; }
        public DbSet<PlayerStatusSqlModel> PlayerStatuses { get; set; }
        public DbSet<PrivilegeSqlModel> Privileges { get; set; }
        public DbSet<SessionSqlModel> Sessions { get; set; }
        public DbSet<SnapshotSqlModel> Snapshots { get; set; }
        public DbSet<UserSqlModel> Users { get; set; }
    }
}
