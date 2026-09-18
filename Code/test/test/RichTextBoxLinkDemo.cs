using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace test
{
    public partial class RichTextBoxLinkDemo : Form
    {
        /// <summary>
        /// 自定义链接信息模型
        /// </summary>
        private class RtfLink
        {
            public int StartIndex { get; set; }
            public int Length { get; set; }
            public string Tag { get; set; }
            public string Text { get; set; }
        }

        private readonly List<RtfLink> _linkList = new List<RtfLink>();

        public RichTextBoxLinkDemo()
        {
            InitializeComponent();
            InitRichTextContent();
        }

        private void InitRichTextContent()
        {
            richTextBox1.ReadOnly = true;
            richTextBox1.DetectUrls = false;
            richTextBox1.Cursor = Cursors.Default;

            string content =
                "1. 打印《兴化市人民医院殡葬服务告知书》与《患者死亡及遗体转运事项告知确认书》\r\n" +
                "（要求：“两书”遗体离院前必备，其中确认书随死亡证明第一联流转，科室与患者家属不得留存；告知书由医生现场交付家属；无家属到场，遗体由科室通知殡仪馆转运，“两书”同死亡证流转至门诊一站式服务中心；非正常死亡或不能推定疾病引起死亡时应当遗体离院前报警）\r\n\r\n" +
                "2. 打印《居民死亡医学证明办理委托书》（要求：必要时再提供打印，在近亲属无法到场申领死亡证时使用，“近亲属”定义可见于上述“两书”）\r\n\r\n" +
                "3. 点击进入死亡上报平台（要求：正常死亡在24小时内应当进行死亡上报，医生打印签字并盖工号章后，科室工作人员送门诊一站式服务中心；无证件卡的逝者姓名和地址信息必须详细）\r\n\r\n" +
                "（温馨提示：请点击以上超链接进行相关操作，死亡上报等业务问题请联系公共卫生科死因监测专员67716，无法打印等软硬件问题请联系信息部；若还需要再次进入本界面，请点击工作站“死亡上报”再次进入。）";

            richTextBox1.Text = content;
            _linkList.Clear();

            // 注册3处超链接
            AddLink("《兴化市人民医院殡葬服务告知书》与《患者死亡及遗体转运事项告知确认书》", "PrintTwoDoc");
            AddLink("《居民死亡医学证明办理委托书》", "PrintDelegateDoc");
            AddLink("死亡上报平台", "OpenDeathReport");
        }

        /// <summary>
        /// 添加超链接文本，蓝色+下划线样式
        /// </summary>
        private void AddLink(string linkText, string tag)
        {
            int start = richTextBox1.Text.IndexOf(linkText);
            if (start < 0) return;

            _linkList.Add(new RtfLink
            {
                StartIndex = start,
                Length = linkText.Length,
                Tag = tag,
                Text = linkText
            });

            richTextBox1.SelectionStart = start;
            richTextBox1.SelectionLength = linkText.Length;
            richTextBox1.SelectionColor = Color.FromArgb(0, 51, 153);
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, FontStyle.Underline);
            richTextBox1.SelectionLength = 0;
        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            int charIndex = richTextBox1.GetCharIndexFromPosition(e.Location);

            foreach (var link in _linkList)
            {
                if (charIndex >= link.StartIndex && charIndex < link.StartIndex + link.Length)
                {
                    ExecuteLink(link.Tag);
                    break;
                }
            }
        }

        private void richTextBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int charIndex = richTextBox1.GetCharIndexFromPosition(e.Location);
            bool hover = false;

            foreach (var link in _linkList)
            {
                if (charIndex >= link.StartIndex && charIndex < link.StartIndex + link.Length)
                {
                    hover = true;
                    break;
                }
            }

            richTextBox1.Cursor = hover ? Cursors.Hand : Cursors.Default;
        }

        /// <summary>
        /// 链接业务入口，在这里对接你项目打印、打开窗口逻辑
        /// </summary>
        private void ExecuteLink(string tag)
        {
            switch (tag)
            {
                case "PrintTwoDoc":
                    MessageBox.Show("执行：打印殡葬服务告知书 + 遗体转运确认书");
                    //TODO: 替换为真实打印代码
                    break;
                case "PrintDelegateDoc":
                    MessageBox.Show("执行：打印居民死亡医学证明办理委托书");
                    //TODO: 替换为真实打印代码
                    break;
                case "OpenDeathReport":
                    MessageBox.Show("执行：打开死亡上报平台界面");
                    //TODO: 替换为跳转死亡上报窗口代码
                    break;
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}