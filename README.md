# Dapper
.NetCore 集成Dapper

一、Class样例

1.创建访问类,继承Dapper基类DalBase
/// <summary>
/// Local 数据仓库操作
/// </summary>
public class LopLocalDal : DalBase
{
    /// <summary>
    /// 
    /// </summary>
    protected override string DbServerId
    {
        get
        {
            return "LOP";
        }
    }
    
    /// <summary>
    /// 初始化数据库连接
    /// </summary>
    public LopLocalDal()
    {
        dbOption = new DbOption
        {
            DbType = AppConfigurtaionServices.GetConfig(string.Format("DbOption:{0}:DbType", DbServerId)),
            ConnectionString = AppConfigurtaionServices.GetConfig(string.Format("DbOption:{0}:ConnectionString", DbServerId))
        };
        if (dbOption == null)
        {
            throw new ArgumentNullException(nameof(DbOption));
        }
        //
        dbExecutor = ConnectionFactory.CreateConnection(dbOption.DbType, dbOption.ConnectionString);
    }

}
2.数据表Dal层继承LopLocalDal类

二、appsettings.json 配置
"DbOption": {
	"LOP": {
	  "ConnectionString": "Data Source=localhost;Initial Catalog=LOP;User ID=sa;Password=sa;Persist Security Info=True;Max Pool Size=100;Min Pool Size=0;Connection Lifetime=300;",
	  "DbType": "SqlServer"
	},
	"HY": {
	  "ConnectionString": "Data Source=localhost;Initial Catalog=HY;User ID=sa;Password=sa;Persist Security Info=True;Max Pool Size=100;Min Pool Size=0;Connection Lifetime=300;",
	  "DbType": "SqlServer"
	}
}
