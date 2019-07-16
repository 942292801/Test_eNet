﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eNet编辑器.AddForm
{
    public partial class sensorAdd : Form
    {
        public sensorAdd()
        {
            InitializeComponent();
        }

        public event Action addSensorNode;

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

        private string panelName = "";

        public string PanelName
        {
            get { return panelName; }
            set { panelName = value; }
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

        private void sensorAdd_Load(object sender, EventArgs e)
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
            txtName.Items.Clear();
            int j = FileMesege.AreaList.Count;
            if (j > 0)
            {
                for (int i = 0; i < j; i++)
                {
                    cbs1.Items.Add(FileMesege.AreaList[i].area);
                }
                cbs1.Enabled = true;
            }
            string filepath = Application.StartupPath + "\\names\\commonName.ini";
            //名称 加入新的元素
            string strs = IniConfig.GetValue(filepath, "treeNode", "sensor");
            if (!String.IsNullOrWhiteSpace(strs))
            {
                string[] strarr = strs.Split(',');
                for (int i = 0; i < strarr.Length; i++)
                {
                    txtName.Items.Add(strarr[i]);
                }
            }
            txtName.SelectedIndex = 0;
            if (xflag == false)
            {
                try
                {
                    if (FileMesege.sectionNodeCopy != null)
                    {
                        string[] sections = FileMesege.sectionNodeCopy.Split('\\');
                        if (sections[0] != "全部")
                        {
                            //新建
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
                btnDecid.Text = "修改";
                //修改
                foreach (DataJson.Sensor sr in FileMesege.sensorList)
                {
                    if (ip == sr.IP)
                    {
                        foreach (DataJson.sensors srs in sr.sensors)
                        {
                            if (num == srs.id.ToString())
                            {
                                DataJson.PointInfo point = DataListHelper.findPointByPid(srs.pid, FileMesege.PointList.link);
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
                                    this.Oldnum = srs.id.ToString();
                                }

                                break;
                            }
                        }

                        break;
                    }
                }


            }
        }

        #region 重绘窗体
        private void sensorAdd_Paint(object sender, PaintEventArgs e)
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
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }
        #endregion


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

        private void btnDecid_Click(object sender, EventArgs e)
        {
            if (txtNum.Text == "")
            {
                MessageBox.Show("感应编组号不能为空");
                return;
            }
            try
            {
                if (Convert.ToInt32(txtNum.Text) > 1999 || Convert.ToInt32(txtNum.Text) <1001)
                {
                    MessageBox.Show("感应编组号范围为1001-1999");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("感应编组号格式错误");
                return;
            }

            if (xflag == false)
            {
                //新建
                if (FileMesege.sensorList != null)
                {
                    //检测是否存在该感应编组号
                    for (int i = 0; i < FileMesege.sensorList.Count; i++)
                    {
                        if (ip == FileMesege.sensorList[i].IP)
                        {
                            foreach (DataJson.sensors srs in FileMesege.sensorList[i].sensors)
                            {
                                if (srs.id.ToString() == txtNum.Text)
                                {
                                    MessageBox.Show("已存在该感应编组号！The Sensor Number already exist", "提示");
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
                //检测是否存在该感应编组号
                for (int i = 0; i < FileMesege.sensorList.Count; i++)
                {
                    if (ip == FileMesege.sensorList[i].IP)
                    {
                        foreach (DataJson.sensors srs in FileMesege.sensorList[i].sensors)
                        {
                            if (srs.id.ToString() == txtNum.Text && this.Num != txtNum.Text)
                            {
                                MessageBox.Show("已存在该感应编组号！The Sensor Number already exist", "提示");
                                return;
                            }

                        }
                    }
                }
            }
            this.Num = txtNum.Text;
            this.PanelName = string.Format("{0}@{1}", txtName.Text, Ip.Split('.')[3]);
            this.Area1 = cbs1.Text;
            this.Area2 = cbs2.Text;
            this.Area3 = cbs3.Text;
            this.Area4 = cbs4.Text;
            //是添加感应编组 还是修改
            if (!xflag)
            {
                //新建感应编组
                foreach (DataJson.PointInfo pi in FileMesege.PointList.link)
                {
                    if (pi.area1 == Area1 && pi.area2 == Area2 && pi.area3 == Area3 && pi.area4 == Area4 && pi.name == PanelName)
                    {
                        MessageBox.Show("该名称已存在，请更换名称");
                        return;
                    }

                }
                addSensorNode();
                txtNum.Text = (Convert.ToInt32(txtNum.Text) + 1).ToString();
            }
            else
            {
                if (oldnum != num)
                {
                    //修改了编号
                    if (area1 == OldArea1 && area2 == OldArea2 && area3 == OldArea3 && area4 == OldArea4 && PanelName == OldName)
                    {
                        this.DialogResult = DialogResult.No;
                        return;
                    }

                }
                else
                {
                    if (area1 == OldArea1 && area2 == OldArea2 && area3 == OldArea3 && area4 == OldArea4 && PanelName == OldName)
                    {
                        this.DialogResult = DialogResult.No;
                        return;
                    }
                }
               

                //修改区域  感应编组号没更改
                foreach (DataJson.PointInfo pi in FileMesege.PointList.link)
                {
                    //存在相同的区域名称
                    if (pi.area1 == Area1 && pi.area2 == Area2 && pi.area3 == Area3 && pi.area4 == Area4 && pi.name == PanelName)
                    {
                        MessageBox.Show("该名称已存在，请更换名称");
                        return;
                    }
                }


                //修改感应编组
                this.DialogResult = DialogResult.OK;
            }
        }


    }
}
