
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

    internal class RPLACE : ICommand
    {
        private ColorInformation? _colorInformation;
        private readonly string _command;
        private string? _value;
        private string? _path;

        internal RPLACE(string command)
        {
            _command = command;
        }

        public void Go()
        {
            if (_colorInformation is null || _colorInformation.DefaultColor is null || _colorInformation.GrayScaleColor is null)
                return;

            string[] roughValue = _value!.Split(' ');
            int[] grayScaleValue = new int[roughValue.Length];
            int[][] rgbValue = new int[roughValue.Length][];

            for (int i = 0; i < roughValue.Length; i++)
            {
                if (!int.TryParse(roughValue[i].Split(':')[0], out grayScaleValue[i]))
                {
                    Console.WriteLine("error: failed parse");
                    return;
                }

                string[] rgbString = roughValue[i].Split(':')[1].Replace("{", "").Replace("}", "").Split(",");
                rgbValue[i] = new int[3];

                for (int j = 0; j < rgbString.Length; j++)
                {
                    if (!int.TryParse(rgbString[j], out rgbValue[i][j]))
                    {
                        Console.WriteLine("error: failed parse");
                        return;
                    }
                }
            }

            int height = _colorInformation.GrayScaleColor.Length;
            int width = _colorInformation.GrayScaleColor[0].Length;
            using Bitmap bmp = new(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < grayScaleValue.Length; k++)
                    {
                        if (_colorInformation.GrayScaleColor[i][j] == grayScaleValue[k])
                        {
                            Color color = Color.FromArgb(rgbValue[k][0], rgbValue[k][1], rgbValue[k][2]);
                            bmp.SetPixel(j, i, color);
                            break;
                        }
                        else
                        {
                            int r = _colorInformation.DefaultColor[i][j].R;
                            int g = _colorInformation.DefaultColor[i][j].G;
                            int b = _colorInformation.DefaultColor[i][j].B;
                            Color color = Color.FromArgb(r, g, b);
                            bmp.SetPixel(j, i, color);
                        }
                    }
                }
            }

            string savePath = _colorInformation.Path!.Split('.')[0] + "_RPLACE.png";
            bmp.Save(savePath, ImageFormat.Png);
            Console.WriteLine("\nsucces save");
        }

        public bool IsFalseCommandCheck() // <command...> -c <[GrayScale1:{R,G,B} 2 N...]> <PATH::json_file_address...>
        {
            // variable result of dividing command spell and value
            string[] commandValueSplited = _command.Split(" -c ");

            // check the command instruction
            if (!commandValueSplited[0].Contains("RPLACE", StringComparison.CurrentCultureIgnoreCase))
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

            // variable result of dividing value and path
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

            // check the rules of value sytle
            if (!(value.StartsWith('[') && value.EndsWith(']')))
            {
                Console.WriteLine("error: incorrect value writing rules");
                return true;
            }

            value = value[1..];
            value = value[..^1];

            if (!value.All<char>(c => "1234567890 :{},".Contains(c)))
            {
                Console.WriteLine("error: grayscale ​​are just numbers and separated by colon and whitespace");
                return true;
            }

            foreach (string item in value.Split(' '))
            {
                if (!item.All<char>(c => "1234567890:{},".Contains(c)))
                {
                    Console.WriteLine("eror: there are characters not allowed in the value");
                    return true;
                }

                string[] sparatedItem = item.Split(':');
                if (!sparatedItem[0].All<char>(c => "1234567890".Contains(c)))
                {
                    Console.WriteLine("eror: there are characters not allowed in the value");
                    return true;
                }

                if (!(sparatedItem[1].StartsWith('{') && sparatedItem[1].EndsWith('}')))
                {
                    Console.WriteLine("error: incorrect value writing rules");
                    return true;
                }

                sparatedItem[1] = sparatedItem[1][1..];
                sparatedItem[1] = sparatedItem[1][..^1];

                if (!sparatedItem[1].All<char>(c => "1234567890,".Contains(c)))
                {
                    Console.WriteLine("eror: RGB value is number only and separated by comma");
                    return true;
                }

                if (sparatedItem[1].Split(',').Length != 3)
                {
                    Console.WriteLine("eror: RGB value false value");
                    return true;
                }
            }

            _path = path;
            _value = value;

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
