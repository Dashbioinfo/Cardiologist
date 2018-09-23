using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardiologistV2.Matlab.DomainModels
{
    public static class Utils
    {
        public static int index;
        public static Dictionary<double, double> tuplePoints;
        public static double[] NormalizeVector(double[] Input)
        {
            double[] tmp = new double[Input.Length];
            double  max = Input.Max();
            for (int i = 0; i < Input.Length; i++)
            {
                tmp[i] = Input[i] / max;
            }
            return tmp;
        }
        public static double[] SquareVector(double[] Input)
        {
            double[] tmp = new double[Input.Length];
            for (int i = 0; i < Input.Length; i++)
            {
                tmp[i] = Math.Pow(Input[i], 2);
            }
            return tmp;
        }
        public static double[] DifferentiateVector(double[] Input)
        {
            double[] diffvector = new double[Input.Length];
            // double[] diffvector; //= new double[Input.Length];
           // List<double> newList = new List<double>();
            for (int i = 0; i < Input.Length-1; i++)
            {
                    //0 = 1 -0       //0 = 0- -1
                                     //1 = 
                    //n = n+1 - n    //n =  n-1
                    //diffvector[i] = Input[i + 1] - Input[i];\\
                  //  newList.Add(Input[i + 1] - Input(i));
              //  diffvector[0] = Input[1] - Input[0];
                diffvector[i] = Input[i+1] - Input[i];
                
            }
         //   diffvector = newList.ToArray();
            return diffvector;

        }

        public static double Max(double[] Input, int Start, int Finish)
        {
            double max = Input[Start];
             index = Start;
            for(int i=Start;i<=Finish;i++)
            {
                if(Input[i]>max)
                {
                    max = Input[i];
                    index = i;
                }
            }
            return max;
        }
        public static double Sum(double[] Input, int Start, int Finish)
        {
            double total = 0.0;

            for (int i = Start; i <= Finish; i++)
            {
                total += Input[i];
            }
            return total;
        }
        public static double Mean(double[] Input)
        {
            return Input.Sum() / Input.Count();            

        }
        public static double Index2Time(int index, int fs)
        {
            return (1 + index) / fs;
        }

        public static double Minimalist(List<double> minValues)
        {
           double min =  minValues.Max();
            foreach(var num in minValues)
            {
                if(min>num)
                {
                    min = num;
                }
            }
            return min;
        }
        public static double Maximalist(List<double> maxValues)
        {
            double max = maxValues.Max();
            foreach(var num in maxValues)
            {
                if(max<num)
                {
                    max = num;
                }
            }
            return max;
        }
    }
}
