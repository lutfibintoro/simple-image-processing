

namespace ImageQuantizationData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ScottPlot;
    using ScottPlot.Plottables;
    using ScottPlot.WinForms;






    internal class Plots : Form
    {

        internal Plots(string formName, Images images)
        {

            this.Name = formName;
            this.Text = formName;
            this.ClientSize = new Size(600, 400);
            this.AutoScaleDimensions = new SizeF(9F, 20F);

            Panel panel = new()
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(panel);


            double[] y = new double[256];

            foreach (KeyValuePair<int, List<int>> item in images.SortColorGrouping)
            {
                y[item.Key] = item.Value.Count;
            }


            FormsPlot formsPlot = HistogramPlot(y);
            panel.Controls.Add(formsPlot);
            formsPlot.Refresh();
        }



        private static FormsPlot HistogramPlot(double[] y)
        {
            FormsPlot formsPlot = new()
            {
                Dock = DockStyle.Fill
            };

            LollipopPlot lolipop = formsPlot.Plot.Add.Lollipop(y);
            lolipop.MarkerSize = 0;

            int turn = 1;
            for (int x = 0; x < y.Length; x++)
            {
                if (turn == 1)
                {
                    string label = $"{y[x]}";
                    Text text = formsPlot.Plot.Add.Text(label, x, -5);
                    text.LabelFontSize = 10;
                    text.LabelBold = true;
                    turn = 2;
                }
                else if (turn == 2)
                {
                    string label = $"\n{y[x]}";
                    Text text = formsPlot.Plot.Add.Text(label, x, -5);
                    text.LabelFontSize = 10;
                    text.LabelBold = true;
                    turn = 3;
                }
                else
                {
                    string label = $"\n\n{y[x]}";
                    Text text = formsPlot.Plot.Add.Text(label, x, -5);
                    text.LabelFontSize = 10;
                    text.LabelBold = true;
                    turn = 1;
                }
            }

            formsPlot.Plot.Add.Signal(y);

            return formsPlot;
        }
    }
}

