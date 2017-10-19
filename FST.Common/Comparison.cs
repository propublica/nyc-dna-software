#define DEBUGOUT // comment this to turn debugout off for production environments where running this code is useless and possibly slow

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Globalization;
using System.IO;
using System.Diagnostics;

/// <summary>
/// This is the class where all the action happens. The comparison is described within in full detail. 
/// </summary>
namespace FST.Common
{

    public class Comparison
    {
        #region Debugout Class
#if DEBUGOUT
        // holds race as key, one copy of the debug data per race
        public Dictionary<string, DebugData> debugData;// = new Dictionary<string, DebugData>();
        bool debugDoingNumeartor = true;
        /// <summary>
        /// This class holds debug data for the comparison, and is used to write this data to a file in the DebugOut folder.
        /// It will only work for one locus in an individual comparison, and was left enabled only at the manual entry screen. Usually, to use the debugOut functionality,
        /// the user will enter the evidence, comparison, and known profiles for one locus and for an individual comparison at the manual entry screen.
        /// The output is done for each of the races. See the DebugPrintDataToFile() method for more details on printing. 
        /// See the DoCompare() and NumeratorDenominatorCalculation() methods for how this class is filled with data.
        /// </summary>
        public class DebugData
        {
            // dropins
            public float Pc0, Pc1, Pc2;
            // numerator dropouts
            public float npHet0, npHet1, npHet2, npHom0, npHom1;
            // denominator dropouts
            public float dpHet0, dpHet1, dpHet2, dpHom0, dpHom1;
            // frequencies
            public DataTable alleleFrequencies;

            // these hold any profile, whether they're a comparison, a known, or a permutation of the evidence
            public string Profile1;
            public string Profile2;
            public string Profile3;
            public string Profile4;

            // these are the evidence alleles for each replicate
            public string Evidence1;
            public string Evidence2;
            public string Evidence3;

            // these are parameters for the comparison
            public string Deducible;
            public string ComparisonType;
            public string DNAQuantity;

            // these hold the numerator and denominator factors
            public double Numerator = 1;
            public double Denominator = 1;
            // this is the final result of the comparison
            public double LR;

            // numerator and denominator rows with data (evidence permutation/known/comparison profiles, associated frequencies, dropouts, and dropins, and )
            public List<DebugRow> NumeratorRows = new List<DebugRow>();
            public List<DebugRow> DenominatorRows = new List<DebugRow>();

            public class DebugRow
            {
                // evidence permutations, known, or comparison profile data
                public string genotype1;
                public string genotype2;
                public string genotype3;
                public string genotype4;
                // freuencies associated with the profile. frequencies are only used for permutations of the evidence, never for knowns or comparisons
                public float freq1 = 1;
                public float freq2 = 1;
                public float freq3 = 1;
                public float freq4 = 1;
                // dropin and dropout data per replicate of evidence
                public Replicate rep1;
                public Replicate rep2;
                public Replicate rep3;
                // row product and running sum
                public float product;
                public double total;

                public class Replicate
                {
                    public float dropout1;
                    public float dropout2;
                    public float dropout3;
                    public float dropout4;
                    public float dropin;
                }
            }
        }
#endif
        #endregion

        // name of the user running the comparison. this is only used for the debugout file writing at the moment.
        public string UserName = string.Empty;
        // dictionaries holding allele data for the evidence and the comparison and known profiles
        Dictionary<string, Dictionary<int, string>> evidenceAlleles { get; set; }
        Dictionary<int, Dictionary<string, string>> comparisonAlleles { get; set; }
        Dictionary<int, Dictionary<string, string>> knownAlleles { get; set; }
        // list of loci that are used in the comparison. in this case, comparison actually means the entire calculation, not the comparison profile.
        // this list is used to match the loci that are found in the evidence, all comparison profiles, and all known profiles. only those loci are compared.
        List<string> comparisonLoci = new List<string>();
        // the dictionary that gets returned as a result of the DoCompare() method
        Dictionary<string, float> DNAComparsionResult { get; set; }
        // the string representation of the DNA template amount that is sent from the UI
        string dropoutOption;
        // drop-in rates used for the comparison
        float Pc0, Pc1, Pc2;
        // tables that store the dropOut rates for the lab kit for the numerator and denominator. these differ based on the number of contributors in the scenario
        // so they may be different in a comparison such as C+3U/3U where there are four contributors to the numerator and three contributors to the denominator
        public DataTable numeratorDropOutRateTable;
        public DataTable denominatorDropOutRateTable;
        // dictionaries that contain the numerator and demoninator permutation caches for the comparison so we don't need to regnerate them each time
        public Dictionary<string, Dictionary<int, List<AllelesPair>>> numeratorPermutationBulkCache { get; set; }
        public Dictionary<string, Dictionary<int, List<AllelesPair>>> denominatorPermutationBulkCache { get; set; }
        // a list of races which are used for the comparison. these come from the database
        static DataTable raceTable = null;
        // this is the number of replicates we have in the evidence. 
        const int replicates = 3;
        // database 
        Database myDb;
        // data class containing all the comparison scenario configuration data including all the known alleles, etc.
        ComparisonData comparisonData;

        /// <summary>
        /// This struct holds a pair of alleles for the comparison. This is the genotype of a comaprison, a known profile, or an evidence permutation (unknown profile) at a locus
        /// </summary>
        public struct AllelesPair
        {
            public string alleles; // should be two alleles separated by a comma
            public bool fromEvidence; // indicated whether this is an evidence permutation (unknown profile) or a comparison/known profile
        }

        public Comparison(ComparisonData comparisonData)
        {
            this.comparisonData = comparisonData;

            // set up local references to our comparison allele data (evidence and known profiles and comparison profiles)
            Dictionary<string, Dictionary<int, string>> unknown = comparisonData.EvidenceAlleles;
            Dictionary<int, Dictionary<string, string>> comparison = comparisonData.ComparisonAlleles;
            Dictionary<int, Dictionary<string, string>> known = comparisonData.KnownsAlleles;
            // copy the relevant data from the comparison data strucutres to some local variables
            int compareMethod = comparisonData.CompareMethodID;
            dropoutOption = comparisonData.DNAAmount.ToString();
            string labKitID = comparisonData.LabKitID.ToString();
            string strDeducible = comparisonData.Deducible ? "Yes" : "No";
            string deducible = strDeducible;

            // if we have no evidence or no comparison then this can't work. maybe we should throw an exception here??
            if (unknown == null || comparison == null || comparison.Count == 0)
                return;

            ReadDropinRate(dropoutOption, labKitID);

            // get the number of persons involved
            int numPersonsInvolved = comparisonData.NumeratorProfiles.ComparisonCount + comparisonData.NumeratorProfiles.KnownCount + comparisonData.NumeratorProfiles.UnknownCount;
            int denPersonsInvolved = comparisonData.DenominatorProfiles.ComparisonCount + comparisonData.DenominatorProfiles.KnownCount + comparisonData.DenominatorProfiles.UnknownCount;

            // get the dropout rates from the database (see the ReadDropOut() method for how the rates are adjusted using linear regression for the current DNA template amount)
            numeratorDropOutRateTable = ReadDropoutRate(dropoutOption, numPersonsInvolved, numPersonsInvolved > 1 ? strDeducible : "Yes", labKitID);
            denominatorDropOutRateTable = ReadDropoutRate(dropoutOption, denPersonsInvolved, denPersonsInvolved > 1 ? strDeducible : "Yes", labKitID);

            // we set up our local allele dictionary copies. 
            // we make sure that we only compare the loci where there are alleles in the evidence, the comparison profiles, and the known profiles
            // we ignore the loci where some of the above, but not all, have alleles. 
            comparisonAlleles = new Dictionary<int, Dictionary<string, string>>();
            evidenceAlleles = new Dictionary<string, Dictionary<int, string>>();
            knownAlleles = new Dictionary<int, Dictionary<string, string>>();

            // this is our definitive list of which loci are included in the comparison
            comparisonLoci = new List<string>();
            DataTable dtLoci = new Business_Interface().GetLocus(comparisonData.LabKitID);

            // we go through each of the loci in the lab kit (retrieved from the DB above)
            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString().ToUpper();
                // this is the flag that confirms we are excluding this locus. we start with the assumption that we don't
                bool excludeLocus = false;

                // if this locus has no alleles in any of the comparison profiles, then we exclude it
                foreach (int id in comparison.Keys)
                    if (comparison[id][locus].Trim() == string.Empty)
                        excludeLocus = true;

                // if this locus has no alleles in any of the known profiles, then we exclude it
                foreach (int id in known.Keys)
                    if (known[id][locus].Trim() == string.Empty)
                        excludeLocus = true;

                // if this locus has no alleles in any of the evidence replicates, then we exclude it
                bool notInAnyReplicate = true;
                for (int i = 1; i <= replicates; i++)
                    if (unknown[locus][i].Trim() != string.Empty && unknown[locus][i] != "NEG")
                        notInAnyReplicate = false;
                if (notInAnyReplicate)
                    excludeLocus = true;

                // if we're not exlcuding this locus, then we copy the data to the local dictionary copy form the comparison data class
                if (!excludeLocus)
                {
                    // copy the comparison profiles (comparisons are usually only in the numerator)
                    for (int i = 1; i <= comparisonData.NumeratorProfiles.ComparisonCount; i++)
                    {
                        if (!comparisonAlleles.ContainsKey(i))
                            comparisonAlleles.Add(i, new Dictionary<string, string>());
                        comparisonAlleles[i].Add(locus, comparisonData.ComparisonAlleles[i][locus]);
                    }

                    // we check which of the known counts is higher, numerator or denominator, and we copy all the profiles
                    int knownCount = (comparisonData.NumeratorProfiles.KnownCount > comparisonData.DenominatorProfiles.KnownCount)
                                        ? comparisonData.NumeratorProfiles.KnownCount 
                                        : comparisonData.DenominatorProfiles.KnownCount;
                    for (int i = 1; i <= knownCount; i++)
                    {
                        if (!knownAlleles.ContainsKey(i))
                            knownAlleles.Add(i, new Dictionary<string, string>());
                        knownAlleles[i].Add(locus, comparisonData.KnownsAlleles[i][locus]);
                    }

                    // we got through each of the replicates and copy the alleles from the evidence
                    for (int i = 1; i <= replicates; i++)
                    {
                        if (!evidenceAlleles.ContainsKey(locus)) evidenceAlleles.Add(locus, new Dictionary<int, string>());
                        evidenceAlleles[locus].Add(i, comparisonData.EvidenceAlleles[locus][i]);
                    }

                    // we add the locus to our list of loci included in this comparison
                    comparisonLoci.Add(locus);
                }
            }
        }

        /// <summary>
        /// This function checks for the total frequencies according to races and removes the allelles from calculation
        /// if the sum of frequencies are greater than 0.97.
        /// </summary>
        public void CheckFrequencyForRemoval(DataTable dtFrequencies)
        {
            // if our db connection isn't initialized, do it. then, get all the ethnicities (races)
            myDb = myDb ?? new Database();
            DataTable raceTable = myDb.getAllEthnics();
            int intsr = 0;
            string[] srem = new string[comparisonLoci.Count];

            // we go through all the comparison loci and check whether the sum of the frequencies for that locus is greater than 0.97.
            // if it is, we remove the locus. frequencies are only used for the alleles in the evidence replicates.
            for (int i = 0; i < comparisonLoci.Count; i++)
            {
                bool blRemove = false;
                // get a CSV list of alleles for all the replicates at a locus
                IEnumerable<string> unknownPair = EvidenceAllelesAtLocus(evidenceAlleles[comparisonLoci[i]]);
                // check if the frequency is greater than 0.97 for any of the races. frequencies are values for an allele at a locus for a certain race
                foreach (DataRow eachRow in raceTable.Rows)
                {
                    string raceName = eachRow.Field<string>("EthnicName");
                    float freqSum = GetFrenquencySum(unknownPair, comparisonLoci[i], raceName, dtFrequencies);

                    if (freqSum >= 0.97)
                    {
                        blRemove = true;
                        break;
                    }
                }
                if (blRemove)
                {
                    srem[intsr] = comparisonLoci[i];
                    intsr++;
                }
            }

            // now we iterate through all the loci and remove them from the list of comparison loci, the evidence, and known and comparison profiles
            for (int i = 0; i < srem.Length; i++)
            {
                if (srem[i] != null)
                {
                    string locus = srem[i];

                    // remove the locus from the comparisons
                    for (int j = 1; j <= comparisonData.NumeratorProfiles.ComparisonCount; j++)
                    {
                        if(comparisonAlleles[j].ContainsKey(locus))
                            comparisonAlleles[j].Remove(locus);
                    }

                    // remove the locus from the knowns
                    int knownCount = (comparisonData.NumeratorProfiles.KnownCount > comparisonData.DenominatorProfiles.KnownCount)
                                        ? comparisonData.NumeratorProfiles.KnownCount
                                        : comparisonData.DenominatorProfiles.KnownCount;
                    for (int j = 1; j <= knownCount; j++)
                    {
                        if(knownAlleles[j].ContainsKey(locus))
                            knownAlleles[j].Remove(locus);
                    }

                    // remove the locus from the evidence replicates
                    for (int j = 1; j <= replicates; j++)
                    {
                        if(evidenceAlleles.ContainsKey(locus))
                            evidenceAlleles.Remove(locus);
                    }

                    // remove the locus from the list of comparison loci
                    comparisonLoci.Remove(locus);
                }
            }
        }


        // set of stopwatches used for performance monitoring. some of these are not being used, so check the code before using them. change this comment if that's no longer the case.
        static Stopwatch swFreq = new Stopwatch();
        static Stopwatch swDropOut = new Stopwatch();
        static Stopwatch swDropIn = new Stopwatch();
        static Stopwatch swPerms = new Stopwatch();
        static Stopwatch swNumerator = new Stopwatch();
        static Stopwatch swGetData = new Stopwatch();
        static Stopwatch swOtherStuff = new Stopwatch();
        static bool startStopwatches = false;

        /// <summary>
        /// This function loops according to the race calculating the frequency, the numerator (Pn), the denominator (Pd)
        /// and lastly the Ratio (P) for each race.
        /// </summary>
        /// <param name="Theta"></param>
        /// <param name="compareMethod"></param>
        /// <param name="intProfileNo"></param>
        /// <returns></returns>
        public Dictionary<string, float> DoCompare(
            DataTable dtFrequencies = null,
            Dictionary<string, Dictionary<string, double>> perRaceLocusDenominatorCache = null,
            Dictionary<string, Dictionary<string, Dictionary<string, float>>> perRaceLocusFrequencyCache = null,
            Dictionary<int, Dictionary<string, Dictionary<string, float>>> perReplicateDropOutCache = null,
            Dictionary<string, Dictionary<int, List<AllelesPair>>> numeratorPermutationCache = null,
            Dictionary<string, Dictionary<int, List<AllelesPair>>> denominatorPermutationCache = null)
        {
            // set up our result dictionary, database connection, and race table
            DNAComparsionResult = new Dictionary<string, float>();
            myDb = myDb ?? new Database();
            raceTable = raceTable ?? myDb.getAllEthnics();

            int compareMethod = comparisonData.CompareMethodID;
            // check to see if we should remove any loci based on the fact that the frequencies for the alleles at that locus across all replicates sum up to a value greater than 0.97
            CheckFrequencyForRemoval(dtFrequencies);

            // here we iterate to find the LR for each of the races (they are different because the frequency values are different per-race)
            foreach (DataRow eachRow in raceTable.Rows)
            {
                string raceName = eachRow.Field<string>("EthnicName");

                #region DebugOut adding report parameters
#if DEBUGOUT
                if (debugData != null)
                    try
                    {
                        if (debugData.ContainsKey(raceName))
                            debugData.Remove(raceName);
                        debugData.Add(raceName, new DebugData());
                        debugData[raceName].Deducible = comparisonData.Deducible ? "Yes" : "No";
                        debugData[raceName].DNAQuantity = dropoutOption;
                        debugData[raceName].ComparisonType = Convert.ToString(myDb.GetCaseTypes().Select("FieldValue=" + compareMethod)[0]["FieldName"]);
                    }
                    catch { }
#endif
                #endregion

                double P = 1;
                for (int i = 0; i < comparisonLoci.Count; i++)
                {
                    // initialize the permutation cache
                    if (!cachedNumeratorPermutations.ContainsKey(comparisonLoci[i])) cachedNumeratorPermutations.Add(comparisonLoci[i], null);
                    if (!cachedDenominatorPermutations.ContainsKey(comparisonLoci[i])) cachedDenominatorPermutations.Add(comparisonLoci[i], null);

                    if (startStopwatches) swGetData.Start();
                    // get the dropout rates for this locus from the table of numerator dropout rates (which contains all loci)
                    Dictionary<string, float> curDropoutRate = DropOutRateForLocus(comparisonLoci[i], numeratorDropOutRateTable);
                    // get the CSV list of alleles at this locus in the evidence across all replicates
                    IEnumerable<string> unknownPair = EvidenceAllelesAtLocus(evidenceAlleles[comparisonLoci[i]]);
                    // get the frequencies for the alleles at this locus in the evidence for this race
                    DataTable tblfreq = GetFrenquencyTable(unknownPair, comparisonLoci[i], raceName, dtFrequencies);
                    if (tblfreq == null)
                        continue;
                    #region DebugOut writing alleles, dropin rates, and requencies
#if DEBUGOUT
                    if (debugData != null)
                        try
                        {
                            debugData[raceName].alleleFrequencies = tblfreq;
                            debugData[raceName].Pc0 = Pc0;
                            debugData[raceName].Pc1 = Pc1;
                            debugData[raceName].Pc2 = Pc2;
                            debugData[raceName].Evidence1 = evidenceAlleles[comparisonLoci[i]][1];
                            debugData[raceName].Evidence2 = evidenceAlleles[comparisonLoci[i]][2];
                            debugData[raceName].Evidence3 = evidenceAlleles[comparisonLoci[i]][3];
                            foreach (int key in comparisonAlleles.Keys)
                            {
                                if (key == 1) debugData[raceName].Profile1 = comparisonAlleles[key][comparisonLoci[i]];
                                if (key == 2) debugData[raceName].Profile2 = comparisonAlleles[key][comparisonLoci[i]];
                                if (key == 3) debugData[raceName].Profile3 = comparisonAlleles[key][comparisonLoci[i]];
                                if (key == 4) debugData[raceName].Profile4 = comparisonAlleles[key][comparisonLoci[i]];
                            }
                            if (knownAlleles != null) foreach (int key in knownAlleles.Keys)
                                {
                                    if (string.IsNullOrEmpty(debugData[raceName].Profile1)) { debugData[raceName].Profile1 = knownAlleles[key][comparisonLoci[i]]; continue; }
                                    if (string.IsNullOrEmpty(debugData[raceName].Profile2)) { debugData[raceName].Profile2 = knownAlleles[key][comparisonLoci[i]]; continue; }
                                    if (string.IsNullOrEmpty(debugData[raceName].Profile3)) { debugData[raceName].Profile3 = knownAlleles[key][comparisonLoci[i]]; continue; }
                                    if (string.IsNullOrEmpty(debugData[raceName].Profile4)) { debugData[raceName].Profile4 = knownAlleles[key][comparisonLoci[i]]; continue; }
                                }
                            debugDoingNumeartor = true;
                        }
                        catch { }
#endif
                    #endregion
                    if (startStopwatches) swGetData.Stop();

                    double Pn = 0.0f;
                    if (startStopwatches) swPerms.Start();
                    // get our permutations of the evidence data for this locus. permutations are generated for the unknowns in the comparison scenario.
                    Dictionary<int, List<AllelesPair>> sveAllelesForLocus = cachedNumeratorPermutations[comparisonLoci[i]] ?? GetPermutations(unknownPair, true);
                    if (startStopwatches) swPerms.Stop();

                    if (startStopwatches) swNumerator.Start();
                    // calculate the numerator value for this locus. 
                    // calculates a product of the frequencies, drop out rates, and dropin rates for all the unknown permutations and comparison/known profiles.
                    // the profiles which are used are configured in comparisonData.NumeratorProfiles. check the NumeratorDenominatorCalculation() method for details
                    Pn = NumeratorDenominatorCalculation(true, evidenceAlleles[comparisonLoci[i]], sveAllelesForLocus, curDropoutRate, tblfreq, comparisonData.Theta, comparisonLoci[i], perRaceLocusFrequencyCache, raceName, perReplicateDropOutCache);

                    #region DebugOut writing numerator dropout rates
#if DEBUGOUT
                    if (debugData != null)
                        try
                        {
                            debugData[raceName].npHet0 = curDropoutRate["PHET0"];
                            debugData[raceName].npHet1 = curDropoutRate["PHET1"];
                            debugData[raceName].npHet2 = curDropoutRate["PHET2"];
                            debugData[raceName].npHom0 = curDropoutRate["PHOM0"];
                            debugData[raceName].npHom1 = curDropoutRate["PHOM1"];
                            debugDoingNumeartor = false;
                        }
                        catch { }
#endif
                    #endregion

                    // place our permutations reference in the cache
                    cachedNumeratorPermutations[comparisonLoci[i]] = sveAllelesForLocus;

                    if (startStopwatches) swNumerator.Stop();

                    double Pd = 0.0f;

                    // here we cehck if our denominator cache is in use. if it is, then we get the denominator data from the cache.
                    // the cache is per race and per locus, and if we don't have a value we also go in here to calculate and set it.
                    if (perRaceLocusDenominatorCache == null ||
                        (perRaceLocusDenominatorCache != null && double.IsNaN(perRaceLocusDenominatorCache[raceName][comparisonLoci[i]])))
                    {
                        // get our permutations of the evidence data for this locus. permutations are generated for the unknowns in the comparison scenario.
                        sveAllelesForLocus = cachedDenominatorPermutations[comparisonLoci[i]] ?? GetPermutations(unknownPair, false);

                        // get the dropout rates for this locus from the table of denominator dropout rates (which contains all loci)
                        curDropoutRate = DropOutRateForLocus(comparisonLoci[i], denominatorDropOutRateTable);

                        // calculate the denominator value for this locus. 
                        // calculates a product of the frequencies, drop out rates, and dropin rates for all the unknown permutations and comparison/known profiles.
                        // the profiles which are used are configured in comparisonData.DenominatorProfiles. check the NumeratorDenominatorCalculation() method for details
                        Pd = NumeratorDenominatorCalculation(false, evidenceAlleles[comparisonLoci[i]], sveAllelesForLocus, curDropoutRate, tblfreq, comparisonData.Theta, comparisonLoci[i], perRaceLocusFrequencyCache, raceName, null);

                        #region DebugOut writing denominator dropout rates
#if DEBUGOUT
                        if (debugData != null)
                            try
                            {
                                debugData[raceName].Numerator *= Pn;
                                debugData[raceName].Denominator *= Pd;
                                debugData[raceName].dpHet0 = curDropoutRate["PHET0"];
                                debugData[raceName].dpHet1 = curDropoutRate["PHET1"];
                                debugData[raceName].dpHet2 = curDropoutRate["PHET2"];
                                debugData[raceName].dpHom0 = curDropoutRate["PHOM0"];
                                debugData[raceName].dpHom1 = curDropoutRate["PHOM1"];
                            }
                            catch { }
#endif
                        #endregion

                        cachedDenominatorPermutations[comparisonLoci[i]] = sveAllelesForLocus;

                        // if we're using a denominator cache, set the value in the cache for this race and locus
                        if (perRaceLocusDenominatorCache != null)
                            perRaceLocusDenominatorCache[raceName][comparisonLoci[i]] = Pd;
                    }
                    else // get the value from the cache
                        Pd = perRaceLocusDenominatorCache[raceName][comparisonLoci[i]];

                    // clear out some data that we're not going to use for the GC to handle
                    if (startStopwatches) swOtherStuff.Start();
                    sveAllelesForLocus = null;
                    tblfreq.Dispose();
                    curDropoutRate.Clear();
                    curDropoutRate = null;
                    if (startStopwatches) swOtherStuff.Stop();

                    // multiply our data into the LR value for this race. this is where our result gets compounded
                    if (Pd != 0)
                        P = P * (Pn / Pd);
                }
                // add the result for this race to the result dictionary
                DNAComparsionResult.Add(raceName, (float)P);
                #region DebugOut writing per-race LRs
#if DEBUGOUT
                if (debugData != null)
                    try
                    {
                        debugData[raceName].LR = (float)P;
                    }
                    catch { }
#endif
                #endregion
            }

            #region DebugOut PRINT CALL
#if DEBUGOUT
            if (debugData != null)
                try
                {
                    DebugPrintDataToFile();
                }
                catch { }
#endif
            #endregion

            // sets our LR values for the comparison to the configuration class. used in the print code (for individual comparisons only)
            comparisonData.AsianLR = DNAComparsionResult["Asian"];
            comparisonData.BlackLR = DNAComparsionResult["Black"];
            comparisonData.CaucasianLR = DNAComparsionResult["Caucasian"];
            comparisonData.HispanicLR = DNAComparsionResult["Hispanic"];

            // sets references to our permutation caches for this calculation
            if (numeratorPermutationCache != null) numeratorPermutationBulkCache = cachedNumeratorPermutations;
            if (denominatorPermutationCache != null) denominatorPermutationBulkCache = cachedDenominatorPermutations;

            return DNAComparsionResult;
        }

        #region DebugOut Print Function
#if DEBUGOUT
        /// <summary>
        /// This method prints out the data we gathered for the DebugOut functionality. This is not supposed to be deployed to production, and should only
        /// really be used to enable the users to test the comparison functionality in a staging environment.
        /// </summary>
        public void DebugPrintDataToFile()
        {
            if (debugData != null)
                foreach (string key in debugData.Keys) // this goes through each of the races
                {
                    // we create a directory for the user that is running this. 
                    // if the login functionality is disabled this should fail gracefully by putthing files in the root debugout directory
                    string path = @"C:\work\FST\Web\FST.Web\DebugOut\" + this.UserName.Substring(this.UserName.LastIndexOf(@"\") + 1) + @"\";
                    //string path = @"C:\inetpub\wwwroot\FST_Research\DebugOut\" + this.UserName.Substring(this.UserName.LastIndexOf(@"\") + 1) + @"\";
                    //string path = @"C:\inetpub\FST_Staging\DebugOut\" + this.UserName.Substring(this.UserName.LastIndexOf(@"\") + 1) + @"\";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    // the filename is usually the current time in ticks followed by the race. this could be improved, but has sufficed thus far
                    path += DateTime.Now.Ticks.ToString() + "-" + key + ".csv";

                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        // we write some column headers
                        sw.WriteLine("," + debugData[key].ComparisonType);
                        sw.WriteLine(",Deducible," + debugData[key].Deducible);
                        sw.WriteLine(",DNA Quantity," + debugData[key].DNAQuantity);
                        sw.WriteLine(",");
                        sw.WriteLine(",Allele Frequencies,,,Numerator Dropout Rates,,,,,Drop-in Rates,,,Denominator Dropout Rates");
                        sw.WriteLine(",Allele,Frequency,,Heterozygote,,Homozygote,,,,,,Heterozygote,,Homozygote");

                        // write our frequencies, dropout, and dropin rates
                        for (int i = 0; i < debugData[key].alleleFrequencies.Rows.Count; i++)
                        {
                            sw.Write("," + debugData[key].alleleFrequencies.Rows[i][0] + "," + debugData[key].alleleFrequencies.Rows[i][1] + ",,");

                            if (i == 0) sw.Write("pHet0," + debugData[key].npHet0 + ",");
                            if (i == 1) sw.Write("pHet1," + debugData[key].npHet1 + ",");
                            if (i == 2) sw.Write("pHet2," + debugData[key].npHet2 + ",,,,");

                            if (i == 0) sw.Write("pHom0," + debugData[key].npHom0 + ",,");
                            if (i == 1) sw.Write("pHom1," + debugData[key].npHom1 + ",,");

                            if (i == 0) sw.Write("pC0," + debugData[key].Pc0 + ",,");
                            if (i == 1) sw.Write("pC1," + debugData[key].Pc1 + ",,");
                            if (i == 2) sw.Write("pC2+," + debugData[key].Pc2 + ",,");

                            if (i == 0) sw.Write("pHet0," + debugData[key].dpHet0 + ",");
                            if (i == 1) sw.Write("pHet1," + debugData[key].dpHet1 + ",");
                            if (i == 2) sw.Write("pHet2," + debugData[key].dpHet2 + ",\r\n");

                            if (i == 0) sw.Write("pHom0," + debugData[key].dpHom0 + "\r\n");
                            if (i == 1) sw.Write("pHom1," + debugData[key].dpHom1 + "\r\n");

                            if (i > 2) sw.Write("\r\n");
                        }

                        // write the known profile genotypes, the evidence replicates, and the numerator, denominator, and LR for the comparison
                        sw.WriteLine(",");
                        sw.WriteLine(",Comparison Profile,,,Evidence Profiles,,,,,Final Values");
                        sw.WriteLine(",Known1,\"" + debugData[key].Profile1 + "\",,Rep 1,\"" + debugData[key].Evidence1 + "\",,,,Num," + debugData[key].Numerator);
                        sw.WriteLine(",Known2,\"" + debugData[key].Profile2 + "\",,Rep 2,\"" + debugData[key].Evidence2 + "\",,,,Den," + debugData[key].Denominator);
                        sw.WriteLine(",Known3,\"" + debugData[key].Profile3 + "\",,Rep 3,\"" + debugData[key].Evidence3 + "\"");
                        sw.WriteLine(",Known4,\"" + debugData[key].Profile4 + "\",,,,,,,LR," + debugData[key].LR);
                        sw.WriteLine(",");

                        // some more column headers
                        sw.WriteLine("Numerator,");
                        sw.WriteLine("Gen1,Gen2,Gen3,Gen4,Freq1,Freq2,Freq3,Freq4,Rep1Drop1,Rep1Drop2,Rep1Drop3,Rep1Drop4,Rep1Cont1,Rep2Drop1,Rep2Drop2,Rep2Drop3,Rep2Drop4,Rep2Cont1,Rep3Drop1,Rep3Drop2,Rep3Drop3,Rep3Drop4,Rep3Cont1,Product,Running Total");

                        // write the numerator data including genotypes (allele pair for each contributor), frequencies, and dropout and dropin rates per replicate, as well as the row product and running sum
                        foreach (DebugData.DebugRow dr in debugData[key].NumeratorRows)
                        {
                            sw.Write("\"" + dr.genotype1 + "\",\"" + dr.genotype2 + "\",\"" + dr.genotype3 + "\",\"" + dr.genotype4 + "\"," + dr.freq1 + "," + dr.freq2 + "," + dr.freq3 + "," + dr.freq4 + ",");
                            if (dr.rep1 != null)
                                sw.Write(dr.rep1.dropout1 + "," + dr.rep1.dropout2 + "," + dr.rep1.dropout3 + "," + dr.rep1.dropout4 + "," + dr.rep1.dropin + ",");
                            else
                                sw.Write(",,,,,");
                            if (dr.rep2 != null)
                                sw.Write(dr.rep2.dropout1 + "," + dr.rep2.dropout2 + "," + dr.rep2.dropout3 + "," + dr.rep2.dropout4 + "," + dr.rep2.dropin + ",");
                            else
                                sw.Write(",,,,,");
                            if (dr.rep3 != null)
                                sw.Write(dr.rep3.dropout1 + "," + dr.rep3.dropout2 + "," + dr.rep3.dropout3 + "," + dr.rep3.dropout4 + "," + dr.rep3.dropin + ",");
                            else
                                sw.Write(",,,,,");
                            sw.Write(dr.product + "," + dr.total + "\r\n");
                        }

                        // write some column headers
                        sw.WriteLine(",");
                        sw.WriteLine("Denominator,");
                        sw.WriteLine("Gen1,Gen2,Gen3,Gen4,Freq1,Freq2,Freq3,Freq4,Rep1Drop1,Rep1Drop2,Rep1Drop3,Rep1Drop4,Rep1Cont1,Rep2Drop1,Rep2Drop2,Rep2Drop3,Rep2Drop4,Rep2Cont1,Rep3Drop1,Rep3Drop2,Rep3Drop3,Rep3Drop4,Rep3Cont1,Product,Running Total");

                        // write the denominator data including genotypes (allele pair for each contributor), frequencies, and dropout and dropin rates per replicate, as well as the row product and running sum
                        foreach (DebugData.DebugRow dr in debugData[key].DenominatorRows)
                        {
                            sw.Write("\"" + dr.genotype1 + "\",\"" + dr.genotype2 + "\",\"" + dr.genotype3 + "\",\"" + dr.genotype4 + "\"," + dr.freq1 + "," + dr.freq2 + "," + dr.freq3 + "," + dr.freq4 + ",");
                            if (dr.rep1 != null)
                                sw.Write(dr.rep1.dropout1 + "," + dr.rep1.dropout2 + "," + dr.rep1.dropout3 + "," + dr.rep1.dropout4 + "," + dr.rep1.dropin + ",");
                            else
                                sw.Write(",,,,,");
                            if (dr.rep2 != null)
                                sw.Write(dr.rep2.dropout1 + "," + dr.rep2.dropout2 + "," + dr.rep2.dropout3 + "," + dr.rep2.dropout4 + "," + dr.rep2.dropin + ",");
                            else
                                sw.Write(",,,,,");
                            if (dr.rep3 != null)
                                sw.Write(dr.rep3.dropout1 + "," + dr.rep3.dropout2 + "," + dr.rep3.dropout3 + "," + dr.rep3.dropout4 + "," + dr.rep3.dropin + ",");
                            else
                                sw.Write(",,,,,");
                            sw.Write(dr.product + "," + dr.total + "\r\n");
                        }
                        sw.Flush();
                        sw.Close();
                    }
                }
        }
#endif
        #endregion

        // these dictionaries are where we cache the permutations for the numerator and denominator so we don't have to recalcualte them every time we switch
        // races in the calculation since these stay the same for each race.
        Dictionary<string, Dictionary<int, List<AllelesPair>>> cachedNumeratorPermutations = new Dictionary<string,Dictionary<int,List<AllelesPair>>>();
        Dictionary<string, Dictionary<int, List<AllelesPair>>> cachedDenominatorPermutations = new Dictionary<string, Dictionary<int, List<AllelesPair>>>();

        /// <summary>
        /// This method generates a list of potential genotypes (allele pairs) based on the evidence. Every time a comparison includes an Unknown in the numerator or
        /// denominator or both, we use this method to generate permutations of the alleles in the evidence at a locus, across all the replicates. Each of the permutations
        /// generates a row of values. For ex
        /// </summary>
        /// <param name="unknownPair">List of the unique alleles found at this locus across all evidence replicates.</param>
        /// <param name="doingNumerator">Flag that tells us whether we're doing the numerator or denominator.</param>
        /// <returns></returns>
        protected Dictionary<int, List<AllelesPair>> GetPermutations(IEnumerable<string> unknownPair, bool doingNumerator)
        {
            // this strucutre holds our current allele pair (an allele pair is known as a genotype also)
            AllelesPair strucPair;
            // this is a dictionary which holds a list unknown permutation rows in a list. this could have been a list of lists too...
            Dictionary<int, List<AllelesPair>> locusPermutationRows = new Dictionary<int, List<AllelesPair>>();
            string[] alleles = unknownPair.ToArray();
            // we get our unknown count for our numerator or denominator
            int unknowns = doingNumerator ? comparisonData.NumeratorProfiles.UnknownCount : comparisonData.DenominatorProfiles.UnknownCount;

            // we calculate our permutation count as Pc = n * (n + 1) / 2
            int permutationCount = (alleles.Length * (alleles.Length + 1)) / 2;
            // create an array for our permutations
            string[] permutations = new string[permutationCount];

            // generate a list of permutations to generate the rows
            int counter = 0;
            for (int i = 0; i < alleles.Length; i++)
                for (int j = i; j < alleles.Length; j++)
                    permutations[counter++] = alleles[i] + "," + alleles[j];

            // go through the number of permutation rows which is usually the permutation count raised to the number of unknowns.
            for (int i = 0; i < Math.Pow(permutationCount, unknowns); i++) // cool side-effect: (x^0 |x!=0) = 1, so we get a free row to stick in our knowns in the calc
            {
                List<AllelesPair> row = new List<AllelesPair>();

                // generate each of the genotypes in the row
                for (int j = 1; j <= unknowns; j++)
                {
                    strucPair = new AllelesPair();
                    // calculate permtuation index based on the row id. 
                    // this would be our row id divided by the permutation count raised to the current unknown minus one and then take the remainder from permutation count
                    // for the first unknown, this would be (row id / perm count ^ 0) % perm count.. this gets us row id % perm count which means the permutation index
                    // cycles from 0 to perm count-1. for the next power, 1, it's row id / perm count % perm count, or 0 for the first perm count rows, 1 between permcount
                    // and 2*perm count - 1, and so on... the next power up counts based on permcount^2, and so on..
                    int permIdx = (i / System.Convert.ToInt32(System.Math.Pow(permutationCount, j - 1))) % permutationCount;
                    strucPair.alleles = permutations[permIdx];
                    strucPair.fromEvidence = true;
                    row.Add(strucPair);
                }

                // add the row to our row list
                locusPermutationRows.Add(i, row);
            }

            return locusPermutationRows;
        }
        /// <summary>
        /// This method is where the calculation for each of the rows in the comparison is done. Here, we determine the frequencies for the unknown evidence genotypes, the dropout values
        /// for the evidence, comparison, and known genotypes against the evidence for each replicate, and the dropin for all genotypes against the evidence for each replicate. 
        /// We then compute the product of the frequencies, dropins for all replicates, and dropouts for all replicates. We then add it to a running sum. The result of this is the value
        /// of the numerator or denominator for this locus which is used in the DoCompare() calling function to calculate a LR per race. In this method we also add the comparison and known
        /// profiles into the rows returned from the permutation calculation. It should be noted that the permutation generating method GetPermutations() generates at least one empty row
        /// for us to insert these knowns and comparisons if there are no unknown contributors in the numerator or denominator for this comparison.
        /// </summary>
        /// <param name="doingNumerator">Flag indicating whether we're doing the numerator or denominator.</param>
        /// <param name="evidenceAllele">Dictionary containing the alleles from the evidnce for this locus.</param>
        /// <param name="permutationRows">Dictionary containing lists of unknown permutations for possible genotypes for unknowns generated from the evidence alleles.</param>
        /// <param name="curDropoutRate">Dictionary containing dropout rate information pulled from the database and calculated for this locus.</param>
        /// <param name="tblFreq">Data table containing the frequencies found for the evidence alleles at this locus</param>
        /// <param name="Theta">Theta value for this comparison, entered in the UI</param>
        /// <param name="locus">String containing the name of the locus we are currently processing.</param>
        /// <param name="perRaceLocusFrequencyCache">Cache of frequency values for this locus.</param>
        /// <param name="race">String representing the name of the race we are currently processing.</param>
        /// <param name="perReplicateDropOutCache">Dictionary containing cache of the dropout values for the genotype per locus.</param>
        /// <returns>The computed numerator or denominator value.</returns>
        protected double NumeratorDenominatorCalculation(
            bool doingNumerator, 
            Dictionary<int, string> evidenceAllele, 
            Dictionary<int, List<AllelesPair>> permutationRows, 
            Dictionary<string, float> curDropoutRate, 
            DataTable tblFreq, 
            float Theta, 
            string locus, 
            Dictionary<string, Dictionary<string, Dictionary<string, float>>> perRaceLocusFrequencyCache, 
            string race, 
            Dictionary<int, Dictionary<string, Dictionary<string, float>>> perReplicateDropOutCache)
        {
            //Compute Norminator
            double PI = 0;

            // we only do the frequency calculation if we have an unknown in the current part of the comparison we are processing (numerator or denominator)
            bool doFrequencies = doingNumerator ? comparisonData.NumeratorProfiles.UnknownCount > 0 : comparisonData.DenominatorProfiles.UnknownCount > 0;

            try
            {
                // we iterate through each of the rows in the permutations list, and add in our comparison profiles and known profiles
                // then and do frequency, dropout, dropin, product, and running sums.
                foreach (int index in permutationRows.Keys)
                {
                    #region DebugOut creating new debugrow
#if DEBUGOUT
                    DebugData.DebugRow debugRow = null;
                    if (debugData != null)
                        try
                        {
                            debugRow = new DebugData.DebugRow();
                        }
                        catch { }
#endif
                    #endregion

                    if (startStopwatches) swFreq.Start();
                    // get a copy of the permutations list
                    List<AllelesPair> permutationRowGenotypes = new List<AllelesPair>(permutationRows[index].ToList());

                    // if we're doing the numerator, copy the profiles we use in the numerator. otherwise, use the ones we use in the denominator
                    if (doingNumerator)
                    {
                        // copy the comparisons
                        for (int i = 1; i <= comparisonData.NumeratorProfiles.ComparisonCount; i++)
                            permutationRowGenotypes.Add(new AllelesPair { alleles = comparisonAlleles[i][locus], fromEvidence = false });
                        // copy the knowns
                        for (int i = 1; i <= comparisonData.NumeratorProfiles.KnownCount; i++)
                            permutationRowGenotypes.Add(new AllelesPair { alleles = knownAlleles[i][locus], fromEvidence = false });
                    }
                    else
                    {
                        // copy the comparisons
                        for (int i = 1; i <= comparisonData.DenominatorProfiles.ComparisonCount; i++)
                            permutationRowGenotypes.Add(new AllelesPair { alleles = comparisonAlleles[i][locus], fromEvidence = false });
                        // copy the knowns
                        for (int i = 1; i <= comparisonData.DenominatorProfiles.KnownCount; i++)
                            permutationRowGenotypes.Add(new AllelesPair { alleles = knownAlleles[i][locus], fromEvidence = false });
                    }

                    float fre = 1;
                    // this is going to be a list of the alleles for all the genotypes found on this row (knowns and unknowns)
                    StringBuilder contaminatorString = new StringBuilder();
                    for (int ind = 0; ind < permutationRowGenotypes.Count; ind++)
                    {
                        // add the alleles in the genotypes for each of the contributors in the list
                        contaminatorString.Append(permutationRowGenotypes[ind].alleles);
                        contaminatorString.Append(",");

                        #region DebugOut adding row genotypes
#if DEBUGOUT
                        if (debugData != null)
                            try
                            {
                                if (ind == 0) debugRow.genotype1 = permutationRowGenotypes[ind].alleles;
                                if (ind == 1) debugRow.genotype2 = permutationRowGenotypes[ind].alleles;
                                if (ind == 2) debugRow.genotype3 = permutationRowGenotypes[ind].alleles;
                                if (ind == 3) debugRow.genotype4 = permutationRowGenotypes[ind].alleles;
                            }
                            catch { }
#endif
                        #endregion

                        // if we're doing frequencies and the current genotype is one from the evidence (therefore, one of the unknown permutations) then we do the frequency calc
                        if (doFrequencies && permutationRowGenotypes[ind].fromEvidence)
                        {
                            float freVal = 0.0f;

                            // if we don't have a cache, or we don't have a value in the cache then we get the data from the frequencies table
                            if (perRaceLocusFrequencyCache == null || !perRaceLocusFrequencyCache[race][locus].ContainsKey(permutationRowGenotypes[ind].alleles))
                            {
                                // split the genotype into individual alleles
                                string[] thisPair = permutationRowGenotypes[ind].alleles.Split(',');
                                // this method chooses and performs the correct frequency calculation. 
                                // theta is used in the calculation of homozygous genotypes (this means the same alleles twice)
                                freVal = Frenquency(thisPair[0], thisPair[1], tblFreq, Theta);

                                // if we have a cache, then place the value in the cache
                                if (perRaceLocusFrequencyCache != null)
                                    perRaceLocusFrequencyCache[race][locus].Add(permutationRowGenotypes[ind].alleles, freVal);
                            }
                            else // we pick up the data from the cache
                                freVal = perRaceLocusFrequencyCache[race][locus][permutationRowGenotypes[ind].alleles];

                            #region DebugOut adding row frequencies
#if DEBUGOUT
                            if (debugData != null)
                                try
                                {
                                    if (ind == 0) debugRow.freq1 = freVal;
                                    if (ind == 1) debugRow.freq2 = freVal;
                                    if (ind == 2) debugRow.freq3 = freVal;
                                    if (ind == 3) debugRow.freq4 = freVal;
                                }
                                catch { }
#endif
                            #endregion

                            // multiply the frequencies together. this gets multiplied into the row product eventually
                            fre = fre * freVal;
                        }
                    }

                    if (startStopwatches) swFreq.Stop();

                    // remove last comma
                    if (contaminatorString[contaminatorString.Length - 1].ToString(CultureInfo.CurrentCulture) == ",")
                        contaminatorString.Remove(contaminatorString.Length - 1, 1);

                    float Pa = 1;

                    // we go through each of the replicates and calculate dropin and dropouts
                    for (int replicate = 0; replicate < replicates; replicate++)
                    {
                        if (!string.IsNullOrEmpty(evidenceAllele[replicate + 1])) // if allele is empty, means inconclusive.
                        {
                            if (startStopwatches) swDropOut.Start();
                            // for each of the contributors (comparisons and knowns, and the unknowns from evidence) we calculate a dropout
                            for (int ind = 0; ind < permutationRowGenotypes.Count; ind++)
                            {
                                // we make sure that there is an allele at this location. this probably should never happen
                                if (!String.IsNullOrEmpty(permutationRowGenotypes[ind].alleles))
                                {
                                    float eachPn = 0.0f;

                                    // if we're not using a cache or we don't have the value we seek in the cache then we calculate it
                                    if (perReplicateDropOutCache == null ||
                                        !perReplicateDropOutCache[replicate][locus].ContainsKey(permutationRowGenotypes[ind].alleles))
                                    {
                                        // call to dropout check. this looks in the evidence to see if either or both of the alleles at this genotype have dropped out. 
                                        eachPn = GetGenotypeDropOutRate(evidenceAllele[replicate + 1], permutationRowGenotypes[ind], curDropoutRate);

                                        // if we're using a cache, put the value in the cache
                                        if (perReplicateDropOutCache != null)
                                            perReplicateDropOutCache[replicate][locus].Add(permutationRowGenotypes[ind].alleles, eachPn);
                                    }
                                    else // get the value from the cache
                                        eachPn = perReplicateDropOutCache[replicate][locus][permutationRowGenotypes[ind].alleles];

                                    #region DebugOut adding per-replicate dropouts
#if DEBUGOUT
                                    if (debugData != null)
                                        try
                                        {
                                            DebugData.DebugRow.Replicate rep = null;
                                            if (replicate == 0) rep = debugRow.rep1;
                                            if (replicate == 1) rep = debugRow.rep2;
                                            if (replicate == 2) rep = debugRow.rep3;
                                            if (rep == null) rep = new DebugData.DebugRow.Replicate();

                                            if (ind == 0) rep.dropout1 = eachPn;
                                            if (ind == 1) rep.dropout2 = eachPn;
                                            if (ind == 2) rep.dropout3 = eachPn;
                                            if (ind == 3) rep.dropout4 = eachPn;
                                            if (replicate == 0) debugRow.rep1 = rep;
                                            if (replicate == 1) debugRow.rep2 = rep;
                                            if (replicate == 2) debugRow.rep3 = rep;
                                        }
                                        catch { }
#endif
                                    #endregion

                                    // multiply each of our dropout values into the row product
                                    Pa = Pa * eachPn;
                                }

                            }
                            if (startStopwatches) swDropOut.Stop();

                            if (startStopwatches) swDropIn.Start();
                            string[] evidence = evidenceAllele[replicate + 1].Split(',');
                            // list of the alleles from the known genotypes on this row
                            List<string> contaminators = new List<string>(contaminatorString.ToString().Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries));
                            int extras = 0;
                            // we check the evidence for dropin (also called contamination). this means we check each of the alleles in the evidence to see if they are not
                            // found in any of the genotypes on this row. if they are not found, we pick a dropin rate for this.
                            foreach (string allele in evidence)
                                if (!contaminators.Contains(allele))
                                    extras++;
                            // based on whether we have no dropins, one dropin, or two or more dropins, we pick the appropriate rate
                            float eachCo = GetGenotypeDropInRate(extras);

                            #region DebugOut adding per-replicate dropins
#if DEBUGOUT
                            if (debugData != null)
                                try
                                {
                                    if (replicate == 0) debugRow.rep1.dropin = eachCo;
                                    if (replicate == 1) debugRow.rep2.dropin = eachCo;
                                    if (replicate == 2) debugRow.rep3.dropin = eachCo;
                                }
                                catch { }
#endif
                            #endregion

                            // mutliply our dropin rate (contamination rate) into the row product
                            Pa = Pa * eachCo;

                            if (startStopwatches) swDropIn.Stop();
                        }
                    }

                    if (Pa != 1)
                    {
                        Pa = Pa * fre;  // multiply the frequencies into the row product
                        PI = PI + Pa;   // add the row product to the running sum

                        #region DebugOut writing row product and running sum
#if DEBUGOUT
                        if (debugData != null)
                            try
                            {
                                debugRow.product = Pa;
                                debugRow.total = PI;
                                if (debugDoingNumeartor)
                                    debugData[race].NumeratorRows.Add(debugRow);
                                else
                                    debugData[race].DenominatorRows.Add(debugRow);
                            }
                            catch { }
#endif
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return PI;
        }

        /// <summary>
        /// Gets the dropout rate based on whether none, one, or both of the alleles in a genotype are not found in the evidence replicate.
        /// The dropout rate is determined based on whether the genotype is homozygous or heterozygous, and how many dropouts were detected.
        /// Since homozygous genotypes have the same allele, there can only be one allelic dropout, so PHOM1 represents both of them dropping out. If there is no dropout, PHOM0 is used.
        /// Heterozygous genotypes can have neither, both, or just one allele drop out. This is represented by PHET0 for no dropout, PHET1 for one dropout, or PHET2 for both.
        /// </summary>
        /// <param name="evidence">CSV string of alleles at the evidence replicate in question.</param>
        /// <param name="allelesPair">Genotype (allele pair) fbeing tested for dropout.</param>
        /// <param name="curDropoutRate">Drop out rate dictionary.</param>
        /// <returns></returns>
        private float GetGenotypeDropOutRate(string evidence, AllelesPair allelesPair, Dictionary<string, float> curDropoutRate)
        {
            float val = 1.0f;
            // get data into lists
            List<string> lstEvidence = evidence.Split(',').ToList();
            List<string> lstAlleles = allelesPair.alleles.Split(',').ToList();

            // blow up if the profile alleles are missing
            if (lstAlleles.Count != 2) throw new Exception("Invalid Allele Pair: " + allelesPair.alleles);

            // count how many profile alleles are missing from the evidence alleles
            int dropOut = 0;
            foreach (string allele in lstAlleles)
                if (!lstEvidence.Contains(allele))
                    dropOut++;

            // if we have a homozygous genotype, get the homozygous values
            if (lstAlleles[0] == lstAlleles[1])
                if (dropOut == 0)
                    val = curDropoutRate["PHOM0"];
                else
                    val = curDropoutRate["PHOM1"];
            else  // if we have a heterozygous genotype, get the heterozygous values (obviously)
                switch (dropOut)
                {
                    default:
                    case 0: val = curDropoutRate["PHET0"]; break;
                    case 1: val = curDropoutRate["PHET1"]; break;
                    case 2: val = curDropoutRate["PHET2"]; break;
                }

            return val;
        }

        /// <summary>
        /// This function prepares the evidence alleles list from the evidence replicates based on a locus and also adds an unknown (w) value to it.
        /// </summary>
        /// <param name="thisLocusEvidenceAlleles"></param>
        /// <returns></returns>
        protected IEnumerable<string> EvidenceAllelesAtLocus(Dictionary<int, string> thisLocusEvidenceAlleles)
        {
            // this is going to be our enumerable collection of distinct alleles from all replicates
            IEnumerable<string> unknownPair = null;
            // we build a CSV string with the values from all the replicates
            StringBuilder union = new StringBuilder();
            for (int replicate = 0; replicate < replicates; replicate++)
            {
                if (!String.IsNullOrEmpty(thisLocusEvidenceAlleles[replicate + 1]))
                {
                    if (!String.IsNullOrEmpty(union.ToString()))
                        union.Append(",");
                    if (!String.IsNullOrEmpty(thisLocusEvidenceAlleles[replicate + 1]))
                        union.Append(thisLocusEvidenceAlleles[replicate + 1]);
                }
            }
            // if we have some alleles (at least one) then we append an omega and return the set
            if (!String.IsNullOrEmpty(union.ToString()))
            {
                union.Append(",w");
                // remove the empty entries
                int commaInd = union.ToString().IndexOf(",,", StringComparison.CurrentCulture);
                while (commaInd > 0)
                {
                    union.Replace(",,", ",");
                    commaInd = union.ToString().IndexOf(",,", StringComparison.CurrentCulture);
                }
                // get a list of DISTINCT alleles (Distinct() method being called)
                unknownPair = (IEnumerable<string>)union.ToString().Trim().Split(',').Distinct();

            }
            return unknownPair;
        }

        /// <summary>
        /// This function returns the sum of the frequencies at a locus for a certain race
        /// </summary>
        /// <param name="evidenceAlleles">List of alleles at this locus in the evidence across the replicates.</param>
        /// <param name="locus">The name of the locus on which we are currently working.</param>
        /// <param name="race">The name of the race on which we are currently working.</param>
        /// <param name="dtFrequencies">A datatable with the frequency cache.</param>
        /// <returns>A sum of the frequencies at this locus for this race. The omega (w, unknown) allele is excluded.</returns>
        protected float GetFrenquencySum(IEnumerable<string> evidenceAlleles, string locus, string race, DataTable dtFrequencies)
        {
            DataTable tblFreq = null;
            float sum = 0;
            myDb = myDb ?? new Database();

            // if we have no cache, retrieve the frequency values from the database
            if (dtFrequencies == null)
                tblFreq = myDb.getAllelesFrequence(locus, race, evidenceAlleles);
            else // if we have a cache, pick up the values from the cache
            {
                // get the frequencies for this locus and this race from the cache
                DataRow[] drs = dtFrequencies.Select("race='" + race + "' AND locus='" + locus + "'");

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("AlleleNo", typeof(string)));
                dt.Columns.Add(new DataColumn("freq", typeof(float)));

                // for each one of the evidence alleles, we create a new row, put the allele in, and pick a frequency
                // our frequency is either a value found in the table, or a default. we usually have defaults in the database with an allele value of 0
                // if the 0 allele is not found, then we use a default frequency of 0.02, which is acceptable but unexpected.
                foreach (string val in evidenceAlleles)
                {
                    // put a new row in with our allele
                    DataRow newDr = dt.NewRow();
                    newDr["AlleleNo"] = val;

                    float defaultFrequency = 0.02f;

                    // look through the cache for the allele, and set our default frequency in case we don't find it
                    bool found = false;
                    foreach (DataRow dr in drs)
                        if (dr["AlleleNo"].ToString() == val)
                        {
                            newDr["freq"] = dr["frequency"];
                            found = true;
                        }
                        else if (dr["AlleleNo"].ToString() == "0")
                            defaultFrequency = Convert.ToSingle(dr["frequency"]);

                    // if this isn't the omega allele (unknown or w)
                    if(val != "w")
                    {
                        // if we haven't found the value in the cache, use the default
                        if (!found)
                            newDr["freq"] = defaultFrequency;

                        dt.Rows.Add(newDr);
                    }
                }

                tblFreq = dt;
            }

            // calculate our sum
            if (tblFreq != null)
            {
                sum = tblFreq.AsEnumerable().Sum(p => p.Field<float>("freq"));
            }
            return sum;
        }

        /// <summary>
        /// This function gets the frequency values of the alleles based on the race and locus.
        /// </summary>
        /// <param name="evidenceAlleles">List of alleles at this locus in the evidence across the replicates.</param>
        /// <param name="locus">The name of the locus on which we are currently working.</param>
        /// <param name="race">The name of the race on which we are currently working.</param>
        /// <param name="dtFrequencies">A datatable with the frequency cache.</param>
        /// <returns>A data table containing the frequency values at this locus for this race including the w (omega, unknown) allele.</returns>
        protected DataTable GetFrenquencyTable(IEnumerable<string> evidenceAlleles, string locus, string race, DataTable dtFrequencies)
        {
            DataTable tblFreq = null;
            myDb = myDb ?? new Database();

            // if we have no cache, retrieve the frequency values from the database
            if (dtFrequencies == null) 
                tblFreq = myDb.getAllelesFrequence(locus, race, evidenceAlleles);
            else // if we have a cache, pick up the values from the cache
            {
                // get the frequencies for this locus and this race from the cache
                DataRow[] drs = dtFrequencies.Select("race='" + race + "' AND locus='" + locus + "'");

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("AlleleNo", typeof(string)));
                dt.Columns.Add(new DataColumn("freq", typeof(float)));

                // for each one of the evidence alleles, we create a new row, put the allele in, and pick a frequency
                // our frequency is either a value found in the table, or a default. we usually have defaults in the database with an allele value of 0
                // if the 0 allele is not found, then we use a default frequency of 0.02, which is acceptable but unexpected.
                foreach (string val in evidenceAlleles)
                {
                    // put a new row in with our allele
                    DataRow newDr = dt.NewRow();
                    newDr["AlleleNo"] = val;

                    float defaultFrequency = 0.02f;

                    // look through the cache for the allele, and set our default frequency in case we don't find it
                    bool found = false;
                    foreach (DataRow dr in drs)
                        if (dr["AlleleNo"].ToString() == val)
                        {
                            newDr["freq"] = dr["frequency"];
                            found = true;
                        }
                        else if (dr["AlleleNo"].ToString() == "0")
                            defaultFrequency = Convert.ToSingle(dr["frequency"]);

                    // if this isn't the omega allele (unknown or w)
                    if (val != "w")
                    {
                        // if we haven't found the value in the cache, use the default
                        if (!found)
                            newDr["freq"] = defaultFrequency;

                        dt.Rows.Add(newDr);
                    }
                }

                tblFreq = dt;
            }

            if (tblFreq != null)
            {
                // calculate the frequency sum and subtract from 1 for the omega (w, unknown) value
                float w = tblFreq.AsEnumerable().Sum(p => p.Field<float>("freq"));
                w = 1 - w;
                // fail if we calculate a bad omega
                if (w < 0 || w >= 1)
                    return null;//modified by Dhruba
                else
                {
                    // add a new row to the table with our omega frequency
                    DataRow dr = tblFreq.NewRow();
                    dr["AlleleNo"] = "w";
                    dr["freq"] = w;
                    tblFreq.Rows.Add(dr);
                }
            }
            return tblFreq;
        }

        /// <summary>
        /// This method picks a dropin rate to use based on whether we had no dropin, one allele dropin, or more than one.
        /// </summary>
        /// <param name="dropIns">Integer representation of the number of alleles that dropped in.</param>
        /// <returns>The correct rate based on how many alleles dropped in.</returns>
        protected float GetGenotypeDropInRate(int dropIns)
        {
            float Pxy = 1;
            switch (dropIns)
            {
                case 0:
                    Pxy = Pc0;
                    break;
                case 1:
                    Pxy = Pc1;
                    break;
                case 2:
                default:
                    Pxy = Pc2;
                    break;
            }
            return Pxy;
        }

        /// <summary>
        /// This function calculates the genotype frequency based on the allelic frequencies. This calculation differs between homozygous and heterozygous genotypes.
        /// For the heterozygous case, for a genotype (Asy, Bsy) with allelic frequencies (p,q) the genotype frequency is: F = 2 * p * q
        /// For the homozygous case, for a genotype (Asy, Asy) with allelc freuqencies (p,p), and theta T, the genotype frequency is: F = p + T * p * (1 - p) or F = P + T * (p^2 - p)
        /// </summary>
        /// <param name="Asy"></param>
        /// <param name="Bsy"></param>
        /// <param name="tblFreq"></param>
        /// <param name="Theta"></param>
        /// <returns></returns>
        protected float Frenquency(string Asy, string Bsy, DataTable tblFreq, float Theta)
        {
            float freq = 0;
            try
            {
                // check if our genotype is homozygous
                if (Asy == Bsy)
                {
                    // if we don't have a frequencies table use a default (this should never happen, but here it is)
                    if (tblFreq == null || tblFreq.Rows.Count == 0)
                    {
                        freq = (float)0.02;
                    }
                    else
                    {
                        // get our genotype frequency
                        string key = "AlleleNo='" + Asy.ToString() + "'";
                        DataRow[] dr = tblFreq.Select(key);
                        // if we found a frequency, return the frequency
                        if (dr.Length > 0)
                        {
                            // if we have exactly one frequency value, then use it
                            if (dr.Length == 1)
                                freq = (float)dr[0]["freq"];
                            else
                                throw new Exception("duplicate Allele.");
                        }
                        else // if we have no frequency values, use a default (this should never happen, but here it is)
                            freq = (float)0.02;
                    }

                    // calculate the homozygous genotype frequency: F = p * (p + (T * (1 - p)))
                    freq = Convert.ToSingle(freq * (freq + (Theta * (1 - freq))));//added by Dhruba
                }
                else
                {
                    // our genotype is heterozygous
                    float a = 0;
                    float b = 0;
                    // if we don't have frequencies, use the default (this should never happen, but here it is)
                    if (tblFreq == null || tblFreq.Rows.Count == 0)
                    {
                        a = (float)0.02;
                        b = (float)0.02;
                    }
                    else
                    {
                        // get our Asy allele frequency
                        string key = "AlleleNo='" + Asy.ToString() + "'";
                        DataRow[] dr = tblFreq.Select(key);
                        if (dr.Length > 0)
                        {
                            // if we have exactly one frequency value, then use it
                            if (dr.Length == 1)
                                a = (float)dr[0]["freq"];
                            else
                                throw new Exception("duplicate Allele.");
                        }
                        else // if we have no frequency values, use a default (this should never happen, but here it is)
                            a = (float)0.02;

                        // get our Bsy allele frequency
                        key = "AlleleNo='" + Bsy.ToString() + "'";
                        dr = tblFreq.Select(key);
                        if (dr.Length > 0)
                        {
                            // if we have exactly one frequency value, then use it
                            if (dr.Length == 1)
                                b = (float)dr[0]["freq"];
                            else
                                throw new Exception("duplicate Allele.");
                        }
                        else // if we have no frequency values, use a default (this should never happen, but here it is)
                            b = (float)0.02;
                    }

                    // calculate the heterozygous genotype frequency: F = 2 * p * q
                    freq = 2 * a * b;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Frequency Error." + e.Message);
            }
            return freq;
        }

        /// <summary>
        /// This method gets the dropout rates from the selected dropout table for this locus. We then calculate the zero-dropout rates based on the complement
        /// of the sum of the other rates. We have only one rate for homozygotes (genotypes where the two alleles are the same) because there is only one allele
        /// that can drop out. For heterozygotes, we have two. 
        /// </summary>
        /// <param name="locusName"></param>
        /// <param name="dropOutRateTable"></param>
        /// <returns></returns>
        private Dictionary<string, float> DropOutRateForLocus(string locusName, DataTable dropOutRateTable)
        {
            Dictionary<string, float> curDrouputRate = new Dictionary<string, float>();

            if (dropOutRateTable.Rows.Count != 0)
            {
                string filter = "LocusName='" + locusName + "'";
                DataRow[] thisLocusDropoutRate = dropOutRateTable.Select(filter);
                if (thisLocusDropoutRate.Length > 0)
                {
                    for (int i = 0; i < thisLocusDropoutRate.Length; i++)
                        curDrouputRate.Add(thisLocusDropoutRate[i]["Type"].ToString().Trim(), (float)thisLocusDropoutRate[i]["DropoutRate"]);

                    // calculate the zero dropout rates based on the complement of the sum of the non-zero dropout rates
                    curDrouputRate.Add("PHOM0", 1 - curDrouputRate["PHOM1"]);
                    curDrouputRate.Add("PHET0", 1 - curDrouputRate["PHET1"] - curDrouputRate["PHET2"]);
                }
            }
            return curDrouputRate;

        }

        /// <summary>
        /// This method retrieves dropout rates from the database, and computes a rate based on the DNA evidence amount. If the DNA evidence amount happens to match one of the
        /// data points for which we have a measured dropout rate, then we use that rate exactly. If the rate is not matched, then we compute a rate based on a linear regression
        /// between the two nearest rates. There is also a special case where the DNA evidence amount may be a value between 100 and 101 picograms of DNA evidence. In this case,
        /// we use the 101 picogram dropout rate explicitly.
        /// </summary>
        /// <param name="dnaEvidenceAmount">DNA evidence amount entered in the UI in picograms.</param>
        /// <param name="NoOfPersonsInvolvd">The number of persons assumed to be involved based on this comparison scenario.</param>
        /// <param name="strDeducible">A string containing 'Yes' or 'No' based on whether the number of contributors to this evidence is deducible. Different dropout rates are selected.</param>
        /// <param name="labKitID">A GUID representing which Lab Kit is being used for this comparison. Different Lab Kits contain different dropout rates.</param>
        /// <returns>Data table with the dropout rates.</returns>
        protected DataTable ReadDropoutRate(string dnaEvidenceAmount, int NoOfPersonsInvolvd, string strDeducible, string labKitID)
        {
            decimal tempDropout = 0.0m;
            string strDropOutOptionBegin = "";
            string strDropOutOptionEnd = "";
            // sometimes the users may copy a DNA evidence amount that includes the units. remove it, trim
            dnaEvidenceAmount = dnaEvidenceAmount.Replace("pg", "").Trim();
            tempDropout = Convert.ToDecimal(dnaEvidenceAmount);
            // cap the amount at 500 picograms
            if (tempDropout > 500m)
                tempDropout = 500m;
            // if the amount is between 100 and 101 picograms, use the 101 picogram rate.
            if (100m < tempDropout && tempDropout < 101m)
                tempDropout = 101m;

            // format the string to match the values in the DropoutOptions table in the database
            dnaEvidenceAmount = tempDropout + " pg";

            // if our value matches any of the dropout rate points, then we use that exact rate. 
            if (tempDropout == 6.25m || tempDropout == 12.5m || tempDropout == 25m || tempDropout == 50m || tempDropout == 100m || tempDropout == 101m
                 || tempDropout == 150m || tempDropout == 250m || tempDropout == 500m)
            {
                strDropOutOptionBegin = dnaEvidenceAmount;
                strDropOutOptionEnd = dnaEvidenceAmount;
            }
            // otherwise, we pick a lower rate and an upper rate (strDropOutOptionBegin, strDropOutOptionEnd)
            else if (6.25m < tempDropout && tempDropout < 12.5m)
            {
                strDropOutOptionBegin = "6.25 pg";
                strDropOutOptionEnd = "12.5 pg";
                tempDropout = tempDropout - 6.25m;
            }
            else if (12.5m < tempDropout && tempDropout < 25m)
            {
                strDropOutOptionBegin = "12.5 pg";
                strDropOutOptionEnd = "25 pg";
                tempDropout = tempDropout - 12.5m;
            }
            else if (25m < tempDropout && tempDropout < 50m)
            {
                strDropOutOptionBegin = "25 pg";
                strDropOutOptionEnd = "50 pg";
                tempDropout = tempDropout - 25m;
            }
            else if (50m < tempDropout && tempDropout < 100m)
            {
                strDropOutOptionBegin = "50 pg";
                strDropOutOptionEnd = "100 pg";
                tempDropout = tempDropout - 50m;
            }
            else if (101m < tempDropout && tempDropout < 150m)
            {
                strDropOutOptionBegin = "101 pg";
                strDropOutOptionEnd = "150 pg";
                tempDropout = tempDropout - 101m;
            }
            else if (150m < tempDropout && tempDropout < 250m)
            {
                strDropOutOptionBegin = "150 pg";
                strDropOutOptionEnd = "250 pg";
                tempDropout = tempDropout - 150m;
            }
            else if (250m < tempDropout && tempDropout < 500m)
            {
                strDropOutOptionBegin = "250 pg";
                strDropOutOptionEnd = "500 pg";
                tempDropout = tempDropout - 250m;
            }

            // get the upper and lower dropout rate points from the database based on the number of people involved and whether the number of people is deducible from the evidence
            myDb = myDb ??  new Database();
            DataTable dt = myDb.getDropOutRate(strDropOutOptionBegin, strDropOutOptionEnd, NoOfPersonsInvolvd, strDeducible, labKitID);
            // if our DNA evidence amount is exactly one of the dropout rate points then return the rates
            if (strDropOutOptionBegin == strDropOutOptionEnd)
                return dt;
            else
            {   // otherwise, calculate the rates by doing a linear regression around the two points for every locus. we iterate every two rows, and calculate the value for one row
                for (int i = 0; i < dt.Rows.Count; i = i + 2)
                {
                    dt.Rows[i]["DropOutRate"] = Convert.ToDecimal(dt.Rows[i]["DropOutRate"].ToString()) +
                         ((Convert.ToDecimal(dt.Rows[i + 1]["DropOutRate"].ToString()) - Convert.ToDecimal(dt.Rows[i]["DropOutRate"].ToString())) /
                         (Convert.ToDecimal(dt.Rows[i + 1]["DropoutOption"].ToString()) - Convert.ToDecimal(dt.Rows[i]["DropoutOption"].ToString()))) * tempDropout;
                }

                // then we go delete every other row because that rate is no longer used. it was used only for the linear regression
                for (int i = dt.Rows.Count - 1; i >= 0; i = i - 2)
                    dt.Rows.RemoveAt(i);

                return dt;

            }
        }

        /// <summary>
        /// This method retrieves degraded dropout rates from the database, and computes a rate based on the DNA evidence amount. If the DNA evidence amount happens to match one of the
        /// data points for which we have a measured dropout rate, then we use that rate exactly. If the rate is not matched, then we compute a rate based on a linear regression
        /// between the two nearest rates. There is also a special case where the DNA evidence amount may be a value between 100 and 101 picograms of DNA evidence. In this case,
        /// we use the 101 picogram dropout rate explicitly.
        /// NOTE: Degradation is calculated in the database by the stored procedure.
        /// </summary>
        /// <param name="dnaEvidenceAmount">DNA evidence amount entered in the UI in picograms.</param>
        /// <param name="NoOfPersonsInvolvd">The number of persons assumed to be involved based on this comparison scenario.</param>
        /// <param name="strDeducible">A string containing 'Yes' or 'No' based on whether the number of contributors to this evidence is deducible. Different dropout rates are selected.</param>
        /// <param name="strDegradedType">The string that represents how degraded the sample is ('Mild', 'Severe')</param>
        /// <param name="labKitID">A GUID representing which Lab Kit is being used for this comparison. Different Lab Kits contain different dropout rates.</param>
        protected DataTable ReadDegradedDropoutRate(string dnaEvidenceAmount, int NoOfPersonsInvolvd, string strDeducible, string strDegradedType)
        {
            decimal tempDropout = 0.0m;
            string strDropOutOptionBegin = "";
            string strDropOutOptionEnd = "";
            // sometimes the users may copy a DNA evidence amount that includes the units. remove it, trim
            dnaEvidenceAmount = dnaEvidenceAmount.Replace("pg", "").Trim();
            tempDropout = Convert.ToDecimal(dnaEvidenceAmount);
            // cap the amount at 500 picograms
            if (tempDropout > 500m)
                tempDropout = 500m;
            // if the amount is between 100 and 101 picograms, use the 101 picogram rate.
            if (100m < tempDropout && tempDropout < 101m)
                tempDropout = 101m;

            // format the string to match the values in the DropoutOptions table in the database
            dnaEvidenceAmount = tempDropout + " pg";

            // if our value matches any of the dropout rate points, then we use that exact rate. 
            if (tempDropout == 6.25m || tempDropout == 12.5m || tempDropout == 25m || tempDropout == 50m || tempDropout == 100m || tempDropout == 101m
                 || tempDropout == 150m || tempDropout == 250m || tempDropout == 500m)
            {
                strDropOutOptionBegin = dnaEvidenceAmount;
                strDropOutOptionEnd = dnaEvidenceAmount;
            }
            // otherwise, we pick a lower rate and an upper rate (strDropOutOptionBegin, strDropOutOptionEnd)
            else if (6.25m < tempDropout && tempDropout < 12.5m)
            {
                strDropOutOptionBegin = "6.25 pg";
                strDropOutOptionEnd = "12.5 pg";
                tempDropout = tempDropout - 6.25m;
            }
            else if (12.5m < tempDropout && tempDropout < 25m)
            {
                strDropOutOptionBegin = "12.5 pg";
                strDropOutOptionEnd = "25 pg";
                tempDropout = tempDropout - 12.5m;
            }
            else if (25m < tempDropout && tempDropout < 50m)
            {
                strDropOutOptionBegin = "25 pg";
                strDropOutOptionEnd = "50 pg";
                tempDropout = tempDropout - 25m;
            }
            else if (50m < tempDropout && tempDropout < 100m)
            {
                strDropOutOptionBegin = "50 pg";
                strDropOutOptionEnd = "100 pg";
                tempDropout = tempDropout - 50m;
            }
            else if (101m < tempDropout && tempDropout < 150m)
            {
                strDropOutOptionBegin = "101 pg";
                strDropOutOptionEnd = "150 pg";
                tempDropout = tempDropout - 101m;
            }
            else if (150m < tempDropout && tempDropout < 250m)
            {
                strDropOutOptionBegin = "150 pg";
                strDropOutOptionEnd = "250 pg";
                tempDropout = tempDropout - 150m;
            }
            else if (250m < tempDropout && tempDropout < 500m)
            {
                strDropOutOptionBegin = "250 pg";
                strDropOutOptionEnd = "500 pg";
                tempDropout = tempDropout - 250m;
            }

            // get the upper and lower dropout rate points from the database based on the number of people involved and whether the number of people is deducible from the evidence
            // NOTE: degradation is calculated in the database
            myDb = myDb ?? new Database();
            DataTable dt = myDb.getDegradedDropOutRate(strDropOutOptionBegin, strDropOutOptionEnd, NoOfPersonsInvolvd, strDeducible, strDegradedType);
            // if our DNA evidence amount is exactly one of the dropout rate points then return the rates
            if (strDropOutOptionBegin == strDropOutOptionEnd)
                return dt;
            else
            {   // otherwise, calculate the rates by doing a linear regression around the two points for every locus. we iterate every two rows, and calculate the value for one row
                for (int i = 0; i < dt.Rows.Count; i = i + 2)
                {
                    Decimal dropOutRate = Convert.ToDecimal(dt.Rows[i]["DropOutRate"].ToString(), CultureInfo.CurrentCulture) +
                         ((Convert.ToDecimal(dt.Rows[i + 1]["DropOutRate"].ToString(), CultureInfo.CurrentCulture) - Convert.ToDecimal(dt.Rows[i]["DropOutRate"].ToString(), CultureInfo.CurrentCulture)) /
                         (Convert.ToDecimal(dt.Rows[i + 1]["DropoutOption"].ToString(), CultureInfo.CurrentCulture) - Convert.ToDecimal(dt.Rows[i]["DropoutOption"].ToString(), CultureInfo.CurrentCulture))) * tempDropout;

                    // cap the rate if the value is greater than 0.5
                    if (dropOutRate > 0.5m)
                        dropOutRate = 0.5m;

                    dt.Rows[i]["DropOutRate"] = dropOutRate;
                }
                // then we go delete every other row because that rate is no longer used. it was used only for the linear regression
                for (int i = dt.Rows.Count - 1; i >= 0; i = i - 2)
                    dt.Rows.RemoveAt(i);

                return dt;
            }
        }

        /// <summary>
        /// Reads the dropin rates from the database based on the DNA evidence amount and the Lab Kit we are using for this comparison.
        /// </summary>
        /// <param name="dnaEvidenceAmount">DNA evidence amount entered in the UI in picograms.</param>
        /// <param name="labKitID">A GUID representing which Lab Kit is being used for this comparison. Different Lab Kits contain different dropout rates.</param>
        protected void ReadDropinRate(string dnaEvidenceAmount, string labKitID)
        {
            decimal tempDropout = 0.0m;
            string strType = "Less Than Equal To 100 pg";
            // sometimes the users may copy a DNA evidence amount that includes the units. remove it, trim
            dnaEvidenceAmount = dnaEvidenceAmount.Replace("pg", "").Trim();
            tempDropout = Convert.ToDecimal(dnaEvidenceAmount);
            if (tempDropout > 100m)
                strType = "Greater Than 100 pg";

            // get the dropin rates from the database
            myDb = myDb ?? new Database();
            DataTable dt = myDb.getDropInRate(labKitID);

            // go through all the rates and choose which rates we are using based on whether our DNA evidence amount is greater than 100
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                float dropInRate = Convert.ToSingle(dt.Rows[i]["DropInRate"]);

                if (dt.Rows[i]["DropinRateID"].ToString() == "pC0" && dt.Rows[i]["Type"].ToString() == strType)
                    Pc0 = dropInRate;
                else if (dt.Rows[i]["DropinRateID"].ToString() == "pC1" && dt.Rows[i]["Type"].ToString() == strType)
                    Pc1 = dropInRate;
                else if (dt.Rows[i]["DropinRateID"].ToString() == "pC2" && dt.Rows[i]["Type"].ToString() == strType)
                    Pc2 = dropInRate;
            }
        }

    }
}
