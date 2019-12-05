using Mio.Core.Dal.Db;
using Mio.Core.Dal.Options;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace Mio.Core.Dal
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DalBase
    {
        /// <summary>
        /// 表名称
        /// </summary>
        protected string _TableName;
        /// <summary>
        /// 服务器逻辑名
        /// </summary>
        protected string dbServerId = "";
        /// <summary>
        /// 
        /// </summary>
        protected DbOption dbOption;
        /// <summary>
        /// 数据库对象
        /// </summary>
        protected IDbConnection dbExecutor = null;
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        protected IDbConnection DBExecutor
        {
            get
            {                
                if (this.dbExecutor == null)
                {
                    throw new Exception("Failed to connect to the database, please check that the database connection configuration is correct");
                }
                else
                {

                }
                return this.dbExecutor;
            }
        }
        /// <summary>
        /// 表名
        /// </summary>
        public virtual string TableName
        {
            get
            {
                return this._TableName;
            }
        }
        /// <summary>
        /// 数据库逻辑名称
        /// </summary>
        protected abstract string DbServerId { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        protected DalBase()
        {
        }
        /// <summary>
        /// 获取连接数据库字符串
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private string GetKernelConnectionStringWithDbName(string dbName)
        {
            string result = "";
            var kOption = new DbOption
            {
                DbType = AppConfigurtaionServices.GetConfig(string.Format("DbOption:{0}:DbType", "Kernel")),
                ConnectionString = AppConfigurtaionServices.GetConfig(string.Format("DbOption:{0}:ConnectionString", "Kernel"))
            };
            if (!string.IsNullOrEmpty(kOption.ConnectionString))
            {
                var dbKernel = ConnectionFactory.CreateConnection(kOption.DbType, kOption.ConnectionString);
                DynamicParameters dyParameter = new DynamicParameters();
                dyParameter.Add("Name", dbName, DbType.String);
                var dbInfo = dbKernel.QueryFirst<Model.KernelDbInfoModel>("select * from v_DbInfo where Name=@Name", param: dyParameter);
                if (dbInfo == null)
                {
                    throw new Exception(string.Format("The database ({0}) connection configuration is not defined.", dbName));
                }
                result = string.Format(result, new object[] { dbInfo.ServerName, dbInfo.RealName, dbInfo.DbUserName, dbInfo.PassWord });
            }
            return result;
        }
        /// <summary>
        /// 校验数据表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public bool CheckTableCreated(string tableName)
        {
            DynamicParameters dyParameter = new DynamicParameters();
            dyParameter.Add("name", tableName, DbType.String);
            dyParameter.Add("type", "U", DbType.String);
            var dyObject = DBExecutor.QueryFirst<int>("select count(1) from sysobjects where name=@name and type=@type", param: dyParameter);
            return dyObject > 0;
        }
        /// <summary>
        /// 创建表索引
        /// </summary>
        /// <param name="indexSqlString"></param>
        /// <returns></returns>
        public bool CreateIndex(string indexSqlString)
        {
            return DBExecutor.Execute(indexSqlString) > 0;
        }
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public bool CreateTable(string sqlString)
        {
            return DBExecutor.Execute(sqlString) > 0;
        }
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="listCmd"></param>
        /// <returns></returns>
        public bool ExecuteMultipleCommand(List<CommandDefinition> listCmd)
        {
            bool result = false;
            int index = -1;
            try
            {
                foreach (var cmd in listCmd)
                {
                    index = index++;
                    DBExecutor.Execute(cmd.CommandText, cmd.Parameters);
                }
                result = true;
            }
            catch (Exception exp)
            {
                result = false;
                throw new Exception(string.Format("索引行{0}发生错误{1}", index, exp.Message));
            }
            return result;
        }
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="listCmd"></param>
        /// <returns></returns>
        public bool ExecuteTransaction(List<CommandDefinition> listCmd)
        {
            bool result = false;
            using (IDbConnection dbConnection = dbExecutor)
            {
                int index = -1;
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                try
                {
                    foreach (var cmd in listCmd)
                    {
                        index = index++;

                        dbConnection.Execute(cmd.CommandText, cmd.Parameters, transaction);
                    }
                    transaction.Commit();
                    result = true;
                }
                catch (Exception exp)
                {
                    result = false;
                    transaction.Rollback();
                    throw new Exception(string.Format("索引行{0}发生错误{1}", index, exp.Message));
                }
                finally
                {
                    transaction = null;
                    dbConnection.Close();
                }
            }
            return result;
        }

        #region IDisposable Support

        // 要检测冗余调用
        private bool disposedValue = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    dbExecutor?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
