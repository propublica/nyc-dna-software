using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Globalization;
using System.Configuration;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using FST.Common;
using System.Reflection;


namespace FST_WindowsService
{
    /// <summary>
    /// Main class for the Windows Service. This is where we instantiate our web "service" thread, and monitor our jobs in the "service thread"
    /// </summary>
    public partial class FSTService : ServiceBase
    {
        public FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

        Thread dequeuerThread;
        Thread serviceThread;
        Thread jobsReportThread;

        // queue for jobs that won't take too long. this is basically all of them
        Queue<DataRow> jobQueue = new Queue<DataRow>();
        // queue for jobs that do take a long time. this is going to be bulk searches for C+3U/3U, C+3U/4U, and (for some reason) C+2U/2U
        Queue<DataRow> longJobQueue = new Queue<DataRow>();
        // object used to synchronize access to the above queues
        Object jobQueueLock = new Object();

        // this is a list of jobs which are currently running, used to check if we can start new jobs
        List<ComparisonCase> runningCases = new List<ComparisonCase>();
        // this is a list of "long" jobs which are currently running, used to check if we can start new jobs
        List<ComparisonCase> longRunningCases = new List<ComparisonCase>();
        // object used to synchronize access to the above queues
        Object runningCasesLock = new Object();

        // this tells the threads to stop
        bool stopThreads = false;

        // reference to the instance of the service. since this is instantiated by the system and we only access it from itself and objects spawned within it should be threadsafe
        internal static FSTService Instance;
        public BulkReportPrinter BulkPrinter { get; set; }
        public IndividualReportPrinter IndividualPrinter { get; set; }

        public FSTService()
        {
            InitializeComponent();

            this.ServiceName = ConfigurationManager.AppSettings["FST_SERVICE_NAME"];

            FSTService.Instance = this;
            // this is how we log the messages from the DB class
            bi.m_dbinstance.OnCreateDailyLogEntry += new FST.Common.Database.CreateDailyLogEntryDelegate(FSTService.CreateDailyLogEntry);
        }

        protected override void OnStart(string[] args)
        {
            Initialize();
            StartProcess();
            CreateDailyLogEntry("Process Started");
        }

        /// <summary>
        /// This is called by Windows when the service is stopped.
        /// </summary>
        protected override void OnStop()
        {
            // stop our threads
            stopThreads = true;
            dequeuerThread.Abort();
            serviceThread.Abort();
        }

        public void Initialize()
        {
            // this is where we instantiate the report printers. we pass in the writeable output path, and the associated report template. 
            // maybe these should come from config, be shared with web?
            IndividualPrinter = new IndividualReportPrinter(Application.StartupPath + "\\FSTReport\\", Application.StartupPath + "\\Reports\\FSTResultReport.rdlc");
            BulkPrinter = new BulkReportPrinter(Application.StartupPath + "\\FSTReport\\", Application.StartupPath + "\\Reports\\FSTReportTemplate.xlsx");

            // instantiating the threads.
            dequeuerThread = new Thread(new ThreadStart(dequeueJobs));
            serviceThread = new Thread(new ThreadStart(serviceHost));
            jobsReportThread = new Thread(new ThreadStart(jobsReportThreadMethod));
        }

        public void StartProcess()
        {
            // check the database for jobs first
            databasePoll();

            // start our threads
            dequeuerThread.Start();
            serviceThread.Start();
            jobsReportThread.Start();
        }

        /// <summary>
        /// This method instantiates an instance of the web service
        /// </summary>
        private void serviceHost()
        {
            try
            {
                Uri httpUrl = new Uri(ConfigurationManager.AppSettings["FST_SERVICE_ADDRESS"]);
                ServiceHost host = new ServiceHost(typeof(FSTWebService), httpUrl);
                host.AddServiceEndpoint(typeof(IFSTWebService), new BasicHttpBinding(), "");
                //Enable metadata exchange
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);
                host.Open();
            }
            catch (Exception ex)
            {
                CreateDailyLogEntry("Error instantiating web service:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// This method is called by the web service when a job gets sent by one of the clients.
        /// </summary>
        /// <param name="recordID"></param>
        internal void RunJob(string recordID)
        {
            // We spawn a temporary thread so the service can return to the client quickly. If this isn't done, the service throws exceptions from here as well as its own.
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
                DataTable dtPendingCase = bi.GetPendingCase(recordID);
                foreach (DataRow dr in dtPendingCase.Rows)
                {
                    // check to see whether the case we have is a long-running job or a normal job and queue appropriately
                    switch (dr["CompareMethod"].ToString())
                    {
                        case "13":
                        case "14":
                        case "17":
                            lock (jobQueueLock)
                                if (dr["Compare_Type"].ToString() == "N")
                                    jobQueue.Enqueue(dr);
                                else
                                    longJobQueue.Enqueue(dr);
                            break;

                        default:
                            lock (jobQueueLock)
                                jobQueue.Enqueue(dr);
                            break;
                    }

                    // set the case status to processing
                    bi.UpdateCaseStatus(dr["RecordID"].ToString(), "P");
                }
                return;
            }));
        }

        /// <summary>
        /// Polls the database for new cases. Useful when the service is restarted and some jobs were submitted while the Windows Service was down
        /// </summary>
        private void databasePoll()
        {
            try
            {
                int rows = 0;

                do
                {
                    DataTable dtPendingCases = bi.GetPendingCases();
                    rows = dtPendingCases == null ? 0 : dtPendingCases.Rows.Count;
                    foreach (DataRow dr in dtPendingCases.Rows)
                    {
                        // check to see whether the case we have is a long-running job or a normal job and queue appropriately
                        switch (dr["CompareMethod"].ToString())
                        {
                            case "13":
                            case "14":
                            case "17":
                                lock (jobQueueLock)
                                    if (dr["Compare_Type"].ToString() == "N")
                                        jobQueue.Enqueue(dr);
                                    else
                                        longJobQueue.Enqueue(dr);
                                break;

                            default:
                                lock (jobQueueLock)
                                    jobQueue.Enqueue(dr);
                                break;
                        }
                        // set the case status to processing
                        bi.UpdateCaseStatus(dr["RecordID"].ToString(), "P");
                    }
                } while (rows > 0);
            }
            catch (Exception ex)
            {
                // log errors but don't break on them. we want to keep the service running even if we experience issues.
                CreateDailyLogEntry("Error in service: " + ex.ToString() + ex.StackTrace);
            }
        }

        private void dequeueJobs()
        {
            int maxJobs = Environment.ProcessorCount;
            int maxLongJobs = (Environment.ProcessorCount > 4) ? 2 : 1;         // give ourselves an extra long job thread if we have over four cores. ideally, this would be the default configuration
            if (maxJobs > 1) maxJobs -= maxLongJobs; // leave a processor for the big jobs, but don't block small jobs if we're on a single core box (please don't run this on a single core box)

            while (!stopThreads)
            {
                lock (runningCasesLock)
                {
                    DataRow dr = null;

                    // if we're not maxxed out on normal threads, try to dequeue and start a job
                    if (runningCases.Count < maxJobs)
                    {
                        lock (jobQueueLock)
                        {
                            if (jobQueue.Count > 0)
                                dr = jobQueue.Dequeue();
                        }

                        if (dr != null)
                        {
                            // this is where we start jobs (new thread is created in constructor)
                            ComparisonCase cc = new ComparisonCase(dr);
                            runningCases.Add(cc);
                        }
                    }

                    DataRow drLong = null;

                    // if we're not maxxed out on long job threads, try to dequeue and start a job
                    if (longRunningCases.Count < maxLongJobs)
                    {
                        lock (jobQueueLock)
                        {
                            if (longJobQueue.Count > 0)
                                drLong = longJobQueue.Dequeue();
                        }

                        if (drLong != null)
                        {
                            // this is where we start jobs (new thread is created in constructor)
                            ComparisonCase cc = new ComparisonCase(drLong);
                            longRunningCases.Add(cc);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// This is called by the normal jobs instance of the ComparisonCase class when it is done running its 
        /// comparison and it is ready to remove itself from the running list to allow new jobs to continue
        /// </summary>
        /// <param name="cc">ComparisonCase instance of the job that just finished</param>
        internal static void removeRunningCase(ComparisonCase cc)
        {
            lock (FSTService.Instance.runningCasesLock)
            {
                FSTService.Instance.runningCases.Remove(cc);
            }
        }

        /// <summary>
        /// This is called by the long jobs instance of the ComparisonCase class when it is done running its 
        /// comparison and it is ready to remove itself from the running list to allow new jobs to continue
        /// </summary>
        /// <param name="cc">ComparisonCase instance of the job that just finished</param>
        internal static void removeLongRunningCase(ComparisonCase cc)
        {
            lock (FSTService.Instance.runningCasesLock)
            {
                FSTService.Instance.longRunningCases.Remove(cc);
            }
        }

        private static Object logLock = new Object();
        /// <summary>
        /// This method gets called to write data to the local text file log. This log usually has more details about the process than the
        /// SQL log because SQL and/or network may fail.
        /// </summary>
        /// <param name="logEntry">String representing the data to be logged to the file.</param>
        internal static void CreateDailyLogEntry(string logEntry)
        {
            lock (logLock)
            {
                try
                {
                    string dailyLogFilename = String.Format(CultureInfo.CurrentCulture, "{0:yyyy-MM-dd}.log", DateTime.Now);
                    string entryTime = String.Format(CultureInfo.CurrentCulture, "{0:HH:mm:ss t} -- ", DateTime.Now);
                    string fullFilename = Path.Combine(Application.StartupPath + "\\LogFiles", dailyLogFilename);
                    using (StreamWriter sw = new StreamWriter(fullFilename, true))
                    {
                        sw.Write(entryTime + logEntry + Environment.NewLine);
                        sw.Flush();
                        sw.Close();
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Checks if it's time to e-mail the jobs report, and e-mails it when it's time.
        /// </summary>
        private void jobsReportThreadMethod()
        {
            string sendTime = ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_JOBS_REPORT_TIME"];
            DateTime sendDateTime = Convert.ToDateTime(DateTime.Now.Date.ToString("MM/dd/yyyy") + " " + sendTime);

            while (!stopThreads)
            {
                Thread.Sleep(10000);
                if (DateTime.Now >= sendDateTime)
                {
                    SendEmailJobsReport();
                    sendDateTime = Convert.ToDateTime(DateTime.Now.AddDays(1).Date.ToString("MM/dd/yyyy") + " " + sendTime);
                }
            }
        }

        /// <summary>
        /// E-mail the counts for different job statuses to a user specified in the config file.
        /// </summary>
        private void SendEmailJobsReport()
        {
            try
            {
                string strSubject = ConfigurationManager.AppSettings["FST_SERVICE_NAME"] + " - Jobs Report";

                StringBuilder sbBody = new StringBuilder();

                int waitingToProcess = bi.GetCaseCounterByStatus("N");
                int processing = bi.GetCaseCounterByStatus("P");
                int finished = bi.GetCaseCounterByStatus("Y");
                int deleted = bi.GetCaseCounterByStatus("D");
                int total = waitingToProcess + processing + finished + deleted;

                sbBody.Append("\r\nJobs Report:\r\n");
                sbBody.Append("\r\nJobs Waiting To Process: " + waitingToProcess);
                sbBody.Append("\r\nJobs Processing: " + processing);
                sbBody.Append("\r\nJobs Finished: " + finished);
                sbBody.Append("\r\nJobs Deleted: " + deleted);
                sbBody.Append("\r\nTotal: " + deleted);

                using (MailMessage msg = new MailMessage())
                {
                    msg.From = new MailAddress(ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_ADDRESS_FROM"], ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_ADDRESS_NAME"]);
                    string emailTo = ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_JOBS_REPORT_TO"];
                    string[] emailToString = emailTo.Split(';');//,1, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string s in emailToString)
                    {
                        msg.To.Add(new MailAddress(s, s));
                    }
                    //msg.To.Add(new MailAddress(ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_JOBS_REPORT_TO"], ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_JOBS_REPORT_TO"]),;

                    msg.Subject = strSubject;
                    msg.Body = sbBody.ToString();
                    //SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER"]);
                    SmtpClient client = new SmtpClient((ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER"]), Convert.ToInt32(ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER_PORT"]));
                    using (client)
                    {
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        string username = ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER_USERNAME"];
                        string password = ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER_PASSWORD"];
                        client.Credentials = new NetworkCredential(username, password);
                        //client.Credentials = new NetworkCredential("username","password");
                        client.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                FSTService.CreateDailyLogEntry(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
