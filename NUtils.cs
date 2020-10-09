using System;
using System.Security;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
namespace NUtils
{
	[Serializable]
  	public struct SimpleBitVector32
  	{
	    private int data;
	
	    internal SimpleBitVector32(int data)
	    {
	      this.data = data;
	    }

	    internal int IntegerValue
	    {
	      get
	      {
	        return this.data;
	      }
	      set
	      {
	        this.data = value;
	      }
	    }

	    internal bool this[int bit]
	    {
	      	get
	      	{
	        	return (this.data & bit) == bit;
	      	}
	      	set
	      	{
		        int data = this.data;
		        if (value)
		          this.data = data | bit;
		        else
		          this.data = data & ~bit;
	      	}
	    }

	    internal int this[int mask, int offset]
	    {
	      	get
	      	{
	        	return (this.data & mask) >> offset;
	      	}
	      	set
	      	{
	        	this.data = this.data & ~mask | value << offset;
	      	}
	    }

	    internal void Set(int bit)
	    {
	      	this.data |= bit;
	    }

	    internal void Clear(int bit)
	    {
	      	this.data &= ~bit;
	    }
  	}
	
  	public static class ImageConverter
  	{
  		public static byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
  	}
  	
	public static class Msec
	{
	    internal const int ONE_SECOND = 1000;
	    internal const int ONE_MINUTE = 60000;
	    internal const int ONE_HOUR = 3600000;
	    internal const int ONE_DAY = 86400000;
	    internal const int ONE_WEEK = 604800000;
	    internal const long ONE_YEAR = 31536000000;
	    internal const long ONE_LEAP_YEAR = 31622400000;
	}
	
	public static class Sec
	{
	    internal const int ONE_SECOND = 1;
	    internal const int ONE_MINUTE = 60;
	    internal const int ONE_HOUR = 3600;
	    internal const int ONE_DAY = 86400;
	    internal const int ONE_WEEK = 604800;
	    internal const int ONE_YEAR = 31536000;
	    internal const int ONE_LEAP_YEAR = 31622400;
	}
	
	public static class Bits
	{
	   private static readonly uint MASK_0101010101010101 = 1431655765;
	   private static readonly uint MASK_0011001100110011 = 858993459;
	   private static readonly uint MASK_0000111100001111 = 252645135;
	   private static readonly uint MASK_0000000011111111 = 16711935;
	   private static readonly uint MASK_1111111111111111 = (uint) ushort.MaxValue;
	
	   public static int Count(uint num)
	    {
		      num = (uint) (((int) num & (int) Bits.MASK_0101010101010101) + ((int) (num >> 1) & (int) Bits.MASK_0101010101010101));
		      num = (uint) (((int) num & (int) Bits.MASK_0011001100110011) + ((int) (num >> 2) & (int) Bits.MASK_0011001100110011));
		      num = (uint) (((int) num & (int) Bits.MASK_0000111100001111) + ((int) (num >> 4) & (int) Bits.MASK_0000111100001111));
		      num = (uint) (((int) num & (int) Bits.MASK_0000000011111111) + ((int) (num >> 8) & (int) Bits.MASK_0000000011111111));
		      num = (num & Bits.MASK_1111111111111111) + (num >> 16);
		      return (int) num;
	    }
	
	   public static bool ExactlyOne(uint num)
	    {
	      	if (num != 0U)
	        	return ((int) num & (int) num - 1) == 0;
	      	return false;
	    }
	
	   public static bool MoreThanOne(uint num)
	    {
	      	return (num & num - 1U) > 0U;
	    }
	
	   public static uint ClearLeast(uint num)
	    {
	      	return num & num - 1U;
	    }
	
	   public static int LeastPosition(uint num)
	    {
	      	if (num == 0U)
	        	return 0;
	      	return Bits.Count(num ^ num - 1U);
	    }
	}
	
	public static class Endian
	{
	 	[SecurityCritical]
	    internal static unsafe void DWORDFromLittleEndian(uint* x, int digits, byte* block)
	    {
		      int index1 = 0;
		      int index2 = 0;
		      while (index1 < digits)
		      {
			        x[index1] = (uint) ((int) block[index2] | (int) block[index2 + 1] << 8 | (int) block[index2 + 2] << 16 | (int) block[index2 + 3] << 24);
			        ++index1;
			        index2 += 4;
		      }
	    }

	    internal static void DWORDToLittleEndian(byte[] block, uint[] x, int digits)
	    {
		      int index1 = 0;
		      int index2 = 0;
		      while (index1 < digits)
		      {
		        	block[index2] = (byte) (x[index1] & (uint) byte.MaxValue);
		        	block[index2 + 1] = (byte) (x[index1] >> 8 & (uint) byte.MaxValue);
		        	block[index2 + 2] = (byte) (x[index1] >> 16 & (uint) byte.MaxValue);
		        	block[index2 + 3] = (byte) (x[index1] >> 24 & (uint) byte.MaxValue);
			        ++index1;
			        index2 += 4;
		      }
	    }

    	[SecurityCritical]
    	internal static unsafe void DWORDFromBigEndian(uint* x, int digits, byte* block)
	    {
	      	int index1 = 0;
	      	int index2 = 0;
	      	while (index1 < digits)
	      	{
		        x[index1] = (uint) ((int) block[index2] << 24 | (int) block[index2 + 1] << 16 | (int) block[index2 + 2] << 8) | (uint) block[index2 + 3];
		        ++index1;
		        index2 += 4;
	      	}
	    }

	    internal static void DWORDToBigEndian(byte[] block, uint[] x, int digits)
	    {
	      	int index1 = 0;
	      	int index2 = 0;
	      	while (index1 < digits)
	      	{
		        block[index2] = (byte) (x[index1] >> 24 & (uint) byte.MaxValue);
		        block[index2 + 1] = (byte) (x[index1] >> 16 & (uint) byte.MaxValue);
		        block[index2 + 2] = (byte) (x[index1] >> 8 & (uint) byte.MaxValue);
		        block[index2 + 3] = (byte) (x[index1] & (uint) byte.MaxValue);
		        ++index1;
		        index2 += 4;
	      	}
	    }

	    [SecurityCritical]
	    internal static unsafe void QuadWordFromBigEndian(ulong* x, int digits, byte* block)
	    {
	      	int index1 = 0;
	      	int index2 = 0;
	      	while (index1 < digits)
	      	{
		        x[index1] = (ulong) ((long) block[index2] << 56 | (long) block[index2 + 1] << 48 | (long) block[index2 + 2] << 40 | (long) block[index2 + 3] << 32 | (long) block[index2 + 4] << 24 | (long) block[index2 + 5] << 16 | (long) block[index2 + 6] << 8) | (ulong) block[index2 + 7];
		        ++index1;
		        index2 += 8;
	      	}
	    }

	    internal static void QuadWordToBigEndian(byte[] block, ulong[] x, int digits)
	    {
	      	int index1 = 0;
	      	int index2 = 0;
	      	while (index1 < digits)
	      	{
		        block[index2] = (byte) (x[index1] >> 56 & (ulong) byte.MaxValue);
		        block[index2 + 1] = (byte) (x[index1] >> 48 & (ulong) byte.MaxValue);
		        block[index2 + 2] = (byte) (x[index1] >> 40 & (ulong) byte.MaxValue);
		        block[index2 + 3] = (byte) (x[index1] >> 32 & (ulong) byte.MaxValue);
		        block[index2 + 4] = (byte) (x[index1] >> 24 & (ulong) byte.MaxValue);
		        block[index2 + 5] = (byte) (x[index1] >> 16 & (ulong) byte.MaxValue);
		        block[index2 + 6] = (byte) (x[index1] >> 8 & (ulong) byte.MaxValue);
		        block[index2 + 7] = (byte) (x[index1] & (ulong) byte.MaxValue);
		        ++index1;
		        index2 += 8;
	      	}
	    }

		private static ulong QuadWordFromBigEndian(byte[] block)
	    {
	      return (ulong) ((long) block[0] << 56 | (long) block[1] << 48 | (long) block[2] << 40 | (long) block[3] << 32 | (long) block[4] << 24 | (long) block[5] << 16 | (long) block[6] << 8) | (ulong) block[7];
	    }

	    internal static byte[] Int(uint i)
	    {
	      	return new byte[4]
	      	{
		        (byte) (i >> 24),
		        (byte) (i >> 16),
		        (byte) (i >> 8),
		        (byte) i
	      	};
	    }
	}
	
  	public sealed class BitConverterLE
  {
    private BitConverterLE() { }

    private static void WriteUInt32ToByteArrayBigEndian(uint value, byte[] buffer, int offset)
    {
      buffer[offset + 0] = (byte) (value >> 24);
      buffer[offset + 1] = (byte) (value >> 16);
      buffer[offset + 2] = (byte) (value >> 8);
      buffer[offset + 3] = (byte) value;
    }
    
    private static unsafe byte[] GetUShortBytes(byte* bytes)
    {
      	if (BitConverter.IsLittleEndian)
        	return new byte[2]{ *bytes, bytes[1] };
      	return new byte[2]{ bytes[1], *bytes };
    }

    private static unsafe byte[] GetUIntBytes(byte* bytes)
    {
	      if (BitConverter.IsLittleEndian)
	        return new byte[4]
	        {
	          *bytes,
	          bytes[1],
	          bytes[2],
	          bytes[3]
	        };
	      return new byte[4]
	      {
	        bytes[3],
	        bytes[2],
	        bytes[1],
	        *bytes
	      };
    }

    private static unsafe byte[] GetULongBytes(byte* bytes)
    {
      	if (BitConverter.IsLittleEndian)
        	return new byte[8]
        	{
	          *bytes,
	          bytes[1],
	          bytes[2],
	          bytes[3],
	          bytes[4],
	          bytes[5],
	          bytes[6],
	          bytes[7]
        	};
	      return new byte[8]
	      {
	        bytes[7],
	        bytes[6],
	        bytes[5],
	        bytes[4],
	        bytes[3],
	        bytes[2],
	        bytes[1],
	        *bytes
	      };
    }

    internal static byte[] GetBytes(bool value)
    {
      	return new byte[1]{ !value ? (byte) 0 : (byte) 1 };
    }

    internal static unsafe byte[] GetBytes(char value)
    {
      	return BitConverterLE.GetUShortBytes((byte*) &value);
    }

    internal static unsafe byte[] GetBytes(short value)
    {
      	return BitConverterLE.GetUShortBytes((byte*) &value);
    }

    internal static unsafe byte[] GetBytes(int value)
    {
      	return BitConverterLE.GetUIntBytes((byte*) &value);
    }

    internal static unsafe byte[] GetBytes(long value)
    {
      	return BitConverterLE.GetULongBytes((byte*) &value);
    }

    internal static unsafe byte[] GetBytes(ushort value)
    {
      	return BitConverterLE.GetUShortBytes((byte*) &value);
    }

    internal static unsafe byte[] GetBytes(uint value)
    {
      	return BitConverterLE.GetUIntBytes((byte*) &value);
    }

    internal static unsafe byte[] GetBytes(ulong value)
    {
      	return BitConverterLE.GetULongBytes((byte*) &value);
    }

    internal static unsafe byte[] GetBytes(float value)
    {
      	return BitConverterLE.GetUIntBytes((byte*) &value);
    }

    internal static unsafe byte[] GetBytes(double value)
    {
      	return BitConverterLE.GetULongBytes((byte*) &value);
    }

    private static unsafe void UShortFromBytes(byte* dst, byte[] src, int startIndex)
    {
	      if (BitConverter.IsLittleEndian)
	      {
	        *dst = src[startIndex];
	        dst[1] = src[startIndex + 1];
	      }
	      else
	      {
	        *dst = src[startIndex + 1];
	        dst[1] = src[startIndex];
	      }
    }

    private static unsafe void UIntFromBytes(byte* dst, byte[] src, int startIndex)
    {
	      if (BitConverter.IsLittleEndian)
	      {
	        *dst = src[startIndex];
	        dst[1] = src[startIndex + 1];
	        dst[2] = src[startIndex + 2];
	        dst[3] = src[startIndex + 3];
	      }
	      else
	      {
	        *dst = src[startIndex + 3];
	        dst[1] = src[startIndex + 2];
	        dst[2] = src[startIndex + 1];
	        dst[3] = src[startIndex];
	      }
    }

    private static unsafe void ULongFromBytes(byte* dst, byte[] src, int startIndex)
    {
      	if (BitConverter.IsLittleEndian)
      	{
        	for (int index = 0; index < 8; ++index)
         	 dst[index] = src[startIndex + index];
      	}
      	else
      	{
        	for (int index = 0; index < 8; ++index)
          	dst[index] = src[startIndex + (7 - index)];
      	}
    }

    internal static bool ToBoolean(byte[] value, int startIndex)
    {
      	return value[startIndex] != (byte) 0;
    }

    internal static unsafe char ToChar(byte[] value, int startIndex)
    {
      	char ch;
      	BitConverterLE.UShortFromBytes((byte*) &ch, value, startIndex);
      	return ch;
    }

    internal static unsafe short ToInt16(byte[] value, int startIndex)
    {
      	short num;
     	BitConverterLE.UShortFromBytes((byte*) &num, value, startIndex);
      	return num;
    }

    internal static unsafe int ToInt32(byte[] value, int startIndex)
    {
      	int num;
     	BitConverterLE.UIntFromBytes((byte*) &num, value, startIndex);
      	return num;
    }

    internal static unsafe long ToInt64(byte[] value, int startIndex)
    {
      	long num;
      	BitConverterLE.ULongFromBytes((byte*) &num, value, startIndex);
      	return num;
    }

    internal static unsafe ushort ToUInt16(byte[] value, int startIndex)
    {
      	ushort num;
      	BitConverterLE.UShortFromBytes((byte*) &num, value, startIndex);
      	return num;
    }

    internal static unsafe uint ToUInt32(byte[] value, int startIndex)
    {
      	uint num;
      	BitConverterLE.UIntFromBytes((byte*) &num, value, startIndex);
      	return num;
    }

    internal static unsafe ulong ToUInt64(byte[] value, int startIndex)
    {
      	ulong num;
      	BitConverterLE.ULongFromBytes((byte*) &num, value, startIndex);
      	return num;
    }

    internal static unsafe float ToSingle(byte[] value, int startIndex)
    {
      	float num;
     	BitConverterLE.UIntFromBytes((byte*) &num, value, startIndex);
      	return num;
    }

    internal static unsafe double ToDouble(byte[] value, int startIndex)
    {
      	double num;
      	BitConverterLE.ULongFromBytes((byte*) &num, value, startIndex);
      	return num;
    }
  }
	
	public static class NU
	{
		public static string BinaryToHex(byte[] data)
	    {
	      	if (data == null)
	        	return (string) null;
	      	char[] chArray = new char[checked (data.Length * 2)];
	      	for (int index = 0; index < data.Length; ++index)
	      	{
		        byte num = data[index];
		        chArray[2 * index] = NibbleToHex((byte) ((uint) num >> 4));
		        chArray[2 * index + 1] = NibbleToHex((byte) ((uint) num & 15U));
	      	}
	      	return new string(chArray);
	    }
	
	    public static char NibbleToHex(byte nibble)
	    {
	      	return nibble < (byte) 10 ? (char) ((int) nibble + 48) : (char) ((int) nibble - 10 + 65);
	    }
	    
	    public static byte[] IntPtrToBytes(IntPtr p, long offset, long length)
	    {
	      	byte[] numArray = new byte[16 + IntPtr.Size];
	      	for (int index = 0; index < 8; ++index)
	        	numArray[index] = (byte) ((ulong) (offset >> 8 * index) & (ulong) byte.MaxValue);
	      	for (int index = 0; index < 8; ++index)
	        	numArray[8 + index] = (byte) ((ulong) (length >> 8 * index) & (ulong) byte.MaxValue);
	      	if (IntPtr.Size == 4)
	      	{
	        	int int32 = p.ToInt32();
	        	for (int index = 0; index < 4; ++index)
	          	numArray[16 + index] = (byte) (int32 >> 8 * index & (int) byte.MaxValue);
	      	}
	      	else
	      	{
	        	long int64 = p.ToInt64();
	        	for (int index = 0; index < 8; ++index)
	          	numArray[16 + index] = (byte) ((ulong) (int64 >> 8 * index) & (ulong) byte.MaxValue);
	      	}
	      	return numArray;
	    }
	    
	    internal static int ConvertByteArrayToInt(byte[] input)
	    {
	      int num = 0;
	      for (int index = 0; index < input.Length; ++index)
	        num = num * 256 + (int) input[index];
	      return num;
	    }
	    
	    internal static byte[] ConvertIntToByteArray(int dwInput)
	    {
	      byte[] numArray1 = new byte[8];
	      int length = 0;
	      if (dwInput == 0)
	        return new byte[1];
	      int num1 = dwInput;
	      while (num1 > 0)
	      {
	        int num2 = num1 % 256;
	        numArray1[length] = (byte) num2;
	        num1 = (num1 - num2) / 256;
	        ++length;
	      }
	      byte[] numArray2 = new byte[length];
	      for (int index = 0; index < length; ++index)
	        numArray2[index] = numArray1[length - index - 1];
	      return numArray2;
	    }
	
		internal static string ToBinary(Int64 Decimal)
		{
		  	Int64 BinaryHolder;
		  	char[] BinaryArray;
		  	string BinaryResult = "";
		  	while (Decimal > 0)
		  	{
			    BinaryHolder = Decimal % 2;
			    BinaryResult += BinaryHolder;
			    Decimal = Decimal / 2;
		  	}
		  	BinaryArray = BinaryResult.ToCharArray();
		  	Array.Reverse(BinaryArray);
		  	BinaryResult = new string(BinaryArray);
		  	return BinaryResult;
		}
	}
	
	internal static class ParserCss
  	{
	    public const string CssProperties = ";?[^;\\s]*:[^\\{\\}:;]*(\\}|;)?";
	    public const string CssComments = "/\\*[^*/]*\\*/";
	    public const string CssAtRules = "@.*\\{\\s*(\\s*[^\\{\\}]*\\{[^\\{\\}]*\\}\\s*)*\\s*\\}";
	    public const string CssMediaTypes = "@media[^\\{\\}]*\\{";
	    public const string CssBlocks = "[^\\{\\}]*\\{[^\\{\\}]*\\}";
	    public const string CssNumber = "{[0-9]+|[0-9]*\\.[0-9]+}";
	    public const string CssPercentage = "([0-9]+|[0-9]*\\.[0-9]+)\\%";
	    public const string CssLength = "([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)";
	    public const string CssColors = "(#\\S{6}|#\\S{3}|rgb\\(\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)";
	    public const string CssLineHeight = "(normal|{[0-9]+|[0-9]*\\.[0-9]+}|([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%)";
	    public const string CssBorderStyle = "(none|hidden|dotted|dashed|solid|double|groove|ridge|inset|outset)";
	    public const string CssBorderWidth = "(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|thin|medium|thick)";
	    public const string CssFontFamily = "(\"[^\"]*\"|'[^']*'|\\S+\\s*)(\\s*\\,\\s*(\"[^\"]*\"|'[^']*'|\\S+))*";
	    public const string CssFontStyle = "(normal|italic|oblique)";
	    public const string CssFontVariant = "(normal|small-caps)";
	    public const string CssFontWeight = "(normal|bold|bolder|lighter|100|200|300|400|500|600|700|800|900)";
	    public const string CssFontSize = "(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)";
	    public const string CssFontSizeAndLineHeight = "(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)(\\/(normal|{[0-9]+|[0-9]*\\.[0-9]+}|([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%))?(\\s|$)";
	    public const string HtmlTag = "<[^<>]*>";
	    public const string HmlTagAttributes = "[^\\s]*\\s*=\\s*(\"[^\"]*\"|[^\\s]*)";

	    public static MatchCollection Match(string regex, string source)
	    {
	      	return new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(source);
	    }

	    public static string Search(string regex, string source)
	    {
	      	int position;
	      	return ParserCss.Search(regex, source, out position);
	    }

	    public static string Search(string regex, string source, out int position)
	    {
	      	MatchCollection matchCollection = ParserCss.Match(regex, source);
	      	if (matchCollection.Count > 0)
	      	{
		        position = matchCollection[0].Index;
		        return matchCollection[0].Value;
	      	}
	      	position = -1;
	      	return (string) null;
	    }
  }
	
	class Program
	{
		public static void Main(string[] args)
		{
			// TEST: ImageConverter
//			byte[] bytes = ImageConverter.ImageToByteArray(Image.FromFile(@"C:\Users\fdshfgas\Pictures\2020\2.jpg"));
//           
//            Image image = ImageConverter.ByteArrayToImage(bytes);
//            image.Save(@"C:\Users\fdshfgas\Desktop\121212.jpg");
			
//			Console.WriteLine(NU.ToBinary(2)); // 10
			
			Console.ReadKey(true);
		}
	}
}
