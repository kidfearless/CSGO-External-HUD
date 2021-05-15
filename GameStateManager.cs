using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSGSI;
using CSGSI.Events;

namespace GameStateIntegration
{
	class GameStateManager
	{
		private GameStateListener Listener { get; set; }
		public int Money { get; set; }
		public int Health { get; set; }
		public int Armor { get; set; }
		public bool HasHelmet { get; set; }
		public int RoundKills { get; set; }
		public int AmmoClip { get; set; }
		public int AmmoReserve { get; set; }

		public GameStateManager()
		{
			Listener = new GameStateListener(3000);
			Listener.RoundBegin += Listener_RoundBegin;
			Listener.NewGameState += Listener_NewGameState;
			Listener.BombDefused += Listener_BombDefused;
			Listener.BombExploded += Listener_BombExploded;
			Listener.PlayerFlashed += Listener_PlayerFlashed;
			Listener.RoundEnd += Listener_RoundEnd;
			Listener.RoundPhaseChanged += Listener_RoundPhaseChanged;
			Listener.EnableRaisingIntricateEvents = true;
			if (!Listener.Start())
			{
				Console.WriteLine("Failed to start listener\n");
			}
		}

		private void Listener_RoundPhaseChanged(RoundPhaseChangedEventArgs e)
		{
			Console.WriteLine("Listener_RoundPhaseChanged");
		}

		private void Listener_RoundEnd(RoundEndEventArgs e)
		{
			Console.WriteLine("Listener_RoundEnd");
		}

		private void Listener_PlayerFlashed(PlayerFlashedEventArgs e)
		{
			Console.WriteLine("Listener_PlayerFlashed");
		}

		private void Listener_BombExploded(BombExplodedEventArgs e)
		{
			Console.WriteLine("Listener_BombExploded");
		}

		private void Listener_BombDefused(BombDefusedEventArgs e)
		{
			Console.WriteLine("Listener_BombDefused");
		}

		private void Listener_NewGameState(GameState gs)
		{
			try
			{
				this.Money = gs.Player.State.Money;
				this.Health = gs.Player.State.Health;
				this.Armor = gs.Player.State.Armor;
				this.HasHelmet = gs.Player.State.Helmet;
				this.RoundKills = gs.Player.State.RoundKills;
				this.AmmoClip = gs.Player.Weapons.ActiveWeapon.AmmoClip;
				this.AmmoReserve = gs.Player.Weapons.ActiveWeapon.AmmoReserve;
			}
			catch (Exception ex)
			{
			}
			Console.WriteLine(gs.JSON + "\n");
		}

		private static void Listener_RoundBegin(RoundBeginEventArgs e)
		{
			Console.WriteLine("Listener_RoundBegin");
		}
	}
}
