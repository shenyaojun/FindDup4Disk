using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1;

namespace FindDup4Disk
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    /// <summary>
    /// 网页调用C#方法
    /// </summary>
    public class ScriptCallbackObject
    {
        public string UserName { get; set; } = "我是C#属性";

        public void ClickMe1()
        {
            FormMain currentForm = (FormMain)Form.ActiveForm;
            // 使用currentForm进行操作

            currentForm.button1_Click(null,null);
        }
        public void ClickMe2()
        {
            FormMain currentForm = (FormMain)Form.ActiveForm;
            // 使用currentForm进行操作

            currentForm.button5_Click(null, null);
        }
        public void ClickMe3()
        {
            FormMain currentForm = (FormMain)Form.ActiveForm;
            // 使用currentForm进行操作

            currentForm.button2_Click(null, null);
        }

        public void ShowMessageArg(string arg)
        {
            MessageBox.Show("【网页调用C#】:" + arg);
        }

        public string GetData(string arg)
        {
            return "【网页调用C#获取数据】;" + arg;
        }

        [System.Runtime.CompilerServices.IndexerName("Items")]
        public string this[int index]
        {
            get { return m_dictionary[index]; }
            set { m_dictionary[index] = value; }
        }
        private Dictionary<int, string> m_dictionary = new Dictionary<int, string>();
    }
}
