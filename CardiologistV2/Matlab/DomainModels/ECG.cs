using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardiologistV2.Matlab.DomainModels
{
    public partial class ECG
    {
        public string fileName;
        public string ExtractFeatures(string fileData,string fileName, int fs)
        {
            this.fileName = fileName;
            string[] lines = fileData.Split('\n');
            //Check For string Line Sanity ...
            List<string> linesList = lines.ToList<string>();

            for (int i = 0; i < linesList.Count; i++)
            {
                if (linesList.ElementAt(i) == "")
                {
                    linesList.RemoveAt(i);
                }
            }
            //End Checking
            lines = linesList.ToArray();
            double[][] RecordChunk = new double[lines.Count()][];
            string[] chunk = null;
            int currentpointer = 0;
            int MyWidth = 0;
            IdentifiedPulse pulse = null;
            for (int i = 0; i < lines.Count(); i++)
            {
                chunk = lines[i].Split(' ');

                for (int j = 0; j <= chunk.Length; j++)
                {
                    string sample = chunk[j];
                    if (sample != "")
                    {
                        RecordChunk[i] = new double[3];
                        RecordChunk[i][0] = Convert.ToDouble(sample);
                        currentpointer = j + 1;
                        break;
                    }

                }
                for (int j = currentpointer; j <= chunk.Length; j++)
                {
                    string lead2 = chunk[j];
                    if (lead2 != "")
                    {

                        RecordChunk[i][1] = Convert.ToDouble(lead2);
                        currentpointer = j + 1;
                        break;
                    }

                }
                for (int j = currentpointer; j <= chunk.Length; j++) //no =
                {
                    string leadv2 = chunk[j];
                    if (leadv2 != "")
                    {

                        RecordChunk[i][2] = Convert.ToDouble(leadv2);

                        break;
                    }

                }
            }

            int cols = RecordChunk[0].Length;
            int rows = RecordChunk.Count();

            MyWidth = (int)RecordChunk[RecordChunk.Count() - 1][0] - (int)RecordChunk[0][0] + 1;//2
            pulse = new IdentifiedPulse(RecordChunk, 370, MyWidth); //IdentifiedPulse(double[][] ecg, int fs)

            #region AnalysisSucceeded

            string P = "", Q = "", R = "", S = "", T = "", QRS = "", PR = "", QT = "", ST = "", RR = "";
            for (int i = 0; i < pulse.Pvalue.Length; i++)
            {
                P += pulse.Pvalue[i].ToString() + " ";
                Q += pulse.Qvalue[i].ToString() + " ";
                R += pulse.Rvalue[i].ToString() + " ";
                S += pulse.Svalue[i].ToString() + " ";
                T += pulse.Tvalue[i].ToString() + " ";
                QRS += pulse.QRS[i].ToString() + " ";
                PR += pulse.PR[i].ToString() + " ";
                QT += pulse.QT[i].ToString() + " ";
                ST += pulse.ST[i].ToString() + " ";
                RR += pulse.RR[i].ToString() + " ";
            }

            string InputVector = P + "\n\n" + Q + "\n\n" + R + "\n\n" + S + "\n\n" + T + "\n\n" + QRS + "\n\n" + PR + "\n\n" + QT + "\n\n" + ST + "\n\n" + RR + "\n\n";
            #endregion
            return InputVector;
        }           
    }
}
