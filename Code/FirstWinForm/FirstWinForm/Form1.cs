using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private DataTable sourceDt;

        private void Form1_Load(object sender, EventArgs e)
        {
            textEdit_input.Focus();
            InitData();
        }


        private void InitData()
        {
            string connString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.31.201)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=oracle)));User Id=system;Password=manager;Persist Security Info=True;";
            OracleConnection conn = new OracleConnection(connString);//实例化
            conn.Open();

            string sql = "select t.* from COMM.DIED_DIAGNOSIS_DICT t";
            OracleCommand cmd = new OracleCommand(sql, conn);
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataSet ds = new DataSet();//定义数据集
            da.Fill(ds);
            sourceDt = ds.Tables[0];
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {


            textEdit_outPut.Text = "";

            //示例A54.002
            var str = textEdit_input.Text;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            var icdCode = GetICDCode(str);
            if (string.IsNullOrEmpty(icdCode))
            {
                MessageBox.Show("未找到对应ICD编码");
            }

            textEdit_outPut.Text = icdCode;


        }

        private string GetICDCode(string input)
        {
            string outPut = ""; // 初始化输出字符串为空
            var inputChar = input.ToCharArray()[0]; // 获取输入字符串的第一个字符

            foreach (DataRow row in sourceDt.Rows) // 遍历数据源中的每一行
            {
                var diseaseIcd = row["DISEASE_ICD"].ToString(); // 获取当前行的疾病ICD代码
                if (!string.IsNullOrEmpty(outPut)) break; // 如果输出字符串不为空，结束循环

                // 1.示例:B15-B19
                if (diseaseIcd.Contains("-") && diseaseIcd.Contains(inputChar)) // 检查ICD代码是否包含范围（如"-"）和输入字符
                {
                    var items = diseaseIcd.Split('-'); // 将ICD代码按"-"分割成两个部分
                    if (items.Length == 2) // 确保分割后的数组长度为2
                    {
                        var strart = items[0].ToString(); // 获取范围的起始部分
                        var end = items[1].ToString(); // 获取范围的结束部分

                        List<string> strartNumbers = getAllNum(strart); // 提取起始部分的数字
                        List<string> endNumbers = getAllNum(end); // 提取结束部分的数字
                        decimal stratNum;
                        decimal.TryParse(strartNumbers[0].ToString(), out stratNum); // 将起始部分的数字转换为decimal
                        decimal endNum;
                        decimal.TryParse(endNumbers[0].ToString(), out endNum); // 将结束部分的数字转换为decimal

                        decimal inputNum;
                        if(input.Contains("x") && !diseaseIcd.Contains("x"))
                        {
                            input= input.Replace("x", "").Trim().ToString();
                        }
                        decimal.TryParse(Regex.Replace(input, "[A-Z]", "").Trim().ToString(), out inputNum); // 提取输入字符串中的数字部分并转换为decimal
                        if (inputNum >= stratNum && inputNum < endNum) // 检查输入数字是否在起始和结束数字之间
                        {
                            outPut = row["DISEASE_CODE"].ToString(); // 如果匹配，设置输出字符串为当前行的疾病代码
                            break; // 结束循环
                        }
                    }
                }
                else if (diseaseIcd.Contains("、") && diseaseIcd.Contains(inputChar)) // 示例J09.A56、J09.A58、J09.A74、J09.A92、J09.A103、J09.A108、J09.A38、J09.SA11、J09
                {
                    var items = diseaseIcd.Split('、'); // 将ICD代码按"、"分割成多个部分
                    foreach (var item in items) // 遍历分割后的每一部分
                    {
                        if (item.Contains(".") || input.Contains("x") || input.Contains("X")) // 检查ICD代码部分是否包含"." 或输入是否包含"x"或"X"
                        {
                            if (input.Contains(item)) // 如果输入包含当前ICD代码部分
                            {
                                outPut = row["DISEASE_CODE"].ToString(); // 设置输出字符串为当前行的疾病代码
                                break; // 结束循环
                            }
                        }
                        else
                        {
                            if (items.Contains(input)) // 如果输入字符串在ICD代码部分中
                            {
                                outPut = row["DISEASE_CODE"].ToString(); // 设置输出字符串为当前行的疾病代码
                                break; // 结束循环
                            }
                        }
                    }
                }
                else if (diseaseIcd.IndexOf(" ") > 0) // 示例:A60.0  A60.9 中间含空格
                {
                    var items = diseaseIcd.Split(' '); // 将ICD代码按空格分割成多个部分
                    foreach (var item in items) // 遍历分割后的每一部分
                    {
                        if (string.IsNullOrEmpty(item)) // 如果当前部分为空，跳过
                        {
                            continue;
                        }

                        if (input.Contains(item.Replace('+', ' ').Trim().ToString())) // 特殊处理:B01.00+  B01.10+ B01.20+  B01.80 B01.90
                        {
                            outPut = row["DISEASE_CODE"].ToString(); // 如果输入包含当前ICD代码部分
                            break; // 结束循环
                        }
                    }
                }
                else // 示例B06或A09.0
                {
                    if (input.Contains(diseaseIcd)) // 如果输入包含当前ICD代码
                    {
                        outPut = row["DISEASE_CODE"].ToString(); // 设置输出字符串为当前行的疾病代码
                        break; // 结束循环
                    }
                }
            }

            return outPut; // 返回输出字符串
        }




        private List<string> getAllNum(string str)
        {
            List<string> _list = new List<string>();

            if (!string.IsNullOrEmpty(str))
            {
                String pattern = @"\d*\.\d*|0\.\d*[1-9]\d*$";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection matchs = regex.Matches(str);
                foreach (Match m in matchs)
                {
                    _list.Add(m.Groups[0].Value);
                }
                str = Regex.Replace(str, pattern, "");

                string pattern2 = @"[0-9]+";
                Regex regex2 = new Regex(pattern2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection matchs2 = regex2.Matches(str);
                foreach (Match m in matchs2)
                {
                    _list.Add(m.Groups[0].Value);
                }
            }

            return _list;
        }



    }
}
