// #######################################################
///Objective            : This screen is the Business Layer. It acts as an interface between the front end and the database layer
///Developed by			: Vivien Song
///First Developed on	: 7/24/2009
///Modified by			: Dhrubajyoti Chattopadhyay
///Last Modified On	    : 1/14/2010
// #######################################################
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.Globalization;
using System.Data.OleDb;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for Business_Interface
/// </summary>
namespace FST.Common
{
	public class Business_Interface
	{
        // this class is where all the database access methods are. we use this class mostly as a proxy for some business logic.
        // however, some business logic is unforunately also in the database class. please check in there as well.
		public Database m_dbinstance;

        /// <summary>
        /// Default consturctor initiates a conenction to the database
        /// </summary>
		public Business_Interface()
		{
            if (m_dbinstance == null)
                m_dbinstance = new Database();
		}

        // case processing related
        public string SaveCase(
            string xml,
            string version,
            string DropOut,
            string FBNo,
            string SuspectNo,
            string RefNo,
            string ItemNo,
            string userName,
            string Theta,
            int CompareMethod,
            string Deducible,
            string Degraded_Type,
            string Case12,
            string Hp_Head,
            string Hd_Head,
            string Compare_Type,
            string Lab_Popultn_Type,
            string Processed,
            string LabKitID,
            Guid guid,
            DataTable fromFile,
            out Guid jobGuid
            )
        {
            string res = "";
            res = m_dbinstance.SaveCase(
                                        xml,
                                        version,
                                        DropOut,
                                        FBNo,
                                        SuspectNo,
                                        RefNo,
                                        ItemNo,
                                        userName,
                                        Theta,
                                        CompareMethod,
                                        Deducible,
                                        Degraded_Type,
                                        Case12,
                                        Hp_Head,
                                        Hd_Head,
                                        Compare_Type,
                                        Lab_Popultn_Type,
                                        Processed,
                                        LabKitID,
                                        guid,
                                        fromFile,
                                        out jobGuid);
            return res;
        }
        public DataTable GetPendingCases()
        {
            DataTable dt = m_dbinstance.GetPendingCases();
            return dt;
        }
        public string UpdateCaseStatus(string strRecordID, string status)
        {
            string strResult = "";
            strResult = m_dbinstance.UpdateCaseStatus(strRecordID, status);
            return strResult;
        }
        public DataTable GetPendingCase(string recordID)
        {
            DataTable dt = m_dbinstance.GetPendingCase(recordID);
            return dt;
        }
        public DataTable GetEmailId(string strUserName)
        {
            DataTable dt = m_dbinstance.GetEmailId(strUserName);
            return dt;
        }
        public DataTable GetKnown_Profile(string strType, Guid guid)
        {
            DataTable dt = m_dbinstance.GetKnown_Profile(strType, guid);
            return dt;
        }


        // case data related
        public DataTable GetLocusInOrder(Guid LabKitID)
        {
            DataTable dt = m_dbinstance.GetLocusInOrder(LabKitID);
            return dt;
        }
        public DataTable getLocusAlleles(string locusName)
        {
            DataTable dt = m_dbinstance.getLocusAlleles(locusName);
            return dt;
        }

        // admin related
        #region Edit Frequencies
        /// <summary>
        /// This function picks up all the values of Alleles based on the Locus and on their status
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetAlleles(string strLocus)
        {
            DataTable dt = m_dbinstance.GetAlleles(strLocus);
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable GetFrequencyData()
        {
            DataTable dt = m_dbinstance.GetFrequencyData();
            return dt;
        }
        /// <summary>
        /// added by Dhruba
        /// </summary>
        /// <param name="Alleles"></param>
        /// <param name="DropOut"></param>
        /// <param name="FBNo"></param>
        /// <param name="SuspectNo"></param>
        /// <param name="RefNo"></param>
        /// <param name="ItemNo"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string UpdateFrequency(string strFreqId, string strFreqNew)
        {
            string strResult = "";
            strResult = m_dbinstance.UpdateFrequency(strFreqId, strFreqNew);
            return strResult;
        }
        #endregion

        #region View Submitted Job Status page related
        /// <summary>
        /// This function picks up processing/processed jobs based on status
        /// Added by win
        /// </summary>
        /// <returns></returns>
        public DataTable GetSubmittedJobsInfo(string caseStatus)
        {
            DataTable dt = m_dbinstance.GetSubmittedJobsInfo(caseStatus);
            if (dt.Rows.Count == 0)
            {
                DataRow row;
                row = dt.NewRow();
                dt.Rows.Add(row);
            }
            return dt;
        }
        public void DeleteCaseByRecordID(string recordID)
        {
            m_dbinstance.DeleteCaseByRecordID(recordID);
        }
        public void UpdateCaseByRecordID(string recordID, string caseStatus)
        {
            m_dbinstance.UpdateCaseByRecordID(recordID, caseStatus);
        }
        public int GetCaseCounterByStatus(string caseStatus)
        {
            int caseCounter = 0;
            caseCounter = m_dbinstance.GetCaseCounterByStatus(caseStatus);
            return caseCounter;
        }
        #endregion

        #region Edit Profiles (Lab Types and Population)
        // save profile
        /// <summary>
        /// Added by Dhruba
        /// It is used for insert/update of Known Profiles
        /// </summary>
        /// <param name="strParameters"></param>
        /// <param name="strInsertEdit"></param>
        /// <returns></returns>
        public string SaveKnownProfile(string strInsertEdit, string ID, string active, Dictionary<string, string> locusAllelesDict)
        {
            string strResult = "";
            strResult = m_dbinstance.InsertOrUpdateKnown(strInsertEdit, ID, "Lab Types", null, Guid.Empty, active, locusAllelesDict);
            return strResult;
        }
        /// <summary>
        /// Added by Dhruba
        /// It is used for insert/update of Known Population
        /// </summary>
        /// <param name="strParameters"></param>
        /// <param name="strInsertEdit"></param>
        /// <returns></returns>
        public string SaveKnownPopulation(string strInsertEdit, string ID, string ethnicID, string active, Dictionary<string, string> locusAllelesDict)
        {
            string strResult = "";
            strResult = m_dbinstance.InsertOrUpdateKnown(strInsertEdit, ID, "Population", ethnicID, Guid.Empty, active, locusAllelesDict);
            return strResult;
        }

        // update profile
        /// <summary>
        /// It is used for updating the Status for Known Profile
        /// </summary>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public string UpdateKnownProfileStatus(string strStatus)
        {
            string strResult = "";
            strResult = m_dbinstance.UpdateKnownStatus("Lab Types", strStatus, null, null);
            return strResult;
        }
        /// <summary>
        /// It is used for updating the Status for Known Population
        /// </summary>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public string UpdateKnownPopulationStatus(string strStatus, string strEthnicID)
        {
            string strResult = "";
            strResult = m_dbinstance.UpdateKnownStatus("Population", strStatus, null, strEthnicID);
            return strResult;
        }
        public void UpdateKnownPopulationStatusByID(string keyID, bool active)
        {
            m_dbinstance.UpdateKnownStatus("Population", active ? "Yes" : "No", keyID, null);
        }
        public void UpdateKnownProfileStatusByID(string keyID, bool active)
        {
            m_dbinstance.UpdateKnownStatus("Lab Types", active ? "Yes" : "No", keyID, null);
        }

        // delete profile
        public void DeleteKnownPopulationByID(string curID)
        {
            m_dbinstance.DeleteFromKnownByID("Population", curID);
        }
        public void DeleteKnownProfileByID(string curID)
        {
            m_dbinstance.DeleteFromKnownByID("Lab Types", curID);
        }
        
        // get profile data for screen
        /// <summary>
        /// This function picks up the Known Profile Details
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDetailKnownProfile(string strID)
        {
            DataTable dt = m_dbinstance.GetDetailKnown(strID, "Lab Types");
            return dt;
        }
        /// <summary>
        /// This function picks up the Known Population Details
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDetailKnownPopulation(string strID)
        {
            DataTable dt = m_dbinstance.GetDetailKnown(strID, "Population");
            return dt;
        }
        /// <summary>
        /// This function picks up all the Ethnic data
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetEthnicData()
        {
            DataTable dt = m_dbinstance.GetEthnicData();
            return dt;
        }
        /// <summary>
        /// This function picks up all the Known Profile
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetMasterKnownProfile()
        {
            DataTable dt = m_dbinstance.GetMasterKnown("Lab Types", null);
            return dt;
        }
        /// <summary>
        /// This function picks up all the Known Population based on the Races
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetMasterKnownPopulation(string strEthnicID)
        {
            DataTable dt = m_dbinstance.GetMasterKnown("Population", strEthnicID);
            return dt;
        }

        #endregion

        #region Edit Lab Kits Page and View Drop-Outs (also some Degraded Drop-Outs stuff, although this was disabled)
        // save lab kit and data
        public string SaveLabKit(string labKitName, string creator)
        {
            string strResult = "";
            strResult = m_dbinstance.SaveLabKit(labKitName, creator);
            return strResult;
        }
        public void SaveLabKitData(string guid, DataTable dropOuts, DataTable dropIns, List<int> labKitLoci)
        {
            string strResult = "";
            strResult = m_dbinstance.SaveLabKitData(guid, dropOuts, dropIns, labKitLoci);
        }

        // delete lab kit
        public void DeleteLabKit(string Guid)
        {
            m_dbinstance.DeleteLabKit(Guid);
        }

        // update lab kit data
        /// <summary>
        /// added by Dhruba
        /// </summary>
        /// <param name="strDropoutRateID"></param>
        /// <param name="strDropOutRateNew"></param>
        /// <returns></returns>
        public string UpdateDropOutRates(string labKitID, string strDropoutRateID, string strDropOutRateNew)
        {
            string strResult = "";
            strResult = m_dbinstance.UpdateDropOutRates(labKitID, strDropoutRateID, strDropOutRateNew);
            return strResult;
        }
        /// <summary>
        /// added by Dhruba
        /// </summary>
        /// <param name="strDropoutRateID"></param>
        /// <param name="strDropOutRateNew"></param>
        /// <returns></returns>
        public string UpdateDegradedDropOutRates(string strDropoutRateID, string strDropOutRateNew)
        {
            string strResult = "";
            strResult = m_dbinstance.UpdateDegradedDropOutRates(strDropoutRateID, strDropOutRateNew);
            return strResult;
        }
        /// <summary>
        /// added by Dhruba
        /// </summary>
        /// <param name="strDropoutRateID"></param>
        /// <param name="strDropOutRateNew"></param>
        /// <returns></returns>
        public string UpdateDropInRates(string strID, string strDropInRateNew, string labKitID)
        {
            string strResult = "";
            strResult = m_dbinstance.UpdateDropInRates(strID, strDropInRateNew, labKitID);
            return strResult;
        }

        // get lab kit data
        /// <summary>
        /// This function picks up all the values of DropOuts
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDropOutData(string labKitID)
        {
            DataTable dt = m_dbinstance.GetDropOutData(labKitID);
            return dt;
        }
        /// <summary>
        /// This function picks up all the values of Degraded DropOuts
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDegradedDropOutData()
        {
            DataTable dt = m_dbinstance.GetDegradedDropOutData();
            return dt;
        }
        /// <summary>
        /// This function picks up all the values of DropOut Rates
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable getDropOutRate(string DropoutOptionBegin, string DropoutOptionEnd, int NoOfPersonsInvolvd, string strDeducible, string labKitID)
        {
            DataTable dt = m_dbinstance.getDropOutRate(DropoutOptionBegin, DropoutOptionEnd, NoOfPersonsInvolvd, strDeducible, labKitID);
            return dt;
        }
        /// <summary>
        /// This function picks up all the values of DropIns
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDropInData(string labKitID)
        {
            DataTable dt = m_dbinstance.GetDropInData(labKitID);
            return dt;
        }
        /// <summary>
        /// This function picks up different probable types
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDropOutTypes()
        {
            DataTable dt = m_dbinstance.GetDropOutTypes();
            return dt;
        }
        public DataTable getDropOutOptions()
        {
            DataTable dt = m_dbinstance.getDropOutOptions();
            return dt;
        }
        /// <summary>
        /// This function picks up the no of persons involved in a case
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetNoOfPersonsInvolved()
        {
            DataTable dt = m_dbinstance.GetNoOfPersonsInvolved();
            return dt;
        }
        #endregion

        #region Scenario Management
        public DataTable GetCaseTypesTable()
        {
            return m_dbinstance.GetCaseTypesTable();
        }
        public void UpdateCaseTypeStatusByID(string keyID, bool enabled)
        {
            m_dbinstance.UpdateCaseTypeStatusByID(keyID, enabled);
        }
        #endregion


        // testing related
        public void WriteTest(Guid testID, Guid subTestID, string testName, string comparisonID, int comparisonType, bool testManualEntry, bool testBulk, string LabKitID, int dnaEvidenceAmount)
        {
            m_dbinstance.WriteTest(testID, subTestID, testName, comparisonID, comparisonType, testManualEntry, testBulk, LabKitID, dnaEvidenceAmount);
        }
        public void WriteTestResults(string subTestId, string comparisonId, string Asian, string Black, string Caucasian, string Hispanic)
        {
            m_dbinstance.WriteTestResults(subTestId, comparisonId, Asian, Black, Caucasian, Hispanic);
        }

        // UI related
        public DataTable GetProfilesFromFile(FileUpload fuPopulationUpload, string path, Guid LabKitID)
        {
            string strErrMsg = string.Empty;
            DataTable dt = null;
            DataTable loci = this.GetLocus(LabKitID);

            try
            {
                string connString = string.Empty;
                string extension = string.Empty;
                // determine file type
                if (fuPopulationUpload.FileName.EndsWith(".csv")) extension = ".csv";
                else if (fuPopulationUpload.FileName.EndsWith(".xlsx")) extension = ".xlsx";
                else if (fuPopulationUpload.FileName.EndsWith(".xls")) extension = ".xls";
                // save file
                string filename = DateTime.Now.Ticks.ToString() + extension;
                fuPopulationUpload.SaveAs(path + filename);

                // create connection string
                switch (extension)
                {
                    case ".csv":
                        connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='text;HDR=YES;FMT=CSVDelimited';");
                        break;
                    case ".xlsx":
                        connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + filename + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=2';");
                        break;
                    case ".xls":
                        connString = string.Format("Provider=Microsoft.Jet.OLEDB.12.0;Data Source=" + path + filename + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';");
                        break;
                }

                // open the file
                using (OleDbConnection cn = new OleDbConnection(connString))
                {
                    cn.Open();
                    string query = string.Empty;
                    string sheetName = "Sheet1$";

                    DataTable dtTables = cn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                    if (dtTables.Rows.Count == 1)
                        sheetName = (string)dtTables.Rows[0]["TABLE_NAME"];

                    switch (extension)
                    {
                        case ".csv": query = "SELECT * FROM [" + filename + "]"; break;
                        case ".xlsx": query = "SELECT * FROM [" + sheetName + "]"; break;
                        case ".xls": query = "SELECT * FROM [" + sheetName + "]"; break;
                    }
                    // read file
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, cn);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    // remove blank rows
                    foreach (DataRow dr in dt.Select("ID='' OR ID IS NULL"))
                        dt.Rows.Remove(dr);

                    // remove unneeded columns
                    List<string> removeColumns = new List<string>();
                    foreach (DataColumn dc in dt.Columns)
                        if (dc.ColumnName.ToUpper() != "ID" && loci.Select("FieldName='" + dc.ColumnName + "'").Length == 0)
                            removeColumns.Add(dc.ColumnName);
                    foreach (string colName in removeColumns)
                        dt.Columns.Remove(colName);

                    foreach (DataRow dr in dt.Rows)
                    {
                        // break on cells where there are no commas between values
                        foreach (DataColumn dc in dt.Columns)
                            if (dc.ColumnName.ToUpper() != "ID")
                            {
                                dr[dc.ColumnName] = dr[dc.ColumnName].ToString().Trim();
                                if (dr[dc.ColumnName].ToString().IndexOf(",") == -1 && dr[dc.ColumnName].ToString().Contains(' ')) // no commas, values separated by space
                                {
                                    string[] arr = dr[dc.ColumnName].ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                                    if (arr.Length != 2)
                                    {
                                        MessageBox.Show("There were too many values in one of the data cells.");
                                        return null;
                                    }

                                    dr[dc.ColumnName] = arr[0] + "," + arr[1];
                                }

                                if (dr[dc.ColumnName].ToString().IndexOf(",") != dr[dc.ColumnName].ToString().LastIndexOf(","))  // too many commas
                                {
                                    MessageBox.Show("There were too many commas one of the data cells.");
                                    return null;
                                }
                            }

                        // if we have a single value then we duplicate it (sorry for this)
                        foreach (DataColumn dc in dt.Columns)
                            if (dc.ColumnName != "ID" && dc.ColumnName != "Ethnicity")
                                if (!dr[dc.ColumnName].ToString().Contains(",")
                                    || string.IsNullOrEmpty(dr[dc.ColumnName].ToString().Trim().Split(new char[] { ',' })[1]))
                                    if (!dr[dc.ColumnName].ToString().Contains(",") || string.IsNullOrEmpty(dr[dc.ColumnName].ToString().Trim().Split(new char[] { ',' })[1])) dr[dc.ColumnName] = dr[dc.ColumnName].ToString().Trim().Replace(",", "") + "," + dr[dc.ColumnName].ToString().Trim().Replace(",", "");

                        foreach (DataColumn dc in dt.Columns)
                            if (dc.ColumnName.ToUpper() != "ID")
                            {
                                string vallele = dr[dc.ColumnName].ToString();
                                string[] valleles = vallele.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                dr[dc.ColumnName] = valleles.Length > 0 ? valleles[0].Trim() + "," + valleles[1].Trim() : string.Empty;
                            }
                    }

                    // order the columns in the proper order
                    dt.Columns["ID"].SetOrdinal(dt.Columns.Count - 1);
                    int idx = 0;
                    foreach (DataRow dr in loci.Rows)
                        if (dt.Columns.Contains(dr["FieldName"].ToString()))
                            dt.Columns[dr["FieldName"].ToString()].SetOrdinal(idx++);

                    // uppercase all the columns except vWA which needs a special case
                    foreach (DataColumn dc in dt.Columns)
                        dc.ColumnName = dc.ColumnName.ToUpper();
                }
            }
            catch
            {
                // handle errors
                MessageBox.Show("There was an error reading the uploaded file. Please try uploading an Excel or CSV file in the correct format.");
                return null;
            }

            return dt;
        }
		/// <summary>
		/// This function picks up all the different types of Compare Cases based on their status
		/// Added by Dhruba
		/// </summary>
		/// <returns></returns>
		public DataTable GetCaseTypes()
		{
			DataTable dt = m_dbinstance.GetCaseTypes();
			return dt;
		}
		/// <summary>
		/// This function picks up all the values of Theta based on their status
		/// Added by Dhruba
		/// </summary>
		/// <returns></returns>
		public DataTable GetTheta()
		{
			DataTable dt = m_dbinstance.GetTheta();
			return dt;
		}
		/// <summary>
		/// This function picks up all the values of Ethnic Races based on their status
		/// Added by Dhruba
		/// </summary>
		/// <returns></returns>
		public DataTable GetRaces()
		{
			DataTable dt = m_dbinstance.GetRaces();
			return dt;
		}

        // general
        /// <summary>
        /// This function picks up all the values of Locus based on their status
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetLocus(Guid LabKitID)
        {
            DataTable dt = m_dbinstance.GetLocus(LabKitID);
            return dt;
        }
        /// <summary>
        /// This method returns lab kits from the DB like its name implies
        /// </summary>
        /// <returns>A DataTable with LabKits</returns>
        public DataTable GetLabKits()
        {
            DataTable dt = m_dbinstance.GetLabKits(); ;
            return dt;
        }

    }
}
