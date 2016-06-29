using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IMAGECONVERT
{
	[ApiVersion(1, 23)]
	public class ImageConvert : TerrariaPlugin
	{
		Bitmap[] Image = new Bitmap[255];
		private static string savepath = Path.Combine(TShock.SavePath, "ImageConvert\\");
		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}
		public override string Author
		{
			get { return "Ancientgods"; }
		}
		public override string Name
		{
			get { return "ImageConvert"; }
		}

		public override string Description
		{
			get { return "Spawns images in your terraria world"; }
		}
		public ImageConvert(Main game)
			: base(game)
		{
			Order = 1;
		}

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command("imageconvert", image, "image"));
			if (!Directory.Exists(savepath))
			{
				Directory.CreateDirectory(savepath);
			}
			loadinfo();
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			base.Dispose(disposing);
		}

		public bool checkextension(string s)
		{
			switch (s.Split('.')[1])
			{
				case "png":
				case "jpeg":
				case "jpg":
				case "bmp":
				case "gif":
				case "tiff":
					return true;
			}
			return false;
		}

		private void image(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("Invalid syntax! proper syntax: /image <draw/load/list>");
				return;
			}
			switch (args.Parameters[0])
			{
				case "list":
					#region list
					string[] result = Directory.GetFiles(savepath).Where(f => checkextension(f)).ToArray();
					for (int i = 0; i < result.Length; i++)
						args.Player.SendInfoMessage(result[i]);
					#endregion list
					return;
				case "load":
					#region load image
					if (args.Parameters.Count < 2)
					{
						args.Player.SendErrorMessage("Invalid syntax! proper syntax: /image load <file name>");
						return;
					}
					string name = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
					if (!LoadImage(args, name))
					{
						args.Player.SendInfoMessage("Image loading failed!");
						return;
					}
					Bitmap image = Image[args.Player.Index];
					args.Player.SendInfoMessage("Image loaded successfully!");
					args.Player.SendInfoMessage(string.Format("Image Height: {0}, Image Width: {1}", image.Height, image.Width));
					#endregion load image
					return;
				case "draw":
					#region draw image
					if (Image[args.Player.Index] == null)
					{
						args.Player.SendErrorMessage("No image selected!");
						return;
					}
					args.Player.SendInfoMessage("Drawing image...");
					ThreadPool.QueueUserWorkItem((o) =>
					{
						if (!DrawImage(args))
						{
							args.Player.SendErrorMessage("Failed to draw image!");
							return;
						}
						UpdateTiles();
						args.Player.SendSuccessMessage("Image successfully drawn!");
					});
					#endregion draw image
					return;
				default:
					args.Player.SendErrorMessage("Invalid syntax! proper syntax: /image <draw/load/list>");
					return;
			}
		}

		public bool DrawImage(CommandArgs args)
		{
			try
			{
				int TileX = args.Player.TileX;
				int TileY = args.Player.TileY;
				Bitmap bitmap = Image[args.Player.Index];
				Image[args.Player.Index] = null;
				for (int i = 0; i < bitmap.Width; i++)
				{
					for (int j = 0; j < bitmap.Height; j++)
					{
						System.Drawing.Color pixel = bitmap.GetPixel(i, j);
						Color c = new Color(pixel.R, pixel.G, pixel.B, pixel.A);
						Main.tile[TileX + i, TileY + j] = FindTileByColor(c);
					}
				}
				return true;
			}
			catch (Exception ex) { TShock.Log.ConsoleError(ex.ToString()); return false; }
		}


		public static void UpdateTiles()
		{
			foreach (TSPlayer ts in TShock.Players)
			{
				if (ts == null || !ts.Active)
					return;

				for (int i = 0; i < 255; i++)
				{
					for (int j = 0; j < Main.maxSectionsX; j++)
					{
						for (int k = 0; k < Main.maxSectionsY; k++)
						{
							Netplay.Clients[i].TileSections[j, k] = false;
						}
					}
				}
			}
		}

		public bool LoadImage(CommandArgs args, string imagename)
		{
			try
			{
				imagename = imagename.Replace(".png", "");
				imagename += ".png";
				if (!File.Exists(Path.Combine(savepath, imagename)))
				{
					args.Player.SendErrorMessage("File does not exist!");
					return false;
				}

				Image[args.Player.Index] = new Bitmap(Path.Combine(savepath, imagename));

				if (Image[args.Player.Index].Height > Main.maxTilesY - 100 || Image[args.Player.Index].Width > Main.maxTilesX - 150)
				{
					args.Player.SendErrorMessage("Image is too large!");
					Image[args.Player.Index] = null;
					return false;
				}
				return true;

			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.ToString());
				return false;
			}
		}

		Dictionary<Color, int> Tiles = new Dictionary<Color, int>();
		public void loadinfo()
		{
			#region tiles
			Tiles.Add(new Color(151, 107, 75), 191);
			Tiles.Add(new Color(128, 128, 128), 38);
			Tiles.Add(new Color(140, 101, 80), 6);
			Tiles.Add(new Color(150, 67, 22), 47);
			Tiles.Add(new Color(185, 164, 23), 45);
			Tiles.Add(new Color(185, 194, 195), 46);
			Tiles.Add(new Color(98, 95, 167), 140);
			Tiles.Add(new Color(109, 90, 128), 25);
			Tiles.Add(new Color(104, 86, 84), 37);
			Tiles.Add(new Color(181, 62, 59), 39);
			Tiles.Add(new Color(146, 81, 68), 40);
			Tiles.Add(new Color(66, 84, 109), 41);
			Tiles.Add(new Color(84, 100, 63), 43);
			Tiles.Add(new Color(107, 68, 99), 44);
			Tiles.Add(new Color(200, 246, 254), 54);
			Tiles.Add(new Color(43, 40, 84), 56);
			Tiles.Add(new Color(68, 68, 76), 57);
			Tiles.Add(new Color(142, 66, 66), 76);
			Tiles.Add(new Color(92, 68, 73), 120);
			Tiles.Add(new Color(26, 26, 26), 75);
			Tiles.Add(new Color(11, 80, 143), 121);
			Tiles.Add(new Color(91, 169, 169), 122);
			Tiles.Add(new Color(128, 26, 52), 111);
			Tiles.Add(new Color(181, 172, 190), 117);
			Tiles.Add(new Color(238, 225, 218), 118);
			Tiles.Add(new Color(107, 92, 108), 119);
			Tiles.Add(new Color(211, 236, 241), 148);
			Tiles.Add(new Color(190, 171, 94), 151);
			Tiles.Add(new Color(128, 133, 184), 152);
			Tiles.Add(new Color(104, 100, 126), 157);
			Tiles.Add(new Color(145, 81, 85), 158);
			Tiles.Add(new Color(148, 133, 98), 158);
			Tiles.Add(new Color(144, 195, 232), 161);
			Tiles.Add(new Color(174, 145, 214), 163);
			Tiles.Add(new Color(218, 182, 204), 164);
			Tiles.Add(new Color(129, 125, 93), 175);
			Tiles.Add(new Color(62, 82, 114), 167);
			Tiles.Add(new Color(132, 157, 127), 176);
			Tiles.Add(new Color(152, 171, 198), 177);
			Tiles.Add(new Color(73, 120, 17), 188);
			Tiles.Add(new Color(223, 255, 255), 189);
			Tiles.Add(new Color(182, 175, 130), 190);
			Tiles.Add(new Color(26, 196, 84), 3);
			Tiles.Add(new Color(56, 121, 255), 193);
			Tiles.Add(new Color(157, 157, 107), 194);
			Tiles.Add(new Color(134, 22, 34), 195);
			Tiles.Add(new Color(147, 144, 178), 196);
			Tiles.Add(new Color(97, 200, 225), 197);
			Tiles.Add(new Color(62, 61, 52), 198);
			Tiles.Add(new Color(216, 152, 144), 200);
			Tiles.Add(new Color(213, 178, 28), 202);
			Tiles.Add(new Color(128, 44, 45), 203);
			Tiles.Add(new Color(125, 55, 65), 204);
			Tiles.Add(new Color(124, 175, 201), 206);
			Tiles.Add(new Color(88, 105, 118), 208);
			Tiles.Add(new Color(191, 233, 115), 211);
			Tiles.Add(new Color(239, 90, 50), 221);
			Tiles.Add(new Color(231, 96, 228), 222);
			Tiles.Add(new Color(57, 85, 101), 223);
			Tiles.Add(new Color(227, 125, 22), 225);
			Tiles.Add(new Color(141, 56, 0), 226);
			Tiles.Add(new Color(255, 156, 12), 229);
			Tiles.Add(new Color(131, 79, 13), 230);
			Tiles.Add(new Color(219, 71, 38), 248);
			Tiles.Add(new Color(235, 38, 231), 249);
			Tiles.Add(new Color(86, 85, 92), 250);
			Tiles.Add(new Color(235, 150, 23), 251);
			Tiles.Add(new Color(153, 131, 44), 252);
			Tiles.Add(new Color(57, 48, 97), 253);
			#endregion tiles

			#region walls
			//still have to do thisone
			#endregion walls
		}

		public Tile FindTileByColor(Color c)
		{
			Tile t = new Tile();
			if (c.A == 0)
			{
				t.active(false);
				return t;
			}
			t.active(true);
			t.inActive(true);
			Color closestColor = c;
			double diff = 99999999;
			foreach (KeyValuePair<Color, int> p in Tiles)
			{
				double i = Math.Pow(c.R - p.Key.R, 2) + Math.Pow(c.G - p.Key.G, 2) + Math.Pow(c.B - p.Key.B, 2);
				if (i < diff)
				{
					diff = i;
					closestColor = p.Key;
					t.type = (byte)p.Value;
				}
			}
			return t;
		}
	}
}