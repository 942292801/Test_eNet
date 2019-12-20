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
using System.Text.RegularExpressions;
using eNet编辑器.Controller;
using System.Net.Sockets;
using System.Threading;


namespace eNet编辑器.DgvView
{
    //public delegate void DgvNameCursorDefault();
    public partial class DgvDevice : Form
    {
        public DgvDevice()
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

        //public event DgvNameCursorDefault dgvNameCursorDefault;

        public event Action unSelectTitleNode;
        public event Action unSelectSectionNode;

        public event Action<string> AppTxtShow;

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

        #region 刷新窗体事件

        public void dgvDeviceAddItem()
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
        /// 网关加载到DGV表格信息
        /// </summary>
        /// <param name="num">树状图索引号</param>
        public void TabIni()
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
                        this.dataGridView1.Rows[index].Cells[6].Style.ForeColor = Color.Red;
                        this.dataGridView1.Rows[index].Cells[6].Value = Resources.DevStateOff;
                        this.dataGridView1.Rows[index].Cells[7].Value = "初始化";
                    }
                    break;
                }//eq.ip == ips
            }//foreach DeviceList表
            this.cbOnline.CheckedChanged -= new System.EventHandler(this.cbOnline_CheckedChanged);
            if (FileMesege.isDgvNameDeviceConnet)
            {
                cbOnline.Checked = true;
                clientConnect();
            }
            else
            {
                cbOnline.Checked = false;
                client = null;
            }
            this.cbOnline.CheckedChanged += new System.EventHandler(this.cbOnline_CheckedChanged);
            DgvMesege.RecoverDgvForm(dataGridView1, X_Value, Y_Value, rowcount, columnCount);

            
        }

        public void clearDgvClear()
        {
            dataGridView1.Rows.Clear();
        }

        #endregion

        #region 鼠标点击DGV表操作
        /// <summary>
        /// 添加dgv表格中地域的信息  
        /// </summary>
        /// <param name="count">行号</param>
        private void sectionAdd(int count)
        {
            try
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
            catch { 
            
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
            
            string title = FileMesege.titleinfo;
            //纯文字title
            string strTitle = Regex.Replace(title, @"[\d]$", "");
            HashSet<int> hasharry = new HashSet<int>();
            string num = "";
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.name != null)
                {
                    //titleinfo与当前数组名相同
                    if (dev.name.Contains(strTitle))
                    {
                        //去除当前名字
                        num = dev.name.Replace(strTitle, "");
                        if (num != "")
                        {
                            hasharry.Add(Convert.ToInt32(num));

                        }
                    }
                    
                }               
                foreach (DataJson.Module md in dev.module)
                {
                    if (md.name != null)
                    {
                        //sts的长度为2 且titleinfo与当前数组名相同 
                        if ( md.name.Contains(strTitle))
                        {
                            //去除当前名字
                            num = md.name.Replace(strTitle, "");
                            if (num != "")
                            {
                                hasharry.Add(Convert.ToInt32(num));

                            }
                        }
                        
                    }//if
                }
            }
            //哈希表 同一个区域的所有序号都在里面
            List<int> arry = hasharry.ToList<int>();
            arry.Sort();
            if (arry.Count == 0)
            {
                //该区域节点前面数字不存在
                return strTitle + "1";
            }
            //哈希表 不存在序号 直接返回
            for (int i = 0; i < arry.Count; i++)
            {
                if (arry[i] != i + 1)
                {
                    return strTitle + (i + 1).ToString();
                }
            }
            return strTitle + (arry[arry.Count - 1] + 1).ToString();
        }

       

        private void devIni()
        {
            if (dataGridView1.Rows[rowcount].Cells[6].Value == null || dataGridView1.Rows[rowcount].Cells[6].Value.ToString() == Resources.DevStateOff)
            {
                return;
            }
            IniForm iniform = new IniForm();
            //把窗口向屏幕中间刷新
            iniform.StartPosition = FormStartPosition.CenterParent;
            iniform.ShowDialog();
            if (iniform.DialogResult == DialogResult.Yes)
            {
                
                //链接ini发信息
                Socket sock = null;
                try
                {
                    string ip = FileMesege.tnselectNode.Text.Split(' ')[0];
                    string dataIP = ip.Split('.')[3];
                    string dataID = dataGridView1.Rows[rowcount].Cells[0].Value.ToString();
                    //发送查询指令  
                    string data = "set;{" + dataIP + ".0." + dataID + ".255:255};acac;\r\n";

                    TcpSocket ts = new TcpSocket();

                    sock = ts.ConnectServer(ip, 6003, 1);

                    ToolsUtil.DelayMilli(1000);
                    if (sock == null)
                    {
                        return ;
                    }


                    int flag = -1;

                    //0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常
                    flag = ts.SendData(sock, data, 1);
                    if (sock != null)
                    {

                        sock.Dispose();
                    }
                    if (flag == 0)
                    {
                        AppTxtShow(string.Format("初始化设备（{0}）成功", dataID));

                    }
                    else
                    {
                        AppTxtShow(string.Format("初始化设备（{0}）失败", dataID));
                    }

                    
                    
                }
                catch (Exception e)
                {
                    AppTxtShow(e.ToString());
                    

                }
            }
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

        int oldrowCount = 0;
        int oldcolumnCount = 0;
        /// <summary>
        /// 选中行索引
        /// </summary>
        private int rowcount = 0;

        /// <summary>
        /// 选中列索引
        /// </summary>
        private int columnCount = 0;

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
                    
                    if (rowcount >= 0)
                    {
                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            case "DeviceState":
                                //弹出操作对话框
                                //dataGridView1.Rows[count].Cells[4].Style.BackColor = Color.Red;
                                break;
                            case "DeviceSection":
                                //添加地域信息
                                sectionAdd(rowcount);
                                break;
                            case "DeviceTitle":
                                //添加名称
                                nameAdd(rowcount);
                                break;
                            case "DeviceIni":
                                //初始化弹框
                                devIni();
                                break;
                            default: break;
                        }
                        try
                        {
                            //更改内容回自动刷新到第一行
                            dataGridView1.CurrentCell = dataGridView1.Rows[rowcount].Cells[columnCount];
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

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            oldrowCount = rowcount;
            oldcolumnCount = columnCount;
            rowcount = e.RowIndex;
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
                if (dataGridView1.SelectedCells.Count == 1 && rowcount == oldrowCount)
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


        #endregion

        #region 按钮刷新处理

        ClientAsync client;

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count;i++ )
                {
                    this.dataGridView1.Rows[i].Cells[6].Style.BackColor = Color.White;
                    this.dataGridView1.Rows[i].Cells[6].Style.ForeColor = Color.Red;
                    this.dataGridView1.Rows[i].Cells[6].Value = Resources.DevStateOff;
                }
             
                clientConnect();
                
            }
            catch
            {
                timer1.Stop();
                client = null;
                return;
            }
        }


        private void cbOnline_CheckedChanged(object sender, EventArgs e)
        {
            if (cbOnline.Checked)
            {
                FileMesege.isDgvNameDeviceConnet = true;
                clientConnect();
                timer1.Start();

            }
            else
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    this.dataGridView1.Rows[i].Cells[6].Style.BackColor = Color.White;
                    this.dataGridView1.Rows[i].Cells[6].Style.ForeColor = Color.Red;
                    this.dataGridView1.Rows[i].Cells[6].Value = Resources.DevStateOff;
                }
                FileMesege.isDgvNameDeviceConnet = false;
                client = null;
                timer1.Stop();
                return;
            }

        }
 

        private void clientConnect()
        {
            try
            {
   
                string strip = FileMesege.tnselectNode.Text.Split(' ')[0];
                if (client != null)
                {
                    client.Dispoes();
                }
   
                client = new ClientAsync();
                ClientIni();
                //异步连接
                client.ConnectAsync(strip, 6001);
                if (client != null && client.Connected())
                {
                    client.SendAsync("read serial.json$");

                }
            }
            catch
            {
                client = null;
                timer1.Stop();
                return;
            }
        }

       


        /// <summary>
        /// 刷新按钮  获取serial设备信息 并处理
        /// </summary>
        private void ClientIni()
        {
            //接收区信息缓存buffer
            string bufferMsg = "";
            string strip = FileMesege.tnselectNode.Text.Split(' ')[0];
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
                try
                {
                    bufferMsg = bufferMsg + msg;
                    if (msg.Length == 1024)
                    {
                        return;
                    }
                 
                    //SocketUtil.WriteLog(bufferMsg);
                    if (bufferMsg.Contains("serial"))
                    {

                        // 序列化接收信息
                        FileMesege.serialList = JsonConvert.DeserializeObject<DataJson.Serial>(bufferMsg);
                        bufferMsg = "";
                        foreach (DataJson.Device dev in FileMesege.DeviceList)
                        {
                            //当IP相等时候进入module里面 
                            if (dev.ip == strip)
                            {

                                foreach (DataJson.serials sl in FileMesege.serialList.serial)
                                {
                                    //获取到的Serial文件在线 的ID 对比dataList信息
                                    foreach (DataJson.Module mdl in dev.module)
                                    {
                                        //当设备的号码相同 名字相同  修改序列号 版本号 状态 
                                        if (sl.id == mdl.id && sl.serial.Trim() == mdl.device)
                                        {
                                            mdl.sn = sl.mac8.Trim().Replace(":", "");
                                            mdl.ver = sl.version.Trim();
                                            changeSn_ver(mdl);
                                            //寻找到该信息就退出当前循环
                                            break;
                                        }

                                    }//for 

                                }

                            }

                        }//for dev


                    }//ifserial
                }
                catch { }
                
            });
          
           
            
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
                if (dataGridView1.Rows[i].Cells[0].Value.ToString()== mdl.id.ToString())
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
                    dataGridView1.Rows[i].Cells[6].Style.ForeColor = Color.Black;
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
                delInfo();
               
             
            
            }
        }

        private void delInfo()
        {
            try
            {
                bool ischange = false;
                string[] parents = FileMesege.tnselectNode.Text.Split(' ');
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    //获取当前选中单元格的行序号
                    int countIndex = dataGridView1.SelectedCells[i].RowIndex;
                    //当粘贴选中单元格为对象和参数

                    foreach (DataJson.Device dev in FileMesege.DeviceList)
                    {
                        //判断IP地址
                        if (dev.ip == parents[0])
                        {

                            if (colIndex == 4)
                            {
                                if (countIndex > 0)
                                {
                                    //为设备
                                    dev.module[countIndex - 1].area1 = "";
                                    dev.module[countIndex - 1].area2 = "";
                                    dev.module[countIndex - 1].area3 = "";
                                    dev.module[countIndex - 1].area4 = "";

                                }
                                else
                                {
                                    //为网关
                                    dev.area1 = "";
                                    dev.area2 = "";
                                    dev.area3 = "";
                                    dev.area4 = "";
                                }
                                this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[4].Value = null;
                            }
                            if (colIndex == 5)
                            {
                                if (countIndex > 0)
                                {
                                    //为设备
                                    dev.module[countIndex - 1].name = "";
                                }
                                else
                                {
                                    //为网关
                                    dev.name = "";
                                }
                                this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = null;
                            }
                        }


                    }

                }//for
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
                //取消选中section和title的树状图
                unSelectTitleNode();
                unSelectSectionNode();
                //鼠标图标变为正常 DGVname DGVdevice也变
                //cursor_default();
                //dgvNameCursorDefault();
            }
            DgvMesege.endDataViewCurrent(dataGridView1, e.Y, e.X);
        }

        bool isClick = false;
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


        #region 复制 粘贴
        /// <summary>
        /// 复制设备的区域和名称
        /// </summary>
        public void copyData()
        {
            //获取当前选中单元格的列序号
            int colIndex = dataGridView1.CurrentRow.Cells.IndexOf(dataGridView1.CurrentCell);
            //当粘贴选中单元格为操作
            if (colIndex == 4 || colIndex == 5)
            {
                string ip = FileMesege.tnselectNode.Text.Split(' ')[0];
                if (dataGridView1.CurrentRow.Cells[0].Value.ToString() == "网关")
                {
                    //修改某IP下某ID 型号的设备信息
                    foreach (DataJson.Device dev in FileMesege.DeviceList)
                    {
                        if (dev.ip == ip)
                        {
                            DataJson.Module md = new DataJson.Module();
                            md.area1 = dev.area1;
                            md.area2 = dev.area2;
                            md.area3 = dev.area3;
                            md.area4 = dev.area4;
                            md.name = dev.name;
                            FileMesege.copyDevice = md;
                            break;
                        }
                    }
                }
                else
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                    DataJson.Module md = DataListHelper.findDeviceByIP_ID(ip, id);
                    if (md == null)
                    {
                        return;
                    }
                    FileMesege.copyDevice = md;
                }
                
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
                string ip = FileMesege.tnselectNode.Text.Split(' ')[0];
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value.ToString() == "网关")
                    {
                        //修改某IP下某ID 型号的设备信息
                        foreach (DataJson.Device dev in FileMesege.DeviceList)
                        {
                            if (dev.ip == ip)
                            {
                                if (colIndex == 4)
                                {
                                    ischange = true;
                                    dev.area1 = FileMesege.copyDevice.area1;
                                    dev.area2 = FileMesege.copyDevice.area2;
                                    dev.area3 = FileMesege.copyDevice.area3;
                                    dev.area4 = FileMesege.copyDevice.area4;
                                    dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[4].Value = string.Format("{0} {1} {2} {3}", dev.area1, dev.area2, dev.area3, dev.area4).Trim();

                                }//if
                                else if (colIndex == 5)
                                {
                                    ischange = true;
                                    dev.name = FileMesege.copyDevice.name;
                                    dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = FileMesege.copyDevice.name;
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                        DataJson.Module md = DataListHelper.findDeviceByIP_ID(ip, id);
                        if (md == null)
                        {
                            return;
                        }
                        if (colIndex == 4)
                        {
                            ischange = true;
                            md.area1 = FileMesege.copyDevice.area1;
                            md.area2 = FileMesege.copyDevice.area2;
                            md.area3 = FileMesege.copyDevice.area3;
                            md.area4 = FileMesege.copyDevice.area4;
                            dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[4].Value = string.Format("{0} {1} {2} {3}", md.area1, md.area2, md.area3, md.area4).Trim();

                        }//if
                        else if (colIndex == 5)
                        {
                            ischange = true;
                            md.name = FileMesege.copyDevice.name;
                            dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = FileMesege.copyDevice.name;
                        }
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

        #region 升序 相同
        public void Same()
        {
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                string ip = FileMesege.tnselectNode.Text.Split(' ')[0];
                int colIndex = 0;
                DataJson.Module md = null;
                for (int i = dataGridView1.SelectedCells.Count-1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (dataGridView1.SelectedCells[i].RowIndex == 0)
                    {
                        //修改某IP下某ID 型号的设备信息
                        foreach (DataJson.Device dev in FileMesege.DeviceList)
                        {
                            if (dev.ip == ip)
                            {
                                if (i == dataGridView1.SelectedCells.Count - 1)
                                {
                                    //获取第一行对象
                                    md = new DataJson.Module();
                                    md.area1 = dev.area1;
                                    md.area2 = dev.area2;
                                    md.area3 = dev.area3;
                                    md.area4 = dev.area4;
                                    md.name = dev.name;
                                    FileMesege.copyDevice = md;
                                    continue;
                                }

                                if (colIndex == 4)
                                {
                                    ischange = true;
                                    dev.area1 = FileMesege.copyDevice.area1;
                                    dev.area2 = FileMesege.copyDevice.area2;
                                    dev.area3 = FileMesege.copyDevice.area3;
                                    dev.area4 = FileMesege.copyDevice.area4;
                                    dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[4].Value = string.Format("{0} {1} {2} {3}", dev.area1, dev.area2, dev.area3, dev.area4).Trim();

                                }//if
                                else if (colIndex == 5)
                                {
                                    ischange = true;
                                    dev.name = FileMesege.copyDevice.name;
                                    dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = FileMesege.copyDevice.name;
                                }

                            }
                        }
                    }
                    else
                    {
                        int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                        md = DataListHelper.findDeviceByIP_ID(ip, id);
                        if (md == null)
                        {
                            return;
                        }
                        if (i == dataGridView1.SelectedCells.Count - 1)
                        {
                            FileMesege.copyDevice = md;
                            continue;
                        }
                        if (colIndex == 4)
                        {
                            ischange = true;
                            md.area1 = FileMesege.copyDevice.area1;
                            md.area2 = FileMesege.copyDevice.area2;
                            md.area3 = FileMesege.copyDevice.area3;
                            md.area4 = FileMesege.copyDevice.area4;
                            dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[4].Value = string.Format("{0} {1} {2} {3}", md.area1, md.area2, md.area3, md.area4).Trim();

                        }//if
                        else if (colIndex == 5)
                        {
                            ischange = true;
                            md.name = FileMesege.copyDevice.name;
                            dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = FileMesege.copyDevice.name;
                        }
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

        public void Ascending()
        {
            return;
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
