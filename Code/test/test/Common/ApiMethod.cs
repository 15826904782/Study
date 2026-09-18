using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Common
{
    public class ApiMethod
    {
        public class Admit
        {
            public static string GetAdmitPatInfoList = "Admit/GetAdmitPatInfoList";//获取已登记病人信息
            public static string Test = "Admit/Test";//

        }


        public class InputMethod
        {
            public static string GetInputTypeList = "InputMethod/GetInputTypeList"; //获取输入法不包含inputcode的数据集合
            public static string GetInputSettingDataList = "InputMethod/GetInputSettingDataList"; //获取输入码数据
            public static string GetInputSettingList = "InputMethod/GetInputSettingList"; // 获取输入码列属性数据
        }

        public class Hospital
        {
            public static string GetHospitalDataList = "Hospital/GetHospitalDataList"; //获取医院数据
            public static string GetHospitalData = "Hospital/GetHospitalData"; //获取医院数据
        }

    }
}