using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Threading;

namespace FST.Common
{
    public class BulkReportPrinter
    {
        /// <summary>
        /// Stores the path where the report files are written. This must obviously be set to writeable for both the ASP.Net application pool user account and the Windows Service user account.
        /// </summary>
        string outputPath;
        /// <summary>
        /// Stores the path to the template Excel file which is the base for our final report. Read the Print() method for information on how this template is used esp. w/r/t dynamic locus columns.
        /// </summary>
        string templatePath;
        /// <summary>
        /// This is used to make sure reports write one at a time.
        /// </summary>
        static object lockWriting = new Object();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="outputPath">The path where the report files are written. 
        /// This must obviously be set to writeable for both the ASP.Net application pool user account and the Windows Service user account.
        /// </param>
        /// <param name="templatePath">The path to the template Excel file which is the base for our final report. 
        /// Read the Print() method for information on how this template is used esp. w/r/t dynamic locus columns.
        /// </param>
        public BulkReportPrinter(string outputPath, string templatePath)
        {
            this.outputPath = outputPath;
            this.templatePath = templatePath;
        }

        /// <summary>
        /// This method uses the Results dictionary, Known Profiles table, and Comparison Data structure to fill out the template passed as this class' constructor.
        /// </summary>
        /// <param name="Results">Dictionary holding results from the bulk comparison. See FST_WindowsService.</param>
        /// <param name="dtKnownProfile">Data Table holding profiles used in the bulk comparison. See FST_WindowsService and GetKnown_Profile in Business_Interface in FST.Common.</param>
        /// <param name="comparisonData">Comparison Data structure holding comparison information. See ComparisonData in FST.Common</param>
        /// <returns>A string path to the saved report.</returns>
        public string Print(Dictionary<string, Dictionary<string, float>> Results, DataTable dtKnownProfile, ComparisonData comparisonData)
        {
            // READ THIS: there are quite a few assumptions made in this code. you might want to read through the comments to understand
            // what is going on. specifically, you will want to look at how the column names from Excel are mapped to the first row as column names (see dtSchema and colMap)
            // also, you might want to see how we generate the locus columns dynamically in the third sheet, and then read them back.
            // you may also want to look at how we grab the last write time for the output file, then we check to see if it has been written to, and whether it's locked.
            // also, there is an assumption about the columns which are present in the template, and their order (see the DELETE statements)

            // this returns the printed dataset from Comparison Data. check method for details on how we prepare data for printing.
            DataSet dsPrint = comparisonData.Print();

            DateTime bulkLastWriteTime;
            int retries = 0;

            #region Generate File Path
            // here we generate a filename for the final report based on the username and the current time.
            // there may be a false assumption here that no user will finish two comparisons in the same second.
            string strUserName = comparisonData.UserName.Substring(comparisonData.UserName.LastIndexOf('\\') + 1);

            string filename = outputPath + "FSTReport_" + strUserName + "_" + DateTime.Now.Year.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Month.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Day.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Hour.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Minute.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Second.ToString().PadLeft(2, '0');
            filename += ".xlsx";
            #endregion
            lock (lockWriting)
            {
                #region Prepare File
                // get a copy of the template and store the last time it was written to. the last time it was written to is used to see if the ACE OLEDB buffer has flushed.
                File.Copy(templatePath, filename);
                bulkLastWriteTime = File.GetLastWriteTime(filename);
                #endregion
                #region Connection String
                string connString = string.Empty;
                connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties='Excel 12.0;HDR=No';");
                #endregion

                // open the file
                using (OleDbConnection cn = new OleDbConnection(connString))
                {
                    #region Prepare Some Variables
                    OleDbCommand cmd;                       // recycled command refernce, used quite a lot below
                    cn.Open();
                    string query = string.Empty;            // recycled query string for the command above
                    string sheetName = string.Empty;        // this is the table name in the query and the sheet name in Excel (with $ appended)

                    // this table is used to retrieve the schema of the current table. 
                    // we do not bring the data in with headers because we need to insert new columns for the dynamically generated locus columns
                    DataTable dtSchema;
                    Dictionary<string, string> colMap;      // this is used to map the friendly column names to Excel column names F1,F2,F3,...,Fn
                    #endregion
                    sheetName = "Results$"; // we switch to the Results table here
                    #region DELETE from Results
                    // delete everything on the sheet except the first column (hence checking where F1 <> 'ID')
                    // our assumption is that this is the first column. if this changes, this code must change
                    query = "DELETE FROM [" + sheetName + "] WHERE F1 <> 'ID'";
                    cmd = new OleDbCommand(query, cn);
                    cmd.ExecuteNonQuery();
                    #endregion
                    #region Get Column Mapping
                    // we get the schema and column map for the current sheet/table
                    dtSchema = getTable(cn, sheetName);
                    colMap = getColumnMap(dtSchema);
                    #endregion
                    #region INSERT Results
                    // iterate over the results and insert them into the table. 
                    // it is assumed that our columns in the template and our races in the database will match.
                    // if there is ever a need for dynamic race column generation we could create those too like the loci below
                    query = "INSERT INTO [" + sheetName + "] (" + getColumns(colMap) + ") VALUES (" + getColumnNames(colMap) + ")";

                    foreach (string key in Results.Keys)
                    {
                        cmd = new OleDbCommand(query, cn);
                        cmd.Parameters.AddWithValue("@ID", key);
                        foreach (string race in Results[key].Keys)  // we assume the races match those in the spreadsheet here
                            cmd.Parameters.AddWithValue("@" + race, (Results[key][race] < 0.01 || Results[key][race] > 1000000 ? String.Format("{0:#.##e+00}", Results[key][race]) : Convert.ToString(Math.Round(Results[key][race], 2))));
                        cmd.ExecuteNonQuery();
                    }
                    #endregion
                    sheetName = "Parameters$"; // we switch to the Parameters table here
                    #region DELETE from Parameters
                    // delete everything from the sheet except the first column (hence checking where F1 <> 'FB1')
                    // our assumption is that this is the first column. if this changes, this code must change
                    query = "DELETE FROM [" + sheetName + "] WHERE F1 <> 'FB1'";
                    cmd = new OleDbCommand(query, cn);
                    cmd.ExecuteNonQuery();
                    #endregion
                    #region Get Column Mapping
                    // we get the schema and column map for the current sheet/table
                    dtSchema = getTable(cn, sheetName);
                    colMap = getColumnMap(dtSchema);
                    #endregion
                    #region INSERT Parameters
                    // use the parameters from the Comparison Data structure to fill out the report parameters sheet
                    // it is assumed that at least these columns exist. if they do not, this code will error.
                    query = "INSERT INTO [" + sheetName + "] (" + getColumns(colMap) + ") VALUES (" + getColumnNames(colMap) + ")";

                    cmd = new OleDbCommand(query, cn);
                    cmd.Parameters.AddWithValue("@FB1", comparisonData.FB1);
                    cmd.Parameters.AddWithValue("@FB2", comparisonData.FB2);
                    cmd.Parameters.AddWithValue("@Item", comparisonData.Item);
                    cmd.Parameters.AddWithValue("@Comparison", comparisonData.Comparison);
                    cmd.Parameters.AddWithValue("@DNA_Template_Amount", comparisonData.DNAAmount);
                    cmd.Parameters.AddWithValue("@Created_By", comparisonData.UserName);
                    cmd.Parameters.AddWithValue("@Prosecutorial_Hypothesis", comparisonData.HpHead);
                    cmd.Parameters.AddWithValue("@Defense_Hypothesis", comparisonData.HdHead);
                    cmd.Parameters.AddWithValue("@Deducible", comparisonData.Deducible ? "Yes" : "No");
                    cmd.Parameters.AddWithValue("@Lab_Kit", comparisonData.LabKitName);
                    cmd.ExecuteNonQuery();
                    #endregion
                    sheetName = "Profiles$"; // we switch to the Parameters table here
                    #region DELETE from Profiles
                    // delete everything from the sheet except the first column (hence checking where F1 <> 'Type')
                    // our assumption is that this is the first column. if this changes, this code must change
                    query = "DELETE FROM [" + sheetName + "] WHERE F1 <> 'Type'";
                    cmd = new OleDbCommand(query, cn);
                    cmd.ExecuteNonQuery();
                    #endregion
                    #region ADD COLUMNS from Lab Kit Loci
                    // get the loci based on the lab kit from the database based on the lab kit used in the lab kit ID
                    DataTable dtLoci = new Business_Interface().GetLocus(comparisonData.LabKitID);

                    // get the schema for the table so we can write the locus columns from the database (above)
                    DataTable dtSchemaTemp = getTable(cn, sheetName);
                    int firstRow = 0;
                    for (int i = 0; i < dtSchemaTemp.Columns.Count; i++)
                        if (dtSchemaTemp.Rows[0][i].ToString().Trim() == string.Empty) // if we find the first column with no header we can start inserting loci here
                        {
                            firstRow = i;
                            break;
                        }

                    // starting at the row we found above we go through and add the first row with the locus names
                    for (int i = firstRow; i < dtLoci.Rows.Count + firstRow; i++)
                    {
                        string locusName = dtLoci.Rows[i - firstRow]["FieldName"].ToString();
                        query = "UPDATE [" + sheetName + "] SET F" + (i + 1) + "='" + locusName + "' WHERE F1 = 'Type'";
                        cmd = new OleDbCommand(query, cn);
                        cmd.ExecuteNonQuery();
                    }
                    #endregion
                    #region Get Column Mapping (after adding new ones)
                    // we get the schema and column map for the current sheet/table again. this is after we wrote our locus columns from the DB
                    dtSchema = getTable(cn, sheetName);
                    colMap = getColumnMap(dtSchema);
                    #endregion
                    #region INSERT Evidence
                    // we use the Evidence dictionary from the ComparisonData structure to fill out the first 3 rows of the profiles table, one per replicate
                    query = "INSERT INTO [" + sheetName + "] (" + getColumns(colMap) + ") VALUES (" + getColumnNames(colMap) + ")";

                    int cnt = 1;
                    // here we start 3 rows from the bottom of the tblAlleles table. this is where the evidence gets printed. 3 is for 3 replicates.
                    for (int i = dsPrint.Tables["tblAlleles"].Rows.Count - 3; i < dsPrint.Tables["tblAlleles"].Rows.Count; i++)
                    {
                        cmd = new OleDbCommand(query, cn);
                        cmd.Parameters.AddWithValue("@Type", "Evidence");
                        cmd.Parameters.AddWithValue("@FileName", comparisonData.EvidenceFileName ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ID", cnt++);

                        foreach (DataRow dr in dtLoci.Rows)
                        {
                            string strLocus = dr["FieldName"].ToString();
                            string strColumnLocus = strLocus.Replace(" ", "_"); // here we replace spaces in column names with underscores so they fit as parameter names to our command (see getColumnMap)
                            cmd.Parameters.AddWithValue("@" + strColumnLocus, dsPrint.Tables["tblAlleles"].Rows[i][strLocus.ToUpper()].ToString());
                        }
                        cmd.ExecuteNonQuery();
                    }
                    #endregion
                    #region INSERT Blank Row
                    // we insert a blank row to separate the Evidence from the Known profiles
                    cmd = new OleDbCommand(query, cn);
                    cmd.Parameters.AddWithValue("@Type", ' ');
                    cmd.Parameters.AddWithValue("@FileName", ' ');
                    cmd.Parameters.AddWithValue("@ID", ' ');
                    foreach (DataRow dr in dtLoci.Rows)
                    {
                        string strLocus = dr["FieldName"].ToString();
                        string strColumnLocus = strLocus.Replace(" ", "_"); // replace spaces in column names with underscores so they work as parameters (see getColumnMap)
                        cmd.Parameters.AddWithValue("@" + strColumnLocus, ' ');
                    }
                    cmd.ExecuteNonQuery();
                    #endregion
                    #region INSERT Known Alleles
                    // here we iterate through all the Known profiles in the KnownAlleles dictionary of the ComparisonData structure. index starts at 1, and we may have up to 4.
                    for (int i = 1; i <= comparisonData.KnownsAlleles.Count; i++)
                    {
                        cmd = new OleDbCommand(query, cn);
                        cmd.Parameters.AddWithValue("@Type", "Known");
                        // here we get the known profile uploaded file name (if any) and the custom name entered for this profile (if any)
                        switch (i)
                        {
                            case 1:
                                cmd.Parameters.AddWithValue("@FileName", comparisonData.Known1FileName ?? string.Empty);
                                cmd.Parameters.AddWithValue("@ID", comparisonData.Known1Name ?? string.Empty);
                                break;
                            case 2:
                                cmd.Parameters.AddWithValue("@FileName", comparisonData.Known2FileName ?? string.Empty);
                                cmd.Parameters.AddWithValue("@ID", comparisonData.Known2Name ?? string.Empty);
                                break;
                            case 3:
                                cmd.Parameters.AddWithValue("@FileName", comparisonData.Known3FileName ?? string.Empty);
                                cmd.Parameters.AddWithValue("@ID", comparisonData.Known3Name ?? string.Empty);
                                break;
                            case 4:
                                cmd.Parameters.AddWithValue("@FileName", comparisonData.Known4FileName ?? string.Empty);
                                cmd.Parameters.AddWithValue("@ID", comparisonData.Known4Name ?? string.Empty);
                                break;
                        }

                        // we go through each one of the loci in the lab kit, and we take that value from the known profile and add its value to the parameters for this command
                        foreach (DataRow dr in dtLoci.Rows)
                        {
                            string strLocus = dr["FieldName"].ToString();
                            string strColumnLocus = strLocus.Replace(" ", "_");
                            cmd.Parameters.AddWithValue("@" + strColumnLocus, comparisonData.KnownsAlleles[i][strLocus.ToUpper()].ToString());
                        }
                        cmd.ExecuteNonQuery();
                    }
                    #endregion
                    #region INSERT Blank Row
                    // we insert a blank row to separate the Known profiles from the Comparison profiles
                    cmd = new OleDbCommand(query, cn);
                    cmd.Parameters.AddWithValue("@Type", ' ');
                    cmd.Parameters.AddWithValue("@FileName", ' ');
                    cmd.Parameters.AddWithValue("@ID", ' ');
                    foreach (DataRow dr in dtLoci.Rows)
                    {
                        string strLocus = dr["FieldName"].ToString();
                        string strColumnLocus = strLocus.Replace(" ", "_");
                        cmd.Parameters.AddWithValue("@" + strColumnLocus, ' ');
                    }
                    cmd.ExecuteNonQuery();
                    #endregion
                    #region INSERT Bulk Comparison Profiles
                    // here we iterate through the dtKnownProfile table (which actually holds our comparisons) and we add them to the parameters for the insert command
                    foreach (DataRow dr in dtKnownProfile.Rows)
                    {
                        cmd = new OleDbCommand(query, cn);
                        cmd.Parameters.AddWithValue("@Type", "Comparison");
                        cmd.Parameters.AddWithValue("@FileName", comparisonData.BulkType == ComparisonData.enBulkType.FromFile ? (comparisonData.FromFileName != string.Empty ? comparisonData.FromFileName : "From File") : comparisonData.BulkType == ComparisonData.enBulkType.LabTypes ? "Lab Types" : "Population");
                        cmd.Parameters.AddWithValue("@ID", dr["ID"].ToString());
                        foreach (DataRow drLocus in dtLoci.Rows)
                        {
                            string strLocus = drLocus["FieldName"].ToString();
                            string strColumnLocus = strLocus.Replace(" ", "_"); // here we replace a space with an underscore so the locus name makes a valid column name
                            cmd.Parameters.AddWithValue("@" + strColumnLocus, dr[strLocus]);
                        }

                        cmd.ExecuteNonQuery();
                    }
                    #endregion

                    cn.Close();
                    cn.Dispose();
                    GC.Collect();
                }
            }

            // we check for the writing ot have started by waiting for the last write time to be different from the template last write time
            while (bulkLastWriteTime >= File.GetLastWriteTime(filename) && ++retries < 300)
            {
                GC.Collect();   // do not remove this, the ACE OLEDB has GC issues and invoking the collector causes it to flush its buffer to disk
                Thread.Sleep(1000);
            }

            // give ACE OLEDB some time to write
            bulkLastWriteTime = File.GetLastWriteTime(filename);
            retries = 0;
            do Thread.Sleep(15000);
            while (File.GetLastWriteTime(filename) > bulkLastWriteTime && ++retries < 4);

            // we check for whether we can send out the file by seeing if we can write to the file
            retries = 0;
            while (IsFileLocked(new FileInfo(filename)) && ++retries < 10)
            {
                GC.Collect();   // do not remove this, the ACE OLEDB has GC issues and invoking the collector causes it to flush its buffer to disk
                Thread.Sleep(1000);
            }

            return filename;
        }

        /// <summary>
        /// Check if a certain file is locked by try/catch'ing a file open with exclusive write access
        /// </summary>
        /// <param name="file">FileInfo class describing the file we are checking</param>
        /// <returns>Boolean value that describes whether we are unable to gain exclusive access to the file</returns>
        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        /// <summary>
        /// Returns a datatable based on a table name and a connection
        /// </summary>
        /// <param name="con">Connection to the Excel document</param>
        /// <param name="tableName">Worksheet name we are looking for (MUST have $ appended)</param>
        /// <returns>DataTable with the values filled</returns>
        private DataTable getTable(OleDbConnection con, string tableName)
        {
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + tableName + "]", con);
            da.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Gets us a dictionary filled with column names from Excel and their mapping to the names in the first row. 
        /// The column names are preceeded by an at sign ('@') and spaces (' ') are substituted with an underscore ('_') for parameterization.
        /// </summary>
        /// <param name="dtSchema">DataTable with at least the first row of the sheet</param>
        /// <returns>Dictionary mapping string column names to string names in the first row</returns>
        private Dictionary<string, string> getColumnMap(DataTable dtSchema)
        {
            Dictionary<string, string> val = new Dictionary<string, string>();
            foreach (DataColumn dc in dtSchema.Columns)
                if (dtSchema.Rows[0][dc.ColumnName].ToString().Trim() != string.Empty)
                    val.Add("@" + dtSchema.Rows[0][dc.ColumnName].ToString().Replace(" ", "_"), dc.ColumnName); // here we replace spaces with _'s and prepend an @ for parameterization
            return val;
        }

        /// <summary>
        /// Generates a list of the parameter names of columns in the column map separated by commas. Used to pass as the paramters list to the commands sent to the ACE OLEDB Excel access component
        /// </summary>
        /// <param name="colMap">Dictionary with a map of Excel columns to first row of data (parameter names of columns)</param>
        /// <returns>A list of the parameter names of columns in the column map separated by commas.</returns>
        private string getColumns(Dictionary<string, string> colMap)
        {
            StringBuilder val = new StringBuilder();
            foreach (string key in colMap.Keys)
            {
                val.Append(colMap[key]);
                val.Append(',');
            }
            val.Remove(val.Length - 1, 1);
            return val.ToString();
        }

        /// <summary>
        /// Generates a list of the Excel columns in the column map separated by commas. Used to pass as the column names to the commands sent to the ACE OLEDB Excel access component
        /// </summary>
        /// <param name="colMap">Dictionary with a map of Excel columns to first row of data (parameter names of columns)</param>
        /// <returns>A list of the Excel columns in the column map separated by commas.</returns>
        private string getColumnNames(Dictionary<string, string> colMap)
        {
            StringBuilder val = new StringBuilder();
            foreach (string key in colMap.Keys)
            {
                val.Append(key);
                val.Append(',');
            }
            val.Remove(val.Length - 1, 1);
            return val.ToString();
        }
    }
}
