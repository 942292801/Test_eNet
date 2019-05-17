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
    public partial class timerHHMM : Form
    {
        private string date;

        /// <summary>
        /// 记录当前控件选中状态
        /// </summary>
        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public timerHHMM()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 翻回窗口回调
        /// </summary>
        public event Action<string> AddShortTime;

        private void timerHHMM_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(date) || date == "日出时间" || date == "日落时间")
            {
                return;
            }
            
            string hour = date.Split(':')[0];
            string min = date.Split(':')[1];
            int tmpHour = 0;
            int tmpMin = 0;
            if (hour.Contains("*"))
            {
                cbHour.Checked = true;
            }
            else
            {
                tmpHour = Convert.ToInt32( hour);
            }
            if (min.Contains("*"))
            {
                cbMin.Checked = true;
            }
            else
            {
                tmpMin = Convert.ToInt32(min);
            }
            TimeSpan ts = new TimeSpan(tmpHour,tmpMin,0);
            tmSelect.SelectedTime = ts;
        }


        private void timerHHMM_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timerHHMM_Paint(object sender, PaintEventArgs e)
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

        private void timeSelector1_OkClick(object sender, EventArgs e)
        {
            string hour = tmSelect.SelectedDateTime.Hour.ToString();
            string min = tmSelect.SelectedDateTime.Minute.ToString();
            if (cbHour.Checked)
            {
                hour = "**";
            }
            if (cbMin.Checked)
            {
                min = "**";
            }
            while (hour.Length < 2)
            {
                hour = "0" + hour;
            }
            while (min.Length < 2)
            {
                min = "0" + min;
            }
            AddShortTime( string.Format("{0}:{1}", hour, min));

        }

        private void btnSunUP_Click(object sender, EventArgs e)
        {
            AddShortTime("日出时间");
        }

        private void btnSunOut_Click(object sender, EventArgs e)
        {
            AddShortTime("日落时间");
        }

        private void tmSelect_SelectedTimeChanged(object sender, EventArgs e)
        {
            timeSelector1_OkClick(this, EventArgs.Empty);

        }

        //每小时
        private void cbHour_MouseUp(object sender, MouseEventArgs e)
        {
            timeSelector1_OkClick(this, EventArgs.Empty);
        }

        //每分钟 
        private void cbMin_MouseUp(object sender, MouseEventArgs e)
        {
            timeSelector1_OkClick(this, EventArgs.Empty);
        }

     

        
    }
}
