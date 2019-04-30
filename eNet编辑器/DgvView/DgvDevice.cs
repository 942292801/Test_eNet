using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using eNet编辑器.Properties;


namespace eNet编辑器.DgvView
{
    //public delegate void DgvNameCursorDefault();
    public partial class DgvDevice : Form
    {
        public DgvDevice()
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

        //public event DgvNameCursorDefault dgvNameCursorDefault;

        public event Action updateSectionTitleNode;
        /// <summary>
        /// 解决窗体闪烁问题
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void DgvDevice_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 网关加载到DGV表格信息
        /// </summary>
        /// <param name="num">树状图索引号</param>
        public void dgvDeviceAddItem()
        {
            this.dataGridView1.Rows.Clear();
            if (FileMesege.tnselectNode == null)
            {
                return;
            }
            string[] ips = FileMesege.tnselectNode.Text.Split(' ');   
            if (ips == null)
            {
                return;
            }
            //封装一个添加DGV信息的函数 打开配置文件自己读
            foreach (DataJson.Device dec in FileMesege.DeviceList)
            {
                if (dec.ip == ips[0])
                {
                    int index = this.dataGridView1.Rows.Add();

                    this.dataGridView1.Rows[index].Cells[4].Value = string.Format("{0} {1} {2} {3}", dec.area1, dec.area2, dec.area3, dec.area4).Trim();
                    this.dataGridView1.Rows[index].Cells[5].Value = dec.name;
                    this.dataGridView1.Rows[index].Cells[0].Value = "网关";
                    this.dataGridView1.Rows[index].Cells[1].Value = dec.master;
                    this.dataGridView1.Rows[index].Cells[2].Value = dec.sn;
                    this.dataGridView1.Rows[index].Cells[3].Value = dec.ver;
                    

                    //添加区域名称和操作
                    foreach (DataJson.Module m in dec.module)
                    {
                        index = this.dataGridView1.Rows.Add();
                        this.dataGridView1.Rows[index].Cells[0].Value = m.id;
                        this.dataGridView1.Rows[index].Cells[1].Value = m.device;
                        this.dataGridView1.Rows[index].Cells[2].Value = m.sn;
                        this.dataGridView1.Rows[index].Cells[3].Value = m.ver;
                        this.dataGridView1.Rows[index].Cells[4].Value = string.Format("{0} {1} {2} {3}", m.area1, m.area2, m.area3, m.area4).Trim();
                        this.dataGridView1.Rows[index].Cells[5].Value = m.name;
                    
                    }
                    break;
                }//eq.ip == ips
            }//foreach DeviceList表
            btnNew_Click(this,EventArgs.Empty);
            
        }


        #region 鼠标点击DGV表操作
        /// <summary>
        /// 添加dgv表格中地域的信息  
        /// </summary>
        /// <param name="count">行号</param>
        private void sectionAdd(int count)
        {
            
            //位置的信息是否为空 
            if (FileMesege.sectionNode != null)
            {
                //区域信息 选中节点非空
                if (FileMesege.sectionNode != null)
                {
                    //devices 里面ini的名字
                    //string keyVal = "";
                    string path = Application.StartupPath + "\\devices\\";
                    string[] parents = FileMesege.tnselectNode.Text.Split(' ');
                    //把选中节点保存至文档中 
                    TreeMesege tm = new TreeMesege();
                    string[] sections = tm.GetSectionByNode(FileMesege.sectionNode).Split('\\');
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();

                    foreach (DataJson.Device dev in FileMesege.DeviceList)
                    {
                        //判断IP地址
                        if (dev.ip == parents[0])
                        {
                            dataGridView1.Rows[count].Cells[4].Value = FileMesege.sectionNode.FullPath.Replace("\\", " ");
                            if (count > 0)
                            {
                                
                                //为设备
                                dev.module[count - 1].area1 = sections[0];
                                dev.module[count - 1].area2 = sections[1];
                                dev.module[count - 1].area3 = sections[2];
                                dev.module[count - 1].area4 = sections[3];
                                /*  暂时屏蔽  做Bind面板的时候再考虑
                                //判断设备是否为key面板 是就添加设备信息
                                keyVal = IniConfig.GetValue(path + dataGridView1.Rows[count].Cells[1].Value + ".ini", "input", "key");
                                if (keyVal != "null")
                                {

                                    //获取IP最后一位
                                    string IP = SocketUtil.GetIPstyle(parents[0], 4);
                                    //获取10进制的设备ID号
                                    string idstr = dataGridView1.Rows[count].Cells[0].Value.ToString(); //Regex.Replace(devs[0], @"[^\d]*", "");
                                    //十六进制的ID号
                                    string ID = SocketUtil.strtohexstr(idstr);
                                    string address = IP + "00" + ID + "00";
                                    TreeMesege tm = new TreeMesege();
                                    //获取id1 - id4
                                    string[] tmID = tm.GetSectionId(FileMesege.sectionNode.FullPath.Split('\\'));
                                    foreach (DataJson.PointInfo e in FileMesege.PointList.equipment)
                                    {
                                        //循环判断 NameList中是否存在该节点
                                        if (address == e.address && e.ip == parents[0])
                                        {
                                            e.id1 = tmID[0];
                                            e.id2 = tmID[1];
                                            e.id3 = tmID[2];
                                            e.id4 = tmID[3];
                                            //存在
                                            //更新NameList里面的类型信息 
                                            e.objType = "10.0_key";
                                            return;

                                        }

                                    }
                                    //不存在 插入新信息
                                    DataJson.Equipment eq = new DataJson.Equipment();
                                    eq.id1 = tmID[0];
                                    eq.id2 = tmID[1];
                                    eq.id3 = tmID[2];
                                    eq.id4 = tmID[3];
                                    eq.ip = parents[0];
                                    eq.address = address;
                                    eq.objType = "10.0_key";
                                    FileMesege.MapperList.equipment.Add(eq);
                                }*/
                                
                            }
                            else
                            {
                                //为网关
                                dev.area1 = sections[0];
                                dev.area2 = sections[1];
                                dev.area3 = sections[2];
                                dev.area4 = sections[3];
                                //dev.name = 
                            }

                            //撤销
                            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                            FileMesege.cmds.DoNewCommand(NewList, OldList);
                            
                        }
                    }

                    //FileMesege.DeviceList
                }


            }
           
           
        }

        /// <summary>
        /// 添加dgv表格中名称的信息
        /// </summary>
        /// <param name="count">行号</param>
        private void nameAdd(int count)
        {
            
                //名字信息 选中节点非空
                if (FileMesege.titleinfo != "")
                {
                    //devices 里面ini的名字
                    //string keyVal = "";
                    string path = Application.StartupPath + "\\devices\\";
                    string[] parents = FileMesege.tnselectNode.Text.Split(' ');
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();

                    foreach (DataJson.Device dev in FileMesege.DeviceList)
                    {
                        //判断IP地址
                        if (dev.ip == parents[0])
                        {
                            //添加名称   序号排序
                            string name = infoindex();
                            dataGridView1.Rows[count].Cells[5].Value = name;
                            if (count > 0)
                            {
                                //为设备
                                dev.module[count - 1].name = name;
                                /** 暂时屏蔽 做Bind面板的时候再考虑
                                //判断设备是否为key面板 是就添加设备信息
                                keyVal = IniConfig.GetValue(path + dataGridView1.Rows[count].Cells[1].Value + ".ini", "input", "key");
                                if (keyVal != "null")
                                {
                                    //获取IP最后一位
                                    string IP = SocketUtil.GetIPstyle(parents[0], 4);
                                    //获取10进制的设备ID号
                                    string idstr = dataGridView1.Rows[count].Cells[0].Value.ToString(); //Regex.Replace(devs[0], @"[^\d]*", "");
                                    //十六进制的ID号
                                    string ID = SocketUtil.strtohexstr(idstr);
                                    string address = IP + "00" + ID + "00";
                                    //TreeMesege tm = new TreeMesege();
                                    //获取id1 - id4
                                    //string[] tmID = tm.GetSectionId(FileMesege.sectionNode.FullPath.Split('\\'));
                                    foreach (DataJson.Equipment e in FileMesege.MapperList.equipment)
                                    {
                                        //循环判断 NameList中是否存在该节点
                                        if (address == e.address && e.ip == parents[0])
                                        {
                                            e.name = name;

                                            return;
                                        }
                                    }
                                    DataJson.Equipment eq = new DataJson.Equipment();
                                    eq.name = name;
                                    eq.ip = parents[0];
                                    eq.address = address;
                                    FileMesege.MapperList.equipment.Add(eq);
                                }**/
                            }
                            else
                            {
                                //为网关
                                dev.name = name;
                            }
                            //撤销
                            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                            FileMesege.cmds.DoNewCommand(NewList, OldList);
                            
                            return;
                        }
                        

                    }
                }


        }

        /// <summary>
        /// 比较泛型表中名字一样 并按顺序补缺 例如12345 删除3  后面自动补3
        /// </summary>
        /// <returns>返回新的值</returns>
        private string infoindex()
        {
            
            //string [] strs=null;
            string title = FileMesege.titleinfo;
            string num = "";
            int count = 0;//判断当前是否有匹配
            int old =0 , now = 0;
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.name != null)
                {
                    //titleinfo与当前数组名相同
                    if (dev.name.Contains(title))
                    {
                        count++;
                        //去除当前名字
                        num = dev.name.Replace(title, "");
                        if (num != "" )
                        {
                            now = Convert.ToInt32(num);
                            if (now > old)
                            {
                                old = now;
                            }
                                                            
                        }
                    }
                    
                }               
                foreach (DataJson.Module md in dev.module)
                {
                    if (md.name != null)
                    {
                        //strs = md.location.Split('/');
                        //表示当前有name
     
                            //sts的长度为2 且titleinfo与当前数组名相同 
                            if ( md.name.Contains(title))
                            {
                                count++;
                                //去除当前名字
                                num = md.name.Replace(title, "");
                                if (num != "")
                                {
                                    now = Convert.ToInt32(num);
                                    if (now > old)
                                    {
                                        old = now;
                                    }

                                }
                            }
                        
                    }//if
                }
            }
            if (count == 0)
            {
                return title;
            }
            
            return title + (old+1).ToString();
        }

   

        //编辑离开
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int count = e.RowIndex;
                string[] parents = FileMesege.tnselectNode.Text.Split(' ');
                dataGridView1.Columns[5].ReadOnly = true;
                dataGridView1.Columns[4].ReadOnly = true;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                foreach (DataJson.Device dev in FileMesege.DeviceList)
                {
                    //判断IP地址
                    if (dev.ip == parents[0])
                    {

                        string name = "";
                        //当前编辑行 区域不为空
                        if (dataGridView1.Rows[count].Cells[5].Value != null)
                        {
                            name = dataGridView1.Rows[count].Cells[5].EditedFormattedValue.ToString();
                        }

                        if (count > 0)
                        {
                            //为设备
                            dev.module[count - 1].name = name;
                        }
                        else
                        {
                            //为网关
                            dev.name = name;
                        }
                        //撤销

                        DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                        FileMesege.cmds.DoNewCommand(NewList, OldList);
                        return;
                    }


                }
            }
            catch { }
            
        }

        

        private bool isFirstClick = true;
        private bool isDoubleClick = false;
        private int milliseconds = 0;

        /// <summary>
        /// 选中行索引
        /// </summary>
        private int count = 0;
        /// <summary>
        /// 选中列索引
        /// </summary>
        private int ind = 0;

        private void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            milliseconds += 100;
            // 第二次鼠标点击超出双击事件间隔
            if (milliseconds >= SystemInformation.DoubleClickTime)
            {
                doubleClickTimer.Stop();


                if (isDoubleClick)
                {
                    //处理双击事件操作
                    //可以编辑第五项
                    dataGridView1.Columns[5].ReadOnly = false;
                    
                }
                else
                {

                    //DGV的行号
                    
                    if (count >= 0)
                    {
                        switch (dataGridView1.Columns[ind].Name)
                        {
                            case "DeviceState":
                                //弹出操作对话框
                                //dataGridView1.Rows[count].Cells[4].Style.BackColor = Color.Red;
                                break;
                            case "DeviceSection":
                                //添加地域信息
                                sectionAdd(count);
                                break;
                            case "DeviceTitle":
                                //添加名称
                                nameAdd(count);
                                break;
                            default: break;
                        }
                    }

                   
                }
                isFirstClick = true;
                isDoubleClick = false;
                milliseconds = 0;
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            count = e.RowIndex;
            ind = e.ColumnIndex;
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
           
        }


        #endregion

        #region 按钮刷新处理

        ClientAsync client;
        /// <summary>
        /// 刷新在线按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (client != null && client.Connected())
                {
                    client.Dispoes();
                }

                //清空在线标志 
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Cells[6].Value = Resources.DevStateOff;
                }
                client = new ClientAsync();
                selectHandle();
            }
            catch
            {
                client = null;
                return;
            }
        }

        /// <summary>
        /// 刷新按钮  获取serial设备信息 并处理
        /// </summary>
        private void selectHandle()
        {
            
            string[] strip = FileMesege.tnselectNode.Text.Split(' ');
            client.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync.EnSocketAction>((c, enAction) =>
            {
                 string key = "";

                try
                {
                    if ( c.Client.Connected)
                    {
                        IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                        key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                    }
                }
                catch {}
                switch (enAction)
                {
                    case ClientAsync.EnSocketAction.Connect:
                       // MessageBox.Show("已经与" + key + "建立连接");
                        break;
                    case ClientAsync.EnSocketAction.SendMsg:

                        //MessageBox.Show(DateTime.Now + "：向" + key + "发送了一条消息");
                        break;
                    case ClientAsync.EnSocketAction.Close:
                        
                        //MessageBox.Show("服务端连接关闭");
                        break;
                    case ClientAsync.EnSocketAction.Error:

                        //MessageBox.Show("连接发生错误,请检查网络连接");

                        break;
                    default:
                        break;
                }
            });
            //信息接收处理
            client.Received += new Action<string, string>((key, msg) =>
            {
                if (msg.Contains("serial"))
                {
                   // 序列化接收信息
                    FileMesege.serialList = JsonConvert.DeserializeObject<DataJson.Serial>(msg);
                    
                    foreach (DataJson.Device dev in FileMesege.DeviceList)
                    {
                        //当IP相等时候进入module里面 
                        if(dev.ip ==  strip[0] )
                        {

                            foreach (DataJson.serials sl in FileMesege.serialList.serial)
                            {
                                //获取到的Serial文件在线 的ID 对比dataList信息
                                foreach (DataJson.Module mdl in dev.module)
                                {
                                    //当设备的号码相同 名字相同  修改序列号 版本号 状态 
                                    if ( sl.id.ToString() == mdl.id && sl.serial.Trim()== mdl.device)
                                    {
                                        mdl.sn = sl.mac8.Trim().Replace(":","");
                                        mdl.ver = sl.version.Trim();
                                        changeSn_ver(mdl);
                                        //寻找到该信息就退出当前循环
                                        break;
                                    }

                                }//for 
                                
                            }
                    
                        }

                    }//for dev

                    
                    
                }//if msg 
                
            });
            //异步连接
            client.ConnectAsync(strip[0], 6001);
            if (client != null && client.Connected())
            {
                client.SendAsync("read serial.json$");
            }
           
            
        }

        /// <summary>
        /// 修改表格中的sn ver 和 状态
        /// </summary>
        /// <param name="mdl"></param>
        private void changeSn_ver(DataJson.Module mdl)
        {
            //循环DGV表格
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //找到该ID 退出循环
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == mdl.id)
                {
                    if (dataGridView1.Rows[i].Cells[2].Value != null && dataGridView1.Rows[i].Cells[3].Value != null)
                    {
                        if (mdl.sn != dataGridView1.Rows[i].Cells[2].Value.ToString() || dataGridView1.Rows[i].Cells[3].Value.ToString() != mdl.ver)
                        {
                            dataGridView1.Rows[i].Cells[2].Style.ForeColor = Color.Red;
                            dataGridView1.Rows[i].Cells[3].Style.ForeColor = Color.Red;
                        }
                    }
                    
                    dataGridView1.Rows[i].Cells[2].Value = mdl.sn;
                    dataGridView1.Rows[i].Cells[3].Value = mdl.ver;
                    dataGridView1.Rows[i].Cells[6].Value = Resources.DevStateOn;
                    dataGridView1.Rows[i].Cells[6].Style.BackColor = Color.Lime;
                    break;
                }
                

            }
        }

        #endregion

        #region Del按键处理  
        /// <summary>
        /// 删除按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {

                if (FileMesege.tnselectNode == null)
                {
                    return;
                }
                switch (dataGridView1.CurrentCell.ColumnIndex)
                { 
                    case 4:
                        //区域 清空区域
                        DelKeySection(dataGridView1.CurrentCell.RowIndex);
                        break;
                    case 5:
                        //名称 清空名称
                        DelKeyName(dataGridView1.CurrentCell.RowIndex);
                        break;
                    default: return;
                }
                dataGridView1.CurrentCell.Value = null;
            
            }
        }

        //删除名称信息
        private void DelKeyName(int count)
        {
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            string[] parents = FileMesege.tnselectNode.Text.Split(' ');
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                //判断IP地址
                if (dev.ip == parents[0])
                {
                    if (count > 0)
                    {
                        //为设备
                        dev.module[count - 1].name= "";
                    }
                    else
                    {
                        //为网关
                        dev.name = "";
                    }
                    //撤销

                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    return;
                }


            }
        }

        //删除区域信息
        private void DelKeySection(int count)
        {
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            string[] parents = FileMesege.tnselectNode.Text.Split(' ');
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                //判断IP地址
                if (dev.ip == parents[0])
                {
                    if (count > 0)
                    {
                        //为设备
                        dev.module[count - 1].area1 = "";
                        dev.module[count - 1].area2 = "";
                        dev.module[count - 1].area3 = "";
                        dev.module[count - 1].area4 = "";

                    }
                    else
                    {
                        //为网关
                        dev.area1 = "";
                        dev.area2 = "";
                        dev.area3 = "";
                        dev.area4 = "";
                    }
                    //撤销
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    return;
                }


            }
        }
        #endregion


        #region 鼠标右击 和鼠标图标更改

        /// <summary>
        /// 右击鼠标清位置信息与名字信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //treesection临时存放数据处
                FileMesege.sectionNode = null;
                //treetitle名字临时存放
                FileMesege.titleinfo = "";
                //更新section和title的树状图
                updateSectionTitleNode();
                //鼠标图标变为正常 DGVname DGVdevice也变
                //cursor_default();
                //dgvNameCursorDefault();
            }
        }


        /// <summary>
        /// 鼠标图标更改为正常图标
        /// </summary>
        public void cursor_default()
        {

            dataGridView1.Cursor = Cursors.Default;
        }
       
        /// <summary>
        /// 鼠标图标更改为正常图标
        /// </summary>
        public void cursor_copy()
        {
           
            //定义图片
            Bitmap a = (Bitmap)Bitmap.FromFile(Application.StartupPath + "\\cursor32.png");
            //定义加载到那个控件
            DgvMesege.SetCursor(a, new Point(0, 0), dataGridView1);
            
        }
        #endregion

       

      

    }
}
