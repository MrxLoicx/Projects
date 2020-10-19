using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PhotoEditor;

namespace PhotoEditor
{
	public static class HslRgbUtils
    {
        public static Color HslToRgb(double h, double sl, double l)
        {

            double v;
            double r, g, b;
            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            return Color.FromArgb(Convert.ToByte(r * 255.0f),Convert.ToByte(g * 255.0f),Convert.ToByte(b * 255.0f));

        }

        public static void RgbToHsl(Color rgb, out double h, out double s, out double l)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            h = 0; // default to black
            s = 0;
            l = 0;
            v = Math.Max(r, g);
            v = Math.Max(v, b);
            m = Math.Min(r, g);
            m = Math.Min(m, b);

            l = (m + v) / 2.0;
            if (l <= 0.0)
            {
                return;
            }
            vm = v - m;
            s = vm;
            if (s > 0.0)
            {
                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
            }
            else
            {
                return;
            }
            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;
            if (r == v)
            {
                h = (g == m ? 5.0 + b2 : 1.0 - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0 + r2 : 3.0 - b2);
            }
            else
            {
                h = (r == m ? 3.0 + g2 : 5.0 - r2);
            }
            h /= 6.0;
        }
    }
	
	public class LAB
	{
		public double L;
        public double A;
        public double B; 
	}
	
	public class XYZ
	{
		public double X;
        public double Y;
        public double Z;	
	}
	
	public class HSLColor
	{
	    public float Hue;
	    public float Saturation;
	    public float Luminosity;
	
	    public HSLColor(float H, float S, float L)
	    {
	        Hue = H;
	        Saturation = S;
	        Luminosity = L;
	    }
	}
	
	public class CmykColor
	{
		public float C { get; set; }
		public float M { get; set; }
		public float Y { get; set; }
		public float K { get; set; }
		
		public CmykColor(float c, float m, float y, float k) {
			this.C = c;
			this.M = m;
			this.Y = y;
			this.K = k;
		}
	}
	
	public static class ConverterColorsApi
	{
		// Например new byte[]{70,70,48,48,70,70}
		public static string BytesToHex(byte[] bytes) {
			char[] chars = bytes.Select(x => (char)x).ToArray();
			string hex = new string(chars);
			return hex;
		}
		
		public static int[] BytesToRGB(byte[] bytes) {
			var str1 = new string(new char[] { (char)bytes[0], (char)bytes[1] });
			var str2 = new string(new char[] { (char)bytes[2], (char)bytes[3] });
			var str3 = new string(new char[] { (char)bytes[4], (char)bytes[5] });
			 
			int r = Convert.ToInt32(str1, 16);
			int g = Convert.ToInt32(str2, 16);
			int b = Convert.ToInt32(str3, 16);
			
			return new int[] { r, g, b };
		}
		
		public static Color CmykToRgb(float c, float m, float y, float k)
		{
		  int r;
		  int g;
		  int b;
		
		  r = Convert.ToInt32(255 * (1 - c) * (1 - k));
		  g = Convert.ToInt32(255 * (1 - m) * (1 - k));
		  b = Convert.ToInt32(255 * (1 - y) * (1 - k));
		
		  return Color.FromArgb(r, g, b);
		}
		
		public static CmykColor RgbToCmyk(int r, int g, int b)
		{
		  float c;
		  float m;
		  float y;
		  float k;
		  float rf;
		  float gf;
		  float bf;
		
		  rf = r / 255F;
		  gf = g / 255F;
		  bf = b / 255F;
		
		  k = ClampCmyk(1 - Math.Max(Math.Max(rf, gf), bf));
		  c = ClampCmyk((1 - rf - k) / (1 - k));
		  m = ClampCmyk((1 - gf - k) / (1 - k));
		  y = ClampCmyk((1 - bf - k) / (1 - k));
		
		  return new CmykColor(c, m, y, k);
		}
		
		private static float ClampCmyk(float value)
		{
		  if (value < 0 || float.IsNaN(value))
		  {
		    value = 0;
		  }
		
		  return value;
		}
	
		public static Color HslToRgb(double h, double s, double l) {
			return HslRgbUtils.HslToRgb(h,s,l);
		}
		
		public static HSLColor RgbToHsl(Color color) {
			double h = 0;
			double s = 0;
			double l = 0;
			
			HslRgbUtils.RgbToHsl
				(color, out h, out s, out l);
			
			return new HSLColor((float)h,(float)s,(float)l);
		}
	
		public static Color XyzToRgb(XYZ xyz) {
			xyz.X = xyz.X / 100;
            xyz.Y = xyz.Y / 100;
            xyz.Z = xyz.Z / 100;
 
            double rFloat = xyz.X * 3.2406 + xyz.Y * -1.5372 + xyz.Z * -0.4986;
            double gFloat = xyz.X * -0.9689 + xyz.Y * 1.8758 + xyz.Z * 0.0415;
            double bFloat = xyz.X * 0.0557 + xyz.Y * 0.2040 + xyz.Z * 1.0570;

 
            rFloat = rFloat > 0.0031308 ? 1.055 * Math.Pow(rFloat, 0.41666) - 0.055 : 12.92 * rFloat;
            gFloat = gFloat > 0.0031308 ? 1.055 * Math.Pow(gFloat, 0.41666) - 0.055 : 12.92 * gFloat;
            bFloat = bFloat > 0.0031308 ? 1.055 * Math.Pow(bFloat, 0.41666) - 0.055 : 12.92 * bFloat;
 
            return Color.FromArgb((int)(rFloat * 255), (int)(gFloat * 255), (int)(bFloat * 255));
		}
		
		public static XYZ RgbToXyz(Color c) {
			XYZ xyz = new XYZ();
 
            double rFloat = c.R / 255;      // Нормализация цветов RGB
            double gFloat = c.G / 255;
            double bFloat = c.B / 255;
 
            /* Преобразование значений RGB в пространство цветов sRGB */
            rFloat = rFloat > 0.04045 ? Math.Pow((rFloat + 0.055) / 1.055, 2.2) : rFloat / 12.92;
            gFloat = gFloat > 0.04045 ? Math.Pow((gFloat + 0.055) / 1.055, 2.2) : gFloat / 12.92;
            bFloat = bFloat > 0.04045 ? Math.Pow((bFloat + 0.055) / 1.055, 2.2) : bFloat / 12.92;
 
            /* Вычисление XYZ с использовением коррекции D65 */
            xyz.X = rFloat * 0.4124 + gFloat * 0.3576 + bFloat * 0.1805;
            xyz.Y = rFloat * 0.2126 + gFloat * 0.7152 + bFloat * 0.0722;
            xyz.Z = rFloat * 0.0193 + gFloat * 0.1192 + bFloat * 0.9505;
 
            return xyz;
		}
		
		public static XYZ LabToXyz(LAB lab) {
			XYZ xyz = new XYZ();
 
            xyz.Y = (lab.L + 16.0) / 116.0;
            xyz.X = lab.A / 500.0 + xyz.Y;
            xyz.Z = xyz.Y - lab.B / 200.0;
 
            xyz.X = oXYZ(xyz.X);
            xyz.Y = oXYZ(xyz.Y);
            xyz.Z = oXYZ(xyz.Z);
 
            xyz.X = 95.047 * xyz.X;
            xyz.Y = 100 * xyz.Y;
            xyz.Z = 108.883 * xyz.Z;
 
            return xyz;
		}
		
		public static LAB XyzToLab(XYZ xyz) {
			LAB lab = new LAB();
 
            double tmp = fXYZ(xyz.Y);                             // Чтобы три раза не считать, а только один
 
            lab.L = 116 * tmp - 16;
            lab.A = 500 * (fXYZ(xyz.X / 0.9505) - tmp);
            lab.B = 200 * (tmp - fXYZ(xyz.Z / 1.089));
 
            return lab;
		}
		
		private static double fXYZ(double tmp)
        {
            return tmp > 0.008856 ? Math.Pow(tmp, 1.0 / 3.0) : (7.787 * tmp + 16 / 116);
        }
		
		private static double oXYZ(double tmp)
        {
            return Math.Pow(tmp, 3) > 0.008856 ? Math.Pow(tmp, 3) : (tmp - 16.0 / 116.0) / 7.787;
        }
		
		public static LAB RgbToLab(Color c) {
			return XyzToLab(RgbToXyz(c));
		}
		
		public static Color LabToRgb(LAB lab) {
			return XyzToRgb(LabToXyz(lab));
		}
	}
	
	/// Фильтры для графики
	public class Filters
	{
		public static Bitmap BlachAndWhite(Bitmap bmpImg, int P)
		{
		  Bitmap result = new Bitmap(bmpImg.Width, bmpImg.Height);
		  Color color = new Color();
		  try
		  {
		    for (int j = 0; j < bmpImg.Height; j++)
		    {
		      for (int i = 0; i < bmpImg.Width; i++)
		      {
		         color = bmpImg.GetPixel(i, j);
		         int K = (color.R + color.G + color.B) / 3;
		         result.SetPixel(i, j, K <= P ? Color.Black : Color.White);
		      }
		    }
		  }
		  catch (Exception ex)
		  {
		     MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		  }
		  return result;
		}
	}

	public struct RGB_Color { 
		public RGB_Color(int Red, int Green, int Blue) { 
			R = Red; 
			G = Green; 
			B = Blue; 
		} 
	public int R, G, B; 
	}

	public class Filter { 
		private Bitmap UPicture = null; 
		private BitmapData BmpData = null; 
		private unsafe byte* Begin = (byte*)IntPtr.Zero; 
		private int BytesPerPix = 0; 
		public Filter(Bitmap MainBitmap) { 
			if (MainBitmap != null) { 
				UPicture = (Bitmap)MainBitmap.Clone(); 
				switch (UPicture.PixelFormat) { 
					case PixelFormat.Format24bppRgb: { 
						BytesPerPix = 3; break; 
					} 
					case PixelFormat.Format32bppArgb: { 
						BytesPerPix = 4; break; 
						} 
					default: { 
							throw new NotSupportedException("Формат пикселей не соответствует стандарту"); 
						} 
				} 
				BmpData = UPicture.LockBits(new Rectangle(0, 0, UPicture.Width, UPicture.Height), 
				                            ImageLockMode.ReadWrite, UPicture.PixelFormat); 
				
				unsafe { 
					Begin = (byte*)BmpData.Scan0; 
				} 
			} else 
				throw new ArgumentException("Неверный параметр #1"); 
		} 
		
		public Bitmap Picture { 
			get { return UPicture; } 
		} 
		
		public int Height { 
			get { return UPicture.Height; } 
		} 
		
		public int Width { 
			get { return UPicture.Width; } 
		} 
		
		public int BytesPerPixel { get { return BytesPerPix; } } 
		public IntPtr Safe_IMG_Scan0 { get { return BmpData.Scan0; } } 
		public unsafe byte* Unsafe_IMG_Scan0 { get { return Begin; } } 
		public int AllPixelsBytes { get { return UPicture.Width * UPicture.Height * BytesPerPix; } } 
		public void UnLock() { UPicture.UnlockBits(BmpData); }
		
		public unsafe RGB_Color GetPixel(int X, int Y) { 
			RGB_Color Pixel = new RGB_Color(); 
			int IDX = (Y * UPicture.Width + X) * BytesPerPix; //Вычисляем позицию пикселя
			Pixel.B = *(Begin + (IDX + 0)); 
			//B 
			Pixel.G = *(Begin + (IDX + 1));
			//G 
			Pixel.R = *(Begin + (IDX + 2));
			//R 
			return Pixel;
		} 
		
		public unsafe void SetPixel(RGB_Color CL,int X,int Y) { 
			int IDX = (Y * UPicture.Width + X) * BytesPerPix; 
			//Вычисляем позицию пикселя 
			*(Begin + (IDX + 0)) = Convert.ToByte(CL.B); //B 
			*(Begin + (IDX + 1)) = Convert.ToByte(CL.G);//G 
			*(Begin + (IDX + 2)) = Convert.ToByte(CL.R); //R
		} 
	}


	// Использование фильтра Негатив:
	// Bitmap Test1 = Negative.ProcessImage(new Filter(TestBitmap)) //TestBitmap - это ваша картинка
	public class Negative { 
		public static unsafe Bitmap ProcessImage(Filter Main) { 
			for (int I = 0; I < Main.AllPixelsBytes; I += Main.BytesPerPixel) { 
				*(Main.Unsafe_IMG_Scan0 + (I + 0)) = (byte)(255 - (*(Main.Unsafe_IMG_Scan0 + I + 0))); //B 
				*(Main.Unsafe_IMG_Scan0 + (I + 1)) = (byte)(255 - (*(Main.Unsafe_IMG_Scan0 + I + 1))); //G 
				*(Main.Unsafe_IMG_Scan0 + (I + 2)) = (byte)(255 - (*(Main.Unsafe_IMG_Scan0 + I + 2))); //R 
			} 
			Main.UnLock(); 
			return Main.Picture; }
	}
	
	// Использование:
	// Bitmap Test2 = Sepia.ProcessImage(new Filter(TestBitmap)) //TestBitmap - это ваша картинка
	public class Sepia
	{
		public static unsafe Bitmap ProcessImage(Filter Main) { 
			RGB_Color TMP = new RGB_Color(); 
			byte Tone = 0; 
			for (int I = 0; I < Main.AllPixelsBytes; I += Main.BytesPerPixel) { 
				TMP.B = *(Main.Unsafe_IMG_Scan0 + (I + 0)); //B 
				TMP.G = *(Main.Unsafe_IMG_Scan0 + (I + 1)); //G 
				TMP.R = *(Main.Unsafe_IMG_Scan0 + (I + 2)); //R 
				Tone = (byte)(0.299 * TMP.R + 0.587 * TMP.G + 0.114 * TMP.B); //Вычисляем новый цвет 
				//Нормализуем цвет 
				*(Main.Unsafe_IMG_Scan0 + (I + 2)) = (byte)((Tone > 206) ? 255 : Tone + 49); //R 
				*(Main.Unsafe_IMG_Scan0 + (I + 1)) = (byte)((Tone < 14) ? 0 : Tone - 14); //G 
				*(Main.Unsafe_IMG_Scan0 + (I + 0)) = (byte)((Tone < 56) ? 0 : Tone - 56); //B 
			} 
			Main.UnLock(); return Main.Picture; }

	}
	
	// Формула по которой будем менять яркость:
	//Pixel[X, Y] = Pixel[X, Y] + Brightness; 
	//где Pixel[X, Y] – это цвет пикселя с координатами X, Y, а Brightness – это значение яркости.
	
	// Использование фильтра Яркость:
	// Bitmap Test1 = Brightness.ProcessImage(new Filter(TestImage),85); //TestImage - это ваше изображение
	public class Brightness
	{
		private static byte ToByte(int Val) { 
			if (Val > 255) Val = 255; 
			else if (Val < 0) Val = 0; 
			return (byte)Val; 
		}
		
		public static unsafe Bitmap ProcessImage(Filter Main,int Value) { 
			for (int I = 0; I < Main.AllPixelsBytes; I += Main.BytesPerPixel) { 
				*(Main.Unsafe_IMG_Scan0 + I + 0) = ToByte(*(Main.Unsafe_IMG_Scan0 + I + 0) + Value);//B 
				*(Main.Unsafe_IMG_Scan0 + I + 1) = ToByte(*(Main.Unsafe_IMG_Scan0 + I + 1) + Value);//G 
				*(Main.Unsafe_IMG_Scan0 + I + 2) = ToByte(*(Main.Unsafe_IMG_Scan0 + I + 2) + Value);//R 
			} 
			Main.UnLock(); 
			return Main.Picture; 
		}

	}
	
	// Изменение контрастности изображения:
	/* Формула вычисления
	  Contrast = (100.0 + Value) / 255.0; 
	//Вычисляем общее значение 
	Contrast = Contrast * Contrast; 
	//Вычисляем контрастность для заданного пикселя 
	Pixel= RGB_Channels / 255.0; 
	Pixel = Pixel - 0.5; 
	Pixel = Pixel * Contrast; 
	Pixel = Pixel + 0.5; 
	Pixel = Pixel * 255; 
	if(Pixel > 255) Pixel = 255; 
	else if(Pixel < 0) Pixel = 0;
	
	где RGB_Channels – это каналы RGB (красный, зелёный, синий).
	 */
	
	// Использование:
	// Bitmap Test1 = Contrast.ProcessImage(new Filter(TestImage),35); //TestImage - это ваше изображение
	public class Contrast
	{
		public static unsafe Bitmap ProcessImage(Filter Main,int Value) { 
			int RedVal, GreenVal, BlueVal; 
			double Pixel; 
			double Contrast = (100.0 + Value) / 100.0; 
			//Вычисляем общее значение контраста 
			Contrast = Contrast * Contrast; 
			for (int I = 0; I < Main.AllPixelsBytes; I += Main.BytesPerPixel) { 
				BlueVal = *(Main.Unsafe_IMG_Scan0 + (I + 0)); //B 
				GreenVal = *(Main.Unsafe_IMG_Scan0 + (I + 1)); //G 
				RedVal = *(Main.Unsafe_IMG_Scan0 + (I + 2)); //R 
				Pixel = RedVal / 255.0; 
				Pixel = Pixel - 0.5; 
				Pixel = Pixel * Contrast; 
				Pixel = Pixel + 0.5; 
				Pixel = Pixel * 255; 
				if (Pixel < 0) Pixel = 0; 
				if (Pixel > 255) Pixel = 255; 
				*(Main.Unsafe_IMG_Scan0 + (I + 2)) = Convert.ToByte(Pixel); 
				Pixel = GreenVal / 255.0; 
				Pixel = Pixel - 0.5; 
				Pixel = Pixel * Contrast; 
				Pixel = Pixel + 0.5; 
				Pixel = Pixel * 255; 
				if (Pixel < 0) Pixel = 0; 
				if (Pixel > 255) Pixel = 255; 
				*(Main.Unsafe_IMG_Scan0 + (I + 1)) = Convert.ToByte(Pixel); 
				Pixel = BlueVal / 255.0; 
				Pixel = Pixel - 0.5; 
				Pixel = Pixel * Contrast; 
				Pixel = Pixel + 0.5; 
				Pixel = Pixel * 255; 
				if (Pixel < 0) Pixel = 0; 
				if (Pixel > 255) Pixel = 255; 
				*(Main.Unsafe_IMG_Scan0 + (I + 0)) = Convert.ToByte(Pixel); 
			} 
			Main.UnLock(); return Main.Picture; }

	}
		
	// Изменение тона изображения
	
	public struct HSL_Color { 
		public HSL_Color(double Hue, double Saturation, double Lightness) { 
			H = Hue; 
			S = Saturation; 
			L = Lightness; 
		} 
		public double H, S, L; 
	}
	
	public class ConverterHsl {
		
	public static HSL_Color RGB_TO_HSL(RGB_Color CL) { 
			double H = 0, S = 0, L = 0; 
			double R = (double)CL.R / 255.0; // 
			double G = (double)CL.G / 255.0; // Приводим к диапазону от 0 до 1 
			double B = (double)CL.B / 255.0; // 
			double Max = Math.Max(R, Math.Max(G, B)); 
			double Min = Math.Min(R, Math.Min(G, B)); //Вычисляем тон 
			if (Max == Min) { H = 0; } 
			else if (Max == R && G >= B) { 
				H = 60.0 * (G - B) / (Max - Min); 
			} else if (Max == R && G < B) { 
				H = 60.0 * (G - B) / (Max - Min) + 360.0; 
			} else if (Max == G) { 
				H = 60.0 * (B - R) / (Max - Min) + 120.0; 
			} else if (Max == B) { 
				H = 60.0 * (R - G) / (Max - Min) + 240.0; 
			} //Вычисляем светлоту 
			L = (Max + Min) / 2.0; 
			//Вычисляем насыщенность 
			if (L == 0 || Max == Min) { S = 0; } 
			else if (0 < L && L <= 0.5) { 
				S = (Max - Min) / (Max + Min); 
			} else if (L > 0.5) { 
				S = (Max - Min) / (2 - (Max + Min)); 
			} 
			return new HSL_Color(H, S, L); 
		}

	public static RGB_Color HSL_TO_RGB(HSL_Color CL) { 
		int R, G, B; 
		if (CL.S == 0) { 
			R = (int)Math.Round(CL.L * 255.0); // 
			G = (int)Math.Round(CL.L * 255.0); //Округляем значения 
			B = (int)Math.Round(CL.L * 255.0); // 
		} else { 
			double Q = (CL.L < 0.5) ? (CL.L * (1.0 + CL.S)) : (CL.L + CL.S - (CL.L * CL.S));
			double P = (2.0 * CL.L) - Q;
			double HK = CL.H / 360.0;
			double[] T = new double[3]; //Массив для хранения значений R,G,B 
			T[0] = HK + (1.0 / 3.0); // R 
			T[1] = HK; // G 
			T[2] = HK - (1.0 / 3.0); // B 
			for (int i = 0; i < 3; i++) { 
				if (T[i] < 0) T[i] += 1.0; 
				if (T[i] > 1) T[i] -= 1.0; 
				if ((T[i] * 6) < 1) { 
					T[i] = P + ((Q - P) * 6.0 * T[i]); 
				} else if ((T[i] * 2.0) < 1) { 
					T[i] = Q; 
				} else if ((T[i] * 3.0) < 2) { 
					T[i] = P + (Q - P) * ((2.0 / 3.0) - T[i]) * 6.0; 
				} else { 
					T[i] = P; 
				} 
			} 
			R = (int)(T[0] * 255.0); // 
			G = (int)(T[1] * 255.0); //Приводим к диапазону от 0 до 255 
			B = (int)(T[2] * 255.0); // 
		} 
		return new RGB_Color(R, G, B); 
	}
	}
	

	// Корректировка фона
	// Использование:
	// Bitmap TestBitmap = Hue.ProcessImage(new Filter(MyImage), 280); //MyImage - это выше изображение
	public class Hue { 
		public unsafe static Bitmap ProcessImage(Filter Main,int Value) { 
			RGB_Color CL_RGB = new RGB_Color(); 
			//Создаем структуры 
			HSL_Color CL_HSL = new HSL_Color(); 
			for (int I = 0; I < Main.AllPixelsBytes; I += Main.BytesPerPixel) 
				//Проходимся по каждому пикселю 
			{ 
				CL_RGB.B = *(Main.Unsafe_IMG_Scan0 + (I + 0)); 
				//Получаем значение синего 
				CL_RGB.G = *(Main.Unsafe_IMG_Scan0 + (I + 1)); 
				//Получаем значение зелёного 
				CL_RGB.R = *(Main.Unsafe_IMG_Scan0 + (I + 2)); 
				//Получаем значение красного 
				CL_HSL = ConverterHsl.RGB_TO_HSL(CL_RGB); //RGB -> HSL 
				CL_HSL.H = (double)Value; //Изменяем тон 
				CL_RGB = ConverterHsl.HSL_TO_RGB(CL_HSL); //HSL -> RGB 
				*(Main.Unsafe_IMG_Scan0 + (I + 0)) = (byte)CL_RGB.B; 
				*(Main.Unsafe_IMG_Scan0 + (I + 1)) = (byte)CL_RGB.G; 
				*(Main.Unsafe_IMG_Scan0 + (I + 2)) = (byte)CL_RGB.R; 
			} 
			Main.UnLock();//Разблокируем биты изображения 
			return Main.Picture;
		} 
	}
	
	// Изменение гаммы изображения
	/*
		Для коррекции гаммы сначала вычисляется таблица значений (RampTable). 
		Таблица состоит из 255 ячеек. 
		Далее в зависимости от значений R, G, Bберутся данные из таблицы и присваиваются пикселю. 
		Значения гаммы находятся в промежутке от 0.0 до 5.0.
	*/
	// Использование:
	// Bitmap TestBitmap = Gamma.ProcessImage(new Filter(MyImage), 3.4f); //MyImage - это выше изображение
	public class Gamma {
		private static byte[] RampTable = new byte[256];
		
		// Генерация таблицы
		private static void GenerateRampTable(float Value)
			//Здесь Value - это значение гаммы 
		{ 
			double Gam = Math.Max(0.1, Math.Min(5.0, Value));
			//Вычислям общий коэффицент гаммы,который потребуется для вычисления главного значения 
			double G = 1 / Gam; //Главное значение гаммы 
			for (int I = 0; I < 256; I++) { 
				RampTable[I] = (byte)Math.Min(255, (int)(Math.Pow(I / 255.0, G) * 255 + 0.5));
				//Вычисляем табличные данные 
			} 
		}
		
		public static unsafe Bitmap ProcessImage(Filter Main,float Value) { 
			GenerateRampTable(Value); //Генерируем гамма-таблицу 
			for (int I = 0; I < Main.AllPixelsBytes; I += Main.BytesPerPixel) 
				//Проходим по каждому пикселю изображения 
			{ 
				*(Main.Unsafe_IMG_Scan0 + (I + 0)) = RampTable[*(Main.Unsafe_IMG_Scan0 + (I + 0))];
				//В зависимости от значения синего,ему присваивается значение из таблицы 
				*(Main.Unsafe_IMG_Scan0 + (I + 1)) = RampTable[*(Main.Unsafe_IMG_Scan0 + (I + 1))];
				//В зависимости от значения зелёного,ему присваивается значение из таблицы 
				*(Main.Unsafe_IMG_Scan0 + (I + 2)) = RampTable[*(Main.Unsafe_IMG_Scan0 + (I + 2))];
				//В зависимости от значения красного,ему присваивается значение из таблицы 
			} 
			Main.UnLock(); //Разблокируем биты изображения 
			return Main.Picture; 
		}

	}
	
	public class ImageDecorator
	{
		// Добавить текст на картинку
		public static void AddTextInImage(string path, string new_path, string text) {
			Image img = Bitmap.FromFile(path); //путь к картинке
 
			Graphics g = Graphics.FromImage(img);
			
			g.DrawString(text,new Font("Verdana", (float)20),
				new SolidBrush(Color.White),15,img.Height / 2); //месторасположения текста
 
			img.Save(new_path, ImageFormat.Jpeg); //путь и имя сохранения файла
 
			g = null;
			img = null;
		}
	}
		
		
	
	
	public class DarkMenuStripRenderer: ToolStripProfessionalRenderer
	{
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
			base.OnRenderMenuItemBackground(e);
	      	if (!e.Item.Enabled)
	        	return;
			var rc = new Rectangle(Point.Empty, e.Item.Size);
			Color c = (e.Item.Selected) ? Color.FromArgb(14,14,14) : Color.FromArgb(46,48,49);
			Brush brush = new SolidBrush(c);
			e.Graphics.FillRectangle(brush, rc);
		}
		
	    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
	      base.OnRenderSeparator(e);
	      SolidBrush solidBrush = new SolidBrush(Color.FromArgb(30, 30, 30));
	      Rectangle rect = new Rectangle(30, 3, e.Item.Width - 32, 1);
	      e.Graphics.FillRectangle((Brush) solidBrush, rect);
	    }
	    
	    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) {
	      base.OnRenderItemCheck(e);
	      if (e.Item.Selected) {
	        Rectangle rect1 = new Rectangle(4, 2, 18, 18);
	        Rectangle rect2 = new Rectangle(5, 3, 16, 16);
	        SolidBrush solidBrush1 = new SolidBrush(Color.Black);
	        SolidBrush solidBrush2 = new SolidBrush(Color.FromArgb(40,40,40));
	        e.Graphics.FillRectangle((Brush) solidBrush1, rect1);
	        e.Graphics.FillRectangle((Brush) solidBrush2, rect2);
	        e.Graphics.DrawImage(e.Image, new Point(5, 3));
	      } else {
	        Rectangle rect1 = new Rectangle(4, 2, 18, 18);
	        Rectangle rect2 = new Rectangle(5, 3, 16, 16);
	        SolidBrush solidBrush1 = new SolidBrush(Color.FromArgb(14,14,14));
	        SolidBrush solidBrush2 = new SolidBrush(Color.FromArgb((int) byte.MaxValue, 80, 90, 90));
	        e.Graphics.FillRectangle((Brush) solidBrush1, rect1);
	        e.Graphics.FillRectangle((Brush) solidBrush2, rect2);
	        e.Graphics.DrawImage(e.Image, new Point(5, 3));
	      }
	    }
	}
		
	public static class Declarator
	{
		public static string ImagesPath = @"C:\Users\fdshfgas\Documents\_USB_\Разработки 2020\App 2020\PhotoEditor\Images\";
	}
	
	public class PhotoEditor : Form
	{
		public Panel pHeader = new Panel();
		public Panel pBody = new Panel();
		public MenuStrip menuStrip1 = new MenuStrip();
		public ToolStripMenuItem tFile = new ToolStripMenuItem();
		public ToolStripMenuItem tAddFolder = new ToolStripMenuItem();
		public ToolStripMenuItem tDeleteAllFolders = new ToolStripMenuItem();
		public ToolStripMenuItem tExit = new ToolStripMenuItem();
		
		public ToolStripMenuItem tEdit= new ToolStripMenuItem();
		public ToolStripMenuItem tBrowse = new ToolStripMenuItem();
		public ToolStripMenuItem tAudioPlayer = new ToolStripMenuItem();
		public ToolStripMenuItem tFilter = new ToolStripMenuItem();
		
		public ToolStripMenuItem tTools = new ToolStripMenuItem();
		public ToolStripMenuItem tConverterColors = new ToolStripMenuItem();
		
		public Label lTitle = new Label();
		public SplitContainer splitContainerMain = new SplitContainer();
		public ToolStripMenuItem tAbout = new ToolStripMenuItem();
		
		public ToolStripMenuItem tWindow = new ToolStripMenuItem();
		public ToolStripMenuItem tReload = new ToolStripMenuItem();
		
		public PictureBox pClose = new PictureBox();
		public PictureBox pExpand = new PictureBox();
		public PictureBox pCollapse = new PictureBox();
		public Panel panelLeft = new Panel();
		public Panel panelRight = new Panel();
		
		public TreeView treeViewFolders = new TreeView();
		// Включен ли режим просмотра изображений
		public bool isBrowseable = true;
		
		//-------- Form Filter ----------//
		Form FilterForm = new Form();
		Panel pHeaderFilter = new Panel();
		Label lTitleFilter = new Label();
		PictureBox pCloseFilter = new PictureBox();
		Label lSource = new Label();
		Label lResult = new Label();
		PictureBox pictResult = new PictureBox();
		PictureBox pictSource = new PictureBox();
		Button bSaveResult = new Button();
		Button bApply = new Button();
		//-----------------------------//
		
		public PhotoEditor() {
			Initialize();
		}
		
		public void ExpandForm_Click(object sender, EventArgs args) {
			if (WindowState == FormWindowState.Normal) {
				WindowState = FormWindowState.Maximized;
			} else {
				WindowState = FormWindowState.Normal;
			}
		}
		
		public void CollapseForm_Click(object sender, EventArgs args) {
			if (WindowState == FormWindowState.Normal || WindowState == FormWindowState.Maximized) {
				WindowState = FormWindowState.Minimized;
			}
		}
		
		int OffsetX, OffsetY;
		bool isMouseDown = false;
	    
	    private void MouseDown_Click(object sender, MouseEventArgs e)
	    {
	      if (e.Button == MouseButtons.Left)
	      {
	        this.isMouseDown = true;
	        Point screen = this.PointToScreen(new Point(e.X, e.Y));
	        this.OffsetX = this.Location.X - screen.X;
	        this.OffsetY = this.Location.Y - screen.Y;
	      }
	      else
	        this.isMouseDown = false;
	      if (e.Clicks != 2)
	        return;
	      this.isMouseDown = false;
	    }
	
	    private void MouseMove_Click(object sender, MouseEventArgs e)
	    {
	      if (!this.isMouseDown)
	        return;
	      if (this.WindowState == FormWindowState.Maximized)
	        this.WindowState = FormWindowState.Normal;
	      Point screen = this.pHeader.PointToScreen(new Point(e.X, e.Y));
	      screen.Offset(new Point(OffsetX, OffsetY));
	      this.Location = screen;
	    }
	
	    private void MouseUp_Click(object sender, MouseEventArgs e)
	    {
	      this.isMouseDown = false;
	    }

        public void AddFolder_Click(object sender, EventArgs e) {
        	FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        	if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
        		string folderPath = folderBrowserDialog.SelectedPath;
        		TreeNode node = treeViewFolders.Nodes.Add(folderPath);
        		
        		IEnumerable<string> enumerable = Directory.EnumerateFileSystemEntries(folderPath);
        		
        		foreach (string path in enumerable) {
        			node.Nodes.Add(path);
        		}
        	}
        }
        
        public void DeleteAllFolders_Click(object sender, EventArgs e) {
        	treeViewFolders.Nodes.Clear();
	    }
	    
	    public void Reload_Click(object sender, EventArgs e) {
	    	Application.Restart();
	    }
        
	    public void CheckBrowseable(ToolStripMenuItem toolStrip) {
	    	if (toolStrip.Text == "Просмотр") {
	    		ShowInterfaceBrowse();
	    	} else {
	    		HideInterfaceBrowse();
	    	}
	    }
	    
	    // интерфейс просмотра картинок
	    PictureBox pMain = new PictureBox();
	    Button bLeft = new Button();
	    Button bRight = new Button();
	    Label lNamePicture = new Label();
	    
	    /// <summary>
	    ///  Показать интерфейс просмотра картинок
	    /// </summary>
	    public void ShowInterfaceBrowse() {
	    	pMain.Show();
	    	bLeft.Show();
	    	bRight.Show();
	    	lNamePicture.Show();
	    }
	    
	    /// <summary>
	    ///  Скрыть интерфейс просмотра картинок
	    /// </summary>
	    public void HideInterfaceBrowse() {
	    	pMain.Hide();
	    	bLeft.Hide();
	    	bRight.Hide();
	    	lNamePicture.Hide();
	    }
	    
	    string CurrentFolder = string.Empty;
	    static List<string> CurrentFiles = new List<string>();
	    
	    public void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs args) {
	    	string path = args.Node.Text;
	    	
	    	if (Directory.Exists(path)) {
	    		CurrentFolder = path;
	    		CurrentFiles = Directory.EnumerateFiles(CurrentFolder).ToList();
	    	}
	    	
	    	try {
	    		pMain.BackgroundImage = Image.FromFile(path);
	    		lNamePicture.Text = Path.GetFileName(path);
	    	} catch { ; }
	    }
	    
	    int left = CurrentFiles.Count;
	    int right = 0;
	    
	    // TODO
	    public void Left_Click(object sender, EventArgs e) {
	    	if (string.IsNullOrEmpty(CurrentFolder) || CurrentFiles.Count == 0) return;
	    	if (left <= 0) left = CurrentFiles.Count;
	    	
	    	string filename = CurrentFiles[--left];
	    	pMain.BackgroundImage = Image.FromFile(filename);
	    	lNamePicture.Text = Path.GetFileName(filename);
	    }
	    
	    public void Right_Click(object sender, EventArgs e) {
	    	if (string.IsNullOrEmpty(CurrentFolder) || CurrentFiles.Count == 0) return;
	    	if (right >= CurrentFiles.Count-1) right = 0;
	    	
	    	string filename = CurrentFiles[++right];
	    	pMain.BackgroundImage = Image.FromFile(filename);
	    	lNamePicture.Text = Path.GetFileName(filename);
	    }
	    
		public void Initialize() {
        	this.pHeader.BackColor = Color.FromArgb(46,48,49);
        	this.pHeader.BorderStyle = BorderStyle.FixedSingle;
			this.pHeader.Controls.Add(this.lTitle);
			this.pHeader.Controls.Add(this.pCollapse);
	        this.pHeader.Controls.Add(this.pExpand);
	        this.pHeader.Controls.Add(this.pClose);
        this.pHeader.Dock = DockStyle.Top;
        this.pHeader.Location = new Point(0, 0);
        this.pHeader.Size = new Size(630, 36);
        this.pHeader.MouseDown += MouseDown_Click;
		this.pHeader.MouseMove += MouseMove_Click;
		this.pHeader.MouseUp += MouseUp_Click;

		this.pBody.BackColor = Color.FromArgb(46,48,49);
        this.pBody.Controls.Add(this.splitContainerMain);
        this.pBody.Controls.Add(this.menuStrip1);
        this.pBody.Dock = DockStyle.Fill;
        this.pBody.Location = new Point(0, 36);
        this.pBody.Size = new Size(630, 331);
        
        this.pClose.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close.png");
	        this.pClose.BackgroundImageLayout = ImageLayout.Stretch;
	        this.pClose.Dock = DockStyle.Right;
	        this.pClose.Location = new Point(821, 0);
	        this.pClose.Size = new Size(34, 34);
	        this.pClose.Click += (o,e) => this.Close();
	        this.pClose.MouseHover += (o,e) => this.pClose.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close_hover.png");
	        this.pClose.MouseLeave += (o,e) => this.pClose.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close.png");
	
			this.pExpand.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "expand.png");
	        this.pExpand.BackgroundImageLayout = ImageLayout.Stretch;
	        this.pExpand.Dock = DockStyle.Right;
	        this.pExpand.Location = new Point(787, 0);
	        this.pExpand.Size = new Size(34, 34);
	        this.pExpand.MouseHover += (o,e) => this.pExpand.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "expand_hover.png");
	        this.pExpand.MouseLeave += (o,e) => this.pExpand.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "expand.png");
	        this.pExpand.Click += ExpandForm_Click;
	
			this.pCollapse.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "collapse.png");
	        this.pCollapse.BackgroundImageLayout = ImageLayout.Stretch;
	        this.pCollapse.Dock = DockStyle.Right;
	        this.pCollapse.Location = new Point(753, 0);
	        this.pCollapse.Size = new Size(34, 34);
			this.pCollapse.MouseHover += (o,e) => this.pCollapse.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "collapse_hover.png");
	        this.pCollapse.MouseLeave += (o,e) => this.pCollapse.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "collapse.png");
	        this.pCollapse.Click += CollapseForm_Click;
	        

        this.menuStrip1.Items.AddRange(
        	new ToolStripItem[8]{this.tFile, this.tEdit, this.tBrowse, this.tAudioPlayer, this.tFilter, this.tTools, this.tWindow, this.tAbout});
        this.menuStrip1.Location = new Point(0, 0);
        this.menuStrip1.Size = new Size(630, 24);
        this.menuStrip1.BackColor = Color.FromArgb(46,48,49);
        this.menuStrip1.Renderer = new DarkMenuStripRenderer();

        this.tFile.DropDownItems.AddRange(new ToolStripItem[] { tAddFolder, tDeleteAllFolders, tExit });
        this.tFile.Size = new Size(48, 20);
        this.tFile.Text = "Файл";
        this.tFile.ForeColor = Color.WhiteSmoke;
        
        this.tAddFolder.Size = new Size(48, 20);
        this.tAddFolder.Text = "Добавить папку";
        this.tAddFolder.ForeColor = Color.WhiteSmoke;
        this.tAddFolder.Click += AddFolder_Click;
        
        this.tDeleteAllFolders.Size = new Size(48, 20);
        this.tDeleteAllFolders.Text = "Удалить все папки из дерева";
        this.tDeleteAllFolders.ForeColor = Color.WhiteSmoke;
        this.tDeleteAllFolders.Click += DeleteAllFolders_Click;
        
        this.tExit.Size = new Size(48, 20);
        this.tExit.Text = "Выход";
        this.tExit.ForeColor = Color.WhiteSmoke;
        this.tExit.Click += (o, e) => this.Close();
 
        this.tEdit.Size = new Size(59, 20);
        this.tEdit.Text = "Правка";
        this.tEdit.ForeColor = Color.WhiteSmoke;

        this.tBrowse.Size = new Size(76, 20);
        this.tBrowse.Text = "Просмотр";
        this.tBrowse.ForeColor = Color.WhiteSmoke;
        this.tBrowse.Click += (o,e) => CheckBrowseable(tBrowse);
 
        this.tAudioPlayer.Size = new Size(86, 20);
        this.tAudioPlayer.Text = "Аудиоплеер";
        this.tAudioPlayer.ForeColor = Color.WhiteSmoke;
        this.tAudioPlayer.Click += (o,e) => CheckBrowseable(tAudioPlayer);

        this.tFilter.Size = new Size(60, 20);
        this.tFilter.Text = "Фильтры";
        this.tFilter.ForeColor = Color.WhiteSmoke;
        this.tFilter.Click += (o,e) => CheckBrowseable(tFilter);
 
        this.tTools.DropDownItems.Add(tConverterColors);
        this.tTools.Size = new Size(95, 20);
        this.tTools.Text = "Инструменты";
        this.tTools.ForeColor = Color.WhiteSmoke;
        
        this.tConverterColors.Size = new Size(95, 20);
        this.tConverterColors.Text = "Конвертер цветов";
        this.tConverterColors.ForeColor = Color.WhiteSmoke;

        this.tWindow.DropDownItems.Add(tReload);
        this.tWindow.Size = new Size(48, 20);
        this.tWindow.Text = "Окно";
        this.tWindow.ForeColor = Color.WhiteSmoke;
        
        this.tReload.Size = new Size(95, 20);
        this.tReload.Text = "Перезагрузить";
        this.tReload.ForeColor = Color.WhiteSmoke;
        this.tReload.Click += Reload_Click;

        this.lTitle.Location = new Point(3, 7);
        this.lTitle.Size = new Size(340, 26);
        this.lTitle.Text = "PhotoEditor v1.0.0.0 2020";
        this.lTitle.MouseDown += MouseDown_Click;
		this.lTitle.MouseMove += MouseMove_Click;
		this.lTitle.MouseUp += MouseUp_Click;
 
        this.splitContainerMain.Dock = DockStyle.Fill;
        this.splitContainerMain.Location = new Point(0, 24);
        this.splitContainerMain.Size = new Size(630, 307);
        this.splitContainerMain.SplitterDistance = 210;
        
        this.splitContainerMain.Panel1.BackColor = Color.FromArgb(41,41,41);
        this.splitContainerMain.Panel1.Controls.Add(treeViewFolders);
        this.splitContainerMain.Panel2.BackColor = Color.FromArgb(41,41,41);
        
        this.splitContainerMain.Panel2.Controls.AddRange(new Control[] {
                              pMain, bLeft, bRight, lNamePicture                           
                                                         });
        
        pMain.BorderStyle = BorderStyle.FixedSingle;
        pMain.BackgroundImageLayout = ImageLayout.Stretch;
        pMain.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        pMain.Size = new Size(280,180);
        pMain.Left = 70;
        pMain.Top = 70;
        
        lNamePicture.Anchor = AnchorStyles.Bottom;
        lNamePicture.Left = 170;
        lNamePicture.Top = 260;
        lNamePicture.ForeColor = Color.WhiteSmoke;
        lNamePicture.Width = 500;
        
        bLeft.Left = 20;
        bLeft.Top = 170;
        bLeft.Anchor = AnchorStyles.Left;
        bLeft.Text = "<";
        bLeft.Width = 40;
        bLeft.FlatStyle = FlatStyle.Flat;
        bLeft.FlatAppearance.BorderSize = 0;
        bLeft.Font = new Font("Consolas", 22, FontStyle.Regular);
        bLeft.ForeColor = Color.White;
        bLeft.Click += Left_Click;
        
        bRight.Left = 370;
        bRight.Top = 170;
        bRight.Anchor = AnchorStyles.Right;
        bRight.Text = ">";
        bRight.Width = 40;
        bRight.FlatStyle = FlatStyle.Flat;
        bRight.FlatAppearance.BorderSize = 0;
        bRight.Font = new Font("Consolas", 22, FontStyle.Regular);
        bRight.ForeColor = Color.White;
        bRight.Click += Right_Click;
        
        treeViewFolders.BackColor = Color.FromArgb(41,41,41);
	        treeViewFolders.ForeColor = Color.WhiteSmoke;
	        treeViewFolders.Left = 2;
	        treeViewFolders.Top = 30;
	        treeViewFolders.Width = 160;
	        treeViewFolders.Height = 250;
	        treeViewFolders.BorderStyle = BorderStyle.None;
	        treeViewFolders.Scrollable = true;
	        treeViewFolders.ImageIndex = 15;
	        treeViewFolders.SelectedImageIndex = 15;
	        //treeViewFolders.ImageList = this.ProjectExplorerImageList;
	        treeViewFolders.Font = new Font("Consolas", 11, FontStyle.Regular, GraphicsUnit.Point, (Byte)204);
	        treeViewFolders.Anchor = (AnchorStyles)(AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom);
	        treeViewFolders.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(TreeView_NodeMouseDoubleClick);

        this.tAbout.Size = new Size(94, 20);
        this.tAbout.Text = "О программе";
        this.tAbout.ForeColor = Color.WhiteSmoke;

        this.BackColor = SystemColors.AppWorkspace;
        this.ClientSize = new Size(630, 367);
        this.Controls.Add(this.pBody);
        this.Controls.Add(this.pHeader);
        this.Font = new Font("Consolas", 14.25f, FontStyle.Regular, GraphicsUnit.Point, (Byte)0);
        this.ForeColor = SystemColors.ButtonFace;
        this.FormBorderStyle = FormBorderStyle.None;
        
        //======= Filter Form =========//
        this.pHeaderFilter.Controls.Add(this.lTitleFilter);
        this.pHeaderFilter.Controls.Add(this.pCloseFilter);
        this.pHeaderFilter.Dock = DockStyle.Top;
        this.pHeaderFilter.Location = new Point(0, 0);
        this.pHeaderFilter.Size = new Size(649, 40);
        this.pHeaderFilter.MouseDown += MouseDown_Click;
		this.pHeaderFilter.MouseMove += MouseMove_Click;
		this.pHeaderFilter.MouseUp += MouseUp_Click;
        // 
        // lSource
        // 
        this.lSource.Location = new Point(92, 73);
        this.lSource.Size = new Size(196, 25);
        this.lSource.Text = TitleFilter;
        this.lSource.MouseDown += MouseDown_Click;
		this.lSource.MouseMove += MouseMove_Click;
		this.lSource.MouseUp += MouseUp_Click;
        
        
        this.pCloseFilter.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close.png");
	    this.pCloseFilter.BackgroundImageLayout = ImageLayout.Stretch;
	        this.pCloseFilter.Dock = DockStyle.Right;
	        this.pCloseFilter.Location = new Point(821, 0);
	        this.pCloseFilter.Size = new Size(34, 34);
	        this.pCloseFilter.Click += (o,e) => FilterForm.Hide();
	        this.pCloseFilter.MouseHover += (o,e) => this.pCloseFilter.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close_hover.png");
	        this.pCloseFilter.MouseLeave += (o,e) => this.pCloseFilter.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close.png");
	
        // 
        // lResult
        // 
        this.lResult.Location = new Point(401, 73);
        this.lResult.Size = new Size(196, 25);
        this.lResult.Text = "Результат:";
        // 
        // pictResult
        // 
        this.pictResult.BorderStyle = BorderStyle.FixedSingle;
        this.pictResult.BackgroundImageLayout = ImageLayout.Stretch;
        this.pictResult.Location = new Point(334, 123);
        this.pictResult.Size = new Size(293, 211);
        // 
        // pictSource
        // 
        this.pictSource.BorderStyle = BorderStyle.FixedSingle;
        this.pictSource.BackgroundImageLayout = ImageLayout.Stretch;
        this.pictSource.Location = new Point(22, 123);
        this.pictSource.Size = new Size(293, 211);
        // 
        // bSaveResult
        // 
        this.bSaveResult.BackColor = SystemColors.ButtonShadow;
        this.bSaveResult.FlatAppearance.BorderSize = 0;
        this.bSaveResult.FlatStyle = FlatStyle.Flat;
        this.bSaveResult.Font = new Font("Consolas", 12, FontStyle.Regular, GraphicsUnit.Point, (Byte)204);
        this.bSaveResult.Location = new Point(434, 345);
        this.bSaveResult.Size = new Size(193, 29);
        this.bSaveResult.Text = "Сохранить результат";
        this.bSaveResult.UseVisualStyleBackColor = false;
        // 
        // lTitleFilter
        // 
        this.lTitleFilter.Location = new Point(12, 9);
        this.lTitleFilter.Size = new Size(409, 25);
        this.lTitleFilter.Text = "Эффект:";
        // 
        // bApply
        // 
        this.bApply.BackColor = Color.FromArgb(41,41,41);;
        this.bApply.FlatAppearance.BorderSize = 0;
        this.bApply.FlatStyle = FlatStyle.Flat;
        this.bApply.Font = new Font("Consolas", 12, FontStyle.Regular, GraphicsUnit.Point, (Byte)204);
        this.bApply.Location = new Point(232, 345);
        this.bApply.Size = new Size(183, 29);
        this.bApply.Text = "Применить фильтр";
        this.bApply.UseVisualStyleBackColor = false;
		
        FilterForm.BackColor = Color.FromArgb(41,41,41);;
        FilterForm.ClientSize = new Size(649, 386);
        FilterForm.Controls.Add(this.bApply);
        FilterForm.Controls.Add(this.bSaveResult);
        FilterForm.Controls.Add(this.pictSource);
        FilterForm.Controls.Add(this.pictResult);
        FilterForm.Controls.Add(this.lResult);
        FilterForm.Controls.Add(this.lSource);
        FilterForm.Controls.Add(this.pHeaderFilter);
        FilterForm.Font = new Font("Consolas", 14.25f, FontStyle.Regular, GraphicsUnit.Point, (Byte)204);
        FilterForm.FormBorderStyle = FormBorderStyle.None;
        FilterForm.Show();
		}
	    
	    public string TitleFilter {
			get {
				return this.lTitleFilter.Text;
			}
			set {
				this.lTitleFilter.Text = value;
			}
		} 
	}
	
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			// TEST:Добавление текста на картинку: SUCCESS
			/*
			string path = @"C:\Users\fdshfgas\Pictures\20\city.png";
			string new_path = @"C:\Users\fdshfgas\Desktop\new_city.png";
			ImageDecorator.AddTextInImage(path, new_path, "I LOVE YOU");
			*/
			
			//PhotoEditor photoEditor = new PhotoEditor();
			//Application.Run(photoEditor);
			
			// TEST API:
//			byte[] bytes = new byte[] { 70,70,48,48,70,70 };
//			
//			string hex = ConverterColorsApi.BytesToHex(bytes);
//			int[] rgbs = ConverterColorsApi.BytesToRGB(bytes);
//			
//			StringBuilder builder = new StringBuilder();
//			
//			builder.Append(rgbs[0] + ",").Append(rgbs[1] + ",").Append(rgbs[2]);
//			
//			Console.WriteLine(hex);
//			Console.WriteLine(builder.ToString());
			
			// TEST RGB vs CMYK:
//			CmykColor cmykColor = ConverterColorsApi.RgbToCmyk(12,34,56);
//			Console.WriteLine(cmykColor.C + " ^ " + cmykColor.M + " ^ " + cmykColor.Y + " ^ " + cmykColor.K);
//			
//			Color color = ConverterColorsApi.CmykToRgb(cmykColor.C,cmykColor.M,cmykColor.Y,cmykColor.K);
//			Console.WriteLine(color.R + " ^ " + color.G + " ^ " + color.B);
//			
			// TEST RGB vs HSL
//			Color color = HslRgbUtils.HslToRgb(0.12f,0.12f,0.12f);
//			Console.WriteLine(color.R + " " + color.G + " " + color.B);
//			
//			double h = 0;
//			double s = 0;
//			double l = 0;
//			
//			HslRgbUtils.RgbToHsl
//				(color, out h, out s, out l);
//			
//			Console.WriteLine(h + " " + s + " "  + l);
			
			//TEST :
//			Color color = Color.FromArgb(255, 0, 0);
//			XYZ xyz = ConverterColorsApi.RgbToXyz(color);
//			
//			Color rescolor = ConverterColorsApi.XyzToRgb(xyz);
//			
//			
//			Console.WriteLine(rescolor.R + " " + rescolor.G +  " " + rescolor.B);
//			
			//Console.ReadLine();
		}
	}
}
