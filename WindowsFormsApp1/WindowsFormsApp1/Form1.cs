using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Image imgOriginal;
        int parlaklık = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "JPG|*.jpg|BMP|*.bmp|  All Files|*.*";
            if(sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }


            //pictureBox1.ImageLocation = sfd.FileName;
            pictureBox1.Image = Image.FromFile(sfd.FileName);
            imgOriginal = pictureBox1.Image;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);
            Bitmap gri = GriYap(image);

            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = gri;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);
            Bitmap negatif = NegatifYap(image);

            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = negatif;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);
            Bitmap dondur = Döndür(image,90.0f);

            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = dondur;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);
            Bitmap aynala = Aynala(image);

            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = aynala;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);
            Bitmap histogrameşitleme = histogramEşitleme(image);

            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = histogrameşitleme;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);
            Bitmap otsu = Otsu(image);

            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = otsu;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value > -100 && trackBar1.Value < 100)
            {
                pictureBox2.SizeMode = PictureBoxSizeMode.Normal;
                pictureBox2.Image = Zoom(imgOriginal, new Size(trackBar1.Value, trackBar1.Value));
                label4.Text = trackBar1.Value.ToString();
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label3.Text = trackBar2.Value.ToString();
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            Bitmap image = new Bitmap(pictureBox1.Image);
            pictureBox2.Image = Parlaklik(image, trackBar2.Value);
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label6.Text = trackBar3.Value.ToString();
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            Bitmap image = new Bitmap(pictureBox1.Image);
            pictureBox2.Image = Contrast(image, trackBar3.Value);
        }

        private Bitmap GriYap(Bitmap bmp)
        {
            for(int i = 0;i < bmp.Height; i++)
            {
                for(int j =  0;j < bmp.Width; j++)
                {
                    int deger = (bmp.GetPixel(j, i).R + bmp.GetPixel(j, i).G + bmp.GetPixel(j, i).B) / 3;

                    Color renk;

                    renk = Color.FromArgb(deger, deger, deger);

                    bmp.SetPixel(j, i, renk);
                }

            }

            return bmp;
        }

        private Bitmap NegatifYap(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Height; i++)
            {
                for(int j = 0;j < bmp.Width; j++)
                {
                    Color p = bmp.GetPixel(j, i);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    r = 255 - r;
                    g = 255 - g;
                    b = 255 - b;

                    bmp.SetPixel(j, i, Color.FromArgb(a, r, g, b));
                }
            }

            return bmp;
        }

        private Bitmap Döndür(Bitmap bitmap, float angle)
        {
            Bitmap returnBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Graphics graphics = Graphics.FromImage(returnBitmap);
            graphics.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2);
            graphics.RotateTransform(angle);
            graphics.TranslateTransform(-(float)bitmap.Width / 2, -(float)bitmap.Height / 2);
            graphics.DrawImage(bitmap, new Point(0, 0));
            return returnBitmap;
        }

        private Image Zoom(Image img, Size size)
        {
            Bitmap bmp = new Bitmap(img, img.Width + (img.Width * size.Width / 100), img.Height + (img.Height * size.Height / 100));
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            return bmp;
        }

        private static Bitmap Parlaklik(Bitmap bmp, int deger)
        {
            System.Drawing.Bitmap TempBitmap = bmp;
            float finalValue = (float)deger / 255.0f;
            System.Drawing.Bitmap NewBitmap = new System.Drawing.Bitmap(TempBitmap.Width, TempBitmap.Height);
            System.Drawing.Graphics NewGraphics = System.Drawing.Graphics.FromImage(NewBitmap);

            float[][] FloatColorMatrix =
            {
                new float[]  {1, 0, 0, 0, 0},
                new float[]  {0, 1, 0, 0, 0},
                new float[]  {0, 0, 1, 0, 0},
                new float[]  {0, 0, 0, 1, 0},
                new float[]  {finalValue, finalValue, finalValue, 1, 1},
            };

            System.Drawing.Imaging.ColorMatrix NewColorMatrix = new System.Drawing.Imaging.ColorMatrix(FloatColorMatrix);
            System.Drawing.Imaging.ImageAttributes Attributes = new System.Drawing.Imaging.ImageAttributes();
            Attributes.SetColorMatrix(NewColorMatrix);
            NewGraphics.DrawImage(TempBitmap, new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), 0, 0, TempBitmap.Width, TempBitmap.Height, System.Drawing.GraphicsUnit.Pixel, Attributes);
            Attributes.Dispose();
            NewGraphics.Dispose();
            return NewBitmap;

         }

        private Bitmap Aynala(Bitmap bmp)
        {
            Bitmap TempBitmap = new Bitmap(bmp.Width * 2, bmp.Height);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int lx = 0, rx = bmp.Width * 2 - 1; lx < bmp.Width; lx++, rx--)
                {
                    
                    Color p = bmp.GetPixel(lx, y);

                    
                    TempBitmap.SetPixel(lx, y, p);
                    TempBitmap.SetPixel(rx, y, p);
                }
            }

            return TempBitmap;
        }

        public Bitmap histogramEşitleme(Bitmap KaynakResim)
        {
            Bitmap renderedImage = KaynakResim;

            uint pixels = (uint)renderedImage.Height * (uint)renderedImage.Width;
            decimal Const = 255 / (decimal)pixels;

            int x, y, R, G, B;


            int[] HistogramRed2 = new int[256];
            int[] HistogramGreen2 = new int[256];
            int[] HistogramBlue2 = new int[256];


            for (var i = 0; i < renderedImage.Width; i++)
            {
                for (var j = 0; j < renderedImage.Height; j++)
                {
                    var piksel = renderedImage.GetPixel(i, j);

                    HistogramRed2[(int)piksel.R]++;
                    HistogramGreen2[(int)piksel.G]++;
                    HistogramBlue2[(int)piksel.B]++;

                }
            }

            int[] cdfR = HistogramRed2;
            int[] cdfG = HistogramGreen2;
            int[] cdfB = HistogramBlue2;

            for (int r = 1; r <= 255; r++)
            {
                cdfR[r] = cdfR[r] + cdfR[r - 1];
                cdfG[r] = cdfG[r] + cdfG[r - 1];
                cdfB[r] = cdfB[r] + cdfB[r - 1];
            }

            for (y = 0; y < renderedImage.Height; y++)
            {
                for (x = 0; x < renderedImage.Width; x++)
                {
                    Color pixelColor = renderedImage.GetPixel(x, y);

                    R = (int)((decimal)cdfR[pixelColor.R] * Const);
                    G = (int)((decimal)cdfG[pixelColor.G] * Const);
                    B = (int)((decimal)cdfB[pixelColor.B] * Const);

                    Color newColor = Color.FromArgb(R, G, B);
                    renderedImage.SetPixel(x, y, newColor);
                }
            }
            return renderedImage;
        }

        public Bitmap Otsu(Bitmap bmp)
        {

            int x, y;
            //int genislik = pictureBox2.Image.Width;
            //int yukseklik = pictureBox2.Image.Height;
            //byte[] pixeller = new byte[(int)genislik * yukseklik];
            byte[] pixeller = new byte[bmp.Width * bmp.Height];


            //Bitmap resim = (Bitmap)pictureBox2.Image;
            Bitmap resim = bmp;

            for (y = 0; y < bmp.Height; y++)
                for (x = 0; x < bmp.Width; x++)
                    pixeller[y * bmp.Width + x] = resim.GetPixel(x, y).R;


            int renk;
            for (y = 0; y < bmp.Height; y++)
                for (x = 0; x < bmp.Width; x++)
                {
                    renk = pixeller[y * bmp.Width + x];
                    resim.SetPixel(x, y, Color.FromArgb(renk, renk, renk));
                }

            return resim;
        }

        public Bitmap Contrast(Bitmap bmp, double contrast)
        {
            Bitmap temp = bmp;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    double pR = c.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = c.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = c.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    bmap.SetPixel(i, j,
        Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
            bmp = (Bitmap)bmap.Clone();

            return bmp;
        }


    }
}
