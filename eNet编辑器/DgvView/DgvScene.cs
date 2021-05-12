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
            dgvc.ReadOnly = true;
            dgvc.Name = "type";
            
            //插入
            this.dataGridView1.Columns.Insert(1, dgvc);
            //this.dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
         
        }

        #region 刷新窗体事件
 
        public void dgvsceneAddItem()
        {
            Thread t = new Thread(ShowDatatable);
            t.IsBackground = true;
            t.Start();
        }
        #region 测试异步加载
        public delegate void FormIniDelegate();
        private void ShowDatatable()
        {
            try
            {
                this.Invoke(new FormIniDelegate(TabIni));

            }
            catch {
            }

        }


        #endregion

        //加载DgV所有信息
        public void TabIni()
        {
            this.dataGridView1.Rows.Clear();
            multipleList.Clear();
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
                    ip = FileMesege.sceneSelectNode.Parent.Text.Split(' ')[0];
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                    //string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sceneSelectNode));//16进制
                    //获取该节点IP地址场景下的 场景信息对象
                    DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
                    if (sc == null)
                    {
                        return;
                    }
                    List<DataJson.sceneInfo> delScene = new List<DataJson.sceneInfo>();
                    //循环加载该场景号的所有信息
                    foreach (DataJson.sceneInfo info in sc.sceneInfo)
                    {
                        if (addItem(info, ip) != null)
                        {
                            delScene.Add(info);
                        
                        }
                    }
                    for (int i = 0; i < delScene.Count; i++)
                    {
                        sc.sceneInfo.Remove(delScene[i]);
                    }
                    DgvMesege.RecoverDgvForm(dataGridView1, X_Value, Y_Value, rowCount, columnCount);
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

        /// <summary>
        /// 添加一行的信息资料
        /// </summary>
        /// <param name="info"></param>
        /// <param name="delScene"></param>
        /// <param name="ip"></param>
        public DataJson.sceneInfo addItem(DataJson.sceneInfo info, string ip)
        { 
            int dex = dataGridView1.Rows.Add();          
            if (info.pid == 0)
            {
                //pid号为0则为空 按地址来找
                if ( info.address != "" &&info.address != "FFFFFFFF")
                {

                    DataJson.PointInfo point = DataListHelper.findPointByType_address(info.type,info.address,ip);
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
                    dataGridView1.Rows.Remove(dataGridView1.Rows[dex]);
                    return info;
                }
                else
                {
                    //pid号有效
                    try
                    {
                        if (info.address.Substring(2, 6) != point.address.Substring(2, 6))
                        {
                            info.address = point.address;

                        }
                    }
                    catch
                    {
                        info.address = point.address;
                    }
                                    

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
            dataGridView1.Rows[dex].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
            dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(info.address,ip); 
            dataGridView1.Rows[dex].Cells[5].Value = (info.optName + " " + info.opt).Trim();
            dataGridView1.Rows[dex].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
            dataGridView1.Rows[dex].Cells[7].Value = "删除";

            return null;
                    
        }

        public void clearDgvClear()
        {
            dataGridView1.Rows.Clear();
        }
        #endregion

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

    
      
        /*
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
        }*/

     

        #endregion

        #region 增加 清空 载入 调用 删除选中
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
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
                //新建表
                DataJson.sceneInfo info = new DataJson.sceneInfo();
            
                int id = 0;
                string type = "",opt = "",optname = "",add = "";
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                int delay = 0;
                if (sc.sceneInfo.Count > 0)
                {
                    id = sc.sceneInfo[sc.sceneInfo.Count - 1].id;
                    type = sc.sceneInfo[sc.sceneInfo.Count - 1].type;
                    opt = sc.sceneInfo[sc.sceneInfo.Count - 1].opt;
                    optname = sc.sceneInfo[sc.sceneInfo.Count - 1].optName;
                    delay = sc.sceneInfo[sc.sceneInfo.Count - 1].delay;
                    add = sc.sceneInfo[sc.sceneInfo.Count - 1].address;
                }
            
                info.id = id +1 ;
                info.pid = 0;
                info.type = type;
            
                info.opt = opt;
                info.optName = optname;
                info.delay = delay; 
                //地址加一处理 并搜索PointList表获取地址 信息
                if (!string.IsNullOrEmpty(add) && add != "FFFFFFFF")
                {
                    switch (add.Substring(2, 2))
                    { 
                        case "00":
                                add = add.Substring(0,6)+ToolsUtil.strtohexstr((Convert.ToInt32(add.Substring(6, 2), 16) +1).ToString());
                            break;
                        default:
                            string hexnum = ToolsUtil.strtohexstr((Convert.ToInt32(add.Substring(4, 4), 16) + 1).ToString());
                            while(hexnum.Length<4)
                            {
                                hexnum = hexnum.Insert(0,"0");
                            }
                            add = add.Substring(0, 4) + hexnum;
                            break;
                    }
                    //按照地址查找type的类型 
                    type = IniHelper.findIniTypesByAddress(ip, add).Split(',')[0];

                    info.type = type;
                    //添加地域和名称 在sceneInfo表中
                    DataJson.PointInfo point = DataListHelper.findPointByType_address("",add,ip);
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
                    }
                }
                info.address = add;
                //添加操作

                sc.sceneInfo.Add(info);
           

                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                addItem(info,ip);
                DgvMesege.selectLastCount(dataGridView1);
                //第一行为空的话就 弹出对象选择框
                if (info.id == 1 && string.IsNullOrEmpty(info.address))
                {
                    dgvAddress(1, "", "");
                }
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
        
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
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
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                    //获取该节点IP地址场景下的 场景信息对象
                    DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
                    
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
                            sb.optname = info.optName;
                            sb.delay = info.delay;
                            sn.action.Add(sb);
                            
                        }
                        if (sn.action.Count > 0)
                        {
                            //序列化SN对象
                            string sjson = JsonConvert.SerializeObject(sn);
                            
                            //写入数据格式
                            string path = "down /json/s" + sceneNum + ".json$" + sjson;
                            //测试写出文档
                            //File.WriteAllText(FileMesege.filePath + "\\objs\\s" + sceneNum + ".json", path);
                            //string check = "exist /json/s" + sceneNum + ".json$";
                            TcpSocket ts = new TcpSocket(); 
                            int i = 0;

                            while (i <10)
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
                            AppTxtShow("无场景指令！");
                        }

                    }
                    else
                    {
                        AppTxtShow("无场景指令！");
                    }
                    
                }
                catch
                {
                    //Exception ex
                    //TxtShow(ex.ToString());
                    //TxtShow("加载失败！\r\n");
                }
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
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                    //发送调用指令
                    string ip4 = ToolsUtil.getIP(FileMesege.sceneSelectNode);
                    TcpSocket ts = new TcpSocket();
              
                    sock = ts.ConnectServer(ip, 6003, 2);
                    if (sock == null)
                    {
                        //防止一连失败
                        sock = ts.ConnectServer(ip, 6003, 2);
                        if (sock == null)
                        {
                            AppTxtShow("连接失败！");
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

            
            
        }

        //删除选中
        private void btnDel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if ((bool)dataGridView1.Rows[i].Cells[8].EditedFormattedValue)
                {
                    Multiple(i);
                }
            }
            if (dataGridView1.RowCount < 1 || multipleList.Count == 0 )
            {
                //没有选中数据
                return;
            }
            else
            {
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
                foreach (DataJson.sceneInfo info in multipleList)
                {
                    sc.sceneInfo.Remove(info);
                }
                for (int i = 0; i < sc.sceneInfo.Count; i++)
                {
                    sc.sceneInfo[i].id = i + 1;
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                multipleList.Clear();
                dgvsceneAddItem();
              
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

            if (DgvMesege.endDataViewCurrent(dataGridView1, e.Y, e.X))
            {
                isClick = false;
            }

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
            try
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
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

      

        private void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            try{
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
                                dgvAddress(id, objType, obj);

                                break;
                            case "operation":
                                //操作
                                string info = dgvOperation(id, dataGridView1.Rows[rowCount].Cells[1].EditedFormattedValue.ToString());
                                if (info != null)
                                {
                                    dataGridView1.Rows[rowCount].Cells[5].Value = info;
                                }
                                break;
                            case "del":
                                //删除表
                                dgvDel(id);
                           
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
                            case "delay":
                                dataGridView1.Columns[6].ReadOnly = false;
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
                                dgvDel(id);
                              
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
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        string tmodelay = "";
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
                        if (dataGridView1.SelectedRows.Count != 1)
                        {
                            for (int i = dataGridView1.SelectedRows.Count; i > 0; i--)
                            {
                                if (!Convert.ToBoolean(dataGridView1.SelectedRows[i - 1].Cells[8].Value))
                                {
                                    dataGridView1.SelectedRows[i - 1].Cells[8].Value = true;

                                }

                            }
                            //提交编辑
                            dataGridView1.EndEdit();
                        }
                        
                        break;
                    case "delay":
                        
                        if (dataGridView1.Rows[rowNum].Cells[6].Value != null )
                        {
                            tmodelay = dataGridView1.Rows[rowNum].Cells[6].Value.ToString();
                        }
                    
                        break;

                    default: break;
                }
            }
        }

        //存储删除选中行信息点
        HashSet<DataJson.sceneInfo> multipleList = new HashSet<DataJson.sceneInfo>();

        private void Multiple(int rowNumber)
        {
            if (!(bool)dataGridView1.Rows[rowNumber].Cells[8].EditedFormattedValue)
            {
                return;
            }
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, Convert.ToInt32(dataGridView1.Rows[rowNumber].Cells[0].Value));
            multipleList.Add(info);
            //sc.sceneInfo.Remove(info);
            
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
                            dataGridView1.Columns[6].ReadOnly = true;
                            if (dataGridView1.Rows[rowNum].Cells[6].Value != null && Validator.IsNumber(dataGridView1.Rows[rowNum].Cells[6].Value.ToString()))
                            {
                                //改变延时
                                dgvDelay(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), Convert.ToDouble(dataGridView1.Rows[rowNum].Cells[6].Value));
                            }
                            else
                            {
                                dataGridView1.Rows[rowNum].Cells[6].Value = tmodelay;
                                AppTxtShow("延时格式错误，请正确填写！");
                            }
                            break;
                        /*case "type":
                            //改变对象  
                            string isChange = dgvObjtype(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), dataGridView1.Rows[rowNum].Cells[1].EditedFormattedValue.ToString());
                            if (!string.IsNullOrEmpty(isChange))
                            {
                                dataGridView1.Rows[rowNum].Cells[1].Value = IniHelper.findTypesIniNamebyType(isChange);
                            }   
                            break;*/
                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        //ID= 选中行的序号
        private void dgvDel(int id)
        {
    
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, id);
            sc.sceneInfo.Remove(info);
            for (int i = 0; i < sc.sceneInfo.Count; i++)
            {
                sc.sceneInfo[i].id = i + 1;
            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dataGridView1.Rows.Remove(dataGridView1.Rows[id - 1]);
            for (int i = id - 1; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i + 1);
            }

        }

        /// <summary>
        /// 对象跳转获取 场景 定时 编组 point点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataJson.PointInfo dgvJumpSet(int id )
        {
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, id);

            if (string.IsNullOrEmpty(info.address))
            {
                return null;
            }
            if (info.type == "3.0_logic" || info.type == "4.0_scene" || info.type == "5.0_timer" || info.type == "6.1_panel" || info.type == "6.2_sensor")
            {
                return DataListHelper.findPointByType_address(info.type,info.address,ip);
            }

            return null;

        }


        /// <summary>
        /// 修改DGV表中 延时时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time">延时时间</param>
        private void dgvDelay(int id, Double time)
        {
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, id);
            info.delay = Convert.ToInt32(time * 10);
            dataGridView1.Rows[id - 1].Cells[6].Value = Convert.ToDouble(info.delay) / 10;

            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            
        }

        /// <summary>
        /// 修改DGV表类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /*private string dgvObjtype(int id,string type)
        {
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
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
        }*/

        /// <summary>
        /// DGV表 操作栏
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string dgvOperation(int id,string type)
        {


            sceneConcrol sceneconcrol = new sceneConcrol();
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //撤销 
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
            //获取sceneInfo对象表中对应ID号info对象
            DataJson.sceneInfo info = getSceneID(sc, id);
            sceneconcrol.Point = DataListHelper.findPointByPid(info.pid);
            //把窗口向屏幕中间刷新
            sceneconcrol.StartPosition = FormStartPosition.CenterParent;
            sceneconcrol.IP = ip;
            sceneconcrol.ObjType = type;
            sceneconcrol.Opt = info.opt;
            sceneconcrol.Ver = info.optName;
            sceneconcrol.ShowDialog();
            if (sceneconcrol.DialogResult == DialogResult.OK)
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();  
                info.opt = sceneconcrol.Opt;
                info.optName = sceneconcrol.Ver;
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                return sceneconcrol.Ver + " " + sceneconcrol.Opt;
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
        /// <param name="addressType">当前对象的类型</param>
        /// <param name="address">当前对象的地址值</param>
        /// <returns></returns>
        private void dgvAddress(int id ,string nowType,string address)
        {
            sceneAddress dc = new sceneAddress();
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            dc.IP = ip;
            dc.Address = address;
            dc.NowType = nowType;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
                //获取sceneInfo对象表中对应ID号info对象
                DataJson.sceneInfo info = getSceneID(sc, id);
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //地址
                if (string.IsNullOrEmpty(dc.Address))
                {
                    return;
                }
               
                //按照地址查找type的类型 只限制于设备
                string rtType = IniHelper.findIniTypesByAddress(ip, dc.Address).Split(',')[0];
                if(string.IsNullOrEmpty(rtType))
                {
                    MessageBox.Show("寻找不到该地址类型的ini文件");
                    return;
                    
                }
                info.address = dc.Address;

                //区域加名称
                DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                
                if (point != null)
                {
                    info.pid = point.pid;
                    if (info.type != point.type)
                    {
                        List<string> optList = IniHelper.findTypesIniCommandbyType(rtType);
                        if (optList != null)
                        {
                            info.optName = optList[0];
                            info.opt = optList[1];

                        }
                        else
                        {
                            info.optName = "";
                            info.opt = "";

                        }
                    }

                }
                else
                {
                    //搜索一次dev表 
                    info.pid = 0;
                    if (info.type != rtType)
                    {
                        List<string> optList = IniHelper.findTypesIniCommandbyType(rtType);
                        if (optList != null)
                        {
                            info.optName = optList[0];
                            info.opt = optList[1];

                        }
                        else
                        {
                            info.optName = "";
                            info.opt = "";

                        }
                    }
                   
                }
                //确定类型
                info.type = rtType;

                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }

            dgvsceneAddItem();
            

        }

        
       
        #endregion

        #region 复制 粘贴
        /// <summary>
        /// 复制点位的对象 与参数 
        /// </summary>
        public void copyData()
        {
            if (dataGridView1.CurrentCell == null)
            {
                return;
            }
            //获取当前选中单元格的列序号
            int colIndex = dataGridView1.CurrentRow.Cells.IndexOf(dataGridView1.CurrentCell);
            //当粘贴选中单元格为操作
          
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //撤销 
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
                //获取sceneInfo对象表中对应ID号info对象
                FileMesege.copyScene = getSceneID(sc, id);
 
        }

        /// <summary>
        /// 粘贴点位的对象与参数
        /// </summary>
        public void pasteData()
        {
            try
            {
                if (FileMesege.copyScene == null)
                {
                    return;
                }
                bool ischange = false;
                bool isAddress = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                //场景号
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //选中行 序号
                int id = 0;
                //列号
                int colIndex = 0;
                int j = 0;
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                    //当粘贴选中单元格为操作
                    if (colIndex == 5)
                    {
                        if (isAddress)
                        {
                            continue;
                        }
                        //获取sceneInfo对象表中对应ID号info对象
                        DataJson.sceneInfo sceneInfo = getSceneID(sc, id);
                        if (string.IsNullOrEmpty(FileMesege.copyScene.type) || string.IsNullOrEmpty(sceneInfo.type) || sceneInfo.type != FileMesege.copyScene.type)
                        {
                            //类型不一致 并且为空
                            continue;
                        }
                        ischange = true;
                        sceneInfo.opt = FileMesege.copyScene.opt;
                        sceneInfo.optName = FileMesege.copyScene.optName;
                        
                        dataGridView1.Rows[id -1].Cells[5].Value = (sceneInfo.optName + " " + sceneInfo.opt).Trim();
                    }//if
                    else if (colIndex == 2)
                    {
                        isAddress = true;
                        if (string.IsNullOrEmpty(FileMesege.copyScene.address) && FileMesege.copyScene.address == "FFFFFFFF")
                        {
                            continue;
                        }
                        DataJson.sceneInfo sceneInfo = getSceneID(sc, id);
                        sceneInfo.address = DgvMesege.addressAdd(FileMesege.copyScene.address,j);
                        sceneInfo.type = IniHelper.findIniTypesByAddress(ip, sceneInfo.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", sceneInfo.address, ip);
                        if (point != null)
                        {
                            sceneInfo.pid = point.pid;
                            sceneInfo.type = point.type;
                            dataGridView1.Rows[id -1].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id -1].Cells[4].Value = point.name;
                        }
                        else
                        {
                            sceneInfo.pid = 0;
                        }
                        sceneInfo.opt = FileMesege.copyScene.opt;
                        sceneInfo.optName =FileMesege.copyScene.optName;
                        dataGridView1.Rows[id - 1].Cells[1].Value = IniHelper.findTypesIniNamebyType(sceneInfo.type);
                        dataGridView1.Rows[id - 1].Cells[2].Value = DgvMesege.addressTransform(sceneInfo.address,ip);
                        dataGridView1.Rows[id - 1].Cells[5].Value = (sceneInfo.optName + " " + sceneInfo.opt).Trim();
                        dataGridView1.Rows[id - 1].Cells[6].Value = Convert.ToDouble(sceneInfo.delay) / 10;
                        ischange = true;
                        j++;
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

        public void Same()
        {
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                //场景号
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //选中行 序号
                int id = 0;
                //列号
                int colIndex = 0;
                //记录第一个选中格的列号
                int FirstColumnIndex = -1;
               
                DataJson.sceneInfo sceneInfo = null;
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
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
                    sceneInfo = getSceneID(sc, id);
                    if (sceneInfo == null)
                    {
                        continue;
                    }
                    if (i == dataGridView1.SelectedCells.Count - 1)
                    {
                        //记录第一个选中格内容
                        FirstColumnIndex = colIndex;
                        FileMesege.copyScene = sceneInfo;
                        continue;
                    }
                    //当粘贴选中单元格为操作
                    if (colIndex == 5)
                    {
                   
                        //获取sceneInfo对象表中对应ID号info对象
                        if (string.IsNullOrEmpty(FileMesege.copyScene.type) || string.IsNullOrEmpty(sceneInfo.type) || sceneInfo.type != FileMesege.copyScene.type)
                        {
                            //类型不一致 并且为空
                            continue;
                        }
                        ischange = true;
                        sceneInfo.opt = FileMesege.copyScene.opt;
                        sceneInfo.optName = FileMesege.copyScene.optName;
                        dataGridView1.Rows[id - 1].Cells[5].Value = (sceneInfo.optName + " " + sceneInfo.opt).Trim();
                    }//if
                    else if (colIndex == 2)
                    {
                        //选中单元格为地址
                        sceneInfo.address = FileMesege.copyScene.address;
                        if (FileMesege.copyScene.type != sceneInfo.type)
                        {
                            //类型不一样清空类型
                            sceneInfo.opt = string.Empty;
                            sceneInfo.optName = string.Empty;
                        }
                        sceneInfo.type = FileMesege.copyScene.type;
                        sceneInfo.pid = FileMesege.copyScene.pid;
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByPid(sceneInfo.pid);
                        if (point != null)
                        {
                            sceneInfo.type = point.type;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id - 1].Cells[4].Value = point.name;
                        }
                        else
                        {
                            sceneInfo.pid = 0;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id - 1].Cells[4].Value = string.Empty;
                        }
                        dataGridView1.Rows[id - 1].Cells[1].Value = IniHelper.findTypesIniNamebyType(sceneInfo.type);
                        dataGridView1.Rows[id - 1].Cells[2].Value = DgvMesege.addressTransform(sceneInfo.address,ip);
                        dataGridView1.Rows[id - 1].Cells[5].Value = (sceneInfo.optName + " " + sceneInfo.opt).Trim();
                       
                        ischange = true;
                     
                    }
                    else if (colIndex == 6)
                    {
                        //延时相同
                        ischange = true;
                        sceneInfo.delay = FileMesege.copyScene.delay;
                        dataGridView1.Rows[id - 1].Cells[6].Value = Convert.ToDouble(sceneInfo.delay) / 10;
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
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                //场景号
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //选中行 序号
                int id = 0;
                //列号
                int colIndex = 0;
                //记录第一个选中格的列号
                int FirstColumnIndex = -1;
                //地址添加量
                int addCount = 0;
                DataJson.sceneInfo sceneInfo = null;

                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                        sceneInfo = getSceneID(sc, id);
                        if (sceneInfo == null)
                        {
                            return;
                        }
                        FileMesege.copyScene = sceneInfo;
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
                    id = dataGridView1.SelectedCells[i].RowIndex+1;//Convert.ToInt32(].Cells[0].Value);
                    sceneInfo = getSceneID(sc, id);
                    if (sceneInfo == null)
                    {
                        continue;
                    }
 
                    //延时递增
                    if (colIndex == 6)
                    {
        
                        ischange = true;
                        sceneInfo.delay = FileMesege.copyScene.delay + addCount * Convert.ToInt32( FileMesege.AsDesCendingNum*10) ;
                        dataGridView1.Rows[id - 1].Cells[6].Value = Convert.ToDouble(sceneInfo.delay) / 10;
                    }//if
                    else if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(FileMesege.copyScene.address) || FileMesege.copyScene.address == "FFFFFFFF")
                        {
                            continue;
                        }
                        if(!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }
                        if (addCount == 0)
                        {
                            continue;
                        }
                        sceneInfo.address = DgvMesege.addressAdd(FileMesege.copyScene.address, addCount * Convert.ToInt32( FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        sceneInfo.type = IniHelper.findIniTypesByAddress(ip, sceneInfo.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", sceneInfo.address, ip);
                        if (point != null)
                        {
                            sceneInfo.pid = point.pid;
                            sceneInfo.type = point.type;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id - 1].Cells[4].Value = point.name;
                        }
                        else
                        {
                            sceneInfo.pid = 0;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id - 1].Cells[4].Value = string.Empty;
                        }
                        sceneInfo.opt = string.Empty;
                        sceneInfo.optName = string.Empty;
                        dataGridView1.Rows[id - 1].Cells[1].Value = IniHelper.findTypesIniNamebyType(sceneInfo.type);
                        dataGridView1.Rows[id - 1].Cells[2].Value = DgvMesege.addressTransform(sceneInfo.address,ip);
                        dataGridView1.Rows[id - 1].Cells[5].Value = (sceneInfo.optName + " " + sceneInfo.opt).Trim();
                        //dataGridView1.Rows[id - 1].Cells[6].Value = Convert.ToDouble(sceneInfo.delay) / 10;
                        ischange = true;

                    }
                    addCount --;
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

        public void Descending()
        {
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                //场景号
                int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //选中行 序号
                int id = 0;
                //列号
                int colIndex = 0;
                //记录第一个选中格的列号
                int FirstColumnIndex = -1;
                //地址减少量
                int reduceCount = 0;
                DataJson.sceneInfo sceneInfo = null;

                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                        sceneInfo = getSceneID(sc, id);
                        if (sceneInfo == null)
                        {
                            return;
                        }
                        FileMesege.copyScene = sceneInfo;
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
                    sceneInfo = getSceneID(sc, id);
                    if (sceneInfo == null)
                    {
                        continue;
                    }

                    //延时递减
                    if (colIndex == 6)
                    {

                        ischange = true;
                        sceneInfo.delay = FileMesege.copyScene.delay - reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum * 10);
                        if (sceneInfo.delay < 0)
                        {
                            sceneInfo.delay = 0;
                        }
                        dataGridView1.Rows[id - 1].Cells[6].Value = Convert.ToDouble(sceneInfo.delay) / 10;
                    }//if
                    else if (colIndex == 2)
                    {
                        //地址递减
                        if (string.IsNullOrEmpty(FileMesege.copyScene.address) || FileMesege.copyScene.address == "FFFFFFFF")
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
                        sceneInfo.address = DgvMesege.addressReduce(FileMesege.copyScene.address, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        sceneInfo.type = IniHelper.findIniTypesByAddress(ip, sceneInfo.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", sceneInfo.address, ip);
                        if (point != null)
                        {
                            sceneInfo.pid = point.pid;
                            sceneInfo.type = point.type;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id - 1].Cells[4].Value = point.name;
                        }
                        else
                        {
                            sceneInfo.pid = 0;
                            dataGridView1.Rows[id - 1].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id - 1].Cells[4].Value = string.Empty;
                        }
                        sceneInfo.opt = string.Empty;
                        sceneInfo.optName = string.Empty;
                        dataGridView1.Rows[id - 1].Cells[1].Value = IniHelper.findTypesIniNamebyType(sceneInfo.type);
                        dataGridView1.Rows[id - 1].Cells[2].Value = DgvMesege.addressTransform(sceneInfo.address,ip);
                        dataGridView1.Rows[id - 1].Cells[5].Value = (sceneInfo.optName + " " + sceneInfo.opt).Trim();
                        //dataGridView1.Rows[id - 1].Cells[6].Value = Convert.ToDouble(sceneInfo.delay) / 10;
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

        #region Del按键处理
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
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;

                    //当粘贴选中单元格为操作
                    if (colIndex == 5)
                    {
                        int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                        string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                        int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                        //撤销 
                        //获取该节点IP地址场景下的 场景信息对象
                        DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
                        //获取sceneInfo对象表中对应ID号info对象
                        DataJson.sceneInfo sceneInfo = getSceneID(sc, id);
      
                        ischange = true;
                        sceneInfo.opt = "";
                        sceneInfo.optName = "";

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = null;
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


        #region 限制输入 数字 回车 退回 .按键
        private void DgvScene_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8 && e.KeyChar != (char)46 && e.KeyChar != (char)3 && e.KeyChar != (char)22 )
            {
                e.Handled = true;
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

        #region 右击菜单 升序 相同 降序
        private void 相同ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Same();
        }

        private void 升序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ascending();
        }

        private void 降序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Descending();
        }
        #endregion

    }
}
