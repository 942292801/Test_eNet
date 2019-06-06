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
    public partial class timerAdd : Form
    {

        public event Action addTimerNode;
        public timerAdd()
        {
            InitializeComponent();
        }

        //是否新建
        bool xflag = false;

        public bool Xflag
        {
            get { return xflag; }
            set { xflag = value; }
        }
        private string ip = "";

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }
        private string num = "";

        public string Num
        {
            get { return num; }
            set { num = value; }
        }

        private string oldnum;

        public string Oldnum
        {
            get { return oldnum; }
            set { oldnum = value; }
        }

        private string oldArea1;

        public string OldArea1
        {
            get { return oldArea1; }
            set { oldArea1 = value; }
        }
        private string oldArea2;

        public string OldArea2
        {
            get { return oldArea2; }
            set { oldArea2 = value; }
        }
        private string oldArea3;

        public string OldArea3
        {
            get { return oldArea3; }
            set { oldArea3 = value; }
        }
        private string oldArea4;

        public string OldArea4
        {
            get { return oldArea4; }
            set { oldArea4 = value; }
        }

        private string oldName;

        public string OldName
        {
            get { return oldName; }
            set { oldName = value; }
        }

        private string timerName = "";

        public string TimerName
        {
            get { return timerName; }
            set { timerName = value; }
        }

        private string area1;
        public string Area1
        {
            get { return area1; }
            set { area1 = value; }
        }

        private string area2;
        public string Area2
        {
            get { return area2; }
            set { area2 = value; }
        }

        private string area3;
        public string Area3
        {
            get { return area3; }
            set { area3 = value; }
        }

        private string area4;
        public string Area4
        {
            get { return area4; }
            set { area4 = value; }
        }

        private bool isHoliday;

        public bool IsHoliday
        {
            get { return isHoliday; }
            set { isHoliday = value; }
        }

        private void timerAdd_Load(object sender, EventArgs e)
        {

            txtGateway.Text = Ip;
            txtNum.Text = num;
            cbs1.Items.Clear();
            cbs2.Items.Clear();
            cbs3.Items.Clear();
            cbs4.Items.Clear();
            cbs1.Text = "";
            cbs2.Text = "";
            cbs3.Text = "";
            cbs4.Text = "";
            
            int j = FileMesege.AreaList.Count;
            if (j > 0)
            {
                for (int i = 0; i < j; i++)
                {
                    cbs1.Items.Add(FileMesege.AreaList[i].area);
                }
                cbs1.Enabled = true;
            }

            txtName.Items.Clear();
            if (isHoliday)
            {
                //节假日时钟
                txtNum.Enabled = false;
                lbTitle.Text = "添加节假日定时";
                txtName.Enabled = false;
                txtName.Text = "节假日定时";
                cbs1.Text = "";
                cbs2.Text = "";
                cbs3.Text = "";
                cbs4.Text = "";
                cbs1.Enabled = false;
                cbs2.Enabled = false;
                cbs3.Enabled = false;
                cbs4.Enabled = false;
         
            }
            else
            {
                txtName.Enabled = true;
                txtNum.Enabled = true;
                lbTitle.Text = "添加定时";
                string filepath = Application.StartupPath + "\\names\\commonName.ini";
                //名称 加入新的元素
                string strs = IniConfig.GetValue(filepath, "treeNode", "timer");
                if (!String.IsNullOrWhiteSpace(strs))
                {
                    string[] strarr = strs.Split(',');
                    for (int i = 0; i < strarr.Length; i++)
                    {
                        txtName.Items.Add(strarr[i]);
                    }
                }
                txtName.SelectedIndex = 0;
            }
            if (!xflag)
            {
                //新建
                try
                {
                    if (FileMesege.sectionNodeCopy != null)
                    {
                        string[] sections = FileMesege.sectionNodeCopy.Split('\\');
                        if (sections[0] != "全部" && !isHoliday)
                        {
                            
                            ComboBox[] cbs = { cbs1, cbs2, cbs3, cbs4 };
                            cbs[0].Text = sections[0];
                            cbs[1].Text = sections[1];
                            cbs[2].Text = sections[2];
                            cbs[3].Text = sections[3];
                        }

                    }
                }
                catch
                {

                }

            }
            else
            {
                
                //修改
                foreach (DataJson.Timer timer in FileMesege.timerList)
                {
                    if (ip == timer.IP)
                    {
                        foreach (DataJson.timers tms in timer.timers)
                        {
                            if (num == tms.id.ToString())
                            {
                                DataJson.PointInfo point = DataListHelper.findPointByPid(tms.pid, FileMesege.PointList.timer);
                                if (point != null)
                                {
                                    txtName.Text = point.name.Split('@')[0];
                                    ComboBox[] cbs = { cbs1, cbs2, cbs3, cbs4 };
                                    cbs[0].Text = point.area1;
                                    cbs[1].Text = point.area2;
                                    cbs[2].Text = point.area3;
                                    cbs[3].Text = point.area4;
                                    this.oldArea1 = point.area1;
                                    this.oldArea2 = point.area2;
                                    this.oldArea3 = point.area3;
                                    this.oldArea4 = point.area4;
                                    this.oldName = point.name;
                                    this.Oldnum = tms.id.ToString();
                                }

                                break;
                            }
                        }

                        break;
                    }
                }


            }




        }

        //确认添加按钮
        private void btnDecid_Click(object sender, EventArgs e)
        {

            if (txtNum.Text == "")
            {
                MessageBox.Show("定时号不能为空");
                return;
            }
            try
            {
                if (Convert.ToInt32(txtNum.Text) > 65535)
                {
                    MessageBox.Show("定时号不能大于65535");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("定时号格式错误");
                return;
            }

            if (!xflag)
            {
                //新建
                if (FileMesege.timerList != null)
                {
                    //检测是否存在该定时号
                    for (int i = 0; i < FileMesege.timerList.Count; i++)
                    {
                        if (ip == FileMesege.timerList[i].IP)
                        {
                            foreach (DataJson.timers tms in FileMesege.timerList[i].timers)
                            {
                                if (tms.id.ToString() == txtNum.Text)
                                {
                                    MessageBox.Show("已存在该定时号！The Timer Number already exist", "提示");
                                    return;
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                //修改
                //检测是否存在该定时号
                for (int i = 0; i < FileMesege.timerList.Count; i++)
                {
                    if (ip == FileMesege.timerList[i].IP)
                    {
                        foreach (DataJson.timers tms in FileMesege.timerList[i].timers)
                        {
                            if (tms.id.ToString() == txtNum.Text && this.Num != txtNum.Text)
                            {
                                MessageBox.Show("已存在该定时号！The Scene Number already exist", "提示");
                                return;
                            }

                        }
                    }
                }
            }
          
            this.Num = txtNum.Text;
            this.TimerName = string.Format("{0}@{1}", txtName.Text, Ip.Split('.')[3]);
            this.Area1 = cbs1.Text;
            this.Area2 = cbs2.Text;
            this.Area3 = cbs3.Text;
            this.Area4 = cbs4.Text;
            //是添加定时 还是修改定时
            if (!xflag)
            {
                //新建定时
                foreach (DataJson.PointInfo pi in FileMesege.PointList.timer)
                {
                    if (pi.area1 == Area1 && pi.area2 == Area2 && pi.area3 == Area3 && pi.area4 == Area4 && pi.name == TimerName)
                    {
                        MessageBox.Show("该名称已存在，请更换名称");
                        return;
                    }

                }
                addTimerNode();
                if (IsHoliday)
                {
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                txtNum.Text = (Convert.ToInt32(txtNum.Text) + 1).ToString();
            }
            else
            {
                if (oldnum != num)
                {
                    //修改了编号
                    this.DialogResult = DialogResult.OK;
                    return;

                }
                if (area1 == OldArea1 && area2 == OldArea2 && area3 == OldArea3 && area4 == OldArea4 && TimerName == OldName)
                {
                    this.DialogResult = DialogResult.No;
                    return;
                }

                //修改区域  定时号没更改
                foreach (DataJson.PointInfo pi in FileMesege.PointList.timer)
                {
                    //存在相同的区域名称
                    if (pi.area1 == Area1 && pi.area2 == Area2 && pi.area3 == Area3 && pi.area4 == Area4 && pi.name == TimerName)
                    {
                        MessageBox.Show("该名称已存在，请更换名称");
                        return;
                    }
                }


                //修改定时
                this.DialogResult = DialogResult.OK;
            }


        }





        //当选择框项改变时候 更新数据
        private void cbs1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbs2.Items.Clear();
            cbs2.Enabled = false;
            cbs3.Items.Clear();
            cbs3.Enabled = false;
            cbs4.Items.Clear();
            cbs4.Enabled = false;
            cbs2.Text = "";
            cbs3.Text = "";
            cbs4.Text = "";
            int j = FileMesege.AreaList[cbs1.SelectedIndex].area2.Count;
            if (j > 0)
            {
                for (int i = 0; i < j; i++)
                {
                    cbs2.Items.Add(FileMesege.AreaList[cbs1.SelectedIndex].area2[i].area);
                }
                cbs2.Enabled = true;
            }
        }

        private void cbs2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbs3.Items.Clear();
            cbs3.Enabled = false;
            cbs4.Items.Clear();
            cbs4.Enabled = false;
            cbs3.Text = "";
            cbs4.Text = "";
            int j = FileMesege.AreaList[cbs1.SelectedIndex].area2[cbs2.SelectedIndex].area3.Count;
            if (j > 0)
            {
                for (int i = 0; i < j; i++)
                {
                    cbs3.Items.Add(FileMesege.AreaList[cbs1.SelectedIndex].area2[cbs2.SelectedIndex].area3[i].area);
                }
                cbs3.Enabled = true;
            }
        }

        private void cbs3_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbs4.Items.Clear();
            cbs4.Enabled = false;
            cbs4.Text = "";
            int j = FileMesege.AreaList[cbs1.SelectedIndex].area2[cbs2.SelectedIndex].area3[cbs3.SelectedIndex].area4.Count;
            if (j > 0)
            {
                for (int i = 0; i < j; i++)
                {
                    cbs4.Items.Add(FileMesege.AreaList[cbs1.SelectedIndex].area2[cbs2.SelectedIndex].area3[cbs3.SelectedIndex].area4[i].area);
                }
                cbs4.Enabled = true;
            }
        }

        private void cbs4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void timerAdd_Paint(object sender, PaintEventArgs e)
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



    }
}
