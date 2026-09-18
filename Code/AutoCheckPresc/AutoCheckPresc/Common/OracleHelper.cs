
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Timer.Common
{
    public class OracleHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString = "";

        public static void SetOracleConnection(string connStr)
        {
            ConnectionString = connStr;
        }

        /// <summary>
        /// 查询 postgre 数据库，返回 DataTable 数据
        /// </summary>
        /// <param name="sqlText">sql查询语句</param>
        /// <param name="param">参数集合</param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string sqlText, params OracleParameter[] param)
        {
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();
                    using (OracleDataAdapter nda = new OracleDataAdapter(sqlText, conn))
                    {

                        if (param != null)
                        {
                            SetParmDBNull(param);
                            nda.SelectCommand.Parameters.AddRange(param);
                        }
                        var ds = new DataSet();
                        nda.Fill(ds);
                        dt = ds.Tables[0];
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString() + "\r\nExecuteQuery:" + sqlText);
            }
        }

        /// <summary>
        /// 查询唯一值（用来查总记录数等等..）
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public static int QueryOnly(string sqlText, params OracleParameter[] param)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    OracleCommand objCommand = new OracleCommand(sqlText, conn);
                    if (param != null)
                    {
                        SetParmDBNull(param);
                        objCommand.Parameters.AddRange(param);
                    }
                    int count = Convert.ToInt32(objCommand.ExecuteScalar());
                    return count;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString() + "\r\nQueryOnly:" + sqlText);
            }
        }

        /// <summary>
        /// 增删改 postgre 数据库，返回是否成功标识
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="param">参数集合</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sqlText, params OracleParameter[] param)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                using (var nSqlCommand = new OracleCommand(sqlText, conn))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    // 添加参数
                    if (param != null)
                    {
                        SetParmDBNull(param);
                        nSqlCommand.Parameters.AddRange(param);
                    }
                    // 执行查询并返回受影响的行数
                    int res = nSqlCommand.ExecuteNonQuery();
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString() + "\r\nExecuteNonQuery:" + sqlText);
            }
        }
        /// <summary>
        /// 插入sql
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keyValuePairs">数据</param>
        /// <returns></returns>
        public static string InsertSql(string tableName, OracleDictionary keyValuePairs)
        {
            List<string> fields = new List<string>();
            List<string> values = new List<string>();
            foreach (var item in keyValuePairs)
            {
                fields.Add(item.Key);
                values.Add(ConvertDBValue(item.Value));
            }
            string fieldStr = string.Join(", ", fields);
            string valueStr = string.Join(", ", values);
            string sql = $"insert into {tableName} ({fieldStr}) values ({valueStr})";
            return sql;
        }
        /// <summary>
        /// 插入sql
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keyValuePairs">数据</param>
        /// <param name="oracleParameters"></param>
        /// <returns></returns>
        public static string InsertSql(string tableName, OracleDictionary keyValuePairs, out OracleParameter[] oracleParameters)
        {
            List<string> fields = new List<string>();
            List<string> values = new List<string>();
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            foreach (var item in keyValuePairs)
            {
                fields.Add(item.Key);
                values.Add(":" + item.Key);
                oracleParameterList.Add(new OracleParameter(":" + item.Key, item.Value));
            }
            string fieldStr = string.Join(", ", fields);
            string valueStr = string.Join(", ", values);
            string sql = $"insert into {tableName} ({fieldStr}) values ({valueStr})";
            oracleParameters = oracleParameterList.ToArray();
            return sql;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keyValuePairs">数据</param>
        /// <returns></returns>
        public static int Insert(string tableName, OracleDictionary keyValuePairs)
        {
            string sql = InsertSql(tableName, keyValuePairs);
            var res = OracleHelper.ExecuteNonQuery(sql);
            return res;
        }
        /// <summary>
        /// 更新Sql
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keyValuePairs">数据</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public static string UpdateSql(string tableName, string where, OracleDictionary keyValuePairs)
        {
            List<string> sets = new List<string>();
            foreach (var item in keyValuePairs)
            {
                sets.Add($"{item.Key}={ConvertDBValue(item.Value)}");
            }
            string setStr = string.Join(", ", sets);
            string sql = $"update {tableName} set {setStr}";
            if (!string.IsNullOrEmpty(where))
                sql += $" where {where}";
            return sql;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keyValuePairs">数据</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public static int Update(string tableName, string where, OracleDictionary keyValuePairs)
        {
            string sql = UpdateSql(tableName, where, keyValuePairs);
            var res = OracleHelper.ExecuteNonQuery(sql);
            return res;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public static int Delete(string tableName, string where)
        {
            string sql = $"delete {tableName} where {where}";
            var res = OracleHelper.ExecuteNonQuery(sql);
            return res;
        }

        /// <summary>
        /// 批量执行
        /// </summary>
        /// <param name="sqlList">语句列表</param>
        /// <returns></returns>
        public static int BatchExecute(List<string> sqlList)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("begin ");
            for (int i = 0; i < sqlList.Count; i++)
            {
                sqlBuilder.Append(sqlList[i].TrimEnd(';')).Append(";");
            }
            sqlBuilder.Append("end;");
            string sql = sqlBuilder.ToString();
            var res = OracleHelper.ExecuteNonQuery(sql);
            return res;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>
        public static int ExecuteTransSQL(List<String> SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;

                string sqlText = null;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        sqlText = SQLStringList[n];
                        if (sqlText.Trim().Length > 1)
                        {
                            cmd.CommandText = sqlText;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw new Exception(ex.Message + "\r\nExecuteTransSQL:" + sqlText);
                }
            }
        }
        /// <summary>
        /// 将参数中null值转换为DBNull
        /// </summary>
        /// <param name="param"></param>
        public static void SetParmDBNull(OracleParameter[] param)
        {
            foreach (var p in param)
            {
                if (p.Value == null)
                {
                    p.Value = DBNull.Value;
                }
            }
        }
        /// <summary>
        /// DataTable转换List，反射机制
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> DataTableToList<T>(DataTable dt) where T : new()
        {
            IList<T> list = new List<T>();// 定义集合
            Type type = typeof(T); // 获得此模型的类型
            var tempName = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        if (!pi.CanWrite) continue;
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 转换成数据库值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertDBValue<T>(T value)
        {
            if (value == null)
                return "null";
            Type valueType = value.GetType();
            Type[] numberTypes = new Type[] { typeof(Int16), typeof(Int16?), typeof(Int32), typeof(Int32?), typeof(Int64), typeof(Int64?), typeof(decimal), typeof(decimal?), typeof(double), typeof(double?) };
            if (numberTypes.Contains(valueType))
                return value.ToString();
            else if (valueType == typeof(DateTime))
                return $"to_date('{value}', 'yyyy-mm-dd hh24:mi:ss')";
            else if (valueType == typeof(bool) || valueType.FullName.Contains(".Enun"))
                return Convert.ToInt32(value).ToString();
            return $"'{value}'";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ListConvertDBValue<T>(IEnumerable<T> values)
        {
            string value = string.Join(",", values.Select(t => ConvertDBValue(t)));
            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string In<T>(string columnName, IEnumerable<T> values)
        {
            if (values == null || !values.Any())
                return "1=1";
            string value = ListConvertDBValue(values);
            string result = string.Format("{0} in({1})", columnName, value);
            return result;
        }
        /// <summary>
        /// and name = value
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AndEquals(string columnName, object value)
        {
            string valueStr = ConvertDBValue(value);
            string result = string.Format(" And {0}={1}", columnName, valueStr);
            return result;
        }
        /// <summary>
        /// and name in()
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string AndIn<T>(string columnName, IEnumerable<T> values)
        {
            if (values == null || !values.Any())
                return "1=1";
            string value = ListConvertDBValue(values);
            string result = string.Format(" And {0} in({1})", columnName, value);
            return result;
        }
        /// <summary>
        /// or name in()
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string OrIn<T>(string columnName, IEnumerable<T> values)
        {
            if (values == null || !values.Any())
                return "1=1";
            string value = ListConvertDBValue(values);
            string result = string.Format(" Or {0} in({1})", columnName, value);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Like(string columnName, string value)
        {
            string result = string.Format("{0} like '%{1}%'", columnName, value);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AndLike(string columnName, string value)
        {
            string result = string.Format(" And {0} like '%{1}%'", columnName, value);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string OrLike(string columnName, string value)
        {
            string result = string.Format(" Or {0} like '%{1}%'", columnName, value);
            return result;
        }

        public static int GetCount(string sql)
        {
            int count = OracleHelper.QueryOnly($"SELECT count(*) FROM ({sql})");
            return count;
        }
        /// <summary>
        /// 分页foreach
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="count"></param>
        /// <param name="pageSize"></param>
        /// <param name="foreachAction"></param>
        public static void PageForeach(string sql, int count, int pageSize, Action<int, DataTable, Exception> foreachAction)
        {
            int pageCount = (int)Math.Ceiling(count / (double)pageSize);
            for (int i = 0; i < pageCount; i++)
            {
                int startNum = i * pageSize;
                int endNum = startNum + pageSize + 1;

                //                DataTable pageData = OracleHelper.ExecuteQuery($@"
                //SELECT PAGE_START_.* FROM (
                //  SELECT PAGE_END_.*, ROWNUM PAGE_START_NUM_ FROM(
                //    {sql}
                //  ) PAGE_END_ WHERE ROWNUM < {endNum}
                //) PAGE_START_ WHERE PAGE_START_.PAGE_START_NUM_  > {startNum}");

                try
                {
                    DataTable pageData = OracleHelper.ExecuteQuery($@"
SELECT * FROM (
  SELECT page___.*, ROWNUM rnum 
  FROM (
    {sql}
  ) page___ 
  WHERE ROWNUM < {endNum}
) 
WHERE rnum > {startNum}");

                    foreachAction?.Invoke(startNum, pageData, null);
                }
                catch (Exception ex)
                {
                    foreachAction?.Invoke(-1, null, ex);
                }
            }
        }

        //        /// <summary>
        //        /// 分页获取数据
        //        /// </summary>
        //        /// <param name="sql"></param>
        //        /// <param name="pageSize"></param>
        //        /// <returns></returns>
        //        public static DataTable PageGetData(string sql, int pageSize = 10)
        //        {
        //            DataTable data = null;
        //            int count = OracleHelper.QueryOnly($"SELECT count(*) FROM ({sql})");
        //            if (count == 0)
        //                return new DataTable();

        //            int pageCount = (int)Math.Ceiling(count / (double)pageSize);
        //            for (int i = 0; i < pageCount; i++)
        //            {
        //                int startNum = i * pageSize;
        //                int endNum = startNum + pageSize + 1;

        //                DataTable pageData = OracleHelper.ExecuteQuery($@"
        //SELECT PAGE_START_.* FROM (
        //  SELECT PAGE_END_.*, ROWNUM PAGE_START_NUM_ FROM(
        //    {sql}
        //  ) PAGE_END_ WHERE ROWNUM < {endNum}
        //) PAGE_START_ WHERE PAGE_START_.PAGE_START_NUM_  > {startNum}");
        //                if(data == null)
        //                {
        //                    data = pageData.Clone();
        //                }
        //                foreach (DataRow row in pageData.Rows)
        //                {
        //                    data.ImportRow(row);
        //                }
        //            }
        //            return data;

        //        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static DataTable PageGetData(string sql, int pageSize = 10)
        {
            DataTable data = null;
            if (pageSize <= 0)
            {
                data = ExecuteQuery(sql);
                return data;
            }

            int pageIndex = 0;
            while (true)
            {
                pageIndex++;
                int startNum = pageIndex * pageSize;
                int endNum = startNum + pageSize + 1;

                DataTable pageData = OracleHelper.ExecuteQuery($@"
SELECT PAGE_START_.* FROM (
  SELECT PAGE_END_.*, ROWNUM PAGE_START_NUM_ FROM(
    {sql}
  ) PAGE_END_ WHERE ROWNUM < {endNum}
) PAGE_START_ WHERE PAGE_START_.PAGE_START_NUM_  > {startNum}");
                int count = pageData.Rows.Count;
                if (data == null)
                {
                    data = pageData.Clone();
                }
                if (count == 0)
                {
                    break;
                }
                foreach (DataRow row in pageData.Rows)
                {
                    data.ImportRow(row);
                }
                if (count < pageSize)
                {
                    break;
                }
            }
            return data;

        }

    }

    public class OracleDictionary : Dictionary<string, object>
    {
    }
}

