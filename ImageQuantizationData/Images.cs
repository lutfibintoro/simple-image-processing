namespace ImageQuantizationData
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class ColorInformation
    {
        public string? Path { get; set; }
        public ImageRGB[][]? DefaultColor { get; set; }
        public int[][]? GrayScaleColor { get; set; }
    }
    public class ImageRGB
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }



    internal class Images
    {
        internal readonly string Path;
        internal int Width;
        internal int Height;
        internal int[,]? ColorValueRGB;
        internal Dictionary<int, List<int>> SortColorGrouping = [];
        internal Bitmap? Bitmap;
        internal string ImageName
        {
            get
            {
                return Path.Split('/')[^1];
            }
        }


        internal Images(string path)
        {
            Path = path.Replace("\\", "/");
            Path = Path.Replace("\"", string.Empty);
        }



        private bool ValidImage()
        {
            if (!File.Exists(Path))
                return true;

            string[] splitedPath = Path.Split('/');
            string extensionFile = splitedPath[^1].Split('.')[1];

            if (!(extensionFile.Equals("png") || extensionFile.Equals("jpg")))
                return true;

            return false;
        }



        internal Images? Process()
        {
            if (ValidImage())
                return null;

            Console.WriteLine("processing...");
            using (Bitmap bmp = new(Path))
            {
                Width = bmp.Width;
                Height = bmp.Height;
                ColorValueRGB = new int[Height, Width];

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Color pxColor = bmp.GetPixel(x, y);
                        ColorValueRGB[y, x] = (int)(0.299 * pxColor.R + 0.587 * pxColor.G + 0.114 * pxColor.B);
                    }
                }
                ReWriteToGrayImage();
                GroupRGB();
            }

            return this;
        }


        private void ReWriteToGrayImage()
        {
            using Bitmap bmp = new(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int value = ColorValueRGB![y, x];
                    Color color = Color.FromArgb(value, value, value);
                    bmp.SetPixel(x, y, color);
                }
            }

            Bitmap = bmp;
            string[] splitedPath = Path.Split('.');
            string fullPath = splitedPath[0] + "_gray.png";
            bmp.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
        }


        private void GroupRGB()
        {
            int[] pxLenght = new int[ColorValueRGB!.Length];
            int index = 0;
            for (int y = 0; y < ColorValueRGB!.GetLength(0); y++)
            {
                for (int x = 0; x < ColorValueRGB!.GetLength(1); x++)
                {
                    pxLenght[index] = ColorValueRGB![y, x];
                    index++;
                }
            }


            Array.Sort(pxLenght);
            index = pxLenght[0];
            List<int> similarNumber = [];
            for (int i = 0; i < pxLenght.Length; i++)
            {
                if (pxLenght[i] == index)
                {
                    similarNumber.Add(pxLenght[i]);
                }
                else
                {
                    SortColorGrouping!.Add(index, similarNumber);
                    index = pxLenght[i];
                    similarNumber = [];
                    similarNumber.Add(index);
                }

                if (i == pxLenght.Length - 1)
                    SortColorGrouping!.Add(index, similarNumber);
            }

        }
    }
}
