using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ImageTest
{
    public partial class MainForm : Form
    {
        private static Image origin;
        public static Bitmap changedImage;
        private static MotionForm motionModalWin;

        public MainForm()
        {
            InitializeComponent();
        }

        public void MotionImage(bool isVer, bool isHor, int horSpeed, int vertSpeed)
        {
            if (changedImage == null)
                return;

            double[,] core;
            if (isHor)
            {
                core = new double[horSpeed, 1];
                for (int i = 0; i < horSpeed; i++)
                    core[i, 0] = 1;
                ImageEffects.Convolution(changedImage, core);
            }

            if (isVer)
            {
                core = new double[1,vertSpeed];
                for (int j = 0; j < vertSpeed; j++)
                    core[0,j] = 1;
                ImageEffects.Convolution(changedImage, core);
            }


            pictureBox1.Invalidate();
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
            if (motionModalWin == null || motionModalWin.IsDisposed == true) 
                motionModalWin = new MotionForm(this);
            motionModalWin.Show();
        }

        private void SepiaButton_Click(object sender, EventArgs e)
        {
            if (changedImage != null)
            {
                ImageEffects.Sepia(changedImage);
                pictureBox1.Invalidate();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (changedImage != null)
            {
                changedImage = (Bitmap)origin.Clone();
                pictureBox1.Invalidate();
            }
        }

        private void PosterImageButton_Click(object sender, EventArgs e)
        {
            if (changedImage != null)
            {
                ImageEffects.Poster(changedImage,3);
                pictureBox1.Invalidate();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (changedImage != null)
            {
                ImageEffects.GrayScale(changedImage);
                pictureBox1.Invalidate();
            }
        }

        private void GaussBlurButton_Click(object sender, EventArgs e)
        {
            if (changedImage != null)
            {
                double[,] core = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
                ImageEffects.Convolution(changedImage, core);
                pictureBox1.Invalidate();
            }
        }
    }
}
