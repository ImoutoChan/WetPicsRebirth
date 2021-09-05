using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data
{
    public class WetPicsRebirthDbContext : DbContext
    {
        public WetPicsRebirthDbContext(DbContextOptions<WetPicsRebirthDbContext> options)
            : base(options)
        {
        }

        public DbSet<Actress> Actresses { get; private set; } = default!;

        public DbSet<PostedMedia> PostedMedia { get; private set; } = default!;

        public DbSet<Scene> Scenes { get; private set; } = default!;

        public DbSet<User> Users { get; private set; } = default!;

        public DbSet<Vote> Votes { get; private set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Actress>()
                .HasKey(x => x.Id);

            builder
                .Entity<Actress>()
                .HasOne(x => x.Scene)
                .WithMany(x => x!.Actresses)
                .HasForeignKey(x => x.ChatId)
                .HasPrincipalKey(x => x!.ChatId);

            builder
                .Entity<Scene>()
                .HasKey(x => x.ChatId);

            builder
                .Entity<PostedMedia>()
                .HasKey(x => x.Id);

            builder
                .Entity<PostedMedia>()
                .HasMany(x => x.Votes)
                .WithOne(x => x!.PostedMedia!)
                .HasForeignKey(x => x.PostedMediaId);

            builder
                .Entity<User>()
                .HasKey(x => x.Id);

            builder
                .Entity<User>()
                .HasMany(x => x.Votes)
                .WithOne(x => x!.User!)
                .HasForeignKey(x => x.UserId);

            builder
                .Entity<Vote>()
                .HasKey(x => new { x.UserId, x.PostedMediaId });

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entityEntry in ChangeTracker.Entries<IEntityBase>())
            {
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        entityEntry.Entity.AddedDate = DateTimeOffset.Now;
                        entityEntry.Entity.ModifiedDate = DateTimeOffset.Now;
                        break;
                    case EntityState.Modified:
                        entityEntry.Entity.ModifiedDate = DateTimeOffset.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
