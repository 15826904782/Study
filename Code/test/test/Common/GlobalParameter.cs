using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Common
{
    public static class GlobalParameter
    {
        public static string Authorization { get; set; }


        /// <summary>
        /// 权限列表
        /// </summary>
        public static DataTable PermissionList { get; set; }

        public static string ZoneCode { get; set; }

        public static string ZoneName { get; set; }
        public static string HospitalName { get; set; }
        public static string CreaterDept { get; set; }       

        //用户权限
        public static string UserCapability { get; set; }

        //应用程序名
        public static string ApplicationName { get; set; }

        public static DataRow OpenReportDataRowData { get; set; }

        //药房
        public static List<string> OutpDrugStorage { get; set; }
        //选中药房
        public static List<string> DrugStorageSelected { get; set; }
        //病区编码
        public static string WardCode { get; set; }
        public static string WardName { get; set; }
        //科室编码
        public static string DeptCode { get; set; }


        public static string BedPosition { get; set; }

        /// <summary>
        /// 是否支持IC卡
        /// </summary>
        public static bool UseCard { get; set; }
    }
}
