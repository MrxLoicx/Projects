using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
namespace TextManipulators
{
	public static class BinMan
	{
		public static string ByteToStr(byte b)
		{
		    StringBuilder str = new StringBuilder(8);
		    int[] bl  = new int[8];
		
		    for (int i = 0; i < bl.Length; i++)
		    {               
		        bl[bl.Length - 1 - i] = ((b & (1 << i)) != 0) ? 1 : 0;
		    }
		
		    foreach (int num in bl) str.Append(num);
		
		    return str.ToString();
		}
		
		// Перевод текста в бинарный код
		public static string TextToBin(string str)
		{
			StringBuilder sb = new StringBuilder();
			byte[] bytes = Encoding.Default.GetBytes(str);
			return string.Join(" ",
			                   bytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
		}
		
		// Перевод бинарного кода в текст
		public static string BinToText(string binaryStr)
		{
			var bytes = binaryStr.Split(' ').Select(x => Convert.ToByte(x,2)).ToArray();
 			return Encoding.Default.GetString(bytes);
		}
		
		// Перевод числа из десятичной в троичную систему счисления
		public static string DecToTre(int num)
		{
			string s = "";
            while (num > 0)
            {
                int t = num % 3;
                num /= 3;
                s += t.ToString();
            }
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new String(arr);
		}
		
		// Перевод числа из троичной в десятичную систему счисления
		public static int TreToDec(int number)
		{
			int result = 0;
 
		    for (int i = 0; number != 0; i++)
		    {
		    	result += Convert.ToInt32((number % 10) * Math.Pow(3, i));
		        number /= 10;
		    }
		    return result;
		}
		
		// Обмен 2-х нибблов в целом числе
		public static int SwapNibbles(int x) 
		{ 
		    return ((x & 0x0F) << 4 | (x & 0xF0) >> 4); 
		}
		
		// Получить 1-й ниббл байта
		public static byte GetNibble1(byte x)
		{
			return (byte) (x & 0x0F);
		}
		
		// Получить 2-й ниббл байта
		public static byte GetNibble2(byte x)
		{
			return (byte)((x & 0xF0) >> 4);
		}
		
		// Получить исходный байт из 2-х нибблов
		public static byte GetByteOriginalFromTwoNibbles(byte nibble1, byte nibble2)
		{
			return (byte)((nibble2 << 4) | nibble1);
		}
		
		/// <summary>
		/// Extracts a nibble from a large number.
		/// </summary>
		/// <typeparam name="T">Any integer type.</typeparam>
		/// <param name="t">The value to extract nibble from.</param>
		/// <param name="nibblePos">The nibble to check,
		/// where 0 is the least significant nibble.</param>
		/// <returns>The extracted nibble.</returns>
		public static byte GetNibble<T>(this T t, int nibblePos)
		 	where T : struct, IConvertible
		{
		 	nibblePos *= 4;
		 	var value = t.ToInt64(CultureInfo.CurrentCulture);
		 	return (byte)((value >> nibblePos) & 0xF);
		}
		
		// Возвращает сколько нат в указанном числе бит
		public static double BitToNat(int bits)
		{
			return bits / 1.443;
		}
		
		// Возвращает сколько бит в указанном числе нат
		public static double NatToBit(int nats)
		{
			return nats / 0.693;
		}
		
		// Возвращает сколько байт в указанном числе нат
		public static double NatToByte(int nats)
		{
			return nats / 5.545;
		}
	}
	
	public static class Datex
	{
		// Возвращает true если данная datetime строка равна текущей дате.
		public static bool IsToday(DateTime datenow)
		{
			return datenow == DateTime.UtcNow;
		}
		
		// Возвращает true если данная дата на текущей неделе.
		public static bool IsThisWeek(DateTime datenow)
		{
			return DateTime.UtcNow.DayOfWeek == datenow.DayOfWeek;
		}
		
		// Возвращает true если данная дата в текущем месяце.
		public static bool IsThisMonth(DateTime datenow)
		{
			int numberMonth = DateTime.UtcNow.Month;
			
			if (datenow.Month == numberMonth)
				return true;
			return false;
		}
		
		// Возвращает true если данная дата в текущем году.
		public static bool IsThisYear(DateTime datenow)
		{
			int numberYear = DateTime.UtcNow.Year;
			
			if (datenow.Year == numberYear)
				return true;
			return false;
		}
		
		// Возвращает true если данная дата была вчера
		public static bool WasYesterday(DateTime datenow)
		{
			int numberDay = DateTime.UtcNow.Day - 1;
			
			if (datenow.Day == numberDay)
				return true;
			return false;
		}
			
		// Возвращает true если данная дата будет завтра.
		public static bool IsTomorrow(DateTime datenow)
		{
			int numberDay = DateTime.UtcNow.Day + 1;
			
			if (datenow.Day == numberDay)
				return true;
			return false;
		}
		
		// Возвращает текущее время в Гринвиче 
		// Разница с Москвой - 3 часа
		public static DateTime GetNetworkDateTimeGMT()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Connect("time.nist.gov", 13);
                    using (StreamReader rstream = new StreamReader(new NetworkStream(socket)))
                    {
                        string value = rstream.ReadToEnd().Trim();
                        MatchCollection matches = Regex.Matches(value, @"((\d*)-(\d*)-(\d*))|((\d*):(\d*):(\d*))");
                        string[] dd = matches[0].Value.Split('-');
                        return DateTime.Parse(String.Format("{0} {1}.{2}.{3}", matches[1].Value, dd[2], dd[1], dd[0]));
                    }
                }
                catch
                {
                    return default(DateTime);
                }
            }
        }
	}
	
	
	class Program
	{
		public static void Main(string[] args)
		{
//			string bins = BinMan.TextToBin("Mrx get() 45");
//			Console.WriteLine(bins);
//			Console.WriteLine(BinMan.BinToText(bins));
			
//			Console.WriteLine(BinMan.DecToTre(4));
//			Console.WriteLine(BinMan.TreToDec(11));
			//Console.WriteLine(Datex.GetNetworkDateTimeGMT().Hour + 4);
			//Console.WriteLine(Datex.WasYesterday(DateTime.Parse("05/10/2020 00:00:00.000")));
			// Console.WriteLine(Datex.IsTomorrow(DateTime.Parse("07/10/2020 00:00:00.000")));
			
			// TEST : Обмен 2-х нибблов
//			Console.WriteLine(BinMan.SwapNibbles(12));
//			Console.WriteLine(BinMan.SwapNibbles(192));
			
//			byte original = 245;
//			byte n1, n2;
//			Console.WriteLine(n1 = BinMan.GetNibble1(original));
//			Console.WriteLine(n2 = BinMan.GetNibble2(original));
//			Console.WriteLine(BinMan.GetByteOriginalFromTwoNibbles(n1, n2));
			
			Console.ReadKey(true);
		}
	}
}
