using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;
namespace DesignGUI
{
	public class ytButton : Control {
		private StringFormat SF = new StringFormat();
		private bool MouseEntered = false;
		private bool MousePressed = false;
		
		public ytButton() {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
			DoubleBuffered = true;
			
			Size = new Size(100, 30);
			BackColor = Color.Tomato;
			SF.Alignment = StringAlignment.Center;
			SF.LineAlignment = StringAlignment.Center;
		}
		
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Graphics graph = e.Graphics;
			graph.SmoothingMode = SmoothingMode.HighQuality;
			graph.Clear(Parent.BackColor);
			
			Rectangle rect = new Rectangle(0,0, Width - 1, Height - 1);
			
			graph.DrawRectangle(new Pen(BackColor), rect);
			graph.FillRectangle(new SolidBrush(BackColor), rect);
			
			if (MouseEntered) {
				graph.DrawRectangle(new Pen(Color.FromArgb(60, Color.White)), rect);
				graph.FillRectangle(new SolidBrush(Color.FromArgb(60, Color.White)), rect);
			}
			
			if (MousePressed) {
				graph.DrawRectangle(new Pen(Color.FromArgb(30, Color.Black)), rect);
				graph.FillRectangle(new SolidBrush(Color.FromArgb(30, Color.Black)), rect);
			}
			
			graph.DrawString(Text, Font, new SolidBrush(ForeColor), rect, SF);
		}
		
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			
			MouseEntered = true;
			Invalidate();
		}
		
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			
			MouseEntered = false;
			Invalidate();
		}
		
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			
			MousePressed = true;
			Invalidate();
		}
		
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			
			MousePressed = true;
			Invalidate();
		}
	}
	
	public class winCard : Control {
		#region Переменные
		private float CurtainHeight; // базовая высота шторки
		
		#endregion
		
		#region Свойства
		public Color BackColorCurtain {get; set;}
		
		#endregion
		
		public winCard() {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
			DoubleBuffered = true;
			
			Size = new Size(100, 30);
			CurtainHeight = Height - 60;
			
			Font = new Font("Verdana", 9F, FontStyle.Regular);
			BackColor = Color.White;
			
		}
		
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			
			Graphics graph = e.Graphics;
			graph.SmoothingMode = SmoothingMode.HighQuality;
			graph.Clear(Parent.BackColor);
			
			Rectangle rect = new Rectangle(0,0, Width - 1, Height -1);
			Rectangle rectCurtain = new Rectangle(0,0, Width - 1, (int)CurtainHeight);
			
			graph.FillRectangle(new SolidBrush(BackColor), rect);
		}
	}
	
	static class Program
	{
		public static void myClick(object sender, EventArgs e) {
			MessageBox.Show("Hello", "Message", MessageBoxButtons.OK);
		}
		
		public static void Main(string[] args)
		{
			/*Form f = new Form();
			ytButton yb = new ytButton();
			yb.Text = "Click";
			yb.Click += myClick;
			
			f.Controls.Add(yb);
			Application.Run(f);
			*/
			
			// EXAMPLE 2:
			Form f = new Form();
			
			//f.Controls.Add(yb);
			Application.Run(f);
			Console.ReadKey(true);
		}
		
		
	}
}
