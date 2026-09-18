using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test.Dtos;

namespace RKYC.UI.Windows.HIMS.Nurse.InsuranceAdmit
{
    public partial class InsurSyinfoFrm : Form
    {
        public InsurSyinfoFrm()
        {
            InitializeComponent();
        }

        public InsurSyinfo insurSyinfo;

        private void InsurSyinfoMzFrm_Load(object sender, EventArgs e)
        {
            InitData1();
            InitData3();
            InitData2();
            dateEdit_sy.DateTime = DateTime.Now;


        }


        private void InitData2()
        {
            var list = new List<string>()
            {
                "1正常产",
                "3剖宫产",
                "5流产",
                "6引产",
                "8放置宫内节育器",
                "9取出宫内节育器",
                "a绝育手术(输卵管)",
                "b绝育手术(输精管)",
                "c更换宫内节育器"
            };
            comboBoxEdit2.Properties.Items.Clear();
            comboBoxEdit2.Properties.Items.AddRange(list);
        }

        private void InitData1()
        {
            var list = new List<string>()
            {
                "1放置宫内节育器",
                "2取出宫内节育器",
                "3流产术",
                "4绝育手术",
                "5绝育复通手术",
                "3、剖宫产",
            };
            comboBoxEdit1.Properties.Items.Clear();
            comboBoxEdit1.Properties.Items.AddRange(list);
        }
        private void InitData3()
        {
            var list = new List<string>()
            {
                "7、产前检查",
                "1、正常产",
                "10、绝育手术(输卵管)",
                "11、绝育手术(输精管)",
                "2、助娩产",
                "3、剖宫产",
                "4、难产",
                "5、流产",
                "6、引产",
                "8、放置宫内节育器"
            };
            comboBoxEdit3.Properties.Items.Clear();
            comboBoxEdit3.Properties.Items.AddRange(list);
        }

        private void simpleButton_ok_Click(object sender, EventArgs e)
        {
            insurSyinfo = new InsurSyinfo();

            if (!string.IsNullOrEmpty(comboBoxEdit2.Text))
                insurSyinfo.matn_type = comboBoxEdit2.Text.Trim().Substring(0, 1);

            if (insurSyinfo.matn_type == "a")
            {
                insurSyinfo.matn_type = "10";
            }

            if (insurSyinfo.matn_type == "b")
            {
                insurSyinfo.matn_type = "11";
            }

            if (insurSyinfo.matn_type == "c")
            {
                insurSyinfo.matn_type = "953";
            }

            if (checkEdit_wy.Checked)
            {
                insurSyinfo.latechb_flag = "1";
            }
            else
            {
                insurSyinfo.latechb_flag = "0";
            }

            if (checkEdit_zc.Checked)
            {
                insurSyinfo.pret_flag = "1";
            }
            else
            {
                insurSyinfo.pret_flag = "0";
            }

            if (!string.IsNullOrEmpty(comboBoxEdit1.Text))
                insurSyinfo.birctrl_type = comboBoxEdit1.Text.Trim().Substring(0, 1);
            insurSyinfo.birctrl_matn_date = dateEdit_sy.DateTime.ToString("yyyy-MM-dd HH:mm:ss");

            //孕周
            int geso_val;
            int.TryParse(textEdit_yz.Text,out geso_val);
            insurSyinfo.geso_val = geso_val;

            //胎次
            int fetts;
            int.TryParse(textEdit_tc.Text,out fetts);
            insurSyinfo.fetts = fetts;

            //胎儿数
            int fetus_cnt;
            int.TryParse(textEdit_trs.Text,out fetus_cnt);
            insurSyinfo.fetus_cnt = fetus_cnt;
            Close();
        }
    }
}
