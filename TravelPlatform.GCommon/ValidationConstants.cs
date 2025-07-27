namespace TravelPlatform.GCommon
{
    public static class ValidationConstants
    {
		public static class UserValidationConstants
		{
			public const int BioMaxLength = 500;
		}

		public static class DestinationValidationConstants
		{
			public const int DestinationNameMaxLength = 60;
			public const int DestinationNameMinLength = 4;
			public const int DestinationTownMaxLength = 50;
			public const int DestinationTownMinLength = 3;
			public const int DestinationCountryMaxLength = 100;
			public const int DestinationCountryMinLength = 3;
		}

		public static class PostValidationConstants
		{
			public const int PostTitleMaxLength = 60;
			public const int PostTitleMinLength = 5;
			public const int PostContentMaxLength = 2000;
		}

		public static class CommentValidationConstants
		{
			public const int CommentContentMaxLength = 1000;
			public const int CommentContentMinLength = 5;
		}

		public static class NotificationValidationConstants
		{
			public const int NotificationMessageMaxLength = 100;
		}

		public static class MessageContactValidationConstants
		{
			public const int NamesMaxLength = 50;
			public const int NamesMinLength = 2;
			public const int SubjectMaxLength = 100;
			public const int SubjectMinLength = 5;
			public const int ContentMaxLength = 1000;
			public const int ContentMinLength = 10;
		}
	}
}
