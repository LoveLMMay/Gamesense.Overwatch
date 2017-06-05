using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gamesense.Overwatch.Models
{
	[DataContract]
	public class EventBinding
	{
		[DataMember(Name = "game")]
		public string Game;
		[DataMember(Name = "event")]
		public string EventName;
		[DataMember(Name = "min_value")]
		public int MinValue;
		[DataMember(Name = "max_value")]
		public int MaxValue;
		[DataMember(Name = "icon_id")]
		public int IconId;
		[DataMember(Name = "handlers")]
		public List<BaseHandler> Handlers;

		public static EventBinding GetOverwatchBinding(string name, List<BaseHandler> handlers)
		{
			return new EventBinding
			{
				Game = "OVERWATCH",
				EventName = name,
				MinValue = 0,
				MaxValue = 1,
				IconId = 1,
				Handlers = handlers
			};
		}
	}
}
