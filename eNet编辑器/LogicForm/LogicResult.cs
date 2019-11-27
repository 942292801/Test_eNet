using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace eNet编辑器.LogicForm
{
    public partial class LogicResult : Form
    {
        private string result;

        public string Result
        {
            get { return result; }
            set { result = value; }
        }

        private string returnResult;

        public string ReturnResult
        {
            get { return returnResult; }
            set { returnResult = value; }
        }

        public LogicResult()
        {
            InitializeComponent();
        }

        private void LogicResult_Load(object sender, EventArgs e)
        {
            ControlIni();
            if (!string.IsNullOrEmpty(result))
            {
                string opt = Validator.GetStr(result);
                string[] num = Regex.Split(result,opt);
                cbOperation.Text = opt;
                if (num.Length == 1)
                {
                    
                    txtRight.Text = num[0];
                }
                else if (num.Length == 2)
                {
                    
                    txtLeft.Text = num[0];
                    txtRight.Text = num[1];
                }
            }
        }

        /// <summary>
        /// 控件初始化
        /// </summary>
        private void ControlIni()
        {
            cbOperation.Items.Add("=");
            cbOperation.Items.Add(">");
            cbOperation.Items.Add("<");
            cbOperation.Items.Add("!=");
            cbOperation.Items.Add(">=");
            cbOperation.Items.Add("<=");
            cbOperation.Items.Add("-");
            cbOperation.SelectedIndex = 0;

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
        private Point mPoint;

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }


        private void LogicResult_Paint(object sender, PaintEventArgs e)
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

        
        #endregion


        private void cbOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOperation.Text == "-")
            {
                txtLeft.Enabled = true;
            }
            else
            {
                txtLeft.Text = "";
                txtLeft.Enabled = false;
            }
        }

        private void txtLeft_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8 && e.KeyChar != (char)3 && e.KeyChar != (char)22)
            {
                e.Handled = true;
            } 
        }

        private void txtRight_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8 && e.KeyChar != (char)3 && e.KeyChar != (char)22)
            {
                e.Handled = true;
            } 
        }

        private void btnDecid_Click(object sender, EventArgs e)
        {
            returnResult = "";
            if (cbOperation.Text == "-")
            {
                if (string.IsNullOrEmpty(txtLeft.Text) || string.IsNullOrEmpty(txtRight.Text))
                {
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    returnResult = string.Format("{0}{1}{2}", txtLeft.Text, cbOperation.Text, txtRight.Text);
                    this.DialogResult = DialogResult.OK;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtRight.Text))
                {
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    returnResult = string.Format("{0}{1}",cbOperation.Text, txtRight.Text);
                    this.DialogResult = DialogResult.OK;
                }
            }
        }


 


    }
}
