using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
using TravelPlatform.Data.Models;
using TravelPlatform.Services.Core;
using TravelPlatform.ViewModels;
using TravelPlatform.Web.Data;

namespace Tests
{
	[TestFixture]
	public class Tests
	{
		private TravelPlatformDbContext _dbContext;
		private TravelService _travelService;
		private DashboardService _dashboardService;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<TravelPlatformDbContext>()
				.UseInMemoryDatabase(databaseName: "TravelTestDb")
				.Options;

			_dbContext = new TravelPlatformDbContext(options);

			_travelService = new TravelService(_dbContext);
			_dashboardService = new DashboardService(_dbContext);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext?.Dispose();
		}

		[Test]
		public async Task SendMessageAsync_WithValidEmail_AddsMessageAndReturnsTrue()
		{
			var model = new AddMessageInputModel
			{
				FirstName = "John",
				LastName = "Doe",
				Email = "john@example.com",
				Subject = "Hello",
				Content = "Test message"
			};

			var messagesCountBefore = _dbContext.MessagesContacts.Count();

			var result = await _travelService.SendMessageAsync(model);

			ClassicAssert.IsTrue(result);
			ClassicAssert.AreEqual(messagesCountBefore + 1, _dbContext.MessagesContacts.Count());
			var message = _dbContext.MessagesContacts.First();
			ClassicAssert.AreEqual("John", message.FirstName);
			ClassicAssert.AreEqual("john@example.com", message.Email);
		}

		[Test]
		public async Task SendMessageAsync_WithInvalidEmail_ReturnsFalseAndDoesNotAddMessage()
		{
			var model = new AddMessageInputModel
			{
				FirstName = "John",
				LastName = "Doe",
				Email = "invalid-email",
				Subject = "Hello",
				Content = "Test message"
			};

			var result = await _travelService.SendMessageAsync(model);

			ClassicAssert.IsFalse(result);
			ClassicAssert.AreEqual(0, _dbContext.MessagesContacts.Count());
		}

		[Test]
		public async Task EditUserProfileAsync_WithValidUser_UpdatesUserAndReturnsTrue()
		{
			// Arrange - add user
			var user = new ApplicationUser
			{
				Id = "user1",
				UserName = "OldName",
				Bio = "Old bio",
				ProfilePictureUrl = "oldpic.png"
			};
			_dbContext.ApplicationUsers.Add(user);
			await _dbContext.SaveChangesAsync();

			var model = new EditAccountInputModel
			{
				Username = "NewName",
				Bio = "New bio",
				ProfilePictureUrl = "newpic.png"
			};

			// Act
			var result = await _travelService.EditUserProfileAsync(model, user.Id);

			// Assert
			ClassicAssert.IsTrue(result);
			var updatedUser = _dbContext.ApplicationUsers.Find(user.Id);
			ClassicAssert.AreEqual("NewName", updatedUser.UserName);
			ClassicAssert.AreEqual("New bio", updatedUser.Bio);
			ClassicAssert.AreEqual("newpic.png", updatedUser.ProfilePictureUrl);
		}

		[Test]
		public async Task EditUserProfileAsync_WithNonExistentUser_ReturnsFalse()
		{
			var model = new EditAccountInputModel
			{
				Username = "NewName",
				Bio = "New bio",
				ProfilePictureUrl = "newpic.png"
			};

			var result = await _travelService.EditUserProfileAsync(model, "nonexistent");

			ClassicAssert.IsFalse(result);
		}

		[Test]
		public async Task DeleteProfileAsync_WithExistingUser_RemovesUserAndReturnsTrue()
		{
			var user = new ApplicationUser { Id = "user2", UserName = "DeleteMe" };
			_dbContext.ApplicationUsers.Add(user);
			await _dbContext.SaveChangesAsync();

			var result = await _travelService.DeleteProfileAsync(user.Id);

			ClassicAssert.IsTrue(result);
			ClassicAssert.IsNull(await _dbContext.ApplicationUsers.FindAsync(user.Id));
		}

		[Test]
		public async Task DeleteProfileAsync_WithNonExistentUser_ReturnsFalse()
		{
			var result = await _travelService.DeleteProfileAsync("nonexistent");

			ClassicAssert.IsFalse(result);
		}

		[Test]
		public async Task AddPostAsync_WithNewDestination_AddsPostAndDestination()
		{
			var user = new ApplicationUser { Id = "user3", UserName = "Poster" };
			_dbContext.ApplicationUsers.Add(user);
			await _dbContext.SaveChangesAsync();

			var destinationsCountBefore = _dbContext.Destinations.Count();
			var postsCountBefore = _dbContext.Posts.Count();

			var model = new AddPostInputModel
			{
				Title = "My Title",
				Content = "Post content",
				ImageUrl = "image.png",
				DestinationName = "DestName",
				DestinationTown = "DestTown",
				DestinationCountry = "DestCountry"
			};

			var result = await _travelService.AddPostAsync(model, user.Id);

			ClassicAssert.IsTrue(result);
			ClassicAssert.AreEqual(postsCountBefore + 1, _dbContext.Posts.Count());
			ClassicAssert.AreEqual(destinationsCountBefore + 1, _dbContext.Destinations.Count());
			var post = _dbContext.Posts.Include(p => p.Destination).First();
			ClassicAssert.IsNotNull(post);
			ClassicAssert.AreEqual("My Title", post.Title);
			ClassicAssert.AreEqual("DestName", post.Destination.Name);
		}

		[Test]
		public async Task AddPostAsync_WithExistingDestination_AddsPostOnly()
		{
			var user = new ApplicationUser { Id = "user4", UserName = "Poster" };
			var destination = new Destination
			{
				Name = "ExistDest",
				Town = "Town",
				Country = "Country"
			};
			_dbContext.ApplicationUsers.Add(user);
			_dbContext.Destinations.Add(destination);
			await _dbContext.SaveChangesAsync();

			var model = new AddPostInputModel
			{
				Title = "New Post",
				Content = "Content",
				ImageUrl = "img.png",
				DestinationName = "ExistDest",
				DestinationTown = "Town",
				DestinationCountry = "Country"
			};

			var result = await _travelService.AddPostAsync(model, user.Id);

			ClassicAssert.IsTrue(result);
			ClassicAssert.AreEqual(1, _dbContext.Posts.Count());
			ClassicAssert.AreEqual(1, _dbContext.Destinations.Count());
		}

		[Test]
		public async Task SendMessageAsync_WithMissingRequiredFields_ReturnsFalseAndDoesNotAddMessage()
		{
			var model = new AddMessageInputModel
			{
				FirstName = "",  // Missing
				LastName = "Doe",
				Email = "john@example.com",
				Subject = "",   // Missing
				Content = "Test message"
			};

			var result = await _travelService.SendMessageAsync(model);

			ClassicAssert.IsFalse(result);
			ClassicAssert.AreEqual(0, _dbContext.MessagesContacts.Count());
		}

		[Test]
		public async Task EditUserProfileAsync_WithEmptyFields_UpdatesUserCorrectly()
		{
			var user = new ApplicationUser
			{
				Id = "user5",
				UserName = "OldUser",
				Bio = "Old bio",
				ProfilePictureUrl = "old.png"
			};
			_dbContext.ApplicationUsers.Add(user);
			await _dbContext.SaveChangesAsync();

			var model = new EditAccountInputModel
			{
				Username = "",   // empty username
				Bio = null,     // null bio
				ProfilePictureUrl = ""  // empty pic URL
			};

			var result = await _travelService.EditUserProfileAsync(model, user.Id);

			ClassicAssert.IsTrue(result);
			var updatedUser = _dbContext.ApplicationUsers.Find(user.Id);
			ClassicAssert.AreEqual("", updatedUser.UserName);
			ClassicAssert.IsNull(updatedUser.Bio);
			ClassicAssert.AreEqual("", updatedUser.ProfilePictureUrl);
		}


		[Test]
		public async Task AddPostAsync_WithMissingDestinationInfo_ReturnsFalseAndDoesNotAddPost()
		{
			var user = new ApplicationUser { Id = "user6", UserName = "Poster6" };
			_dbContext.ApplicationUsers.Add(user);
			await _dbContext.SaveChangesAsync();

			var model = new AddPostInputModel
			{
				Title = "Title",
				Content = "Content",
				ImageUrl = "img.png",
				DestinationName = "",  // missing destination name
				DestinationTown = "",
				DestinationCountry = ""
			};

			var result = await _travelService.AddPostAsync(model, user.Id);

			ClassicAssert.IsFalse(result);
			ClassicAssert.AreEqual(0, _dbContext.Posts.Count());
		}

		[Test]
		public async Task DeleteProfileAsync_CalledTwice_ReturnsFalseSecondTime()
		{
			var user = new ApplicationUser { Id = "user7", UserName = "ToDelete" };
			_dbContext.ApplicationUsers.Add(user);
			await _dbContext.SaveChangesAsync();

			var firstDelete = await _travelService.DeleteProfileAsync(user.Id);
			var secondDelete = await _travelService.DeleteProfileAsync(user.Id);

			ClassicAssert.IsTrue(firstDelete);
			ClassicAssert.IsFalse(secondDelete);
		}

		[Test]
		public async Task AddPostAsync_WithNullUserId_ReturnsFalse()
		{
			var model = new AddPostInputModel
			{
				Title = "Test",
				Content = "Content",
				ImageUrl = "img.png",
				DestinationName = "Dest",
				DestinationTown = "Town",
				DestinationCountry = "Country"
			};

			var result = await _travelService.AddPostAsync(model, null);

			ClassicAssert.IsFalse(result);
		}

		[Test]
		public async Task GetDashboardTotalDataAsync_EmptyDatabase_ReturnsZeros()
		{
			var result = await _dashboardService.GetDashboardTotalDataAsync();

			Assert.That(result.TotalUsers, Is.EqualTo(0));
			Assert.That(result.TotalPosts, Is.EqualTo(0));
			Assert.That(result.TotalLikes, Is.EqualTo(0));
			Assert.That(result.TotalComments, Is.EqualTo(0));
		}
	}
}
