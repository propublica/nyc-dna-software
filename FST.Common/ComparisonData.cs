using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;
using System.Data;

namespace FST.Common
{
    /// <summary>
    /// This class contains all the configuration data for a comparison, some properties that make naming easier to access, 
    /// and a method for creating a data set for report printing purposes.
    /// </summary>
    [Serializable()]
    public class ComparisonData
    {
        /// <summary>
        /// This constructor gets called by the public constructor and the XmlSerializer on deserialization
        /// </summary>
        private ComparisonData()
        {
            // initialize some of our member properties
            NumeratorProfiles = new Contributors();
            DenominatorProfiles = new Contributors();
            ComparisonAlleles = new Dictionary<int, Dictionary<string, string>>();
            KnownsAlleles = new Dictionary<int, Dictionary<string, string>>();
            EvidenceAlleles = new Dictionary<string, Dictionary<int, string>>();
        }

        /// <summary>
        /// This is the default public constructor which takes a comparisonID parameter that describes which of the comparison scenarios we're going to use.
        /// </summary>
        /// <param name="comparisonID">Database ID of the current comparison.</param>
        public ComparisonData(int comparisonID)
            : this()
        {
            this.compareMethodID = comparisonID;
            // set up the configuration for this scenario. this may eventually be done automatically with configuration data from the database.
            configureComparison(comparisonID);
        }

        /// <summary>
        /// This method configures the number of comparisons, knowns, and unknowns in the numerator and denominator, sets comparison long and sort names,
        /// sets the comparison heading format strings, and sets which comparisons have their individual variant run on the windows service.
        /// </summary>
        /// <param name="comparisonID"></param>
        private void configureComparison(int comparisonID)
        {
            // we do this because the .NET XML Serializer does not actually set public properties to the stored value, but instead it looks into the getter/setter methods
            // and directly sets the underlying value. Since the comparisonID is actually processed by calling the configureComparison() method, we have this public member
            // to store/restore the value for ComparisonID from it. 
            compareMethodIDSerializationBackupDoNotMessWithThisVariableBecauseItsAnUglyHackAroundACrappyLimitiationOfTheFCLsCrappyXMLSerializer = comparisonID;

            // the default setting for comparisons is to run the individual search on the website, and not in the service
            RunIndividualOnService = false;
            
            // for each of the comparison IDs we set up the comparison and known profile dictionary keys (they count 1,2,3,..) and the number of unknowns
            // we also set up the short and long names for the comparisons, and format strings which allow the numerator and denominator headers to be 
            // generated using the names given by the user. the format strings use {0},{1},{2},{3] for comparisons 1-4, and {4},{5},{6},{7} for knowns 1-4.
            #region switch (comparisonID)
            switch (comparisonID)
            {
                case 1:
                    // add one comparison to the ID list
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    // add one unknown in the denominator
                    DenominatorProfiles.UnknownCount = 1;
                    // set the long name
                    CompareMethodLongName = "Comparison / Unknown";
                    // set the short name
                    CompareMethodShortName = "C / U";
                    // set up the numerator header format string
                    hpHeadFormatString = "{0} (Comparison)";
                    // set up the denominator header format string
                    hdHeadFormatString = "Unknown";
                    break;
                case 2:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    // set a known in the numerator
                    NumeratorProfiles.KnownIDs.Add(1);
                    // set the same known in the denominator
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 1;
                    CompareMethodLongName = "Comparison + Known / Known + Unknown";
                    CompareMethodShortName = "C + K / K + U";
                    // we use {4} to denote the known name
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known)";
                    hdHeadFormatString = "{4} (Known) + Unknown";
                    break;
                case 3:
                    // this comparison ID is unused at this time
                    throw new NotImplementedException();
                case 4:
                    // this comparison ID is unused at this time
                    throw new NotImplementedException();
                case 5:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "Comparison + Unknown / 2 Unknowns";
                    CompareMethodShortName = "C + U / 2 U";
                    hpHeadFormatString = "{0} (Comparison) + Unknown";
                    hdHeadFormatString = "2 Unknowns";
                    break;
                case 6:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 2;
                    DenominatorProfiles.UnknownCount = 3;
                    CompareMethodLongName = "Comparison + 2 Unknowns / 3 Unknowns";
                    CompareMethodShortName = "C + 2 U / 3 U";
                    hpHeadFormatString = "{0} (Comparison) + 2 Unknowns";
                    hdHeadFormatString = "3 Unknowns";
                    break;
                case 7:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "Comparison + Known + Unknown / Known + 2 Unknowns";
                    CompareMethodShortName = "C + K + U / K + 2 U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + Unknown";
                    hdHeadFormatString = "{4} (Known) + 2 Unknowns";
                    break;
                case 8:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    // we add two knowns, 1 and 2, to the numerator
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(2);
                    // and the same two to the denominator
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.UnknownCount = 1;
                    CompareMethodLongName = "Comparison + 2 Knowns / 2 Known + Unknown";
                    CompareMethodShortName = "C + 2 K / 2 K + U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + {5} (Known)";
                    hdHeadFormatString = "{4} (Known) + {5} (Known) + Unknown";
                    break;
                case 9:
                    // this comparison ID is unused at this time
                    throw new NotImplementedException();
                case 10:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(2);
                    NumeratorProfiles.KnownIDs.Add(3);
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.KnownIDs.Add(3);
                    DenominatorProfiles.UnknownCount = 1;
                    CompareMethodLongName = "Comparison + 3 Knowns / 3 Knowns + Unknown";
                    CompareMethodShortName = "C + 3 K / 3 K + U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + {5} (Known) + {6} (Known)";
                    hdHeadFormatString = "{4} (Known) + {5} (Known) + {6} (Known) + Unknown";
                    break;
                case 11:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(2);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "Comparison + 2 Knowns + Unknown / 2 Knowns + 2 Unknowns";
                    CompareMethodShortName = "C + 2 K + U / 2 K + 2 U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + {5} (Known) + Unknown";
                    hdHeadFormatString = "{4} (Known) + {5} (Known) + 2 Unknowns";
                    break;
                case 12:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 2;
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 3;
                    CompareMethodLongName = "Comparison + Known + 2 Unknown / Known + 3 Unknowns";
                    CompareMethodShortName = "C + K + 2 U / K + 3 U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + 2 Unknowns";
                    hdHeadFormatString = "{4} (Known) + 3 Unknowns";
                    break;
                case 13:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 3;
                    DenominatorProfiles.UnknownCount = 4;
                    CompareMethodLongName = "Comparison + 3 Unknowns / 4 Unknowns";
                    CompareMethodShortName = "C + 3 U / 4 U";
                    hpHeadFormatString = "{0} (Comparison) + 3 Unknowns";
                    hdHeadFormatString = "4 Unknowns";
                    RunIndividualOnService = true;
                    break;
                case 14:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 2;
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "Comparison + 2 Unknowns / 2 Unknowns";
                    CompareMethodShortName = "C + 2 U / 2 U";
                    hpHeadFormatString = "{0} (Comparison) + 2 Unknowns";
                    hdHeadFormatString = "2 Unknowns";
                    break;
                case 15:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 1;
                    CompareMethodLongName = "Comparison + Known + Unknown / Known + Unknown";
                    CompareMethodShortName = "C + K + U / K + U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + Unknown";
                    hdHeadFormatString = "{4} (Known) + Unknown";
                    break;
                case 16:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(2);
                    CompareMethodLongName = "Comparison + 2 Knowns / 2 Knowns";
                    CompareMethodShortName = "C + 2 K / 2 K";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + {5} (Known)";
                    hdHeadFormatString = "{4} (Known) + {5} (Known)";
                    break;
                case 17:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 3;
                    DenominatorProfiles.UnknownCount = 3;
                    CompareMethodLongName = "Comparison + 3 Unknowns / 3 Unknowns";
                    CompareMethodShortName = "C + 3 U / 3 U";
                    hpHeadFormatString = "{0} (Comparison) + 3 Unknowns";
                    hdHeadFormatString = "3 Unknowns";
                    RunIndividualOnService = true;
                    break;
                case 18:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 2;
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "Comparison + Known + 2 Unknowns / Known + 2 Unknowns";
                    CompareMethodShortName = "C + K + 2 U / K + 2 U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + 2 Unknowns";
                    hdHeadFormatString = "{4} (Known) + 2 Unknowns";
                    break;
                case 19:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(2);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.UnknownCount = 1;
                    CompareMethodLongName = "Comparison + 2 Knowns + Unknown / 2 Knowns + Unknown";
                    CompareMethodShortName = "C + 2 K + U / 2 K + U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + {5} (Known) + Unknown";
                    hdHeadFormatString = "{4} (Known) + {5} (Known) + Unknown";
                    break;
                case 20:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(2);
                    NumeratorProfiles.KnownIDs.Add(3);
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.KnownIDs.Add(3);
                    CompareMethodLongName = "Comparison + 3 Knowns / 3 Knowns";
                    CompareMethodShortName = "C + 3 K / 3 K";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + {5} (Known) + {6} (Known)";
                    hdHeadFormatString = "{4} (Known) + {5} (Known) + {6} (Known)";
                    break;
                case 21:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.UnknownCount = 1;
                    CompareMethodLongName = "Comparison + Unknown / Unknown";
                    CompareMethodShortName = "C + U / U";
                    hpHeadFormatString = "{0} (Comparison) + Unknown";
                    hdHeadFormatString = "Unknown";
                    break;
                case 22:
                    // this is an example with two comparisons
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "2 Comparisons / 2 Unknowns";
                    CompareMethodShortName = "2 C / 2 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison)";
                    hdHeadFormatString = "2 Unknowns";
                    break;
                case 23:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "Comparison / 2 Unknowns";
                    CompareMethodShortName = "C / 2 U";
                    hpHeadFormatString = "{0} (Comparison)";
                    hdHeadFormatString = "2 Unknowns";
                    break;
                case 24:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.UnknownCount = 3;
                    CompareMethodLongName = "Comparison + Unknown / 3 Unknowns";
                    CompareMethodShortName = "C + U / 3 U";
                    hpHeadFormatString = "{0} (Comparison) + Unknown";
                    hdHeadFormatString = "3 Unknowns";
                    break;
                case 25:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "Comparison + Known / Known + 2 Unknowns";
                    CompareMethodShortName = "C + K / K + 2 U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known)";
                    hdHeadFormatString = "{4} (Known) + 2 Unknowns";
                    break;
                case 26:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 2;
                    DenominatorProfiles.UnknownCount = 4;
                    CompareMethodLongName = "Comparison + 2 Unknowns / 4 Unknowns";
                    CompareMethodShortName = "C + 2 U / 4 U";
                    hpHeadFormatString = "{0} (Comparison) + 2 Unknowns";
                    hdHeadFormatString = "4 Unknowns";
                    RunIndividualOnService = true;
                    break;
                case 27:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 3;
                    CompareMethodLongName = "Comparison + Known + Unknown / Known + 3 Unknowns";
                    CompareMethodShortName = "C + K + U / K + 3 U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + Unknown";
                    hdHeadFormatString = "{4} (Known) + 3 Unknowns";
                    break;
                case 28:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "Comparison + 2 Knowns / 2 Knowns + 2 Unknowns";
                    CompareMethodShortName = "C + 2 K / 2K + 2 U";
                    hpHeadFormatString = "{0} (Comparison) + {4} (Known) + {5} (Known)";
                    hdHeadFormatString = "{4} (Known) + {5} (Known) + 2 Unknowns";
                    break;
                case 29:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    DenominatorProfiles.UnknownCount = 1;
                    CompareMethodLongName = "2 Comparisons / Unknown";
                    CompareMethodShortName = "2 C / U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison)";
                    hdHeadFormatString = "Unknown";
                    break;
                case 30:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    NumeratorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "2 Comparisons + Known / Known + 2 Unknowns";
                    CompareMethodShortName = "2 C + K / K + 2 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison) + {4} (Known)";
                    hdHeadFormatString = "{4} (Known) + 2 Unknowns";
                    break;
                case 31:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    NumeratorProfiles.UnknownCount = 2;
                    DenominatorProfiles.UnknownCount = 4;
                    CompareMethodLongName = "2 Comparisons + 2 Unknowns / 4 Unknowns";
                    CompareMethodShortName = "2 C + 2 U / 4 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison) + 2 Unknowns";
                    hdHeadFormatString = "4 Unknowns";
                    RunIndividualOnService = true;
                    break;
                case 32:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 3;
                    CompareMethodLongName = "2 Comparisons + Known + Unknown / Known + 3 Unknowns";
                    CompareMethodShortName = "2 C + K + U / K + 3 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison) + {4} (Known) + Unknown";
                    hdHeadFormatString = "{4} (Known) + 3 Unknowns";
                    break;
                case 33:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    NumeratorProfiles.KnownIDs.Add(1);
                    NumeratorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(2);
                    DenominatorProfiles.UnknownCount = 2;
                    CompareMethodLongName = "2 Comparisons  + 2 Knowns / 2 Knowns + 2 Unknowns";
                    CompareMethodShortName = "2 C + 2 K / 2 K + 2 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison) + {4} (Known) + {5} (Known)";
                    hdHeadFormatString = "{4} (Known) + {5} (Known) + 2 Unknowns";
                    break;
                case 34:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    NumeratorProfiles.ComparisonIDs.Add(3);
                    DenominatorProfiles.UnknownCount = 3;
                    CompareMethodLongName = "3 Comparisons / 3 Unknowns";
                    CompareMethodShortName = "3 C / 3 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison) + {2} (Comparison)";
                    hdHeadFormatString = "3 Unknowns";
                    break;
                case 35:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    NumeratorProfiles.ComparisonIDs.Add(3);
                    NumeratorProfiles.UnknownCount = 1;
                    DenominatorProfiles.UnknownCount = 4;
                    CompareMethodLongName = "3 Comparisons + Unknown / 4 Unknowns";
                    CompareMethodShortName = "3 C + U / 4 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison) + {2} (Comparison) + Unknown";
                    hdHeadFormatString = "4 Unknowns";
                    RunIndividualOnService = true;
                    break;
                case 36:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    NumeratorProfiles.ComparisonIDs.Add(3);
                    NumeratorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 3;
                    CompareMethodLongName = "3 Comparisons + Known / Known + 3 Unknowns";
                    CompareMethodShortName = "3 C + K / K + 3 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison) + {2} (Comparison) + {4} (Known)";
                    hdHeadFormatString = "{4} (Known) + 3 Unknowns";
                    break;
                case 37:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.ComparisonIDs.Add(2);
                    NumeratorProfiles.ComparisonIDs.Add(3);
                    NumeratorProfiles.ComparisonIDs.Add(4);
                    DenominatorProfiles.UnknownCount = 4;
                    CompareMethodLongName = "4 Comparisons / 4 Unknowns";
                    CompareMethodShortName = "4 C / 4 U";
                    hpHeadFormatString = "{0} (Comparison) + {1} (Comparison) + {2} (Comparison) + {3} (Comparison)";
                    hdHeadFormatString = "4 Unknowns";
                    RunIndividualOnService = true;
                    break;
                case 38:
                    NumeratorProfiles.ComparisonIDs.Add(1);
                    NumeratorProfiles.UnknownCount = 1;
                    // this is an example where we have a known in the numerator, but not in the denominator
                    DenominatorProfiles.KnownIDs.Add(1);
                    DenominatorProfiles.UnknownCount = 1;
                    CompareMethodLongName = "Comparison + Unknown / Known + Unknown";
                    CompareMethodShortName = "C + U / K + U";
                    hpHeadFormatString = "{0} (Comparison) + Unknown";
                    hdHeadFormatString = "{4} (Known) + Unknown";
                    break;
                default: break;
            }
            #endregion
        }

        // username of the user that ran the report with the domain name. can be used to get e-mail address via Business_Interface
        public string UserName { get; set; }
        // this is the date at which the comparison began
        public DateTime ReportDate { get; set; }

        // holds our comparisonID so the property below can have some other code in the setter method
        private int compareMethodID;
        // hack around serializer limits
        public int compareMethodIDSerializationBackupDoNotMessWithThisVariableBecauseItsAnUglyHackAroundACrappyLimitiationOfTheFCLsCrappyXMLSerializer;
        // this method calls the configureComparison() method 
        public int CompareMethodID
        {
            get { return compareMethodID != 0 ? compareMethodID : compareMethodIDSerializationBackupDoNotMessWithThisVariableBecauseItsAnUglyHackAroundACrappyLimitiationOfTheFCLsCrappyXMLSerializer; }
            set { configureComparison(value); }
        }

        // stores a long name for the comparison scenario (like: "Comparison + Known + Unknown / 2 Knowns + Unknown")
        public string CompareMethodLongName { get; set; }
        // stores a short name for the comparison scenario (like: "C + K + U / 2 K + U")
        public string CompareMethodShortName { get; set; }

        // format string that is used to format the report header
        private string hpHeadFormatString;
        // property that uses the above format string to generate a header based on the names provided in the UI
        public string HpHead { get { return string.Format(hpHeadFormatString, new object[] { Comparison1Name, Comparison2Name, Comparison3Name, Comparison4Name, Known1Name, Known2Name, Known3Name, Known4Name }); } }

        // format string that is used to format the report header
        private string hdHeadFormatString;
        // property that uses the above format string to generate a header based on the names provided in the UI
        public string HdHead { get { return string.Format(hdHeadFormatString, new object[] { Comparison1Name, Comparison2Name, Comparison3Name, Comparison4Name, Known1Name, Known2Name, Known3Name, Known4Name }); } }

        // these are fields that are provided for individual comparisons in the UI
        public string Comparison { get; set; }
        public string FB1 { get; set; }
        public string FB2 { get; set; }
        public string Item { get; set; }

        // these are fields that apply to all comparisons and are provided in the UI
        public bool Deducible { get; set; }
        public decimal DNAAmount { get; set; }

        // these dictionaries hold the evidence replicate alleles and the profile alleles. 
        // dictionaries are not readily serializable by the XmlSerializer for obvious reasons, so we use the DictionaryProxy class below to serialize our dictionaries
        [XmlIgnore()]
        public Dictionary<int, Dictionary<string, string>> ComparisonAlleles { get; set; }
        [XmlIgnore()]
        public Dictionary<int, Dictionary<string, string>> KnownsAlleles { get; set; }
        [XmlIgnore()]
        public Dictionary<string, Dictionary<int, string>> EvidenceAlleles { get; set; }

        // these instances of DictionaryProxy are used to copy data into and out of them when serializing the class
        public DictionaryProxy<int, DictionaryProxy<string, string>> ComparisonAllelesProxy = new DictionaryProxy<int, DictionaryProxy<string, string>>();
        public DictionaryProxy<int, DictionaryProxy<string, string>> KnownsAllelesProxy = new DictionaryProxy<int, DictionaryProxy<string, string>>();
        public DictionaryProxy<string, DictionaryProxy<int, string>> EvidenceAllelesProxy = new DictionaryProxy<string, DictionaryProxy<int, string>>();

        // these values are inputted at the main screen
        public string LabKitName { get; set; }
        public Guid LabKitID { get; set; }
        public float Theta { get; set; }

        // these names are optionally supplied by the user at the main screen
        public string Known1Name { get; set; }
        public string Known2Name { get; set; }
        public string Known3Name { get; set; }
        public string Known4Name { get; set; }

        // these names are optionally supplied by the user at the main screen
        public string Comparison1Name { get; set; }
        public string Comparison2Name { get; set; }
        public string Comparison3Name { get; set; }
        public string Comparison4Name { get; set; }

        // if a user uploads known profiles from files, we store the filenames here for printing in the reports
        public string Known1FileName { get; set; }
        public string Known2FileName { get; set; }
        public string Known3FileName { get; set; }
        public string Known4FileName { get; set; }

        // if a user uploads comparison profiles from files, we store the filenames here for printing in the reports
        public string Comparison1FileName { get; set; }
        public string Comparison2FileName { get; set; }
        public string Comparison3FileName { get; set; }
        public string Comparison4FileName { get; set; }

        // if a user uploads evidence from file, we store the filenames here for printing in the reports
        public string EvidenceFileName { get; set; }

        // this tells us whether this is a bulk search or an individual comparison and is set based on which button the user hits on the main screen
        public bool Bulk { get; set; }

        // this tells us whether the source of the comparisons 
        public enBulkType BulkType { get; set; }
        public enum enBulkType { FromFile = 0, Population, LabTypes };

        // this is used to store a GUID which gets passed to the database for later retrieval of the comparison profile data associated with this "case"
        public Guid FromFileGuid { get; set; }
        // if a user uploads a file with comparison profiles we store the filename here so we can print it out in the reports later
        public string FromFileName { get; set; }

        // this is for degradation. currently, should be disabled
        public enDegradation Degradation { get; set; }
        public enum enDegradation { None = 0, Mild, Severe };

        // this stores which version of the FST was used to run this comparison
        public string Version { get; set; }

        // these are the LRs, the result of our calculation
        public float AsianLR { get; set; }
        public float BlackLR { get; set; }
        public float CaucasianLR { get; set; }
        public float HispanicLR { get; set; }

        // this field is the processed status of this comparison
        public char Processed { get; set; }

        // this flag is configured in the configureComparison() method and tells the code whether the comparison should run in the background for an individual comparison
        public bool RunIndividualOnService { get; set; }

        // these classes hold configuration data for the numerator and denominator profiles and number of unknowns
        public Contributors NumeratorProfiles { get; set; }
        public Contributors DenominatorProfiles { get; set; }

        public class Contributors
        {
            public Contributors()
            {
                ComparisonIDs = new List<int>();
                KnownIDs = new List<int>();
            }

            // list of IDs to be included in the comparison dictionary
            public List<int> ComparisonIDs { get; set; }
            public int ComparisonCount { get { if (ComparisonIDs != null) return ComparisonIDs.Count; else return 0; } }

            // list of IDs to be included in the comparison dictionary
            public List<int> KnownIDs { get; set; }
            public int KnownCount { get { if (KnownIDs != null) return KnownIDs.Count; else return 0; } }

            // number of unknowns for which we generate genotypes form permutations of the evidence alleles
            public int UnknownCount { get; set; }
        }

        /// <summary>
        /// This class copies the evidence replicate and profile data from the Dictionaries to the associated DictionaryProxies, 
        /// and then serializes the ComparisonData class to a string using XmlSerializer
        /// </summary>
        /// <returns>XML serialized version of this ComparisonData instance</returns>
        public string Serialize()
        {
            // copy the evidence dictionary
            Dictionary<string, DictionaryProxy<int, string>> tmpEvidenceAllelesProxy = new Dictionary<string, DictionaryProxy<int, string>>();
            foreach (string key in EvidenceAlleles.Keys)
            {
                Dictionary<int, string> curDict = EvidenceAlleles[key];
                DictionaryProxy<int, string> tmpDict = new DictionaryProxy<int, string>(curDict);
                tmpEvidenceAllelesProxy.Add(key, tmpDict);
            }
            EvidenceAllelesProxy = new DictionaryProxy<string, DictionaryProxy<int, string>>(tmpEvidenceAllelesProxy);

            // copy the comparisons dictionary
            Dictionary<int, DictionaryProxy<string, string>> tmpComparisonAllelesProxy = new Dictionary<int, DictionaryProxy<string, string>>();
            foreach (int key in ComparisonAlleles.Keys)
            {
                Dictionary<string, string> curDict = ComparisonAlleles[key];
                DictionaryProxy<string, string> tmpDict = new DictionaryProxy<string, string>(curDict);
                tmpComparisonAllelesProxy.Add(key, tmpDict);
            }
            ComparisonAllelesProxy = new DictionaryProxy<int, DictionaryProxy<string, string>>(tmpComparisonAllelesProxy);

            // copy the knowns dictionary
            Dictionary<int, DictionaryProxy<string, string>> tmpKnownsAllelesProxy = new Dictionary<int, DictionaryProxy<string, string>>();
            foreach (int key in KnownsAlleles.Keys)
            {
                Dictionary<string, string> curDict = KnownsAlleles[key];
                DictionaryProxy<string, string> tmpDict = new DictionaryProxy<string, string>(curDict);
                tmpKnownsAllelesProxy.Add(key, tmpDict);
            }
            KnownsAllelesProxy = new DictionaryProxy<int, DictionaryProxy<string, string>>(tmpKnownsAllelesProxy);

            // serialize
            XmlSerializer xsr = new XmlSerializer(typeof(ComparisonData));
            var sr = new StringWriter();
            xsr.Serialize(sr, this);
            return sr.ToString();
        }

        /// <summary>
        /// This static method will deserialize an instance of ComparisonData from an XML string source.
        /// </summary>
        /// <param name="XML">String parameter that contains the XML-serialized representation of a ComparisonData class.</param>
        /// <returns>ComparisonData instance deserialized from parameter.</returns>
        public static ComparisonData Deserialize(string XML)
        {
            ComparisonData val = null;

            // deserialize the class from XML
            XmlSerializer xsr = new XmlSerializer(typeof(ComparisonData));
            using(TextReader tr = new StringReader(XML))
            {
                val = (ComparisonData) xsr.Deserialize(tr);
            }

            // copy the evidence 
            Dictionary<string, DictionaryProxy<int, string>> tmpEvidenceAllelesProxy = val.EvidenceAllelesProxy.ToDictionary();
            foreach (string key in tmpEvidenceAllelesProxy.Keys)
            {
                Dictionary<int, string> curDict = tmpEvidenceAllelesProxy[key].ToDictionary();
                val.EvidenceAlleles.Add(key, curDict);
            }

            // copy the comparisons
            Dictionary<int, DictionaryProxy<string, string>> tmpComparisonAllelesProxy = val.ComparisonAllelesProxy.ToDictionary();
            foreach (int key in tmpComparisonAllelesProxy.Keys)
            {
                Dictionary<string, string> curDict = tmpComparisonAllelesProxy[key].ToDictionary();
                val.ComparisonAlleles.Add(key, curDict);
            }

            // copy the knowns
            Dictionary<int, DictionaryProxy<string, string>> tmpKnownsAllelesProxy = val.KnownsAllelesProxy.ToDictionary();
            foreach (int key in tmpKnownsAllelesProxy.Keys)
            {
                Dictionary<string, string> curDict = tmpKnownsAllelesProxy[key].ToDictionary();
                val.KnownsAlleles.Add(key, curDict);
            }

            return val;
        }

        /// <summary>
        /// This class prepares a DataSet that is passed into the ReportPrinter classes to print out a copy of the report.
        /// </summary>
        /// <returns>A DataSet with DataTables populated with the data for printing.</returns>
        public DataSet Print()
        {
            // we dynamically generate the locus columns based on this lab kit
            DataTable dtLocus = new Business_Interface().GetLocus(this.LabKitID);
            List<string> loci = new List<string>();
            foreach (DataRow dr in dtLocus.Rows)
                loci.Add(dr["FieldName"].ToString().ToUpper());

            DataSet val = new DataSet("PrintDataSet");

            // table with report parameters
            DataTable dtParams = new DataTable("tblParameters");
            dtParams.Columns.Add("FBNo", typeof(string));
            dtParams.Columns.Add("RefNo", typeof(string));
            dtParams.Columns.Add("ItemNo", typeof(string));
            dtParams.Columns.Add("SuspectNo", typeof(string));
            dtParams.Columns.Add("DropOut", typeof(string));
            dtParams.Columns.Add("CreatedBy", typeof(string));
            dtParams.Columns.Add("Hp", typeof(string));
            dtParams.Columns.Add("Hd", typeof(string));
            dtParams.Columns.Add("Deducible", typeof(string));
            dtParams.Columns.Add("Degradation", typeof(string));

            DataRow drParams = dtParams.NewRow();
            drParams["FBNo"] = this.FB1;
            drParams["RefNo"] = this.FB2;
            drParams["ItemNo"] = this.Item;
            drParams["SuspectNo"] = this.Comparison;
            drParams["DropOut"] = this.DNAAmount;
            drParams["CreatedBy"] = this.UserName;
            drParams["Hp"] = this.HpHead;
            drParams["Hd"] = this.HdHead;
            drParams["Deducible"] = this.Deducible ? "Yes" : "No";
            drParams["Degradation"] = this.Degradation == enDegradation.None ? "ND" : this.Degradation == enDegradation.Mild ? "MD" : "SD";

            dtParams.Rows.Add(drParams);
            
            // table with the comaprison profiles/evidence
            DataTable dtAlleles = new DataTable("tblAlleles");
            dtAlleles.Columns.Add("Type", typeof(string));
            dtAlleles.Columns.Add("Profile", typeof(string));
            // we dynamically generate the locus columns based on this lab kit
            foreach (string locus in loci) dtAlleles.Columns.Add(locus);

            // copy the comparisons alleles from the dictionaries
            if (this.ComparisonAlleles.Count >= 1)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = this.Comparison1Name != string.Empty ? string.Format("{0} (Comparison)", this.Comparison1Name) : string.Format("Comparison{0}", this.NumeratorProfiles.ComparisonCount > 1 ? " 1" : string.Empty);
                dr["Profile"] = string.Empty;
                foreach (string locus in loci) if(ComparisonAlleles[1].ContainsKey(locus)) dr[locus] = ComparisonAlleles[1][locus];
                dtAlleles.Rows.Add(dr);
            }

            if (this.ComparisonAlleles.Count >= 2)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = this.Comparison2Name != string.Empty ? string.Format("{0} (Comparison)", this.Comparison2Name) : "Comparison 2";
                dr["Profile"] = string.Empty;
                foreach (string locus in loci) if (ComparisonAlleles[2].ContainsKey(locus)) dr[locus] = ComparisonAlleles[2][locus];
                dtAlleles.Rows.Add(dr);
            }

            if (this.ComparisonAlleles.Count >= 3)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = this.Comparison3Name != string.Empty ? string.Format("{0} (Comparison)", this.Comparison2Name) : "Comparison 3";
                dr["Profile"] = string.Empty;
                foreach (string locus in loci) if (ComparisonAlleles[3].ContainsKey(locus)) dr[locus] = ComparisonAlleles[3][locus];
                dtAlleles.Rows.Add(dr);
            }

            if (this.ComparisonAlleles.Count >= 4)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = this.Comparison4Name != string.Empty ? string.Format("{0} (Comparison)", this.Comparison2Name) : "Comparison 4";
                dr["Profile"] = string.Empty;
                foreach (string locus in loci) if (ComparisonAlleles[4].ContainsKey(locus)) dr[locus] = ComparisonAlleles[4][locus];
                dtAlleles.Rows.Add(dr);
            }

            // copy the knowns alleles from the dictionaries
            if (this.KnownsAlleles.Count >= 1)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = this.Known1Name != string.Empty ? string.Format("{0} (Known)", this.Known1Name) : string.Format("Known{0}", this.NumeratorProfiles.KnownCount > 1 ? " 1" : string.Empty);
                dr["Profile"] = string.Empty;
                foreach (string locus in loci) if (KnownsAlleles[1].ContainsKey(locus)) dr[locus] = KnownsAlleles[1][locus];
                dtAlleles.Rows.Add(dr);
            }
            if (this.KnownsAlleles.Count >= 2)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = this.Known2Name != string.Empty ? string.Format("{0} (Known)", this.Known2Name) : string.Format("Known 2");
                dr["Profile"] = string.Empty;
                foreach (string locus in loci) if (KnownsAlleles[2].ContainsKey(locus)) dr[locus] = KnownsAlleles[2][locus];
                dtAlleles.Rows.Add(dr);
            }
            if (this.KnownsAlleles.Count >= 3)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = this.Known3Name != string.Empty ? string.Format("{0} (Known)", this.Known3Name) : string.Format("Known 3");
                dr["Profile"] = string.Empty;
                foreach (string locus in loci) if (KnownsAlleles[3].ContainsKey(locus)) dr[locus] = KnownsAlleles[3][locus];
                dtAlleles.Rows.Add(dr);
            }
            if (this.KnownsAlleles.Count >= 4)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = this.Known4Name != string.Empty ? string.Format("{0} (Known)", this.Known4Name) : string.Format("Known 4");
                dr["Profile"] = string.Empty;
                foreach (string locus in loci) if (KnownsAlleles[4].ContainsKey(locus)) dr[locus] = KnownsAlleles[4][locus];
                dtAlleles.Rows.Add(dr);
            }

            // copy the evidence from the dictionaries
            List<DataRow> drEvidenceRows = new List<DataRow>();
            for (int replicate = 1; replicate <= 3; replicate++)
            {
                DataRow dr = dtAlleles.NewRow();
                dr["Type"] = "Evidence";
                dr["Profile"] = replicate.ToString();
                drEvidenceRows.Add(dr);
            }

            foreach (string locus in loci)
                foreach (int replicate in EvidenceAlleles[locus].Keys)
                    if(EvidenceAlleles.ContainsKey(locus))
                        drEvidenceRows[replicate - 1][locus] = EvidenceAlleles[locus][replicate].Replace(",", ", "); // here we add a space after the comma so that alleles are properly separated

            foreach (DataRow dr in drEvidenceRows)
                dtAlleles.Rows.Add(dr);

            // copy the result data and format it accordingly (scientific notation for values less than 0.01 or greater than 1000000)
            DataTable dtResult = new DataTable("tblResults");
            dtResult.Columns.Add("Asian");
            dtResult.Columns.Add("Black");
            dtResult.Columns.Add("Caucasian");
            dtResult.Columns.Add("Hispanic");

            dtResult.Rows.Add(new object[] 
            { 
                (this.AsianLR < 0.1 || this.AsianLR >= 1000 ? String.Format("{0:#.##e+00}", this.AsianLR) : Convert.ToString(Math.Round(this.AsianLR, 2))), 
                (this.BlackLR < 0.1 || this.BlackLR >= 1000 ? String.Format("{0:#.##e+00}", this.BlackLR) : Convert.ToString(Math.Round(this.BlackLR, 2))), 
                (this.CaucasianLR < 0.1 || this.CaucasianLR >= 1000 ? String.Format("{0:#.##e+00}", this.CaucasianLR) : Convert.ToString(Math.Round(this.CaucasianLR, 2))), 
                (this.HispanicLR < 0.1 || this.HispanicLR >= 1000 ? String.Format("{0:#.##e+00}", this.HispanicLR) : Convert.ToString(Math.Round(this.HispanicLR, 2)))
            });

            val.Tables.Add(dtParams);
            val.Tables.Add(dtAlleles);
            val.Tables.Add(dtResult);

            return val;
        }

        /// <summary>
        /// This just serializes our class. Used when logging the session data.
        /// </summary>
        /// <returns>XMLSerialized version of the class</returns>
        public override string ToString()
        {
            return this.Serialize();
        }
    }
}


