using DevExpress.XtraEditors;
using Insurance.Insur.Helper;
using RKYC.UI.Windows.HIMS.Nurse.Common;
using RKYC.UI.Windows.HIMS.Nurse.InsuranceAdmit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using test.Common;
using test.Dtos;

namespace test
{
    public partial class Form1 : Form
    {
        HttpHelper hh = new HttpHelper();
        public Form1()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //GetAdmitPatInfoList(textEdit1.Text);
            Test();
        }

        private void Test()
        {
            AdmitPatInfoOutput input = new AdmitPatInfoOutput()
            {
                Sex = "男"
            };
            string queryText = hh.GetQueryString(ApiMethod.Admit.Test,null);
            var result= hh.Post<bool>(queryText, input);

        }

        private List<AdmitPatInfoOutput> GetAdmitPatInfoList(string patientId)
        {
            try
            {
                Dictionary<string, string> dit = new Dictionary<string, string>();
                dit.Add("patientid", patientId);

                // 构建查询字符串
                string queryText = hh.GetQueryString(ApiMethod.Admit.GetAdmitPatInfoList, dit);

                // 同步调用Get方法
                var patientInHospitalInfoList = hh.Get<List<AdmitPatInfoOutput>>(queryText);

                return patientInHospitalInfoList;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"获取数据失败: {ex.Message}");
                return new List<AdmitPatInfoOutput>(); // 返回空列表而非null，避免后续处理空引用
            }
        }

        private void textEditYblb_Click(object sender, EventArgs e)
        {
            InputMethodForm inputMethodForm = new InputMethodForm();
            inputMethodForm.dictType = "医保类别";
            inputMethodForm.dicTypeName = "医保类别";
            inputMethodForm.needColField = "INSURANCE_TYPE_NAME";
            inputMethodForm.orderBy = "INPUT_CODE";
            if (inputMethodForm.ShowDialog() == DialogResult.Cancel)
            {
                if (!string.IsNullOrEmpty(inputMethodForm.dicSelectItem))
                {
                    textEditYblb.Text = inputMethodForm.dicSelectItem;
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            AdmitSaveInput admitSaveInput = new AdmitSaveInput();
            //生育判断
            var insuranceType = textEditYblb.Text;
            if (insuranceType == "生育住院*")
            {
                InsurSyinfoFrm isf = new InsurSyinfoFrm();
                var diaRes = isf.ShowDialog();
                if (diaRes == DialogResult.Cancel)
                {
                    admitSaveInput.FpscNo = isf.insurSyinfo.fpsc_no;
                    admitSaveInput.MatnType = isf.insurSyinfo.matn_type;
                    admitSaveInput.BirctrlType = isf.insurSyinfo.birctrl_type;
                    admitSaveInput.LatechbFlag = isf.insurSyinfo.latechb_flag;
                    admitSaveInput.GesoVal = isf.insurSyinfo.geso_val;
                    admitSaveInput.Fetts = isf.insurSyinfo.fetts;
                    admitSaveInput.FetusCnt = isf.insurSyinfo.fetus_cnt;
                    admitSaveInput.PretFlag = isf.insurSyinfo.pret_flag;
                    admitSaveInput.BirctrlMatnDate = isf.insurSyinfo.birctrl_matn_date;
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //获取当前机器ip地址
            string ipAddress = "";
            try
            {
                System.Net.IPAddress[] arrIPAddresses = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
                foreach (System.Net.IPAddress ip in arrIPAddresses)
                {
                    if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                    {
                        ipAddress = ip.ToString();
                        break;
                    }
                }

                string outpOrInp = "";
                string sql = string.Format(@"select OUTP_OR_INP from dept_dict where dept_code='{0}'", "010232");
                DataSet dataBySql = DbHelperOra.Query(sql);
                if (dataBySql.Tables[0].Rows.Count > 0)
                {
                    outpOrInp = dataBySql.Tables[0].Rows[0][0].ToString();
                }


                int program = -1;//非门诊住院 -1
                if (outpOrInp == "0")
                {
                    program = 0;
                }
                else if (outpOrInp == "1")
                {
                    program = 1;
                }
                string insertsql = string.Format(@"insert into whrk.login_record_log (login_user,login_date,login_ip,program) values ('{0}',to_date('{1}','yyyy-mm-dd hh24:mi:ss'),'{2}','{3}')", "0000", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ipAddress, program);
                if (DbHelperOra.ExecuteSql(insertsql) < 0)
                {
                    MessageBox.Show("记录登录日志失败！");

                }
            }
            catch (Exception ex)
            {

            }

   
        }

        private Timer _timer;


        private void simpleButton3_Click(object sender, EventArgs e)
        {
            StartJob();
        }
                    private void StartJob()
        {
            _timer = new Timer();
            _timer.Interval = 60 * 1000; // 1分钟执行一次
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            try
            {
                DoWork();
            }
            catch (Exception ex)
            {
                MessageBox.Show("任务执行失败：" + ex.Message);
            }
            finally
            {
                _timer.Start();
            }
        }

        private void DoWork()
        {
            //Console.WriteLine("执行定时任务：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            MessageBox.Show("执行定时任务：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // 这里写你的业务逻辑
            // 比如查询数据库、调用接口、写日志等
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
          string versonNumber=  GetNewVersionNumber("version_number_OUPT");
            MessageBox.Show("最新版本号:"+versonNumber);
        }

        public string GetNewVersionNumber(string key)
        {
            string newVersionNumber = "";
            string xmlText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""ServerURI"" value=""http://192.168.81.25:5025"" />
    <add key=""version_number_OUPT"" value=""20260323001"" />
  </appSettings>
</configuration>";

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlText);

                XmlNodeList nodes = doc.SelectNodes("/configuration/appSettings/add");
                if (nodes == null)
                {
                    return string.Empty;
                }

                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes != null &&
                        node.Attributes["key"] != null &&
                        node.Attributes["value"] != null &&
                        node.Attributes["key"].Value == key)
                    {
                        return node.Attributes["value"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                //LogUtil.WriteLine(ex, "获取最新版本信息");
                MessageBox.Show("信息：解析ZgEMRControl.dll.config文件错误\r\n错误信息：" + ex.Message, "获取最新版本信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newVersionNumber;
        }

        public  string DataTableToString(DataTable dt)
        {
            if (dt == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            // 表头
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0) sb.Append("\t");
                sb.Append(dt.Columns[i].ColumnName);
            }
            sb.AppendLine();

            // 数据行
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) sb.Append("\t");

                    object value = row[i];
                    sb.Append(value == DBNull.Value ? string.Empty : Convert.ToString(value));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("name", typeof(string));
            DataRow newRow = dt.NewRow();
            newRow["id"] = 1;
            newRow["name"] = "cs";
            dt.Rows.Add(newRow);

            string str = DataTableToString(dt);
            XtraMessageBox.Show(str, "提示");
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("该患者已经存在未停止的护理等级医嘱，请先停止后再开新的护理等级医嘱\r\n如存在作废护理等级医嘱，请联系护士站校对后重新开立。", "提示");

        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            RichTextBoxLinkDemo richTextBoxLinkDemo = new RichTextBoxLinkDemo();
            richTextBoxLinkDemo.ShowDialog();
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {

        }
    }
}
