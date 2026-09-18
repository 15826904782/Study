using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test.Common;

namespace RKYC.UI.Windows.HIMS.Nurse.Common
{
    public partial class InputMethodForm : Form
    {
        public string dictType = "";
        public string dicTypeName = "";
        public string dicSelectItem = "";
        public string needColField = "";
        public string orderBy = "";
        public string orderAscOrDesc = "";
        public string where = "";
        public DataRow SelectItemRow;
        DataTable dtInputSetting = new DataTable();
        DataTable inputSettingDataList = new DataTable();
        public InputMethodForm()
        {
            InitializeComponent();
        }
        HttpHelper hh = new HttpHelper();

        private  void InputMethodForm_Load(object sender, EventArgs e)
        {
             BindInputType();
             BindInputSetting();
             BindHospital();
             BindInputSettingData();
            this.Text = dicTypeName;
        }

        private bool BindHospital()
        {
            string queryHospitalText = hh.GetQueryString(ApiMethod.Hospital.GetHospitalDataList, null);
            var hospitalList =  hh.Get<DataTable>(queryHospitalText);

            comboBoxEdit_hospital.Properties.Items.Clear();
            foreach (DataRow dr in hospitalList.Rows)
            {
                DropdownInfo drodownInfo = new DropdownInfo();
                drodownInfo.Key = dr["zoneName"].ToString();
                drodownInfo.Value = dr["zoneCode"].ToString();
                comboBoxEdit_hospital.Properties.Items.Add(drodownInfo);

            }
            comboBoxEdit_hospital.SelectedIndex = 0;
            return true;
        }

        private bool BindInputType()
        {
            Dictionary<string, string> dit = new Dictionary<string, string>();
            dit.Add("dictType", dictType);

            dit.Add("flagShow", "Y");
            string queryInputSettingText = hh.GetQueryString(ApiMethod.InputMethod.GetInputSettingList, dit);
            List<string> exclusionInputType = new List<string>();
            exclusionInputType.Add("00");
            var inputTypeResultList =  hh.Post<DataTable>(ApiMethod.InputMethod.GetInputTypeList, exclusionInputType);
            var inputSettingResultList =  hh.Get<DataTable>(queryInputSettingText);
            dtInputSetting = inputSettingResultList;//列属性记性

            comboBoxEdit_InputType.Properties.Items.Clear();
            foreach (DataRow dr in inputTypeResultList.Rows)
            {
                DataRow[] exsitDr = inputSettingResultList.Select("inputCode ='" + dr["inputCode"] + "'");
                if (exsitDr.Count() != 0)
                {
                    //每一个单元按钮对应的选线item
                    ComboBoxItem item = new ComboBoxItem();
                    //设置选项的value值
                    item.Value = dr["inputCode"].ToString() + "-" + dr["inputName"].ToString();
                    //将新增的选项添加到radiogroup的Items中
                    comboBoxEdit_InputType.Properties.Items.Add(item);
                }
            }
            comboBoxEdit_InputType.SelectedIndex = 0;
            return true;
        }

        private bool BindInputSetting()
        {
            Dictionary<string, string> ditData = new Dictionary<string, string>();
            ditData.Add("dictType", dictType);
            ditData.Add("orderBy", orderBy);
            ditData.Add("orderAscOrDesc", orderAscOrDesc);
            //ditData.Add("flagShow", "Y");只查询show的字段返回
            ditData.Add("where",where);
            ditData.Add("currentUserName", "whrk");
            ditData.Add("currentUserCnName", "武汉瑞康");
            string queryInputSettingDataText = hh.GetQueryString(ApiMethod.InputMethod.GetInputSettingDataList, ditData);
            string inputSettingDataResult =  hh.Get(queryInputSettingDataText);
            UnifyResultDto inputSettingDataResultObj = JsonConvert.DeserializeObject<UnifyResultDto>(inputSettingDataResult);
            var inputSettingDataResultList = JsonConvert.DeserializeObject<DataTable>(inputSettingDataResultObj.Data.ToString());
            gridView_inputSettingData.Columns.Clear();
            foreach (DataRow dr in dtInputSetting.Rows)
            {
                if (dr["flagShow"].ToString() == "Y")
                {
                    GridColumn column = new GridColumn();
                    column.FieldName = dr["dataCol"].ToString().ToUpper();//避免配置表dataCol为小写时，数据不显示
                    column.Name = "gridColumn_" + dr["dataCol"].ToString();
                    column.Caption = dr["dataTitle"].ToString();
                    column.Visible = true;
                    gridView_inputSettingData.Columns.Add(column);
                }
            }
            inputSettingDataList = inputSettingDataResultList;
            return true;
        }

        private  void simpleButton_refresh_Click(object sender, EventArgs e)
        {
             BindInputType();
             BindInputSetting();
             BindHospital();
            BindInputSettingData();
            textEdit_inputText.Text = "";
            this.Text = dicTypeName;
        }

        private void comboBoxEdit_InputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindInputSettingData();
        }

        private void textEdit_inputText_TextChanged(object sender, EventArgs e)
        {
            BindInputSettingData();
        }

        private void BindInputSettingData()
        {
            if (inputSettingDataList.Rows.Count != 0)
            {
                if (string.IsNullOrEmpty(textEdit_inputText.Text))
                {
                    if (comboBoxEdit_hospital.Properties.Items.Count > 0 && inputSettingDataList.Columns.Contains("ZONECODE"))
                    {
                        string value = ((DropdownInfo)comboBoxEdit_hospital.SelectedItem).Value;
                        DataTable newdt = new DataTable();
                        newdt = inputSettingDataList.Clone();
                        DataRow[] dr = inputSettingDataList.Select("ZONECODE = '" + value + "'");

                        for (int i = 0; i < dr.Length; i++)
                        {
                            newdt.ImportRow((DataRow)dr[i]);
                        }
                        gridControl_InputData.DataSource = newdt;
                    }
                    else
                    {
                        gridControl_InputData.DataSource = inputSettingDataList;
                    }
                }
                else
                {
                    if (comboBoxEdit_InputType.SelectedIndex >= 0)
                    {
                        string inputType = comboBoxEdit_InputType.Properties.Items[comboBoxEdit_InputType.SelectedIndex].ToString();
                        string inputCode = inputType.Split('-')[0];

                        DataRow[] exsitDr = dtInputSetting.Select("inputCode ='" + inputCode + "'");

                        string colFiled = exsitDr[0]["dataCol"].ToString();
                        string selectStr = colFiled + " like '%" + textEdit_inputText.Text + "%'";
                        if (comboBoxEdit_hospital.Properties.Items.Count > 0 && inputSettingDataList.Columns.Contains("ZONECODE"))
                        {
                            string value = ((DropdownInfo)comboBoxEdit_hospital.SelectedItem).Value;
                            selectStr = selectStr + " and ZONECODE = '" + value + "'";
                        }
                        DataTable newdt = new DataTable();
                        newdt = inputSettingDataList.Clone();
                        DataRow[] dr = inputSettingDataList.Select(selectStr);

                        for (int i = 0; i < dr.Length; i++)
                        {
                            newdt.ImportRow((DataRow)dr[i]);
                        }
                        gridControl_InputData.DataSource = newdt;
                    }
                    else
                    {
                        XtraMessageBox.Show("查询条件不能为空", "提示");
                    }
                }
            }
        }

        private void gridView_inputSettingData_DoubleClick(object sender, EventArgs e)
        {
            if (gridView_inputSettingData.FocusedRowHandle >= 0)
            {
                DataRow currRow = gridView_inputSettingData.GetDataRow(gridView_inputSettingData.FocusedRowHandle);
                dicSelectItem = currRow[needColField].ToString();
                SelectItemRow = currRow;
                this.Close();
            }
            else
            {
                dicSelectItem = "";
                SelectItemRow = null;
                this.Close();
            }
        }

        private void comboBoxEdit_hospital_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindInputSettingData();
        }
    }
}
