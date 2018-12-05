using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZeroTeleprompter
{
    public partial class Form1 : Form
    {

        public Control SourceControl
        {
            get
            {
                return richTextBox1;
            }
        }
        public DestForm DestForm
        {
            get;
            set;
        }
        public Form1()
        {
            this.FullScreen();
            FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    DestForm.Close();
                    break;
                case Keys.F4:
                    DestForm.MirrorState = !DestForm.MirrorState;
                    //Left = DestForm.MirrorState?600:0;
                    Left = DestForm.MirrorState ? 0 : Screen.PrimaryScreen.WorkingArea.Width;
                    break;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }


    public class DestForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr wnd);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr wnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
            int nWidthDest, int nHeightDest,
            IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
            uint dwRop);

        const uint SRCCOPY = 0x00CC0020;

        Timer timer;

        Form1 srcForm;
        public DestForm(Form1 srcForm)
        {
            Label lb1 = new Label();
            lb1.Text = "镜像窗体";
            this.Controls.Add(lb1);
            this.srcForm = srcForm;
            srcForm.DestForm = this;
            srcForm.Show();
            this.FullScreen();
            TopMost = false;
            timer = new Timer();

            Text = "Mirror";

            GotFocus += (s, e) => srcForm.SourceControl.Focus();

            timer.Interval = 1;
            timer.Enabled = true;
            timer.Tick += (s, e) => CapSrcFormAndMirror();

        }

        public bool MirrorState
        {
            get;
            set;
        }

        void CapSrcFormAndMirror()
        {
            var width = srcForm.SourceControl.Width;
            var height = srcForm.SourceControl.Height;
            srcForm.SourceControl.Invalidate();
            using (var g = CreateGraphics())
            {
                var srcDc = GetDC(srcForm.SourceControl.Handle);
                var dstDc = g.GetHdc();

                if (MirrorState)
                    StretchBlt(dstDc, 0, 0, width, height, srcDc, width - 1, 0, -width, height, SRCCOPY);
                else
                    StretchBlt(dstDc, 0, 0, width, height, srcDc, 0, 0, width, height, SRCCOPY);

                g.ReleaseHdc();
                ReleaseDC(srcForm.SourceControl.Handle, srcDc);
            }
        }
    }

    public static class Program233
    {

        public static void FullScreen(this Form frm)
        {
            frm.StartPosition = FormStartPosition.Manual;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Size = Screen.PrimaryScreen.Bounds.Size;
            frm.Location = new Point(0, 0);
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //            Application.Run(new Form1());
            Application.Run(new DestForm(new Form1()));

        }
    }

}
