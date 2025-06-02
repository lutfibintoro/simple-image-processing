
namespace TresStresHold
{
    using System.Threading;
    using Command;


    internal class Program
    {
        internal static void Main(string[] _)
        {
            while (true)
            {
                Console.Write("enter command: ");
                string? fullCommand = Console.ReadLine();

                if (string.IsNullOrEmpty(fullCommand) || string.IsNullOrWhiteSpace(fullCommand))
                    continue;
                if (fullCommand.Equals("n") || fullCommand.Equals("N") || fullCommand.ToLower().Equals("exit"))
                    break;
                if (fullCommand.ToLower().Equals("help"))
                {
                    Help();
                    continue;
                }

                if (File.Exists(fullCommand))
                {
                    Console.WriteLine("exist");
                }

                Execute(fullCommand);
                Thread.Sleep(150);
            }
        }

        internal static void Help()
        {
            Console.WriteLine("\nRMIC\t <command...> -c <[GrayScale1 2 N...]>\t\t <PATH::json_file_address...>\t-hapus warna grayscale tertentu pada gambar");
            Console.WriteLine("RPLACE\t <command...> -c <[GrayScale1:{R,G,B} 2 N...]>\t <PATH::json_file_address...>\t-ubah warna tertentu menjadi warna tertentu");
            Console.WriteLine("MASK\t <command...> -c <<+/->threshold<t/g>...>\t <PATH::json_file_address...>\t-menentukan edge dan gradasi pada gambar");
            Console.WriteLine("TRSHOLD\t <command...> -c <<+/->threshold...>\t\t <PATH::json_file_address...>\t-threasholding sebuah gambar\n");
        }

        private static void Execute(string fullCommand)
        {
            ICommand? command = default;
            if (!fullCommand.Contains("-c"))
            {
                Console.WriteLine("eror: wrong command, type help for information");
                return;
            }
            string spell = fullCommand.Split(" -c")[0];


            if (!(spell.ToUpper().Equals("RMIC") || spell.ToUpper().Equals("RPLACE") || spell.ToUpper().Equals("TRSHOLD") || spell.ToUpper().Equals("MASK")))
            {
                Console.WriteLine("error: wrong spell, type help for information");
                return;
            }


            if (spell.ToUpper().Equals("RMIC"))
                command = new RMIC(fullCommand);
            else if (spell.ToUpper().Equals("RPLACE"))
                command = new RPLACE(fullCommand);
            else if (spell.ToUpper().Equals("TRSHOLD"))
                command = new TRSHOLD(fullCommand);
            else if (spell.ToUpper().Equals("MASK"))
                command = new MASK(fullCommand);
            else
                return;


            Thread thread = new(() =>
            {
                if (command.IsFalseCommandCheck())
                    return;
                command.SetColorInformation()!.Go();
            });
            thread.Start();
        }
    }
}
