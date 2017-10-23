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

namespace WifiVideo
{
    public partial class Form1 : Form
    {
        private int count = 0;
        public Form1()
        {
            InitializeComponent();
        }
        //声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);
        static string FileName = Application.StartupPath + "\\Config.ini";

        public string ReadIni(string Section, string Ident, string Default)
        {
            Byte[] Buffer = new Byte[65535];
            int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);
            string s = Encoding.GetEncoding(0).GetString(Buffer);
            s = s.Substring(0, bufLen);
            return s.Trim();
        }

        string CameraIp = "http://192.168.8.1:8083/?action=snapshot";
        string ControlIp = "192.168.8.1";
        string Port = "2001";
        string CMD_Forward = "", CMD_Backward = "", CMD_TurnLeft = "", CMD_TurnRight = "", CMD_TurnLeft1 = "", CMD_TurnRight1 = "", CMD_TurnLeft2 = "", CMD_TurnRight2 = "", CMD_Stop = "", CMD_EngineUp = "", CMD_EngineDown = "", CMD_Engineleft = "", CMD_Engineright = "", CMD_ledon = "", CMD_ledoff = "";
        Bitmap[] images = new Bitmap[6];
        char[][] result = new char[6][];

        private void button1_Click(object sender, EventArgs e)
        {
            images[0] = new Bitmap("Blue.jpg");
            images[1] = new Bitmap("Red.jpg");
            images[2] = new Bitmap("Green.jpg");
            images[3] = new Bitmap("Orange.jpg");
            images[4] = new Bitmap("Yellow.jpg");
            images[5] = new Bitmap("White.jpg");
            ColorRecognition cr = new ColorRecognition(images);
            images[0] = new Bitmap("test.jpg");
            images[1] = new Bitmap("test.jpg");
            images[2] = new Bitmap("test.jpg");
            images[3] = new Bitmap("test.jpg");
            images[4] = new Bitmap("test.jpg");
            images[5] = new Bitmap("test.jpg");
            cr.GetColors(ref result, images);
            timer1.Enabled = true;
            timer1.Interval = 3000;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(count<6)
            {
                pictureBox1.ImageLocation = CameraIp;
                Bitmap image = new Bitmap(pictureBox1.Image);
                image.Save("face"+count.ToString()+".jpg");
                count++;
            }
            else
            {
                //count = 0;
                timer1.Enabled = false;
            }
           
        }

        void SendData(string data)
        {
            try
            {
                IPAddress ips = IPAddress.Parse(ControlIp.ToString());
                IPEndPoint ipe = new IPEndPoint(ips, Convert.ToInt32(Port.ToString()));//把ip和端口转化为IPEndPoint实例
                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket

                c.Connect(ipe);//连接到服务器

                byte[] bs = Encoding.ASCII.GetBytes(data);
                c.Send(bs, bs.Length, 0);
                c.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            GetIni();
        }
        private void GetIni()
        {
            CameraIp = ReadIni("VideoUrl", "videoUrl", "");
            ControlIp = ReadIni("ControlUrl", "controlUrl", "");
            Port = ReadIni("ControlPort", "controlPort", "");
            CMD_Forward = ReadIni("ControlCommand", "CMD_Forward", "");
            CMD_Backward = ReadIni("ControlCommand", "CMD_Backward", "");
            CMD_TurnLeft = ReadIni("ControlCommand", "CMD_TurnLeft", "");
            CMD_TurnRight = ReadIni("ControlCommand", "CMD_TurnRight", "");
            CMD_TurnLeft1 = ReadIni("ControlCommand", "CMD_TurnLeft1", "");
            CMD_TurnRight1 = ReadIni("ControlCommand", "CMD_TurnRight1", "");
            CMD_TurnLeft2 = ReadIni("ControlCommand", "CMD_TurnLeft2", "");
            CMD_TurnRight2 = ReadIni("ControlCommand", "CMD_TurnRight2", "");
            CMD_Stop = ReadIni("ControlCommand", "CMD_Stop", "");
            CMD_EngineUp = ReadIni("ControlCommand", "CMD_EngineUp", "");
            CMD_EngineDown = ReadIni("ControlCommand", "CMD_EngineDown", "");
            CMD_Engineleft = ReadIni("ControlCommand", "CMD_Engineleft", "");
            CMD_Engineright = ReadIni("ControlCommand", "CMD_Engineright", "");
            CMD_ledon = ReadIni("ControlCommand", "CMD_ledon", "");
            CMD_ledoff = ReadIni("ControlCommand", "CMD_ledoff", "");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
