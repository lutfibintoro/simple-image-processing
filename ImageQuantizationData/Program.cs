namespace ImageQuantizationData
{
    using System;



    internal class Program
    {
        internal static void Main(string[] _)
        {

            while (true)
            {
                Console.Write("path of the image: ");
                string? path = Console.ReadLine();

                if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                    continue;
                if (path.Equals("n") || path.Equals("N") || path.ToLower().Equals("exit"))
                    break;

                Run(path);
                Thread.Sleep(100);
            }

            //foreach (Thread thread in threads)
            //    thread.Join();
        }

        internal static Thread? Run(string path)
        {
            Thread thread = new(() =>
            {
                Application.EnableVisualStyles();
                Plots? plot = New(path);
                if (plot == null)
                {
                    Console.WriteLine("failed...");
                    return;
                }
                Application.Run(plot);
            })
            {
                IsBackground = true
            };
            thread.Start();

            return thread;
        }


        internal static Plots? New(string path)
        {
            Images? images = new Images(path).Process();
            WriteOutput wo = new(images);
            wo.WriteCSV();
            //wo.WriteImageXLSX();
            wo.WriteImageJson();

            if (images is null)
                return null;


            List<double> x = [];
            List<double> y = [];
            foreach (KeyValuePair<int, List<int>> item in images.SortColorGrouping)
            {
                x.Add(item.Key);
                y.Add(item.Value.Count);
            }

            Thread thread = new(() =>
            {
                Application.EnableVisualStyles();
                Picker picker = new(images.ImageName, images.Path);
                Application.Run(picker);
            })
            {
                IsBackground = true
            };
            thread.Start();

            return new Plots(images.ImageName, images);
        }
    }
}
