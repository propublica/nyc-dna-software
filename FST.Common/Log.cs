using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace FST.Common
{
    public static class Log
    {
        private static FST.Common.Database db = new FST.Common.Database();

        private static string sevCrash = "Crash";
        private static string sevError = "Error";
        private static string sevWarning = "Warning";
        private static string sevInfo = "Info";

        private static string typeUnhandled = "Unhandled";

        /// <summary>
        /// Used to log crashes
        /// </summary>
        /// <param name="username">Name of the user for whom the application crashed</param>
        /// <param name="pageName">The page name on which it crashed (or SVC if it's in the service)</param>
        /// <param name="session">Session information if this comes from the web application</param>
        /// <param name="extraInformation">String containing extra information to log</param>
        /// <param name="ex">The exception that caused the crash</param>
        public static void Crash(string username, string pageName, HttpSessionState session, string extraInformation, Exception ex)
        {
            LogToDB(
                    username,
                    pageName,
                    sevCrash,
                    typeUnhandled,
                    SerializeSession(session),
                    extraInformation,
                    ex == null ? null : ex.GetType().Name,
                    ex == null ? null : ex.Message,
                    ex == null ? null : ex.StackTrace
                    );
        }

        /// <summary>
        /// Used to log errors
        /// </summary>
        /// <param name="username">Name of the user for whom the application errored</param>
        /// <param name="pageName">The page name on which it errored (or SVC if it's in the service)</param>
        /// <param name="session">Session information if this comes from the web application</param>
        /// <param name="type">The type of error we encountered</param>
        /// <param name="extraInformation">String containing extra information to log</param>
        /// <param name="ex">The exception that caused the error</param>
        public static void Error(string username, string pageName, HttpSessionState session, string type, string extraInformation, Exception ex)
        {
            LogToDB(
                    username,
                    pageName,
                    sevError,
                    type,
                    SerializeSession(session),
                    extraInformation,
                    ex == null ? null : ex.GetType().Name,
                    ex == null ? null : ex.Message,
                    ex == null ? null : ex.StackTrace
                    );
        }

        /// <summary>
        /// Used to log warnings from the application
        /// </summary>
        /// <param name="username">Name of the user for whom the application generated a warning</param>
        /// <param name="pageName">The page name on which the application generated a warning</param>
        /// <param name="session">Session information if this comes from the web application</param>
        /// <param name="extraInformation">String containing extra information to log</param>
        public static void Warning(string username, string pageName, HttpSessionState session, string extraInformation)
        {
            LogToDB(
                    username,
                    pageName,
                    sevWarning,
                    typeUnhandled,
                    SerializeSession(session),
                    extraInformation,
                    null,
                    null,
                    null
                    );
        }

        /// <summary>
        /// Used to log information about the web application
        /// </summary>
        /// <param name="username">Name of the user for whom we are logging an informative message</param>
        /// <param name="pageName">Page on which the information originated</param>
        /// <param name="session">Session information if this comes from the web applciation</param>
        /// <param name="type">The type of information we are logging</param>
        /// <param name="extraInformation">String containing extra inforamtion to log</param>
        public static void Info(string username, string pageName, HttpSessionState session, string type, string extraInformation)
        {
            LogToDB(
                    username,
                    pageName,
                    sevInfo,
                    type,
                    SerializeSession(session),
                    extraInformation,
                    null,
                    null,
                    null
                    );
        }

        /// <summary>
        /// Used to log information about the windows service
        /// </summary>
        /// <param name="username">Name of the user for whom we are logging an informative message</param>
        /// <param name="logData">A string with the data which we are logging</param>
        /// <param name="type">The type of information we are logging</param>
        /// <param name="extraInformation">String containing extra information to log</param>
        public static void Info(string username, string logData, string type, string extraInformation)
        {
            LogToDB(
                    username,
                    "SVC",
                    sevInfo,
                    type,
                    logData,
                    extraInformation,
                    null,
                    null,
                    null
                    );
        }
        
        /// <summary>
        /// Helper method to iterate through the contents of a Session object and serialize the objects referenced withing
        /// </summary>
        /// <param name="session">Session object whose contents we attempt to serialize</param>
        /// <returns>A string with the key and ToString() value of the session objects joined by a colon and separated by commas</returns>
        private static string SerializeSession(HttpSessionState session)
        {
            string val = string.Empty;
            foreach (string key in session.Keys)
            {
                try
                {
                    if (session[key] != null) val += "\"" + key + "\":\"" + session[key].ToString() + "\",";
                }
                catch
                {
                    continue;
                }
            }
            return val;
        }

        /// <summary>
        /// Method that combines all the other methods in this class into one call to the method that saves this data to the database
        /// </summary>
        /// <param name="username">Name of the user for whom this data is being logged</param>
        /// <param name="pageName">Page or "SVC" representing where this activity happened</param>
        /// <param name="severity">The severity of the activity we are logging</param>
        /// <param name="type">The type of activity we are logging</param>
        /// <param name="session">String representing a serialized session or other data we are logging in palce of the session</param>
        /// <param name="extraInformation">Extra information to be logged</param>
        /// <param name="exceptionName">Name of the exception we are logging, if any</param>
        /// <param name="exceptionMessage">Message of the exception we are logging, if any</param>
        /// <param name="exceptionStackTrace">Stack trace of the exception we are logging, if any</param>
        private static void LogToDB(string username, string pageName, string severity, string type, string session, string extraInformation, string exceptionName, string exceptionMessage, string exceptionStackTrace)
        {
            db.InsertLog(username, pageName, severity, type, session, extraInformation, exceptionName, exceptionMessage, exceptionStackTrace);
        }
    }
}