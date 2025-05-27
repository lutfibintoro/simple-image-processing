
namespace ImageQuantizationData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ScottPlot.Colormaps;


    internal class Picker : Form
    {
        private int _x;
        private int _y;
        private readonly Bitmap _bitmap;
        private readonly int[][] _grayscaleBitmap;
        private readonly ImagesPlots _imagesPlots;
        private readonly Panel _panel = new()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };

        internal Picker(string formName, string path)
        {
            _imagesPlots = new(formName, [[0], [0]]);
            Thread thread = new(() =>
            {
                Application.EnableVisualStyles();
                Application.Run(_imagesPlots);
            })
            {
                IsBackground = true
            };
            thread.Start();

            _bitmap = new(path);
            _grayscaleBitmap = new int[_bitmap.Height][];
            for (int i = 0; i < _bitmap.Height; i++)
            {
                _grayscaleBitmap[i] = new int[_bitmap.Width];
                for (int j = 0; j < _bitmap.Width; j++)
                {
                    Color pxColor = _bitmap.GetPixel(j, i);
                    _grayscaleBitmap[i][j] = (int)(0.299 * pxColor.R + 0.587 * pxColor.G + 0.114 * pxColor.B);
                }
            }

            this.Name = formName;
            this.Text = formName;

            PictureBox pictureBox = new()
            {
                Image = Image.FromFile(path.Split('.')[0] + "_gray.png"),
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            _panel.Controls.Add(pictureBox);
            _panel.Cursor = new("unnamed.cur");
            this.Controls.Add(_panel);

            _panel.Focus();
            _panel.MouseWheel += Panel_MouseWheel;
            pictureBox.MouseMove += PictureBox_MouseMove;
        }

        private void Panel_MouseWheel(object? sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (e.Delta > 0)
                {
                    _x -= 15;
                    _panel.AutoScrollPosition = new Point(_x, _y);
                }
                else if (e.Delta < 0)
                {
                    _x += 15;
                    _panel.AutoScrollPosition = new Point(_x, _y);
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    _y -= 10;
                    _panel.AutoScrollPosition = new Point(_x, _y);
                }
                else if (e.Delta < 0)
                {
                    _y += 10;
                    _panel.AutoScrollPosition = new Point(_x, _y);
                }
            }
        }


        private void PictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            int xMin = e.X - 7;
            int yMin = e.Y - 7;
            int xMax = e.X + 7;
            int yMax = e.Y + 7;
            if (xMin < 0) xMin = 0;
            if (yMin < 0) yMin = 0;
            if (xMax > _bitmap.Width - 1) xMax = _bitmap.Width - 1;
            if (yMax > _bitmap.Height - 1) yMax = _bitmap.Height - 1;
            int height = yMax - yMin;
            int width = xMax - xMin;

            int[][] colorValue = new int[height + 1][];
            for (int i = 0; i <= height; i++)
            {
                colorValue[i] = new int[width + 1];
                for (int j = 0; j <= width; j++)
                {
                    colorValue[i][j] = _grayscaleBitmap[i + yMin][j + xMin];
                }
            }

            _imagesPlots.RenderImage(colorValue);
        }
    }
}
