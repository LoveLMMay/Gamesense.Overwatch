using System.Runtime.Serialization;

namespace Gamesense.Overwatch.Models
{
	[DataContract]
	public class GameEvent
	{
		[DataMember(Name = "game")]
		public string Game;
		[DataMember(Name = "event")]
		public string EventName;
		[DataMember(Name = "data")]
		public GameEventData Data;

		public static GameEvent GetGameEvent(string name)
		{
			return new GameEvent
			{
				Game = "OVERWATCH",
				EventName = name,
				Data = new GameEventData { Value = 1 }
			};
		}
	}

	[DataContract]
	public class GameEventData
	{
		[DataMember(Name = "value")]
		public int Value;
	}
}