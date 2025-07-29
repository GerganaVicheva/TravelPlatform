using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection.Emit;
using TravelPlatform.Data.Models;
using TravelPlatform.GCommon;

namespace TravelPlatform.Web.Data;

public class TravelPlatformDbContext : IdentityDbContext
{
	public TravelPlatformDbContext(DbContextOptions<TravelPlatformDbContext> options)
		: base(options)
	{
	}

	public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

	public DbSet<Comment> Comments { get; set; } = null!;

	public DbSet<Destination> Destinations { get; set; } = null!;

	public DbSet<Follow> Follows { get; set; } = null!;

	public DbSet<Like> Likes { get; set; } = null!;

	public DbSet<Notification> Notifications { get; set; } = null!;

	public DbSet<Post> Posts { get; set; } = null!;

	public DbSet<MessageContact> MessagesContacts { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<ApplicationUser>().Property(x => x.Id).HasMaxLength(225);
		builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(225);
		builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(225);
		builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(225);
		builder.Entity<IdentityUserToken<string>>().Property(x => x.Name).HasMaxLength(112);
		builder.Entity<IdentityUserToken<string>>().Property(x => x.LoginProvider).HasMaxLength(112);

		builder.Entity<Post>(entity =>
		{
			entity.HasOne(p => p.User)
			.WithMany()
			.OnDelete(DeleteBehavior.Restrict);

			entity.HasOne(p => p.Destination)
				.WithMany(d => d.Posts)
				.OnDelete(DeleteBehavior.Restrict);
		});

		builder.Entity<Comment>(entity =>
		{
			entity.HasOne(c => c.Post)
				.WithMany(p => p.Comments)
				.OnDelete(DeleteBehavior.NoAction);

			entity.HasOne(c => c.User)
				.WithMany()
				.OnDelete(DeleteBehavior.Cascade);
		});

		builder.Entity<Like>(entity =>
		{
			entity.HasKey(lk => new { lk.UserId, lk.PostId });

			entity.HasOne(l => l.Post)
				.WithMany(p => p.Likes)
				.OnDelete(DeleteBehavior.NoAction);

			entity.HasOne(l => l.User)
				.WithMany()
				.OnDelete(DeleteBehavior.Cascade);
		});

		builder.Entity<Follow>(entity =>
		{
			entity.HasKey(f => new { f.FollowerId, f.FollowingId });

			entity.HasOne(f => f.Follower)
				.WithMany(u => u.Following)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasOne(f => f.Following)
				.WithMany(u => u.Followers)
				.OnDelete(DeleteBehavior.Restrict);
		});

		builder.Entity<Notification>(entity =>
		{
			entity.Property(n => n.Type)
				.HasConversion(
				t => t.ToString(), //When saving the entity to the database
				t => (NotificationType)Enum.Parse(typeof(NotificationType), t)); //When reading the entity from the database

			entity.HasOne(n => n.User)
				.WithMany()
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(n => n.FromUser)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);

			entity.HasOne(n => n.Post)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);

			entity.HasOne(n => n.Comment)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);
		});
	}
}
