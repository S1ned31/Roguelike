using Godot;

namespace Roguelike.Entities
{
	public static class ImageLoader
	{
		private static ImageTexture _ImageTexture = new ImageTexture();

		public static ImageTexture LoadTexture(string path, bool isPixelTexture)
		{
			Godot.Image image = new Godot.Image();
			image.Load(path);

			if (isPixelTexture)
				_ImageTexture.CreateFromImage(image, 0);
			else
				_ImageTexture.CreateFromImage(image);

			return _ImageTexture;
		}
	}
}

