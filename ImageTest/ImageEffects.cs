using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageTest
{
    public class Line
    {
        public PointF Start;
        public PointF End;
        public double VecX;
        public double VecY;
        public double Angle;
        public Line(PointF start, PointF end)
        {
            Start = start;
            End = end;
            VecX = end.X - start.X / VecDistance(end.X - start.X, end.Y - start.Y);
            VecY = end.Y - start.Y / VecDistance(end.X - start.X, end.Y - start.Y);
        }
        public Line(double angle)
        {
            VecX = Math.Cos(angle);
            VecY = Math.Sin(angle);
            Angle = angle;
            Start = new PointF();
            End = new PointF((float)VecX, (float)VecY);
        }
        double VecDistance(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        double GetDistanceToPoint(PointF point)
        {
            return Math.Abs(point.Y * VecX - point.X * VecY);
        }

        //public static double[,] GetInterpulatedLine(int length)
        //{     
        //    Line line = new Line(45.0*Math.PI/180); 
        //    int xSize = (int)Math.Ceiling(Math.Abs(line.VecX * length));
        //    int ySize = (int)Math.Ceiling(Math.Abs(line.VecY * length));

        //    double[,] xArr = { { 0, xSize },{ 0, xSize } };
        //    double[,] yArr = { { 0, 0 }, { ySize, ySize } };
        //    double[,] zArr = { { 0, 1 }, { 1, 0 } };


        //    //return mask;
        //}
    }
    public class ImageEffects
    {
        public static void Convolution(Bitmap input, double[,] kernel)
        {
            //Получаем байты изображения

            BitmapData data = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadWrite, input.PixelFormat);
 

            byte[] inputBytes = new byte[Math.Abs(data.Stride) * input.Height];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, inputBytes, 0, inputBytes.Length);
             
            byte[] outputBytes = new byte[inputBytes.Length];

            int width = input.Width;
            int height = input.Height;

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    double rSum = 0, gSum = 0, bSum = 0, kSum = 0;

                    for (int i = 0; i < kernelWidth; i++)
                        for (int j = 0; j < kernelHeight; j++)
                        {
                            int pixelPosX = x + (i - (kernelWidth / 2));
                            int pixelPosY = y + (j - (kernelHeight / 2));
                            if ((pixelPosX < 0) ||
                              (pixelPosX >= width) ||
                              (pixelPosY < 0) ||
                              (pixelPosY >= height)) continue;

                            if(GetStepByte(input.PixelFormat) == 1)
                            {
                                rSum += inputBytes[(width * pixelPosY + pixelPosX)] * kernel[i, j];
                                kSum += kernel[i, j];
                                continue;
                            }


                            byte r = inputBytes[3 * (width * pixelPosY + pixelPosX) + 0];
                            byte g = inputBytes[3 * (width * pixelPosY + pixelPosX) + 1];
                            byte b = inputBytes[3 * (width * pixelPosY + pixelPosX) + 2];

                            double kernelVal = kernel[i, j];

                            rSum += r * kernelVal;
                            gSum += g * kernelVal;
                            bSum += b * kernelVal;

                            kSum += kernelVal;
                        }

                    if (kSum <= 0) kSum = 1;

                    //Контролируем переполнения переменных
                    rSum /= kSum;
                    if (rSum < 0) rSum = 0;
                    if (rSum > 255) rSum = 255;

                    gSum /= kSum;
                    if (gSum < 0) gSum = 0;
                    if (gSum > 255) gSum = 255;

                    bSum /= kSum;
                    if (bSum < 0) bSum = 0;
                    if (bSum > 255) bSum = 255;

                    //Записываем значения в результирующее изображение

                    if (GetStepByte(input.PixelFormat) == 1)
                    {
                        outputBytes[(width * y + x)] = (byte)rSum;
                        continue;
                    }
                    outputBytes[3 * (width * y + x) + 0] = (byte)rSum;
                    outputBytes[3 * (width * y + x) + 1] = (byte)gSum;
                    outputBytes[3 * (width * y + x) + 2] = (byte)bSum;
                }
        

            //Возвращаем отфильтрованное изображение
            System.Runtime.InteropServices.Marshal.Copy(outputBytes, 0, data.Scan0, outputBytes.Length);
            input.UnlockBits(data);
            return; ///BitmapBytes.GetBitmap(outputBytes, width, height);
        }


        public static void Sepia(Bitmap input)
        {
            //if (GetStepByte(input.PixelFormat) == 1)
            //{
            //    BitmapData data = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadWrite, input.PixelFormat);
            //    Bitmap temp = new Bitmap(input.Width, input.Height, data.Stride, PixelFormat.Format24bppRgb, data.Scan0);
            //    input.UnlockBits(data);
            //    input = temp.Clone(new Rectangle(0, 0, input.Width, input.Height), PixelFormat.Format24bppRgb);
            //    temp.Dispose();
            //}
            if (GetStepByte(input.PixelFormat) == 1)
                return;
                for (int i = 0; i < input.Width; i++)
                for (int j = 0; j < input.Height; j++)
                {

                    Color p = input.GetPixel(i, j);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    //calculate temp value
                    int tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                    int tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                    int tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);

                    //set new RGB value
                    r = tr > 255 ? 255 : tr;
                    g = tg > 255 ? 255 : tg;
                    b = tb > 255 ? 255 : tb;

                    //set the new RGB value in image pixel
                    input.SetPixel(i, j, Color.FromArgb(a, r, g, b));

                }
        }

        public static void Poster(Bitmap input, int count)
        {
            if (GetStepByte(input.PixelFormat) == 1)
                return;
            for (int i = 0; i < input.Width; i++)
                for (int j = 0; j < input.Height; j++)
                {
                    Color p = input.GetPixel(i, j);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;


                    for (int k = 0; k < count; k++)
                    {
                        double X1 = 255 * k / (double)count;
                        double X2 = 255 * (k+1) / (double)count;
                        if (r >= X1 && r < X2) r = (int)((X2 + X1) / 2.0);
                        if (g >= X1 && g < X2) g = (int)((X2 + X1) / 2.0);
                        if (b >= X1 && b < X2) b = (int)((X2 + X1) / 2.0);
                    }


                    //set the new RGB value in image pixel
                    input.SetPixel(i, j, Color.FromArgb(a, r, g, b));

                }
        }

        public static void GrayScale(Bitmap input)
        {
            for (int i = 0; i < input.Width; i++)
                for (int j = 0; j < input.Height; j++)
                {
                    Color p = input.GetPixel(i, j);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    int avg = (r + g + b) / 3;

                    //set the new RGB value in image pixel
                    input.SetPixel(i, j, Color.FromArgb(a, avg, avg, avg));

                }
        }

        static int GetStepByte(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Format8bppIndexed:
                    return 1;
                case PixelFormat.Format24bppRgb:
                    return 3;
                case PixelFormat.Format32bppArgb:
                    return 4;
            }
            return 0;
        }

    }

}
