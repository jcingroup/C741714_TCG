using System.Configuration;
using System.Data.SqlClient;
using OutWeb.Service;

namespace OutWeb.Models
{
    public class ConnectionConfig
    {
        private string m_serverName = string.Empty;
        private string m_databaseName = string.Empty;
        private string m_databaseUserID = string.Empty;
        private string m_databasePassword = string.Empty;

        Service.Service CService = new Service.Service();

        public ConnectionConfig()
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["conn_string"].ConnectionString;
            string connectionString = CService.conn_string();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);

            string server = builder.DataSource;
            string database = builder.InitialCatalog;

            this.m_serverName = builder.DataSource;
            this.m_databaseName = builder.InitialCatalog;
            this.m_databaseUserID = builder.UserID;
            this.m_databasePassword = builder.Password;
        }

        /// <summary>
        /// Database路徑
        /// </summary>
        public string ServerName { get { return this.m_serverName; } set { this.m_serverName = value; } }

        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string DatabaseName { get { return this.m_databaseName; } set { this.m_databaseName = value; } }

        /// <summary>
        /// 資料庫登入帳號
        /// </summary>
        public string DatabaseUserID { get { return this.m_databaseUserID; } set { this.m_databaseUserID = value; } }

        /// <summary>
        /// 資料庫登入密碼
        /// </summary>
        public string DatabasePassword { get { return this.m_databasePassword; } set { this.m_databasePassword = value; } }
    }
}