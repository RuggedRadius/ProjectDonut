using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Tools
{
    public static class MathTools
    {
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Clamp(this float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static int Lerp(int a, int b, float t)
        {
            return (int)(a + (b - a) * t);
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        public static int InverseLerp(int a, int b, int value)
        {
            return (value - a) / (b - a);
        }

        public static float InverseLerp(float a, float b, float value)
        {
            return (value - a) / (b - a);
        }

        public static double InverseLerp(double a, double b, double value)
        {
            return (value - a) / (b - a);
        }

        public static int Map(int value, int fromMin, int fromMax, int toMin, int toMax)
        {
            return (value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
        }

        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
        }

        public static double Map(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            return (value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
        }
    }
}
