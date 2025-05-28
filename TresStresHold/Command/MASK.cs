
namespace TresStresHold.Command
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal class MASK : ICommand
    {
        private ColorInformation? _colorInformation;
        private readonly string _command;
        private string? _value;
        private string? _path;

        internal MASK(string command)
        {
            _command = command;
        }

        public void Go()
        {
            if (_colorInformation is null || _colorInformation.DefaultColor is null || _colorInformation.GrayScaleColor is null)
                return;

            int threshold = int.Parse(_value![1..^1]);
            char barrier = _value![0];
            char edgeType = _value![^1];

            int height = _colorInformation.GrayScaleColor.Length;
            int width = _colorInformation.GrayScaleColor[0].Length;

            using Bitmap bmpMASK = new(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bmpMASK.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }

            for (int y = 1; y < (height - 1); y++)
            {
                for (int x = 1; x < (width - 1); x++)
                {
                    int[,] yx = new int[3, 3];
                    int[][] sobelSV = [[-1, -2, -1], [0, 0, 0], [1, 2, 1]];
                    int[][] sobelSH = [[-1, 0, 1], [-2, 0, 2], [-1, 0, 1]];
                    int sv = 0;
                    int sh = 0;

                    for (int i = 0; i < yx.GetLength(0); i++)
                    {
                        for (int j = 0; j < yx.GetLength(1); j++)
                        {
                            yx[i, j] = _colorInformation.GrayScaleColor[y - 1 + i][x - 1 + j];
                            sv += yx[i, j] * sobelSV[i][j];
                            sh += yx[i, j] * sobelSH[i][j];
                        }
                    }

                    if (edgeType == 't')
                    {
                        Treshold(barrier, threshold, bmpMASK, y, x, sv, sh);
                    }
                    else if (edgeType == 'g')
                    {
                        Gradient(sv, sh, bmpMASK, x, y);
                    }
                    else
                    {
                        Console.WriteLine("ohh");
                        return;
                    }
                }
            }

            string savePath = _colorInformation.Path!.Split('.')[0] + "_MASK.png";
            bmpMASK.Save(savePath, ImageFormat.Png);
            Console.WriteLine("\nsucces save");
        }

        private static Bitmap Gradient(int sv, int sh, Bitmap bmpMASK, int x, int y)
        {
            double gradient = Sqrt(Pow(sv, 2) + Pow(sh, 2));
            int c = Math.Min(Floor(gradient), 255);
            bmpMASK.SetPixel(x, y, Color.FromArgb(c, c, c));

            return bmpMASK;
        }

        private Bitmap Treshold(char barrier, int threshold, Bitmap bmp, int y, int x, int sv, int sh)
        {
            double gradient = Sqrt(Pow(sv, 2) + Pow(sh, 2));

            if (_colorInformation is null || _colorInformation.DefaultColor is null || _colorInformation.GrayScaleColor is null)
                return bmp;

            if (barrier == '+')
            {
                if (gradient > threshold)
                {
                    Color color = Color.FromArgb(255, 255, 255);
                    bmp.SetPixel(x, y, color);
                }
                else
                {
                    Color color = Color.FromArgb(0, 0, 0);
                    bmp.SetPixel(x, y, color);
                }
            }
            else if (barrier == '-')
            {
                if (gradient > threshold)
                {
                    Color color = Color.FromArgb(0, 0, 0);
                    bmp.SetPixel(x, y, color);
                }
                else
                {
                    Color color = Color.FromArgb(255, 255, 255);
                    bmp.SetPixel(x, y, color);
                }
            }
            else
            {
                Console.WriteLine("Ahh");
                return bmp;
            }

            return bmp;
        }

        private static double Sqrt(double x) => Math.Sqrt(x);
        private static double Pow(double x, double y) => Math.Pow(x, y);
        private static int Floor(double x) => (int)Math.Floor(x);


        public bool IsFalseCommandCheck()
        {
            // variable result of dividing command spell and value
            string[] commandValueSplited = _command.Split(" -c ");


            if (!commandValueSplited[0].Contains("MASK", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("error: spell command is invalid");
                return true;
            }


            // check the rules of path address
            if (!commandValueSplited[1].Contains(" PATH::"))
            {
                Console.WriteLine("error: incorrect address writing rules");
                return true;
            }


            string[] valuePathSplited = commandValueSplited[1].Split(" PATH::");

            // final value
            string value = valuePathSplited[0];
            while (true)
            {
                if (value[0] == ' ')
                    value = value[1..];
                else
                    break;
            }
            while (true)
            {
                if (value[^1] == ' ')
                    value = value[..^1];
                else
                    break;
            }

            //final path
            string path = valuePathSplited[1].Replace("\"", "").Replace('\\', '/');
            while (true)
            {
                if (path[0] == ' ')
                    path = path[1..];
                else
                    break;
            }
            while (true)
            {
                if (path[^1] == ' ')
                    path = path[..^1];
                else
                    break;
            }


            // check is json or not
            if (!path.EndsWith("json"))
            {
                Console.WriteLine("error: file is json only");
                return true;
            }

            // check file existence
            if (!File.Exists(path))
            {
                Console.WriteLine("error: file isn't exist");
                return true;
            }

            if (!value.All<char>(c => "-+1234567890tg".Contains(c)))
            {
                Console.WriteLine("eror: there are characters not allowed in the value");
                return true;
            }

            if (!value[1..^1].All<char>(c => "1234567890".Contains(c)))
            {
                Console.WriteLine("eror: there are characters not allowed in the value");
                return true;
            }

            if (!(value.StartsWith('+') || value.StartsWith('-')))
            {
                Console.WriteLine("error: incorrect value writing rules");
                return true;
            }

            if (!(value.EndsWith('g') || value.EndsWith('t')))
            {
                Console.WriteLine("error: incorrect value writing rules");
                return true;
            }


            if (!int.TryParse(value[1..^1], out int intValue))
            {
                Console.WriteLine($"error: something wrong when converting {value[1..]}");
                return true;
            }

            if (intValue < 0 || intValue > 255)
            {
                Console.WriteLine("error: threshold value out of limit");
                return true;
            }


            _value = value;
            _path = path;

            return false;
        }

        public ICommand? SetColorInformation()
        {
            string json = File.ReadAllText(_path!);
            _colorInformation = JsonConvert.DeserializeObject<ColorInformation>(json);
            return this;
        }
    }
}
