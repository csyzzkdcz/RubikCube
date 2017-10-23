using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RubiksCube
{
    class ColorRecognition
    {
        private int[] x = new int[9] { 243, 332, 421, 243, 337, 424, 245, 338, 427};
        private int[] y = new int[9] { 55, 57, 55, 145, 146, 144, 236, 237, 234};
        private int x_center_ = 327, y_center_ = 137;
        private int[][] base_color_ = new int[6][];

        public ColorRecognition(Bitmap[] faces)
        {
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
            bgr = image.GetPixel(x_center_, y_center_);
            B = bgr.B;
            G = bgr.G;
            R = bgr.R;
      
            base_color[index] = new int[3] {B, G, R};
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
            char result_ = 'O';
            double temp_distance_;
            double distance_ = Distance(color, base_color_[0]);
            temp_distance_ = Distance(color, base_color_[1]);
            if (temp_distance_ > distance_)
            {
                result_ = 'W';
                distance_ = temp_distance_;
            }
            temp_distance_ = Distance(color, base_color_[2]);
            if (temp_distance_ > distance_)
            {
                result_ = 'R';
                distance_ = temp_distance_;
            }
            temp_distance_ = Distance(color, base_color_[3]);
            if (temp_distance_ > distance_)
            {
                result_ = 'Y';
                distance_ = temp_distance_;
            }
            temp_distance_ = Distance(color, base_color_[4]);
            if (temp_distance_ > distance_)
            {
                result_ = 'B';
                distance_ = temp_distance_;
            }
            temp_distance_ = Distance(color, base_color_[5]);
            if (temp_distance_ > distance_)
            {
                result_ = 'G';
                distance_ = temp_distance_;
            }
            return result_;
        }
        private char GetColorOfOnePoint(int x, int y, Bitmap image)
        {
            int [] color_ = new int[3] { 0, 0, 0 };

            for(int i = -2; i < 3; i++)
            {
                for(int j = -2; j<3; j++)
                {
                    Color bgr = image.GetPixel(x+i, y+j);
                    color_[0] += bgr.B;
                    color_[1] += bgr.G;
                    color_[2] += bgr.R;
                }
            }
            color_[0] /= 9;
            color_[1] /= 9;
            color_[2] /= 9;
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
                        //char c = GetColorOfOnePoint(start_x_ + base_length_ * j, start_y_ + base_length_ * k, faces[i]);
                        char c = GetColorOfOnePoint(x[k*3+j], y[k*3+j], faces[i]);
                        color[i][k * 3 + j] = c;
                    }
                }
            }
        }
    }
}
