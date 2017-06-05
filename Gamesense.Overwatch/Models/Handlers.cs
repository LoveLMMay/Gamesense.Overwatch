using System.Runtime.Serialization;

namespace Gamesense.Overwatch.Models
{
	public abstract class BaseHandler { }

	[DataContract]
	public class GeneralHandler : BaseHandler
	{
		[DataMember(Name = "device-type")]
		public string DeviceType { get; set; }
		[DataMember(Name = "zone")]
		public string Zone { get; set; }
		[DataMember(Name = "color")]
		public RGBA Color { get; set; }
		[DataMember(Name = "mode")]
		public string Mode { get; set; }

		public static GeneralHandler GetHandler(RGBA c)
		{
			return new GeneralHandler
			{
				DeviceType = "keyboard",
				Zone = "function-keys",
				Color = c,
				Mode = "color"
			};
		}
	}

	[DataContract]
	public class PerKeyHandler : BaseHandler
	{
		[DataMember(Name = "device-type")]
		public string DeviceType { get; set; }
		[DataMember(Name = "custom-zone-keys")]
		public int[] ZoneKeys { get; set; }
		[DataMember(Name = "color")]
		public RGBA Color { get; set; }
		[DataMember(Name = "mode")]
		public string Mode { get; set; }

		public static PerKeyHandler GetHandler(RGBA c, int keyId)
		{
			return new PerKeyHandler
			{
				DeviceType = "rgb-per-key-zones",
				ZoneKeys = new int[] { keyId },
				Color = c,
				Mode = "color"
			};
		}
	}
}