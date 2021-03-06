﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace eNet编辑器.AddForm
{
    public delegate void AddNode();
    public partial class tsSection : Form
    {
        public tsSection()
        {
            InitializeComponent();
        }

        //记录上次关闭的选中点
        private int selectindex = 0;

        public int Selectindex
        {
            get { return selectindex; }
            set { selectindex = value; }
        }
        /// <summary>
        /// 新建第一个节点标志
        /// </summary>
        private bool newflag = false;

        public bool Newflag
        {
            get { return newflag; }
            set { newflag = value; }
        }

        private string lbText = "";

        public string LbText
        {
            get { return lbText; }
            set { lbText = value; }
        }


        //回调添加节点
        public event AddNode addNode;
        private void tsSection_Load(object sender, EventArgs e)
        {
            lbTitle.Text = lbText;
            addtype(cbtype);
            cbtype.SelectedIndex = selectindex;
           
        }

        private void btnDecid_Click(object sender, EventArgs e)
        {
            if (cbname.Text == "" || cbname.Text == null)
            {
                MessageBox.Show("新建节点名称不能为空");
                return;
            }
            FileMesege.info = cbname.Text;
            try
            {
                cbname.SelectedIndex = cbname.SelectedIndex + 1;
            }
            catch { 
            
            }
            //利用回调机制 
            addNode();
        }

      

        private void cbtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            addname(cbname, cbtype.Text);
        }

        /// <summary>
        /// 获取Ini文件所有key值
        /// </summary>
        /// <param name="cbtype"></param>
        private void addtype(ComboBox cbtype) 
        {
            List<string> keys = IniConfig.ReadKeys("sectionNode", String.Format("{0}{1}", Application.StartupPath, "\\names\\commonName.ini"));
            cbtype.Items.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                cbtype.Items.Add(keys[i]);
            }
            cbtype.SelectedIndex = 0;
        }

        /// <summary>
        /// 获取key值类型下的所有名称
        /// </summary>
        /// <param name="cbname"></param>
        /// <param name="type"></param>
        private void addname(ComboBox cbname,string type)
        {
            string[] node = IniConfig.GetValue(String.Format("{0}{1}", Application.StartupPath, "\\names\\commonName.ini"), "sectionNode", type).Split(',');
            cbname.Items.Clear();
            cbname.Text = "";
            for (int i = 0; i < node.Length; i++)
            {
                cbname.Items.Add(node[i]);
            }
            cbname.SelectedIndex = 0;
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
        private void tsSection_Paint(object sender, PaintEventArgs e)
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
