namespace MyPhotoshop
{
	public class Photo
	{
		public int Width { get; }
		public int Height { get; }
		private readonly Pixel[,] data;

		public Photo(int width, int height)
		{
			Width = width;
			Height = height;
			data = new Pixel[Width, Height];
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					data[x, y] = new Pixel();
				}
			}
		}

		public Pixel this[int x, int y]
		{
			get { return data[x, y]; }
		}
	}
}