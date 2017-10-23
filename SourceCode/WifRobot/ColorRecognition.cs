using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WifiVideo
{
    class ColorRecognition
    {
        private int start_x_;
        private int start_y_;
        private int base_length_;
        private int[][] base_color_ = new int[6][];

        public ColorRecognition(Bitmap[] faces)
        {
            start_x_ = 35;
            start_y_ = 35;
            base_length_ = 70;

            for (int i = 0; i < 6; i++)
            {
                GenBaseBGR(faces[i], i, base_color_);
            }
        }

        private void GenBaseBGR(Bitmap image, int index, int[][] base_color)
        {
            int []bgr_color_ = new int[3];
            int B, G, R;

            Color bgr;
            B = G = R = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    bgr = image.GetPixel(start_x_ + base_length_ * j, start_y_ + base_length_ * i);
                    B += bgr.B;
                    G += bgr.G;
                    R += bgr.R;
                }
            }
            base_color[index] = new int[3] { B / 9, G / 9, R / 9 };
        }
        private double Distance(int[] color1, int[] color2)
        {
            double result_ = 0;
            for (int i = 0; i < 3; i++)
            {
                result_ += color1[i] * color2[i];
            }
            result_ /= Math.Sqrt(color1[0] * color1[0] + color1[1] * color1[1] + color1[2] * color1[2]);
            result_ /= Math.Sqrt(color2[0] * color2[0] + color2[1] * color2[1] + color2[2] * color2[2]);
            return result_;
        }
        private char JudgeColor(int[] color)
        {
            char result_ = 'B';
            double temp_distance_;
            double distance_ = Distance(color, base_color_[0]);
            temp_distance_ = Distance(color, base_color_[1]);
            if (temp_distance_ > distance_)
            {
                result_ = 'R';
                distance_ = temp_distance_;
            }
            temp_distance_ = Distance(color, base_color_[2]);
            if (temp_distance_ > distance_)
            {
                result_ = 'G';
                distance_ = temp_distance_;
            }
            temp_distance_ = Distance(color, base_color_[3]);
            if (temp_distance_ > distance_)
            {
                result_ = 'O';
                distance_ = temp_distance_;
            }
            temp_distance_ = Distance(color, base_color_[4]);
            if (temp_distance_ > distance_)
            {
                result_ = 'Y';
                distance_ = temp_distance_;
            }
            temp_distance_ = Distance(color, base_color_[5]);
            if (temp_distance_ > distance_)
            {
                result_ = 'W';
                distance_ = temp_distance_;
            }
            return result_;
        }
        private char GetColorOfOnePoint(int x, int y, Bitmap image)
        {
            int [] color_ = new int[3];
            Color bgr = image.GetPixel(x, y);
            color_[0] = bgr.B;
            color_[1] = bgr.G;
            color_[2] = bgr.R;

            return JudgeColor(color_);
        }
        public void GetColors(ref char[][] color, Bitmap[] faces)
        {
            for (int i = 0; i < 6; i++)
            {
                color[i] = new char[9];
                for (int k = 0; k < 3; k++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        char c = GetColorOfOnePoint(start_x_ + base_length_ * j, start_y_ + base_length_ * k, faces[i]);
                        color[i][k * 3 + j] = c;
                    }
                }
            }
        }
}
}
