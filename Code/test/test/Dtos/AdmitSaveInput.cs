using System;

namespace test.Dtos
{
    /// <summary>
    /// 入院登记
    /// </summary>
    public class AdmitSaveInput
    {
        ///// <summary>
        ///// 
        ///// </summary>
        //public NoAdmCardInfoOutput NoAdmCardInfoOutput { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public InsurCard CardInfo { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public InsuranceInitInput InsurInitInput { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public int checkDise { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public InsurBaseInput InsurBaseInput { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BusinessCycle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CenterSerialNo { get; set; }
        /// <summary>
        /// 计划生育服务证号
        /// </summary>
        public string FpscNo { get; set; }

        /// <summary>
        /// 计划生育手术类别
        /// </summary>
        public string BirctrlType { get; set; }
        /// <summary>
        /// 计划生育手术或生育日期
        /// </summary>
        public string BirctrlMatnDate { get; set; }
        /// <summary>
        /// 生育类别
        /// </summary>
        public string MatnType { get; set; }
        /// <summary>
        /// 晚育标志
        /// </summary>
        public string LatechbFlag { get; set; }
        /// <summary>
        /// 孕周数
        /// </summary>
        public int? GesoVal { get; set; }
        /// <summary>
        /// 胎次
        /// </summary>
        public int? Fetts { get; set; }
        /// <summary>
        /// 胎儿数
        /// </summary>
        public int? FetusCnt { get; set; }
        /// <summary>
        /// 早产标志
        /// </summary>
        public string PretFlag { get; set; }
        public int CheckDise { get; set; }
    }

    /// <summary>
    /// 无住院证患者信息
    /// </summary>
    public class NoAdmCardInfoOutput
    {
        /// <summary>
        /// 病人标识号
        /// </summary>  
        public string PatientId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int VisitId { get; set; }
        /// <summary>
        /// 住院号
        /// </summary>  
        public string InpNo { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>  
        public string Name { get; set; }
        /// <summary>
        /// 姓名拼音
        /// </summary>  
        public string NamePhonetic { get; set; }
        /// <summary>
        /// 性别
        /// </summary>  
        public string Sex { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>  
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }
        /// <summary>
        /// 出生地代码
        /// </summary>  
        public string BirthPlace { get; set; }
        /// <summary>
        /// 出生地名称
        /// </summary>  
        public string BirthPlaceName { get; set; }
        /// <summary>
        /// 国籍
        /// </summary>  
        public string Citizenship { get; set; }
        /// <summary>
        /// 民族
        /// </summary>  
        public string Nation { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>  
        public string IdNo { get; set; }
        /// <summary>
        /// 身份
        /// </summary>  
        public string Identity { get; set; }
        /// <summary>
        /// 费别
        /// </summary>  
        public string ChargeType { get; set; }
        /// <summary>
        /// 合同单位
        /// </summary>  
        public string UnitInContract { get; set; }
        /// <summary>
        /// 合同单位名称
        /// </summary>  
        public string UnitInContractName { get; set; }
        /// <summary>
        /// 通信地址
        /// </summary>  
        public string MailingAddress { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>  
        public string ZipCode { get; set; }
        /// <summary>
        /// 联系人电话号码
        /// </summary>  
        public string PhoneNumberHome { get; set; }
        /// <summary>
        /// 家庭电话号码
        /// </summary>  
        public string PhoneNumberBusiness { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>  
        public string NextOfKin { get; set; }
        /// <summary>
        /// 社会关系代码
        /// </summary>  
        public string Relationship { get; set; }
        /// <summary>
        /// 社会关系名称
        /// </summary>  
        public string RelationshipName { get; set; }
        /// <summary>
        /// 联系人地址
        /// </summary>  
        public string NextOfKinAddr { get; set; }
        /// <summary>
        /// 联系人邮政编码
        /// </summary>  
        public string NextOfKinZipCode { get; set; }
        /// <summary>
        /// 联系人电话号码
        /// </summary>  
        public string NextOfKinPhone { get; set; }
        /// <summary>
        /// 上次就诊日期
        /// </summary>  
        public DateTime? LastVisitDate { get; set; }
        /// <summary>
        /// 重要人物标志
        /// </summary>  
        public int VipIndicator { get; set; }
        /// <summary>
        /// 建卡日期
        /// </summary>  
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>  
        public string Operator { get; set; }
        /// <summary>
        /// 医疗体系病人标志
        /// </summary>  
        public string ServiceAgency { get; set; }
        /// <summary>
        /// 单位邮编
        /// </summary>  
        public string BusinessZipCode { get; set; }
        /// <summary>
        /// 照片
        /// </summary>  
        public byte[] Photo { get; set; }
        /// <summary>
        /// 入院来源代码
        /// </summary>  
        public string PatientClass { get; set; }
        /// <summary>
        /// 入院来源名称
        /// </summary>  
        public string PatientClassName { get; set; }
        /// <summary>
        /// 学历
        /// </summary>  
        public string Degree { get; set; }
        /// <summary>
        /// 种族
        /// </summary>  
        public string Race { get; set; }
        /// <summary>
        /// 宗教
        /// </summary>  
        public string Religion { get; set; }
        /// <summary>
        /// 母语
        /// </summary>  
        public string MotherLanguage { get; set; }
        /// <summary>
        /// 第二外语
        /// </summary>  
        public string ForeignLanguage { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>  
        public string IdType { get; set; }
        /// <summary>
        /// 会员号
        /// </summary>  
        public string VipNo { get; set; }
        /// <summary>
        /// 英文名字
        /// </summary>  
        public string EName { get; set; }
        /// <summary>
        /// 
        /// </summary>  
        public string Occupation { get; set; }
        /// <summary>
        /// 
        /// </summary>  
        public string OccupationCode { get; set; }
        /// <summary>
        /// NEXT_OF_SEX
        /// </summary>  
        public string NextOfSex { get; set; }
        /// <summary>
        /// NEXT_OF_BATH
        /// </summary>  
        public DateTime? NextOfBath { get; set; }
        /// <summary>
        /// NEXT_OF_ID
        /// </summary>  
        public string NextOfId { get; set; }
        /// <summary>
        /// 籍贯（区划代码）
        /// </summary>  
        public string NativePlace { get; set; }
        /// <summary>
        /// 户口地址行政区划
        /// </summary>  
        public string MailingAddressCode { get; set; }
        /// <summary>
        /// 健康卡号
        /// </summary>  
        public string HealthyCardNo { get; set; }
        /// <summary>
        /// 户口地址乡镇/街道代码
        /// </summary>  
        public string MailingStreetCode { get; set; }
        /// <summary>
        /// MARITAL_STATUS
        /// </summary>  
        public string MaritalStatus { get; set; }
        /// <summary>
        /// 出生地址
        /// </summary>  
        public string BirthCertno { get; set; }
        /// <summary>
        /// JHR_IDNO
        /// </summary>  
        public string JhrIdno { get; set; }
        /// <summary>
        /// EHC_CARD
        /// </summary>  
        public string EhcCard { get; set; }
        /// <summary>
        /// CURRENT_ADDRESS_ZONE
        /// </summary>  
        public string CurrentAddressZone { get; set; }
        /// <summary>
        /// JHR_JK_PHONE
        /// </summary>  
        public string JhrJkPhone { get; set; }
        /// <summary>
        /// CURRENT_ADDRESS_COUNTY
        /// </summary>  
        public string CurrentAddressCounty { get; set; }
        /// <summary>
        /// CURRENT_ADDRESS_CITY
        /// </summary>  
        public string CurrentAddressCity { get; set; }
        /// <summary>
        /// CURRENT_ADDRESS_PROVINCE
        /// </summary>  
        public string CurrentAddressProvince { get; set; }
        /// <summary>
        /// 幼托学校地址
        /// </summary>  
        public string StudyPlace { get; set; }
        /// <summary>
        /// KID_ID
        /// </summary>  
        public string KidId { get; set; }
        /// <summary>
        /// PERMANENT_ADDRESS_ZONE
        /// </summary>  
        public string PermanentAddressZone { get; set; }
        /// <summary>
        /// PERMANENT_ADDRESS_COUNTY
        /// </summary>  
        public string PermanentAddressCounty { get; set; }
        /// <summary>
        /// 提案号
        /// </summary>
        public string MrProvidedIndicator { get; set; }
        /// <summary>
        /// PERMANENT_ADDRESS_CITY
        /// </summary>  
        public string PermanentAddressCity { get; set; }
        /// <summary>
        /// PERMANENT_ADDRESS_PROVINCE
        /// </summary>  
        public string PermanentAddressProvince { get; set; }
        /// <summary>
        /// 籍贯-市
        /// </summary>  
        public string PlaceOfOriginCity { get; set; }
        /// <summary>
        /// 籍贯-省
        /// </summary>  
        public string PlaceOfOriginProvince { get; set; }
        /// <summary>
        /// 出生地-市
        /// </summary>  
        public string PlaceOfBirthCity { get; set; }
        /// <summary>
        /// 出生地-县
        /// </summary>  
        public string PlaceOfBirthCounty { get; set; }
        /// <summary>
        /// 出生地-省
        /// </summary>  
        public string PlaceOfBirthProvince { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>  
        public string FatherBirthPlace { get; set; }
        /// <summary>
        /// INFECT_TYPE
        /// </summary>  
        public string InfectType { get; set; }
        /// <summary>
        /// 工作单位
        /// </summary>
        public string WorkUnit { get; set; }
        /// <summary>
        /// 入院方式
        /// </summary>  
        public string PatientClassWay { get; set; }
        /// <summary>
        /// 入院方式名称
        /// </summary>  
        public string PatientClassWayName { get; set; }
        /// <summary>
        /// 
        /// </summary>  
        public string CountryName { get; set; }
        /// <summary>
        /// 来源目的
        /// </summary>  
        public string AdmissionCause { get; set; }
        /// <summary>
        /// 入院科室代码
        /// </summary>
        public string InpDeptCode { get; set; }
        /// <summary>
        /// 入院科室名称
        /// </summary>
        public string InpDeptName { get; set; }
        /// <summary>
        /// 接诊医生名称
        /// </summary>
        public string ConsultingDoctor { get; set; }
        /// <summary>
        /// 接诊医生工号
        /// </summary>
        public string ConsultingDoctorCode { get; set; }
        /// <summary>
        /// 门诊诊断名称
        /// </summary>
        public string DiagnosisDesc { get; set; }
        /// <summary>
        /// 门诊诊断代码
        /// </summary>
        public string DiagnosisCode { get; set; }
        /// <summary>
        /// 医保号
        /// </summary>
        public string InsuranceNo { get; set; }
        /// <summary>
        /// 医保类别
        /// </summary>
        public string InsuranceType { get; set; }
        /// <summary>
        /// 登记时间
        /// </summary>
        public string AdmitDate { get; set; }
    }
}

