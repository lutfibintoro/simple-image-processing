
namespace TresStresHold.Command
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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

        }

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
            Console.WriteLine(value);
            _path = path;

            return false;
        }

        public ICommand? SetColorInformation()
        {
            return this;
        }
    }
}
