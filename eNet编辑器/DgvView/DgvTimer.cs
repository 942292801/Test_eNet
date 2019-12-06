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
using System.Net.Sockets;
using Newtonsoft.Json;
using DevComponents.DotNetBar.Controls;
using System.Threading;

namespace eNet编辑器.DgvView
{
    public partial class DgvTimer : Form
    {
        public DgvTimer()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
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

        string ip = "";
      

        /// <summary>
        /// 传输point点跳转窗口
        /// </summary>
        public event Action<DataJson.PointInfo> jumpSetInfo;

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
            dgvc.ReadOnly = true;
            dgvc.Name = "type";

            //插入
            this.dataGridView1.Columns.Insert(1, dgvc);
        }

        void ListBox3DataItemVisualCreated(object sender, DataItemVisualEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)e.Visual;
            item.HotTracking = true;
            
        }

        #region 刷新dgv框相关操作 

        public void TimerAddItem()
        {
            Thread t = new Thread(ShowDatatable);
            t.IsBackground = true;
            t.Start();
        }
        #region 测试异步加载
        public delegate void FormIniDelegate();
        private void ShowDatatable()
        {
            this.Invoke(new FormIniDelegate(TabIni));

        }


        #endregion
        /// <summary>
        /// 刷新该节点的所有信息
        /// </summary>
        public void TabIni()
        {
           
            try
            {
                IniForm();
                //查看获取对象是否存在
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                dealtmsDates_prior(tms);
                List<DataJson.timersInfo> delTimer = new List<DataJson.timersInfo>();
                ip = FileMesege.timerSelectNode.Parent.Text.Split(' ')[0];
                //循环加载该定时号的所有信息
                foreach (DataJson.timersInfo tmInfo in tms.timersInfo)
                {

                    if (addItem(tmInfo, ip) != null)
                    {
                        delTimer.Add(tmInfo);
                    }


                }
                for (int i = 0; i < delTimer.Count; i++)
                {
                    tms.timersInfo.Remove(delTimer[i]);
                }
                DgvMesege.RecoverDgvForm(dataGridView1, X_Value, Y_Value, rowCount, columnCount);

            }
            catch (Exception ex)
            {
                this.dataGridView1.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }

           
           
        }

        public DataJson.timersInfo addItem(DataJson.timersInfo tmInfo, string ip)
        {
            int dex = dataGridView1.Rows.Add();

            if (tmInfo.pid == 0)
            {
                //pid号为0则为空 按地址来找
                if (tmInfo.address != "" && tmInfo.address != "FFFFFFFF")
                {
                    DataJson.PointInfo point = DataListHelper.findPointByType_address(tmInfo.type, tmInfo.address, ip);
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
                   
                    dataGridView1.Rows.Remove(dataGridView1.Rows[dex]);
                    return tmInfo;
                }
                else
                {
                    //pid号有效

                    try
                    {
                        if (tmInfo.address.Substring(2, 6) != point.address.Substring(2, 6))
                        {
                            tmInfo.address = point.address;

                        }
                    }
                    catch
                    {
                        tmInfo.address = point.address;
                    }
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
            dataGridView1.Rows[dex].Cells[1].Value = IniHelper.findTypesIniNamebyType(tmInfo.type);
            dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(tmInfo.address);

            dataGridView1.Rows[dex].Cells[5].Value = (tmInfo.optName + " " + tmInfo.opt).Trim();
            dataGridView1.Rows[dex].Cells[6].Value = tmInfo.shortTime;
            dataGridView1.Rows[dex].Cells[7].Value = "删除";
            return null;
        }

        /// <summary>
        /// 清空初始化窗体
        /// </summary>
        private void IniForm()
        {
            clear1_7Check();
            clearVery_customCheck();
            cbPriorHoliday.Checked = false;
            panel9.Visible = false;
            listbox.Visible = false;
            listbox.Items.Clear();
            multipleList.Clear();
            this.dataGridView1.Rows.Clear();
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
                    panel9.Visible = true;
                    listbox.Visible = true;
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
                        rdbcheck = true;
                        break;
                    case "01000001":
                        cbPriorHoliday.Checked = false;
                        rdbcheck = false;
                        break;
                    default:
                        break;
                }
            }
            
        }

        public void clearDgvClear()
        {
            dataGridView1.Rows.Clear();
        }

        #endregion

        #region 数据操作工具
       

        /// <summary>
        /// 定时信息 timerInfo重新赋值排序
        /// </summary>
        /// <param name="tms"></param>
        private void timerInfoSort(DataJson.timers tms)
        {
            int i = 1;
            foreach(DataJson.timersInfo tmInfo in tms.timersInfo )
            {
                tmInfo.id = i;
                i++;
            }
        }

        #endregion

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
            if (cbCustom.Checked)
            {
                panel9.Visible = true;
                listbox.Visible = true;
            }
            else
            {
                panel9.Visible = false;
                listbox.Visible = false;
            }
            datesUpdate();
        }

        //自定义假期
        private void cbCustom_MouseUp(object sender, MouseEventArgs e)
        {
            clear1_7Check();
            cbEveryday.Checked = false;
            if (cbCustom.Checked)
            {
                panel9.Visible = true;
                listbox.Visible = true;
            }
            else
            {
                panel9.Visible = false;
                listbox.Visible = false;
            }
            datesUpdate();

        }

        bool rdbcheck = false;
        //跳过节假日
        private void cbPriorHoliday_MouseUp(object sender, MouseEventArgs e)
        {
            
            if (rdbcheck)
            {
                cbPriorHoliday.Checked = false;
                rdbcheck = false;
            }
            else
            {
                cbPriorHoliday.Checked = true;
                rdbcheck = true;
            }
            priorHolidayUpdate();
        }

     

        //清除每天和 自定义的选中
        private void clearVery_customCheck()
        {
            cbEveryday.Checked = false;
            cbCustom.Checked = false;

            panel9.Visible = false;
            listbox.Visible = false;
            
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
            DataJson.timers tms = DataListHelper.getTimersInfoList();
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
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            
            tms.dates = "";
            for (int i = 0; i < tmp.Count; i++)
            {
                tms.dates = string.Format("{0} {1}", tms.dates, tmp[i]);
            }

            tms.dates = tms.dates.Trim().Replace(" ", ",");
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            
            //AppTxtShow(tms.dates);
        }

        //更新timerList是否为网关priorHoliday
        private void priorHolidayUpdate()
        {
            DataJson.timers tms = DataListHelper.getTimersInfoList();
            if (tms == null)
            {
                return;
            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (cbPriorHoliday.Checked)
            {
                tms.priorHoloday = "00000001";
            }
            else
            {
                tms.priorHoloday = "01000001";
            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
           
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
            //-1 为往上排 1为往下排
            //排序
            tmp.Sort(delegate(string x, string y)
            {
                string[] xip = x.Split('/');
                string[] yip = y.Split('/');

                if (x.Contains("*"))
                {
                    //x存在*
                    if (y.Contains("*"))
                    {
                        //////////////////////////y存在星  xy同时存在星
                        if (xip[0].Contains("*"))
                        {
                            //x年存在*
                            if (yip[0].Contains("*"))
                            {
                                //y年存在*   （xy年都存在*）
                                if (xip[1].Contains("*"))
                                {
                                    //x月存在*
                                    if (yip[1].Contains("*"))
                                    {
                                        //y月存在* （xy月都存在*）
                                        return Convert.ToInt32(xip[2]).CompareTo(Convert.ToInt32(yip[2]));
                                    }
                                    else
                                    {
                                        //y月不存在*
                                        return -1;
                                    }
                                }
                                else
                                {
                                    //x月不存在*
                                    if (yip[1].Contains("*"))
                                    {
                                        //y月存在*
                                        return 1;
                                    }
                                    else
                                    {
                                        //y月不存在*
                                        if (xip[1] == yip[1])
                                        {
                                            //年月都相同
                                            return Convert.ToInt32(xip[2]).CompareTo(Convert.ToInt32(yip[2]));
                                        }
                                        else
                                        {
                                            //对比月
                                            return Convert.ToInt32(xip[1]).CompareTo(Convert.ToInt32(yip[1]));
                                        }
                                    }
                                   
                                }
                            }
                            else
                            {
                                //y年不存在*
                                return -1;
                            }
                        }
                        else
                        {
                            //x年不存在*
                            if (yip[0].Contains("*"))
                            {
                                //y年存在*
                                return 1;
                            }
                            else
                            {
                                //y年不存在*
                                if (xip[0] == yip[0])
                                {
                                    //年相同 则*在月 只能比较日期
                                    return Convert.ToInt32(xip[2]).CompareTo(Convert.ToInt32(yip[2]));
                                }
                                else
                                { 
                                    //年不相同 则比较年限
                                    return Convert.ToInt32(xip[0]).CompareTo(Convert.ToInt32(yip[0]));
                                }
                            
                            }
                        }

                        ////////////////////////////////////////

                    }
                    else
                    {
                        //y不存在星
                        return -1;
                    }
                }
                else
                { 
                    //x不存在*
                    if (y.Contains("*"))
                    {
                        //y存在星
                        return 1;
                    }
                    else
                    { 
                        //y不存在星  正常比较
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

                }

                    
              

            });
            listbox.Items.Clear();
            for (int i = 0; i < tmp.Count; i++)
            {
                listbox.Items.Add(tmp[i]);
            }
        }

        #endregion

        #region 增加 清空 下载 开启 关闭 删除选中行
        //增加
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                
                //新建表
                DataJson.timersInfo tmInfo = new DataJson.timersInfo();

                int id = 0;
                string type = "", opt = "", optname = "", add = "", shortTime = "";
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                if (tms.timersInfo.Count > 0)
                {
                    id = tms.timersInfo[tms.timersInfo.Count - 1].id;
                    type = tms.timersInfo[tms.timersInfo.Count - 1].type;
                    opt = tms.timersInfo[tms.timersInfo.Count - 1].opt;
                    optname = tms.timersInfo[tms.timersInfo.Count - 1].optName;
                    shortTime = tms.timersInfo[tms.timersInfo.Count - 1].shortTime;
                    add = tms.timersInfo[tms.timersInfo.Count - 1].address;
                }
                tmInfo.id = id +1;
                tmInfo.pid = 0;
                tmInfo.type = type;
                tmInfo.opt = opt;
                tmInfo.optName = optname;
                //地址加一处理 并搜索PointList表获取地址 信息
                if (!string.IsNullOrEmpty(add) && add != "FFFFFFFF")
                {
                    switch (add.Substring(2, 2))
                    {
                        case "00":
                            //设备类地址
                            add = add.Substring(0, 6) + ToolsUtil.strtohexstr((Convert.ToInt32(add.Substring(6, 2), 16) + 1).ToString());
                            break;
                        default:
                            string hexnum = ToolsUtil.strtohexstr((Convert.ToInt32(add.Substring(4, 4), 16) + 1).ToString());
                            while (hexnum.Length < 4)
                            {
                                hexnum = hexnum.Insert(0, "0");
                            }
                            add = add.Substring(0, 4) + hexnum;
                            break;
                    }
                    
                    //按照地址查找type的类型 
                    type = IniHelper.findIniTypesByAddress(ip, add).Split(',')[0];
                
                    tmInfo.type = type;
                    //添加地域和名称 在sceneInfo表中
                    DataJson.PointInfo point = DataListHelper.findPointByType_address(type, add,ip);
                    if (point != null)
                    {
                        tmInfo.pid = point.pid;
                        tmInfo.type = point.type;
                        if (tmInfo.type != point.type)
                        {
                            tmInfo.opt = "";
                            tmInfo.optName = "";
                        }
                    }
                    else
                    {
                        tmInfo.pid = 0;
                        //tmInfo.opt = "";
                        //tmInfo.optName = "";
                    }
                }

                tmInfo.address = add;
                
                tmInfo.shortTime = shortTime;
                tms.timersInfo.Add(tmInfo);
                //排序
                //timerInfoSort(tms);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                addItem(tmInfo,ip);
                DgvMesege.selectLastCount(dataGridView1);  
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }


        public void selectLastCount()
        {
            DgvMesege.selectLastCount(dataGridView1);  
        }

        //清空
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                tms.timersInfo.Clear();
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                this.dataGridView1.Rows.Clear();
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        //下载
        private void btnDown_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            try
            {
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                //定时信息不为空
                if (tms.timersInfo.Count > 0)
                {
                    DataJson.Tn tn = new DataJson.Tn();
                    tn.timer = new List<DataJson.Timernumber>();
                    if (string.IsNullOrEmpty(tms.dates))
                    {
                        //没有填写执行日期
                        AppTxtShow("载入失败,需要填写执行日期！");
                        return;
                    }
                    //把有效的对象操作 放到SN对象里面
                    foreach (DataJson.timersInfo tmInfo in tms.timersInfo)
                    {
                        //确保有信息
                        if (string.IsNullOrEmpty(tmInfo.opt) || string.IsNullOrEmpty(tmInfo.shortTime))
                        {
                            continue;
                        }
                        int hour = 255;
                        int min = 255;
                        if (tmInfo.shortTime == "日出时间")
                        {
                            hour = 254;
                            min = 254;
                        }
                        else if (tmInfo.shortTime == "日落时间")
                        {
                            hour = 253;
                            min = 253;
                        }
                        else
                        {
                            string tmpHour = tmInfo.shortTime.Split(':')[0];
                            string tmpMin = tmInfo.shortTime.Split(':')[1];
                            if (!tmpHour.Contains("*"))
                            {
                                hour = Convert.ToInt32(tmpHour);
                            }
                            if (!tmpMin.Contains("*"))
                            {
                                min = Convert.ToInt32(tmpMin);
                            }

                        }
                        
                        string[] dates = tms.dates.Split(',');
                        if (tms.dates.Contains("/"))
                        {
                            //自定义日期
                            for (int i = 0; i < dates.Length; i++)
                            {
                                DataJson.Timernumber sb = new DataJson.Timernumber();

                                sb.num = tmInfo.id;
                                sb.obj = tmInfo.address;
                                sb.data = tmInfo.opt;
                                sb.optname = tmInfo.optName;
                                sb.hour = hour;
                                sb.min = min;
                                string[] ymd = dates[i].Split('/');
                                if (ymd[0].Contains("*"))
                                {
                                    //年为255
                                    sb.year = 255;
                                }
                                else
                                {
                                    sb.year = Convert.ToInt32(ymd[0]);
                                }
                                if (ymd[1].Contains("*"))
                                {
                                    //月为255
                                    sb.mon = 255;
                                }
                                else
                                {
                                    sb.mon = Convert.ToInt32(ymd[1]);
                                }
                                //这个是日
                                sb.date = Convert.ToInt32(ymd[2]);
                                //这个是周
                                sb.day = 255;
                                tn.timer.Add(sb);
                            }
                        }
                        else
                        {
                            //星期一到日 0-7
                            for (int i = 0; i < dates.Length; i++)
                            {
                                DataJson.Timernumber sb = new DataJson.Timernumber();
                                sb.num = tmInfo.id;
                                sb.obj = tmInfo.address;
                                sb.data = tmInfo.opt;
                                sb.optname = tmInfo.optName;
                                sb.hour = hour;
                                sb.min = min;
                                sb.year = 255;
                                sb.mon = 255;
                                sb.date = 255;
                                sb.day = Convert.ToInt32(dates[i]);
                                tn.timer.Add(sb);
                            }
                        }
                        
                    }
                    if (tn.timer.Count > 0)
                    {
                        //序列化SN对象
                        string sjson = JsonConvert.SerializeObject(tn);

                        //写入数据格式
                        string path = "down /json/t" + tms.id + ".json$" + sjson;
                        //测试写出文档
                        //File.WriteAllText(FileMesege.filePath + "\\objs\\s" + sceneNum + ".json", path);
                        //string check = "exist /json/s" + sceneNum + ".json$";
                        TcpSocket ts = new TcpSocket();
                        int i = 0;
                        while (i < 10)
                        {
                            sock = ts.ConnectServer(ip, 6001, 1);
                            if (sock == null)
                            {
                                i++;
                            }
                            else
                            {
                                i = 0;
                                break;
                            }
                        }

                        int flag = -1;
                        while (sock != null)
                        {
                            if (sock == null)
                            {
                                break;
                            }
                            if (i == 10)
                            {
                                break;
                            }
                            //重连
                            //0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常
                            flag = ts.SendData(sock, path, 1);
                            //flag = ts.SendData(sock, "exist /json/s" + sceneNum + ".json$", 1);
                            if (flag == 0)
                            {
                                AppTxtShow("加载成功！");
                                break;
                            }
                            i++;

                        }
                        if (sock != null)
                        {
                            sock.Dispose();
                        }

                        if (i == 10)
                        {
                            AppTxtShow("加载失败");
                        }

                    }//if有场景信息
                    else
                    {
                        AppTxtShow("无定时指令！");
                    }

                }
                else
                {
                    AppTxtShow("无定时指令！");
                }

            }
            catch
            {
                //Exception ex
                //TxtShow("加载失败！\r\n");
            }
        }

        private void btnOn_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.timerSelectNode == null || FileMesege.timerSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string[] ids = FileMesege.timerSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = ToolsUtil.getIP(FileMesege.timerSelectNode);
                TcpSocket ts = new TcpSocket();

                sock = ts.ConnectServer(ip, 6003, 2);
                if (sock == null)
                {
                    //防止一连失败
                    sock = ts.ConnectServer(ip, 6003, 2);
                    if (sock == null)
                    {
                        AppTxtShow("连接失败！请检查网络");
                        //sock.Close();
                        return;
                    }

                }
                string number = "";
                if (sceneNum < 256)
                {
                    number = String.Format("0.{0}", sceneNum.ToString());
                }
                else
                {
                    //模除剩下的数
                    int num = sceneNum % 256;
                    //有多小个256
                    sceneNum = (sceneNum - num) / 256;
                    number = String.Format("{0}.{1}", sceneNum.ToString(), num.ToString());
                }


                string oder = String.Format("SET;00000001;{{{0}.32.{1}}};\r\n", ip4, number);  // "SET;00000001;{" + ip4 + ".16." + number + "};\r\n";
                int flag = ts.SendData(sock, oder, 2);
                if (flag == 0)
                {
                    AppTxtShow("发送指令成功！");
                    sock.Dispose();
                }
                else
                {
                    flag = ts.SendData(sock, oder, 2);
                    if (flag == 0)
                    {
                        AppTxtShow("发送指令成功！");
                        sock.Dispose();
                    }
                }
            }
            catch
            {
                //TxtShow("发送指令失败！\r\n");
            }
            
        }

        private void btnOff_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.timerSelectNode == null || FileMesege.timerSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string[] ids = FileMesege.timerSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = ToolsUtil.getIP(FileMesege.timerSelectNode);
                TcpSocket ts = new TcpSocket();

                sock = ts.ConnectServer(ip, 6003, 2);
                if (sock == null)
                {
                    //防止一连失败
                    sock = ts.ConnectServer(ip, 6003, 2);
                    if (sock == null)
                    {
                        AppTxtShow("连接失败！请检查网络");
                        //sock.Close();
                        return;
                    }

                }
                string number = "";
                if (sceneNum < 256)
                {
                    number = String.Format("0.{0}", sceneNum.ToString());
                }
                else
                {
                    //模除剩下的数
                    int num = sceneNum % 256;
                    //有多小个256
                    sceneNum = (sceneNum - num) / 256;
                    number = String.Format("{0}.{1}", sceneNum.ToString(), num.ToString());
                }


                string oder = String.Format("SET;00000000;{{{0}.32.{1}}};\r\n", ip4, number);  // "SET;00000001;{" + ip4 + ".16." + number + "};\r\n";
                int flag = ts.SendData(sock, oder, 2);
                if (flag == 0)
                {
                    AppTxtShow("发送指令成功！");
                    sock.Dispose();
                }
                else
                {
                    flag = ts.SendData(sock, oder, 2);
                    if (flag == 0)
                    {
                        AppTxtShow("发送指令成功！");
                        sock.Dispose();
                    }
                }
            }
            catch
            {
                //TxtShow("发送指令失败！\r\n");
            }
        }

        //删除选中行
        private void btnDel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if ((bool)dataGridView1.Rows[i].Cells[8].EditedFormattedValue)
                {
                    Multiple(i);
                }
            }
            if (dataGridView1.RowCount < 1 || multipleList.Count == 0)
            {
                //没有选中数据
                return;
            }
            else
            {
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //获取该节点IP地址场景下的 场景信息对象
                foreach (DataJson.timersInfo info in multipleList)
                {
                    tms.timersInfo.Remove(info);
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                multipleList.Clear();
                timerInfoSort(tms);
                TimerAddItem();
            }
        }


        //存储删除选中行信息点
        HashSet<DataJson.timersInfo> multipleList = new HashSet<DataJson.timersInfo>();

        //记录选中的项的timerInfo信息
        private void Multiple(int rowNumber)
        {

            if (!(bool)dataGridView1.Rows[rowNumber].Cells[8].EditedFormattedValue)
            {
                return;
            }
            DataJson.timers tms = DataListHelper.getTimersInfoList();
            if (tms == null)
            {
                return;
            }
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.timersInfo info = DataListHelper.getTimerInfo(tms, Convert.ToInt32(dataGridView1.Rows[rowNumber].Cells[0].Value));
            if (info == null)
            {
                return;
            }
            multipleList.Add(info);
        }

        #endregion


        #region del按键 删除操作
        //del按键
        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Delete)
                {

                    DelKeyOpt();

                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        //删除操作
        private void DelKeyOpt()
        {
            try
            {


                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                    DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, id);
                    if (tmInfo == null)
                    {
                        continue;
                    }
                    if (colIndex == 5)
                    {

                        ischange = true;
                        tmInfo.opt = "";
                        tmInfo.optName = "";

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = null;
                    }//if
                    if (colIndex == 6)
                    {

                        ischange = true;
                        tmInfo.shortTime = "";

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[6].Value = null;
                    }//if
                   
                }
                if (ischange)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }

        }
        #endregion

        #region 表格单击双击 操作 高亮显示
        private bool isFirstClick = true;
        private bool isDoubleClick = false;
        private int milliseconds = 0;
        /// <summary>
        /// 行号
        /// </summary>
        private int rowCount = 0;
        /// <summary>
        /// 列号
        /// </summary>
        private int columnCount = 0;
        private int oldrowCount = 0;
        private int oldcolumnCount = 0;

        bool isClick = false;

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            DgvMesege.endDataViewCurrent(dataGridView1, e.Y, e.X);
        }

        //移动到删除的时候高亮一行
        private void dataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (isClick == true)
            {
                return;

            }
            else
            {
                //选中行号
                int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[rowNum].Selected = true;//选中行
                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            oldrowCount = rowCount;
            oldcolumnCount = columnCount;
            rowCount = e.RowIndex;
            columnCount = e.ColumnIndex;
            // 鼠标单击.
            if (isFirstClick)
            {
                isFirstClick = false;
                doubleClickTimer.Start();
            }
            // 鼠标双击
            else
            {

                isDoubleClick = true;
            }
            if (isClick)
            {
                if (dataGridView1.SelectedCells.Count == 1 && rowCount == oldrowCount && columnCount == oldcolumnCount)
                {
                    return;

                }
                else if (oldcolumnCount == columnCount)
                {
                    return;
                }
                isClick = false;
            }
            else
            {
                isClick = true;
            }
        }

        private void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            milliseconds += 100;
            // 第二次鼠标点击超出双击事件间隔
            if (milliseconds >= SystemInformation.DoubleClickTime)
            {
                doubleClickTimer.Stop();


                if (isDoubleClick)
                {

                    if (rowCount >= 0 && columnCount >= 0)
                    {
                        int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            case "address":
                                //改变地址
                                string add = "";
                                if (dataGridView1.Rows[rowCount].Cells[2].Value != null)
                                {
                                    //原地址
                                    add = dataGridView1.Rows[rowCount].Cells[2].Value.ToString();
                                }
                                string objType = dataGridView1.Rows[rowCount].Cells[1].EditedFormattedValue.ToString();
                                //赋值List 并添加地域 名字
                                dgvAddress(id, objType, add);

                                break;
                            case "operation":

                                //操作
                                string info = dgvOperation(Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value), dataGridView1.Rows[rowCount].Cells[1].EditedFormattedValue.ToString());
                                if (info != null)
                                {
                                    dataGridView1.Rows[rowCount].Cells[5].Value = info;
                                }

                                break;

                            case "shortTime":
                                //改变延时
                                dgvShortTimer(Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value), dataGridView1.Rows[rowCount].Cells[6].Value.ToString());
                                break;
                            case "del":
                                //删除表
                                dgvDle(id);
                     
                                break;
                            case "num":
                                //设置对象跳转
                                DataJson.PointInfo point = dgvJumpSet(id);
                                if (point != null)
                                {
                                    //传输到form窗口控制
                                    //AppTxtShow("传输到主窗口"+DateTime.Now);
                                    jumpSetInfo(point);
                                }
                                break;
                            default: break;
                        }
                        try
                        {
                            //更改内容回自动刷新到第一行
                            dataGridView1.CurrentCell = dataGridView1.Rows[rowCount].Cells[columnCount];
                        }
                        catch
                        {
                            if (dataGridView1.Rows.Count > 0)
                            {
                                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[columnCount];
                            }

                        }
                    }
                }
                else
                {
                    //处理单击事件操作

                    if (rowCount >= 0 && columnCount >= 0)
                    {
                        //DGV的行号
                        int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            case "del":
                                //删除表
                                dgvDle(id);
                                break;
                            default: break;



                        }
                        try
                        {
                            //更改内容回自动刷新到第一行
                            dataGridView1.CurrentCell = dataGridView1.Rows[rowCount].Cells[columnCount];
                        }
                        catch
                        {
                            if (dataGridView1.Rows.Count > 0)
                            {
                                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[columnCount];
                            }

                        }
                    }
                }
                isFirstClick = true;
                isDoubleClick = false;
                milliseconds = 0;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

      
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //选中行号
            int rowNum = e.RowIndex;
            //选中列号
            int columnNum = e.ColumnIndex;
            if (rowNum >= 0 && columnNum >= 0)
            {
                switch (dataGridView1.Columns[columnNum].Name)
                {

                    case "checkDel":
                        dataGridView1.Rows[rowNum].Selected = true;//选中行

                        for (int i = dataGridView1.SelectedRows.Count; i > 0; i--)
                        {
                            dataGridView1.SelectedRows[i - 1].Cells[8].Value = true;

                        }
                        //提交编辑
                        dataGridView1.EndEdit();
                        break;


                    default: break;
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //选中行号
                int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    switch (dataGridView1.Columns[columnNum].Name)
                    {
                        
                        case "type":
                            //改变对象  
                            string isChange = dgvObjtype(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), dataGridView1.Rows[rowNum].Cells[1].EditedFormattedValue.ToString());
                            if (!string.IsNullOrEmpty(isChange))
                            {
                                dataGridView1.Rows[rowNum].Cells[1].Value = IniHelper.findTypesIniNamebyType(isChange);
                            }
                            break;
                        
                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }


        /// <summary>
        /// 对象跳转获取 场景 定时 编组 point点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataJson.PointInfo dgvJumpSet(int id)
        {
            DataJson.timers tms = DataListHelper.getTimersInfoList();
            if (tms == null)
            {
                return null;
            }
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, id);
            if (tmInfo == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(tmInfo.address))
            {
                return null;
            }
            if (tmInfo.type == "4.0_scene" || tmInfo.type == "5.0_timer" || tmInfo.type == "6.1_panel" || tmInfo.type == "6.2_sensor")
            {
                return DataListHelper.findPointByType_address(tmInfo.type, tmInfo.address,ip);
            }

            return null;

        }

        /// <summary>
        /// 修改DGV表类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private string dgvObjtype(int id, string type)
        {

            DataJson.timers tms = DataListHelper.getTimersInfoList();
            if (tms == null)
            {
                return null;
            }
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, id);
            if (tmInfo == null)
            {
                return null;
            }
            
            if (tmInfo.pid != 0)
            {
                DataJson.PointInfo point = DataListHelper.findPointByPid(tmInfo.pid);
                if (point.type != "")
                {

                    return point.type;
                }

            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            tmInfo.type = IniHelper.findTypesIniTypebyName(type);
            tmInfo.opt = "";
            tmInfo.optName = "";
            
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return null;
        }


        //ID= 选中行的序号
        private void dgvDle(int id)
        {

            DataJson.timers tms = DataListHelper.getTimersInfoList();
            if (tms == null)
            {
                return ;
            }
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, id);
            if (tmInfo == null)
            {
                return ;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            tms.timersInfo.Remove(tmInfo);
            timerInfoSort(tms);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dataGridView1.Rows.Remove(dataGridView1.Rows[id - 1]);
            for (int i = id - 1; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i + 1);
            }
        }



        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="add">当前对象的地址</param>
        /// <returns></returns>
        private void dgvAddress(int id, string objType, string add)
        {
            sceneAddress dc = new sceneAddress();
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            //dc.Obj = obj;
            dc.ObjType = objType;
            dc.Obj = add;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                //获取sceneInfo对象表中对应ID号info对象
                DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, id);
                if (tmInfo == null)
                {
                    return;
                }
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //地址
                tmInfo.address = dc.Obj;
                if (string.IsNullOrEmpty(tmInfo.address))
                {
                    //地址为空直接退出
                    return;
                }
                
                //按照地址查找type的类型 
                string type = IniHelper.findIniTypesByAddress(ip, tmInfo.address).Split(',')[0];
                if (string.IsNullOrEmpty(type))
                {
                    type = dc.RtType; 
                }
                tmInfo.type = type;
                //获取树状图的IP第四位  + Address地址的 后六位
                string ad = tmInfo.address;
                //区域加名称
                DataJson.PointInfo point = DataListHelper.findPointByType_address(type, ad,ip);
                //撤销 
                
                if (point != null)
                {
                    tmInfo.pid = point.pid;
                    tmInfo.type = point.type;
                    if (tmInfo.type != point.type)
                    {
                        tmInfo.opt = "";
                        tmInfo.optName = "";
                    }
                }
                else
                {
                    tmInfo.pid = 0;
                    tmInfo.opt = "";
                    tmInfo.optName = "";
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            TimerAddItem();

        }


        /// <summary>
        /// DGV表 操作栏
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string dgvOperation(int id, string type)
        {

            
            sceneConcrol dc = new sceneConcrol();
            DataJson.timers tms = DataListHelper.getTimersInfoList();
            if (tms == null)
            {
                return null;
            }
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, id);
            if (tmInfo == null)
            {
                return null;
            }
            dc.Point = DataListHelper.findPointByPid(tmInfo.pid, FileMesege.PointList.equipment);
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            dc.ObjType = type;
            dc.Opt = tmInfo.opt;
            dc.Ver = tmInfo.optName;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                tmInfo.opt = dc.Opt;
                tmInfo.optName = dc.Ver;
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                return dc.Ver + " " + dc.Opt;
            }
            
            return null;

            
        }


        /// <summary>
        /// DGV表  定时时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shortTime"></param>
        private void dgvShortTimer(int id, string shortTime)
        {
            timerHHMM th = new timerHHMM();
            selectCellId = id;
            th.AddShortTime += new Action<string>(th_AddShortTime);
            Point pt = MousePosition;
            //把窗口向屏幕中间刷新
            th.StartPosition = FormStartPosition.Manual;
            th.Left = pt.X + 10;
            th.Top = pt.Y + 10;
            th.Date = shortTime;
            //把窗口向屏幕中间刷新
            th.Show();


            
        }

        //选中行的id号
        int selectCellId = 0;

        //设置shortTime回调函数
        private void th_AddShortTime(string delShortTime)
        {
            DataJson.timers tms = DataListHelper.getTimersInfoList();
            if (tms == null)
            {
                return;
            }
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, selectCellId);
            if (tmInfo == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            tmInfo.shortTime = delShortTime;
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dataGridView1.Rows[rowCount].Cells[6].Value = delShortTime;
        }

        #endregion


        #region 复制 粘贴
        /// <summary>
        /// 复制点位的对象 与参数 
        /// </summary>
        public void copyData()
        {
            //获取当前选中单元格的列序号
            int colIndex = dataGridView1.CurrentRow.Cells.IndexOf(dataGridView1.CurrentCell);
            //当粘贴选中单元格为操作
            if (colIndex == 5 || colIndex == 6)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                
                //获取sceneInfo对象表中对应ID号info对象
                DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, id);
                if (tmInfo == null)
                {
                    return;
                }
                //获取sceneInfo对象表中对应ID号info对象
                FileMesege.copyTimer = tmInfo;

            }


        }

        /// <summary>
        /// 粘贴点位的对象与参数
        /// </summary>
        public void pasteData()
        {
            
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                    DataJson.timersInfo tmInfo = DataListHelper.getTimerInfo(tms, id);
                    if (tmInfo == null)
                    {
                        continue;
                    }

                    if (FileMesege.copyTimer.type == "" || tmInfo.type == "" || tmInfo.type != FileMesege.copyTimer.type)
                    {
                        continue;
                    }
                    if (colIndex == 5)
                    {
                       
                        ischange = true;
                        tmInfo.opt = FileMesege.copyTimer.opt;
                        tmInfo.optName = FileMesege.copyTimer.optName;

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = (tmInfo.optName + " " + tmInfo.opt).Trim();
                    }//if
                    else if (colIndex == 6)
                    {
                        ischange = true;
                        tmInfo.shortTime = FileMesege.copyTimer.shortTime;

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[6].Value = tmInfo.shortTime;
                    }
                }
                if (ischange)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }

            }//try
            catch
            {

            }


        }
        #endregion


        
        #region 升序 相同 降序

        //相同
        public void Same()
        {
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
         
                //选中行 序号
                int id = 0;
                //列号
                int colIndex = 0;
                //记录第一个选中格的列号
                int FirstColumnIndex = -1;
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                DataJson.timersInfo tmInfo = null;
               
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作 第一个选中格的列号 
                        continue;
                    }
                    id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                    tmInfo = DataListHelper.getTimerInfo(tms, id);
                    if (tmInfo == null)
                    {
                        continue;
                    }
                    if (i == dataGridView1.SelectedCells.Count - 1)
                    {
                        //记录第一个选中格内容
                        FirstColumnIndex = colIndex;
                        FileMesege.copyTimer = tmInfo;
                        continue;
                    }
                    //当粘贴选中单元格为操作
                    if (colIndex == 5)
                    {

                        //获取sceneInfo对象表中对应ID号info对象
                        if (string.IsNullOrEmpty(FileMesege.copyTimer.type) || string.IsNullOrEmpty(tmInfo.type) || tmInfo.type != FileMesege.copyTimer.type)
                        {
                            //类型不一致 并且为空
                            continue;
                        }
                        ischange = true;
                        tmInfo.opt = FileMesege.copyTimer.opt;
                        tmInfo.optName = FileMesege.copyTimer.optName;
                        dataGridView1.Rows[id - 1].Cells[5].Value = (tmInfo.optName + " " + tmInfo.opt).Trim();
                    }//if
                    else if (colIndex == 2)
                    {
                        //选中单元格为地址

                        tmInfo.address = FileMesege.copyTimer.address;
                        if (FileMesege.copyTimer.type != tmInfo.type)
                        {
                            //类型不一样清空类型
                            tmInfo.opt = string.Empty;
                            tmInfo.optName = string.Empty;
                        }
                        tmInfo.type = FileMesege.copyTimer.type;
                        tmInfo.pid = FileMesege.copyTimer.pid;
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByPid(tmInfo.pid);
                        if (point != null)
                        {
                            tmInfo.type = point.type;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id - 1].Cells[4].Value = point.name;
                        }
                        else
                        {
                            tmInfo.pid = 0;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id - 1].Cells[4].Value = string.Empty;
                        }
                        dataGridView1.Rows[id - 1].Cells[1].Value = IniHelper.findTypesIniNamebyType(tmInfo.type);
                        dataGridView1.Rows[id - 1].Cells[2].Value = DgvMesege.addressTransform(tmInfo.address);
                        dataGridView1.Rows[id - 1].Cells[5].Value = (tmInfo.optName + " " + tmInfo.opt).Trim();
              
                        ischange = true;

                    }
                    else if (colIndex == 6)
                    {
                        //定时相同
                        ischange = true;
                        tmInfo.shortTime = FileMesege.copyTimer.shortTime;
                        dataGridView1.Rows[id - 1].Cells[6].Value = tmInfo.shortTime; ;
                    }

                }
                if (ischange)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }



            }//try
            catch
            {

            }
        }

        //升序
        public void Ascending()
        {
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
      
                //选中行 序号
                int id = 0;
                //列号
                int colIndex = 0;
                //记录第一个选中格的列号
                int FirstColumnIndex = -1;
                //地址添加量
                int addCount = 0;
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                DataJson.timersInfo tmInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                        tmInfo = DataListHelper.getTimerInfo(tms, id);
                        if (tmInfo == null)
                        {
                            continue;
                        }
                        FileMesege.copyTimer = tmInfo;
                        continue;
                    }
                    if (colIndex == FirstColumnIndex)
                    {
                        addCount++;
                    }
                }

                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        continue;
                    }
                    id = dataGridView1.SelectedCells[i].RowIndex + 1;//Convert.ToInt32(].Cells[0].Value);
                    tmInfo = DataListHelper.getTimerInfo(tms, id);
                    if (tmInfo == null )
                    {
                        continue;
                    }
                    if (addCount == 0)
                    {
                        continue;
                    }
                    //延时递增
                    if (colIndex == 6)
                    {
                        if (string.IsNullOrEmpty(FileMesege.copyTimer.shortTime))
                        {
                            return;
                        }
                        ischange = true;
                        string hour = FileMesege.copyTimer.shortTime.Split(':')[0];
                        string min = FileMesege.copyTimer.shortTime.Split(':')[1];
                        if (hour.Contains("*"))
                        {
                            if (min.Contains("*"))
                            {
                                tmInfo.shortTime = FileMesege.copyTimer.shortTime;
                            }
                            else
                            {
                                TimeSpan ts = new TimeSpan(24, Convert.ToInt32(min), 0);
                                TimeSpan addTs  = new TimeSpan(0, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum), 0);
                                ts = ts.Add(addTs);
                                tmInfo.shortTime = string.Format("**:{0}", ts.Minutes.ToString("D2"));
                            }
                        }
                        else
                        {
                            if (min.Contains("*"))
                            {
                                TimeSpan ts = new TimeSpan(Convert.ToInt32(hour),0, 0);
                                TimeSpan addTs = new TimeSpan(0, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum), 0);
                                ts = ts.Add(addTs);
                                tmInfo.shortTime = string.Format("{0}:**", ts.Hours.ToString("D2"));
                            }
                            else
                            {
                                TimeSpan ts = new TimeSpan(Convert.ToInt32(hour), Convert.ToInt32(min), 0);
                                TimeSpan addTs = new TimeSpan(0, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum), 0);
                                ts = ts.Add(addTs);
                                tmInfo.shortTime = string.Format("{0}:{1}", ts.Hours.ToString("D2"), ts.Minutes.ToString("D2"));
                            }
                        }
                        dataGridView1.Rows[id - 1].Cells[6].Value = tmInfo.shortTime;

                    }//if
                    else if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(FileMesege.copyTimer.address) || FileMesege.copyTimer.address == "FFFFFFFF")
                        {
                            continue;
                        }
                        if (!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }
                       
                        tmInfo.address = DgvMesege.addressAdd(FileMesege.copyTimer.address, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        tmInfo.type = IniHelper.findIniTypesByAddress(ip, tmInfo.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", tmInfo.address, ip);
                        if (point != null)
                        {
                            tmInfo.pid = point.pid;
                            tmInfo.type = point.type;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id - 1].Cells[4].Value = point.name;
                        }
                        else
                        {
                            tmInfo.pid = 0;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id - 1].Cells[4].Value = string.Empty;
                        }
                        tmInfo.opt = string.Empty;
                        tmInfo.optName = string.Empty;
                        dataGridView1.Rows[id - 1].Cells[1].Value = IniHelper.findTypesIniNamebyType(tmInfo.type);
                        dataGridView1.Rows[id - 1].Cells[2].Value = DgvMesege.addressTransform(tmInfo.address);
                        dataGridView1.Rows[id - 1].Cells[5].Value = (tmInfo.optName + " " + tmInfo.opt).Trim();
                      
                        ischange = true;

                    }
                    addCount--;
                }
                if (ischange)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }
        }

        //降序
        public void Descending()
        {
            
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            
                //选中行 序号
                int id = 0;
                //列号
                int colIndex = 0;
                //记录第一个选中格的列号
                int FirstColumnIndex = -1;
                //地址减少量
                int reduceCount = 0;
                DataJson.timers tms = DataListHelper.getTimersInfoList();
                if (tms == null)
                {
                    return;
                }
                DataJson.timersInfo tmInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                        tmInfo = DataListHelper.getTimerInfo(tms, id);
                        if (tmInfo == null)
                        {
                            continue;
                        }
                        FileMesege.copyTimer = tmInfo;
                        continue;
                    }
                    if (colIndex == FirstColumnIndex)
                    {
                        reduceCount++;
                    }
                }

                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        continue;
                    }
                    id = dataGridView1.SelectedCells[i].RowIndex + 1;//Convert.ToInt32(].Cells[0].Value);
                    tmInfo = DataListHelper.getTimerInfo(tms, id);
                    if (tmInfo == null)
                    {
                        continue;
                    }
                   
                    if (colIndex == 6)
                    {
                        //延时递减
                        if (string.IsNullOrEmpty(FileMesege.copyTimer.shortTime))
                        {
                            return;
                        }
                        if (reduceCount == 0)
                        {
                            continue;
                        }
                        ischange = true;
                        string hour = FileMesege.copyTimer.shortTime.Split(':')[0];
                        string min = FileMesege.copyTimer.shortTime.Split(':')[1];
                        if (hour.Contains("*"))
                        {
                            if (min.Contains("*"))
                            {
                                tmInfo.shortTime = FileMesege.copyTimer.shortTime;
                            }
                            else
                            {
                                TimeSpan ts = new TimeSpan(24, Convert.ToInt32(min), 0);
                                TimeSpan addTs = new TimeSpan(0, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum), 0);
                                ts = ts.Subtract(addTs);
                                tmInfo.shortTime = string.Format("**:{0}", ts.Minutes.ToString("D2"));
                            }
                        }
                        else
                        {
                            if (min.Contains("*"))
                            {
                                TimeSpan ts = new TimeSpan(Convert.ToInt32(hour), 59, 0);
                                if (ts.Hours == 0)
                                {
                                    ts = new TimeSpan(24, 0, 0);
                                }
                                TimeSpan addTs = new TimeSpan(0, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum), 0);
                                ts = ts.Subtract(addTs);
                                tmInfo.shortTime = string.Format("{0}:**", ts.Hours.ToString("D2"));
                            }
                            else
                            {
                                TimeSpan ts = new TimeSpan(Convert.ToInt32(hour), Convert.ToInt32(min), 0);
                                if (ts.Hours == 0 && ts.Minutes == 0)
                                {
                                    ts = new TimeSpan(23, 60, 0);
                                }
                                TimeSpan addTs = new TimeSpan(0, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum), 0);
                                ts = ts.Subtract(addTs);
                                tmInfo.shortTime = string.Format("{0}:{1}", ts.Hours.ToString("D2"), ts.Minutes.ToString("D2"));
                            }
                        }
                        dataGridView1.Rows[id - 1].Cells[6].Value = tmInfo.shortTime;
                    }//if
                    else if (colIndex == 2)
                    {
                        //地址递减
                        if (string.IsNullOrEmpty(FileMesege.copyTimer.address) || FileMesege.copyTimer.address == "FFFFFFFF")
                        {
                            continue;
                        }
                        if (!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }
                        if (reduceCount == 0)
                        {
                            continue;
                        }
                        tmInfo.address = DgvMesege.addressReduce(FileMesege.copyTimer.address, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        tmInfo.type = IniHelper.findIniTypesByAddress(ip, tmInfo.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", tmInfo.address, ip);
                        if (point != null)
                        {
                            tmInfo.pid = point.pid;
                            tmInfo.type = point.type;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id - 1].Cells[4].Value = point.name;
                        }
                        else
                        {
                            tmInfo.pid = 0;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id - 1].Cells[4].Value = string.Empty;
                        }
                        tmInfo.opt = string.Empty;
                        tmInfo.optName = string.Empty;
                        dataGridView1.Rows[id - 1].Cells[1].Value = IniHelper.findTypesIniNamebyType(tmInfo.type);
                        dataGridView1.Rows[id - 1].Cells[2].Value = DgvMesege.addressTransform(tmInfo.address);
                        dataGridView1.Rows[id - 1].Cells[5].Value = (tmInfo.optName + " " + tmInfo.opt).Trim();
                  
                        ischange = true;

                    }
                    reduceCount--;
                }
                if (ischange)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }
        }

    

        #endregion

        #region 记录滑动条位置
        //滑动条位置
        int X_Value; // Stores position of Horizontal scroll bar
        int Y_Value; // Stores position of Vertical scroll bar
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                X_Value = e.NewValue;
            }

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                Y_Value = e.NewValue;
            }

        }
        #endregion

        



    }
}
