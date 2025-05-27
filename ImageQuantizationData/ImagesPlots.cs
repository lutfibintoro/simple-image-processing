namespace ImageQuantizationData
{
    using System;
    using System.Windows.Forms;


    internal class ImagesPlots : Form
    {
        private int _cellWidth = 26;
        private int _cellHeight = 26;
        private readonly Bitmap _bitmap;
        private readonly Images _images;
        private DataGridView _dataGridView = new()
        {
            ColumnHeadersVisible = false,
            RowHeadersVisible = false,
            AllowDrop = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AllowUserToResizeColumns = false,
            AllowUserToOrderColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
            Dock = DockStyle.Fill,
            RowCount = 15,
            ColumnCount = 15
        };
        private readonly Panel _panel = new()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };
        // field or property




        public ImagesPlots(string formName, Bitmap bitmap, Images images)
        {
            _bitmap = bitmap;
            _images = images;

            this.Name = formName;
            this.Text = formName;


            _panel.Controls.Add(_dataGridView);
            this.Controls.Add(_panel);
            RenderImage();

            this.MouseWheel += Gulir_Mouse;
        }


#pragma warning disable CS8618
        public ImagesPlots(string formName, int[][] colorValue)
        {
#pragma warning restore

            this.Name = formName;
            this.Text = formName;

            this.Height = 434;
            this.Width = 411;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;


            _panel.Controls.Add(_dataGridView);
            this.Controls.Add(_panel);


            foreach (DataGridViewRow row in _dataGridView.Rows)
                row.Height = _cellHeight;
            foreach (DataGridViewColumn column in _dataGridView.Columns)
                column.Width = _cellWidth;
            RenderImage(colorValue);
        }


        public void RenderImage(int[][] colorValue)
        {
            for (int i = 0; i < _dataGridView.RowCount; i++)
            {
                for (int j = 0; j < _dataGridView.ColumnCount; j++)
                {
                    _dataGridView.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(255, 255, 255);
                    _dataGridView.Rows[i].Cells[j].Value = "";
                }
            }

            //_dataGridView.ColumnCount = colorValue[0].Length;
            //_dataGridView.RowCount = colorValue.Length;

            for (int i = 0; i < colorValue.Length; i++)
            {
                _dataGridView.Rows[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                for (int j = 0; j < colorValue[i].Length; j++)
                {
                    int color = colorValue[i][j];
                    _dataGridView.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(color, color, color);
                    if (color <= 127)
                        _dataGridView.Rows[i].Cells[j].Style.ForeColor = Color.White;
                    else
                        _dataGridView.Rows[i].Cells[j].Style.ForeColor = Color.Black;

                    _dataGridView.Rows[i].Cells[j].Value = "";
                }
            }


            if ((string)_dataGridView.Rows[0].Cells[0].Value == string.Empty)
                AddText(colorValue);
        }


        private void AddText(int[][] colorValue)
        {
            for (int i = 0; i < colorValue.Length; i++)
            {
                for (int j = 0; j < colorValue[i].Length; j++)
                {
                    int color = colorValue[i][j];
                    _dataGridView.Rows[i].Cells[j].Value = $"{color}";
                }
            }
        }



        private void RenderImage()
        {
            _dataGridView.ColumnCount = _images.Width;
            _dataGridView.RowCount = _images.Height;

            for (int i = 0; i < _images.Height; i++)
            {
                _dataGridView.Rows[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                for (int j = 0; j < _images.Width; j++)
                {
                    int color = _images.ColorValueRGB![i, j];
                    _dataGridView.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(color, color, color);
                    _dataGridView.Rows[i].Cells[j].Style.ForeColor = Color.DarkGreen;
                    _dataGridView.Rows[i].Cells[j].Value = "";
                }
            }

            foreach (DataGridViewRow row in _dataGridView.Rows)
                row.Height = _cellHeight;
            foreach (DataGridViewColumn column in _dataGridView.Columns)
                column.Width = _cellWidth;

            AddText();
        }






        private void AddText()
        {
            for (int i = 0; i < _images.Height; i++)
            {
                for (int j = 0; j < _images.Width; j++)
                {
                    int color = _images.ColorValueRGB![i, j];
                    _dataGridView.Rows[i].Cells[j].Value = $"{color}";
                }
            }
        }



        private void RemoveText()
        {
            for (int i = 0; i < _images.Height; i++)
            {
                for (int j = 0; j < _images.Width; j++)
                {
                    _dataGridView.Rows[i].Cells[j].Value = "";
                }
            }
        }



        private void Gulir_Mouse(object? sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if ((_cellHeight <= 25 && _cellWidth <= 25) && (string)_dataGridView.Rows[0].Cells[0].Value != string.Empty)
                    RemoveText();
                if ((_cellHeight > 25 && _cellWidth > 25) && (string)_dataGridView.Rows[0].Cells[0].Value == string.Empty)
                    AddText();


                if (e.Delta > 0)
                {
                    _cellHeight += 1;
                    _cellWidth += 1;

                    foreach (DataGridViewRow row in _dataGridView.Rows)
                        row.Height = _cellHeight;
                    foreach (DataGridViewColumn column in _dataGridView.Columns)
                        column.Width = _cellWidth;
                }
                else if (e.Delta < 0)
                {
                    _cellHeight -= 1;
                    _cellWidth -= 1;

                    foreach (DataGridViewColumn column in _dataGridView.Columns)
                        column.Width = _cellWidth;
                    foreach (DataGridViewRow row in _dataGridView.Rows)
                        row.Height = _cellHeight;
                }
            }
        }
    }
}