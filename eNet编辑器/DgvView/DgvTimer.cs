using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Reflection;
using System.Text.RegularExpressions;
using eNet编辑器.AddForm;
using DevComponents.DotNetBar.Primitives;
using System.IO;

namespace eNet编辑器.DgvView
{
    public partial class DgvTimer : Form
    {
        public DgvTimer()
        {
            //设置窗体双缓存
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            InitializeComponent();
            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.dataGridView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> AppTxtShow;

        

        private void DgvTimer_Load(object sender, EventArgs e)
        {
            //Listbox的hot特效
            listbox.DataItemVisualCreated += new DataItemVisualEventHandler(ListBox3DataItemVisualCreated);
            listbox.ValueMember = "Id"; // Id property will be used as ValueMemeber so SelectedValue will return the Id
            listbox.DisplayMember = "Text"; // Text property will be used as the item text

            //新增对象列 加载
            this.dataGridView1.Rows.Clear();
            DataGridViewComboBoxColumn dgvc = new DataGridViewComboBoxColumn();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                dgvc.Items.Add(IniConfig.GetValue(file.FullName, "define", "name"));
            }
            //设置列名
            dgvc.HeaderText = "类型";
            //设置下拉列表的默认值 
            dgvc.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //或者这样设置 默认选择第一项
            dgvc.DefaultCellStyle.NullValue = dgvc.Items[0];

            dgvc.Name = "type";

            //插入
            this.dataGridView1.Columns.Insert(1, dgvc);
            dataGridView1.Rows.Add();
        }

        void ListBox3DataItemVisualCreated(object sender, DataItemVisualEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)e.Visual;
            item.HotTracking = true;
            
        }

        #region 刷新dgv框相关操作 
        //加载DgV所有信息
        public void TimerAddItem()
        {
            
            
            
                try
                {
                    clear1_7Check();
                    clearVery_customCheck();
                    cbPriorHoliday.Checked = false;
                    listbox.Items.Clear();
                    this.dataGridView1.Rows.Clear();
                    //multipleList.Clear();
                    //查看获取对象是否存在
                    DataJson.timers tms = getTimersInfoList();
                    if (tms == null)
                    {
                        return;
                    }
                    dealtmsDates_prior(tms);
                    List<DataJson.timersInfo> delTimer = new List<DataJson.timersInfo>();
                    string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.timerSelectNode));//16进制
                    //循环加载该场景号的所有信息
                    foreach (DataJson.timersInfo tmInfo in tms.timersInfo)
                    {
                        
                        int dex = dataGridView1.Rows.Add();

                        if (tmInfo.pid == 0)
                        {
                            //pid号为0则为空 按地址来找
                            if (tmInfo.address != "" && tmInfo.address != "FFFFFFFF")
                            {
                                DataJson.PointInfo point = DataListHelper.findPointByType_address(tmInfo.type, ip4 + tmInfo.address.Substring(2, 6));
                                if (point != null)
                                {
                                    tmInfo.pid = point.pid;
                                    tmInfo.address = point.address;
                                    tmInfo.type = point.type;
                                    dataGridView1.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                    dataGridView1.Rows[dex].Cells[4].Value = point.name;
                                }
                            }

                        }
                        else
                        {
                            //pid号有效 需要更新address type
                            DataJson.PointInfo point = DataListHelper.findPointByPid(tmInfo.pid);
                            if (point == null)
                            {
                                //pid号有无效 删除该场景
                                delTimer.Add(tmInfo);
                                dataGridView1.Rows.Remove(dataGridView1.Rows[dex]);
                                continue;
                            }
                            else
                            {
                                //pid号有效
                                tmInfo.address = point.address;
                                //////////////////////////////////////////////////////争议地域
                                //类型不一致 在value寻找
                                if (tmInfo.type != point.type && !string.IsNullOrEmpty(point.value) && !string.IsNullOrEmpty(point.objType))
                                {
                                    //根据value寻找type                        
                                    point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, point.value);
                                }
                                //////////////////////////////////////////////////////到这里
                                if (tmInfo.type != point.type || tmInfo.type == "")
                                {
                                    //当类型为空时候清空操作
                                    tmInfo.opt = "";
                                    tmInfo.optName = "";
                                }
                                tmInfo.type = point.type;
                                dataGridView1.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                dataGridView1.Rows[dex].Cells[4].Value = point.name;
                            }

                        }
                        dataGridView1.Rows[dex].Cells[0].Value = tmInfo.id;
                        
                        dataGridView1.Rows[dex].Cells[1].Value = DgvMesege.addressTransform(tmInfo.address);
                        dataGridView1.Rows[dex].Cells[2].Value = IniHelper.findTypesIniNamebyType(tmInfo.type);
                        dataGridView1.Rows[dex].Cells[5].Value = (tmInfo.optName + " " + tmInfo.opt).Trim();
                        dataGridView1.Rows[dex].Cells[6].Value = tmInfo.hour + ":" + tmInfo.min;
                        dataGridView1.Rows[dex].Cells[7].Value = "删除";


                    }
                    for (int i = 0; i < delTimer.Count; i++)
                    {
                        tms.timersInfo.Remove(delTimer[i]);
                    }

                }
                catch (Exception ex)
                {
                    this.dataGridView1.Rows.Clear();
                    MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
                }

           
           
        }

        /// <summary>
        /// 处理基本日期信息
        /// </summary>
        /// <param name="tms"></param>
        private void dealtmsDates_prior(DataJson.timers tms)
        {
            if (!string.IsNullOrEmpty(tms.dates))
            {
                if (tms.dates.Contains("/"))
                {
                    //为自定义日期
                    cbCustom.Checked = true;
                    string[] dates = tms.dates.Split(',');
                    for (int i = 0; i < dates.Length; i++)
                    {
                        listbox.Items.Add(dates[i]);
                    }
                }
                else
                {
                    //为普通周一到周五
                    string[] dates = tms.dates.Split(',');
                    if (dates.Length == 7)
                    {
                        cbEveryday.Checked = true;
                    }
                    else
                    {
                        for (int i = 0; i < dates.Length; i++)
                        {
                            switch (dates[i])
                            {
                                case "0":
                                    cbSun.Checked = true;
                                    break;
                                case "1":
                                    cbMon.Checked = true;
                                    break;
                                case "2":
                                    cbTue.Checked = true;
                                    break;
                                case "3":
                                    cbWed.Checked = true;
                                    break;
                                case "4":
                                    cbThur.Checked = true;
                                    break;
                                case "5":
                                    cbFri.Checked = true;
                                    break;
                                case "6":
                                    cbSat.Checked = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
            }
            
            if (!string.IsNullOrEmpty(tms.priorHoloday))
            {
                switch (tms.priorHoloday)
                {
                    case "00000001":
                        cbPriorHoliday.Checked = true;
                        break;
                    case "01000001":
                        cbPriorHoliday.Checked = false;
                        break;
                    default:
                        break;
                }
            }
            
        }

        #endregion


        /// <summary>
        /// 在选中节点的基础上 按IP和定时号ID 寻找timerList表中是timers
        /// </summary>
        /// <returns></returns>
        private DataJson.timers getTimersInfoList()
        {
            if (FileMesege.timerSelectNode == null || FileMesege.timerSelectNode.Parent == null)
            {
                return null;
            }
            string ip = FileMesege.timerSelectNode.Parent.Text.Split(' ')[0];
            string[] timerNodetxt = FileMesege.timerSelectNode.Text.Split(' ');
            int timerNum = Convert.ToInt32(Regex.Replace(timerNodetxt[0], @"[^\d]*", ""));
            return DataListHelper.getTimersInfoList(ip, timerNum);
        }


        #region 基本日期选择处理

        private void cbMon_MouseUp(object sender, MouseEventArgs e)
        {
            clearVery_customCheck();
            datesUpdate();
        }

        private void cbTue_MouseUp(object sender, MouseEventArgs e)
        {
            clearVery_customCheck();
            datesUpdate();
        }

        private void cbWed_MouseUp(object sender, MouseEventArgs e)
        {
            clearVery_customCheck();
            datesUpdate();
        }

        private void cbThur_MouseUp(object sender, MouseEventArgs e)
        {
            clearVery_customCheck();
            datesUpdate();
        }

        private void cbFri_MouseUp(object sender, MouseEventArgs e)
        {
            clearVery_customCheck();
            datesUpdate();
        }

        private void cbSat_MouseUp(object sender, MouseEventArgs e)
        {
            clearVery_customCheck();
            datesUpdate();
        }

        private void cbSun_MouseUp(object sender, MouseEventArgs e)
        {
            clearVery_customCheck();
            datesUpdate();
        }

        private void cbEveryday_MouseUp(object sender, MouseEventArgs e)
        {
            clear1_7Check();
            cbCustom.Checked = false;
            datesUpdate();
        }

        //自定义假期
        private void cbCustom_MouseUp(object sender, MouseEventArgs e)
        {
            clear1_7Check();
            cbEveryday.Checked = false;
            datesUpdate();

        }

        //跳过节假日
        private void cbPriorHoliday_MouseUp(object sender, MouseEventArgs e)
        {
            priorHolidayUpdate();
        }

      

        //清除每天和 自定义的选中
        private void clearVery_customCheck()
        {
            cbEveryday.Checked = false;
            cbCustom.Checked = false;
        }

        //清除星期一 至 星期日的选中
        private void clear1_7Check()
        {
            cbMon.Checked = false;
            cbTue.Checked = false;
            cbWed.Checked = false;
            cbThur.Checked = false;
            cbFri.Checked = false;
            cbSat.Checked = false;
            cbSun.Checked = false;
        }

        //更新timerList日期dates
        private void datesUpdate()
        {
            DataJson.timers tms = getTimersInfoList();
            if (tms == null)
            {
                return;
            }
            List<string> tmp = new List<string>();
            
            //全部选中
            if (cbEveryday.Checked)
            {
                tmp.Add("0");
                tmp.Add("1");
                tmp.Add("2");
                tmp.Add("3");
                tmp.Add("4");
                tmp.Add("5");
                tmp.Add("6");
            }
            else if (cbCustom.Checked)
            {
                //自定义日期
                for (int i = 0; i < listbox.Items.Count; i++)
                {

                    tmp.Add(listbox.Items[i].ToString());
                }
            }
            else
            {
                if (cbSun.Checked)
                {
                    tmp.Add("0");
                }
                if (cbMon.Checked)
                {
                    tmp.Add("1");
                }
                if (cbTue.Checked)
                {
                    tmp.Add("2");
                }
                if (cbWed.Checked)
                {
                    tmp.Add("3");
                }
                if (cbThur.Checked)
                {
                    tmp.Add("4");
                }
                if (cbFri.Checked)
                {
                    tmp.Add("5");
                }
                if (cbSat.Checked)
                {
                    tmp.Add("6");
                }
            }
            tms.dates = "";
            for (int i = 0; i < tmp.Count; i++)
            {
                tms.dates = string.Format("{0} {1}", tms.dates, tmp[i]);
            }
            tms.dates = tms.dates.Trim().Replace(" ", ",");
            AppTxtShow(tms.dates);
        }

        //更新timerList是否为网关priorHoliday
        private void priorHolidayUpdate()
        {
            DataJson.timers tms = getTimersInfoList();
            if (tms == null)
            {
                return;
            }
            if (cbPriorHoliday.Checked)
            {
                tms.priorHoloday = "00000001";
            }
            else
            {
                tms.priorHoloday = "01000001";
            }
        }

        #endregion

        #region 自定义日期
        private void btnAddDay_Click(object sender, EventArgs e)
        {
            
            
            if (!cbCustom.Checked)
            {
                return;
            }
            if (FileMesege.timerSelectNode == null || FileMesege.timerSelectNode.Parent == null)
            {
                return;
            }
            timerYYHHDD tmy = new timerYYHHDD();
            tmy.AddCustomDate += new Action<string>(tmy_AddCustomDate);
            Point pt = MousePosition;
            //把窗口向屏幕中间刷新
            tmy.StartPosition = FormStartPosition.Manual;
            tmy.Left = pt.X + 10;
            tmy.Top = pt.Y + 10;
            //把窗口向屏幕中间刷新
            tmy.Show();
        }

        private void btnDelDay_Click(object sender, EventArgs e)
        {
            if (!cbCustom.Checked)
            {
                return;
            }
            if (FileMesege.timerSelectNode == null || FileMesege.timerSelectNode.Parent == null)
            {
                return ;
            }
            listbox.Items.Remove(listbox.SelectedItem);
            datesUpdate();


        }


        //添加日期回调 用哈希数表排序
        private void tmy_AddCustomDate(string date)
        {
            
            for (int i = 0; i < listbox.Items.Count; i++)
            {
                if (date == listbox.Items[i].ToString())
                {
                    //存在相同的
                    return;
                }
            }
            listbox.Items.Add(date);    
            //排序   
            lisboxSort();
            datesUpdate();
        }

        /// <summary>
        /// 排序列表日期
        /// </summary>
        private void lisboxSort()
        {
            List<string> tmp = new List<string>();
            for (int i = 0; i < listbox.Items.Count; i++)
            {
                tmp.Add(listbox.Items[i].ToString());
            }
            //排序
            tmp.Sort(delegate(string x, string y)
            {
                string[] xip = x.Split('/');
                string[] yip = y.Split('/');

                //月份为* 
                if (xip[1].Contains("*"))
                {
                    //比较项月份为星
                    if (yip[1].Contains("*"))
                    {
                        if (xip[0].Contains("*") && yip[0].Contains("*"))
                        {
                            return Convert.ToInt32(xip[2]).CompareTo(Convert.ToInt32(yip[2]));
                        }
                        if (xip[0].Contains("*") && !yip[0].Contains("*"))
                        {
                            return -1;
                        }
                        if (!xip[0].Contains("*") && yip[0].Contains("*"))
                        {
                            return 1;
                        }


                    }
                    else
                    {
                        //月份不为星
                        return -1;
                    }
                }

                //年为* 
                if (xip[0].Contains("*"))
                {
                    //比较项月份为*
                    if (yip[1].Contains("*"))
                    {
                        return 1;
                    }
                    //比较项年份为*
                    if (yip[0].Contains("*"))
                    {
                        //同月份  不同月份
                        if (xip[1] == yip[1])
                        {
                            return Convert.ToInt32(xip[2]).CompareTo(Convert.ToInt32(yip[2]));
                        }
                        else
                        {
                            return Convert.ToInt32(xip[1]).CompareTo(Convert.ToInt32(yip[1]));
                        }
                    }
                    return -1;
                }

                try
                {
                    if (xip[0] == yip[0])
                    {
                        if (xip[1] == yip[1])
                        {

                            return Convert.ToInt32(xip[2]).CompareTo(Convert.ToInt32(yip[2]));

                        }

                        return Convert.ToInt32(xip[1]).CompareTo(Convert.ToInt32(yip[1]));

                    }

                    return Convert.ToInt32(xip[0]).CompareTo(Convert.ToInt32(yip[0]));
                }
                catch
                {
                    return 1;
                }

            });
            listbox.Items.Clear();
            for (int i = 0; i < tmp.Count; i++)
            {
                listbox.Items.Add(tmp[i]);
            }
        }

        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }

        private void btnDown_Click(object sender, EventArgs e)
        {

        }

        private void btnOn_Click(object sender, EventArgs e)
        {

        }

        private void btnOff_Click(object sender, EventArgs e)
        {

        }

        private void btnDel_Click(object sender, EventArgs e)
        {

        }

  

      

     
    }
}
