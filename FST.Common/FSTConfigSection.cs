using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Configuration;

namespace FST.Common
{
    public class FSTConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("defaultConnectionStringName", DefaultValue = "LocalSqlServer")]
        public string DefaultConnectionStringName
        {
            get { return (string)base["defaultConnectionStringName"]; }
            set { base["defaultConnectionStringName"] = value; }
        }

        [ConfigurationProperty("defaultCacheDuration", DefaultValue = "600")]
        public int DefaultCacheDuration
        {
            get { return (int)base["defaultCacheDuration"]; }
            set { base["defaultCacheDuration"] = value; }
        }

        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString 
        {
            get
            {
                // Return the base class' ConnectionString property.
                // The name of the connection string to use is retrieved from the site's 
                // custom config section and is used to read the setting from the <connectionStrings> section
                // If no connection string name is defined for the <articles> element, the
                // parent section's DefaultConnectionString prop is used.
                string connStringName = (String.IsNullOrEmpty(this.ConnectionStringName) ?
                   "LocalSqlServer" : this.ConnectionStringName);
                return WebConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }
    }
}
