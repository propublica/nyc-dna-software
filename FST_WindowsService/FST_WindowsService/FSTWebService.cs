using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace FST_WindowsService
{
    [ServiceContract()]
    interface IFSTWebService
    {
        [OperationContract()]
        void RunJob(string recordID);
    }

    /// <summary>
    /// Currently, this is only used to run jobs. Ideally, this would have endpoints for checking job status, progress, job count, etc.
    /// </summary>
    class FSTWebService : IFSTWebService
    {
        /// <summary>
        /// Tells the service to run a job. Pass in a GUID referencing the RecordID column of the Cases table.
        /// </summary>
        /// <param name="recordID">GUID referencing the RecordID column of the Cases table.</param>
        public void RunJob(string recordID)
        {
            FSTService.Instance.RunJob(recordID);
        }
    }
}
