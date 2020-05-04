<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main()
{
	var path = @"C:\Source\GeneSequenceImager";
	
	var file = "USA 20-03-2020";
	//var file = @"AUS 25-01-2020";

	var sequence = GetSequence(Path.Combine(path, $"{file}.txt"));
	var width = (int) Math.Ceiling(Math.Sqrt(sequence.Length));
	var height = (int) Math.Ceiling(Math.Sqrt(sequence.Length));

	using (var image = new DirectBitmap(width, height))
	{
		for (int i = 0; i < sequence.Length; i++)
		{
			var colour = GetColour(sequence[i]);

			var x = i % height;
			var y = i / width;

			image.SetPixel(x, y, colour);
		}
		image.Bitmap.Save(Path.Combine(path, $"{file}.png"), ImageFormat.Png);
	}
}

private Color GetColour(char c)
{
	switch (c)
	{
		case 'g':
			return Color.Red;
		case 'a':
			return Color.Blue;
		case 't':
			return Color.Green;
		case 'c':
			return Color.Yellow;
		default:
			return Color.White;
	}
}

private char[] GetSequence(string filename)
{
	var output = new List<char>();
	
	var input = File.ReadLines(filename);
	foreach (var line in input)
	{
		foreach (var c in line.Skip(10).Where(c => c != ' '))
		{
			output.Add(c);
		}
	}
	return output.ToArray();
}

public class DirectBitmap : IDisposable
{
	public Bitmap Bitmap { get; private set; }
	public Int32[] Bits { get; private set; }
	public bool Disposed { get; private set; }
	public int Height { get; private set; }
	public int Width { get; private set; }

	protected GCHandle BitsHandle { get; private set; }

	public DirectBitmap(int width, int height)
	{
		Width = width;
		Height = height;
		Bits = new Int32[width * height];
		BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
		Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
	}

	public void SetPixel(int x, int y, Color colour)
	{
		int index = x + (y * Width);
		int col = colour.ToArgb();

		Bits[index] = col;
	}

	public Color GetPixel(int x, int y)
	{
		int index = x + (y * Width);
		int col = Bits[index];
		Color result = Color.FromArgb(col);

		return result;
	}

	public void Dispose()
	{
		if (Disposed) return;
		Disposed = true;
		Bitmap.Dispose();
		BitsHandle.Free();
	}
}
