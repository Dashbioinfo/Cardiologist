using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Models.DomainModels
{
    public partial class QRSdetection
    {
        public double[] Time;
        public int fs = 370; // M.I.T Standard
        public int w = 30;//20;
        public double[] INT1;
        public double[] INT2;
        public double[] S1;
        public double[] S2;
        public double[] D1;
        public double[] D2;
        public double[] DD1;
        public double[] DD2;
        public double[] Impulse1;
        public double[] Impulse2;
        public int Beg;
        public int width = 2000;

       public QRSdetection(double[][] ecg)
        {
            doMagic(ecg);
        }
       public QRSdetection(double[][] ecg, int fs, int w)
        {
            this.fs = fs;
            this.w = w;

            doMagic(ecg);

        }
       public QRSdetection(double[][] ecg, int fs,int w, int offset)
       {
           this.fs = fs;
           this.w = w;

           doMagic(ecg,offset);
       }
       public QRSdetection(double[][] ecg, int width)
       {
           this.fs = fs;
           this.w = 30;

           doMagic(ecg, width);
       }

        public void doMagic(double[][] ecg)
        {
            this.Time = new double[width];
            this.S1 = new double[width];
            this.S2 = new double[width];
            
            #region Smoothing
            for (int i = 0; i < width; i++)
            {
                this.Time[i] = ecg[i][0] / fs;
                this.S1[i] = ecg[i][1];
                this.S2[i] = ecg[i][2];

            }

            double Max_S1 = S1.AsParallel().Max();
            double Max_S2 = S2.AsParallel().Max();
            #endregion

            #region Normalization  // Data Normalization

            for (int i = 0; i < width; i++)
            {
                S1[i] = S1[i] / Max_S1;
                S2[i] = S2[i] / Max_S2;
            }

            #endregion

            #region Plotting //GDI+ Implementation Goes Here...

            #endregion

            #region Differentiation

            D1 = Utils.DifferentiateVector(S1);
            D2 = Utils.DifferentiateVector(S2);

            #endregion

            #region Squaring
            DD1 = new double[width];
            DD2 = new double[width];

            for (int i = 0; i < D1.Length; i++)
            {
                DD1[i] = Math.Pow(D1[i], 2);
                DD2[i] = Math.Pow(D2[i], 2); 
            }

            #endregion

            #region Integration

            int beg = this.w / 2;
            INT1 = new double[Time.Length];
            INT2 = new double[Time.Length];

            #region DataIntegration
            for (int i = beg; i < DD1.Length - beg; i++)
            {
                INT1[i] = Utils.Sum(DD1, i - beg, i + beg);
                INT1[i] = INT1[i] / this.w;
            }
            #endregion

            #region Data Normalization

            double max1 = INT1.Max();
            double mean1 = Utils.Mean(INT1);

            for (int i = 0; i < INT1.Length; i++)
            {
                INT1[i] = (INT1[i] - mean1) / max1;
            }
            #endregion

            #region DataIntegration


            for (int i = beg; i < DD2.Length - beg; i++)
            {
                INT2[i] = Utils.Sum(DD2, i - beg, i + beg);
                INT2[i] = INT2[i] / this.w;

            }
            #endregion

            #region DataNormalization

            double max2 = INT2.Max();
            double mean2 = Utils.Mean(INT2);

            for (int i = 0; i < INT2.Length; i++)
            {
                INT2[i] = (INT2[i] - mean2) / max2;
            }

            #endregion


            #endregion

            #region Thresholding

            double comparison1 = 0.3 * INT1.Max();
            double comparison2 = 0.3 * INT2.Max();

            Impulse1 = new double[width];
            for (int i = 0; i < INT1.Length; i++)
            {
                if (INT1[i] > comparison1)
                {
                    Impulse1[i] = 1;
                }
            }

            Impulse2 = new double[width];
            for (int i = 0; i < INT2.Length; i++)
            {
                if (INT2[i] > comparison2)
                {
                    Impulse2[i] = 1;
                }
            }
            #endregion

            #region  Plotting //GDI+ Implementation Goes Here...
            #endregion

        }
        public void doMagic(double[][] ecg, int width)
        {
            this.Time = new double[width];
            this.S1 = new double[width];
            this.S2 = new double[width];

            #region Smoothing
            for (int i = 0; i < width; i++)
            {
                this.Time[i] = ecg[i][0] / fs;
                this.S1[i] = ecg[i][1];
                this.S2[i] = ecg[i][2];

            }

            double Max_S1 = S1.AsParallel().Max();
            double Max_S2 = S2.AsParallel().Max();
            #endregion

            #region Normalization  // Data Normalization

            for (int i = 0; i < width; i++)
            {
                S1[i] = S1[i] / Max_S1;
                S2[i] = S2[i] / Max_S2;
            }

            #endregion

            #region Plotting //GDI+ Implementation Goes Here...

            #endregion

            #region Differentiation

            D1 = Utils.DifferentiateVector(S1);
            D2 = Utils.DifferentiateVector(S2);

            #endregion

            #region Squaring
            DD1 = new double[width];
            DD2 = new double[width];

            for (int i = 0; i < D1.Length; i++)
            {
                DD1[i] = Math.Pow(D1[i], 2);
                DD2[i] = Math.Pow(D2[i], 2);
            }

            #endregion

            #region Integration

            int beg = this.w / 2;
            INT1 = new double[Time.Length];
            INT2 = new double[Time.Length];

            #region DataIntegration
            for (int i = beg; i < DD1.Length - beg; i++)
            {
                INT1[i] = Utils.Sum(DD1, i - beg, i + beg);
                INT1[i] = INT1[i] / this.w;
            }
            #endregion

            #region Data Normalization

            double max1 = INT1.Max();
            double mean1 = Utils.Mean(INT1);

            for (int i = 0; i < INT1.Length; i++)
            {
                INT1[i] = (INT1[i] - mean1) / max1;
            }
            #endregion

            #region DataIntegration


            for (int i = beg; i < DD2.Length - beg; i++)
            {
                INT2[i] = Utils.Sum(DD2, i - beg, i + beg);
                INT2[i] = INT2[i] / this.w;

            }
            #endregion

            #region DataNormalization

            double max2 = INT2.Max();
            double mean2 = Utils.Mean(INT2);

            for (int i = 0; i < INT2.Length; i++)
            {
                INT2[i] = (INT2[i] - mean2) / max2;
            }

            #endregion


            #endregion

            #region Thresholding

            double comparison1 = 0.3 * INT1.Max();
            double comparison2 = 0.3 * INT2.Max();

            Impulse1 = new double[width];
            for (int i = 0; i < INT1.Length; i++)
            {
                if (INT1[i] > comparison1)
                {
                    Impulse1[i] = 1;
                }
            }

            Impulse2 = new double[width];
            for (int i = 0; i < INT2.Length; i++)
            {
                if (INT2[i] > comparison2)
                {
                    Impulse2[i] = 1;
                }
            }
            #endregion

            #region  Plotting //GDI+ Implementation Goes Here...
            #endregion

        }
    }
}
