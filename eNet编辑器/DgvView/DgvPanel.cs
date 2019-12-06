using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Threading;
using eNet编辑器.AddForm;
using System.Reflection;
using Newtonsoft.Json;

namespace eNet编辑器.DgvView
{
    public partial class DgvPanel : Form
    {
        public DgvPanel()
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
        DataGridViewComboBoxColumn objType ;
        DataGridViewComboBoxColumn mode ;
   
        DataGridViewComboBoxColumn showmode;

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> AppTxtShow;

        public event Action updateSectionTitleNode;
        /// <summary>
        /// 传输point点跳转窗口
        /// </summary>
        public event Action<DataJson.PointInfo> jumpSetInfo;

        string ip = "";

        private void DgvBind_Load(object sender, EventArgs e)
        {

            //新增对象列 加载
            objType = new DataGridViewComboBoxColumn();
            mode = new DataGridViewComboBoxColumn();
    
            showmode = new DataGridViewComboBoxColumn();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string name = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                name = IniConfig.GetValue(file.FullName, "define", "name");
                if (name != "")
                {
                    objType.Items.Add(name);
                }
            }
            showmode.Items.Add("无");
            showmode.Items.Add("同步");
            showmode.Items.Add("反显");
            showmode.Items.Add("图形按键");
            showmode.Items.Add("图形滑动条");
            showmode.Items.Add("图形图标");
            showmode.Items.Add("图形数值1");
            showmode.Items.Add("图形数值0.1");
            showmode.Items.Add("图形数值0.01");
            showmode.Items.Add("图形数值0.001");
            showmode.Items.Add("图形数值0.0001");
            //设置列名
            objType.HeaderText = "类型";
            //设置下拉列表的默认值 
            objType.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            objType.DefaultCellStyle.NullValue = objType.Items[0];
            objType.ReadOnly = true;
            objType.Name = "objType";

            //设置列名
            mode.HeaderText = "操作";
            //设置下拉列表的默认值 
            mode.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            mode.Width = 130;
            mode.ReadOnly = true;
            //或者这样设置 默认选择第一项
            //mode.DefaultCellStyle.NullValue = null;
            mode.Name = "operation";

            //设置列名
            showmode.HeaderText = "显示模式";
            //设置下拉列表的默认值 
            showmode.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            showmode.DefaultCellStyle.NullValue = showmode.Items[0];
            showmode.Name = "showMode";

            //插入执行对象类型
            this.dataGridView1.Columns.Insert(3, objType);
            //插入执行模式
            this.dataGridView1.Columns.Insert(6, mode);
            //插入显示模式
            this.dataGridView1.Columns.Insert(8, showmode);
            this.dataGridView1.Columns[8].ReadOnly = true;
            
        }


        #region 刷新窗体事件

        public void dgvPanelAddItem()
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
        /// 加载DgV所有信息
        /// </summary>
        public void TabIni()
        {
            
            try
            {
                this.dataGridView1.Rows.Clear();
                cbKeyNum.SelectedIndex = 0;
                imgSlider.ImageList = null;
                cbPage.SelectedIndex = 0;
                cbDevNum.Items.Clear();
                cbDevNum.Text = "";
                int tmpId = -1;
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null || pls.keyNum ==0)
                {
                    return;
                }
                ip = FileMesege.panelSelectNode.Parent.Text.Split(' ')[0];//IP地址
                cbKeyNum.Text = pls.keyNum.ToString();
                //面板图片
                if (pls.keyNum <= 8)
                {
                    DevExpress.Utils.ImageCollection[] imags = { imgList1, imgList2, imgList3, imgList4, imgList5, imgList6, imgList7, imgList8 };
                    imgSlider.ImageList = imags[pls.keyNum - 1];

                }
                else
                {
                    imgSlider.ImageList = null;
                }
                findKeyPanel();
                List<DataJson.panelsInfo> delPanel = new List<DataJson.panelsInfo>();
                
                //循环加载该定时号的所有信息
                foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
                {
                    //新增行号
                    int dex = dataGridView1.Rows.Add();
                    if (plInfo.pid == 0)
                    {
                        //pid号为0则为空 按地址来找
                        if (plInfo.objAddress != "" && plInfo.objAddress != "FFFFFFFF")
                        {
                            DataJson.PointInfo point = DataListHelper.findPointByType_address(plInfo.objType,plInfo.objAddress,ip);
                            if (point != null)
                            {
                                plInfo.pid = point.pid;
                                plInfo.objAddress = point.address;
                                plInfo.objType = point.type;
                                dataGridView1.Rows[dex].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                dataGridView1.Rows[dex].Cells[5].Value = point.name;
                            }
                        }
                    }
                    else
                    {
                        //pid号有效 需要更新address type
                        DataJson.PointInfo point = DataListHelper.findPointByPid(plInfo.pid);
                        if (point == null)
                        {
                            //pid号有无效 删除该场景
                            delPanel.Add(plInfo);
                            dataGridView1.Rows.Remove(dataGridView1.Rows[dex]);
                            continue;
                        }
                        else
                        {
                            //pid号有效
                          
                            try
                            {
                                if (plInfo.objAddress.Substring(2, 6) != point.address.Substring(2, 6))
                                {
                                    plInfo.objAddress = point.address;

                                }
                            }
                            catch
                            {
                                plInfo.objAddress = point.address;
                            }
                            //////////////////////////////////////////////////////争议地域
                            //类型不一致 在value寻找
                            if (plInfo.objType != point.type && !string.IsNullOrEmpty(point.value) && !string.IsNullOrEmpty(point.objType))
                            {
                                //根据value寻找type                        
                                point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, point.value);
                            }
                            //////////////////////////////////////////////////////到这里
                            if (plInfo.objType != point.type || plInfo.objType == "")
                            {
                                //当类型为空时候清空操作
                                plInfo.opt = 0;
                                //tmInfo.optName = "";
                            }
                            plInfo.objType = point.type;
                            dataGridView1.Rows[dex].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[dex].Cells[5].Value = point.name;
                        }

                    }
                    if (tmpId == plInfo.id)
                    {
                        plInfo.showAddress = "";
                        plInfo.showMode = "";
                        //同上的
                        dataGridView1.Rows[dex].Cells[0].Value = null;
                        dataGridView1.Rows[dex].Cells[10].ReadOnly = true;
                    }
                    else
                    {
                        dataGridView1.Rows[dex].Cells[0].Value = plInfo.id;
                        dataGridView1.Rows[dex].Cells[10].Value = "添加";
                    }
                    tmpId = plInfo.id;
                    
                    dataGridView1.Rows[dex].Cells[1].Value = keyAddressTransform(plInfo.keyAddress);
                    dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(plInfo.objAddress);
                    dataGridView1.Rows[dex].Cells[3].Value = IniHelper.findTypesIniNamebyType(plInfo.objType);
                    //执行模式  (操作)
                    dataGridView1.Rows[dex].Cells[6].Value = updataMode(plInfo.objType, dex,plInfo.opt);
                    dataGridView1.Rows[dex].Cells[7].Value = DgvMesege.addressTransform(plInfo.showAddress);
                    dataGridView1.Rows[dex].Cells[8].Value = plInfo.showMode;
                    dataGridView1.Rows[dex].Cells[9].Value = "删除";
                    


                }
                for(int i = 0; i < delPanel.Count; i++)
                {
                    pls.panelsInfo.Remove(delPanel[i]);
                }
                DgvMesege.RecoverDgvForm(dataGridView1, X_Value, Y_Value, rowCount, columnCount);
            }
            catch (Exception ex)
            {
                this.dataGridView1.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }
            
        }

        public void clearDgvClear()
        {
            dataGridView1.Rows.Clear();
        }

        #endregion

        #region 工具类

        /// <summary>
        /// 寻找有key的panel
        /// </summary>
        private void findKeyPanel()
        {
           
            //devices 里面ini的名字
            string keyVal = "";
            string path = Application.StartupPath + "\\devices\\";
            //从设备加载网关信息
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                if (d.ip == ip)
                {
                    //加载设备
                    foreach (DataJson.Module m in d.module)
                    {
                        keyVal = IniConfig.GetValue(path + m.device + ".ini", "input", "key");
                        if (keyVal != "null")
                        {
                            cbDevNum.Items.Add(m.id);

                        }
                    }
                }

            }
        }

        /// <summary>
        /// bindinfo信息按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void panelInfoSort(DataJson.panels pls)
        {
            pls.panelsInfo.Sort(delegate(DataJson.panelsInfo x, DataJson.panelsInfo y)
            {
       
                return x.id.CompareTo(y.id);
            });
           
        }


       /// <summary>
        /// 更改combox的items信息   操作可以为空
       /// </summary>
       /// <param name="type">类型</param>
       /// <param name="rownum">行号</param>
       /// <param name="opt">操作</param>
       /// <returns></returns>
        private string updataMode(string type, int rownum,int opt)
        {
            //敲黑板 重点  重新在某一列上添加combox
            DataGridViewComboBoxCell combox = dataGridView1.Rows[rownum].Cells["operation"] as DataGridViewComboBoxCell;
            combox.Items.Clear();
            //修改mode的combox列表
            string filepath = string.Format("{0}\\types\\{1}.ini", Application.StartupPath, type);
            //获取ini所有keyMode的Key  opt根据TYPE决定
            List<string> keys = IniConfig.ReadKeys("keyMode", filepath);
            if (keys == null)
            {
                return "";
            }
            for (int i = 0; i < keys.Count; i++)
            {
                string tmp = IniConfig.GetValue(filepath, "keyMode", keys[i]);
                combox.Items.Add(tmp);
            }
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] == opt.ToString())
                {
                    return IniConfig.GetValue(filepath, "keyMode", keys[i]);
                }
            }
            return "";
        }


        /// <summary>
        /// 地址转换
        /// </summary>
        /// <returns></returns>
        public static string keyAddressTransform(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return "";
            }
            //string ip = Convert.ToInt32(address.Substring(0, 2), 16).ToString();
            string ID = Convert.ToInt32(address.Substring(2, 2), 16).ToString();
            string num =( Convert.ToInt32(address.Substring(4, 2), 16)* 256 + Convert.ToInt32(address.Substring(6, 2), 16)).ToString();

            return string.Format("{0}.{1}", ID, num);
        }

   

        #endregion

        #region  清空  清除 开启 关闭 下载 删除
        //清空
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                pls.panelsInfo.Clear();
                for (int i = 1; i <= pls.keyNum; i++)
                {
                    //添加改id的按键
                    DataJson.panelsInfo plInfo = new DataJson.panelsInfo();
                    plInfo.id = i;
                    plInfo.pid = 0;
                    plInfo.keyAddress = "";
                    plInfo.objAddress = "";
                    plInfo.objType = "";
                    plInfo.opt = 0;
                    plInfo.showAddress = "";
                    plInfo.showMode = "";
                    pls.panelsInfo.Add(plInfo);
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvPanelAddItem();
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

       

        //载入
        private void btnDown_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            try
            {
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                //面板信息不为空
                if (pls.panelsInfo.Count > 0)
                {
                    DataJson.Kn kn = new DataJson.Kn();
                    kn.key = new List<DataJson.Keynumber>();

                    //把有效的对象操作 放到kN对象里面
                    foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
                    {
                        //确保有信息
                        if (string.IsNullOrEmpty(plInfo.keyAddress) || string.IsNullOrEmpty(plInfo.objAddress) || plInfo.opt == 0 || string.IsNullOrEmpty(plInfo.objType))
                        {
                            continue;
                        }

                        DataJson.Keynumber keyInfo = new DataJson.Keynumber();
                        keyInfo.num = plInfo.id;
                        keyInfo.key = "00"+ plInfo.keyAddress.Substring(2,6);
                        keyInfo.obj = plInfo.objAddress;
                        keyInfo.mode = plInfo.opt;
                        if (string.IsNullOrEmpty(plInfo.showAddress))
                        {
                            keyInfo.fback = "00000000";
                        }
                        else
                        {
                            keyInfo.fback = "FE" + plInfo.showAddress.Substring(2, 6);
                        }
                        //显示模式
                        keyInfo.fbmode = FileMesege.getShowMode(plInfo.showMode);
                        
                        kn.key.Add(keyInfo);
                       
                       

                    }
                    if (kn.key.Count > 0)
                    {
                        //序列化SN对象
                        string sjson = JsonConvert.SerializeObject(kn);

                        //写入数据格式
                        string path = "down /json/k" + pls.id + ".json$" + sjson;
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
                        AppTxtShow("加载失败！无面板指令！");
                    }

                }
                else
                {
                    AppTxtShow("加载失败！无面板指令！");
                }

            }
            catch
            {
                //Exception ex
                //TxtShow("加载失败！\r\n");
            }
        }

        

        //下载
        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        //开启
        private void btnOn_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.panelSelectNode == null || FileMesege.panelSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string[] ids = FileMesege.panelSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = ToolsUtil.getIP(FileMesege.panelSelectNode);
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


                string oder = String.Format("SET;00000001;{{{0}.48.{1}}};\r\n", ip4, number);  // "SET;00000001;{" + ip4 + ".16." + number + "};\r\n";
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

        //关闭
        private void btnOff_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.panelSelectNode == null || FileMesege.panelSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string[] ids = FileMesege.panelSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = ToolsUtil.getIP(FileMesege.panelSelectNode);
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


                string oder = String.Format("SET;00000000;{{{0}.48.{1}}};\r\n", ip4, number);  // "SET;00000001;{" + ip4 + ".16." + number + "};\r\n";
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

        //删除
        private void btnDel_Click(object sender, EventArgs e)
        {
            clearPanelInfo();

        }

        /// <summary>
        /// 清除主机k#.json信息
        /// </summary>
        private void clearPanelInfo()
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.panelSelectNode == null || FileMesege.panelSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string[] ids = FileMesege.panelSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = ToolsUtil.getIP(FileMesege.panelSelectNode);
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


                string oder = String.Format("SET;00000005;{{{0}.48.{1}}};\r\n", ip4, number);  // "SET;00000001;{" + ip4 + ".16." + number + "};\r\n";
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

        #endregion


        #region 表格操作
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
            if (e.Button == MouseButtons.Right)
            {
                //清除title的节点
                updateSectionTitleNode();

            }
            DgvMesege.endDataViewCurrent(dataGridView1, e.Y, e.X);
        }

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
                        string add = "";
                        string objType = "";
                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            case "keyAddress":
                                //键地址
                                if (dataGridView1.Rows[rowCount].Cells[1].Value != null)
                                {
                                    //原地址
                                    add = dataGridView1.Rows[rowCount].Cells[1].Value.ToString();
                                }
                                dgvKeyAddress(add);
                            
                                break;
                            case "objAddress":
                                //改变地址
                                
                                if (dataGridView1.Rows[rowCount].Cells[2].Value != null)
                                {
                                    //原地址
                                    add = dataGridView1.Rows[rowCount].Cells[2].Value.ToString();
                                }
                                objType = dataGridView1.Rows[rowCount].Cells[3].EditedFormattedValue.ToString();
                                //赋值List 并添加地域 名字
                                dgvObjAddress( objType, add);
                             
                                break;
                       
                            case "showAddress":
                                //显示地址
                                if (dataGridView1.Rows[rowCount].Cells[7].Value != null)
                                {
                                    //原地址
                                    add = dataGridView1.Rows[rowCount].Cells[7].Value.ToString();
                                }
                                //赋值List 并添加地域 名字
                                dgvShowAddress( add);
                           
                                break;
                            case "del":
                                //删除
                                dgvDel();
                                break;
                            case "add":
                                //添加
                                addInfo();
                        
                                break;
                            case "id":
                                //设置对象跳转
                                DataJson.PointInfo point = dgvJumpSet();
                                if (point != null)
                                {
                                    //传输到form窗口控制
                                    //AppTxtShow("传输到主窗口"+DateTime.Now);
                                    jumpSetInfo(point);
                                }
                                break;
                            case "operation":
                                //操作
                                mode.ReadOnly = false;
                                break;

                            case "showMode":
                                this.dataGridView1.Columns[8].ReadOnly = false;
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
                                dgvDel();

                                break;
                            case "add":
                                //添加
                                addInfo();
                              
                                break;
                            case "objAddress":
                                setTitleObjAddress();
                                break;
                            case "objType":
                                setTitleObjAddress();
                                break;
                            case "section":
                                setTitleObjAddress();
                                break;
                            case "name":
                                setTitleObjAddress();
                                break;
                            case "showAddress":
                                if (dataGridView1.Rows[rowCount].Cells[10].Value == null)
                                {
                                    dgvShowTxt();
                                    break;
                                }
                                setTitleShowAddress();
                                dgvShowTxt();
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

        /// <summary>
        /// 更新mode的items项 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    case "operation":
                        //操作 更新
                        updataOpt( rowNum);

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
                        case "objType":
                            //改变对象  
                            string isChange = dgvObjtype(rowNum);
                            if (!string.IsNullOrEmpty(isChange))
                            {
                                dataGridView1.Rows[rowNum].Cells[3].Value = IniHelper.findTypesIniNamebyType(isChange);
                            }
                            break;
                        case "operation":
                            //操作
                            dgvOperation(rowNum);
                            mode.ReadOnly = true;
                            break;
                           
                        case "showMode":
                            this.dataGridView1.Columns[8].ReadOnly = true;
                            dgvShowMode(rowNum);
                           
                            break;

                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

 
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

       


        /// <summary>
        /// 对象跳转获取 场景 定时 编组 point点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataJson.PointInfo dgvJumpSet()
        {
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return null;
            }
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.panelsInfo plInfo = pls.panelsInfo[rowCount];
            if (plInfo == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(plInfo.objAddress))
            {
                return null;
            }
            if (plInfo.objType == "4.0_scene" || plInfo.objType == "5.0_timer" || plInfo.objType == "6.1_panel" || plInfo.objType == "6.2_sensor")
            {
    
                return DataListHelper.findPointByType_address(plInfo.objType, plInfo.objAddress, ip);
            }

            return null;

        }


        /// <summary>
        /// 添加新面板
        /// </summary>
        private void addInfo()
        {
             if (dataGridView1.Rows[rowCount].Cells[10].Value == null)
            {
                return;
            }
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return ;
            }
            
            int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
            int opt = 0;
            string objType = "", keyAddress = "", objAddress = "";
            //计算往哪个位置插入
            int inserNum = 0;
            bool isbreak = false;
            foreach (DataJson.panelsInfo find in pls.panelsInfo)
            {
                inserNum++;
                if (find.id == id)
                {
                    //if (!string.IsNullOrEmpty(find.keyAddress))
                    //{
                        objType = find.objType;
                        keyAddress = find.keyAddress;
                        objAddress = find.objAddress;
                        opt = find.opt;
                    //}

                }
                if (find.id == id + 1)
                {
                    isbreak = true;
                    break;
                }

            }
            if (!isbreak && pls.panelsInfo.Count == inserNum)
            {
                inserNum++;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //添加改id的按键
            DataJson.panelsInfo plInfo = new DataJson.panelsInfo();
            //地址加一处理 并搜索PointList表获取地址 信息
            if (!string.IsNullOrEmpty(objAddress) && objAddress != "FFFFFFFF")
            {
                switch (objAddress.Substring(2, 2))
                {
                    case "00":
                        //设备类地址
                        objAddress = objAddress.Substring(0, 6) + ToolsUtil.strtohexstr((Convert.ToInt32(objAddress.Substring(6, 2), 16) + 1).ToString());
                        break;
                    default:
                        string hexnum = ToolsUtil.strtohexstr((Convert.ToInt32(objAddress.Substring(4, 4), 16) + 1).ToString());
                        while (hexnum.Length < 4)
                        {
                            hexnum = hexnum.Insert(0, "0");
                        }
                        objAddress = objAddress.Substring(0, 4) + hexnum;
                        break;
                }
                //按照地址查找type的类型 
                objType = IniHelper.findIniTypesByAddress(ip, objAddress).Split(',')[0];
                plInfo.objType = objType;
                //添加地域和名称 在sceneInfo表中
                DataJson.PointInfo point = DataListHelper.findPointByType_address(objType, objAddress,ip);
                if (point != null)
                {
                    plInfo.pid = point.pid;
                    plInfo.objType = objType;
                    if (plInfo.objType != point.type)
                    {
                        //清空执行模式（操作）
                        plInfo.opt = 0;
                    }
                }
                else
                {
                    plInfo.pid = 0;
                    plInfo.opt = 0;
                }
            }
            plInfo.id = id;
            plInfo.keyAddress = keyAddress;
            plInfo.objAddress = objAddress;
            plInfo.showAddress = "";
            plInfo.showMode = "";
            pls.panelsInfo.Insert(inserNum-1 , plInfo);
            
            //排序
            //panelInfoSort(pls);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvPanelAddItem();
        }

        /// <summary>
        /// 删除 或者清空
        /// </summary>
        private void dgvDel()
        {
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return;
            }
            
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (dataGridView1.Rows[rowCount].Cells[10].Value == null)
            {
                //直接删除
                pls.panelsInfo.Remove( pls.panelsInfo[rowCount]);
                dataGridView1.Rows.Remove(dataGridView1.Rows[rowCount]);
            }
            else
            { 
                //清空当条信息
                pls.panelsInfo[rowCount].keyAddress = "";
                pls.panelsInfo[rowCount].objAddress = "";
                pls.panelsInfo[rowCount].objType = "";
                pls.panelsInfo[rowCount].opt = 0;
                pls.panelsInfo[rowCount].pid = 0;
                pls.panelsInfo[rowCount].showAddress = "";
                pls.panelsInfo[rowCount].showMode = "";
                
                dataGridView1.Rows[rowCount].Cells[1].Value = null;
                dataGridView1.Rows[rowCount].Cells[2].Value = null;
                dataGridView1.Rows[rowCount].Cells[3].Value = "";
                dataGridView1.Rows[rowCount].Cells[4].Value = null;
                dataGridView1.Rows[rowCount].Cells[5].Value = null;
                dataGridView1.Rows[rowCount].Cells[6].Value = "";
                dataGridView1.Rows[rowCount].Cells[7].Value = null;
                dataGridView1.Rows[rowCount].Cells[8].Value = "";
            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            
        }

        /// <summary>
        /// 按键地址选择
        /// </summary>
        private void dgvKeyAddress(string address)
        {
            if (dataGridView1.Rows[rowCount].Cells[10].Value == null)
            {
                return;
            }
            panelCheck plc = new panelCheck();
            //把窗口向屏幕中间刷新
            plc.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            plc.Address = address;
            plc.ShowDialog();
            if (plc.DialogResult == DialogResult.OK)
            {
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                int id = Convert.ToInt32( dataGridView1.Rows[rowCount].Cells[0].Value);
                foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
                {
                    if (plInfo.id == id)
                    {
                        plInfo.keyAddress = plc.RtAddress;
                    }
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            
                dgvPanelAddItem();
            }
        }

        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="obj">当前对象的地址</param>
        /// <returns></returns>
        private void dgvObjAddress(string objType, string obj)
        {
            sceneAddress dc = new sceneAddress();
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            //dc.Obj = obj;
            dc.ObjType = objType;
            dc.Obj = obj;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //地址
                pls.panelsInfo[rowCount].objAddress = dc.Obj;
                if (dataGridView1.Rows[rowCount].Cells[0].Value != null)
                {
                    pls.panelsInfo[rowCount].showAddress = dc.Obj;
                    pls.panelsInfo[rowCount].showMode = "同步";
                }
                
                if (string.IsNullOrEmpty(dc.Obj))
                {
                    return;
                }
                //按照地址查找type的类型 只限制于设备
                string type = IniHelper.findIniTypesByAddress(ip, dc.Obj).Split(',')[0];
                if (string.IsNullOrEmpty(type))
                {
                    type = dc.RtType;

                }
                pls.panelsInfo[rowCount].objType = type;
                //获取树状图的IP第四位  + Address地址的 后六位
                string ad =  dc.Obj;

                //区域加名称
                DataJson.PointInfo point = DataListHelper.findPointByType_address("", ad, ip);

                if (point != null)
                {
                    pls.panelsInfo[rowCount].pid = point.pid;
                    pls.panelsInfo[rowCount].objType = point.type;
                    if (pls.panelsInfo[rowCount].objType != point.type)
                    {
                        pls.panelsInfo[rowCount].opt = 0;
                    }

                }
                else
                {
                    //搜索一次dev表 
                    pls.panelsInfo[rowCount].pid = 0;
                    //info.type = dc.ObjType;
                    pls.panelsInfo[rowCount].opt = 0;
                  
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            dgvPanelAddItem();


        }

        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="obj">当前对象的地址</param>
        /// <returns></returns>
        private void dgvShowAddress( string obj)
        {
            if (dataGridView1.Rows[rowCount].Cells[10].Value == null)
            {
                return;
            }
            sceneAddress dc = new sceneAddress();
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return;
            }
            //按照地址查找type的类型 只限制于设备
            string type = IniHelper.findIniTypesByAddress(ip, pls.panelsInfo[rowCount].showAddress).Split(',')[0];
            if (string.IsNullOrEmpty(type))
            {
                //区域加名称
                DataJson.PointInfo point = DataListHelper.findPointByType_address(type, pls.panelsInfo[rowCount].showAddress,ip);
                if (point == null)
                {
                    dc.ObjType = "";
                }
                else
                {
                    dc.ObjType = IniHelper.findTypesIniNamebyType(point.type);
                }
            }
            else
            {
                dc.ObjType = IniHelper.findTypesIniNamebyType(type);
            }
            dc.Obj = obj;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {

                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //地址
                pls.panelsInfo[rowCount].showAddress = dc.Obj;
                if (string.IsNullOrEmpty(dc.Obj))
                {
                    return;
                }
           
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            dataGridView1.Rows[rowCount].Cells[7].Value = DgvMesege.addressTransform(pls.panelsInfo[rowCount].showAddress);
           


        }

        /// <summary>
        /// 展示该节点的信息到txt中
        /// </summary>
        private void dgvShowTxt()
        {
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return;
            }
            //区域加名称
            DataJson.PointInfo point = DataListHelper.findPointByType_address("", pls.panelsInfo[rowCount].showAddress, ip);
            if (point == null)
            {
                return ;
            }
            string section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
            AppTxtShow(string.Format("显示地址信息 ：{0} {1} ", section , point.name));//改根据地址从信息里面获取
        }

        /// <summary>
        /// 修改DGV表类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private string dgvObjtype(int id)
        {

            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return null;
            }


            if (pls.panelsInfo[id].pid != 0)
            {
                DataJson.PointInfo point = DataListHelper.findPointByPid(pls.panelsInfo[id].pid);
                if (point.type != "")
                {

                    return point.type;
                }

            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            pls.panelsInfo[id].objType = IniHelper.findTypesIniTypebyName(dataGridView1.Rows[id].Cells[3].EditedFormattedValue.ToString());
            pls.panelsInfo[id].opt = 0;

            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return null;
        }

        //tmp 临时存放该类型的地址
        string filepath = "";
        /// <summary>
        /// 更改combox的items信息  
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="rownum">行号</param>
        /// <param name="opt">操作</param>
        /// <returns></returns>
        private void updataOpt(int rownum)
        {
            try
            {
                //敲黑板 重点  重新在某一列上添加combox
                DataGridViewComboBoxCell combox = dataGridView1.Rows[rownum].Cells["operation"] as DataGridViewComboBoxCell;
                combox.Items.Clear();
                string type = IniHelper.findTypesIniTypebyName(dataGridView1.Rows[rownum].Cells[3].Value.ToString());
                if (string.IsNullOrEmpty(type))
                {
                    return;
                }
                //修改mode的combox列表
                filepath = string.Format("{0}\\types\\{1}.ini", Application.StartupPath, type);
                //获取ini所有keyMode的Key  opt根据TYPE决定
                List<string> keys = IniConfig.ReadKeys("keyMode", filepath);
                if (keys == null)
                {
                    return;
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    string tmp = IniConfig.GetValue(filepath, "keyMode", keys[i]);
                    combox.Items.Add(tmp);
                }
            }
            catch 
            {
            }
        }

        
        /// <summary>
        /// 修改dgv下的执行模式 （操作）
        /// </summary>
        /// <param name="id"></param>
        private void dgvOperation(int id)
        {
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                dataGridView1.Rows[rowCount].Cells[7].ToolTipText = null;
                return;
            }
            List<string> keys = IniConfig.ReadKeys("keyMode", filepath);
            if (keys == null)
            {
                return;
            }
            string opt = dataGridView1.Rows[id].Cells[6].EditedFormattedValue.ToString();
            for (int i = 0; i < keys.Count; i++)
            {
                if(opt ==  IniConfig.GetValue(filepath, "keyMode", keys[i]))
                {
                    //撤销 
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    pls.panelsInfo[id].opt = Convert.ToInt32(keys[i]);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    
                    return;
                }
            }
        }

        /// <summary>
        /// 设置显示模式 无 同步 反显
        /// </summary>
        private void dgvShowMode(int id)
        {
            if (dataGridView1.Rows[id].Cells[10].Value == null)
            {
                dataGridView1.Rows[id].Cells[8].Value = "";
                return;
            }
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            string mode = dataGridView1.Rows[id].Cells[8].EditedFormattedValue.ToString();
            pls.panelsInfo[id].showMode = mode;
            if (mode != "无")
            {
                if (!string.IsNullOrEmpty(pls.panelsInfo[id].objAddress))
                {
                    //对象地址不为空进行复制
                    pls.panelsInfo[id].showAddress = pls.panelsInfo[id].objAddress;
                    dataGridView1.Rows[id].Cells[7].Value = dataGridView1.Rows[id].Cells[2].Value;
                }

            }
            else
            {
                //对象地址不为空进行复制
                pls.panelsInfo[id].showAddress = "";
                dataGridView1.Rows[id].Cells[7].Value = "";
            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            

        }

        /// <summary>
        /// 复制title选中的节点 赋地址给ObjAddress
        /// </summary>
        private void setTitleObjAddress()
        {
            try
            {
                int colIndex = dataGridView1.SelectedCells[0].ColumnIndex;
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                int id = dataGridView1.CurrentCell.RowIndex;
                //获取sceneInfo对象表中对应ID号info对象
                DataJson.panelsInfo plInfo = pls.panelsInfo[id];
                List<string> section_name = DataListHelper.dealPointInfo(FileMesege.titlePointSection);

                DataJson.PointInfo eq = DataListHelper.findPointBySection_name(section_name);
                if (eq == null)
                {
                    return;
                }
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();

                if (eq.type == plInfo.objType)
                {
                    plInfo.pid = eq.pid;

                    plInfo.objAddress = eq.address;


                    dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(plInfo.objAddress);
                    dataGridView1.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView1.Rows[id].Cells[5].Value = eq.name;
                    if (eq.address != "FFFFFFFF" && dataGridView1.Rows[id].Cells[0].Value != null)
                    {

                        plInfo.showAddress = eq.address;
                        plInfo.showMode = "同步";
                        dataGridView1.Rows[id].Cells[7].Value = dataGridView1.Rows[id].Cells[2].Value;
                        dataGridView1.Rows[id].Cells[8].Value = "同步";


                    }
                }
                else
                {
                    plInfo.pid = eq.pid;


                    plInfo.objAddress = eq.address;

                    plInfo.objType = eq.type;
                    plInfo.opt = 0;

                    dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(plInfo.objAddress);
                    dataGridView1.Rows[id].Cells[3].Value = IniHelper.findTypesIniNamebyType(plInfo.objType);
                    dataGridView1.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView1.Rows[id].Cells[5].Value = eq.name;
                    dataGridView1.Rows[id].Cells[6].Value = updataMode(plInfo.objType, id, plInfo.opt);
                    if (eq.address != "FFFFFFFF" && dataGridView1.Rows[id].Cells[0].Value != null)
                    {

                        plInfo.showAddress = eq.address;
                        plInfo.showMode = "同步";
                        dataGridView1.Rows[id].Cells[7].Value = dataGridView1.Rows[id].Cells[2].Value;
                        dataGridView1.Rows[id].Cells[8].Value = "同步";
                    }

                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            catch { 
            
            }
       
        }

        /// <summary>
        /// 复制title选中的节点 赋地址给ShowAddress
        /// </summary>
        private void setTitleShowAddress()
        {

            //int colIndex = dataGridView1.SelectedCells[0].ColumnIndex;

            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return;
            }
            int id = dataGridView1.CurrentCell.RowIndex;
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.panelsInfo plInfo = pls.panelsInfo[id];
            List<string> section_name = DataListHelper.dealPointInfo(FileMesege.titlePointSection);

            DataJson.PointInfo eq = DataListHelper.findPointBySection_name(section_name);
            if (eq == null)
            {
                return;
            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
    
           
                plInfo.showAddress = eq.address;
                dataGridView1.Rows[id].Cells[7].Value = DgvMesege.addressTransform(plInfo.showAddress);
            

            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);


        }


        #endregion


        #region 按键号 页号 面板设备号

        /// <summary>
        /// 设置 按键号 页号 面板设备号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAttr_Click(object sender, EventArgs e)
        {
            try
            {
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                bool isSetKey = setKeyNum(pls, Convert.ToInt32(cbKeyNum.Text));
                bool isSetNum = setDevNum(pls);
                if (isSetKey || isSetNum)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    dgvPanelAddItem();
                
                }
                
            }
            catch
            {

            }
            
        }


        /// <summary>
        /// 设置当前面板为 几键面板 成功返回true
        /// </summary>
        /// <param name="keyNum"></param>
        /// <returns></returns>
        private bool setKeyNum(DataJson.panels pls,int keyNum)
        {
            if (string.IsNullOrEmpty(cbKeyNum.Text))
            {
                imgSlider.ImageList = null;
                return false;
            }
            
            if (keyNum > 255 || keyNum <= 0)
            {
                return false;
            }
            if (pls.keyNum == keyNum)
            {
                return false;
            }
            pls.keyNum = keyNum;
            List<DataJson.panelsInfo> delPanel = new List<DataJson.panelsInfo>();
            HashSet<int> numList = new HashSet<int>();
            foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
            {
                if (plInfo.id > keyNum)
                {
                    delPanel.Add(plInfo);
                }
                numList.Add(plInfo.id);
            }
            for (int i = 0; i < delPanel.Count; i++)
            {
                //删除id大于面板按键值
                pls.panelsInfo.Remove(delPanel[i]);
            }
            List<int> list = numList.ToList<int>();
            bool isExit = false;
            for (int i = 1; i <= keyNum; i++)
            {
                isExit = false;
                for(int j = 0;j< list.Count;j++)
                {
                    if (i == list[j])
                    {
                        isExit = true;
                        break;
                    }

                }
                if (!isExit)
                { 
                    //添加改id的按键
                    DataJson.panelsInfo plInfo = new DataJson.panelsInfo();
                    plInfo.id = i;
                    plInfo.pid = 0;
                    plInfo.keyAddress = "";
                    plInfo.objAddress = "";
                    plInfo.objType = "";
                    plInfo.opt = 0;
                    plInfo.showAddress = "";
                    plInfo.showMode = "";
                    pls.panelsInfo.Add(plInfo);
                    //排序
                    panelInfoSort(pls);
                }
            }
            
            return true;
        }

        /// <summary>
        /// 设备号更改
        /// </summary>
        private bool setDevNum(DataJson.panels pls)
        {
            if (string.IsNullOrEmpty(cbDevNum.Text) || string.IsNullOrEmpty(cbPage.Text))
            {
                return false;
            }
            
            string ip = ToolsUtil.strtohexstr(ToolsUtil.getIP(FileMesege.panelSelectNode));
            string id = ToolsUtil.strtohexstr(cbDevNum.Text);
            string tmpNum = "";
            int start = Convert.ToInt32(cbPage.Text) * 255 + Convert.ToInt32(cbPage.Text);
            foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
            {
                
                //非设备类
                tmpNum = ToolsUtil.strtohexstr((start+plInfo.id).ToString());
                while (tmpNum.Length < 4)
                {
                    tmpNum = tmpNum.Insert(0, "0");
                }
                plInfo.keyAddress = string.Format("{0}{1}{2}", ip, id, tmpNum);
            }
            return true;
        }



        #endregion


        #region del删除快捷键

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
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    DataJson.panelsInfo plInfo = pls.panelsInfo[dataGridView1.SelectedCells[i].RowIndex];
                    if (plInfo == null)
                    {
                        continue;
                    }
                    if (colIndex == 6)
                    {

                        ischange = true;
                        plInfo.opt = 0;

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[6].Value = null;
                    }//if
                    if (colIndex == 7)
                    {

                        ischange = true;
                        plInfo.showAddress = "";

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[7].Value = null;
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



        #region 复制 粘贴
        /// <summary>
        /// 复制点位的对象 与参数 
        /// </summary>
        public void copyData()
        {
            //获取当前选中单元格的列序号
            int colIndex = dataGridView1.CurrentRow.Cells.IndexOf(dataGridView1.CurrentCell);
            //当粘贴选中单元格为操作
            if (colIndex == 6 || colIndex == 7)
            {
                int id = dataGridView1.CurrentRow.Index;
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }

                //获取sceneInfo对象表中对应ID号info对象
                DataJson.panelsInfo plInfo = pls.panelsInfo[id];
                if (plInfo == null)
                {
                    return;
                }
                //获取sceneInfo对象表中对应ID号info对象
                FileMesege.copyPanel = plInfo;

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
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    int id = dataGridView1.SelectedCells[i].RowIndex;
                    DataJson.panelsInfo plInfo = pls.panelsInfo[id];
                    if (plInfo == null)
                    {
                        return;
                    }

                    if (FileMesege.copyPanel.objType == "" || plInfo.objType == "" || plInfo.objType != FileMesege.copyPanel.objType)
                    {
                        continue;
                    }
                    if (colIndex == 6)
                    {

                        ischange = true;
                        plInfo.opt = FileMesege.copyPanel.opt;

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[6].Value = updataMode(plInfo.objType, id, plInfo.opt);
                    }//if
                    else if (colIndex == 7)
                    {
                        ischange = true;
                        plInfo.showAddress = FileMesege.copyPanel.showAddress;

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[7].Value = DgvMesege.addressTransform(plInfo.showAddress);
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
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                DataJson.panelsInfo plInfo = null;

                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作 第一个选中格的列号 
                        continue;
                    }
                    id = dataGridView1.SelectedCells[i].RowIndex;
                    plInfo = pls.panelsInfo[id];
                    if (plInfo == null)
                    {
                        return;
                    }
                    if (i == dataGridView1.SelectedCells.Count - 1)
                    {
                        //记录第一个选中格内容
                        FirstColumnIndex = colIndex;
                        FileMesege.copyPanel = plInfo;
                        continue;
                    }
                    //当粘贴选中单元格为操作
                    if (colIndex == 6)
                    {

                        //获取sceneInfo对象表中对应ID号info对象
                        if (string.IsNullOrEmpty(FileMesege.copyPanel.objType) || string.IsNullOrEmpty(plInfo.objType) || plInfo.objType != FileMesege.copyPanel.objType)
                        {
                            //类型不一致 并且为空
                            continue;
                        }
                        ischange = true;
                        plInfo.opt = FileMesege.copyPanel.opt;
                        dataGridView1.Rows[id].Cells[6].Value = updataMode(plInfo.objType, id, plInfo.opt);
                    }//if
                    else if (colIndex == 2)
                    {
                        //选中单元格为地址
                        plInfo.objAddress = FileMesege.copyPanel.objAddress;
                        if (FileMesege.copyPanel.objType != plInfo.objType)
                        {
                            //类型不一样清空类型
                            plInfo.opt = 0;
                          
                        }
                        plInfo.objType = FileMesege.copyPanel.objType;
                        plInfo.pid = FileMesege.copyPanel.pid;

                        if (dataGridView1.Rows[id].Cells[0].Value != null)
                        {
                            plInfo.showAddress = FileMesege.copyPanel.showAddress;
                            plInfo.showMode = FileMesege.copyPanel.showMode;
                        }
                        else
                        {
                            plInfo.showAddress = "";
                            plInfo.showMode = "";
                        }

                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByPid(plInfo.pid);
                        if (point != null)
                        {
                            plInfo.objType = point.type;
                            dataGridView1.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id].Cells[5].Value = point.name;
                        }
                        else
                        {
                            plInfo.pid = 0;
                            dataGridView1.Rows[id].Cells[4].Value = string.Empty;
                            dataGridView1.Rows[id].Cells[5].Value = string.Empty;
                        }
                        dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(plInfo.objAddress);
                        dataGridView1.Rows[id].Cells[3].Value = IniHelper.findTypesIniNamebyType(plInfo.objType);
                        dataGridView1.Rows[id].Cells[6].Value = updataMode(plInfo.objType, id, plInfo.opt);
                        dataGridView1.Rows[id].Cells[7].Value = DgvMesege.addressTransform(plInfo.showAddress);
                        dataGridView1.Rows[id].Cells[8].Value = plInfo.showMode;

                        ischange = true;

                    }
                    else if (colIndex == 8)
                    {
                        if (dataGridView1.Rows[id].Cells[0].Value != null)
                        {
                            if (string.IsNullOrEmpty(FileMesege.copyPanel.showAddress))
                            {
                                //无 mode
                                plInfo.showAddress = FileMesege.copyPanel.showAddress;
                                plInfo.showMode = FileMesege.copyPanel.showMode;
                         
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(plInfo.showAddress))
                                {
                                    //同步 地址为空 
                                    plInfo.showAddress = plInfo.objAddress;
                                    plInfo.showMode = FileMesege.copyPanel.showMode;
                                    
                                }
                                else
                                {
                                    plInfo.showMode = FileMesege.copyPanel.showMode;
                                }
 
                            }
                           

                        }
                        dataGridView1.Rows[id].Cells[7].Value = DgvMesege.addressTransform(plInfo.showAddress);
                        dataGridView1.Rows[id].Cells[8].Value = plInfo.showMode;
                        ischange = true;
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
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                DataJson.panelsInfo plInfo = null;
                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = dataGridView1.SelectedCells[i].RowIndex;
                        plInfo = pls.panelsInfo[id];
                        if (plInfo == null)
                        {
                            return;
                        }
                        FileMesege.copyPanel = plInfo;
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
                    id = dataGridView1.SelectedCells[i].RowIndex;
                    plInfo = pls.panelsInfo[id];
                    if (plInfo == null)
                    {
                        continue;
                    }
                    if (addCount == 0)
                    {
                        continue;
                    }
                    if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(FileMesege.copyPanel.objAddress) || FileMesege.copyPanel.objAddress == "FFFFFFFF")
                        {
                            continue;
                        }
                        if (!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }

                        plInfo.objAddress = DgvMesege.addressAdd(FileMesege.copyPanel.objAddress, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        plInfo.objType = IniHelper.findIniTypesByAddress(ip, plInfo.objAddress).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", plInfo.objAddress, ip);
                        if (point != null)
                        {
                            plInfo.pid = point.pid;
                            plInfo.objType = point.type;
                            dataGridView1.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id].Cells[5].Value = point.name;
                        }
                        else
                        {
                            plInfo.pid = 0;
                            dataGridView1.Rows[id].Cells[4].Value = string.Empty;
                            dataGridView1.Rows[id].Cells[5].Value = string.Empty;
                        }

                        //同步
                        if (dataGridView1.Rows[id].Cells[0].Value != null)
                        {
                            plInfo.showAddress = plInfo.objAddress;
                            plInfo.showMode ="同步";
                        }
                        else
                        {
                            plInfo.showAddress = "";
                            plInfo.showMode = "";
                        }
                        plInfo.opt = 0;
                        dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(plInfo.objAddress);
                        dataGridView1.Rows[id].Cells[3].Value = IniHelper.findTypesIniNamebyType(plInfo.objType);
                        dataGridView1.Rows[id].Cells[6].Value = updataMode(plInfo.objType, id, plInfo.opt);
                        dataGridView1.Rows[id].Cells[7].Value = DgvMesege.addressTransform(plInfo.showAddress);
                        dataGridView1.Rows[id].Cells[8].Value = plInfo.showMode;
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
                //地址添加量
                int reduceCount = 0;
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                DataJson.panelsInfo plInfo = null;
                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = dataGridView1.SelectedCells[i].RowIndex;
                        plInfo = pls.panelsInfo[id];
                        if (plInfo == null)
                        {
                            return;
                        }
                        FileMesege.copyPanel = plInfo;
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
                    id = dataGridView1.SelectedCells[i].RowIndex;
                    plInfo = pls.panelsInfo[id];
                    if (plInfo == null)
                    {
                        continue;
                    }
                    if (reduceCount == 0)
                    {
                        continue;
                    }
                    if (colIndex == 2)
                    {
                        //地址递减
                        if (string.IsNullOrEmpty(FileMesege.copyPanel.objAddress) || FileMesege.copyPanel.objAddress == "FFFFFFFF")
                        {
                            continue;
                        }
                        if (!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }

                        plInfo.objAddress = DgvMesege.addressReduce(FileMesege.copyPanel.objAddress, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        plInfo.objType = IniHelper.findIniTypesByAddress(ip, plInfo.objAddress).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", plInfo.objAddress, ip);
                        if (point != null)
                        {
                            plInfo.pid = point.pid;
                            plInfo.objType = point.type;
                            dataGridView1.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id].Cells[5].Value = point.name;
                        }
                        else
                        {
                            plInfo.pid = 0;
                            dataGridView1.Rows[id].Cells[4].Value = string.Empty;
                            dataGridView1.Rows[id].Cells[5].Value = string.Empty;
                        }

                        //同步
                        if (dataGridView1.Rows[id].Cells[0].Value != null && !string.IsNullOrEmpty(plInfo.objAddress))
                        {
                            plInfo.showAddress = plInfo.objAddress;
                            plInfo.showMode = "同步";
                        }
                        else
                        {
                            plInfo.showAddress = "";
                            plInfo.showMode = "";
                        }
                        plInfo.opt = 0;
                        dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(plInfo.objAddress);
                        dataGridView1.Rows[id].Cells[3].Value = IniHelper.findTypesIniNamebyType(plInfo.objType);
                        dataGridView1.Rows[id].Cells[6].Value = updataMode(plInfo.objType, id, plInfo.opt);
                        dataGridView1.Rows[id].Cells[7].Value = DgvMesege.addressTransform(plInfo.showAddress);
                        dataGridView1.Rows[id].Cells[8].Value = plInfo.showMode;
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
