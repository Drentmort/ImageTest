using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ImageTest
{
    public partial class MainForm : Form
    {
        private static Image origin;
        private static Bitmap changedImage;
        private static MotionForm motionModalWin;

        public MainForm()
        {
            InitializeComponent();
        }

        private void OpenImageButton_Click(object sender, EventArgs e)
        {
            if (OpenImageDialog.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = OpenImageDialog.FileName;

            origin = new Bitmap(filename);
            changedImage = (Bitmap)origin.Clone();
            pictureBox1.Invalidate();

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (origin == null) return;

            Bitmap temp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using(Graphics g = Graphics.FromImage(temp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(changedImage, 0, 0, pictureBox1.Width, pictureBox1.Height);
            }

            e.Graphics.DrawImage(temp, pictureBox1.Location);
        }

        private void MotionBlurButton_Click(object sender, EventArgs e)
        {
            if (motionModalWin == null) ;
                motionModalWin = new MotionForm();
            motionModalWin.Show();
        }
    }
}
