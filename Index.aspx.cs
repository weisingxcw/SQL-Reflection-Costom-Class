using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Reflection;

namespace VANCHEER.Web
{
    public partial class Index : System.Web.UI.Page
    {
        /// 获取SQL反射泛型集合
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlStr">要查询的T-SQL</param>
        /// <returns></returns>
        protected IList<T> sqlReflectClass<T>(string connStr, string sqlStr) where T : class,new()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlStr, sqlConnection))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    IList<T> list = new List<T>();

                    PropertyInfo[] classPropertyInfo = new IndexConfig().GetType().GetProperties();
                    
                    while (dataReader.Read())
                    {
                        T delegateType = new T();
                        foreach (var column in classPropertyInfo)
                        {
                            column.SetValue(delegateType, dataReader[column.Name], null);
                        }
                        list.Add(delegateType);
                    }
                    sqlConnection.Close();
                    return list;
                }
            }
        }
    }

}
