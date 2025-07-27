using TravelPlatform.ViewModels;

namespace TravelPlatform.Services.Core.Contracts
{
    public interface ITravelService
    {
        Task<bool> SendMessageAsync(AddMessageInputModel model);

        Task<UserProfileViewModel> GetUserProfileInfoAsync(string userId);

        Task<EditAccountInputModel> GetUserProfileEditInfoAsync(string userId);

        Task<bool> EditUserProfileAsync(EditAccountInputModel model, string userId);

		Task<bool> DeleteProfileAsync(string userId);

        Task<bool> AddPostAsync(AddPostInputModel model, string userId);

        Task<EditPostInputModel> GetPostEditInfoAsync(int postId);

        Task<bool> EditPostAsync(EditPostInputModel model);

        Task<bool> DeletePostAsync(int postId);

        Task<bool> LikePostAsync(int postId, string userId);

		Task<IEnumerable<CommentViewModel>> GetPostCommentsAsync(int postId);

        Task<bool> AddCommentAsync(int postId, string userId, string content);

        Task<CommentViewModel> GetLastCommentAsync(int postId, string userId);

        Task<IEnumerable<SuggestedProfilesViewModel>> GetSuggestedProfilesInfoAsync(string userId);
	}
}
