using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Models.DomainModels
{
    public partial class peakDetection
    {
        public double[] Time;
        public int fs = 370; // M.I.T Standard
        public int w = 30; // 20;
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

       public peakDetection(double[][] recs)
        {
            doMagic(recs);
        }
       public peakDetection(double[][] recs, int fs)
        {
            this.fs = fs;
            doMagic(recs);

        }
       public peakDetection(double[][] recs, int fs, int w)
        {
            this.fs = fs;
            this.w = w;

            doMagic(recs);

        }
      
        public void doMagic(double[][] recs)
       {
           #region Smoothing
           this.Time = new double[recs.Length];
           this.S1 = new double[recs.Length];
           this.S2 =  new double[recs.Length];

           for (int i = 0; i < recs.Length; i++)
           {
               this.Time[i] = recs[i][0] / fs;
               this.S1[i] = recs[i][1];
               this.S2[i] = recs[i][2];
           }
           #endregion

           double Max_S1 = S1.AsParallel().Max();
           double Max_S2 = S2.AsParallel().Max();

        #region Normalization  // Data Normalization

            for (int i = 0; i < recs.Length; i++)
            {
                S1[i] = S1[i] / Max_S1;
                S2[i] = S2[i] / Max_S2;
            }
        #endregion

  //#region TestingPurposes //Delete ONLY Upon Finishing

             Max_S1 = S1.AsParallel().Max();
             Max_S2 = S2.AsParallel().Max();
            double Min_S1 = S1.AsParallel().Min();
            double Min_S2 = S2.AsParallel().Min();

        #region Plotting //GDI+ Implementation Goes Here...

            #endregion

        #region Differentiation

            D1 = Utils.DifferentiateVector(S1);
            D2 = Utils.DifferentiateVector(S2);
           
        #endregion

   #region TestingPurposes //Delete ONLY Upon Finishing

   //         //testing
            double Max_D1 = D1.AsParallel().Max();
            double Max_D2 = D2.AsParallel().Max();
            double Min_D1 = D1.AsParallel().Min();
            double Min_D2 = D2.AsParallel().Min();
   #endregion
  
        #region Squaring
          DD1 = new double[recs.Length];
          DD2 = new double[recs.Length];

           for(int i=0;i<D1.Length;i++)
           {
               DD1[i] = Math.Pow(D1[i], 2);  // D1[i] * D1[i];
               DD2[i] = Math.Pow(D2[i], 2);  // D2[i]*D2[i];
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
            
            //Done!

           Impulse1 = new double[recs.Length];
            for(int i=0;i<INT1.Length;i++)
            {
                if(INT1[i]>comparison1)
                {
                    Impulse1[i]=1;
                }
            }
           
          Impulse2 = new double[recs.Length];
          for(int i=0;i<INT2.Length;i++)
          {
              if(INT2[i]>comparison2)
              {
                  Impulse2[i] = 1;
              }
          }

         #endregion
        
        //Passed Testing!

        #region  Plotting //GDI+ Implementation Goes Here...
        #endregion

        }

    }
}
