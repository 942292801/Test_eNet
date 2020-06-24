using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace eNet编辑器.AddForm
{
    public delegate void AddDev();
    //public delegate void ChangeDev();
    public partial class tnDevice : Form
    {
        public tnDevice()
        {
            InitializeComponent();
        }
        public static event Action<string> AppTxtShow;
        public bool isNew = false;
        //回调添加设备节点
        public event AddDev adddev;

        private string oldDevNum = "";
        private string oldDevVersion = "";

        private bool isChange = false;

        public bool IsChange
        {
            get { return isChange; }
            set { isChange = value; }
        }

        private string title = "";

        public string Title
        {
            get { return title; }
            set { title = value; }
        }



        private void tnDevice_Load(object sender, EventArgs e)
        {
            lbTitle.Text = title;
            //btnDecid.Tooltip = title;
            string[] infos = FileMesege.info.Split(' ');
            if (infos.Length > 2)
            {
                //修改模式
                cbDevice.Text = Regex.Replace(infos[2], @"[^\d]*", "");
                oldDevNum = cbDevice.Text;
                cbVersion.Text = infos[3];
                oldDevVersion = infos[3];
            }
            //网关名称
            lbip.Text = infos[0];
            //获取设备名称
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//devices");
            string display = null;
            //循环添加设备号
            for (int i = 0; i < 64; i++)
            {
                cbDevice.Items.Add(i);
            }
            //循环添加设备名字
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                display = IniConfig.GetValue(file.FullName, "define", "display");
                cbVersion.Items.Add(display);

            }



        }

   


        private void btnDecid_Click_1(object sender, EventArgs e)
        {
            //设备号不为空
            if (string.IsNullOrEmpty(cbDevice.Text) || string.IsNullOrEmpty(cbVersion.Text))
            {
                AppTxtShow("设备信息错误");
                return;
            }
            //设备ID号为数字
            if (!Regex.IsMatch(cbDevice.Text, @"^[+-]?\d*[.]?\d*$"))
            {

                AppTxtShow("设备号输入格式错误");
                return;
            }
            bool isVersion = false;
            //确保修改的型号存在
            foreach (string version in cbVersion.Items)
            {
                if (version == cbVersion.Text)
                {
                    isVersion = true;
                }
            }
            if (!isVersion)
            {
                AppTxtShow("设备型号不存在");
                return;
            }

            if (isNew)
            {
                foreach (DataJson.Device dev in FileMesege.DeviceList)
                {
                    if (dev.ip == lbip.Text)
                    {
                        foreach (DataJson.Module m in dev.module)
                        {
                            if (m.id.ToString() == cbDevice.Text)
                            {
                                AppTxtShow("操作失败！请检查设备号！");
                                return;
                            }
                        }
                    }
                }
                FileMesege.info = string.Format("{0} {1} {2}", lbip.Text.Replace(" ", ""), cbDevice.Text.Replace(" ", ""), cbVersion.Text.Replace(" ", ""));
                try
                {
                    //自动序号加一
                    cbDevice.SelectedIndex = Convert.ToInt32(cbDevice.Text) + 1;
                }
                catch
                {

                }
                //添加设备回调
                adddev();

            }
            else
            {
                //设备号改变 
                if (cbDevice.Text != oldDevNum)
                {
                    isChange = true;
                }
                if (cbVersion.Text != oldDevVersion)
                {
                    //改设备版本
                    isChange = false;
                }
                FileMesege.info = string.Format("{0} {1} {2} {3} {4}", lbip.Text.Replace(" ", ""), cbDevice.Text.Replace(" ", ""), cbVersion.Text.Replace(" ", ""), oldDevNum, oldDevVersion);
                //修改模式
                this.DialogResult = DialogResult.OK;
            }
        }
        #region 窗体样色


        #region 窗体样色2
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
         );

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:                        // box shadow
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

        }
        #endregion
        private void tnDevice_Paint(object sender, PaintEventArgs e)
        {
            Rectangle myRectangle = new Rectangle(0, 0, this.Width, this.Height);
            //ControlPaint.DrawBorder(e.Graphics, myRectangle, Color.Blue, ButtonBorderStyle.Solid);//画个边框 
            ControlPaint.DrawBorder(e.Graphics, myRectangle,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 2, ButtonBorderStyle.Solid,
                Color.DarkGray, 2, ButtonBorderStyle.Solid
            );
        }

        private Point mPoint;


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void plInfoTitle_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint = new Point(e.X, e.Y);
        }

        private void plInfoTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        #endregion
    }
}
