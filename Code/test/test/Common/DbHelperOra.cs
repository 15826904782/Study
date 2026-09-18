using System.Collections;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;
using System;
using System.Collections.Generic;

namespace Insurance.Insur.Helper
{
    /// <summary>
    /// 数据访问基础类(基于Oracle)
    /// 可以用户可以修改满足自己项目的需要。
    /// </summary>
    public abstract class DbHelperOra
    {
        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
        public static string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        public DbHelperOra()
        {
        }
        #region 公用方法

        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(to_number(" + FieldName + "))+1 from " + TableName;
            object obj = DbHelperOra.GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public static bool Exists(string strSql)
        {
            object obj = DbHelperOra.GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool Exists(string strSql, params OracleParameter[] cmdParms)
        {
            object obj = DbHelperOra.GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region  执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.OracleClient.OracleException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		

        public static bool ExecuteSqlTranS(ArrayList SQLStringList)
        {
            bool StrReturn = true;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = connection;
                OracleTransaction tx = connection.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    tx.Rollback();
                    StrReturn = false;// "数据库操作失败，错误为：" + E.Message;
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
            return StrReturn;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static bool ExecuteSqlTran(ArrayList SQLStringList)

        {
            bool isSucces = false;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = connection;
                OracleTransaction tx = connection.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    isSucces = true;
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    tx.Rollback();
                    isSucces = false;
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();

                }
            }
            return isSucces;
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string content)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand(SQLString, connection);
                System.Data.OracleClient.OracleParameter myParameter = new System.Data.OracleClient.OracleParameter("@content", OracleType.NVarChar);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand(strSQL, connection);
                System.Data.OracleClient.OracleParameter myParameter = new System.Data.OracleClient.OracleParameter("@fs", OracleType.LongRaw);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.OracleClient.OracleException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string strSQL)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand cmd = new OracleCommand(strSQL, connection);
            try
            {
                connection.Open();
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.OracleClient.OracleException e)
            {
                throw new Exception(e.Message);
            }


        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataSet(分页用)
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <param name="StartIndex">制定返回多少行以后的数据</param>
        /// <param name="Count">制定总共返回多少行</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, int StartIndex, int Count)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, StartIndex, Count, "Table1");
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.OracleClient.OracleException E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.OracleClient.OracleException e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string SQLString, params OracleParameter[] cmdParms)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.OracleClient.OracleException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.OracleClient.OracleException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                    return ds;
                }
            }
        }


        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程 返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleDataReader returnReader;
            connection.Open();
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                OracleDataAdapter sqlDA = new OracleDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet GetProcedureDataSet(string storedProcName, List<string> paramKey, List<string> paramValue, string tableName, string sql)
        {
            DataSet ds = new DataSet();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                OracleCommand command = new OracleCommand();
                command.Connection = connection;
                OracleTransaction tx = connection.BeginTransaction();
                command.Transaction = tx;
                try
                {
                    for (int i = 0; i < paramKey.Count; i++)
                    {
                        OracleParameter oraParam = new OracleParameter();
                        oraParam.ParameterName = paramKey[i].ToString();
                        oraParam.Value = paramValue[i].ToString();
                        oraParam.Direction = ParameterDirection.Input;
                        command.Parameters.Add(oraParam);

                    }

                    OracleParameter oraParam1 = new OracleParameter();
                    oraParam1.ParameterName = "REFCURSOR";
                    oraParam1.OracleType = OracleType.Cursor;
                    oraParam1.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oraParam1);

                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;

                    OracleDataAdapter sqlDA = new OracleDataAdapter(command);
                    sqlDA.Fill(ds, tableName);


                    tx.Commit();
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return ds;
        }




        /// <summary>
        /// 构建 OracleCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private static OracleCommand BuildQueryCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                int result;
                connection.Open();
                OracleCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 OracleCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand 对象实例</returns>
        private static OracleCommand BuildIntCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new OracleParameter("ReturnValue",
                OracleType.Int32, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion
        ///// <summary>
        ///// 执行多条SQL语句，实现数据库事务。
        ///// </summary>
        ///// <param name="SQLStringList">多条SQL语句</param>		
        //public static string ExecuteSqlTranS(ArrayList SQLStringList)
        //{
        //    string StrReturn = "操作完成！";
        //    using (OracleConnection connection = new OracleConnection(connectionString))
        //    {
        //        connection.Open();
        //        OracleCommand cmd = new OracleCommand();
        //        cmd.Connection = connection;
        //        OracleTransaction tx = connection.BeginTransaction();
        //        cmd.Transaction = tx;
        //        try
        //        {
        //            for (int n = 0; n < SQLStringList.Count; n++)
        //            {
        //                string strsql = SQLStringList[n].ToString();
        //                if (strsql.Trim().Length > 1)
        //                {
        //                    cmd.CommandText = strsql;
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //            tx.Commit();
        //        }
        //        catch (System.Data.OracleClient.OracleException E)
        //        {
        //            tx.Rollback();
        //            StrReturn = "数据库操作失败，错误为：" + E.Message;
        //            throw new Exception(E.Message);
        //        }
        //        finally
        //        {
        //            cmd.Dispose();
        //            connection.Close();
        //        }
        //    }
        //    return StrReturn;
        //}

        /// 存放图片
        public static string save_picture(byte[] imageBytes, string user_name)
        {
            String connectionstring = connectionString;
            OracleConnection con = new OracleConnection(connectionstring);
            String sql = @"update staff_dict set AUTOGRAPH=:myimage  where user_name='" + user_name + "'";//在testxx表中有个字段叫image是blob类型的注意中的冒号，通过myid制定记录
            OracleCommand cmd = new OracleCommand(sql, con);
            OracleTransaction otx = con.BeginTransaction();
            cmd.Transaction = otx;
            OracleParameter para1 = new OracleParameter("myimage", OracleType.Blob);
            para1.Value = imageBytes;
            cmd.Parameters.Add(para1);
            // cmd.Parameters.Add(new OracleParameter("myimage", OracleType.Blob));//给这个两个参数赋值myimage和myid 
            // //cmd.Parameters.Add(new OracleParameter("myid", OracleType.VarChar));
            ////cmd.Parameters["myid"].Value = user_name;  //给image字段赋值字节数组 
            // byte[] imagebyte = imageBytes;
            // cmd.Parameters["myimage"].Value =  imagebyte;


            try
            {
                int result = cmd.ExecuteNonQuery();
                if (result < 1)
                {
                    return "0";
                }
                else
                {
                    return "-1";
                }
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
        /// <summary>
        /// 执行存储过程    by zhouqx
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static ArrayList GetProcedureDataSet(string storedProcName, List<string> paramKey, List<string> paramKeytype, List<string> paramValue, List<string> paramKeyout, List<string> paramKeyouttype)
        {
            DataSet ds = new DataSet();
            ArrayList out_list = new ArrayList();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                OracleCommand command = new OracleCommand();
                command.Connection = connection;
                OracleTransaction tx = connection.BeginTransaction();
                command.Transaction = tx;
                try
                {
                    //入参
                    int count = 0;
                    for (int i = 0; i < paramKey.Count; i++)
                    {
                        OracleParameter oraParam = new OracleParameter();
                        oraParam.ParameterName = paramKey[i].ToString();

                        if (paramKeytype[i].ToString() == "number")
                        {
                            oraParam.OracleType = OracleType.Number;
                        }
                        if (paramKeytype[i].ToString() == "varchar")
                        {
                            oraParam.OracleType = OracleType.VarChar;
                        }
                        if (paramKeytype[i].ToString() == "datetime")
                        {
                            oraParam.OracleType = OracleType.DateTime;
                        }
                        oraParam.Value = paramValue[i].ToString();
                        oraParam.Direction = ParameterDirection.Input;
                        command.Parameters.Add(oraParam);
                        count++;

                    }
                    //出参
                    for (int j = 0; j < paramKeyout.Count; j++)
                    {
                        OracleParameter oraParam = new OracleParameter();
                        oraParam.ParameterName = paramKeyout[j].ToString();

                        if (paramKeyouttype[j].ToString() == "number")
                        {
                            oraParam.OracleType = OracleType.Number;
                        }
                        if (paramKeyouttype[j].ToString() == "varchar")
                        {
                            oraParam.OracleType = OracleType.VarChar;
                            oraParam.Size = 1000;
                        }
                        if (paramKeyouttype[j].ToString() == "datetime")
                        {
                            oraParam.OracleType = OracleType.DateTime;
                        }

                        oraParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(oraParam);

                        //command.Parameters.Add(paramKeyout[j].ToString(), OracleType.VarChar, 1000);
                        //command.Parameters[paramKeyout[j].ToString()].Direction = ParameterDirection.Output;
                    }

                    //OracleParameter oraParam1 = new OracleParameter();
                    //oraParam1.ParameterName = "REFCURSOR";
                    //oraParam1.OracleType = OracleType.Cursor;
                    //oraParam1.Direction = ParameterDirection.Output;
                    //command.Parameters.Add(oraParam1);

                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;
                    // command.ExecuteNonQuery();

                    // OracleDataAdapter sqlDA = new OracleDataAdapter(command);
                    command.ExecuteNonQuery();
                    // sqlDA.Fill(ds, tableName);
                    //整理输出值

                    for (int j = 0; j < paramKeyout.Count; j++)
                    {
                        out_list.Add((command.Parameters[count + j].Value == null ? "" : command.Parameters[count + j].Value).ToString());
                    }

                    tx.Commit();
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return out_list;
        }

        /// <summary>
        /// 执行存储过程    by zhouqx
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static ArrayList GetProcedureDataSet_DS(string storedProcName, List<string> paramKey, List<string> paramKeytype, List<string> paramValue, List<string> paramKeyout, List<string> paramKeyouttype, string tableName, ref DataSet ds)
        {
            ///DataSet ds = new DataSet();
            ArrayList out_list = new ArrayList();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                OracleCommand command = new OracleCommand();
                command.Connection = connection;
                OracleTransaction tx = connection.BeginTransaction();
                command.Transaction = tx;
                try
                {
                    //入参
                    int count = 0;
                    for (int i = 0; i < paramKey.Count; i++)
                    {
                        OracleParameter oraParam = new OracleParameter();
                        oraParam.ParameterName = paramKey[i].ToString();

                        if (paramKeytype[i].ToString() == "number")
                        {
                            oraParam.OracleType = OracleType.Number;
                        }
                        if (paramKeytype[i].ToString() == "varchar")
                        {
                            oraParam.OracleType = OracleType.VarChar;
                        }
                        if (paramKeytype[i].ToString() == "datetime")
                        {
                            oraParam.OracleType = OracleType.DateTime;
                        }
                        oraParam.Value = paramValue[i].ToString();
                        oraParam.Direction = ParameterDirection.Input;
                        command.Parameters.Add(oraParam);
                        count++;

                    }
                    //出参
                    for (int j = 0; j < paramKeyout.Count; j++)
                    {
                        OracleParameter oraParam = new OracleParameter();
                        oraParam.ParameterName = paramKeyout[j].ToString();

                        if (paramKeyouttype[j].ToString() == "number")
                        {
                            oraParam.OracleType = OracleType.Number;
                        }
                        if (paramKeyouttype[j].ToString() == "varchar")
                        {
                            oraParam.OracleType = OracleType.VarChar;
                            oraParam.Size = 1000;
                        }
                        if (paramKeyouttype[j].ToString() == "datetime")
                        {
                            oraParam.OracleType = OracleType.DateTime;
                        }

                        oraParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(oraParam);

                        //command.Parameters.Add(paramKeyout[j].ToString(), OracleType.VarChar, 1000);
                        //command.Parameters[paramKeyout[j].ToString()].Direction = ParameterDirection.Output;
                    }

                    OracleParameter oraParam1 = new OracleParameter();
                    oraParam1.ParameterName = "aResultset";
                    oraParam1.OracleType = OracleType.Cursor;
                    oraParam1.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oraParam1);

                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;
                    //command.ExecuteNonQuery();

                    OracleDataAdapter sqlDA = new OracleDataAdapter(command);
                    //command.ExecuteNonQuery();
                    sqlDA.Fill(ds, tableName);
                    //整理输出值

                    for (int j = 0; j < paramKeyout.Count; j++)
                    {
                        out_list.Add((command.Parameters[count + j].Value == null ? "" : command.Parameters[count + j].Value).ToString());
                    }

                    tx.Commit();
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return out_list;
        }
        /// <summary>
        /// 执行存储过程    by zhouqx
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static List<string> GetProcedureDataSet_DS2(string storedProcName, List<string> paramKey, List<string> paramKeytype, List<string> paramValue, List<string> paramKeyout, List<string> paramKeyouttype, string tableName, ref DataSet ds)
        {
            ///DataSet ds = new DataSet();
            List<string> out_list = new List<string> { };
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                OracleCommand command = new OracleCommand();
                command.Connection = connection;
                OracleTransaction tx = connection.BeginTransaction();
                command.Transaction = tx;
                try
                {
                    //入参
                    int count = 0;
                    for (int i = 0; i < paramKey.Count; i++)
                    {
                        OracleParameter oraParam = new OracleParameter();
                        oraParam.ParameterName = paramKey[i].ToString();

                        if (paramKeytype[i].ToString() == "number")
                        {
                            oraParam.OracleType = OracleType.Number;
                        }
                        if (paramKeytype[i].ToString() == "varchar")
                        {
                            oraParam.OracleType = OracleType.VarChar;
                        }
                        if (paramKeytype[i].ToString() == "datetime")
                        {
                            oraParam.OracleType = OracleType.DateTime;
                        }
                        oraParam.Value = paramValue[i].ToString();
                        oraParam.Direction = ParameterDirection.Input;
                        command.Parameters.Add(oraParam);
                        count++;

                    }
                    //出参
                    for (int j = 0; j < paramKeyout.Count; j++)
                    {
                        OracleParameter oraParam = new OracleParameter();
                        oraParam.ParameterName = paramKeyout[j].ToString();

                        if (paramKeyouttype[j].ToString() == "number")
                        {
                            oraParam.OracleType = OracleType.Number;
                        }
                        if (paramKeyouttype[j].ToString() == "varchar")
                        {
                            oraParam.OracleType = OracleType.VarChar;
                            oraParam.Size = 1000;
                        }
                        if (paramKeyouttype[j].ToString() == "datetime")
                        {
                            oraParam.OracleType = OracleType.DateTime;
                        }

                        oraParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(oraParam);

                        //command.Parameters.Add(paramKeyout[j].ToString(), OracleType.VarChar, 1000);
                        //command.Parameters[paramKeyout[j].ToString()].Direction = ParameterDirection.Output;
                    }

                    OracleParameter oraParam1 = new OracleParameter();
                    oraParam1.ParameterName = "aResultset";
                    oraParam1.OracleType = OracleType.Cursor;
                    oraParam1.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oraParam1);

                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;
                    //command.ExecuteNonQuery();

                    OracleDataAdapter sqlDA = new OracleDataAdapter(command);
                    //command.ExecuteNonQuery();
                    sqlDA.Fill(ds, tableName);
                    //整理输出值

                    for (int j = 0; j < paramKeyout.Count; j++)
                    {
                        out_list.Add((command.Parameters[count + j].Value == null ? "" : command.Parameters[count + j].Value).ToString());
                    }

                    tx.Commit();
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return out_list;
        }

    }
}
