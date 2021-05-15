using System;
using System.Threading;

using CSGSI;
using CSGSI.Events;


namespace GameStateIntegration
{
	class Program
	{
		public static GameStateManager GameState { get; private set; }
		public static OverlayManager Overlay { get; private set; }

		static void Main(string[] args)
		{
			Overlay = new();
			GameState = new();

			while (true)
			{
				Thread.Sleep(1);
			}
		
		}

		private static void CreateThread(object obj)
		{
			
		}

	
	}
}
