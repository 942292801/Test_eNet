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

namespace eNet编辑器.DgvView
{
    public partial class DgvPanel : Form
    {
        public DgvPanel()
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
        DataGridViewComboBoxColumn obj ;
        DataGridViewComboBoxColumn mode ;
        DataGridViewComboBoxColumn showobj ;
        DataGridViewComboBoxColumn showmode;

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> AppTxtShow;
        
        
        


        private void DgvBind_Load(object sender, EventArgs e)
        {

            //新增对象列 加载
            this.dataGridView1.Rows.Clear();
             obj = new DataGridViewComboBoxColumn();
             mode = new DataGridViewComboBoxColumn();
            showobj = new DataGridViewComboBoxColumn();
             showmode = new DataGridViewComboBoxColumn();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string name = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                
                name = IniConfig.GetValue(file.FullName, "define", "name");
                if (name != "")
                {
                    obj.Items.Add(name);
                    showobj.Items.Add(name);
                }
                
                
            }
            showmode.Items.Add("无");
            showmode.Items.Add("同步");
            showmode.Items.Add("反显");
            //设置列名
            obj.HeaderText = "执行对象";
            //设置下拉列表的默认值 
            obj.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            obj.DefaultCellStyle.NullValue = obj.Items[0];
            obj.Name = "objtype";

            //设置列名
            showobj.HeaderText = "显示对象";
            //设置下拉列表的默认值 
            showobj.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            showobj.DefaultCellStyle.NullValue = showobj.Items[0];
            showobj.Name = "showobjtype";

            //设置列名
            mode.HeaderText = "执行模式";
            //设置下拉列表的默认值 
            mode.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
         
            //或者这样设置 默认选择第一项
            //mode.DefaultCellStyle.NullValue = null;
            mode.Name = "mode";

            //设置列名
            showmode.HeaderText = "显示模式";
            //设置下拉列表的默认值 
            showmode.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            showmode.DefaultCellStyle.NullValue = showmode.Items[0];
            showmode.Name = "showmode";

            //插入执行对象
            this.dataGridView1.Columns.Insert(2, obj);
            //插入执行模式
            this.dataGridView1.Columns.Insert(6, mode);
            //插入显示对象
            this.dataGridView1.Columns.Insert(7, showobj);
            //插入显示模式
            this.dataGridView1.Columns.Insert(9, showmode);
            
        }

        /// <summary>
        /// 加载DgV所有信息
        /// </summary>
        public void dgvbindAddItem()
        {
            dataGridView1.Rows.Clear();
            //节点为空 或者节点不为选中设备
            if (FileMesege.bindSelectNode == null || FileMesege.bindSelectNode.Parent == null)
            {
                return;
            }
            //ip + 网关名称+设备号+设备名称
            string[] commons = getNodeInfo(FileMesege.bindSelectNode);
            //获取节点信息失败
            if (commons == null)
            {
                return;
            }
            
            DataJson.binds bs = getBindInfoList(commons[0],Convert.ToInt32( commons[2]));
            if (bs == null)
            {
                return;
            }
            
            bindInfoSort(bs);
            //循环 面板按钮列表 获取每一个数据
            foreach (DataJson.bindInfo bi in bs.bindInfo)
            {
                int dex = dataGridView1.Rows.Add();
             
                //按键号
                dataGridView1.Rows[dex].Cells[0].Value = bi.keyId;
                //编组
                dataGridView1.Rows[dex].Cells[1].Value = bi.groupId;
                //执行对象 开关 场景。。。。
                dataGridView1.Rows[dex].Cells[2].Value= bi.objType;
                //根据不同类型加载不同类
                updataMode(bi.objType,dex);


                //执行地址
                dataGridView1.Rows[dex].Cells[3].Value = bi.address;

                //设置区域 名称
                setSection(bi.objType, commons[0], bi.address, dex);

                //执行模式  例：单键按下开 
                dataGridView1.Rows[dex].Cells[6].Value = bi.mode;
                //显示对象 
                dataGridView1.Rows[dex].Cells[7].Value = bi.showType;
                //显示地址
                dataGridView1.Rows[dex].Cells[8].Value = bi.showAddress;
                //显示模式 无  同步 反显
                dataGridView1.Rows[dex].Cells[9].Value = bi.showMode;
                //同步
                dataGridView1.Rows[dex].Cells[10].Value = "同步";
                //模式
                dataGridView1.Rows[dex].Cells[11].Value = "删除";

               
              
               
            
            }
           

        }

        /// <summary>
        /// 设置 区域名称
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ip"></param>
        /// <param name="Address"></param>
        /// <param name="dex"></param>
        private void getSection(List<DataJson.PointInfo> list,string ip ,string Address ,int dex)
        {
            /**
            //当地址信息为空退出
            if (Address == "" || Address == null)
            {
                return;
            }
            TreeMesege tm = new TreeMesege();
            //在设备列表中循环 根据IP地址找
            foreach (DataJson.Equipment eq in list)
            {

                //地址相同
                if (eq.address == SocketUtil.getHexIP(ip, 4, Address) && eq.ip == ip)
                {
                    //区域
                    dataGridView1.Rows[dex].Cells[4].Value = tm.GetSection(eq.id1, eq.id2, eq.id3, eq.id4).Replace("\\", " ");
                    //名称
                    dataGridView1.Rows[dex].Cells[5].Value = eq.name;
                    break;
                }
            }**/
        }

        /// <summary>
        /// 设置表中某一行的 区域 和名称 
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="ip">全ip</param>
        /// <param name="Address">十六进制地址 FE地址和非FE地址都可以</param>
        /// <param name="dex">行号</param>
        private void setSection(string type,string ip,string Address,int dex)
        {
            /**
            switch (type)
            {
                case "场景":
                    getSection(FileMesege.MapperList.scene, ip, Address, dex);

                    break;
                case "scene":
                    getSection(FileMesege.MapperList.scene, ip, Address, dex);

                    break;
                case "定时":
                    getSection(FileMesege.MapperList.timer, ip, Address, dex);

                    break;
                case "timer":
                    getSection(FileMesege.MapperList.timer, ip, Address, dex);

                    break;
                case "编组":
                    getSection(FileMesege.MapperList.link, ip, Address, dex);

                    break;
                case "groud":
                    getSection(FileMesege.MapperList.link, ip, Address, dex);

                    break;
                case "逻辑":
                    //待定 后期添加
                    break;
                case "logic":
                    break;
                //为设备00 扫equipment
                default:
                    getSection(FileMesege.MapperList.equipment, ip, Address, dex);
                    break;
            }
             **/
        }

        #region 工具类


        /// <summary>
        /// 获取某个IP点 某个面板的对象列表 否则返回空
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="num">设备号</param>
        /// <returns></returns>
        private DataJson.binds getBindInfoList(string ip, int num)
        {
            foreach (DataJson.Bind bdIP in FileMesege.bindList)
            {
                if (bdIP.IP == ip)
                {
                    foreach (DataJson.binds bd in bdIP.Binds)
                    {
                        if (bd.id == num)
                        {
                            return bd;
                        }
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// 获取节点信息  return 十进制  全ip + 网关名称+设备号+设备名称
        /// </summary>
        /// <param name="selectNode">选中的节点 且为子节点</param>
        /// <returns></returns>
        private string[] getNodeInfo(TreeNode SelectNode)
        {
            try
            {
                //选中子节点
                string[] ips = SelectNode.Parent.Text.Split(' ');
                string[] ids = SelectNode.Text.Split(' ');
                string sceneNum = Regex.Replace(ids[0], @"[^\d]*", "");
                //ip + 网关名称+设备号+设备名称
                string[] commons = { ips[0], ips[1], sceneNum, ids[1] };
                return commons;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 获取当前选中节点binds信息列表
        /// </summary>
        /// <returns></returns>
        private DataJson.binds getBinds()
        {
            //ip + 网关名称+设备号+设备名称
            string[] commons = getNodeInfo(FileMesege.bindSelectNode);
            //获取节点信息失败
            if (commons == null)
            {
                return null;
            }
            DataJson.binds binds = getBindInfoList(commons[0], Convert.ToInt32(commons[2]));
            return binds;
        }

        /// <summary>
        /// bindinfo信息按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void bindInfoSort(DataJson.binds binds)
        {
            binds.bindInfo.Sort(delegate(DataJson.bindInfo x, DataJson.bindInfo y)
            {
                return Convert.ToInt32(x.keyId).CompareTo(Convert.ToInt32(y.keyId));
            });
           
        }

       
        
        public class feedlist
        {
            public List<keyMode> list = null;
        }

         public class keyMode 
        {
            public string key { get; set; }
            public string val { get; set; }
           
        }
        /// <summary>
         /// 获取所有的INI信息
        /// </summary>
        /// <returns></returns>
         private List<keyMode> getkeyMode()
        {
            //修改mode的combox列表
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string name = "";
            //循环扫所有的ini文件
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {

                name = IniConfig.GetValue(file.FullName, "define", "name");


                //获取ini所有keyMode的Key  mode根据TYPE决定
                List<string> keys = IniConfig.ReadKeys("keyMode", file.FullName);
                if (keys == null)
                {
                    return null;
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    string tmp = IniConfig.GetValue(file.FullName, "keyMode", keys[i]);

                    //combox.Items.Add(tmp);
                }

                //changemode(rownum, mode.Items[0].ToString());





            } return null;
        }

        #endregion

         #region 增加 清空  清除 设置 按键
         //增加
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (FileMesege.bindSelectNode == null || FileMesege.bindSelectNode.Parent == null)
            {
                return;
            }
            if (FileMesege.bindList == null)
            {
                FileMesege.bindList = new List<DataJson.Bind>();
            }
            //获取十进制的信息
            string[] commons = getNodeInfo(FileMesege.bindSelectNode);
            if (commons == null)
            {
                return;
            }
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.binds binds = getBindInfoList(commons[0], Convert.ToInt32(commons[2]));
            //获取最后一行下标
            int count = dataGridView1.RowCount-1;
            DataJson.bindInfo binfo = new DataJson.bindInfo();
            if (count >= 0)
            {
                binfo.keyId = binds.bindInfo[count].keyId + 1;
                binfo.groupId = binds.bindInfo[count].groupId;
                binfo.mode = binds.bindInfo[count].mode;
                binfo.objType = binds.bindInfo[count].objType;
                binfo.showType = binds.bindInfo[count].showType;
                binfo.showMode = binds.bindInfo[count].showMode;


                //处理上一个地址加1
                binfo.address = SocketUtil.HexAddressAddOne(binds.bindInfo[count].address);
                binfo.showAddress = SocketUtil.HexAddressAddOne(binds.bindInfo[count].showAddress);
            }
            else
            {
                binfo.groupId = Convert.ToInt32(commons[2]);
                binfo.objType = "开关";
                //binfo.Address
                binfo.showType = "开关";

                binfo.showMode = "无";
                binfo.keyId =1;
                
            }
            binds.bindInfo.Add(binfo);
            //按照KEPID重新排序
            //bindInfoSort(binds);
            //重新加载
            dgvbindAddItem();

        }

        //清空
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (FileMesege.bindSelectNode == null || FileMesege.bindSelectNode.Parent == null)
            {
                return;
            }
            if (FileMesege.bindList == null)
            {
                FileMesege.bindList = new List<DataJson.Bind>();
            }
            //选中子节点
            //获取十进制的信息
            string[] commons = getNodeInfo(FileMesege.bindSelectNode);
            //获取节点信息失败
            if (commons == null)
            {
                return;
            }
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.binds binds = getBindInfoList(commons[0], Convert.ToInt32(commons[2]));
            binds.bindInfo.Clear();
            this.dataGridView1.Rows.Clear();
        }

        //清除
        private void btnRemove_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.bindSelectNode == null || FileMesege.bindSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                //获取十进制的信息
                string[] commons = getNodeInfo(FileMesege.bindSelectNode);
                if (commons == null)
                {
                    return;
                }
                //发送调用指令
                string ip4 = SocketUtil.getIP(FileMesege.bindSelectNode);
                TcpSocket ts = new TcpSocket();
                sock = ts.ConnectServer(commons[0], 6003, 2);
                if (sock == null)
                {
                    //防止一连失败
                    sock = ts.ConnectServer(commons[0], 6003, 2);
                    if (sock == null)
                    {
                        AppTxtShow("连接失败！");
                        //sock.Close();
                        return;
                    }

                }
                //编组号
                int sceneNum = Convert.ToInt32(commons[2]);
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
                //编组号为设备号 清除该编组的信息
                string oder = String.Format("SET;00000005;{{{0}.48.{1}}};\r\n", ip4, number);// "SET;00000005;{" + ip4 + ".48." + number + "};\r\n";
                int flag = ts.SendData(sock, oder, 2);
                if (flag == 0)
                {
                    AppTxtShow("发送指令成功！");
                    sock.Close();
                }
                else
                {
                    AppTxtShow("发送指令失败！");
                }
            }
            catch
            {
                AppTxtShow("发送指令失败！");
            }
        }

        //设置
        private void btnSet_Click(object sender, EventArgs e)
        {
            /* Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.bindSelectNode != null && FileMesege.bindSelectNode.Parent != null)
            {
                try
                {
                    //获取十进制的信息
                    string[] commons = getNodeInfo(FileMesege.bindSelectNode);
                    //获取该节点IP地址场景下的 场景信息对象
                    DataJson.binds binds = getBindInfoList(commons[0], Convert.ToInt32(commons[2]));
                    //场景信息不为空
                    if (binds.bindInfo.Count > 0)
                    {
                        DataJson.Kn kn = new DataJson.Kn();
                        kn.key = new List<DataJson.Keynumber>();
                        //把有效的对象操作 放到SN对象里面
                        foreach (DataJson.bindInfo kinfo in binds.bindInfo)
                        {
                            //确保有信息
                            if (kinfo.Address == "" || kinfo.Address == null || kinfo.mode == "" || kinfo.mode == null)
                            {
                                continue;
                            }
                            DataJson.Scenenumber sb = new DataJson.Scenenumber();

                            sb.num = info.id;
                            sb.obj = info.Add;
                            sb.val = info.opt;
                            sb.delay = info.Delay;
                            sn.action.Add(sb);

                        }
                        if (sn.action.Count > 0)
                        {
                            //序列化SN对象
                            string sjson = FileMesege.ConvertJsonString(JsonConvert.SerializeObject(sn));

                            //写入数据格式
                            string path = "down /json/s" + sceneNum + ".json$" + sjson;
                            //测试写出文档
                            //File.WriteAllText(FileMesege.filePath + "\\objs\\s" + sceneNum + ".json", path);
                            //string check = "exist /json/s" + sceneNum + ".json$";
                            TcpSocket ts = new TcpSocket();

                            sock = ts.ConnectServer(ips[0], 6001, 3);
                            if (sock == null)
                            {
                                //防止一连失败
                                sock = ts.ConnectServer(ips[0], 6001, 3);
                                if (sock == null)
                                {
                                    TxtShow("连接失败！\r\n");
                                    //sock.Close();
                                    return;
                                }

                            }
                            //0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常
                            int flag = ts.SendData(sock, path, 2);
                            if (flag == 0)
                            {
                                //关闭套接字
                                sock.Close();
                                //sock.Disconnect(true);
                                //重连
                                sock = ts.ConnectServer(ips[0], 6001, 3);
                                if (sock == null)
                                {
                                    TxtShow("连接失败！\r\n");
                                    //sock.Close();
                                    return;
                                }
                                flag = ts.SendData(sock, "exist /json/s" + sceneNum + ".json$", 2);
                                if (flag == 0)
                                {
                                    TxtShow("加载成功！\r\n");

                                }
                                else
                                {
                                    TxtShow("加载失败！\r\n");
                                }
                                sock.Close();
                            }



                        }//if有场景信息
                        else
                        {
                            TxtShow("无场景指令！\r\n");
                        }

                    }
                    else
                    {
                        TxtShow("无场景指令！\r\n");
                    }

                }
                catch (Exception ex)
                {
                    //TxtShow(ex.ToString());
                    TxtShow("加载失败！\r\n");
                }
            }
            else
            {
                //无场景信息加载
            }*/
        }



        #endregion


        #region 表格操作

        /// <summary>
        /// 按钮 弹框操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //选中行号
            int rowNum = e.RowIndex;
            //选中列号
            int columnNum = e.ColumnIndex;
            string[] commons = getNodeInfo(FileMesege.bindSelectNode);
            if (rowNum >= 0 && columnNum >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value);
                switch (dataGridView1.Columns[columnNum].Name)
                {
                    case "address":
                        //执行地址 MessageBox.Show("3");
                        string obj = "";
                        if (dataGridView1.Rows[rowNum].Cells[3].Value != null)
                        {
                            //原地址
                            obj = dataGridView1.Rows[rowNum].Cells[3].Value.ToString();
                        }             
                        string objType = dataGridView1.Rows[rowNum].Cells[2].EditedFormattedValue.ToString();
                        //赋值List 并添加地域 名字
                        string address = changeaddress(rowNum,objType,obj);
                        //添加区域
                        setSection(objType,commons[0],address,rowNum);
                        //添加地址
                        dataGridView1.Rows[rowNum].Cells[3].Value = address;
                        break;

                    case "showaddress":
                        //显示地址 MessageBox.Show("8");
                        obj = "";
                        if (dataGridView1.Rows[rowNum].Cells[8].Value != null)
                        {
                            //原地址
                            obj = dataGridView1.Rows[rowNum].Cells[8].Value.ToString();
                        }
                        objType = dataGridView1.Rows[rowNum].Cells[7].EditedFormattedValue.ToString();
                        //赋值List 并添加地域 名字
                        address = changeshowaddress(rowNum, objType, obj);
                        //添加地址
                        dataGridView1.Rows[rowNum].Cells[8].Value = address;

                        break;
                    case "synchronous":
                        //同步 MessageBox.Show("10");

                        changesynchronous(rowNum);
                        
                        break;
                    case "delete":
                        //删除MessageBox.Show("11");

                        changedelete(rowNum);
                        
                        break;
                   
                 
                    default: break;
                }

            }
            
            
        }

        


        /// <summary>
        /// combox 和 Able编辑操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //选中行号
            int rowNum = e.RowIndex;
            //选中列号
            int columnNum = e.ColumnIndex;
            if (rowNum >= 0 && columnNum >= 0)
            {
                switch (dataGridView1.Columns[columnNum].Name)
                {
                    case "keyid":
                        int newid = Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].EditedFormattedValue);
                        //按键号MessageBox.Show("0");
                        changekeyid(rowNum, newid);

                        break;
                    case "groupid":
                        //编组号MessageBox.Show("1")
                        newid = Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[1].EditedFormattedValue);
                        changegroupid(rowNum, newid);
                        break;
                    case "objtype":
                        //执行对象 类型MessageBox.Show("2");

                        changeobjtype(rowNum, dataGridView1.Rows[rowNum].Cells[2].EditedFormattedValue.ToString());
                        break;
                    case "mode":
                        //执行模式 MessageBox.Show("6");
                        //更新combox选项
                        //updataMode(dataGridView1.Rows[rowNum].Cells[2].EditedFormattedValue.ToString(),rowNum);
                        
                        changemode(rowNum, dataGridView1.Rows[rowNum].Cells[6].EditedFormattedValue.ToString());
                        break;
                    case "showobjtype":
                        //显示对象  类型MessageBox.Show("7");
                        changeshowobjtype(rowNum, dataGridView1.Rows[rowNum].Cells[7].EditedFormattedValue.ToString());
                        break;
                    case "showmode":
                        //显示模式 无 同步 反显 MessageBox.Show("9");
                        changeshowmode(rowNum, dataGridView1.Rows[rowNum].Cells[9].EditedFormattedValue.ToString());
                        break;
                    default: break;
                }
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
                    case "keyid":
                        //int newid = Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].EditedFormattedValue);
                        //按键号MessageBox.Show("0");
                        //changekeyid(rowNum, newid);

                        break;
                    case "groupid":
                        //编组号MessageBox.Show("1")
                        //newid = Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[1].EditedFormattedValue);
                        //changegroupid(rowNum, newid);
                        break;
                    case "objtype":
                        //执行对象 类型MessageBox.Show("2");

                        //changeobjtype(rowNum, dataGridView1.Rows[rowNum].Cells[2].EditedFormattedValue.ToString());
                        break;
                    case "mode":
                        //执行模式 MessageBox.Show("6");
                        //更新combox选项
                        updataMode(dataGridView1.Rows[rowNum].Cells[2].EditedFormattedValue.ToString(),rowNum);

                        break;
                    case "showobjtype":
                        //显示对象  类型MessageBox.Show("7");
                        //changeshowobjtype(rowNum, dataGridView1.Rows[rowNum].Cells[7].EditedFormattedValue.ToString());
                        break;
                    case "showmode":
                        //显示模式 无 同步 反显 MessageBox.Show("9");
                        //changeshowmode(rowNum, dataGridView1.Rows[rowNum].Cells[9].EditedFormattedValue.ToString());
                        break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// combox出错时候 什么事情都不做 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }
     

        #region combox  编辑框
        /// <summary>
        /// 修改keyid
        /// </summary>
        /// <param name="rownum">行号 List索引号</param>
        private void changekeyid(int rownum,int newid)
        {
            //获取当前选中节点 
            DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return;
            }
            binds.bindInfo[rownum].keyId = newid;
            
            
        }
        /// <summary>
        /// 修改groupid
        /// </summary>
        /// <param name="rownum">行号 List索引号</param>
        private void changegroupid(int rownum, int newid)
        {
            //获取当前选中节点 
            DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return;
            }
            binds.bindInfo[rownum].groupId = newid;

        }

        /// <summary>
        /// 修改执行对象
        /// </summary>
        /// <param name="rownum">行号 list索引号</param>
        /// <param name="objtype">DGV框执行对象的类型</param>
        private void changeobjtype(int rownum,string objtype)
        { 
            //获取当前选中节点 
            DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return;
            }
            binds.bindInfo[rownum].objType = objtype;
              
            
        }

        /// <summary>
        /// 修改显示对象
        /// </summary>
        /// <param name="rownum">行号 list索引号</param>
        /// <param name="objtype">DGV框显示对象的类型</param>
        private void changeshowobjtype(int rownum, string objtype)
        {
            //获取当前选中节点 
            DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return;
            }
            binds.bindInfo[rownum].showType = objtype;
        }

        /// <summary>
        /// 修改执行模式
        /// </summary>
        /// <param name="rownum">行号 list索引号</param>
        /// <param name="objtype">DGV框 执行模式 的类型</param>
        private void changemode(int rownum, string mode)
        {
            
            //获取当前选中节点 
            DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return;
            }
            binds.bindInfo[rownum].mode = mode;
            //重新刷新每一行的信息
            //获取当前选中节点 
            /*DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return;
            }
            */
            for (int i = 0; i < binds.bindInfo.Count; i++)
            {
                if (i == rownum)
                {
                    continue;
                }
                dataGridView1.Rows[i].Cells[6].Value = binds.bindInfo[i].mode;
            }
           
        }

        /// <summary>
        /// 修改显示模式
        /// </summary>
        /// <param name="rownum">行号 list索引号</param>
        /// <param name="objtype">DGV框 显示模式 的类型</param>
        private void changeshowmode(int rownum, string showmode)
        {
            //获取当前选中节点 
            DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return;
            }
            binds.bindInfo[rownum].showMode = showmode;
        }


        /// <summary>
        /// 更改combox的items信息
        /// </summary>
        /// <param name="objtype"></param>
        private void updataMode(string objtype,int rownum)
        {
            //敲黑板 重点  重新在某一列上添加combox
            DataGridViewComboBoxCell combox = dataGridView1.Rows[rownum].Cells["mode"] as DataGridViewComboBoxCell;
            combox.Items.Clear();    
             //修改mode的combox列表
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string name = "";
            //循环扫所有的ini文件
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {

                name = IniConfig.GetValue(file.FullName, "define", "name");
                if (name == objtype)
                {
                    //获取ini所有keyMode的Key  mode根据TYPE决定
                    List<string> keys = IniConfig.ReadKeys("keyMode", file.FullName);
                    if (keys == null)
                    {
                        return;
                    }
                    for (int i = 0; i < keys.Count; i++)
                    {
                        string tmp = IniConfig.GetValue(file.FullName, "keyMode", keys[i]);

                        combox.Items.Add(tmp);
                    }
                    
                    //changemode(rownum, mode.Items[0].ToString());
                    
                    break;
                }


            }

            
      
            
        }
        #endregion

        #region 按钮 弹框

        /// <summary>
        /// 改变执行地址
        /// </summary>
        /// <param name="rownum">行号</param>
        /// <param name="objType">对象类型</param>
        /// <param name="obj">原来对象地址</param>
        private string changeaddress(int rownum,string objType,string obj)
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
                //获取当前选中节点 
                DataJson.binds binds = getBinds();
                if (binds == null)
                {
                    return "";
                }
                binds.bindInfo[rownum].address = dc.Obj;
               
            }

            return dc.Obj;
        }

        /// <summary>
        /// 改变执行地址
        /// </summary>
        /// <param name="rownum">行号</param>
        /// <param name="objType">对象类型</param>
        /// <param name="obj">原来对象地址</param>
        private string changeshowaddress(int rownum, string objType, string obj)
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
                //获取当前选中节点 
                DataJson.binds binds = getBinds();
                if (binds == null)
                {
                    return "";
                }
                binds.bindInfo[rownum].showAddress = dc.Obj;

            }

            return dc.Obj;
        }


        /// <summary>
        /// 同步按钮
        /// </summary>
        /// <param name="rownum"></param>
        private void changesynchronous(int rownum) 
        {
            //获取当前选中节点 
            DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return ;
            }
            binds.bindInfo[rownum].showAddress = binds.bindInfo[rownum].address;
            binds.bindInfo[rownum].showType = binds.bindInfo[rownum].objType;//7
            binds.bindInfo[rownum].showMode = showmode.Items[1].ToString();//9
            dataGridView1.Rows[rownum].Cells[8].Value = binds.bindInfo[rownum].address;
            dataGridView1.Rows[rownum].Cells[7].Value = binds.bindInfo[rownum].objType;
            dataGridView1.Rows[rownum].Cells[9].Value = showmode.Items[1].ToString();

        }

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="rownum">行号 索引号</param>
        private void changedelete(int rownum)
        {
            //获取当前选中节点 
            DataJson.binds binds = getBinds();
            if (binds == null)
            {
                return;
            }
            binds.bindInfo.Remove(binds.bindInfo[rownum]);
            dataGridView1.Rows.Remove(dataGridView1.Rows[rownum]);
        }
        #endregion

 

      
  
        #endregion


        
        






    }
}
