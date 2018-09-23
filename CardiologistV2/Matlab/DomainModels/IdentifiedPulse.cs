using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardiologistV2.Matlab.DomainModels
{
    public partial class IdentifiedPulse
    {
        public double signalWidth;
        private List<Point> P;
        private List<Point> Q;
        private List<Point> R;
        private List<Point> S;
        private List<Point> T;
        public double[] Pvalue;
        public double[] Qvalue;
        public double[] Rvalue;
        public double[] Svalue;
        public double[] Tvalue;
        public double[] QRS;
        public double[] PR;
        public double[] QT;
        public double[] RR;
        public double[] ST;
        List<QRSdetection> QRSList;
        public double[] S1;
        public double[] S2;
        public Dictionary<int, double> startingPoints;
        public Dictionary<int, double> endingPoints;
        public List<QRSregion> QRSregions;
        public double[] Impulse1;
        public double[] Impulse2;
        public int fs = 370;
        public double[] Inv2;
        //private Point startingP;
        //  private int Pulses;

        public IdentifiedPulse()
        {
        }
        public IdentifiedPulse(double[][] ecg, int fs, int MyWidth)
        {

            startingPoints = new Dictionary<int, double>();
            endingPoints = new Dictionary<int, double>();
            int w = 20;
            DetectPoints(ecg, MyWidth, fs); //PLUG IN the width Here!

            var tmp = new List<double>();
            foreach (var s in QRSList)
            {
                tmp.AddRange(s.S1.ToList());
            }
            this.S1 = tmp.ToArray();
            tmp = null;


            this.Impulse1 = QRSList.ElementAt(0).Impulse1;
            QRSregions = Detect_Regions(this.Impulse1);
            if (QRSregions != null)
            {
                this.Q = new List<Point>();
                this.R = new List<Point>();
                this.S = new List<Point>();
                double meanValue = Utils.Mean(S1); // Calculate mean of the Whole Signal
                for (int i = 0; i < S1.Length; i++)
                {
                    S1[i] = S1[i] - meanValue;  //Math.Abs(S1[i] - meanValue);
                }
                for (int i = 0; i < QRSregions.Count; i++)
                {
                    int start = QRSregions.ElementAt(i).StartingPoint.index;
                    int finish = QRSregions.ElementAt(i).EndingPoint.index;
                    Point r = this.Detect_R(S1, start, finish);
                    Point q = this.Detect_Q(S1, r, start);
                    Point s = this.Detect_S(S1, r, finish);

                    this.Q.Add(q);
                    this.R.Add(r);
                    this.S.Add(s);
                    this.QRSregions.ElementAt(i).Q = q;
                    this.QRSregions.ElementAt(i).R = r;
                    this.QRSregions.ElementAt(i).S = s;
                }

                Inv2 = InvertImpulse(this.Impulse1);
                startingPoints.Clear();
                endingPoints.Clear();

                #region Detect_Points
                for (int i = 0; i < Inv2.Length - 1; i++)
                {
                    if (Inv2[i] - Inv2[i + 1] == -1) //if it's a starting Point
                    {
                        this.startingPoints.Add(i + 1, S1[i + 1]);
                    }
                    if (Inv2[i] - Inv2[i + 1] == 1) //if it's an Ending Point
                    {
                        this.endingPoints.Add(i + 1, S1[i + 1]);
                    }
                }
                #endregion

                if (Inv2[0] == 1)// If Inverse starts as Signal starts
                {
                    this.endingPoints.Remove(0); //remove the first end point
                }
                int startPoint = 0;
                int endPoint = 0;
                int midPoint = 0;
                double max = 0;
                ////////////////////////////////////////////////
                T = new List<Point>();
                P = new List<Point>();
                P.Insert(0, new Point(0, 0.0));
                QRSregions.ElementAt(0).P = new Point(0, 0.0);  //Attach the First P
                //max = Utils.Max(S1, startingP.index, this.startingPoints.ElementAt(0).Key); ////////////////////////////////////////////////////////////////////////////
                //startingP.index = Utils.index;
                //startingP.value = max;
                //P.Insert(0, startingP);
                ///
                double[] S1abs = new double[S1.Length];

                for (int i = 0; i < S1.Length; i++)
                {
                    S1abs[i] = Math.Abs(S1[i]);
                }

                int indexmaxAbs = 0;
                for (int i = 0; i < startingPoints.Count - 1; i++)
                {
                    startPoint = this.startingPoints.ElementAt(i).Key;
                    endPoint = this.endingPoints.ElementAt(i).Key;
                    midPoint = (endPoint + startPoint) / 2;

                    max = Utils.Max(S1abs, startPoint, midPoint);
                    indexmaxAbs = Utils.index;

                    Point pointT = new Point(indexmaxAbs, S1[indexmaxAbs]);

                    T.Insert(i, pointT); // T of the Previous Signal
                    // max = Utils.Max(S1, midPoint, endPoint);
                    max = Utils.Max(S1abs, midPoint, endPoint);
                    indexmaxAbs = Utils.index;

                    Point pointP = new Point(indexmaxAbs, S1[indexmaxAbs]);
                    P.Insert(i + 1, pointP); // P of the Current Signal

                    QRSregions.ElementAt(i).T = pointT;
                    Point pP = pointP;
                    QRSregions.ElementAt(i + 1).P = pP;
                    pointT = null;
                    pointP = null;
                }

                startPoint = this.startingPoints.ElementAt(startingPoints.Count - 1).Key;// Calculating the Last T
                endPoint = this.endingPoints.ElementAt(startingPoints.Count - 1).Key;
                midPoint = (startPoint + endPoint) / 2;

                max = Utils.Max(S1abs, startPoint, midPoint);
                indexmaxAbs = Utils.index;

                Point lastpointT = new Point(indexmaxAbs, S1[indexmaxAbs]);
                T.Insert(startingPoints.Count - 1, lastpointT); //adding to List<Point> T
                QRSregions.ElementAt(startingPoints.Count - 1).T = lastpointT; //adding it to QRSregions
                lastpointT = null;

                QRSregions.RemoveAt(0);
                QRS = new double[QRSregions.Count - 1];
                PR = new double[QRSregions.Count - 1];
                QT = new double[QRSregions.Count - 1];
                ST = new double[QRSregions.Count - 1];

                Pvalue = new double[QRSregions.Count - 1];
                Qvalue = new double[QRSregions.Count - 1];
                Rvalue = new double[QRSregions.Count - 1];
                Svalue = new double[QRSregions.Count - 1];
                Tvalue = new double[QRSregions.Count - 1];

                for (int i = 0; i < QRSregions.Count - 1; i++)
                {
                    Point p = QRSregions.ElementAt(i).P;
                    Point q = QRSregions.ElementAt(i).Q;
                    Point r = QRSregions.ElementAt(i).R;
                    Point s = QRSregions.ElementAt(i).S;
                    Point t = QRSregions.ElementAt(i).T;

                    Pvalue[i] = p.value;
                    Qvalue[i] = q.value;
                    Rvalue[i] = r.value;
                    Svalue[i] = s.value;
                    Tvalue[i] = t.value;

                    QRS[i] = Calculate_QRS(new Point(QRSregions.ElementAt(i).StartingPoint.index, QRSregions.ElementAt(i).StartingPoint.value)
                        , new Point(QRSregions.ElementAt(i).EndingPoint.index, QRSregions.ElementAt(i).EndingPoint.value)); //TODO: Change s to the ending region point
                    PR[i] = Calculate_PR(p, r);
                    QT[i] = Calculate_QT(q, t);
                    ST[i] = Calculate_ST(s, t);
                }

                RR = Detect_RR(this.R).ToArray();

                ////Delete the First Signal with all of its Components

                this.P.RemoveAt(0);
                this.Q.RemoveAt(0);
                this.R.RemoveAt(0);
                this.S.RemoveAt(0);
                this.T.RemoveAt(0);



                double Pmax = Pvalue.Max();
                double Qmin = Qvalue.Min();
                double Qmax = Qvalue.Max();
                double Rmin = Rvalue.Min();
                double Rmax = Rvalue.Max();
                double Smin = Svalue.Min();
                double Smax = Svalue.Max();
                double Tmin = Tvalue.Min();
                double Tmax = Tvalue.Max();

                double QRSmin = QRS.Min();
                double QRSmax = QRS.Max();
                double PRmin = PR.Min();
                double PRmax = PR.Max();
                double QTmin = QT.Min();
                double QTmax = QT.Max();
                double STmin = ST.Min();
                double STmax = ST.Max();
                double RRmin = RR.Min();
                double RRmax = RR.Max();

                double Pmean = Utils.Mean(Pvalue);
                double Qmean = Utils.Mean(Qvalue);
                double Rmean = Utils.Mean(Rvalue);
                double Smean = Utils.Mean(Svalue);
                double Tmean = Utils.Mean(Tvalue);
                double QRSmean = Utils.Mean(QRS);
                double PRmean = Utils.Mean(PR);
                double QTmean = Utils.Mean(QT);
                double STmean = Utils.Mean(ST);
                double RRmean = Utils.Mean(RR);

                for (int i = 0; i < QRS.Length; i++)
                {

                    if (Pmax == 0)
                    {
                        Pmax = 2;
                    }
                    if (Qmax == 0)
                    {
                        Qmax = 2;
                    }

                    if (Rmax == 0)
                    {
                        Rmax = 2;
                    }

                    if (Smax == 0)
                    {
                        Smax = 2;
                    }
                    if (Tmax == 0)
                    {
                        Tmax = 2;
                    }
                    if (QRSmax == 0)
                    {
                        QRSmax = 2;
                    }
                    if (PRmax == 0)
                    {
                        PRmax = 2;
                    }
                    if (QTmax == 0)
                    {
                        QTmax = 2;
                    }
                    if (STmax == 0)
                    {
                        STmax = 2;
                    }
                    if (RRmax == 0)
                    {
                        RRmax = 2;
                    }

                    //if (max - min == 0)
                    //{
                    //    max = 0.01;
                    //    min = 0.001;
                    //}
                    //(value-mean)/max
                    Pvalue[i] = (Pvalue[i] - Pmean) / Pmax;

                    Qvalue[i] = (Qvalue[i] - Qmean) / Qmax;

                    Rvalue[i] = (Rvalue[i] - Rmean) / Rmax;

                    Svalue[i] = (Svalue[i] - Smean) / Smax;

                    Tvalue[i] = (Tvalue[i] - Tmean) / Tmax;

                    QRS[i] = (QRS[i] - QRSmean) / QRSmax;

                    PR[i] = (PR[i] - PRmean) / PRmax;

                    QT[i] = (QT[i] - QTmean) / QTmax;

                    ST[i] = (ST[i] - STmean) / STmax;

                    RR[i] = (RR[i] - RRmean) / RRmax;


                }
            }
            else
            {
                return;
            }
        }
        public void DetectPoints(double[][] ecg, int width, int fs)
        {
            this.QRSList = new List<QRSdetection>();

            QRSdetection d = new QRSdetection(ecg, width, fs);

            for (int i = 0; i < d.Impulse1.Length - 1; i++)
            {

                if (d.Impulse1[i] - d.Impulse1[i + 1] == -1) //if it's a starting Point
                {
                    this.startingPoints.Add(i + 1, d.S1[i + 1]);
                }
                if (d.Impulse1[i] - d.Impulse1[i + 1] == 1) //if it's an Ending Point
                {
                    this.endingPoints.Add(i + 1, d.S1[i + 1]);
                }
            }

            QRSList.Add(d);

        }

        public void DetectPoints(double[][] ecg)
        {
            int width = 2000;
            /*   List<QRSdetection>*/
            this.QRSList = new List<QRSdetection>();

            while (this.startingPoints.Keys.Count < 100 && this.endingPoints.Keys.Count <= 100)
            // 102 يوجد زيادة في عدد النقط , تحسبا فقط في حال النقطة الاولى تدل على إشارة مشوهة
            {

                int Offset = this.QRSList.Count * width;
                if (Offset < ecg.Length)
                {
                    QRSdetection d = new QRSdetection(ecg, 370, 20, Offset);

                    for (int i = 0; i < d.Impulse1.Length - 1; i++)
                    {

                        if (d.Impulse1[i] - d.Impulse1[i + 1] == -1) //if it's a starting Point
                        {
                            this.startingPoints.Add(i + Offset + 1, d.S1[i + 1]);
                        }
                        if (d.Impulse1[i] - d.Impulse1[i + 1] == 1) //if it's an Ending Point
                        {
                            this.endingPoints.Add(i + Offset + 1, d.S1[i + 1]);
                        }
                    }

                    QRSList.Add(d);
                }

            }
            List<double> OverallList = new List<double>();

            foreach (var sig in QRSList)
            {
                OverallList.AddRange(sig.Impulse1.ToList());
            }
            this.Impulse1 = OverallList.ToArray();
        }
        public List<QRSregion> Detect_Regions(double[] Impulse)//Impulse: 0,1 Matrix of the Whole Signal
        {

            List<QRSregion> regions = new List<QRSregion>();

            #region CheckPoints

            if (QRSList.ElementAt(0).Impulse1[0] == 1)// If signal starts after a pulse commences
            {
                //startingP.index =    this.endingPoints.ElementAt(0).Key+1;

                this.endingPoints.Remove(0); //remove it
            }

            #endregion

            for (int i = 0; i < startingPoints.Count; i++)
            {
                QRSregion region = new QRSregion();
                region.StartingPoint = new Point();
                region.EndingPoint = new Point();
                region.StartingPoint.index = startingPoints.ElementAt(i).Key;
                region.StartingPoint.value = startingPoints.ElementAt(i).Value;
                region.EndingPoint.index = endingPoints.ElementAt(i).Key;
                region.EndingPoint.value = endingPoints.ElementAt(i).Value;

                regions.Add(region);
            }
            return regions;

        }
        public Point Detect_R(double[] QRSInput, int start, int finish)
        {
            Point R;
            double max = QRSInput.Min();
            int index = 0;
            for (int i = start; i <= finish; i++)
            {
                if (Math.Abs(QRSInput[i]) > max)
                {
                    max = QRSInput[i];
                    index = i;
                }
            }
            R = new Point(index, max);
            return R;
        }
        public Point Detect_Q(double[] QRSInput, Point R, double start)
        {
            Point Q;
            int index = R.index;

            double min = R.value;

            for (int i = R.index; i >= start; i--)
            {
                if (QRSInput[i] < min)
                {
                    min = QRSInput[i];
                    index = i;
                }
            }
            Q = new Point(index, Math.Abs(min));

            return Q;
        }
        public Point Detect_S(double[] QRSInput, Point R, double finish)
        {
            Point S;
            double min = R.value;
            int index = R.index;
            for (int i = R.index; i <= finish; i++)
            {
                if (QRSInput[i] < min)
                {
                    min = QRSInput[i];
                    index = i;
                }
            }
            S = new Point(index, Math.Abs(min));
            return S;
        }
        public double[] InvertImpulse(double[] Impulse)
        {
            double[] Inverse = new double[Impulse.Length];
            for (int i = 0; i < Inverse.Length; i++)
            {
                if (Impulse[i] == 0)
                {
                    Inverse[i] = 1;
                }
                else
                {
                    Inverse[i] = 0;
                }
            }
            return Inverse;
        }

        public List<double> Detect_RR(List<Point> R)
        {
            List<double> res = new List<double>();
            for (int i = 1; i < R.Count - 1; i++)
            {
                res.Add(Index2Time(R.ElementAt(i).index, fs) - Index2Time(R.ElementAt(i - 1).index, fs));
            }
            //  res.RemoveAt(0);
            return res;
        }
        public double Calculate_QRS(Point Q, Point S)
        {
            double result = Index2Time(S.index, fs) - Index2Time(Q.index, fs);
            return result;
        }
        public double Calculate_PR(Point P, Point R)
        {
            double result = Index2Time(R.index, fs) - Index2Time(P.index, fs);
            return result;
        }
        public double Calculate_QT(Point Q, Point T)
        {
            double result = Index2Time(T.index, fs) - Index2Time(Q.index, fs);
            return result;
        }
        public double Calculate_ST(Point S, Point T)
        {

            double result = Index2Time(T.index, fs) - Index2Time(S.index, fs);
            return result;
        }
        public double Index2Time(int index, int fs)
        {
            return (double)(1 + index) / (double)fs;

        }

    }
}
