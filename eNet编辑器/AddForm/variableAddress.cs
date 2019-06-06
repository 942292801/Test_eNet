using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eNet编辑器.AddForm
{
    public partial class variableAddress : Form
    {
        public variableAddress()
        {
            InitializeComponent();
        }

        //十进制地址
        private string address = "";

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// 十六进制返回地址
        /// </summary>
        private string rtAddress = "";

        public string RtAddress
        {
            get { return rtAddress; }
            set { rtAddress = value; }
        }

        private void variableAddress_Load(object sender, EventArgs e)
        {
            for(int i =1;i<100;i++)
            {
                cb4.Items.Add(i);
            }
            
            if (string.IsNullOrEmpty(address))
            {
                return;
            }
            cb4.Text = address.Split('.')[3];
        }



        #region 重绘

        private void variableAddress_Paint(object sender, PaintEventArgs e)
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
            this.Close();
        }

        #endregion

        private void btnDecid_Click(object sender, EventArgs e)
        {
            try
            {
                string tmp = string.Format( "FEFB03{0}" , SocketUtil.strtohexstr(cb4.Text));
                if (tmp.Length == 8)
                {
                    this.RtAddress = tmp;
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                this.DialogResult = DialogResult.No;
            }
            catch
            {
                this.DialogResult = DialogResult.No;
            }
        }




    }
}
