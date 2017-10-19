using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Threading;
using FST.Common;
using System.Configuration;

namespace FST.Web.Admin
{
    public partial class frmTestCenter : System.Web.UI.Page
    {
        FST.Common.Business_Interface bi = new FST.Common.Business_Interface();
        DataTable dtCaseTypes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dtCaseTypes = bi.GetCaseTypesTable();
                chkComparisonTypes.DataSource = dtCaseTypes;
                chkComparisonTypes.DataBind();
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Literal1.Text = (lastUpdated != DateTime.MinValue ? lastUpdated.ToString() : string.Empty);
        }

        protected void btnRunTest_Click(object sender, EventArgs e)
        {
            throw new Exception("Old code is gone. Get it from TFS to run the old tests. Revert to version checked in 2013-04-10 in the EST morning.");
            //#region initialize the page sessions (needs to be done so the pages have a session handle in the subthread)
            //FST.Web.Default2 page1n = new Default2();
            //var s = page1n.Session;
            //FST.Web.frmBulkSuspTest1 page1b = new frmBulkSuspTest1();
            //s = page1b.Session;
            //frmSelectCase2 page2n = new frmSelectCase2();
            //s = page2n.Session;
            //FST.Web.frmBulkSuspTest2 page2b = new frmBulkSuspTest2();
            //s = page2b.Session;
            //frmSelectCase3 page3n = new frmSelectCase3();
            //s = page3n.Session;
            //FST.Web.frmBulkSuspTest3 page3b = new frmBulkSuspTest3();
            //s = page3b.Session;
            //frmSelectCase8 page8n = new frmSelectCase8();
            //s = page8n.Session;
            //FST.Web.frmBulkSuspTest8 page8b = new frmBulkSuspTest8();
            //s = page8b.Session;
            //frmSelectCase10 page10n = new frmSelectCase10();
            //s = page10n.Session;
            //FST.Web.frmBulkSuspTest10 page10b = new frmBulkSuspTest10();
            //s = page10b.Session;
            //frmSelectCase11 page11n = new frmSelectCase11();
            //s = page11n.Session;
            //FST.Web.frmBulkSuspTest11 page11b = new frmBulkSuspTest11();
            //s = page11b.Session;
            //#endregion

            //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            //{
            //    testName = txtTestName.Text;
            //    testIndividual = chkIndividual.Checked;
            //    testBulk = chkBulk.Checked;
            //    testManualEntry = rbManualEntry.Checked;
            //    deducible = rbDeducible.Checked;
            //    LabKitID = bi.GetLabKits().Rows[0]["FieldValue"].ToString();

            //    foreach (ListItem liComparisonType in chkComparisonTypes.Items)
            //        if (liComparisonType.Selected)
            //            comparisonTypes.Add(Convert.ToInt32(liComparisonType.Value));

            //    comparisonProfiles = bi.GetProfilesFromFile(this.fuComparisonProfiles, Server.MapPath("~/Admin/Upload/"));
            //    known1Profile = bi.GetProfilesFromFile(this.fuKnown1Profile, Server.MapPath("~/Admin/Upload/"));
            //    known2Profile = bi.GetProfilesFromFile(this.fuKnown2Profile, Server.MapPath("~/Admin/Upload/"));
            //    known3Profile = bi.GetProfilesFromFile(this.fuKnown3Profile, Server.MapPath("~/Admin/Upload/"));
            //    evidence = GetEvidenceFromFile(this.fuEvidence, Server.MapPath("~/Admin/Upload/"));
            //    saveTableEvidence = GetSaveTableEvidence(evidence);

            //    Known2Alleles = new Dictionary<string, Dictionary<int, string>>();

            //    EvidenceAlleles = new Dictionary<string, Dictionary<int, string>>();
            //    ReadEvidenceAlleles(evidence, EvidenceAlleles);

            //    testID = Guid.NewGuid();
            //    foreach (int comparisonType in comparisonTypes)
            //    {
            //        Session["CompareMethod"] = comparisonType.ToString();
            //        switch (comparisonType)
            //        {
            //            #region cases 1,5,6,13,14,17,21
            //            case 1:
            //            case 5:
            //            case 6:
            //            case 13:
            //            case 14:
            //            case 17:
            //            case 21:
            //                if (testIndividual)
            //                {
            //                    foreach (DataRow dr in comparisonProfiles.Rows)
            //                    {
            //                        KnownAlleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(dr, KnownAlleles, 1);
            //                        saveTableComparison = GetSaveTable(dr, "Suspect");

            //                        if (testManualEntry)
            //                        {
            //                            dnaEvidenceAmount = 111;
            //                            subTestID = Guid.NewGuid();
            //                            comparisonID = dr["ID"].ToString();

            //                            WriteTest(testID, subTestID, testName, comparisonID, comparisonType, true, false, LabKitID, dnaEvidenceAmount);

            //                            page1n.btnCompare_Click(this, new EventArgs());
            //                            lastUpdated = DateTime.Now;
            //                        }
            //                        else
            //                        {
            //                        }
            //                    }
            //                }
            //                if (testBulk)
            //                {
            //                    if (testManualEntry)
            //                    {
            //                        dnaEvidenceAmount = 111;
            //                        subTestID = Guid.NewGuid();

            //                        foreach (DataRow dr in comparisonProfiles.Rows)
            //                            WriteTest(testID, subTestID, testName, dr["ID"].ToString(), comparisonType, true, true, LabKitID, dnaEvidenceAmount);


            //                        page1b.btnCompare_Click(this, new EventArgs());
            //                        if (saveTableComparison != null && saveTableComparison.Columns.Contains("guid"))
            //                            saveTableComparison.Columns.Remove("guid");
            //                        if (comparisonProfiles.Columns.Contains("guid"))
            //                            comparisonProfiles.Columns.Remove("guid");
            //                        lastUpdated = DateTime.Now;
            //                    }
            //                    else
            //                    {
            //                    }
            //                }
            //                break;
            //            #endregion
            //            #region cases 2,7,15
            //            case 2:
            //            case 7:
            //            case 15:
            //                if (testIndividual)
            //                {
            //                    Known2Alleles = new Dictionary<string, Dictionary<int, string>>();
            //                    ReadComparison(known1Profile.Rows[0], Known2Alleles, 1);
            //                    saveTableKnown1 = GetSaveTable(known1Profile.Rows[0], "Victim");

            //                    foreach (DataRow dr in comparisonProfiles.Rows)
            //                    {
            //                        KnownAlleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(dr, KnownAlleles, 1);
            //                        saveTableComparison = GetSaveTable(dr, "Suspect");


            //                        if (testManualEntry)
            //                        {
            //                            dnaEvidenceAmount = 111;
            //                            subTestID = Guid.NewGuid();
            //                            comparisonID = dr["ID"].ToString();

            //                            WriteTest(testID, subTestID, testName, comparisonID, comparisonType, true, false, LabKitID, dnaEvidenceAmount);

            //                            page2n.btnCompare_Click(this, new EventArgs());
            //                            lastUpdated = DateTime.Now;
            //                        }
            //                        else
            //                        {
            //                        }
            //                    }
            //                }
            //                if (testBulk)
            //                {
            //                    if (testManualEntry)
            //                    {
            //                        Known2Alleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(known1Profile.Rows[0], Known2Alleles, 1);
            //                        saveTableKnown1 = GetSaveTable(known1Profile.Rows[0], "Victim");

            //                        dnaEvidenceAmount = 111;
            //                        subTestID = Guid.NewGuid();

            //                        foreach (DataRow dr in comparisonProfiles.Rows)
            //                            WriteTest(testID, subTestID, testName, dr["ID"].ToString(), comparisonType, true, true, LabKitID, dnaEvidenceAmount);

            //                        page2b.btnCompare_Click(this, new EventArgs());
            //                        if (saveTableComparison != null && saveTableComparison.Columns.Contains("guid"))
            //                            saveTableComparison.Columns.Remove("guid");
            //                        if (comparisonProfiles.Columns.Contains("guid"))
            //                            comparisonProfiles.Columns.Remove("guid");
            //                        lastUpdated = DateTime.Now;
            //                    }
            //                    else
            //                    {
            //                    }
            //                }
            //                break;
            //            #endregion
            //            #region cases 3,4,9,12,18,22
            //            case 3:
            //            case 4:
            //            case 9:
            //            case 12:
            //            case 18:
            //            case 22:
            //                if (testIndividual)
            //                {
            //                    foreach (DataRow dr in comparisonProfiles.Rows)
            //                    {
            //                        KnownAlleles = new Dictionary<string, Dictionary<int, string>>();
            //                        if (comparisonType == 12 || comparisonType == 18)
            //                        {
            //                            ReadComparison(dr, KnownAlleles, 2);
            //                            ReadComparison(known1Profile.Rows[0], KnownAlleles, 1);
            //                        }
            //                        else
            //                        {
            //                            ReadComparison(dr, KnownAlleles, 1);
            //                            ReadComparison(known1Profile.Rows[0], KnownAlleles, 2);
            //                        }
            //                        saveTableComparison = GetSaveTable(dr, "Suspect");
            //                        saveTableKnown1 = GetSaveTable(known1Profile.Rows[0], "Suspect2");

            //                        if (testManualEntry)
            //                        {
            //                            dnaEvidenceAmount = 111;
            //                            subTestID = Guid.NewGuid();
            //                            comparisonID = dr["ID"].ToString();

            //                            WriteTest(testID, subTestID, testName, comparisonID, comparisonType, true, false, LabKitID, dnaEvidenceAmount);

            //                            page3n.btnCompare_Click(this, new EventArgs());
            //                            lastUpdated = DateTime.Now;
            //                        }
            //                        else
            //                        {
            //                        }
            //                    }
            //                }
            //                if (testBulk)
            //                {
            //                    if (testManualEntry)
            //                    {
            //                        KnownAlleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(known1Profile.Rows[0], KnownAlleles, 1);
            //                        saveTableKnown1 = GetSaveTable(known1Profile.Rows[0], "Suspect");

            //                        dnaEvidenceAmount = 111;
            //                        subTestID = Guid.NewGuid();

            //                        foreach (DataRow dr in comparisonProfiles.Rows)
            //                            WriteTest(testID, subTestID, testName, dr["ID"].ToString(), comparisonType, true, true, LabKitID, dnaEvidenceAmount);

            //                        page3b.btnCompare_Click(this, new EventArgs());
            //                        if (saveTableComparison != null && saveTableComparison.Columns.Contains("guid"))
            //                            saveTableComparison.Columns.Remove("guid");
            //                        if (comparisonProfiles.Columns.Contains("guid"))
            //                            comparisonProfiles.Columns.Remove("guid");
            //                        lastUpdated = DateTime.Now;
            //                    }
            //                    else
            //                    {
            //                    }
            //                }
            //                break;
            //            #endregion
            //            #region cases 8,16
            //            case 8:
            //            case 16:
            //                if (testIndividual)
            //                {
            //                    Known2Alleles = new Dictionary<string, Dictionary<int, string>>();
            //                    ReadComparison(known1Profile.Rows[0], Known2Alleles, 1);
            //                    ReadComparison(known2Profile.Rows[0], Known2Alleles, 2);
            //                    saveTableKnown1 = GetSaveTable(known1Profile.Rows[0], "Victim");
            //                    saveTableKnown2 = GetSaveTable(known1Profile.Rows[0], "Victim2");

            //                    foreach (DataRow dr in comparisonProfiles.Rows)
            //                    {
            //                        KnownAlleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(dr, KnownAlleles, 1);
            //                        saveTableComparison = GetSaveTable(dr, "Suspect");


            //                        if (testManualEntry)
            //                        {
            //                            dnaEvidenceAmount = 111;
            //                            subTestID = Guid.NewGuid();
            //                            comparisonID = dr["ID"].ToString();

            //                            WriteTest(testID, subTestID, testName, comparisonID, comparisonType, true, false, LabKitID, dnaEvidenceAmount);

            //                            page8n.btnCompare_Click(this, new EventArgs());
            //                            lastUpdated = DateTime.Now;
            //                        }
            //                        else
            //                        {
            //                        }
            //                    }
            //                }
            //                if (testBulk)
            //                {
            //                    if (testManualEntry)
            //                    {
            //                        Known2Alleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(known1Profile.Rows[0], Known2Alleles, 1);
            //                        ReadComparison(known2Profile.Rows[0], Known2Alleles, 2);
            //                        saveTableKnown1 = GetSaveTable(known1Profile.Rows[0], "Suspect");
            //                        saveTableKnown2 = GetSaveTable(known2Profile.Rows[0], "Suspect2");

            //                        dnaEvidenceAmount = 111;
            //                        subTestID = Guid.NewGuid();

            //                        foreach (DataRow dr in comparisonProfiles.Rows)
            //                            WriteTest(testID, subTestID, testName, dr["ID"].ToString(), comparisonType, true, true, LabKitID, dnaEvidenceAmount);

            //                        page8b.btnCompare_Click(this, new EventArgs());
            //                        if (saveTableComparison != null && saveTableComparison.Columns.Contains("guid"))
            //                            saveTableComparison.Columns.Remove("guid");
            //                        if (comparisonProfiles.Columns.Contains("guid"))
            //                            comparisonProfiles.Columns.Remove("guid");
            //                        lastUpdated = DateTime.Now;
            //                    }
            //                    else
            //                    {
            //                    }
            //                }
            //                break;
            //            #endregion
            //            #region cases 10,20
            //            case 10:
            //            case 20:
            //                if (testIndividual)
            //                {
            //                    Known2Alleles = new Dictionary<string, Dictionary<int, string>>();
            //                    ReadComparison(known1Profile.Rows[0], Known2Alleles, 1);
            //                    ReadComparison(known2Profile.Rows[0], Known2Alleles, 2);
            //                    ReadComparison(known3Profile.Rows[0], Known2Alleles, 3);
            //                    saveTableKnown1 = GetSaveTable(known1Profile.Rows[0], "Suspect2");
            //                    saveTableKnown2 = GetSaveTable(known1Profile.Rows[0], "Suspect3");
            //                    saveTableKnown3 = GetSaveTable(known1Profile.Rows[0], "Suspect4");

            //                    foreach (DataRow dr in comparisonProfiles.Rows)
            //                    {
            //                        KnownAlleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(dr, KnownAlleles, 1);
            //                        saveTableComparison = GetSaveTable(dr, "Suspect");


            //                        if (testManualEntry)
            //                        {
            //                            dnaEvidenceAmount = 111;
            //                            subTestID = Guid.NewGuid();
            //                            comparisonID = dr["ID"].ToString();

            //                            WriteTest(testID, subTestID, testName, comparisonID, comparisonType, true, false, LabKitID, dnaEvidenceAmount);

            //                            page10n.btnCompare_Click(this, new EventArgs());
            //                            lastUpdated = DateTime.Now;
            //                        }
            //                        else
            //                        {
            //                        }
            //                    }
            //                }
            //                if (testBulk)
            //                {
            //                    if (testManualEntry)
            //                    {
            //                        Known2Alleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(known1Profile.Rows[0], Known2Alleles, 1);
            //                        ReadComparison(known2Profile.Rows[0], Known2Alleles, 2);
            //                        ReadComparison(known3Profile.Rows[0], Known2Alleles, 3);

            //                        dnaEvidenceAmount = 111;
            //                        subTestID = Guid.NewGuid();

            //                        foreach (DataRow dr in comparisonProfiles.Rows)
            //                            WriteTest(testID, subTestID, testName, dr["ID"].ToString(), comparisonType, true, true, LabKitID, dnaEvidenceAmount);

            //                        page10b.btnCompare_Click(this, new EventArgs());
            //                        if (saveTableComparison != null && saveTableComparison.Columns.Contains("guid"))
            //                            saveTableComparison.Columns.Remove("guid");
            //                        if (comparisonProfiles.Columns.Contains("guid"))
            //                            comparisonProfiles.Columns.Remove("guid");
            //                        lastUpdated = DateTime.Now;
            //                    }
            //                    else
            //                    {
            //                    }
            //                }
            //                break;
            //            #endregion
            //            #region cases 11,19
            //            case 11:
            //            case 19:
            //                if (testIndividual)
            //                {
            //                    foreach (DataRow dr in comparisonProfiles.Rows)
            //                    {
            //                        KnownAlleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(known1Profile.Rows[0], KnownAlleles, 1);
            //                        ReadComparison(known2Profile.Rows[0], KnownAlleles, 2);
            //                        ReadComparison(dr, KnownAlleles, 3);

            //                        if (testManualEntry)
            //                        {
            //                            dnaEvidenceAmount = 111;
            //                            subTestID = Guid.NewGuid();
            //                            comparisonID = dr["ID"].ToString();

            //                            WriteTest(testID, subTestID, testName, comparisonID, comparisonType, true, false, LabKitID, dnaEvidenceAmount);

            //                            page11n.btnCompare_Click(this, new EventArgs());
            //                            lastUpdated = DateTime.Now;
            //                        }
            //                        else
            //                        {
            //                        }
            //                    }
            //                }
            //                if (testBulk)
            //                {
            //                    if (testManualEntry)
            //                    {
            //                        KnownAlleles = new Dictionary<string, Dictionary<int, string>>();
            //                        ReadComparison(known1Profile.Rows[0], KnownAlleles, 1);
            //                        ReadComparison(known2Profile.Rows[0], KnownAlleles, 2);

            //                        dnaEvidenceAmount = 111;
            //                        subTestID = Guid.NewGuid();

            //                        foreach (DataRow dr in comparisonProfiles.Rows)
            //                            WriteTest(testID, subTestID, testName, dr["ID"].ToString(), comparisonType, true, true, LabKitID, dnaEvidenceAmount);

            //                        page11b.btnCompare_Click(this, new EventArgs());

            //                        if (saveTableComparison != null && saveTableComparison.Columns.Contains("guid"))
            //                            saveTableComparison.Columns.Remove("guid");
            //                        if (comparisonProfiles.Columns.Contains("guid"))
            //                            comparisonProfiles.Columns.Remove("guid");
            //                        lastUpdated = DateTime.Now;
            //                    }
            //                    else
            //                    {
            //                    }
            //                }
            //                break;
            //            #endregion
            //            default:
            //                break;
            //        }
            //    }
            //}));
        }

        protected void btnRunTestNew_Click(object sender, EventArgs e)
        {
            // initialize the page sessions (needs to be done so the pages have a session handle in the subthread)
            FST.Web.frmManualEntry page = new frmManualEntry();
            var s = page.Session;
            string version = ConfigurationSettings.AppSettings.Get("FST_VERSION");

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
                testName = txtTestName.Text;
                testIndividual = chkIndividual.Checked;
                testBulk = chkBulk.Checked;
                testManualEntry = rbManualEntry.Checked;
                deducible = rbDeducible.Checked;
                LabKitID = bi.GetLabKits().Rows[0]["FieldValue"].ToString();

                foreach (ListItem liComparisonType in chkComparisonTypes.Items)
                    if (liComparisonType.Selected)
                        comparisonTypes.Add(Convert.ToInt32(liComparisonType.Value));

                comparisonProfiles = bi.GetProfilesFromFile(this.fuComparisonProfiles, Server.MapPath("~/Admin/Upload/"), Guid.Parse(LabKitID));
                known1Profile = bi.GetProfilesFromFile(this.fuKnown1Profile, Server.MapPath("~/Admin/Upload/"), Guid.Parse(LabKitID));
                known2Profile = bi.GetProfilesFromFile(this.fuKnown2Profile, Server.MapPath("~/Admin/Upload/"), Guid.Parse(LabKitID));
                known3Profile = bi.GetProfilesFromFile(this.fuKnown3Profile, Server.MapPath("~/Admin/Upload/"), Guid.Parse(LabKitID));
                evidence = GetEvidenceFromFile(this.fuEvidence, Server.MapPath("~/Admin/Upload/"));
                saveTableEvidence = GetSaveTableEvidence(evidence);

                Known2Alleles = new Dictionary<string, Dictionary<int, string>>();

                EvidenceAlleles = new Dictionary<string, Dictionary<int, string>>();
                ReadEvidenceAllelesNew(evidence, EvidenceAlleles);

                testID = Guid.NewGuid();

                foreach (int comparisonType in comparisonTypes)
                {
                    Session["CompareMethod"] = comparisonType.ToString();

                    if (testIndividual)
                    {
                        foreach (DataRow dr in comparisonProfiles.Rows)
                        {
                            dnaEvidenceAmount = 111;
                            subTestID = Guid.NewGuid();
                            comparisonID = dr["ID"].ToString();

                            ComparisonData comparisonData = new ComparisonData(comparisonType);
                            comparisonData.Bulk = false;
                            comparisonData.Comparison = "\tes\t";
                            comparisonData.Deducible = deducible;
                            comparisonData.Degradation = FST.Common.ComparisonData.enDegradation.None;
                            comparisonData.DNAAmount = dnaEvidenceAmount = 111;
                            comparisonData.EvidenceAlleles = EvidenceAlleles;
                            comparisonData.FB1 = subTestID.ToString();
                            comparisonData.Item = comparisonID;
                            comparisonData.LabKitID = Guid.Parse(LabKitID);
                            comparisonData.LabKitName = "Indentifiler";
                            comparisonData.Processed = 'N';
                            comparisonData.Theta = 0.03f;
                            comparisonData.UserName = User.Identity.Name;
                            comparisonData.Version = version;

                            ReadComparisonNew(dr, comparisonData.ComparisonAlleles, 1);

                            if (comparisonData.NumeratorProfiles.KnownCount >= 1 || comparisonData.DenominatorProfiles.KnownCount >= 1)
                                ReadComparisonNew(known1Profile.Rows[0], comparisonData.KnownsAlleles, 1);
                            if (comparisonData.NumeratorProfiles.KnownCount >= 2 || comparisonData.DenominatorProfiles.KnownCount >= 2)
                                ReadComparisonNew(known2Profile.Rows[0], comparisonData.KnownsAlleles, 2);
                            if (comparisonData.NumeratorProfiles.KnownCount >= 3 || comparisonData.DenominatorProfiles.KnownCount >= 3)
                                ReadComparisonNew(known3Profile.Rows[0], comparisonData.KnownsAlleles, 3);

                            saveTableComparison = GetSaveTable(dr, "Suspect");

                            this.comparisonData = comparisonData;

                            WriteTest(testID, subTestID, testName, comparisonID, comparisonType, true, false, LabKitID, dnaEvidenceAmount);

                            page.btnCompare_Click(this, new EventArgs());

                            if (saveTableComparison != null && saveTableComparison.Columns.Contains("guid"))
                                saveTableComparison.Columns.Remove("guid");
                            if (comparisonProfiles.Columns.Contains("guid"))
                                comparisonProfiles.Columns.Remove("guid");
                            lastUpdated = DateTime.Now;
                        }
                    }
                    if (testBulk)
                    {
                        dnaEvidenceAmount = 111;
                        subTestID = Guid.NewGuid();

                        ComparisonData comparisonData = new ComparisonData(comparisonType);
                        comparisonData.Bulk = true;
                        comparisonData.Comparison = "\tes\t";
                        comparisonData.Deducible = deducible;
                        comparisonData.Degradation = FST.Common.ComparisonData.enDegradation.None;
                        comparisonData.DNAAmount = dnaEvidenceAmount = 111;
                        comparisonData.EvidenceAlleles = EvidenceAlleles;
                        comparisonData.FB1 = subTestID.ToString();
                        comparisonData.LabKitID = Guid.Parse(LabKitID);
                        comparisonData.LabKitName = "Indentifiler";
                        comparisonData.Processed = 'N';
                        comparisonData.Theta = 0.03f;
                        comparisonData.UserName = User.Identity.Name;
                        comparisonData.Version = version;

                        if (comparisonData.NumeratorProfiles.KnownCount >= 1 || comparisonData.DenominatorProfiles.KnownCount >= 1)
                            ReadComparisonNew(known1Profile.Rows[0], comparisonData.KnownsAlleles, 1);
                        if (comparisonData.NumeratorProfiles.KnownCount >= 2 || comparisonData.DenominatorProfiles.KnownCount >= 2)
                            ReadComparisonNew(known2Profile.Rows[0], comparisonData.KnownsAlleles, 2);
                        if (comparisonData.NumeratorProfiles.KnownCount >= 3 || comparisonData.DenominatorProfiles.KnownCount >= 3)
                            ReadComparisonNew(known3Profile.Rows[0], comparisonData.KnownsAlleles, 3);

                        foreach (DataRow dr in comparisonProfiles.Rows)
                            WriteTest(testID, subTestID, testName, dr["ID"].ToString(), comparisonType, true, true, LabKitID, dnaEvidenceAmount);

                        this.comparisonData = comparisonData;

                        page.btnCompare_Click(this, new EventArgs());

                        if (saveTableComparison != null && saveTableComparison.Columns.Contains("guid"))
                            saveTableComparison.Columns.Remove("guid");
                        if (comparisonProfiles.Columns.Contains("guid"))
                            comparisonProfiles.Columns.Remove("guid");
                        lastUpdated = DateTime.Now;
                    }
                }
            }));
        }

        private DataTable GetSaveTable(DataRow dr, string name)
        {
            DataTable saveTableComparison = new DataTable(name);
            saveTableComparison.Columns.Add("LocusID", typeof(string));
            saveTableComparison.Columns.Add("Allele", typeof(string));

            foreach (DataColumn dc in dr.Table.Columns)
                if (dc.ColumnName.ToUpper() != "ID" || dc.ColumnName.ToUpper() == "GUID")
                    saveTableComparison.Rows.Add(dc.ColumnName, dr[dc.ColumnName]);

            return saveTableComparison;
        }

        private DataTable GetSaveTableEvidence(DataTable evidence)
        {
            DataTable saveTableEvidence = new DataTable("Unknown");
            saveTableEvidence.Columns.Add("RepID", typeof(int));
            saveTableEvidence.Columns.Add("LocusID", typeof(string));
            saveTableEvidence.Columns.Add("Allele", typeof(string));
            saveTableEvidence.Columns.Add("Inconclusive", typeof(bool));

            foreach (DataColumn dc in evidence.Columns)
                if (dc.ColumnName.ToUpper() != "ID" || dc.ColumnName.ToUpper() == "GUID")
                    foreach (DataRow dr in evidence.Rows)
                        saveTableEvidence.Rows.Add(dr["ID"], dc.ColumnName, dr[dc.ColumnName], 0);

            return saveTableEvidence;
        }

        private void WriteTest(Guid testID, Guid subTestID, string testName, string comparisonID, int comparisonType, bool testManualEntry, bool testBulk, string LabKitID, int dnaEvidenceAmount)
        {
            bi.WriteTest(testID, subTestID, testName, comparisonID, comparisonType, testManualEntry, testBulk, LabKitID, dnaEvidenceAmount);
        }

        private void ReadComparison(DataRow dr, Dictionary<string, Dictionary<int, string>> KnownAlleles, int location)
        {
            foreach (DataColumn dc in dr.Table.Columns)
                if (dc.ColumnName.ToUpper() != "ID" || dc.ColumnName.ToUpper() == "GUID")
                {
                    if (!KnownAlleles.ContainsKey(dc.ColumnName))
                        KnownAlleles.Add(dc.ColumnName, new Dictionary<int, string>());

                    KnownAlleles[dc.ColumnName].Add(location, dr[dc.ColumnName].ToString());
                }
        }

        private void ReadComparisonNew(DataRow dr, Dictionary<int, Dictionary<string, string>> KnownAlleles, int location)
        {
            foreach (DataColumn dc in dr.Table.Columns)
                if (dc.ColumnName.ToUpper() != "ID" || dc.ColumnName.ToUpper() == "GUID")
                {
                    if (!KnownAlleles.ContainsKey(location))
                        KnownAlleles.Add(location, new Dictionary<string, string>());

                    KnownAlleles[location].Add(dc.ColumnName.ToUpper(), dr[dc.ColumnName].ToString());
                }
        }

        private void ReadEvidenceAlleles(DataTable evidence, Dictionary<string, Dictionary<int, string>> EvidenceAlleles)
        {
            foreach (DataColumn dc in evidence.Columns)
                if (dc.ColumnName.ToUpper() != "ID" || dc.ColumnName.ToUpper() == "GUID")
                    EvidenceAlleles.Add(dc.ColumnName, new Dictionary<int, string>());

            foreach (string key in EvidenceAlleles.Keys)
            {
                EvidenceAlleles[key].Add(1, evidence.Rows[0][key].ToString());
                EvidenceAlleles[key].Add(2, evidence.Rows[1][key].ToString());
                EvidenceAlleles[key].Add(3, evidence.Rows[2][key].ToString());
            }
        }

        private void ReadEvidenceAllelesNew(DataTable evidence, Dictionary<string, Dictionary<int, string>> EvidenceAlleles)
        {
            foreach (DataColumn dc in evidence.Columns)
                if (dc.ColumnName.ToUpper() != "ID" || dc.ColumnName.ToUpper() == "GUID")
                    EvidenceAlleles.Add(dc.ColumnName.ToUpper(), new Dictionary<int, string>());

            foreach (string key in EvidenceAlleles.Keys)
            {
                EvidenceAlleles[key].Add(1, evidence.Rows[0][key].ToString());
                EvidenceAlleles[key].Add(2, evidence.Rows[1][key].ToString());
                EvidenceAlleles[key].Add(3, evidence.Rows[2][key].ToString());
            }
        }


        private DateTime lastUpdated = DateTime.MinValue;

        public ComparisonData comparisonData = null;

        public float theta = 0.03f;
        public string degradedType = "ND";

        public string testName { get; set; }
        public bool testIndividual { get; set; }
        public bool testBulk { get; set; }
        public List<int> comparisonTypes = new List<int>();
        public bool testManualEntry { get; set; }
        public bool deducible { get; set; }
        public int dnaEvidenceAmount { get; set; }
        public string LabKitID { get; set; }
        public Guid testID { get; set; }
        public Guid subTestID { get; set; }
        public string comparisonID { get; set; }

        public DataTable comparisonProfiles { get; set; }
        public DataTable known1Profile { get; set; }
        public DataTable known2Profile { get; set; }
        public DataTable known3Profile { get; set; }
        public DataTable evidence { get; set; }

        public DataTable saveTableEvidence;
        public DataTable saveTableComparison;
        public DataTable saveTableKnown1;
        public DataTable saveTableKnown2;
        public DataTable saveTableKnown3;

        public Dictionary<string, Dictionary<int, string>> EvidenceAlleles { get; set; }
        public Dictionary<string, Dictionary<int, string>> KnownAlleles { get; set; }
        public Dictionary<string, Dictionary<int, string>> Known2Alleles { get; set; }

        #region Get Evidence
        public DataTable GetEvidenceFromFile(FileUpload fuPopulationUpload, string path)
        {
            string strErrMsg = string.Empty;
            DataTable dt = null;
            DataTable loci = bi.GetLocus(Guid.Parse(LabKitID));

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

                    if (!dt.Columns.Contains("ID")
                    || !dt.Columns.Contains("D8S1179")
                    || !dt.Columns.Contains("D21S11")
                    || !dt.Columns.Contains("D7S820")
                    || !dt.Columns.Contains("CSF1PO")
                    || !dt.Columns.Contains("D3S1358")
                    || !dt.Columns.Contains("TH01")
                    || !dt.Columns.Contains("D13S317")
                    || !dt.Columns.Contains("D16S539")
                    || !dt.Columns.Contains("D2S1338")
                    || !dt.Columns.Contains("D19S433")
                    || !dt.Columns.Contains("vWA")
                    || !dt.Columns.Contains("TPOX")
                    || !dt.Columns.Contains("D18S51")
                    || !dt.Columns.Contains("D5S818")
                    || !dt.Columns.Contains("FGA"))
                    {
                        MessageBox.Show("The uploaded file does not contain one of the required locus columns or the ID column or the ethnicity column.");
                        return null;
                    }

                    // remove blank rows
                    foreach (DataRow dr in dt.Select("ID IS NULL"))
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
                                string[] data = dr[dc.ColumnName].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < data.Length; i++)
                                    data[i] = data[i].Trim();
                                StringBuilder sb = new StringBuilder();
                                for (int i = 0; i < data.Length; i++)
                                {
                                    sb.Append(data[i]);
                                    if (i != data.Length - 1)
                                        sb.Append(",");
                                }
                                dr[dc.ColumnName] = sb.ToString();
                            }
                    }

                    // order the columns in the proper order
                    dt.Columns["ID"].SetOrdinal(dt.Columns.Count - 1);
                    foreach (DataRow dr in loci.Rows)
                        if (dt.Columns.Contains(dr["FieldName"].ToString()))
                            dt.Columns[dr["FieldName"].ToString()].SetOrdinal(Convert.ToInt32(dr["FieldValue"]) - 1);

                    // uppercase all the columns except vWA which needs a special case
                    foreach (DataColumn dc in dt.Columns)
                        if (dc.ColumnName.ToUpper() != "VWA")
                            dc.ColumnName = dc.ColumnName.ToUpper();
                        else dc.ColumnName = "vWA";
                }
            }
            catch (Exception ex)
            {
                // handle errors
                MessageBox.Show("There was an error reading the uploaded file. Please try uploading an Excel or CSV file in the correct format.");
                return null;
            }

            return dt;
        }
        #endregion
    }
}