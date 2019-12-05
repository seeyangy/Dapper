using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mio.Core.Dal.Options
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurtaionServices
    {
        #region Initialize

        /// <summary>
        /// 加锁防止并发操作
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// 配置实例
        /// </summary>
        private static AppConfigurtaionServices _instance = null;

        /// <summary>
        /// 配置根节点
        /// </summary>
        private IConfigurationRoot Config { get; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private AppConfigurtaionServices()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Config = builder.Build();
        }

        /// <summary>
        /// 获取配置实例
        /// </summary>
        /// <returns></returns>
        private static AppConfigurtaionServices GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new AppConfigurtaionServices();
                    }
                }
            }

            return _instance;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="name">配置节点名称</param>
        /// <returns></returns>
        public static string GetConfig(string name)
        {
            return GetInstance().Config.GetSection(name).Value;
        }
        #endregion
        
    }
}
