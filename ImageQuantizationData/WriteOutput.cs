
namespace ImageQuantizationData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ClosedXML.Excel;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Newtonsoft.Json;
    using ScottPlot.Palettes;

    internal class WriteOutput
    {
        private readonly StringBuilder _csvOutput = new("color:;totalPX:;");
        private readonly Images? _images;

        internal WriteOutput(Images? images)
        {
            if (images == null)
                return;

            _images = images;
            _csvOutput.Append($"dimension: {images.Height} x {images.Width}\n");

            int[] countColorPX = new int[256];
            foreach (KeyValuePair<int, List<int>> item in images.SortColorGrouping)
            {
                countColorPX[item.Key] = item.Value.Count;
            }

            for (int i = 0; i < countColorPX.Length; i++)
            {
                _csvOutput.Append($"{i};{countColorPX[i]}\n");
            }
        }

        internal void WriteCSV()
        {
            string path;

            if (_images == null)
                return;

            if (_images.Path.EndsWith(".png"))
                path = _images.Path.Replace(".png", ".csv");
            else if (_images.Path.EndsWith(".jpg"))
                path = _images.Path.Replace(".jpg", ".csv");
            else
                return;

            _csvOutput.Append($"\nMean;{Mean()}\n");
            _csvOutput.Append($"\nVariant;{Variant()}\n");
            _csvOutput.Append($"\nStandartDeviasi;{StandartDeviasi()}\n");
            File.WriteAllText(path, _csvOutput.ToString());
        }

        internal void WriteImageXLSX()
        {
            double width = 3.00d;
            double height = 20.00d;

            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.Worksheets.Add("sheet1");
            worksheet.Rows().Height = height;
            worksheet.Columns().Width = width;

            for (int i = 1; i <= _images?.Height; i++)
            {
                for (int j = 1; j <= _images.Width; j++)
                {
                    int rgbColor = _images.ColorValueRGB![i - 1, j - 1];
                    IXLCell cell = worksheet.Cell(i, j);
                    cell.Value = rgbColor;
                    cell.Style.Font.FontColor = XLColor.DarkGreen;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(rgbColor, rgbColor, rgbColor);
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.Black;
                }
            }

            string filePath = _images?.Path.Split('.')[0] + ".xlsx";
            workbook.SaveAs(filePath);
        }

        internal double Mean()
        {
            if (_images is null)
                return 0;
            if (_images.ColorValueRGB is null)
                return 0;

            int totalNilaiWarnaPX = 0;
            int totalJumlahPX = _images.ColorValueRGB.Length;

            foreach (KeyValuePair<int, List<int>> item in _images!.SortColorGrouping)
            {
                foreach (int nilaWarna in item.Value)
                {
                    totalNilaiWarnaPX += nilaWarna;
                }
            }

            double rataRata = (double)totalNilaiWarnaPX / totalJumlahPX;
            return rataRata;
        }

        internal double Variant()
        {
            if (_images is null)
                return 0;
            if (_images.ColorValueRGB is null)
                return 0;

            double rataRata = Mean();
            double sigmaXiMinMeanKuadrat = default;
            foreach (KeyValuePair<int, List<int>> item in _images!.SortColorGrouping)
            {
                foreach (int nilaWarna in item.Value)
                {
                    sigmaXiMinMeanKuadrat += Math.Pow(nilaWarna - rataRata, 2);
                }
            }
            return (1.0 / _images.ColorValueRGB.Length) * sigmaXiMinMeanKuadrat;
        }

        internal double StandartDeviasi()
        {
            return Math.Sqrt(Variant());
        }

        internal void WriteImageJson()
        {
            ColorInformation colorInformation = new()
            {
                Path = _images!.Path
            };
            Bitmap bitmap = new(colorInformation.Path);
            colorInformation.DefaultColor = new ImageRGB[bitmap.Height][];
            colorInformation.GrayScaleColor = new int[bitmap.Height][];


            for (int i = 0; i < bitmap.Height; i++)
            {
                colorInformation.GrayScaleColor[i] = new int[bitmap.Width];
                colorInformation.DefaultColor[i] = new ImageRGB[bitmap.Width];

                for (int j = 0; j < bitmap.Width; j++)
                {
                    System.Drawing.Color pxColor = bitmap.GetPixel(j, i);
                    colorInformation.DefaultColor[i][j] = new ImageRGB()
                    {
                        R = pxColor.R,
                        G = pxColor.G,
                        B = pxColor.B
                    };

                    colorInformation.GrayScaleColor[i][j] = (int)(0.299 * pxColor.R + 0.587 * pxColor.G + 0.114 * pxColor.B);
                }
            }

            string jsonFile = JsonConvert.SerializeObject(colorInformation, Formatting.Indented);
            string path = colorInformation.Path.Split('.')[0] + ".json";
            File.WriteAllText(path, jsonFile);
        }
    }
}
