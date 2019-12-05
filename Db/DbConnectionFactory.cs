using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Mio.Core.Dal.Db
{
    /// <summary>
    /// 数据库连接工厂类
    /// </summary>
    public class ConnectionFactory
    {
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="strConn">数据库连接字符串</param>
        /// <returns>数据库连接</returns>
        public static IDbConnection CreateConnection(string dbtype, string strConn)
        {
            if (string.IsNullOrWhiteSpace(dbtype))
                throw new ArgumentNullException("The database type is undefined");
            if (string.IsNullOrWhiteSpace(strConn))
                throw new ArgumentNullException("The Database connection undefined");
            var dbType = GetDataBaseType(dbtype);
            return CreateConnection(dbType, strConn);
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="strConn">数据库连接字符串</param>
        /// <returns>数据库连接</returns>
        public static IDbConnection CreateConnection(DatabaseType dbType, string strConn)
        {
            IDbConnection connection = null;
            if (string.IsNullOrWhiteSpace(strConn))
                throw new ArgumentNullException("The Database connection undefined");

            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    connection = new SqlConnection(strConn);
                    break;
                case DatabaseType.MySQL:
                    connection = new MySqlConnection(strConn);
                    break;
                default:
                    throw new ArgumentNullException($"The database type ({dbType.ToString()}) not support");

            }
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            return connection;
        }

        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbtype">数据库类型字符串</param>
        /// <returns>数据库类型</returns>
        public static DatabaseType GetDataBaseType(string dbtype)
        {
            if (string.IsNullOrWhiteSpace(dbtype))
                throw new ArgumentNullException("The database type is undefined");
            DatabaseType returnValue = DatabaseType.SqlServer;
            foreach (DatabaseType databasetype in Enum.GetValues(typeof(DatabaseType)))
            {
                if (databasetype.ToString().Equals(dbtype, StringComparison.OrdinalIgnoreCase))
                {
                    returnValue = databasetype;
                    break;
                }
            }
            return returnValue;
        }


    }
}
