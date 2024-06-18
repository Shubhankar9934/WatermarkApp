using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WatermarkApp
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();
            SetupWatermark();
            SetupTimer();
        }

        private void SetupWatermark()
        {
            // Set the form style to make it transparent and click-through
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.Opacity = 0.5;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);

            this.Paint += new PaintEventHandler(Form1_Paint);
            UpdateWatermark();
        }

        private void SetupTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 30000; // 30 seconds
            timer.Tick += (s, e) => UpdateWatermark();
            timer.Start();
        }

        private void UpdateWatermark()
        {
            this.Invalidate(); // Causes the form to be redrawn
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            string ipAddress = "192.168.1.1"; // Replace with actual method to get IP address
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string username = Environment.UserName;
            string watermarkText = $"{ipAddress} | {timestamp} | {username}";

            Font font = new Font("Arial", 18, FontStyle.Bold); // Smaller font size
            // Use a semi-transparent brush with an alpha value of 64 (out of 255)
            Brush brush = new SolidBrush(Color.FromArgb(64, Color.Red));
            SizeF textSize = e.Graphics.MeasureString(watermarkText, font);
            float angle = -45; // Rotate the text from southwest to northeast

            // Define specific positions for watermarks
            Point[] positions = new Point[]
            {
                new Point(this.Width / 4, this.Height * 3 / 4),
                new Point(this.Width * 3 / 4, this.Height * 3 / 4),
                new Point(this.Width / 4, this.Height / 4),
                new Point(this.Width * 3 / 4, this.Height / 4),
                new Point(this.Width / 2, this.Height / 2)
            };

            // Draw the watermark text at the specified positions with rotation
            foreach (var position in positions)
            {
                e.Graphics.TranslateTransform(position.X, position.Y);
                e.Graphics.RotateTransform(angle);
                e.Graphics.DrawString(watermarkText, font, brush, 0, 0);
                e.Graphics.ResetTransform();
            }
        }

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Set the window to be click-through
            uint initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
        }
    }
}
