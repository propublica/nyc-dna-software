// #######################################################
///Objective            : This screen interacts with the database through the procedures and is the Database Layer
///Developed by			: Vivien Song
///First Developed on	: 7/24/2009
///Modified by			: Dhrubajyoti Chattopadhyay
///Last Modified On		: 1/14/2010
// #######################################################
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.Globalization;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for Database
/// </summary>
namespace FST.Common
{
    public class Database
    {
        private string m_connString;
        public Database()
        {
            FSTConfigSection FSCon = new FSTConfigSection();
            m_connString = FSCon.ConnectionString;
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
            Guid fromFileGuid,
            DataTable fromFile,
            out Guid jobGuid
            )
        {
            jobGuid = Guid.Empty;
            SqlConnection myConnection = null;
            string res = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spInsertCase";
                    cmd.Parameters.AddWithValue("@xml", xml);
                    cmd.Parameters.AddWithValue("@version", version);
                    cmd.Parameters.AddWithValue("@DropOut", DropOut);
                    cmd.Parameters.AddWithValue("@FBNo", FBNo);
                    cmd.Parameters.AddWithValue("@SuspectNo", SuspectNo);
                    cmd.Parameters.AddWithValue("@RefNo", RefNo);
                    cmd.Parameters.AddWithValue("@ItemNo", ItemNo);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@Theta", Theta);
                    cmd.Parameters.AddWithValue("@CompareMethod", CompareMethod);
                    cmd.Parameters.AddWithValue("@Deducible", Deducible);
                    cmd.Parameters.AddWithValue("@Degraded_Type", Degraded_Type);
                    cmd.Parameters.AddWithValue("@Case12", Case12);
                    cmd.Parameters.AddWithValue("@Hp_Head", Hp_Head);
                    cmd.Parameters.AddWithValue("@Hd_Head", Hd_Head);
                    cmd.Parameters.AddWithValue("@Compare_Type", Compare_Type);
                    cmd.Parameters.AddWithValue("@Lab_Popultn_Type", Lab_Popultn_Type);
                    cmd.Parameters.AddWithValue("@Processed", Processed);
                    cmd.Parameters.AddWithValue("@LabKitId", LabKitID);
                    cmd.Parameters.AddWithValue("@FileGuid", fromFileGuid);
                    cmd.Connection = myConnection;

                    jobGuid = Guid.Parse(Convert.ToString(cmd.ExecuteScalar()));
                }

                if (Lab_Popultn_Type == "From File"
                    || (Lab_Popultn_Type == "" && fromFile != null))
                {
                    fromFile.AcceptChanges();
                    foreach (DataRow dr in fromFile.Rows)
                    {
                        Dictionary<string, string> val = new Dictionary<string, string>();

                        foreach (DataColumn dc in fromFile.Columns)
                            if (dc.ColumnName.ToUpper() != "GUID" && dc.ColumnName.ToUpper() != "ID")
                                val.Add(dc.ColumnName.ToString(), dr[dc.ColumnName].ToString());

                        res += InsertOrUpdateKnown("True", dr["ID"].ToString(), "From File", null, fromFileGuid, "Yes", val);
                    }
                }
            }
            catch (SqlException e)
            {
                res += "Save Case:" + e.Message;
            }
            finally
            {
                myConnection.Close();
            }

            return res;
        }
        public DataTable GetKnown_Profile(string strType, Guid guid)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandText = "spGetKnown";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@KnownType", strType);
                        cmd.Parameters.AddWithValue("@FromFileID", guid != Guid.Empty ? (object)guid : (object)DBNull.Value);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                CreateDailyLogEntry(e.Message);
                return null;
            }
            finally
            {
                myConnection.Close();
            }
        }
        public DataTable getAllelesFrequence(string locusName, string race, IEnumerable<string> evidenceAlleles)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "getAlleleFrequence";
                        cmd.Parameters.AddWithValue("@locus", locusName);
                        cmd.Parameters.AddWithValue("@race", race);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);

                        float defaultFrequency = Convert.ToSingle(myDT.Select("AlleleNo='0'")[0]["freq"]);

                        DataTable tab = myDT.Clone();

                        foreach (string unknown in evidenceAlleles)
                        {
                            DataRow dr = tab.NewRow();
                            dr["AlleleNo"] = unknown;

                            bool found = false;
                            foreach (DataRow drCommon in myDT.Rows)
                                if (drCommon["AlleleNo"].ToString() == unknown)
                                {
                                    dr["freq"] = drCommon["freq"];
                                    found = true;
                                    break;
                                }

                            if (unknown != "w")
                            {
                                if (!found)
                                    dr["freq"] = defaultFrequency;

                                tab.Rows.Add(dr);
                            }
                        }

                        return tab;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public DataTable getAllEthnics()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetAllEthnics";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }

        // case data related
        public DataTable GetLocusInOrder(Guid LabKitID)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetLocusInOrder";
                        cmd.Parameters.AddWithValue("@LabKitID", LabKitID != Guid.Empty ? (object)LabKitID : (object)null);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                CreateDailyLogEntry(e.Message);
                return null;
            }
            finally
            {
                myConnection.Close();
            }
        }
        public DataTable getLocusAlleles(string locusName)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    //spGetLocusAllele 
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetLocusAllele";
                        cmd.Parameters.AddWithValue("@locusName", locusName);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public DataTable getLocusSortOrder(Guid LabKitID)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    //spGetLocusAllele 
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetLocusSortOrder";
                        cmd.Parameters.AddWithValue("@LabKitID", LabKitID != Guid.Empty ? (object)LabKitID : (object)null);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }

        // admin related
        #region Edit Frequencies
        /// <summary>
        /// This function picks up all the alleles
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetAlleles(string strLocus)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetAlleles";
                        cmd.Parameters.AddWithValue("@locus", strLocus);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable GetFrequencyData()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetFrequencyData";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
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
            SqlConnection myConnection = null;
            string strResult = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spUpdateFrequencyData";
                    cmd.Parameters.AddWithValue("@FreqID", strFreqId);
                    cmd.Parameters.AddWithValue("@freq", strFreqNew);
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                strResult = "Update Frequency:" + e.Message;
            }
            finally
            {
                myConnection.Close();
            }
            return strResult;
        }
        #endregion
        #region View Submitted Job Status page related
        public DataTable GetSubmittedJobsInfo(string caseStatus)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetCasesByStatus";
                        cmd.Parameters.AddWithValue("@caseStatus", caseStatus);
                        myAdapter.SelectCommand = cmd;
                    }

                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public void DeleteCaseByRecordID(string recordID)
        {
            SqlConnection myConnection = null;
            try
            {

                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = myConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDeleteCaseByRecordID";
                    cmd.Parameters.AddWithValue("@RecordID", recordID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public void UpdateCaseByRecordID(string recordID, string caseStatus)
        {
            SqlConnection myConnection = null;
            try
            {

                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = myConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spUpdateCaseStatus";
                    cmd.Parameters.AddWithValue("@RecordID", recordID);
                    cmd.Parameters.AddWithValue("@caseStatus", caseStatus);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public int GetCaseCounterByStatus(string caseStatus)
        {
            SqlConnection myConnection = null;
            int caseCount = 0;

            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetCasesCountByStatus";
                        cmd.Parameters.AddWithValue("@caseStatus", caseStatus);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        if (myDT.Rows.Count > 0)
                            caseCount = Convert.ToInt32(myDT.Rows[0][0], CultureInfo.CurrentCulture);
                        return caseCount;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        #endregion
        #region Edit Profiles (Lab Types and Population)
        /// <summary>
        /// This function picks up all the Known Population based on the Races
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetMasterKnown(string knownType, string strEthnicID)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetMasterKnown";
                        cmd.Parameters.AddWithValue("@KnownType", knownType);
                        cmd.Parameters.AddWithValue("@EthnicID", (object)strEthnicID ?? (object)DBNull.Value);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up all the Ethnic data
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetEthnicData()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetEthnic";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up the Known Population Details
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetDetailKnown(string strID, string knownType)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDetailKnown";
                        cmd.Parameters.AddWithValue("@ID", strID);
                        cmd.Parameters.AddWithValue("@KnownType", knownType);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public string InsertOrUpdateKnown(string strInsertEdit, string ID, string knownType, string ethnicID, Guid fromFileID, string active, Dictionary<string, string> locusAllelesDict)
        {
            SqlConnection myConnection = null;
            string errMsg = "";

            StringBuilder locusAlleles = new StringBuilder();

            foreach (string locus in locusAllelesDict.Keys)
            {
                locusAlleles.Append(locus);
                locusAlleles.Append('=');
                locusAlleles.Append(locusAllelesDict[locus]);
                locusAlleles.Append('|');
            }

            string val = locusAlleles.ToString();
            val = val.Substring(0, val.Length - 1);

            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (strInsertEdit == "True")
                        cmd.CommandText = "dbo.spInsertKnown";
                    else
                        cmd.CommandText = "dbo.spUpdateKnown";
                    cmd.Parameters.AddWithValue("@ID", ID);
                    cmd.Parameters.AddWithValue("@KnownType", knownType);
                    cmd.Parameters.AddWithValue("@EthnicID", (object)ethnicID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FromFileID", fromFileID != Guid.Empty ? (object)fromFileID : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Active", active);
                    cmd.Parameters.AddWithValue("@LocusAlleles", val);
                    cmd.Parameters.AddWithValue("@errMsg", errMsg);
                    cmd.Parameters["@errMsg"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@errMsg"].Size = 8000;
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                    errMsg = cmd.Parameters["@errMsg"].Value.ToString();
                }
            }
            catch (SqlException e)
            {
                errMsg = e.Message;
                return errMsg;
            }
            finally
            {
                myConnection.Close();
            }
            return errMsg;
        }
        public string UpdateKnownStatus(string knownType, string strStatus, string ID, string ethnicID)
        {
            SqlConnection myConnection = null;
            string strResult = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spUpdateKnownStatus";
                    cmd.Parameters.AddWithValue("@KnownType", knownType);
                    cmd.Parameters.AddWithValue("@Active", strStatus);
                    cmd.Parameters.AddWithValue("@ID", (object)ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EthnicID", (object)ethnicID ?? (object)DBNull.Value);
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                strResult = "Update Known Status:" + e.Message;
            }
            finally
            {
                myConnection.Close();
            }
            return strResult;
        }
        internal void DeleteFromKnownByID(string knownType, string curID)
        {
            SqlConnection myConnection = null;
            try
            {

                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = myConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDeleteFromKnown";
                    cmd.Parameters.AddWithValue("@KnownType", knownType);
                    cmd.Parameters.AddWithValue("@ID", curID);
                    cmd.Parameters.AddWithValue("@FromFileID", DBNull.Value); // should be passed in if we ever add a "From File" delete
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        #endregion
        #region Edit Lab Kits Page and View Drop-Outs (also some Degraded Drop-Outs stuff, although this was disabled)
        // save lab kit data
        internal string SaveLabKit(string labKitName, string creator)
        {
            SqlConnection myConnection = null;
            string guid = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spInsertLabKit";
                    cmd.Parameters.AddWithValue("@LabKitName", labKitName);
                    cmd.Parameters.AddWithValue("@Creator", creator);
                    cmd.Parameters.AddWithValue("@Guid", guid);
                    cmd.Parameters["@Guid"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@Guid"].Size = 300;
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                    guid = cmd.Parameters["@guid"].Value.ToString();
                }
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                myConnection.Close();
            }
            return guid;
        }
        public string SaveLabKitData(string guid, DataTable dropOuts, DataTable dropIns, List<int> labKitLoci)
        {
            SqlConnection myConnection = null;
            string err = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                // save drop-outs
                SqlCommand insertDropOuts = new SqlCommand();
                insertDropOuts.Connection = myConnection;
                insertDropOuts.CommandText = "spInsertDropOutRate";
                insertDropOuts.CommandType = CommandType.StoredProcedure;
                insertDropOuts.Parameters.Add("@LabKitID", SqlDbType.UniqueIdentifier, int.MaxValue, "LabKitID");
                insertDropOuts.Parameters.Add("@LocusID", SqlDbType.Int, int.MaxValue, "LocusID");
                insertDropOuts.Parameters.Add("@TypeID", SqlDbType.Int, int.MaxValue, "TypeID");
                insertDropOuts.Parameters.Add("@DropOptionID", SqlDbType.Int, int.MaxValue, "DropOptionID");
                insertDropOuts.Parameters.Add("@DropOutRate", SqlDbType.Float, int.MaxValue, "DropOutRate");
                insertDropOuts.Parameters.Add("@NoOfPersonsInvolvd", SqlDbType.Int, int.MaxValue, "NoOfPersonsInvolvd");
                insertDropOuts.Parameters.Add("@Deducible", SqlDbType.VarChar, 50, "Deducible");

                SqlDataAdapter sda = new SqlDataAdapter();
                sda.InsertCommand = insertDropOuts;
                sda.Update(dropOuts);

                // save drop-ins
                SqlCommand insertDropIns = new SqlCommand();
                insertDropIns.Connection = myConnection;
                insertDropIns.CommandText = "spInsertDropInRate";
                insertDropIns.CommandType = CommandType.StoredProcedure;
                insertDropIns.Parameters.Add("@LabKitID", SqlDbType.UniqueIdentifier, int.MaxValue, "LabKitID");
                insertDropIns.Parameters.Add("@DropinRateID", SqlDbType.VarChar, 50, "DropInRateID");
                insertDropIns.Parameters.Add("@Type", SqlDbType.VarChar, 50, "Type");
                insertDropIns.Parameters.Add("@DropInRate", SqlDbType.Float, int.MaxValue, "DropInRate");

                SqlDataAdapter sda2 = new SqlDataAdapter();
                sda2.InsertCommand = insertDropIns;
                sda2.Update(dropIns);

                foreach (int locusID in labKitLoci)
                {
                    SqlCommand insertLabKitLoci = new SqlCommand();
                    insertLabKitLoci.Connection = myConnection;
                    insertLabKitLoci.CommandText = "spInsertLabKitLocus";
                    insertLabKitLoci.CommandType = CommandType.StoredProcedure;
                    insertLabKitLoci.Parameters.AddWithValue("@LabKitID", guid);
                    insertLabKitLoci.Parameters.AddWithValue("@LocusID", locusID);
                    insertLabKitLoci.ExecuteNonQuery();
                }
            }
            catch
            {
                return err;
            }
            finally
            {
                myConnection.Close();
            }
            return err;
        }
        // delete lab kit
        internal void DeleteLabKit(string Guid)
        {
            SqlConnection myConnection = null;
            try
            {

                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = myConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDeleteLabKit";
                    cmd.Parameters.AddWithValue("@LabKitID", Guid);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        // update lab kit data
        public string UpdateDropOutRates(string labKitID, string strDropoutRateID, string strDropOutRateNew)
        {
            SqlConnection myConnection = null;
            string strResult = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spUpdateDropOutRates";
                    cmd.Parameters.AddWithValue("@LabKitID", labKitID);
                    cmd.Parameters.AddWithValue("@DropoutRateID", strDropoutRateID);
                    cmd.Parameters.AddWithValue("@DropOutRate", strDropOutRateNew);
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                strResult = "Update DropOut Rates:" + e.Message;
            }
            finally
            {
                myConnection.Close();
            }
            return strResult;
        }
        public string UpdateDegradedDropOutRates(string strDropoutRateID, string strDropOutRateNew)
        {
            SqlConnection myConnection = null;
            string strResult = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spUpdateDegradedDropOutRates";
                    cmd.Parameters.AddWithValue("@DropoutRateID", strDropoutRateID);
                    cmd.Parameters.AddWithValue("@DropOutRate", strDropOutRateNew);
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                strResult = "Update Degraded DropOut Rates:" + e.Message;
            }
            finally
            {
                myConnection.Close();
            }
            return strResult;
        }
        public string UpdateDropInRates(string strID, string strDropInRateNew, string labKitID)
        {
            SqlConnection myConnection = null;
            string strResult = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spUpdateDropInRates";
                    cmd.Parameters.AddWithValue("@ID", strID);
                    cmd.Parameters.AddWithValue("@DropInRate", strDropInRateNew);
                    cmd.Parameters.AddWithValue("@LabKitID", labKitID);
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                strResult = "Update DropIn Rates:" + e.Message;
            }
            finally
            {
                myConnection.Close();
            }
            return strResult;
        }
        // get lab kit data
        public DataTable getDropOutOptions()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    //spGetLocusAllele 
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDropOutOptions";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up the no of persons involved in a case
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetNoOfPersonsInvolved()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetNoOfPersnsInvolvd";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up different probable types
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetDropOutTypes()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDropOutTypes";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up all the values of DropOuts
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDropOutData(string labKitID)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDropOutData";
                        cmd.Parameters.AddWithValue("@LabKitID", labKitID);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up all the values of Degraded DropOuts
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDegradedDropOutData()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDegradedDropOutData";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up all the values of DropIns
        /// Added by Dhruba
        /// </summary>
        /// <returns></returns>
        public DataTable GetDropInData(string labKitID)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDropInData";
                        cmd.Parameters.AddWithValue("@LabKitID", labKitID);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public DataTable getDropOutRate(string DropoutOptionBegin, string DropoutOptionEnd, int NoOfPersonsInvolvd, string strDeducible, string labKitID)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDropoutRate";
                        cmd.Parameters.AddWithValue("@dropoutOptionBegin", DropoutOptionBegin);
                        cmd.Parameters.AddWithValue("@dropoutOptionEnd", DropoutOptionEnd);
                        cmd.Parameters.AddWithValue("@NoOfPersonsInvolvd", NoOfPersonsInvolvd);
                        cmd.Parameters.AddWithValue("@Deducible", strDeducible);
                        cmd.Parameters.AddWithValue("@LabKitID", labKitID);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception("GetDropoutRate:" + e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public DataTable getDegradedDropOutRate(string DropoutOptionBegin, string DropoutOptionEnd, int NoOfPersonsInvolvd, string strDeducible, string strDegradedType)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDegradedDropoutRate";
                        cmd.Parameters.AddWithValue("@dropoutOptionBegin", DropoutOptionBegin);
                        cmd.Parameters.AddWithValue("@dropoutOptionEnd", DropoutOptionEnd);
                        cmd.Parameters.AddWithValue("@NoOfPersonsInvolvd", NoOfPersonsInvolvd);
                        cmd.Parameters.AddWithValue("@Deducible", strDeducible);
                        cmd.Parameters.AddWithValue("@Degradation_Type", strDegradedType);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception("GetDegradedDropoutRate:" + e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public DataTable getDropInRate(string labKitID)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetDropInData";
                        cmd.Parameters.AddWithValue("@LabKitID", labKitID);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception("GetDropinRate:" + e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        #endregion
        #region Scenario Management
        public DataTable GetCaseTypesTable()
        {
            SqlConnection myConnection = null;

            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetCaseTypesWithoutSeparators";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        public void UpdateCaseTypeStatusByID(string keyID, bool enabled)
        {
            SqlConnection myConnection = null;
            try
            {

                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = myConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spUpdateCaseTypeStatusByID";
                    cmd.Parameters.AddWithValue("@ID", keyID);
                    cmd.Parameters.AddWithValue("@Active", enabled);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        #endregion


        // testing related
        // UI related
        /// <summary>
        /// This function picks up all the different types of Compare Cases based on their status
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetCaseTypes()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetCaseTypes";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        // general
        public string getVersion()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    //spGetLocusAllele 
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetVersion";
                        return (string)cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up all the locus values
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetLocus(Guid LabKitID)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetLocus";
                        cmd.Parameters.AddWithValue("@LabKitID", LabKitID != Guid.Empty ? (object)LabKitID : (object)null);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This method gets the lab kits from the database as the name implies
        /// Anthony Nicholas Malczanek
        /// </summary>
        /// <returns>A DataTable with Lab Kits</returns>
        internal DataTable GetLabKits()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetLabKits";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up all the theta values based on their status
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetTheta()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetTheta";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// This function picks up all the races based on their status
        /// Added by Dhruba
        /// </summary>
        /// <param name="dropout"></param>
        /// <returns></returns>
        public DataTable GetRaces()
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetRaces";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }

        // service related
        public delegate void CreateDailyLogEntryDelegate(string entry);
        public event CreateDailyLogEntryDelegate OnCreateDailyLogEntry;
        private void CreateDailyLogEntry(string logEntry)
        {
            if (OnCreateDailyLogEntry != null) OnCreateDailyLogEntry(logEntry);
        }
        public DataTable GetEmailId(string strUserName)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MembershipSql"].ConnectionString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetEmailId";
                        cmd.Parameters.AddWithValue("@UserName", strUserName);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                CreateDailyLogEntry(e.Message);
                return null;
            }
            finally
            {
                myConnection.Close();
            }
        }
        public string UpdateCaseStatus(string strRecordID, string status)
        {
            SqlConnection myConnection = null;
            string strResult = "";
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spUpdateCaseStatus";
                    cmd.Parameters.AddWithValue("@RecordID", strRecordID);
                    cmd.Parameters.AddWithValue("@caseStatus", status);
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                strResult = "Update DropOut Rates:" + e.Message;
                CreateDailyLogEntry("Update DropOut Rates:" + e.Message);
            }
            finally
            {
                myConnection.Close();
            }
            return strResult;
        }
        public DataTable GetPendingCases()
        {
            SqlConnection myConnection = null;
            try
            {
                CreateDailyLogEntry(this.m_connString);
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetPendingCases";
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                CreateDailyLogEntry(e.Message);
                return null;
            }
            finally
            {
                myConnection.Close();
            }
        }
        internal DataTable GetPendingCase(string recordID)
        {
            SqlConnection myConnection = null;
            try
            {
                CreateDailyLogEntry(this.m_connString);
                myConnection = new SqlConnection(this.m_connString);
                using (SqlDataAdapter myAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = myConnection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetPendingCase";
                        cmd.Parameters.AddWithValue("@RecordID", recordID);
                        myAdapter.SelectCommand = cmd;
                    }
                    using (DataTable myDT = new DataTable())
                    {
                        myDT.Locale = CultureInfo.CurrentCulture;
                        myAdapter.Fill(myDT);
                        return myDT;
                    }
                }
            }
            catch (SqlException e)
            {
                CreateDailyLogEntry(e.Message);
                return null;
            }
            finally
            {
                myConnection.Close();
            }
        }

        // logging
        public void InsertLog(string username, string pageName, string severity, string type, string session, string extraInformation, string exceptionName, string exceptionMessage, string exceptionStackTrace)
        {
            SqlConnection myConnection = null;
            try
            {

                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = myConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInsertLog";
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@pageName", pageName);
                    cmd.Parameters.AddWithValue("@severity", severity);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@session", session);
                    if (extraInformation == null) cmd.Parameters.AddWithValue("@extraInformation", DBNull.Value);
                    else cmd.Parameters.AddWithValue("@extraInformation", extraInformation);
                    if (exceptionName == null) cmd.Parameters.AddWithValue("@exceptionName", DBNull.Value);
                    else cmd.Parameters.AddWithValue("@exceptionName", exceptionName);
                    if (exceptionMessage == null) cmd.Parameters.AddWithValue("@exceptionMessage", DBNull.Value);
                    else cmd.Parameters.AddWithValue("@exceptionMessage", exceptionMessage);
                    if (exceptionStackTrace == null) cmd.Parameters.AddWithValue("@exceptionStackTrace", DBNull.Value);
                    else cmd.Parameters.AddWithValue("@exceptionStackTrace", exceptionStackTrace);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                //throw new Exception(e.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }

        // testing
        public void WriteTest(Guid testID, Guid subTestID, string testName, string comparisonID, int comparisonType, bool testManualEntry, bool testBulk, string LabKitID, int dnaEvidenceAmount)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spWriteTest";

                    cmd.Parameters.AddWithValue("@ID", testID);
                    cmd.Parameters.AddWithValue("@SubTestID", subTestID); 
                    cmd.Parameters.AddWithValue("@TestName", testName);
                    cmd.Parameters.AddWithValue("@ComparisonID", comparisonID);
                    cmd.Parameters.AddWithValue("@ComparisonType", comparisonType);
                    cmd.Parameters.AddWithValue("@Manual", testManualEntry);
                    cmd.Parameters.AddWithValue("@Bulk", testBulk);
                    cmd.Parameters.AddWithValue("@LabKitID", LabKitID);
                    cmd.Parameters.AddWithValue("@DNAEvidenceAmount", dnaEvidenceAmount);
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
            }
            finally
            {
                myConnection.Close();
            }
        }
        public void WriteTestResults(string subTestId, string comparisonId, string Asian, string Black, string Caucasian, string Hispanic)
        {
            SqlConnection myConnection = null;
            try
            {
                myConnection = new SqlConnection(this.m_connString);
                myConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.spWriteTestResults";

                    cmd.Parameters.AddWithValue("@SubTestID", subTestId);
                    cmd.Parameters.AddWithValue("@ComparisonID", comparisonId);
                    cmd.Parameters.AddWithValue("@Asian", double.Parse(Asian));
                    cmd.Parameters.AddWithValue("@Black", double.Parse(Black));
                    cmd.Parameters.AddWithValue("@Caucasian", double.Parse(Caucasian));
                    cmd.Parameters.AddWithValue("@Hispanic", double.Parse(Hispanic));
                    cmd.Connection = myConnection;
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
            }
            finally
            {
                myConnection.Close();
            }
        }
    }
}

