using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Lab_1_Zrenie
{
    public partial class Form1 : Form
    {
        Random random = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox1.Text = File.ReadAllText(openFileDialog1.FileName);

                    dataGridView1.Rows.Clear();

                    string[] lines = textBox1.Lines;
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] parts = line.Split(new char[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 3)
                        {
                            dataGridView1.Rows.Add(parts[0], parts[1], parts[2]);
                        }
                    }

                    MessageBox.Show($"Загружено строк: {dataGridView1.Rows.Count}", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        string generator()
        {
            string x = random.Next(1024).ToString();
            string y = random.Next(1024).ToString();
            string z = random.Next(1024).ToString();

            string stroka = $"{x} {y} {z}";
            return stroka;
        }

        private void btnGenerator_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("data.txt"))
            {
                int numString = int.Parse(txtNumString.Text);

                for (int i = 0; i < numString; i++)
                {
                    string s = generator();
                    sw.WriteLine(s);
                }
            }
            
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            using (var savefile = new SaveFileDialog())
            {
                savefile.FileName = "new_data.csv";

                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] lines = textBox1.Lines;
                        string[] result = new string[lines.Length];

                        for (int i = 0;i < result.Length;i++)
                        {
                            string[] line = lines[i].Split(new char[] {' ', ';'}, StringSplitOptions.RemoveEmptyEntries);
                            result[i] = string.Join(";", line);
                        }
                        File.WriteAllLines(savefile.FileName, result, Encoding.UTF8);
                        MessageBox.Show("Файл сохранён", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) return;

            var bmp = new Bitmap(400, 400);
            using (var gfx = Graphics.FromImage(bmp))
                gfx.Clear(Color.Black);

            const double scale = 400.0 / 1024.0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                try
                {
                    int x = int.Parse(row.Cells[0].Value?.ToString() ?? "0");
                    int y = int.Parse(row.Cells[1].Value?.ToString() ?? "0");
                    int z = int.Parse(row.Cells[2].Value?.ToString() ?? "0");

                    int px = (int)(x * scale);
                    int py = 400 - 1 - (int)(y * scale);
                    px = Math.Max(0, Math.Min(px, 400 - 1));
                    py = Math.Max(0, Math.Min(py, 400 - 1));

                    Color color = GetIronColor(z);

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx<= 1; dx++)
                        {
                            int nx = px + dx;
                            int ny = py + dy;
                            if (nx >= 0 && nx < 400 && ny >= 0 && ny < 400)
                            {
                                bmp.SetPixel(nx, ny, color);
                            }
                        }
                    }
                }

                catch { }
            }

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = bmp;
        }

        private Color GetIronColor(int z)
        {
            double t = z / 1023.0;
            byte r, g, b;

            if (t < 0.25)
            {
                r = (byte)(t * 4 * 128);
                g = 0;
                b = 255;
            }

            else if (t < 0.5)
            {
                r = (byte)(128 + (t - 0.25) * 4 * 127);
                g = 0;
                b = (byte)(255 - (t - 0.25) * 4 * 255);
            }

            else if (t < 0.75)
            {
                r = 255;
                g = (byte)((t - 0.5) * 4 * 255);
                b = 0;
            }

            else
            {
                r = 255;
                g = 255;
                b = (byte)((t - 0.75) * 4 * 255);
            }

            return Color.FromArgb(r, g, b);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int count = dataGridView1.RowCount;
            int period = int.Parse(txtPeriod.Text);
            int strok = (count + period - 1)/period;

            int width = period * 10;
            int hight = strok * 10;


            var bmp = new Bitmap(width, hight);

            using (var gfx = Graphics.FromImage(bmp))
                gfx.Clear(Color.Black);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    int x = int.Parse(dataGridView1.Rows[i].Cells[0].Value?.ToString());
                    Color color = GetIronColor(x);

                    for (int dx = 0; dx < 10; dx++)
                    {
                        for (int dy = 0; dy < 10; dy++)
                        {
                            bmp.SetPixel((i % period)*10 + dx, (i / period)*10 + dy, color);
                        }
                    }
                }
            }

            catch { }

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = bmp;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int count = 0;
            int sumX = 0, sumY = 0, sumXY = 0, sumXX = 0;
            if (dataGridView1.Rows.Count == 0) return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                try
                {
                    int x1 = int.Parse(row.Cells[0].Value?.ToString());
                    int y1 = int.Parse(row.Cells[1].Value?.ToString());
                    sumX += x1;
                    sumY += y1;
                    sumXY += y1 * x1;
                    sumXX += x1 * x1;
                    count++;
                }

                catch { }
            }


            double a = ((count * sumXY) - (sumX * sumY)) / (count * sumXX - (sumX * sumX));
            double b = (sumY - (a * sumX)) / count;
            double avgX = (double)sumX / count;
            double avgY = (double)sumY / count;



            var bmp = new Bitmap(400, 400);
            using (var gfx = Graphics.FromImage(bmp))
                gfx.Clear(Color.Black);

            const double scale = 400.0 / 1024.0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                try
                {
                    int x = int.Parse(row.Cells[0].Value?.ToString() ?? "0");
                    int y = int.Parse(row.Cells[1].Value?.ToString() ?? "0");
                    int z = int.Parse(row.Cells[2].Value?.ToString() ?? "0");

                    int px = (int)(x * scale);
                    int py = 400 - 1 - (int)(y * scale);
                    px = Math.Max(0, Math.Min(px, 400 - 1));
                    py = Math.Max(0, Math.Min(py, 400 - 1));

                    Color color = GetIronColor(z);

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = px + dx;
                            int ny = py + dy;
                            if (nx >= 0 && nx < 400 && ny >= 0 && ny < 400)
                            {
                                bmp.SetPixel(nx, ny, color);
                            }
                        }
                    }
                }

                catch { }
            }

            using (var gfx = Graphics.FromImage(bmp))
            {
                //Линия регрессии
                double y0 = a * 0 + b;
                double y1023 = a * 1023 + b;
                //преобразование данных из [0; 1023] в [0; 399]
                int x1 = 0;
                int y1 = 399 - (int)(y0 * 400.0 / 1024.0);
                int x2 = 399;
                int y2 = 399 - (int)(y1023 * 400.0 / 1024.0);

                gfx.DrawLine(new Pen(Color.Red, 2), x1, y1, x2, y2);


                //Средняя X
                int xPixel = (int)(avgX * 400.0 / 1024.0);
                gfx.DrawLine(Pens.Blue, xPixel, 0, xPixel, 399);


                //Средняя Y
                int yPixel = (int)(399 - (avgY * 400.0 / 1024.0));
                gfx.DrawLine(Pens.Green, 0, yPixel, 399, yPixel);
            }

            pictureBox1.Image = bmp;
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
    }
}
