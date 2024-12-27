using System.Drawing;
using System.Security.Cryptography;

public static class Utility
{
	public static int Random(this int maxValue)
	{
		using var rg  = new RNGCryptoServiceProvider();
		var       rno = new byte[5];
		rg.GetBytes(rno);
		var value = BitConverter.ToInt32(rno, 0);
		return (int) ((value / (float) uint.MaxValue + 0.5f) * maxValue);
	}

	public static bool Is(this Color color, Color c)
	{
		return c.R == color.R &&
			 c.G == color.G &&
			 c.B == color.B;
	}

	public static double Radian(this int angle) => Math.PI * angle / 180d;
	public static decimal Gray(this Color color) => new List<decimal> {color.R, color.G, color.B}.Average();
}