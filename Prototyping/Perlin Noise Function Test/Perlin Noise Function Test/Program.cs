using System;
using System.Numerics;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace PerlinNoiseFunctionTest // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PerlinNoise1D Perlin = new PerlinNoise1D(-1.3f,10, 1024);

            for (int i = 0; i < 1024; i++)
            {
                Console.WriteLine(Perlin.Get(i));
            }
        }



    }

    public class PerlinNoise1D
    {
        float Factor1;
        
        int Sample;
        int overallLength;

        public PerlinNoise1D(float seed1, int sampleSize, int length)
        {
            Factor1 = seed1;
            
            Sample = sampleSize;
            overallLength = length;
                        
        }

        public float Get(int x)
        {
            float position = ((float)x / overallLength) * Sample;
            //Console.WriteLine(position);
            return Math.Abs((float)(0.2f * (-3.2f * Math.Sin(Factor1 * position) - 0.2f * Math.Sin(-1.6f * Math.Pow(Math.E, position)) + 1.9f * Math.Sin(0.4f * Math.PI * position))));
        }
    }


       




}