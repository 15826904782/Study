namespace test.Common
{
    public class IdBaseDto
    {
        public int Id { get; set; }
    }

    public class UnifyResultDto<T> where T : class, new()
    {
        //
        // 摘要:
        //     状态码
        public string Code { get; set; }

        //
        // 摘要:
        //     信息
        public string Msg { get; set; }

        //
        // 摘要:
        //     数据
        public T Data { get; set; }

        public UnifyResultDto()
        {
        }

        public UnifyResultDto(string code, string msg, T data)
        {
            Code = code;
            Msg = msg;
            Data = data;
        }

        public UnifyResultDto(bool isSuccess, string msg = "")
        {
            Code = (isSuccess ? 200.ToString() : 400.ToString());
            Msg = msg;
        }

        public UnifyResultDto(bool isSuccess, T data, string msg = "")
        {
            Code = (isSuccess ? 200.ToString() : 400.ToString());
            Data = data;
            Msg = msg;
        }
    }

    public class UnifyResultDto
    {
        //
        // 摘要:
        //     状态码（1：成功）
        public string Code { get; set; }

        //
        // 摘要:
        //     信息
        public string Msg { get; set; }

        //
        // 摘要:
        //     数据
        public object Data { get; set; }

        public UnifyResultDto()
        {
        }

        public UnifyResultDto(string code, string msg, object data)
        {
            Code = code;
            Msg = msg;
            Data = data;
        }

        public UnifyResultDto(bool isSuccess, string msg = "")
        {
            Code = (isSuccess ? 200.ToString() : 400.ToString());
            Msg = msg;
        }

        public UnifyResultDto(bool isSuccess, object data, string msg = "")
        {
            Code = (isSuccess ? 200.ToString() : 400.ToString());
            Data = data;
            Msg = msg;
        }
    }
}