namespace MyPhotoshop
{
	public class Photo
	{
		public int Width { get; }
		public int Height { get; }
		public Pixel[,] Data { get; }

		public Photo(int width, int height)
		{
			Width = width;
			Height = height;
			Data = new Pixel[Width, Height];
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Data[x, y] = new Pixel();
                }
            }
        }
	}
}