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


		//Seeding initial data into the db
		var users = GetSeedUsers();
		var destinations = GetSeedDestinations();
		var posts = GetSeedPosts(users, destinations);
		var comments = GetSeedComments(users, posts);
		var likes = GetSeedLikes(users, posts);
		var follows = GetSeedFollows(users);

		builder.Entity<ApplicationUser>().HasData(users);
		builder.Entity<Destination>().HasData(destinations);
		builder.Entity<Post>().HasData(posts);
		builder.Entity<Comment>().HasData(comments);
		builder.Entity<Like>().HasData(likes);
		builder.Entity<Follow>().HasData(follows);
	}

	private List<ApplicationUser> GetSeedUsers()
	{
		var hasher = new PasswordHasher<ApplicationUser>();

		var user1 = new ApplicationUser
		{
			Id = Guid.NewGuid().ToString(),
			UserName = "alice",
			NormalizedUserName = "ALICE",
			Email = "alice@example.com",
			NormalizedEmail = "ALICE@EXAMPLE.COM",
			EmailConfirmed = true,
			Bio = "Adventurer & travel blogger",
			ProfilePictureUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRMbIjmvU7JiOWP57G7gLNpJUfwhECEUJhIaA&s",
			SecurityStamp = Guid.NewGuid().ToString()
		};
		user1.PasswordHash = hasher.HashPassword(user1, "Password123!");

		var user2 = new ApplicationUser
		{
			Id = Guid.NewGuid().ToString(),
			UserName = "bob",
			NormalizedUserName = "BOB",
			Email = "bob@example.com",
			NormalizedEmail = "BOB@EXAMPLE.COM",
			EmailConfirmed = true,
			Bio = "Photographer who loves mountains",
			ProfilePictureUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTqWAEmYmBD-m3SJPY2svle5ZcoAxMQ7YKbzw&s",
			SecurityStamp = Guid.NewGuid().ToString()
		};
		user2.PasswordHash = hasher.HashPassword(user2, "Password123!");

		var user3 = new ApplicationUser
		{
			Id = Guid.NewGuid().ToString(),
			UserName = "charlie",
			NormalizedUserName = "CHARLIE",
			Email = "charlie@example.com",
			NormalizedEmail = "CHARLIE@EXAMPLE.COM",
			EmailConfirmed = true,
			Bio = "Beach lover and food explorer",
			ProfilePictureUrl = "https://i.pinimg.com/736x/22/5b/e5/225be5ea044c13b96e0d931b82831df7.jpg",
			SecurityStamp = Guid.NewGuid().ToString()
		};
		user3.PasswordHash = hasher.HashPassword(user3, "Password123!");

		return new List<ApplicationUser> { user1, user2, user3 };
	}

	private List<Destination> GetSeedDestinations() =>
		new()
		{
		new Destination { Id = 1, Name = "Eiffel Tower", Town = "Paris", Country = "France" },
		new Destination { Id = 2, Name = "Great Wall", Town = "Beijing", Country = "China" },
		new Destination { Id = 3, Name = "Grand Canyon", Town = "Arizona", Country = "USA" },
		new Destination { Id = 4, Name = "Sea Garden", Town = "Varna", Country = "Bulgaria" }
		};

	private List<Post> GetSeedPosts(List<ApplicationUser> users, List<Destination> destinations) =>
		new()
		{
		new Post
		{
			Id = 1,
			Title = "Sunset at the Eiffel Tower",
			Content = "One of the most beautiful sunsets I've ever seen.",
			ImageUrl = "https://i.pinimg.com/1200x/f0/36/60/f036605f3b16956a7e4639a46c308f62.jpg",
			CreatedOn = DateTime.UtcNow.AddDays(-10),
			UserId = users[0].Id,
			DestinationId = destinations[0].Id
		},
		new Post
		{
			Id = 2,
			Title = "Hiking the Great Wall",
			Content = "An unforgettable journey along history.",
			ImageUrl = "https://i.pinimg.com/1200x/3c/3a/f6/3c3af67b884757c3beaef78971249536.jpg",
			CreatedOn = DateTime.UtcNow.AddDays(-5),
			UserId = users[1].Id,
			DestinationId = destinations[1].Id
		}
		};

	private List<Comment> GetSeedComments(List<ApplicationUser> users, List<Post> posts) =>
		new()
		{
			new Comment
			{
				Id = 1,
				Content = "Wow, amazing shot!",
				CreatedOn = DateTime.UtcNow.AddDays(-9),
				PostId = posts[0].Id,
				UserId = users[1].Id
			},
			new Comment
			{
				Id = 2,
				Content = "I've always wanted to visit here.",
				CreatedOn = DateTime.UtcNow.AddDays(-4),
				PostId = posts[1].Id,
				UserId = users[0].Id
			}
		};

	private List<Like> GetSeedLikes(List<ApplicationUser> users, List<Post> posts) =>
		new()
		{
			new Like { PostId = posts[0].Id, UserId = users[1].Id, CreatedOn = DateTime.UtcNow.AddDays(-9) },
			new Like { PostId = posts[0].Id, UserId = users[2].Id, CreatedOn = DateTime.UtcNow.AddDays(-8) },
			new Like { PostId = posts[1].Id, UserId = users[0].Id, CreatedOn = DateTime.UtcNow.AddDays(-4) }
		};

	private List<Follow> GetSeedFollows(List<ApplicationUser> users) =>
		new()
		{
			new Follow { FollowerId = users[0].Id, FollowingId = users[1].Id, CreatedOn =
				DateTime.UtcNow.AddDays   (-20) },
			new Follow { FollowerId = users[1].Id, FollowingId = users[2].Id, CreatedOn =
				DateTime.UtcNow.AddDays   (-15) },
			new Follow { FollowerId = users[2].Id, FollowingId = users[0].Id, CreatedOn =
				DateTime.UtcNow.AddDays   (-12) }
		};
}
