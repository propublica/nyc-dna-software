using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Configuration;


namespace FST_WindowsService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            serviceInstaller1.ServiceName = GetConfigurationValue("FST_SERVICE_NAME");
        }

        /// <summary>
        /// Gets the service name from the configuration file (the app running this class is usually installutil.exe so we need to find our REAL app.config for values)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetConfigurationValue(string key)
        {
            Assembly service = Assembly.GetAssembly(typeof(ProjectInstaller));
            Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] != null)
            {
                return config.AppSettings.Settings[key].Value;
            }
            else
            {
                throw new IndexOutOfRangeException
                    ("Settings collection does not contain the requested key: " + key);
            }
        }
    }
}
