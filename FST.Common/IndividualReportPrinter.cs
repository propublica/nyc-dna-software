using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Reporting.WinForms;
using System.Data;
using System.Windows.Forms;

namespace FST.Common
{
    public class IndividualReportPrinter
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
        /// Default constructor.
        /// </summary>
        /// <param name="outputPath">The path where the report files are written. 
        /// This must obviously be set to writeable for both the ASP.Net application pool user account and the Windows Service user account.
        /// </param>
        /// <param name="templatePath">The path to the template RDLC file which is the base for our PDF report.</param>
        public IndividualReportPrinter(string outputPath, string templatePath)
        {
            this.outputPath = outputPath;
            this.templatePath = templatePath;
        }

        /// <summary>
        /// This method takes the DataSet from the ComparisonData.Print() method, and passes it directly to ReportViewer. The RDLC template is
        /// specifically written with this DataSet schema in mind.
        /// </summary>
        /// <param name="comparisonData">Comparison Data structure holding comparison information. See ComparisonData in FST.Common</param>
        /// <returns>A string path to the saved report.</returns>
        public string Print(ComparisonData comparisonData)
        {
            // make ourselves a new instance of ReportViewer
            ReportViewer ReportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();

            /// generate our filename
            string username = comparisonData.UserName.Substring(comparisonData.UserName.IndexOf('\\') + 1);
            string filename = outputPath + "FSTReport_" + username + "_" + DateTime.Now.Year.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Month.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Day.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Hour.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Minute.ToString().PadLeft(2, '0');
            filename += DateTime.Now.Second.ToString().PadLeft(2, '0');
            filename += ".pdf";

            // this is where we call the ComparisonData.Print() method. in there is where all the magic happens
            DataSet dsPrint = comparisonData.Print();

            // set our ReportViewer data sources
            ReportDataSource datasource = new ReportDataSource("RSDataSet_tblParameter", dsPrint.Tables["tblParameters"]);
            ReportViewer1.LocalReport.DataSources.Add(datasource);
            datasource = new ReportDataSource("RSDataSet_tblAlleles", dsPrint.Tables["tblAlleles"]);
            ReportViewer1.LocalReport.DataSources.Add(datasource);
            datasource = new ReportDataSource("RSDataSet_tblComparisonResult", dsPrint.Tables["tblResults"]);
            ReportViewer1.LocalReport.DataSources.Add(datasource);

            // set the template path
            ReportViewer1.LocalReport.ReportPath = templatePath;

            // FST version parameter
            ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("FST_VERSION", comparisonData.Version), new ReportParameter("LABKITNAME", comparisonData.LabKitName) });

            // these two lines are for the ASP.Net code... if we use the ReportViewer from ASP.Net and the RDLC includes string concatentation
            // expressions such as ="Blah" + Whatever!Fields.Value then we get #Error in the result if we don't call these two lines.
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            // fill out the report template
            ReportViewer1.LocalReport.Refresh();
            byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            /// UGLY HACK: we hide the second page from the PDF report for the Identifiler kit that doesn't have all the loci... I am sorry for doing this, but it works
            List<int> PageIndices = new List<int>(bytes.Locate(ASCIIEncoding.Default.GetBytes("/Pages ")));
            List<int> CountIndices = new List<int>(bytes.Locate(ASCIIEncoding.Default.GetBytes("/Count ")));

            // if we found an instance of Pages (and we should) and we're doing the Identifiler Lab Kit with loci that all fit on the first page
            if (PageIndices.Count > 1 && comparisonData.LabKitName == "Identifiler")
            {
                // find the first instance of Count that's greater than the first instance of Page
                int PageIndex = PageIndices[0];
                int CountIndex = -1;
                foreach (int idx in CountIndices)
                    if (idx > PageIndex)
                    {
                        CountIndex = idx;
                        continue;
                    }

                CountIndex += "/Count ".Length;

                // set our page Count to 1 (has to be an ASCII 1)
                if (CountIndex > 0)
                    bytes[CountIndex] = (byte)'1';
            }
            /// 

            System.IO.File.WriteAllBytes(filename, bytes);

            return filename;
        }
    }

    /// <summary>
    /// This helper class is used to find patterns in byte arrays. Found on stack overflow. Thank you, Internet!
    /// </summary>
    static class ByteArrayHelper
    {
        static readonly int[] Empty = new int[0];

        public static int[] Locate(this byte[] self, byte[] candidate)
        {
            if (IsEmptyLocate(self, candidate))
                return Empty;

            var list = new List<int>();

            for (int i = 0; i < self.Length; i++)
            {
                if (!IsMatch(self, i, candidate))
                    continue;

                list.Add(i);
            }

            return list.Count == 0 ? Empty : list.ToArray();
        }

        static bool IsMatch(byte[] array, int position, byte[] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
                if (array[position + i] != candidate[i])
                    return false;

            return true;
        }

        static bool IsEmptyLocate(byte[] array, byte[] candidate)
        {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }
    }
}
