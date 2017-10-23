using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using MagicCube_Refined;

namespace RubiksCube
{
    public partial class Form1 : Form
    {
        private int[] interval = new int[6] { 7, 21, 36, 51, 61, 69 };
        private Dictionary<string, double> cost_time = new Dictionary<string, double>();
        private double total_cost_time;
        private int count = 0, count2 = 0;
        private bool is_operate = false, is_send_operation = false, is_recover_cube = false;
        private string out_put_;

        public Form1()
        {
            InitCostTime();
            InitializeComponent();
        }

        string CameraIp = "http://192.168.8.1:8083/?action=snapshot";
        string ControlIp = "192.168.8.1";
        string Port = "2001";
        Bitmap[] images = new Bitmap[6];
        char[][] result = new char[6][];

        private void InitCostTime()
        {
            cost_time.Add("F1", 2.75);
            cost_time.Add("L1", 2.75);
            cost_time.Add("B1", 2.75);
            cost_time.Add("R1", 2.95);
            cost_time.Add("F3", 2.75);
            cost_time.Add("L3", 2.75);
            cost_time.Add("B3", 2.75);
            cost_time.Add("R3", 2.95);
            cost_time.Add("F2", 5.20);
            cost_time.Add("L2", 5.20);
            cost_time.Add("B2", 5.20);
            cost_time.Add("R2", 5.20);
            cost_time.Add("U1", 13.75);
            cost_time.Add("D1", 13.75);
            cost_time.Add("U2", 16.50);
            cost_time.Add("D2", 16.50);
            cost_time.Add("U3", 13.75);
            cost_time.Add("D3", 13.75);
            cost_time.Add("11", 6.10);
            cost_time.Add("22", 6.10);
            cost_time.Add("00", 0.00);
        }

        private void CalCostTime()
        {
            total_cost_time = 0;
            for (int i = 1; i < out_put_.Length / 2; i++)
            {
                string str = out_put_.Substring(i * 2, 2);
                total_cost_time += cost_time[str];
            }
            textBox11.Text = ((int)(total_cost_time)).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            is_operate = false;
            timer2.Enabled = true;
            timer2.Interval = 10;
            count = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            is_operate = true;
            timer1.Enabled = true;
            timer1.Interval = 500;
            timer2.Enabled = true;
            timer2.Interval = 10;
            count = count2 = 0;
            SendData("P1");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string operation, final_operation = "";
            int o_count;

            operation = textBox9.Text;

            if (operation.Length > 0 && textBox10.Text.Length > 0)
            {
                if (operation.Length % 2 == 0)
                {
                    for (int i = 0; i < operation.Length / 2; i++)
                    {
                        char c = operation.ElementAt(i * 2);
                        if (i == 0)
                        {
                            if (c == '0' || c == '1' || c == '2')
                            {
                                continue;
                            }
                        }

                        if (c != 'F' && c != 'L' && c != 'B' && c != 'R' && c != 'U' && c != 'D')
                        {
                            return;
                        }
                        c = operation.ElementAt(i * 2 + 1);
                        if (c != '1' && c != '2' && c != '3')
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
                o_count = int.Parse(textBox10.Text);
                for (int i = 0; i < o_count; i++)
                {
                    final_operation += operation;
                }
            }
            else
            {
                return;
            }

            SendData(final_operation);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (is_operate)
            {
                count++;
                if (count == interval[count2] - 1)
                {
                    pictureBox2.ImageLocation = CameraIp;
                }
                if (count2 < 6 && count == interval[count2])
                {
                    Bitmap image = new Bitmap(pictureBox2.Image);
                    image.Save("test" + (count2 + 1).ToString() + ".jpg");
                    count2++;
                }

                if (count2 == 6)
                {
                    pictureBox2.Image = Properties.Resources.Arduino;
                    is_operate = false;
                    for (int i = 1; i < 7; i++)
                    {
                        images[i - 1] = new Bitmap("test" + i.ToString() + ".jpg");
                    }
                    ColorRecognition cr = new ColorRecognition(images);
                    cr.GetColors(ref result, images);

                    DisplayColors();

                    is_operate = false;
                    is_recover_cube = true;
                }
            }
            else if (is_recover_cube)
            {
                is_recover_cube = false;
                try
                {
                    MagicCubeRecover op_ = new MagicCubeRecover();
                    op_.LoadCube(result);
                    out_put_ = "";
                    op_.RecoverCube(ref out_put_);

                    CalCostTime();
                    timer3.Enabled = true;
                    timer3.Interval = 2000;
                    progressBar1.Value = 0;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = (int)(total_cost_time/2);

                    if(out_put_.Length > 2)
                    {
                        textBox1.Text = out_put_;
                        textBox2.Text = (out_put_.Length / 2).ToString();
                    }
                    else
                    {
                        textBox1.Text = "-";
                        textBox2.Text = "0";
                    }
                    
                    is_send_operation = true;

                    timer1.Interval = 10;
                }
                catch (IndexOutOfRangeException)
                {
                    return;
                }

            }
            else if (is_send_operation)
            {
                is_send_operation = false;
                SendData(out_put_);
                timer1.Enabled = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = CameraIp;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value++;
            }
            else
            {
                timer3.Enabled = false;
            }
        }
       
        private void SendData(string data)
        {
            byte[] bs = new byte[16];

            try
            {
                IPAddress ips = IPAddress.Parse(ControlIp.ToString());
                IPEndPoint ipe = new IPEndPoint(ips, Convert.ToInt32(Port.ToString()));
                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                c.Connect(ipe);

                int number = data.Length / 16;
                if (data.Length % 16 != 0)
                    number += 1;

                for (int i = 0; i < number; i++)
                {
                    if (i < number - 1)
                    {
                        string str = data.Substring(i * 16, 16);
                        bs = Encoding.ASCII.GetBytes(str);
                        c.Send(bs, bs.Length, 0);
                        System.Threading.Thread.Sleep(100);
                    }
                    else
                    {
                        string str = data.Substring(i * 16);
                        bs = Encoding.ASCII.GetBytes(str);
                        c.Send(bs, bs.Length, 0);
                    }
                }
                c.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void DisplayColors()
        {
            string display_string_ = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j < 2)
                    {
                        display_string_ += result[0][3 * i + j] + "   ";
                    }
                    else
                    {
                        display_string_ += result[0][3 * i + j];
                    }
                }
                display_string_ += "          ";
            }
            textBox3.Text = display_string_;
            display_string_ = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j < 2)
                    {
                        display_string_ += result[1][3 * i + j] + "   ";
                    }
                    else
                    {
                        display_string_ += result[1][3 * i + j];
                    }
                }
                display_string_ += "          ";
            }
            textBox4.Text = display_string_;
            display_string_ = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j < 2)
                    {
                        display_string_ += result[2][3 * i + j] + "   ";
                    }
                    else
                    {
                        display_string_ += result[2][3 * i + j];
                    }
                }
                display_string_ += "          ";
            }
            textBox5.Text = display_string_;
            display_string_ = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j < 2)
                    {
                        display_string_ += result[3][3 * i + j] + "   ";
                    }
                    else
                    {
                        display_string_ += result[3][3 * i + j];
                    }
                }
                display_string_ += "          ";
            }
            textBox6.Text = display_string_;
            display_string_ = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j < 2)
                    {
                        display_string_ += result[4][3 * i + j] + "   ";
                    }
                    else
                    {
                        display_string_ += result[4][3 * i + j];
                    }
                }
                display_string_ += "          ";
            }
            textBox7.Text = display_string_;
            display_string_ = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j < 2)
                    {
                        display_string_ += result[5][3 * i + j] + "   ";
                    }
                    else
                    {
                        display_string_ += result[5][3 * i + j];
                    }
                }
                display_string_ += "          ";
            }
            textBox8.Text = display_string_;
        }

        private void SendUDFOperation(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.button3_Click(sender, e);
            }
        }
    }
}
