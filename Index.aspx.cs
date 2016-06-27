using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VANCHEER.Costom;
using VANCHEER.General;
using System.Data.SqlClient;
using System.Reflection;

namespace VANCHEER.Web
{
    public partial class Index : System.Web.UI.Page
    {
        BasicInfoModel BasicInfoMethod = new BasicInfoModel();
        protected BasicInfoModel.BasicInfoValue GetBasicInfo = new BasicInfoModel.BasicInfoValue();

        protected void Page_Load(object sender, EventArgs e)
        {
            BasicInfoMethod.Load();
            GetIndexData();
        }

        protected void GetIndexData()
        {
            StringBuilder sqlGetData = new StringBuilder().AppendFormat(
                @"With Recommend SELECT Top 1 ID,Title FROM V_C_RecommendContent WHERE IsDel=0 and FLowstate=99 Order by Orders");
            var reuslt = sqlReflectClass<IndexConfig>(SQLHelper.ConnectionStringLocalTransaction, sqlGetData.ToString());
        }

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

        /// 获取SQL反射泛型集合
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlStr">要查询的T-SQL</param>
        /// <returns></returns>
        protected T sqlReflectClassSingle<T>(string connStr, string sqlStr) where T : class,new()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlStr, sqlConnection))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    T CostomClass = new T();
                    PropertyInfo[] ClassProperty = CostomClass.GetType().GetProperties();
                    if (!dataReader.Read()) { return CostomClass; }
                    foreach (var Property in ClassProperty)
                    {
                        Property.SetValue(CostomClass, dataReader[Property.Name], null);
                    }
                    sqlConnection.Close();
                    return CostomClass;
                }
            }
        }

        protected partial class IndexConfig 
        {
            public string ID { get; set; }
            public string Title { get; set; }
        }
    }

}