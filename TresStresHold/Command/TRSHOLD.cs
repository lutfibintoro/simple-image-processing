
namespace TresStresHold.Command
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;


    internal class TRSHOLD : ICommand
    {
        private ColorInformation? _colorInformation;
        private readonly string _command;
        private string? _value;
        private string? _path;

        internal TRSHOLD(string command)
        {
            _command = command;
        }

        public void Go()
        {
            if (_colorInformation is null || _colorInformation.DefaultColor is null || _colorInformation.GrayScaleColor is null)
                return;

            int threshold = int.Parse(_value![1..]);
            char barrier = _value![0];


            int height = _colorInformation.GrayScaleColor.Length;
            int width = _colorInformation.GrayScaleColor[0].Length;
            using Bitmap bmp = new(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (barrier == '+')
                    {
                        if (_colorInformation.GrayScaleColor[i][j] > threshold)
                        {
                            Color color = Color.FromArgb(255, 255, 255);
                            bmp.SetPixel(j, i, color);
                        }
                        else
                        {
                            Color color = Color.FromArgb(0, 0, 0);
                            bmp.SetPixel(j, i, color);
                        }
                    }
                    else if (barrier == '-')
                    {
                        if (_colorInformation.GrayScaleColor[i][j] > threshold)
                        {
                            Color color = Color.FromArgb(0, 0, 0);
                            bmp.SetPixel(j, i, color);
                        }
                        else
                        {
                            Color color = Color.FromArgb(255, 255, 255);
                            bmp.SetPixel(j, i, color);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ahh");
                        return;
                    }
                }
            }

            string savePath = _colorInformation.Path!.Split('.')[0] + "_TRSHOLD.png";
            bmp.Save(savePath, ImageFormat.Png);
            Console.WriteLine("\nsucces save");
        }

        public bool IsFalseCommandCheck() // <command...> -c <{+/-}threshold...>\t\t <PATH::json_file_address...>
        {
            // variable result of dividing command spell and value
            string[] commandValueSplited = _command.Split(" -c ");


            if (!commandValueSplited[0].Contains("TRSHOLD", StringComparison.CurrentCultureIgnoreCase))
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

            if (!value.All<char>(c => "-+1234567890".Contains(c)))
            {
                Console.WriteLine("eror: there are characters not allowed in the value");
                return true;
            }

            if (!value[1..].All<char>(c => "1234567890".Contains(c)))
            {
                Console.WriteLine("eror: there are characters not allowed in the value");
                return true;
            }

            if (!(value.StartsWith('+') || value.StartsWith('-')))
            {
                Console.WriteLine("error: incorrect value writing rules");
                return true;
            }


            if (!int.TryParse(value[1..], out int intValue))
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

        public ICommand SetColorInformation()
        {
            string json = File.ReadAllText(_path!);
            _colorInformation = JsonConvert.DeserializeObject<ColorInformation>(json);
            return this;
        }
    }
}
