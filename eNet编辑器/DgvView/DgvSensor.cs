using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using eNet编辑器.AddForm;

namespace eNet编辑器.DgvView
{
    public partial class DgvSensor : Form
    {
        public DgvSensor()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
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

        public event Action updateSectionTitleNode;

        DataGridViewComboBoxColumn showmode;

        DataGridViewComboBoxColumn objType;
        /// <summary>
        /// 传输point点跳转窗口
        /// </summary>
        public event Action<DataJson.PointInfo> jumpSetInfo;

        private void DgvSensor_Load(object sender, EventArgs e)
        {
            //新增对象列 加载
            objType = new DataGridViewComboBoxColumn();
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

            //设置列名
            objType.HeaderText = "类型";
            //设置下拉列表的默认值 
            objType.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            objType.DefaultCellStyle.NullValue = objType.Items[0];
            objType.ReadOnly = true;
            objType.Name = "objType";

            showmode.Items.Add("闭合");
            showmode.Items.Add("释放");
            //设置列名
            showmode.HeaderText = "输入状态";
            //设置下拉列表的默认值 
            showmode.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            showmode.DefaultCellStyle.NullValue = showmode.Items[0];
            showmode.Name = "fbMode";

            this.dataGridView1.Columns.Insert(2, showmode);
            //插入执行对象类型
            this.dataGridView1.Columns.Insert(4, objType);
        }


        /// <summary>
        /// 加载DgV所有信息
        /// </summary>
        public void dgvSensorAddItem()
        {
            try
            {
                
                cbDevNum.Items.Clear();
                cbDevNum.Text = "";
                this.dataGridView1.Rows.Clear();
                this.cbIONum.TextChanged -= new System.EventHandler(this.cbIoNum_TextChanged);
                cbIONum.Text = "";
                this.cbIONum.TextChanged += new System.EventHandler(this.cbIoNum_TextChanged);
                int tmpId = -1;
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    cbIONum.Text = "";
                    return;
                }
                if (srs.sensorsInfo.Count == 0)
                {
                    //新建面板 不存在信息则让 cbIoNumCHnageText事件加载
                    cbIONum.Text = srs.ioNum.ToString();
                    return;
                }
                else
                {
                    this.cbIONum.TextChanged -= new System.EventHandler(this.cbIoNum_TextChanged);
                    cbIONum.Text = srs.ioNum.ToString();
                    this.cbIONum.TextChanged += new System.EventHandler(this.cbIoNum_TextChanged);
                }
                
           
                findKeyPanel();
                List<DataJson.sensorsInfo> delSensor = new List<DataJson.sensorsInfo>();
                string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sensorSelectNode));//16进制
                //循环加载该定时号的所有信息
                foreach (DataJson.sensorsInfo srInfo in srs.sensorsInfo)
                {
                    //新增行号
                    int dex = dataGridView1.Rows.Add();
                    if (srInfo.pid == 0)
                    {
                        //pid号为0则为空 按地址来找
                        if (srInfo.objAddress != "" && srInfo.objAddress != "FFFFFFFF")
                        {
                            DataJson.PointInfo point = DataListHelper.findPointByType_address(srInfo.objType, ip4 + srInfo.objAddress.Substring(2, 6));
                            if (point != null)
                            {
                                srInfo.pid = point.pid;
                                srInfo.objAddress = point.address;
                                srInfo.objType = point.type;
                                dataGridView1.Rows[dex].Cells[5].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                dataGridView1.Rows[dex].Cells[6].Value = point.name;
                            }
                        }
                    }
                    else
                    {
                        //pid号有效 需要更新address type
                        DataJson.PointInfo point = DataListHelper.findPointByPid(srInfo.pid);
                        if (point == null)
                        {
                            //pid号有无效 删除该感应
                            delSensor.Add(srInfo);
                            dataGridView1.Rows.Remove(dataGridView1.Rows[dex]);
                            continue;
                        }
                        else
                        {
                            //pid号有效
                            srInfo.objAddress = point.address;
                            //////////////////////////////////////////////////////争议地域
                            //类型不一致 在value寻找
                            if (srInfo.objType != point.type && !string.IsNullOrEmpty(point.value) && !string.IsNullOrEmpty(point.objType))
                            {
                                //根据value寻找type                        
                                point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, point.value);
                            }
                            //////////////////////////////////////////////////////到这里
                            if (srInfo.objType != point.type || srInfo.objType == "")
                            {
                                //当类型为空时候清空操作
                                srInfo.opt = "";
                                srInfo.optName = "";
                            }
                            srInfo.objType = point.type;
                            dataGridView1.Rows[dex].Cells[5].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[dex].Cells[6].Value = point.name;
                        }

                    }
                    if (tmpId == srInfo.id)
                    {
                        //同上的
                        dataGridView1.Rows[dex].Cells[0].Value = null;
                        dataGridView1.Rows[dex].Cells[9].ReadOnly = true;
                    }
                    else
                    {
                        dataGridView1.Rows[dex].Cells[0].Value = srInfo.id;
                        dataGridView1.Rows[dex].Cells[9].Value = "添加";
                    }
                    tmpId = srInfo.id;

                    dataGridView1.Rows[dex].Cells[1].Value = keyAddressTransform(srInfo.keyAddress);
                    dataGridView1.Rows[dex].Cells[2].Value = findFbMode( srInfo.fbmode);//闭合 释放
                    dataGridView1.Rows[dex].Cells[3].Value = DgvMesege.addressTransform(srInfo.objAddress);
                    dataGridView1.Rows[dex].Cells[4].Value = IniHelper.findTypesIniNamebyType(srInfo.objType);
                    // (操作)
                    dataGridView1.Rows[dex].Cells[7].Value = (srInfo.optName + " " + srInfo.opt).Trim();
                 
                    dataGridView1.Rows[dex].Cells[8].Value = "删除";



                }
                for (int i = 0; i < delSensor.Count; i++)
                {
                    srs.sensorsInfo.Remove(delSensor[i]);
                }

            }
            catch (Exception ex)
            {
                this.dataGridView1.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }
        }


        #region 工具类

        /// <summary>
        /// 寻找有io的panel
        /// </summary>
        private void findKeyPanel()
        {

            //devices 里面ini的名字
            string keyVal = "";
            string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
            string path = Application.StartupPath + "\\devices\\";
            //从设备加载网关信息
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                if (d.ip == ip)
                {
                    //加载设备
                    foreach (DataJson.Module m in d.module)
                    {
                        keyVal = IniConfig.GetValue(path + m.device + ".ini", "input", "io");
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
        private void sensorInfoSort(DataJson.sensors srs)
        {
            srs.sensorsInfo.Sort(delegate(DataJson.sensorsInfo x, DataJson.sensorsInfo y)
            {

                return x.id.CompareTo(y.id);
            });

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
            string num = (Convert.ToInt32(address.Substring(4, 2), 16) * 256 + Convert.ToInt32(address.Substring(6, 2), 16)).ToString();

            return string.Format("{0}.{1}", ID, num);
        }

        private string findFbMode(int fbmode)
        {
            if (fbmode == 1)
            { 
                //按下
                return "闭合";
            }
            else if (fbmode == 2)
            {
                //按下
                return "释放";
            }
            else
            {
                return "";
            }
        }

        #endregion

        #region IO键选择

        private void cbIoNum_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cbIONum.Text))
                {
                    return;
                }
                if (setKeyNum(Convert.ToInt32(cbIONum.Text)))
                {
                    dgvSensorAddItem();
                    cbIONum.SelectionStart = cbIONum.Text.Length;
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
        private bool setKeyNum(int IONum)
        {
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return false;
            }

            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            srs.ioNum = IONum;
            List<DataJson.sensorsInfo> delSensor = new List<DataJson.sensorsInfo>();
            HashSet<int> numList = new HashSet<int>();
            foreach (DataJson.sensorsInfo srInfo in srs.sensorsInfo)
            {
                if (srInfo.id > IONum)
                {
                    delSensor.Add(srInfo);
                }
                numList.Add(srInfo.id);
            }
            for (int i = 0; i < delSensor.Count; i++)
            {
                //删除id大于面板按键值
                srs.sensorsInfo.Remove(delSensor[i]);
            }
            List<int> list = numList.ToList<int>();
            bool isExit = false;
            for (int i = 1; i <= IONum; i++)
            {
                isExit = false;
                for (int j = 0; j < list.Count; j++)
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
                    DataJson.sensorsInfo srInfo = new DataJson.sensorsInfo();
                    srInfo.id = i;
                    srInfo.pid = 0;
                    srInfo.keyAddress = "";
                    srInfo.objAddress = "";
                    srInfo.objType = "";
                    srInfo.opt = "";
                    srInfo.optName = "";
                    srInfo.fbmode = 0;
                    srs.sensorsInfo.Add(srInfo);
                    //排序
                    sensorInfoSort(srs);
                }
            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return true;
        }

        private void cbDevNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDevNum.Text == "")
            {
                return;
            }
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return;
            }
            string ip = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sensorSelectNode));
            string id = SocketUtil.strtohexstr(cbDevNum.Text);
            string tmpNum = "";
            //devices 里面ini的名字
            List<int> ioNumList = findIONum();
            
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            int i = 0;
            for (int j = dataGridView1.SelectedCells.Count -1; j >= 0; j--)
            {

                
                int colIndex = dataGridView1.SelectedCells[j].ColumnIndex;
                if (colIndex != 1)
                {
                    continue;
                }
                int num = dataGridView1.SelectedCells[j].RowIndex;
                DataJson.sensorsInfo srInfo = srs.sensorsInfo[num];
                if (srInfo == null)
                {
                    return;
                }
                if (i >= ioNumList.Count)
                {
       
                    srInfo.keyAddress = "";
                    dataGridView1.Rows[dataGridView1.SelectedCells[j].RowIndex].Cells[1].Value = null;
                }
                else
                {
                    tmpNum = SocketUtil.strtohexstr(ioNumList[i].ToString());
                    while (tmpNum.Length < 4)
                    {
                        tmpNum = tmpNum.Insert(0, "0");
                    }
                    srInfo.keyAddress = string.Format("{0}{1}{2}", ip, id, tmpNum);
                    dataGridView1.Rows[dataGridView1.SelectedCells[j].RowIndex].Cells[1].Value = keyAddressTransform(srInfo.keyAddress);
                }
                
                i++;
                
            }

            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            cbDevNum.SelectedItem = null;
            //dgvSensorAddItem();
        }

        /// <summary>
        /// 寻找该设备号的信息IO数
        /// </summary>
        private List<int> findIONum()
        {
            List<int> ioNumList = new List<int>();
            string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
            //devices 里面ini的名字
            string ioVal = "";
            string path = Application.StartupPath + "\\devices\\";
            //从设备加载网关信息
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                if (d.ip == ip)
                {
                    //加载设备
                    foreach (DataJson.Module m in d.module)
                    {
                        if (m.id.ToString() == cbDevNum.Text)
                        {
                            ioVal = IniConfig.GetValue(path + m.device + ".ini", "input", "io");
                            //iniKEY的内容不为字符 null
                            if (ioVal != "null")
                            {
                                if (ioVal.Contains("-"))
                                {
                                    string[] infos = ioVal.Split('-');
                                    int j = Convert.ToInt32(infos[1]);
                                    if (j > 100)
                                    {
                                        j = 100;
                                    }
                                    for (int i = Convert.ToInt32(infos[0]); i <= j; i++)
                                    {
                                        ioNumList.Add(i);
                                    }
                                }
                                else if (ioVal.Contains(","))
                                {
                                    string[] infos = ioVal.Split(',');

                                    for (int i = 0; i < infos.Length; i++)
                                    {
                                        ioNumList.Add(Convert.ToInt32( infos[i]));
                                    }
                                }
                                else
                                {
                                    ioNumList.Add(Convert.ToInt32(ioVal));
             
                                }

                            }
                            break;
                        }

                    }
                }

            }
            return ioNumList;
        }


        #endregion


        #region 载入 载出 清空  清除 设置 按键
        //载入
        private void btnDown_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            try
            {
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    return;
                }
                //面板信息不为空
                if (srs.sensorsInfo.Count > 0)
                {
                    DataJson.Kn kn = new DataJson.Kn();
                    kn.key = new List<DataJson.Keynumber>();

                    //把有效的对象操作 放到kN对象里面
                    foreach (DataJson.sensorsInfo srInfo in srs.sensorsInfo)
                    {
                        //确保有信息
                        if (string.IsNullOrEmpty(srInfo.keyAddress) 
                            || string.IsNullOrEmpty(srInfo.objAddress) 
                            || string.IsNullOrEmpty(srInfo.opt)  
                            || string.IsNullOrEmpty(srInfo.objType)
                            || srInfo.fbmode == 0
                            )
                        {
                            continue;
                        }

                        DataJson.Keynumber keyInfo = new DataJson.Keynumber();
                        keyInfo.num = srInfo.id;
                        keyInfo.key = "00" + srInfo.keyAddress.Substring(2, 6);
                        keyInfo.obj = srInfo.objAddress;
                        keyInfo.mode= 255;
                        keyInfo.fback = srInfo.opt;

                        keyInfo.fbmode = srInfo.fbmode;

                        kn.key.Add(keyInfo);



                    }
                    if (kn.key.Count > 0)
                    {
                        //序列化SN对象
                        string sjson = JsonConvert.SerializeObject(kn);

                        //写入数据格式
                        string path = "down /json/k" + srs.id + ".json$" + sjson;
                        //测试写出文档
                        //File.WriteAllText(FileMesege.filePath + "\\objs\\s" + sceneNum + ".json", path);
                        //string check = "exist /json/s" + sceneNum + ".json$";
                        TcpSocket ts = new TcpSocket();
                        int i = 0;
                        string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
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
                        AppTxtShow("加载失败！无感应指令！");
                    }

                }
                else
                {
                    AppTxtShow("加载失败！无感应指令！！");
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
            if (FileMesege.sensorSelectNode == null || FileMesege.sensorSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
                string[] ids = FileMesege.sensorSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = SocketUtil.getIP(FileMesege.sensorSelectNode);
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
            if (FileMesege.sensorSelectNode == null || FileMesege.sensorSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
                string[] ids = FileMesege.sensorSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = SocketUtil.getIP(FileMesege.sensorSelectNode);
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

        //清除
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    return ;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                srs.sensorsInfo.Clear();
                for (int i = 1; i <= srs.ioNum; i++)
                {
                    //添加改id的按键
                    DataJson.sensorsInfo srInfo = new DataJson.sensorsInfo();
                    srInfo.id = i;
                    srInfo.pid = 0;
                    srInfo.keyAddress = "";
                    srInfo.objAddress = "";
                    srInfo.objType = "";
                    srInfo.opt = "";
                    srInfo.optName = "";
                    srInfo.fbmode = 0;
                    srs.sensorsInfo.Add(srInfo);
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvSensorAddItem();
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        //删除设置
        private void btnDel_Click(object sender, EventArgs e)
        {
            clearSensorInfo();
        }

        /// <summary>
        /// 清除主机k#.json信息
        /// </summary>
        private void clearSensorInfo()
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.sensorSelectNode == null || FileMesege.sensorSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
                string[] ids = FileMesege.sensorSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = SocketUtil.getIP(FileMesege.sensorSelectNode);
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

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

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
                                if (dataGridView1.Rows[rowCount].Cells[3].Value != null)
                                {
                                    //原地址
                                    add = dataGridView1.Rows[rowCount].Cells[3].Value.ToString();
                                }
                                objType = dataGridView1.Rows[rowCount].Cells[4].EditedFormattedValue.ToString();
                                //赋值List 并添加地域 名字
                                dgvObjAddress( objType, add);
                                break;
                            case "operation":
                                //操作
                                string info = dgvOperation( dataGridView1.Rows[rowCount].Cells[4].EditedFormattedValue.ToString());
                                if (info != null)
                                {
                                    dataGridView1.Rows[rowCount].Cells[7].Value = info;
                                }
                                break;
           
                            case "del":
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
               
                     


                            default: break;
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
                                setTitleAddress();
                                break;
                            case "objType":
                                setTitleAddress();
                                break;
                            case "section":
                                setTitleAddress();
                                break;
                            case "name":
                                setTitleAddress();
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
                     
                        case "fbMode":
                            dgvFbMode(rowNum);
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
        private DataJson.PointInfo dgvJumpSet()
        {
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return null;
            }
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sensorsInfo srInfo = srs.sensorsInfo[rowCount];
            if (srInfo == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(srInfo.objAddress))
            {
                return null;
            }
            if (srInfo.objType == "4.0_scene" || srInfo.objType == "5.0_time" || srInfo.objType == "6.1_panel" || srInfo.objType == "6.2_sensor")
            {
                return DataListHelper.findPointByType_address(srInfo.objType, srInfo.objAddress);
            }

            return null;

        }


        /// <summary>
        /// 按键地址选择
        /// </summary>
        private void dgvKeyAddress(string address)
        {
            if (dataGridView1.Rows[rowCount].Cells[9].Value == null)
            {
                return;
            }
            panelCheck plc = new panelCheck();
            //把窗口向屏幕中间刷新
            plc.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            plc.Address = address;
            plc.IsIO = true;
            plc.ShowDialog();
            if (plc.DialogResult == DialogResult.OK)
            {
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
                foreach (DataJson.sensorsInfo srInfo in srs.sensorsInfo)
                {
                    if (srInfo.id == id)
                    {
                        srInfo.keyAddress = plc.RtAddress;
                    }
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);

                dgvSensorAddItem();
            }
        }

        /// <summary>
        /// 设置显示模式 无 同步 反显
        /// </summary>
        private void dgvFbMode(int id)
        {
           
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                dataGridView1.Rows[id].Cells[2].Value = "";
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            string mode = dataGridView1.Rows[id].Cells[2].EditedFormattedValue.ToString();
            if (mode == "闭合")
            {
                srs.sensorsInfo[id].fbmode = 1;
            }
            else if (mode == "释放")
            {
                srs.sensorsInfo[id].fbmode = 2;
            }
            
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);


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
                string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //地址
                srs.sensorsInfo[rowCount].objAddress = dc.Obj;
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
                srs.sensorsInfo[rowCount].objType = type;
                //获取树状图的IP第四位  + Address地址的 后六位
                string ad = SocketUtil.GetIPstyle(ip, 4) + dc.Obj.Substring(2, 6);
                if (type == "9.0_variable")
                {
                    //变量特殊处理
                    ad = dc.Obj;
                }
                //区域加名称
                DataJson.PointInfo point = DataListHelper.findPointByType_address("", ad);

                if (point != null)
                {
                    srs.sensorsInfo[rowCount].pid = point.pid;
                    srs.sensorsInfo[rowCount].objType = point.type;
                    if (srs.sensorsInfo[rowCount].objType != point.type)
                    {
                        srs.sensorsInfo[rowCount].opt ="";
                        srs.sensorsInfo[rowCount].optName = "";
                    }

                }
                else
                {
                    //搜索一次dev表 
                    srs.sensorsInfo[rowCount].pid = 0;
                    //info.type = dc.ObjType;
                    srs.sensorsInfo[rowCount].opt = "";
                    srs.sensorsInfo[rowCount].optName = "";

                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            dgvSensorAddItem();


        }

        /// <summary>
        /// DGV表 操作栏
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string dgvOperation( string type)
        {

            sceneConcrol dc = new sceneConcrol();
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return null;
            }
            DataJson.sensorsInfo srInfo = srs.sensorsInfo[rowCount];
            dc.Point = DataListHelper.findPointByPid(srInfo.pid);
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            dc.ObjType = type;

            dc.Opt = srInfo.opt;
            dc.Ver = srInfo.optName;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                srInfo.opt = dc.Opt;
                srInfo.optName = dc.Ver;
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                return dc.Ver + " " + dc.Opt;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 删除 或者清空
        /// </summary>
        private void dgvDel()
        {
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return;
            }
            DataJson.sensorsInfo srsInfo = srs.sensorsInfo[rowCount];
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (dataGridView1.Rows[rowCount].Cells[9].Value == null)
            {
                //直接删除
                srs.sensorsInfo.Remove(srsInfo);
                dataGridView1.Rows.Remove(dataGridView1.Rows[rowCount]);
            }
            else
            {
                //清空当条信息
                srsInfo.keyAddress = "";
                srsInfo.objAddress = "";
                srsInfo.objType = "";
                srsInfo.opt = "";
                srsInfo.optName = "";
                srsInfo.pid = 0;
                srsInfo.fbmode = 0;
                dataGridView1.Rows[rowCount].Cells[1].Value = null;
                dataGridView1.Rows[rowCount].Cells[2].Value = "";
                dataGridView1.Rows[rowCount].Cells[3].Value = null;
                dataGridView1.Rows[rowCount].Cells[4].Value = "";
                dataGridView1.Rows[rowCount].Cells[5].Value = null;
                dataGridView1.Rows[rowCount].Cells[6].Value = null;
                dataGridView1.Rows[rowCount].Cells[7].Value = null;
            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);

        }

        /// <summary>
        /// 添加新感应
        /// </summary>
        private void addInfo()
        {
            if (dataGridView1.Rows[rowCount].Cells[9].Value == null)
            {
                return;
            }
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return;
            }
            int fbmode = 0;
            int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
            string objType = "", keyAddress = "", objAddress = "",opt = "",optName="";
            //计算往哪个位置插入
            int inserNum = 0;
            bool isbreak = false;
            foreach (DataJson.sensorsInfo find in srs.sensorsInfo)
            {
                inserNum++;
                if (find.id == id)
                {
                    if (!string.IsNullOrEmpty(find.keyAddress))
                    {
                        objType = find.objType;
                        keyAddress = find.keyAddress;
                        objAddress = find.objAddress;
                        opt = find.opt;
                        optName = find.optName;
                        fbmode = find.fbmode;
                    }

                }
                if (find.id == id + 1)
                {
                    isbreak = true;
                    break;
                }

            }
            if (!isbreak && srs.sensorsInfo.Count == inserNum)
            {
                //插入到信息最后
                inserNum++;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //添加改id的按键
            DataJson.sensorsInfo srInfo = new DataJson.sensorsInfo();
            //地址加一处理 并搜索PointList表获取地址 信息
            if (!string.IsNullOrEmpty(objAddress) && objAddress != "FFFFFFFF")
            {
                switch (objAddress.Substring(2, 2))
                {
                    case "00":
                        //设备类地址
                        objAddress = objAddress.Substring(0, 6) + SocketUtil.strtohexstr((Convert.ToInt32(objAddress.Substring(6, 2), 16) + 1).ToString());
                        break;
                    default:
                        string hexnum = SocketUtil.strtohexstr((Convert.ToInt32(objAddress.Substring(4, 4), 16) + 1).ToString());
                        while (hexnum.Length < 4)
                        {
                            hexnum = hexnum.Insert(0, "0");
                        }
                        objAddress = objAddress.Substring(0, 4) + hexnum;
                        break;
                }
                //按照地址查找type的类型 
                objType = IniHelper.findIniTypesByAddress(FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0], objAddress).Split(',')[0];
                srInfo.objType = objType;
                string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sensorSelectNode));//16进制
                //添加地域和名称 在sceneInfo表中
                DataJson.PointInfo point = DataListHelper.findPointByType_address(objType, ip4 + objAddress.Substring(2, 6));
                if (point != null)
                {
                    srInfo.pid = point.pid;
                    srInfo.objType = objType;
                    if (srInfo.objType != point.type)
                    {
                        //清空执行模式（操作）
                        srInfo.opt = "";
                        srInfo.optName = "";
                    }
                }
                else
                {
                    srInfo.pid = 0;
                    srInfo.opt = "";
                    srInfo.optName = "";
                }
            }
            srInfo.id = id;
            srInfo.keyAddress = keyAddress;
            srInfo.objAddress = objAddress;
            srInfo.fbmode = fbmode;
            srs.sensorsInfo.Insert(inserNum - 1, srInfo);

            //排序
            //panelInfoSort(pls);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvSensorAddItem();
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
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    return;
                }
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    DataJson.sensorsInfo srInfo = srs.sensorsInfo[dataGridView1.SelectedCells[i].RowIndex];
                    if (srInfo == null)
                    {
                        continue;
                    }

                    if (colIndex == 7)
                    {

                        ischange = true;
                        srInfo.opt = "";
                        srInfo.optName = "";

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

        /// <summary>
        /// 复制title选中的节点 赋地址给ObjAddress
        /// </summary>
        private void setTitleAddress()
        {
            //int colIndex = dataGridView1.SelectedCells[0].ColumnIndex;
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return;
            }
            int id = dataGridView1.CurrentCell.RowIndex;
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sensorsInfo srInfo = srs.sensorsInfo[id];
            if (srInfo == null)
            {
                return;
            }
            List<string> section_name = DataListHelper.dealPointInfo(FileMesege.titlePointSection);

            DataJson.PointInfo eq = DataListHelper.findPointBySection_name(section_name);
            if (eq == null)
            {
                return;
            }

            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //if (colIndex == 3 || colIndex == 4 || colIndex == 5 || colIndex == 6 || !isClick)
            //{
                if (eq.type == srInfo.objType)
                {
                    srInfo.pid = eq.pid;
                    srInfo.objAddress = eq.address;
                    dataGridView1.Rows[id].Cells[3].Value = DgvMesege.addressTransform(srInfo.objAddress);
                    dataGridView1.Rows[id].Cells[5].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView1.Rows[id].Cells[6].Value = eq.name;
                }
                else
                {
                    srInfo.pid = eq.pid;
                    srInfo.objAddress = eq.address;
                    srInfo.objType = eq.type;
                    srInfo.opt = "";
                    srInfo.optName = "";
                    dataGridView1.Rows[id].Cells[3].Value = DgvMesege.addressTransform(srInfo.objAddress);
                    dataGridView1.Rows[id].Cells[4].Value = IniHelper.findTypesIniNamebyType(srInfo.objType);
                    dataGridView1.Rows[id].Cells[5].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView1.Rows[id].Cells[6].Value = eq.name;
                    dataGridView1.Rows[id].Cells[7].Value = "";

                }


            //}//if
  

            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);


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
            if (colIndex == 7 )
            {
                int id = dataGridView1.CurrentRow.Index;
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    return;
                }

                //获取sceneInfo对象表中对应ID号info对象
                DataJson.sensorsInfo srInfo = srs.sensorsInfo[id];
                if (srInfo == null)
                {
                    return;
                }
                //获取sceneInfo对象表中对应ID号info对象
                FileMesege.copySensor = srInfo;

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
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    return;
                }
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    int id = dataGridView1.SelectedCells[i].RowIndex;
                    DataJson.sensorsInfo srInfo = srs.sensorsInfo[id];
                    if (srInfo == null)
                    {
                        return;
                    }

                    if (FileMesege.copySensor.objType == "" || srInfo.objType == "" || srInfo.objType != FileMesege.copySensor.objType)
                    {
                        continue;
                    }
                    if (colIndex == 7)
                    {

                        ischange = true;
                        srInfo.opt = FileMesege.copySensor.opt;
                        srInfo.optName = FileMesege.copySensor.optName;
                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[7].Value = (srInfo.optName + " " + srInfo.opt).Trim();
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

       

       

      




    }
}
