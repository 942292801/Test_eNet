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
        }

        void ListBox3DataItemVisualCreated(object sender, DataItemVisualEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)e.Visual;
            item.HotTracking = true;
            
        }

        //加载DgV所有信息
        public void dgvTimerAddItem()
        {

            /*
            this.dataGridView1.Rows.Clear();
            multipleList.Clear();
            if (FileMesege.timerSelectNode == null || FileMesege.timerSelectNode.Parent == null)
            {
                return;
            }
           
                try
                {
                    //选中子节点
                    //循环获取
                    string[] ips = FileMesege.timerSelectNode.Parent.Text.Split(' ');
                    string[] ids = FileMesege.timerSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                    string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sceneSelectNode));//16进制
                    //获取该节点IP地址场景下的 场景信息对象
                    DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
                    if (sc == null)
                    {
                        return;
                    }
                    List<DataJson.sceneInfo> delScene = new List<DataJson.sceneInfo>();
                    //循环加载该场景号的所有信息
                    foreach (DataJson.sceneInfo info in sc.sceneInfo)
                    {
                        int dex = dataGridView1.Rows.Add();

                        if (info.pid == 0)
                        {
                            //pid号为0则为空 按地址来找
                            if (info.address != "" && info.address != "FFFFFFFF")
                            {
                                DataJson.PointInfo point = DataListHelper.findPointByType_address(info.type, ip4 + info.address.Substring(2, 6));
                                if (point != null)
                                {
                                    info.pid = point.pid;
                                    info.address = point.address;
                                    info.type = point.type;
                                    dataGridView1.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                    dataGridView1.Rows[dex].Cells[4].Value = point.name;
                                }
                            }

                        }
                        else
                        {
                            //pid号有效 需要更新address type
                            DataJson.PointInfo point = DataListHelper.findPointByPid(info.pid);
                            if (point == null)
                            {
                                //pid号有无效 删除该场景
                                delScene.Add(info);
                                dataGridView1.Rows.Remove(dataGridView1.Rows[dex]);
                                continue;
                            }
                            else
                            {
                                //pid号有效
                                info.address = point.address;
                                //////////////////////////////////////////////////////争议地域
                                //类型不一致 在value寻找
                                if (info.type != point.type && !string.IsNullOrEmpty(point.value) && !string.IsNullOrEmpty(point.objType))
                                {
                                    //根据value寻找type                        
                                    point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, point.value);
                                }
                                //////////////////////////////////////////////////////到这里
                                if (info.type != point.type || info.type == "")
                                {
                                    //当类型为空时候清空操作
                                    info.opt = "";
                                    info.optName = "";
                                }
                                info.type = point.type;
                                dataGridView1.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                dataGridView1.Rows[dex].Cells[4].Value = point.name;
                            }

                        }
                        dataGridView1.Rows[dex].Cells[0].Value = info.id;
                        dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView1.Rows[dex].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView1.Rows[dex].Cells[5].Value = (info.optName + " " + info.opt).Trim();
                        dataGridView1.Rows[dex].Cells[6].Value = Convert.ToDouble(info.Delay) / 10;
                        dataGridView1.Rows[dex].Cells[7].Value = "删除";


                    }
                    for (int i = 0; i < delScene.Count; i++)
                    {
                        sc.sceneInfo.Remove(delScene[i]);
                    }

                }
                catch (Exception ex)
                {
                    this.dataGridView1.Rows.Clear();
                    MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
                }

           
            */
        }

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
            List<string> tmp = new List<string>();
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
        }

        private void btnDelDay_Click(object sender, EventArgs e)
        {
            if (!cbCustom.Checked)
            {
                return;
            }
        }

        private void listbox_ItemClick(object sender, EventArgs e)
        {
   

                  AppTxtShow(listbox.SelectedItem.ToString());//格式为年月日
        }

        #endregion

  

      

     
    }
}
