using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

public static class Program
{
	private static Vector2 ClickPosition = new(2750, 900);
	private static Vector2 CheckStart    = new(2665, 620);
	private static Vector2 Close         = new(2915, 565);
	private static Vector2 Play          = new(2600, 1375);

	private static DateTime                   _nextStart;
	private static MouseOperations.MousePoint _lastPoint;

	private static int _minDistance = 50;
	private static int _maxDistance = 150;

	private static int _count;
	
	
	
	public static void Main()
	{
		var tasks = new List<Task>
				{
					new(async () => await Check()),
					// new(async () => await Click()),
					// new(async () => await TimeLeft()),
				};
		Parallel.ForEach(tasks, task => task.Start());
		
		Console.ReadLine();
	}

	
	
	private static async Task Check()
	{
		_lastPoint = MouseOperations.GetCursorPosition();
		
		while (true)
		{
			if (MouseOperations.GetCursorPosition().X == _lastPoint.X && 
			    MouseOperations.GetCursorPosition().Y == _lastPoint.Y) continue;

			_lastPoint = MouseOperations.GetCursorPosition();
			var color = ColorAt(_lastPoint.X, _lastPoint.Y);
			Console.WriteLine($"{_lastPoint.X} {_lastPoint.Y} || {color.R} {color.G} {color.B}");
		}
	}

	private static async Task TimeLeft()
	{
		while (true)
		{
			Console.Clear();
			var time = $"{(int) _nextStart.Subtract(DateTime.Now).TotalSeconds / 60:00}:{(int) _nextStart.Subtract(DateTime.Now).TotalSeconds % 60:00}\t{(_count > 0 ? _count : "")}";
			Console.WriteLine(time);
			Thread.Sleep(1000);
		}
	}
	
	private static async Task Click()
	{
		while (true)
		{
			if (DateTime.Compare(_nextStart, DateTime.Now) > 0) continue;
			
			_nextStart = DateTime.Now.AddSeconds(150 + 450.Random());
			ClickSslx();
		}
	}
	
	

	private static void ClickSslx()
	{
		ClickAt(Play, 100, 100);
		Thread.Sleep(4000);

		var angle    = 120.Random()                           - 60;
		var distance = (_maxDistance - _minDistance).Random() + _minDistance;

		_count = 50 + 100.Random();
		for (; _count > 0; _count--)
		{
			var wait = 0;
			for (var j = 0; j < 3; j++)
			{
				ClickAndWait(ClickLocation(ref distance, ref angle, j), ref wait);
			}

			var period = 175 + 50.Random();
			Thread.Sleep(period - wait > 0 ? period - wait : 25);
		}

		_count = 0;
		
		Thread.Sleep(3000 + 2000.Random());
		
		ClickAt(Close, 100, 100);
	}

	private static Vector2 ClickLocation(ref int distance, ref int angle, int i)
	{
		Tweak(ref distance, 20, _minDistance, _maxDistance);

		var xBonus = (int) (Math.Cos(angle) * (60.Random() - 30));
		var yBonus = (int) (Math.Sin(angle) * (60.Random() - 30));


		var aim = i switch
			    {
				    0 => new(ClickPosition.X + distance * (float) Math.Cos(angle.Radian()), ClickPosition.Y + distance * (float) Math.Sin(angle.Radian())),
				    1 => ClickPosition,
				    2 => new(ClickPosition.X - distance * (float) Math.Cos(angle.Radian()), ClickPosition.Y - distance * (float) Math.Sin(angle.Radian()))
			    };

		aim += new Vector2(xBonus, yBonus);
		
		return aim;
	}

	private static void Tweak(ref int value, int step, int min, int max)
	{
		int[] range = [Math.Max(value - step, min), Math.Min(value + step, max)];
		value = range[0] + (range[1] - range[0]).Random();
	}

	private static void ClickAndWait(Vector2 aim, ref int wait)
	{
		var firstwait  = 20 + 20.Random();
		var secondwait = 20 + 10.Random();
				
		ClickAt((int) aim.X, (int) aim.Y, firstwait, secondwait);
				
		wait += firstwait + secondwait;
	}
	
	
	
	

	private static void ClickAt(Vector2 vector, int wait1, int wait2)
	{
		ClickAt((int) vector.X, (int) vector.Y, wait1, wait2);
	}

	private static void ClickAt(int x, int y, int wait1, int wait2)
	{
		MouseOperations.SetCursorPosition(x, y);
		MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
		Thread.Sleep(wait1);
		MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
		Thread.Sleep(wait2);
	}
	
	
	
	
	
	[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
	static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
	static Bitmap screenPixel = new(1, 1, PixelFormat.Format32bppArgb);

	private static Color ColorAt(int x, int y)
	{
		using var gdest  = Graphics.FromImage(screenPixel);
		using var gsrc   = Graphics.FromHwnd(IntPtr.Zero);
		var       hSrcDC = gsrc.GetHdc();
		var       hDC    = gdest.GetHdc();
		var       retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, x, y, (int)CopyPixelOperation.SourceCopy);
		gdest.ReleaseHdc();
		gsrc.ReleaseHdc();

		return screenPixel.GetPixel(0, 0);
	}
}