using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using eNet编辑器.AddForm;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;

namespace eNet编辑器.DgvView
{
    public partial class DgvScene : Form
    {
        public DgvScene()
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
            //this.dataGridView1.DataError += delegate(object sender, DataGridViewDataErrorEventArgs e) { };
        }
        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> TxtShow;

        private void DgvScene_Load(object sender, EventArgs e)
        {

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
            //this.dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        //加载DgV所有信息
        public void dgvsceneAddItem()
        {
            this.dataGridView1.Rows.Clear();
            if (FileMesege.sceneSelectNode == null)
            {
                return;
            }
            if (FileMesege.sceneSelectNode.Parent != null)
            {
                try
                {
                    //选中子节点
                    //循环获取
                    string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
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
                            if ( info.address != "" &&info.address != "FFFFFFFF")
                            {
                                DataJson.PointInfo point = DataListHelper.findPointByType_address(info.type,ip4+ info.address.Substring(2,6));
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
                        dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform( info.address);
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
                catch (Exception ex) {
                    this.dataGridView1.Rows.Clear();
                    MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽"); 
                }
 
            }
            else
            {
                //选中父节点
            }
           
        }

        #region 功能性处理 函数

        /// <summary>
        /// 获取某个Scenes列表中对应ID号的SceneInfo 否则返回空
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataJson.sceneInfo getSceneID(DataJson.scenes sc,int id)
        {
            foreach (DataJson.sceneInfo info in sc.sceneInfo)
            {
                if (info.id == id)
                {
                    return info;
                }
            }
            return null;
        }

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
            }
        }

        /// <summary>
        /// 场景信息按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void sceneInfoSort(DataJson.scenes sc)
        {
            sc.sceneInfo.Sort(delegate(DataJson.sceneInfo x, DataJson.sceneInfo y)
            {
                return (x.id).CompareTo(y.id);
            });
        }

        #endregion

        #region 增加 清空 载入 调用
        //增加
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
            //没有选中或者选中了网关节点
            if (FileMesege.sceneSelectNode == null|| FileMesege.sceneSelectNode.Parent == null)
            {
                return;
            }
            if (FileMesege.sceneList == null)
            {
                FileMesege.sceneList = new List<DataJson.Scene>();
            }
            //选中子节点
            //循环获取
            string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
            //新建表
            DataJson.sceneInfo info = new DataJson.sceneInfo();
            
            int id = 0;
            string type = "",opt = "",optname = "",add = "";
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            int delay = 0;
            HashSet<int> hasharry = new HashSet<int>();
            //寻找最大的ID值
            foreach (DataJson.sceneInfo find in sc.sceneInfo)
            { 
                if(find.id > id)
                {
                    hasharry.Add(find.id);
                    type = find.type;
                    opt = find.opt;
                    optname = find.optName;
                    delay = find.Delay;
                    add = find.address;
                }
            }
            info.id = polishId(hasharry);
            info.pid = 0;
            info.type = type;
         
            //地址加一处理 并搜索PointList表获取地址 信息
            if (!string.IsNullOrEmpty(add) && add != "FFFFFFFF")
            {
                switch (add.Substring(2, 2))
                { 
                    case "00":
                            add = add.Substring(0,6)+SocketUtil.strtohexstr((Convert.ToInt32(add.Substring(6, 2), 16) +1).ToString());
                        break;
                    default:
                        string hexnum = SocketUtil.strtohexstr((Convert.ToInt32(add.Substring(4, 4), 16) + 1).ToString());
                        while(hexnum.Length<4)
                        {
                            hexnum = hexnum.Insert(0,"0");
                        }
                        add = add.Substring(0, 4) + hexnum;
                        break;
                }
                string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sceneSelectNode));//16进制
                //添加地域和名称 在sceneInfo表中
                DataJson.PointInfo point = DataListHelper.findPointByType_address(type, ip4 + add.Substring(2, 6));
                if (point != null)
                {
                    info.pid = point.pid;
                 }
            }

            info.address = add;
            info.opt = opt;
            info.optName = optname;
            info.Delay = delay; 
            sc.sceneInfo.Add(info);
            //排序
            sceneInfoSort(sc);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            //重新刷新
            dgvsceneAddItem();
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
            
        }

        /// <summary>
        /// 计算表中ID序号
        /// </summary>
        /// <param name="hasharry"></param>
        /// <returns></returns>
        private int polishId(HashSet<int> hasharry)
        {
            try
            {
                List<int> arry = hasharry.ToList<int>();
                arry.Sort();
                /*
                if (arry.Count == 0)
                {
                    //该区域节点前面数字不存在
                    return 1;
                }
                //哈希表 不存在序号 直接返回
                for (int i = 0; i < arry.Count; i++)
                {
                    if (arry[i] != i + 1)
                    {
                        return  i + 1;
                    }
                }
                return arry[arry.Count - 1] + 1;*/
                return arry[arry.Count - 1] + 1;
            }
            catch {
                return 1;
            }
        }

        //清空
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
            if (FileMesege.sceneSelectNode == null || FileMesege.sceneSelectNode.Parent == null)
            {
                return;
            }
            if (FileMesege.sceneList == null)
            {
                FileMesege.sceneList = new List<DataJson.Scene>();
            }
            //选中子节点
            //循环获取
            string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
            sc.sceneInfo.Clear();
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            this.dataGridView1.Rows.Clear();
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

 
        
        //载入
        private void btnDown_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.sceneSelectNode != null && FileMesege.sceneSelectNode.Parent != null)
            {
                try
                {
                    string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                    //获取该节点IP地址场景下的 场景信息对象
                    DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
                    
                    //场景信息不为空
                    if (sc.sceneInfo.Count > 0)
                    {
                        DataJson.Sn sn = new DataJson.Sn();
                        sn.action = new List<DataJson.Scenenumber>();
                        //把有效的对象操作 放到SN对象里面
                        foreach (DataJson.sceneInfo info in sc.sceneInfo)
                        {
                            //确保有信息
                            if ( string.IsNullOrEmpty(info.opt))
                            {
                                continue;
                            }
                            DataJson.Scenenumber sb = new DataJson.Scenenumber();
                            
                            sb.num = info.id;
                            sb.obj = info.address;
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
                            int i = 0;

                            while (i <10)
                            {
                                sock = ts.ConnectServer(ips[0], 6001, 1);
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
                                    TxtShow("加载成功！\r\n");
                                    break;
                                }
                                i++;
                            
                            }
                            if (sock != null)
                            {
                                sock.Close();
                            }
                            
                            if (i == 10)
                            {                           
                                TxtShow("加载失败！\r\n");
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
                catch
                {
                    //Exception ex
                    //TxtShow(ex.ToString());
                    //TxtShow("加载失败！\r\n");
                }
            }
            else
            {
                //无场景信息加载
            }
            
        }


       
        //调用
        private void btnCall_Click(object sender, EventArgs e)
        {
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.sceneSelectNode != null && FileMesege.sceneSelectNode.Parent != null)
            {
                try
                {
                    string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                    //发送调用指令
                    string ip4 = SocketUtil.getIP(FileMesege.sceneSelectNode);
                    TcpSocket ts = new TcpSocket();
              
                    sock = ts.ConnectServer(ips[0], 6003, 2);
                    if (sock == null)
                    {
                        //防止一连失败
                        sock = ts.ConnectServer(ips[0], 6003, 2);
                        if (sock == null)
                        {
                            TxtShow("连接失败！");
                            //sock.Close();
                            return;
                        }

                    }
                    string number = "";
                    if (sceneNum < 256)
                    {
                        number = String.Format("0.{0}",  sceneNum.ToString());
                    }
                    else
                    {
                        //模除剩下的数
                        int num = sceneNum % 256;
                        //有多小个256
                        sceneNum = (sceneNum - num) / 256;
                        number = String.Format("{0}.{1}", sceneNum.ToString(),num.ToString());
                    }


                    string oder = String.Format("SET;00000001;{{{0}.16.{1}}};\r\n", ip4, number);  // "SET;00000001;{" + ip4 + ".16." + number + "};\r\n";
                    int flag = ts.SendData(sock, oder, 2);
                    if (flag == 0)
                    {
                        TxtShow("发送指令成功！");
                        sock.Close();
                    }
                    else
                    {
                        //TxtShow("发送指令失败！");
                    }
                }
                catch
                {
                    //TxtShow("发送指令失败！\r\n");
                }
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
        //临时存放旧的Name名称
        //private string oldName = "";

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
                                string obj = "";
                                if (dataGridView1.Rows[rowCount].Cells[2].Value != null)
                                {
                                    //原地址
                                    obj = dataGridView1.Rows[rowCount].Cells[2].Value.ToString();
                                }
                                string objType = dataGridView1.Rows[rowCount].Cells[1].EditedFormattedValue.ToString();
                                //赋值List 并添加地域 名字
                                string address = dgvAddress(id, objType, obj);

                                break;
                            case "operation":
                                //操作
                                string info = dgvOperation(Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value), dataGridView1.Rows[rowCount].Cells[1].EditedFormattedValue.ToString());
                                if (info != null)
                                {
                                    dataGridView1.Rows[rowCount].Cells[5].Value = info;
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
                                //删除表
                                dgvDle(id);
                                //移除该行信息
                                dataGridView1.Rows.Remove(dataGridView1.Rows[rowCount]);
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

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

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
                        case "delay":
                            //改变延时
                            dgvDelay(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), Convert.ToDouble(dataGridView1.Rows[rowNum].Cells[6].Value));
                            break;
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

        //ID= 选中行的序号
        private void dgvDle(int id)
        {
    
            string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, id);
            sc.sceneInfo.Remove(info);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }

        /// <summary>
        /// 修改DGV表中 延时时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time">延时时间</param>
        private void dgvDelay(int id, Double time)
        {
            string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, id);       
            info.Delay = Convert.ToInt32( time * 10);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            
        }

        /// <summary>
        /// 修改DGV表类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private string dgvObjtype(int id,string type)
        {
            string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, id);
            if (info.pid != 0)
            {
                DataJson.PointInfo point = DataListHelper.findPointByPid(info.pid);
                if (point.type != "")
                {

                    return point.type;
                }
                
            }
            info.type = IniHelper.findTypesIniTypebyName(type);
            info.opt = "";
            info.optName = "";
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return null;
        }

        /// <summary>
        /// DGV表 操作栏
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string dgvOperation(int id,string type)
        {


            sceneConcrol dc = new sceneConcrol();
            string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, id);
            dc.Point = DataListHelper.findPointByPid(info.pid,FileMesege.PointList.equipment);
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            dc.ObjType = type;
            
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();  
                info.opt = dc.Opt;
                info.optName = dc.Ver;
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
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="obj">当前对象的值</param>
        /// <returns></returns>
        private string dgvAddress(int id ,string objType,string obj)
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
                string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
                //获取sceneInfo对象表中对应ID号info对象
                DataJson.sceneInfo info = getSceneID(sc, id);
                //地址
                info.address = dc.Obj;
                //按照地址查找type的类型 
                string type = IniHelper.findIniTypesByAddress(ips[0],info.address).Split(',')[0];
                if(string.IsNullOrEmpty(type))
                {
                    type = IniHelper.findTypesIniTypebyName(objType);
                }
                info.type = type;
                //获取树状图的IP第四位  + Address地址的 后六位
                string ad = SocketUtil.GetIPstyle(ips[0], 4) + info.address.Substring(2, 6);
                //区域加名称
                DataJson.PointInfo point = DataListHelper.findPointByType_address(type, ad);
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                if (point != null)
                {
                   
                    info.pid = point.pid;
                    info.type = point.type;
                    if (info.type != point.type)
                    {
                        info.opt = "";
                        info.optName = "";
                    }
                   


                }
                else
                {
                    info.pid = 0;
                    //info.type = "";
                    info.opt = "";
                    info.optName = "";
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            dgvsceneAddItem();
            return dc.Obj;

        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //选中行号
            int rowNum = e.RowIndex;
            //选中列号
            int columnNum = e.ColumnIndex;
            if (rowNum >= 0 && columnNum >= 0)
            {
                switch (dataGridView1.Columns[columnNum].Name)
                {
                    case "del":
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[rowNum].Selected = true;//选中行
                        break;

                    default: break;
                }
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
            if (colIndex == 5)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //撤销 
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
                //获取sceneInfo对象表中对应ID号info对象
                FileMesege.copyScene = getSceneID(sc, id);
               
            }


        }

        /// <summary>
        /// 粘贴点位的对象与参数
        /// </summary>
        public void pasteData()
        {
            try
            {
                //获取当前选中单元格的列序号
                int colIndex = dataGridView1.CurrentRow.Cells.IndexOf(dataGridView1.CurrentCell);
                
                //当粘贴选中单元格为操作
                if (colIndex == 5)
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                    string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                    //撤销 
                    //获取该节点IP地址场景下的 场景信息对象
                    DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
                    //获取sceneInfo对象表中对应ID号info对象
                    DataJson.sceneInfo sceneInfo = getSceneID(sc, id);
                    if (FileMesege.copyScene.type == ""|| sceneInfo.type == "" || sceneInfo.type != FileMesege.copyScene.type)
                    {
                        return;
                    }
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    sceneInfo.opt = FileMesege.copyScene.opt;
                    sceneInfo.optName = FileMesege.copyScene.optName;
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[5].Value = (sceneInfo.optName + " " + sceneInfo.opt).Trim();
                }//if
            }//try
            catch
            {

            }


        }
        #endregion


        #region Del按键处理
        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Delete)
                {

                    DelKeySection();
                    
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }
        private void DelKeySection()
        {
            try
            {
                //获取当前选中单元格的列序号
                int colIndex = dataGridView1.CurrentRow.Cells.IndexOf(dataGridView1.CurrentCell);

                //当粘贴选中单元格为操作
                if (colIndex == 5)
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                    string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                 
                    //获取该节点IP地址场景下的 场景信息对象
                    DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
                    //获取sceneInfo对象表中对应ID号info对象
                    DataJson.sceneInfo sceneInfo = getSceneID(sc, id);
                    
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    sceneInfo.opt = "";
                    sceneInfo.optName = "";
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[5].Value = null;
                }//if
            }//try
            catch
            {

            }

        }
        #endregion

       

     







    }
}
