using System.Runtime.Serialization;

namespace Gamesense.Overwatch.Models
{
	[DataContract]
	public class GameMetaData
	{
		[DataMember(Name = "game")]
		public string GameName;
		[DataMember(Name = "game_display_name")]
		public string GameDisplayName;
		[DataMember(Name = "icon_color_id")]
		public int IconColorId;

		public static GameMetaData GetOverwatchRegistration()
		{
			return new GameMetaData
			{
				GameName = "OVERWATCH",
				GameDisplayName = "Overwatch",
				IconColorId = 4
			};
		}
	}
}
