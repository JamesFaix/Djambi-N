using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Apex.Api.Enums;
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
        public DbSet<UserPrivilegeSqlModel> UserPrivileges { get; set; }

        public ApexDbContext(DbContextOptions<ApexDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DisableCascadingDeletesOnForeignKeys(modelBuilder);
            PopulateStaticData(modelBuilder);
        }

        private void DisableCascadingDeletesOnForeignKeys(ModelBuilder modelBuilder)
        {
            var foreignKeys = modelBuilder.Model.GetEntityTypes()
               .SelectMany(e => e.GetForeignKeys());

            foreach (var fkey in foreignKeys)
            {
                fkey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private void PopulateStaticData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventKindSqlModel>(e =>
            {
                var eventKinds = GetValues<EventKind>()
                    .Select(id => new EventKindSqlModel
                    {
                        Id = id,
                        Name = id.ToString()
                    });
                e.HasData(eventKinds);
            });

            modelBuilder.Entity<GameStatusSqlModel>(e =>
            {
                var eventKinds = GetValues<GameStatus>()
                    .Select(id => new GameStatusSqlModel
                    {
                        Id = id,
                        Name = id.ToString()
                    });
                e.HasData(eventKinds);
            });
        }

        private static IEnumerable<TEnum> GetValues<TEnum>()
            where TEnum : Enum =>
            Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
    }
}
