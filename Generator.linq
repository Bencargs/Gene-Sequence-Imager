<Query Kind="Program">
  <NuGetReference>QRCoder</NuGetReference>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>QRCoder</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
</Query>

void Main()
{
	var path = @"C:\Source\GeneSequenceImager";
	
	var images = new[]
	{
		GetImage(path, "23-12-2019"),
		GetImage(path, "11-01-2020"),
		GetImage(path, "15-01-2020"),
		GetImage(path, "17-01-2020"),
		GetImage(path, "22-01-2020"),
		GetImage(path, "11-02-2020"),
		GetImage(path, "28-02-2020"),
		GetImage(path, "20-03-2020"),
		GetImage(path, "17-04-2020"),
		GetImage(path, "24-04-2020"),
		GetImage(path, "30-04-2020"),
		GetImage(path, "04-05-2020"),
		GetImage(path, "05-05-2020"),
	};

	var qrCode = CreateQRCode(path, "05-05-2020");
	qrCode.Dump();

	var heatmap = CreateHeatmap(images);
	heatmap.Bitmap.Dump();
}

private Bitmap ResiseImage(Bitmap image, int width, int height)
{
	var resize = new Bitmap(width, height);
	using (var graph = Graphics.FromImage(resize))
	{
		graph.InterpolationMode = InterpolationMode.High;
		graph.CompositingQuality = CompositingQuality.HighQuality;
		graph.SmoothingMode = SmoothingMode.AntiAlias;
		graph.DrawImage(image, 0, 0, width, height);
	}
	return resize;
}

private Bitmap CreateDifferance(Bitmap[] images)
{
	var width = images.Min(x => x.Width);
	var height = images.Min(x => x.Height);
	var img = new DirectBitmap(width, height);
	for (int x = 0; x < width; x++)
	{
		for (int y = 0; y < height; y++)
		{
			var pixel = images[0].GetPixel(x, y);
			if (images.Skip(1).Any(i => i.GetPixel(x, y) != pixel))
				//img.SetPixel(x, y, Color.Black);
				img.SetPixel(x, y, Color.White);
			else
				//img.SetPixel(x, y, pixel);
				img.SetPixel(x, y, Color.Black);
		}
	}
	return img.Bitmap;
}

private Bitmap CreateQRCode(string path, string file)
{
	var fullpath = Path.Combine(path, $"{file}.txt");
	var sequence = GetSequence(Path.Combine(path, $"{file}.txt"));
	var compressed = GetHashString(new string(sequence));

	QRCodeGenerator qrGenerator = new QRCodeGenerator();
	QRCodeData qrCodeData = qrGenerator.CreateQrCode(compressed, QRCodeGenerator.ECCLevel.Q);
	QRCode qrCode = new QRCode(qrCodeData);
	Bitmap qrCodeImage = qrCode.GetGraphic(5);

	return qrCodeImage;
}

public static byte[] GetHash(string inputString)
{
	using (HashAlgorithm algorithm = SHA256.Create())
		return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
}

public static string GetHashString(string inputString)
{
	StringBuilder sb = new StringBuilder();
	foreach (byte b in GetHash(inputString))
		sb.Append(b.ToString("X2"));

	return sb.ToString();
}

private DirectBitmap CreateHeatmap(Bitmap[] images)
{
	var width = images.Min(x => x.Width);
	var height = images.Min(x => x.Height);

	var maxFrequency = 0;
	var histoMap = Enumerable.Range(0, width).Select(x =>
		Enumerable.Range(0, height).Select(y =>
			new Dictionary<Color, int>()).ToArray()).ToArray();

	foreach (var img in images)
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				var pixel = img.GetPixel(x, y);
				if (!histoMap[x][y].TryGetValue(pixel, out var frequency))
					histoMap[x][y].Add(pixel, ++frequency);

				if (histoMap[x][y].Keys.Count() > maxFrequency)
					maxFrequency = histoMap[x][y].Keys.Count();
			}
		}
	}

	var heatmap = new DirectBitmap(width, height);
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				var frequency = ((double)histoMap[x][y].Keys.Count / maxFrequency);
				var colour = ToGradient(frequency);
				heatmap.SetPixel(x, y, colour);
			}
		}
		return heatmap;
	}
}

private static Bitmap Gradient = (Bitmap)Bitmap.FromFile(@"C:\Source\GeneSequenceImager\viridisScale.png");
private Color ToGradient(double value)
{
	var scale = (int)(value * 99);
	return Gradient.GetPixel(0, scale);
}

private Bitmap GetImage(string path, string file)
{
	var filename = Path.Combine(path, $"{file}.txt");
	
	var sequence = GetSequence(filename);
	
	return GetImage(sequence).Bitmap;
}

private DirectBitmap GetImage(char[] sequence)
{
	var width = (int)Math.Ceiling(Math.Sqrt(sequence.Length));
	var height = (int)Math.Ceiling(Math.Sqrt(sequence.Length));
	var image = new DirectBitmap(width, height);
	{
		for (int i = 0; i < sequence.Length; i++)
		{
			var colour = GetColour(sequence[i]);

			var x = i % height;
			var y = i / width;

			image.SetPixel(x, y, colour);
		}
		return image;
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