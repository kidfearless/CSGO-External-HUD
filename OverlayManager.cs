using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Color = System.Drawing.Color;

using GameOverlay.Drawing;
using GameOverlay.Windows;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using static GameStateIntegration.Program;

namespace GameStateIntegration
{
	class OverlayManager : IDisposable
	{
		private Graphics Graphics { get; set; }
		private StickyWindow Window { get; set; }
		public int FrameCount { get; private set; }

		private Dictionary<string, IDisposable> Disposables { get; set; }
		const int Height = 1080;
		const int Width = 1920;

		public static readonly Vector2 TopLeft = new(0, 0);
		public static readonly Vector2 TopRight = new(Width, 0);
		public static readonly Vector2 BottomLeft = new(0, Height);
		public static readonly Vector2 BottomRight = new(Width, Height);
		public OverlayManager()
		{
			this.Disposables = new();
			var thread = new Thread(CreateOverlayManager)
			{
				Priority = ThreadPriority.Lowest,
				IsBackground = true
			};

			thread.Start();
		}

		private void CreateOverlayManager(object nill)
		{


			this.Graphics = new Graphics()
			{
				MeasureFPS = false,
				PerPrimitiveAntiAliasing = false,
				TextAntiAliasing = true,
				UseMultiThreadedFactories = true,
				VSync = false,
				WindowHandle = IntPtr.Zero
			};

			this.Window = new StickyWindow(0, 0, 1920, 1080, this.Graphics)
			{
				IsTopmost = true,
				IsVisible = true,
				FPS = 10
			};

			this.Disposables["Graphics"] = this.Graphics;
			this.Disposables["Window"] = this.Window;

			this.Window.Create();


			this.Window.DrawGraphics += Window_DrawGraphics;
			this.Window.SetupGraphics += Window_SetupGraphics;
		}

		private void Window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			Console.WriteLine("Setup graphics");
		}

		private void Window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
			this.Graphics.ClearScene();
			this.Graphics.BeginScene();
			var fontSize = 48;
			var factor = (int)(fontSize * 0.6);
			var pos = Vector2.Subtract(BottomLeft, new(-fontSize -12, fontSize + 12));
			var health = GameState.Health.ToString();
			this.DrawTextWithOutline(health, pos.X, pos.Y, fontSize,  Color.White, Color.Black);

			pos = new Vector2(164, 1020);
			var armor = GameState.Armor.ToString();
			this.DrawTextWithOutline(armor, pos.X, pos.Y, fontSize, Color.Blue, Color.Black);

			pos = Vector2.Subtract(BottomRight, new(164+fontSize, 12+ fontSize));

			var ammo = GameState.AmmoClip.ToString();
			this.DrawTextWithOutline(ammo, pos.X, pos.Y, fontSize, Color.White, Color.Black);

			pos = Vector2.Add(pos, new((int)(ammo.Length*factor), 0));

			this.DrawTextWithOutline("/", pos.X, pos.Y, fontSize, Color.White, Color.Black);

			pos = Vector2.Add(pos, new(20, 0));

			var reserveAmmo = GameState.AmmoReserve.ToString();
			this.DrawTextWithOutline(reserveAmmo, pos.X, pos.Y, fontSize, Color.White, Color.Black);


		}



		public void Dispose()
		{
			foreach (var item in this.Disposables)
			{
				item.Value.Dispose();
			}
		}

		[SuppressMessage(
			"Wrong Usage",
			"DF0100:Marks return values that hides the IDisposable implementation of return value.",
			Justification = "object is stored in the disposables property. Will be safely disposed of later.")]
		IBrush GetBrush(Color color)
		{
			var colorString = color.ToString();
			if (this.Disposables.TryGetValue(colorString, out IDisposable value))
			{
				return value as IBrush;
			}
			var brush = this.Graphics.CreateSolidBrush(color.R, color.G, color.B, color.A);
			this.Disposables[colorString] = brush;
			return brush;
		}

		Font GetFont(string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
		{
			var fontString = $"[f: {fontFamilyName} s:{size:N3} i:{italic} w:{wordWrapping}]";
			if (this.Disposables.TryGetValue(fontString, out IDisposable value))
			{
				return value as Font;
			}
			var font = this.Graphics.CreateFont(fontFamilyName, size, bold, italic, wordWrapping);
			this.Disposables[fontString] = font;
			return font;
		}

		#region drawing functions
		public void DrawBoxEdge(float x, float y, float width, float height, Color color, float thiccness = 2.0f)
		{
			var brush = GetBrush(color);
			this.Graphics.DrawRectangleEdges(brush, x, y, x + width, y + height, thiccness);
		}

		[SuppressMessage("Wrong Usage", "DF0010:Marks undisposed local variables.", Justification = "GetFont method handles disposing object")]
		public void DrawText(string text, float x, float y, int size, Color color, bool bold = false, bool italic = false)
		{
			var font = GetFont("RobotoMono", size, bold, italic);
			var brush = GetBrush(color);

			this.Graphics.DrawText(font, brush, x, y, text);
		}

		public void DrawTextWithOutline(string text, float x, float y, int size, Color color, Color outlinecolor, bool bold = true, bool italic = false)
		{
			DrawText(text, x - 1, y + 1, size, outlinecolor, bold, italic);
			DrawText(text, x + 1, y + 1, size, outlinecolor, bold, italic);
			DrawText(text, x, y, size, color, bold, italic);
		}

		[SuppressMessage("Wrong Usage", "DF0010:Marks undisposed local variables.", Justification = "GetFont method handles disposing object")]
		public void DrawTextWithBackground(string text, float x, float y, int size, Color color, Color backcolor, bool bold = false, bool italic = false)
		{

			var font = GetFont("Arial", size, bold, italic);
			var brush = GetBrush(color);
			var brush2 = GetBrush(backcolor);

			this.Graphics.DrawTextWithBackground(font, brush, brush2, x, y, text);
		}

		public void DrawLine(float fromx, float fromy, float tox, float toy, Color color, float thiccness = 2.0f)
		{
			var brush = GetBrush(color);

			this.Graphics.DrawLine(brush, fromx, fromy, tox, toy, thiccness);
		}

		public void DrawFilledBox(float x, float y, float width, float height, Color color)
		{
			var brush = GetBrush(color);

			this.Graphics.FillRectangle(brush, x, y, x + width, y + height);
		}

		public void DrawCircle(float x, float y, float radius, Color color, float thiccness = 1)
		{
			var brush = GetBrush(color);

			this.Graphics.DrawCircle(brush, x, y, radius, thiccness);
		}

		public void DrawCrosshair(CrosshairStyle style, float x, float y, float size, float thiccness, Color color)
		{
			var brush = GetBrush(color);

			this.Graphics.DrawCrosshair(brush, x, y, size, thiccness, style);
		}

		public void DrawFillOutlineBox(float x, float y, float width, float height, Color color, Color fillcolor, float thiccness = 1.0f)
		{
			var brush = GetBrush(color);
			var brush2 = GetBrush(fillcolor);

			this.Graphics.OutlineFillRectangle(brush, brush2, x, y, x + width, y + height, thiccness);
		}

		public void DrawBox(float x, float y, float width, float height, Color color, float thiccness = 2.0f)
		{
			var brush = GetBrush(color);

			this.Graphics.DrawRectangle(brush, x, y, x + width, y + height, thiccness);
		}

		public void DrawOutlineBox(float x, float y, float width, float height, Color color, float thiccness = 2.0f)
		{
			var brush = GetBrush(color);
			var brush2 = GetBrush(Color.FromArgb(0, 0, 0));
			this.Graphics.OutlineRectangle(brush2, brush, x, y, x + width, y + height, thiccness);
		}

		public void DrawRoundedBox(float x, float y, float width, float height, float radius, Color color, float thiccness = 2.0f)
		{
			var brush = GetBrush(color);

			this.Graphics.DrawRoundedRectangle(brush, x, y, x + width, y + height, radius, thiccness);
		}
		#endregion

	}
}
