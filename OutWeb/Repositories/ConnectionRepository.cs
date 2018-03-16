using OutWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace OutWeb.Repositories
{
    public class ConnectionRepository
    {
        private ConnectionConfig m_connectionInformation;

        internal ConnectionRepository()
        {
            m_connectionInformation = new ConnectionConfig();
        }

        private ConnectionConfig ConectionConfig
        { get { return this.m_connectionInformation; } }

        /// <summary>
        /// 設定資料庫連線字串
        /// </summary>
        /// <returns></returns>
        public string GetEntityConnctionString()
        {
            // Specify the provider name, server and database.
            string providerName = "System.Data.SqlClient";
            string serverName = string.Empty;
            string databaseName = string.Empty;
            string userID = string.Empty;
            string password = string.Empty;
            string connectionString = string.Empty;

            serverName = this.ConectionConfig.ServerName;
            databaseName = this.ConectionConfig.DatabaseName;
            userID = this.ConectionConfig.DatabaseUserID;
            password = this.ConectionConfig.DatabasePassword;

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder =
                new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            //sqlBuilder.IntegratedSecurity = true;
            sqlBuilder.UserID = userID;
            sqlBuilder.Password = password;
            //sqlBuilder.IntegratedSecurity = false;
            //sqlBuilder.MultipleActiveResultSets = true;
            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
                new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = @"
            res://*/Entities.TCGDB.csdl|
            res://*/Entities.TCGDB.ssdl|
            res://*/Entities.TCGDB.msl";

            connectionString = entityBuilder.ToString();

            using (EntityConnection conn =
                new EntityConnection(connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("無法連線至資料庫.", ex);
                }
                conn.Close();
            }
            return connectionString;
        }
    }
}