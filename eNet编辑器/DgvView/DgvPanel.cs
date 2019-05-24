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
        DataGridViewComboBoxColumn objType ;
        DataGridViewComboBoxColumn mode ;
   
        DataGridViewComboBoxColumn showmode;

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> AppTxtShow;
        
        
        


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

            //设置列名
            objType.HeaderText = "类型";
            //设置下拉列表的默认值 
            objType.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            objType.DefaultCellStyle.NullValue = objType.Items[0];
            objType.Name = "objType";

            //设置列名
            mode.HeaderText = "操作";
            //设置下拉列表的默认值 
            mode.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
         
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
            
        }

        /// <summary>
        /// 加载DgV所有信息
        /// </summary>
        public void dgvPanelAddItem()
        {
           
            try
            {
                
                
                int tmpId = -1;
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }
                cbKeyNum.SelectedIndex = pls.keyNum - 1;
                this.dataGridView1.Rows.Clear();
               
                
                List<DataJson.panelsInfo> delPanel = new List<DataJson.panelsInfo>();
                string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.timerSelectNode));//16进制
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
                            DataJson.PointInfo point = DataListHelper.findPointByType_address(plInfo.objType, ip4 + plInfo.objAddress.Substring(2, 6));
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
                            plInfo.objAddress = point.address;
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
                    
                    dataGridView1.Rows[dex].Cells[1].Value = plInfo.keyAddress;
                    dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(plInfo.objAddress);
                    dataGridView1.Rows[dex].Cells[3].Value = IniHelper.findTypesIniNamebyType(plInfo.objType);
                    //执行模式  

                    dataGridView1.Rows[dex].Cells[6].Value = updataMode(plInfo.objType, dex,plInfo.opt);
                    dataGridView1.Rows[dex].Cells[7].Value = DgvMesege.addressTransform(plInfo.showAddress);
                    dataGridView1.Rows[dex].Cells[8].Value = plInfo.showMode;
                    dataGridView1.Rows[dex].Cells[9].Value = "删除";
                    


                }
                for(int i = 0; i < delPanel.Count; i++)
                {
                    pls.panelsInfo.Remove(delPanel[i]);
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
        /// bindinfo信息按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void panelInfoSort(DataJson.panels pls)
        {
            pls.panelsInfo.Sort(delegate(DataJson.panelsInfo x, DataJson.panelsInfo y)
            {
                return Convert.ToInt32(x.id).CompareTo(Convert.ToInt32(y.id));
            });
           
        }


        /// <summary>
        /// 更改combox的items信息
        /// </summary>
        /// <param name="objtype"></param>
        private string updataMode(string objtype, int rownum,int mode)
        {
            //敲黑板 重点  重新在某一列上添加combox
            DataGridViewComboBoxCell combox = dataGridView1.Rows[rownum].Cells["operation"] as DataGridViewComboBoxCell;
            combox.Items.Clear();
            //修改mode的combox列表
            string filepath = string.Format("{0}\\types\\{1}.ini", Application.StartupPath, objType);
            //获取ini所有keyMode的Key  mode根据TYPE决定
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
                if (keys[i] == mode.ToString())
                {
                    return IniConfig.GetValue(filepath, "keyMode", keys[i]);
                }
            }
            return "";
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

                }

            } return null;
        }

        #endregion

        #region 增加 清空  清除 设置 按键


         /*增加
        
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
                if (pls == null)
                {
                    return;
                }

                //新建表
                DataJson.panelsInfo plInfo = new DataJson.panelsInfo();

                int id = 0;
                string objType = "", keyAddress = "", objAddress = "", opt = "", showAddress = "", showMode = "";
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                if (pls.panelsInfo.Count > 0)
                {
                    id = pls.panelsInfo[pls.panelsInfo.Count - 1].id;
                    objType = pls.panelsInfo[pls.panelsInfo.Count - 1].objType;
                    keyAddress = pls.panelsInfo[pls.panelsInfo.Count - 1].keyAddress;
                    objAddress = pls.panelsInfo[pls.panelsInfo.Count - 1].objAddress;
                    opt = pls.panelsInfo[pls.panelsInfo.Count - 1].opt;
                    showAddress = pls.panelsInfo[pls.panelsInfo.Count - 1].showAddress;
                    showMode = pls.panelsInfo[pls.panelsInfo.Count - 1].showMode;
               
                }
                plInfo.id = id + 1;
                plInfo.pid = 0;
                plInfo.objType = objType;
                plInfo.opt = opt;
                plInfo.keyAddress = keyAddress;
                plInfo.showAddress = showAddress;
                plInfo.showMode = showMode;
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
                    objType = IniHelper.findIniTypesByAddress(FileMesege.timerSelectNode.Parent.Text.Split(' ')[0], objAddress).Split(',')[0];
                    if (string.IsNullOrEmpty(objType))
                    {
                        switch (objAddress.Substring(2, 2))
                        {
                            case "10":
                                objType = "4.0_scene";
                                break;
                            case "20":
                                objType = "5.0_time";
                                break;
                            case "30":
                                objType = "6.0_group";
                                break;
                            default:
                                break;
                        }
                    }
                    plInfo.objType = objType;
                    string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.panelSelectNode));//16进制
                    //添加地域和名称 在sceneInfo表中
                    DataJson.PointInfo point = DataListHelper.findPointByType_address(objType, ip4 + objAddress.Substring(2, 6));
                    if (point != null)
                    {
                        plInfo.pid = point.pid;
                        plInfo.objType = objType;
                        if (plInfo.objType != point.type)
                        {
                            //清空执行模式（操作）
                            plInfo.opt = "";
                        }
                    }
                    else
                    {
                        plInfo.pid = 0;
                        plInfo.opt = "";
                    }
                }

                plInfo.objAddress = objAddress;

                pls.panelsInfo.Add(plInfo);
                //排序
                //timerInfoSort(tms);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                //重新刷新
                dgvPanelAddItem();
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }

        }*/

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

        }

        //下载
        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        //开启
        private void btnOn_Click(object sender, EventArgs e)
        {

        }

        //关闭
        private void btnOff_Click(object sender, EventArgs e)
        {

        }
        
        /*清除
        private void btnRemove_Click(object sender, EventArgs e)
        {
            
            Socket sock = null;
            //产生场景文件写进去
            if (FileMesege.panelSelectNode == null || FileMesege.panelSelectNode.Parent == null)
            {
                return;
            }
            try
            {
                string ip = FileMesege.panelSelectNode.Parent.Text.Split(' ')[0];
                string[] ids = FileMesege.panelSelectNode.Text.Split(' ');
                int panelNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                //发送调用指令
                string ip4 = SocketUtil.getIP(FileMesege.panelSelectNode);

                TcpSocket ts = new TcpSocket();
                sock = ts.ConnectServer(ip, 6003, 2);
                if (sock == null)
                {
                    //防止一连失败
                    sock = ts.ConnectServer(ip, 6003, 2);
                    if (sock == null)
                    {
                        AppTxtShow("连接失败！");
                        return;
                    }

                }
        
                string number = "";
                if (panelNum < 256)
                {
                    number = String.Format("0.{0}", panelNum.ToString());
                }
                else
                {
                    //模除剩下的数
                    int num = panelNum % 256;
                    //有多小个256
                    panelNum = (panelNum - num) / 256;
                    number = String.Format("{0}.{1}", panelNum.ToString(), num.ToString());
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
                    flag = ts.SendData(sock, oder, 2);
                    if (flag == 0)
                    {
                        AppTxtShow("发送指令成功！");
                        sock.Close();
                    }
                }
            }
            catch
            {
                AppTxtShow("发送指令失败！");
            }
        }

      */



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
                            case "objAddress":
                               //对象地址

                                break;
                            case "operation":
                                //操作
                                break;
                           
                            case "showAddress":
                                //对象地址

                                break;
                            case "del":
                                //删除
                                dgvDel();
                                break;
                            case "add":
                                //添加
                                addInfo();
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
                            default: break;



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

                        case "showMode":
                            dgvShowMode();
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
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //添加改id的按键
            DataJson.panelsInfo plInfo = new DataJson.panelsInfo();
            plInfo.id =  Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
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
                dataGridView1.Rows[rowCount].Cells[3].Value = null;
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
        /// 设置显示模式 无 同步 反显
        /// </summary>
        private void dgvShowMode()
        {
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return;
            }

        }
        #endregion

        #region 面板键选择
        private void cbKeyNum_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void cbKeyNum_TextChanged(object sender, EventArgs e)
        {
            if (setKeyNum(Convert.ToInt32(cbKeyNum.Text)))
            {
                dgvPanelAddItem();
            }
        }

       

        /// <summary>
        /// 设置当前面板为 几键面板 成功返回true
        /// </summary>
        /// <param name="keyNum"></param>
        /// <returns></returns>
        private bool setKeyNum(int keyNum)
        {
            DataJson.panels pls = DataListHelper.getPanelsInfoListByNode();
            if (pls == null)
            {
                return false;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
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
                }
            }
            //排序
            panelInfoSort(pls);
           
         
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return true;
        }



        #endregion


        #region del删除快捷键

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        #endregion

      

 

    





    }
}
