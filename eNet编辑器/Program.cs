using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using eNet编辑器.AddForm;
using eNet编辑器.Controller;

namespace eNet编辑器
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                //Application.Run(new SetData());
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.ToString());
            }
           
        }
    }
}
