namespace TresStresHold.Command
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Newtonsoft.Json;


    internal class RMIC : ICommand
    {
        private ColorInformation? _colorInformation;
        private readonly string _command;
        private string? _value;
        private string? _path;

        internal RMIC(string command)
        {
            _command = command;
        }

        public void Go()
        {
            // check available ColorInformation
            if (_colorInformation is null || _colorInformation.DefaultColor is null || _colorInformation.GrayScaleColor is null)
                return;

            // parse value from string to int
            string[] stringValue = _value!.Replace("[", "").Replace("]", "").Split(' ');
            int[] intValue = new int[stringValue.Length];

            for (int i = 0; i < stringValue.Length; i++)
            {
                if (!int.TryParse(stringValue[i], out intValue[i]))
                {
                    Console.WriteLine($"something wrong when converting {stringValue[i]}");
                    return;
                }
            }

            int heightIMG = _colorInformation.GrayScaleColor.Length;
            int widthIMG = _colorInformation.GrayScaleColor[0].Length;
            using Bitmap bmp = new(widthIMG, heightIMG);

            for (int i = 0; i < heightIMG; i++)
            {
                for (int j = 0; j < widthIMG; j++)
                {
                    foreach (int item in intValue)
                    {
                        if (_colorInformation.GrayScaleColor[i][j] == item)
                        {
                            Color color = Color.FromArgb(255, 255, 255);
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

            string savePath = _colorInformation.Path!.Split('.')[0] + "_RMIC.png";
            bmp.Save(savePath, ImageFormat.Png);
            Console.WriteLine("\nsucces save");
        }




        public bool IsFalseCommandCheck() // <command...> -c <[GrayScaleColor1 2 N...]> <PATH::json_file_address...>
        {
            // variable result of dividing command spell and value
            string[] commandValueSplited = _command.Split("-c");

            // check the command instruction
            if (!commandValueSplited[0].Contains("RMIC", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("error: spell command is invalid");
                return true;
            }

            // check the rules of path address
            if (!commandValueSplited[1].Contains("PATH::"))
            {
                Console.WriteLine("error: incorrect address writing rules");
                return true;
            }

            // variable result of dividing value and path
            string[] valuePathSplited = commandValueSplited[1].Split("PATH::");

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

            // check the rules of value sytle
            if (!(value.StartsWith('[') && value.EndsWith(']')))
            {
                Console.WriteLine("error: incorrect value writing rules");
                return true;
            }

            // check invalid value
            if (!value.Replace("[", "").Replace("]", "").All<char>(c => "1234567890 ".Contains(c)))
            {
                Console.WriteLine("error: values ​​are just numbers and separated by whitespace");
                return true;
            }

            // final path
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

            _path = path;
            _value = value;

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
