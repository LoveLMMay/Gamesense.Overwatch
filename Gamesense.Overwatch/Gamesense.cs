using System.Collections.Generic;
using Razer;
using Gamesense.Overwatch.Models;

namespace Gamesense.Overwatch
{
	public static class Gamesense
	{
		static HttpTool http;

		internal static void Initialize()
		{
			Util.WriteLog("### OVERWATCH - GAMESENSE ###");

			System.AppDomain.CurrentDomain.UnhandledException += (s, e) =>
			{
				Util.WriteLog($"EXCEPTION!: {((System.Exception)e.ExceptionObject).Message}");
			};

			http = new HttpTool();

			Util.WriteLog("Removing game registration");
			http.PostAsync("remove_game", new { game = "OVERWATCH" }).ConfigureAwait(true);

			Util.WriteLog("Sending game registration");
			http.PostAsync("game_metadata", GameMetaData.GetOverwatchRegistration()).ConfigureAwait(true);
			Util.WriteLog("Done sending game registration");
		}

		internal static void ReplicateKeyEffect(Keyboard.EFFECT_TYPE effect, Keyboard.CUSTOM_EFFECT_TYPE customEffectType)
		{
			Util.WriteLog($"Replicating KeyEffect: {effect}");
			var colList = RGBA.FromUintArray(customEffectType.Color);

			int key = 4;
			var b = EventBinding.GetOverwatchBinding("TEST", new List<BaseHandler>());
			foreach (var c in colList)
			{
				var h = PerKeyHandler.GetHandler(c, key);
				b.Handlers.Add(h);
				key++;
			}

			var e = GameEvent.GetGameEvent("TEST");

			Util.WriteLog("Binding TEST event");
			http.PostAsync("bind_game_event", b).ConfigureAwait(true);

			Util.WriteLog("Sending game event");
			http.PostAsync("game_event", e).ConfigureAwait(true);
		}
	}
}
