using ServerApp.Models;
using ServerApp.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using WebMatrix.WebData;
using CardiologistV2;
using CardiologistV2.Models;
using System.IO;

namespace ServerApp
{
    /// <summary>
    /// Summary description for PatientService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PatientService : System.Web.Services.WebService
    {
        public static string fileData;
        public static string InputVector;

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string Upload(int Id,string filename,string Data,int fs)
        {

            //TODO: Pass the file to the Feature Extractor
           
            #region Filter File
            string path = Server.MapPath("~/Models/MatlabFilters");
            System.IO.File.WriteAllText(path+"\\"+filename,Data);
            MLApp.MLApp matlab = new MLApp.MLApp();
            matlab.Execute("cd('"+path+"')");
            matlab.Execute("a=load('"+filename+"')");
            matlab.Execute("c=baselinefilter(a,"+fs+")");
            var o = (double[,]) matlab.GetVariable("c","base");
            string filedata = "";

            for (int i = 0; i < o.GetLength(0); i++)
            {
                for (int j = 0; j <= o.GetLength(1); j++)
                {
                    filedata += o[i,j]+" ";
                }
                filedata.Trim(' ');

                filedata += "\n";
            }
            #endregion

            #region Pass File to Feature Extractor
            ECG ecgFeatureExtractor = new ECG();
            InputVector = ecgFeatureExtractor.ExtractFeatures(filedata, filename, fs); // fs = 370
            string filename2 = "feat.txt";
            File.WriteAllText(path + "\\" + filename2, InputVector);
            #endregion

           //File.WriteAllText(path + "\\" + filename,InputVector);

           #region Create New Record
           CardiologistV2.DAL.DatabaseContext db = new CardiologistV2.DAL.DatabaseContext();
            var p =  db.Patients.Find(Id);
            var r = new CardiologistV2.Models.Record();
            r.PatientID = Id;
            r.Recdate = DateTime.Now;
            r.Data = filedata;

            //r.Featuress.Add(new Features() {
            //Value=InputVector,
            //})
        ;// = InputVector;
           #endregion
           
            //Calling Matlab...
            #region simulate network
            matlab.Execute("b = load('"+filename2+"')");
            matlab.Execute("n = load('matlab.mat')");
            matlab.Execute("s = sim(n.net ,b)");
            var obj = (double[,])matlab.GetVariable("s", "base");
            #endregion

            #region Make Logical Decision
            int MaxIndex = -1;
            int numpulses = obj.GetLength(1);
            int numpvc = 0; int numlbbb = 0; int numrbbb = 0; int numnormal = 0;
            var vector = new List<double>();
            for (int j = 0; j < numpulses; j++)
            {
                for (int i = 0; i < obj.GetLength(0); i++)
                {
                    vector.Add(obj[i, j]);
                }
                MaxIndex = vector.IndexOf(vector.Max());
                switch (MaxIndex)
                {
                    case 0: //pvc
                        numpvc++;
                        break;
                    case 1: //lbbb
                        numlbbb++;
                        break;
                    case 2: //rbbb
                        numrbbb++;
                        break;
                    case 3: //normal
                        numnormal++;
                        break;
                    default:
                        break;
                }
            }
           
          string result = "Pvc = " + ((double)numpvc * 100.00) / numpulses + "%" + "Lbbb = " 
                + ((double)numlbbb * 100.00) / numpulses + "% " + "Rbbb = "
                 + ((double)numrbbb * 100.00) / numpulses + "% " + "Normal = " 
                 + ((double)numnormal * 100.00) / numpulses + "%";
            #endregion

            #region Save in Database
          r.Result = result;
            p.Records.Add(r);
            db.SaveChanges();
          #endregion
            //TODO: Say everything's Alright
           return "200|"+result;
        }

         [WebMethod]
        public string register(ServerApp.Models.Patient patient,AccountCredentials account)
        {

            /////
            #region Create User Account
            WebSecurity.CreateUserAndAccount(account.username, account.password);

            #endregion

            #region Add User to Role 'Patient'
            bool pat_role = false;
            string[] roles = Roles.GetAllRoles();
            for (int i = 0; i < roles.Length; i++)
            {
                if (roles[i] == "Patient")
                { pat_role = true; break; }
            }
            if (pat_role == false)
            { Roles.CreateRole("Patient"); }
            Roles.AddUsersToRole(new[] { account.username }, "Patient");
            #endregion

            #region Save Patient
            CardiologistV2.DAL.DatabaseContext db = new CardiologistV2.DAL.DatabaseContext();
            db.Users.Add(new CardiologistV2.Models.User()
            {
                UserID = WebSecurity.GetUserId(account.username)
            });

             var p = new CardiologistV2.Models.Patient();
             p.PatientID = WebSecurity.GetUserId(account.username);
             p.Name = patient.Name;
             p.DateOfbirth = patient.DOB;
             p.Gender = patient.Gender;
             p.Address = patient.Address;
             p.Job = patient.Job;
             p.Smoker = patient.Smoker;
             p.Alcoholic = patient.Alcoholic;
             db.Patients.Add(p);

            db.SaveChanges();
            #endregion

            /////

            return "200";
        
        }

         [WebMethod]
        public string Login(AccountCredentials account)
        {
            //Check account existance 
            #region Check User

            int Id = WebSecurity.GetUserId(account.username);

             CardiologistV2.DAL.DatabaseContext db = new CardiologistV2.DAL.DatabaseContext();
             CardiologistV2.Models.Patient p =  db.Patients.Find(Id);

            #endregion

          if (p != null)
          {
              //Authenticate user and return ID to Client

              bool response = WebSecurity.Login(account.username, account.password);
              if (response)
              {
                  return "200|"+p.PatientID;
              }

              else
              {
                  return "422|Request Could not be Processed";
              }

          }

          else
          {
              return "400|Bad Request";
          
          }

        }
    }
}
