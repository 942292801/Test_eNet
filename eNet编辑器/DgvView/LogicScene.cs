using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using eNet编辑器.Properties;
using Newtonsoft.Json;
using System.Reflection;
using eNet编辑器.AddForm;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using eNet编辑器.LogicForm;

namespace eNet编辑器.DgvView
{
    public partial class LogicScene : UserControl
    {
        public LogicScene()
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

        
        public static event Action<string> AppTxtShow;


        //当前选中节点的IP地址
        string ip = "";

        //客户端
        ClientAsync client;
        //Thread thread;

        private void LogicScene_Load(object sender, EventArgs e)
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
          
        }



        #region 窗体数据展现
        /// <summary>
        /// 总：窗体数据展现在窗体 
        /// </summary>
        public void formInfoIni()
        {
            //way1.ok
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

        private void TabIni()
        {
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                cbScene.Text = "";
                dataGridView1.Rows.Clear();
                return;
            }
            //执行模式
            cbAttr.SelectedIndex = LogicInfo.attr;
            ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
            cbSceneGetItem(ip);
            if (string.IsNullOrEmpty(LogicInfo.content))
            {
                cbScene.Text = "";
                dataGridView1.Rows.Clear();
                return;
            }

            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
            DataJson.scenes sc = DataListHelper.getSceneInfoListByPid(ip, logicSceneContent.pid);
            if (sc == null)
            {
                cbScene.Text = "";
                dataGridView1.Rows.Clear();
                return;

            }
            //获取该场景的场景名称 
            DataJson.PointInfo point = DataListHelper.findPointByPid(logicSceneContent.pid);
            if (point != null)
            {
                string section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                string sectionName = string.Format("{0}{1} {2} {3}", Resources.Scene, sc.id, section, point.name);
                cbScene.Text = sectionName;
                //加载DGV信息
                dgvAddItem(logicSceneContent.SceneItemInfo, ip);
                //thread = new Thread(new ThreadStart(ThreadMethod)); //创建线程                 
                //thread.Start(); //启动线程

            }
            DgvMesege.RecoverDgvForm(dataGridView1, X_Value, Y_Value, rowCount, columnCount);
        }

        #endregion

        /// <summary>
        /// 创建线程链接 链接tcp6003端口 刷新状态回来
        /// </summary>
        private void ThreadMethod()
        {
            //刷新状态
            Connet6003();
            //thread.Abort();
        }


        /// <summary>
        /// cbScene获取当前IP存在的场景 和 区域进行筛选
        /// </summary>
        public void cbSceneGetItem(string ip)
        {
            cbScene.Items.Clear();

            string section = "";
            //筛选地域
            if (FileMesege.sceneList != null)
            {
                foreach (DataJson.Scene sc in FileMesege.sceneList)
                {
                    //  添加该网关IP的子节点
                    if (sc.IP == ip)
                    {
                        DataJson.PointInfo point = null;
                        if (string.IsNullOrEmpty(FileMesege.sectionNodeCopy) || FileMesege.sectionNodeCopy.Contains("查看所有区域"))
                        {
                            foreach (DataJson.scenes scs in sc.scenes)
                            {
                                point = DataListHelper.findPointByPid(scs.pid, FileMesege.PointList.scene);
                                if (point != null)
                                {
                                    section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                    cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));

                                }
                            }
                        }
                        else
                        {
                            //区域
                            string[] sections = FileMesege.sectionNodeCopy.Split('\\');
                            foreach (DataJson.scenes scs in sc.scenes)
                            {
                                point = DataListHelper.findPointByPid(scs.pid, FileMesege.PointList.scene);
                                if (point != null)
                                {
                                    
                                    section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                    // 1 2 3 4 用于区分点击区域 区分加载搜索信息 例如东区 把东区所有信息加载进来 
                                    if (string.IsNullOrEmpty(sections[1]) && point.area1 == sections[0])
                                    {
                                        //添加信息
                                        cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));
                                    }
                                    // 1 2 3 0
                                    else if (string.IsNullOrEmpty(sections[2]) && point.area1 == sections[0] && point.area2 == sections[1])
                                    {
                                        //添加信息
                                        cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));
                                    }
                                    // 1 2 0 0
                                    else if (string.IsNullOrEmpty(sections[3]) && point.area1 == sections[0] && point.area2 == sections[1] && point.area3 == sections[2])
                                    {
                                        //添加信息
                                        cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));
                                    }
                                    //1 0 0 0
                                    else if (point.area1 == sections[0] && point.area2 == sections[1] && point.area3 == sections[2] && point.area4 == sections[3])
                                    {
                                        //添加信息
                                        cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));
                                    }
                                    

                                }
                            }
                        }


                    }
                }
            }

            
        }

        /// <summary>
        /// dgv表格展现数据
        /// </summary>
        /// <param name="sceneInfo"></param>
        /// <param name="ip"></param>
        private void dgvAddItem(List<DataJson.SceneItem> sceneInfo, string ip)
        {
            try
            {
                dataGridView1.Rows.Clear();
                List<DataJson.SceneItem> delScene = new List<DataJson.SceneItem>();
                if (sceneInfo == null)
                {
                    return;
                }
                //循环加载该场景号的所有信息
                foreach (DataJson.SceneItem info in sceneInfo)
                {
                    int dex = dataGridView1.Rows.Add();

                    if (info.pid == 0)
                    {
                        //pid号为0则为空 按地址来找
                        if (info.address != "" && info.address != "FFFFFFFF")
                        {

                            DataJson.PointInfo point = DataListHelper.findPointByType_address(info.type, info.address, ip);
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
                    dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView1.Rows[dex].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView1.Rows[dex].Cells[5].Value = (info.optName + " " + info.opt).Trim();
                    dataGridView1.Rows[dex].Cells[6].Value = info.state; //状态
                    dataGridView1.Rows[dex].Cells[7].Value = "删除";
                }
                for (int i = 0; i < delScene.Count; i++)
                {
                    sceneInfo.Remove(delScene[i]);
                }
                

            }
            catch (Exception ex)
            {
                this.dataGridView1.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }
        }


        
        #endregion

       

        #region 设置执行模式和场景

        /// <summary>
        /// 设置执行模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAttr_Click(object sender, EventArgs e)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }

            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //主动模式
            if (cbAttr.SelectedIndex != -1)
            {
                LogicInfo.attr = cbAttr.SelectedIndex;

            }
            else
            {
                cbAttr.SelectedIndex = 1;
                LogicInfo.attr = 1;
            }

            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            AppTxtShow("设置执行模式成功");
        }

        /// <summary>
        /// 设置场景按钮 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecid_Click(object sender, EventArgs e)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null )
            {
                return;
            }
          

            string ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
            int sceneNum = Validator.GetNumber(cbScene.Text.Split(' ')[0]);
            if (sceneNum == -1)
            {
                //场景号不存在
                return;
            }
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
            if (sc == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            /*
            //主动模式
            if (cbAttr.SelectedIndex != -1)
            {
                LogicInfo.attr = cbAttr.SelectedIndex;

            }
            else
            {
                cbAttr.SelectedIndex = 1;
                LogicInfo.attr = 1;
            }*/

            DataJson.LogicSceneContent logicSceneContent = new DataJson.LogicSceneContent();
            logicSceneContent.pid = sc.pid;
            logicSceneContent.SceneItemInfo.Clear();
            List<DataJson.sceneInfo> list = (List<DataJson.sceneInfo>)CommandManager.CloneObject(sc.sceneInfo);
            foreach (DataJson.sceneInfo tmp in list)
            {
                DataJson.SceneItem item = new DataJson.SceneItem();
                item = CommandManager.AutoCopy<DataJson.sceneInfo, DataJson.SceneItem>(tmp);
                logicSceneContent.SceneItemInfo.Add(item);
            }
            //对象反编译为json字符串
            LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(logicSceneContent.SceneItemInfo, ip);
            AppTxtShow("设置场景参数成功");
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


        //移动到格子的时候高亮一行
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
            try
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
                                    /*
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
                                    */
                                    break;
                                case "operation":
                                    /*
                                    //操作
                                    string info = dgvOperation(id, dataGridView1.Rows[rowCount].Cells[1].EditedFormattedValue.ToString());
                                    if (!string.IsNullOrEmpty(info))
                                    {
                                        dataGridView1.Rows[rowCount].Cells[5].Value = info;
                                    }*/
                                    break;
                                case "del":
                                    //删除表
                                    dgvDel(id);

                                    break;
                                case "state":
                                    //状态表
                                    //展示状态窗口
                                    string val =  DgvStateShow(id);
                                    if (!string.IsNullOrEmpty(val))
                                    {
                                        dataGridView1.Rows[rowCount].Cells[6].Value = val;
                                    }
                                    break;
                                case "num":
                                    //设置对象跳转
                                    /*
                                    DataJson.PointInfo point = dgvJumpSet(id);
                                    if (point != null)
                                    {
                                        //传输到form窗口控制
                                        //AppTxtShow("传输到主窗口"+DateTime.Now);
                                        jumpSetInfo(point);
                                    }*/
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
                                case "address":
                                    //setTitleAddress();
                                    break;
                                case "section":
                                    //setTitleAddress();
                                    break;
                                case "name":
                                    //setTitleAddress();
                                    break;
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

        /// <summary>
        /// 根据DGV表中的ID号获取该行的对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataJson.SceneItem getLogicSceneInfo(int id, DataJson.LogicSceneContent logicSceneContent)
        {
            if (logicSceneContent.SceneItemInfo != null)
            {
                foreach (DataJson.SceneItem sceneInfo in logicSceneContent.SceneItemInfo)
                {
                    if (sceneInfo.id == id)
                    {
                        return sceneInfo;
                    }
                }
            }
            return null;

        }

        #region 屏蔽功能
        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="obj">当前对象的地址值</param>
        /// <returns></returns>
        private void dgvAddress(int id, string objType, string obj)
        {
            LogicAddress dc = new LogicAddress();
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            //dc.Obj = obj;
            dc.ObjType = objType;
            dc.Obj = obj;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
                DataJson.sceneInfo info = getLogicSceneInfo(id, logicSceneContent);
                if (info == null)
                {
                    return;
                }
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                info.address = dc.Obj;
                if (dc.RtType == "15.0_LocalVariable")
                {
                    info.pid = 0;
                    if (info.type != "15.0_LocalVariable")
                    {
                        info.type = "15.0_LocalVariable";
                        info.opt = "";
                        info.optName = "";
                    }

                }
                else
                {
                    //按照地址查找type的类型 只限制于设备
                    string type = IniHelper.findIniTypesByAddress(ip, info.address).Split(',')[0];
                    if (string.IsNullOrEmpty(type))
                    {
                        type = dc.RtType;

                    }
                    //区域加名称
                    DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                    if (point != null)
                    {
                        info.pid = point.pid;
                        if (info.type != point.type)
                        {
                            info.opt = "";
                            info.optName = "";
                            info.type = point.type;
                        }
                    }
                    else
                    {
                        //搜索一次dev表 
                        info.pid = 0;
                        if (info.type != type)
                        {
                            info.opt = "";
                            info.optName = "";
                            info.type = type;
                        }

                    }


                }
                LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvAddItem(logicSceneContent.SceneItemInfo, ip);
            }//ok
                
            
        }


        /// <summary>
        /// DGV表 操作栏 返回操作信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string dgvOperation(int id, string type)
        {


            LogicConcrol dc = new LogicConcrol();
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return null;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
            DataJson.sceneInfo info = getLogicSceneInfo(id, logicSceneContent);
            if (info == null)
            {
                return null;
            }
            
            dc.Point = DataListHelper.findPointByPid(info.pid);
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            dc.ObjType = type;
            dc.Opt = info.opt;
            dc.Ver = info.optName;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                info.opt = dc.Opt;
                info.optName = dc.Ver;
                LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
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
        /// 复制title选中的节点 赋地址给Address
        /// </summary>
        private void setTitleAddress()
        {

            int colIndex = dataGridView1.SelectedCells[0].ColumnIndex;
            int id = dataGridView1.CurrentCell.RowIndex;
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
            DataJson.sceneInfo info = getLogicSceneInfo(id + 1, logicSceneContent);
            if (info == null)
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

            if (eq.type == info.type)
            {
                info.pid = eq.pid;

                info.address = eq.address;


                dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                dataGridView1.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                dataGridView1.Rows[id].Cells[4].Value = eq.name;

            }
            else
            {
                info.pid = eq.pid;

                info.address = eq.address;
                info.type = eq.type;
                info.opt = "";
                info.optName = "";

                dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                dataGridView1.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                dataGridView1.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                dataGridView1.Rows[id].Cells[4].Value = eq.name;
                dataGridView1.Rows[id].Cells[5].Value = "";



            }



            LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);


        }
        #endregion

        /// <summary>
        /// 状态框界面显示
        /// </summary>
        private string DgvStateShow(int id)
        {
            LogicState dns = new LogicState();
            if (dataGridView1.Rows[rowCount].Cells[1] == null || dataGridView1.Rows[rowCount].Cells[2] == null)
            {
                return null;
;
            }
            dns.TypeName = dataGridView1.Rows[rowCount].Cells[1].EditedFormattedValue.ToString();
            dns.AddressDecimal = dataGridView1.Rows[rowCount].Cells[2].EditedFormattedValue.ToString();
            if (dataGridView1.Rows[rowCount].Cells[6].Value != null)
            {
                //当前状态显示值
                dns.StateValue = dataGridView1.Rows[rowCount].Cells[6].Value.ToString();
            }
            else
            {
                dns.StateValue = "";
            }
            Point pt = MousePosition;
            //把窗口向屏幕中间刷新
            dns.StartPosition = FormStartPosition.Manual;
            dns.Left = pt.X + 10;
            dns.Top = pt.Y + 10;
            dns.ShowDialog();
            

            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return null;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
            DataJson.SceneItem info = getLogicSceneInfo(id, logicSceneContent);
            if (info == null)
            {
                return null;
            }

            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (info.state == dns.ReturnVal)
            {
                return null;
            }
            info.state = dns.ReturnVal;
            LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return info.state;

         
        }

        private  void dgvDel(int id)
        {


            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return ;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
            DataJson.SceneItem info = getLogicSceneInfo(id, logicSceneContent);
            if (info == null)
            {
                return;
            }
           
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (!logicSceneContent.SceneItemInfo.Remove(info))
            {
                return;
            }
            //排序
            sceneInfoSort(logicSceneContent);
            LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(logicSceneContent.SceneItemInfo, ip);
           
        }

        /// <summary>
        /// 场景信息 scenInfo 序号重新排序赋值
        /// </summary>
        private void sceneInfoSort(DataJson.LogicSceneContent sc)
        {
            int i = 1;
            foreach (DataJson.SceneItem scinfo in sc.SceneItemInfo)
            {
                scinfo.id = i;
                i++;
            }
        }

        

        #endregion

        #region 表格新增一行回调
        private void btnIfAdd_Click(object sender, EventArgs e)
        {
            dgvAddRow();
        }

        public void dgvAddRow()
        {

            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null || LogicInfo.content == null)
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
            if (logicSceneContent.pid == 0)
            {
                return;
            }
             //新建表
            DataJson.SceneItem info = new DataJson.SceneItem();
            info.id = logicSceneContent.SceneItemInfo.Count + 1;
            info.pid = 0;
            //info.type = "";
            //info.opt = "";
            //info.optName = "";
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            logicSceneContent.SceneItemInfo.Add(info);
            LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(logicSceneContent.SceneItemInfo, ip);
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
                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
                
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

                        DataJson.sceneInfo info = getLogicSceneInfo(id, logicSceneContent);
                        if (info == null)
                        {
                            continue;
                        }

                        ischange = true;
                        info.opt = "";
                        info.optName = "";
                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = null;

                    }//if
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }

        }
        #endregion

        #region  状态获取处理

        /// <summary>
        /// 链接主机IP 6003端口
        /// </summary>
        public void Connet6003()
        {

            try
            {
                if (client != null)
                {
                    client.Dispoes();
                }
                //实例化客户端
                client = new ClientAsync();
                IniClient();
                //异步连接
                client.ConnectAsync(ip, 6003);
                if (client != null && client.Connected())
                {
                    timer1_Tick(this, EventArgs.Empty);
                    timer1.Start();
                }

            }
            catch
            {
                timer1.Stop();
                client = null;
                return;
            }
        }


        /// <summary>
        /// 5秒自动发码 一次获取设备在线状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count < 1)
                {
                    return;
                }
                if (client != null && client.Connected())
                {
                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        if (dataGridView1.Rows[j].Cells[2].Value == null)
                        {
                            continue;
                        }
                        string msg = string.Format("GET;{{{0}}};\r\n",dataGridView1.Rows[j].Cells[2].Value);
                        //客户端发送数据
                        client.SendAsync(msg);
                    }
                   
                }
            }
            catch
            {
                timer1.Stop();
                client = null;
                return;
            }

        }



        /// <summary>
        /// 初始化客户端的处理
        /// </summary>
        private void IniClient()
        {

            //实例化事件 传值到封装函数  c为函数类处理返回的client
            client.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync.EnSocketAction>((c, enAction) =>
            {
                string key = "";

                try
                {
                    if (c.Client.Connected)
                    {
                        //强转类型
                        IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                        //返回的IP 和 端口号
                        key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                    }
                }
                catch { }

                switch (enAction)
                {
                    case ClientAsync.EnSocketAction.Connect:
                        //MessageBox.Show("已经与" + key + "建立连接");
                        //btnNew.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2003;
                        //timer1.Start();

                        break;
                    case ClientAsync.EnSocketAction.SendMsg:

                        //MessageBox.Show(DateTime.Now + "：向" + key + "发送了一条消息");
                        break;
                    case ClientAsync.EnSocketAction.Close:
                        //client.Close();
                        //btnNew.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
                        //MessageBox.Show("服务端连接关闭");
                        break;
                    case ClientAsync.EnSocketAction.Error:
                        //btnNew.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
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

                    //获取FB开头的信息
                    string[] strArray = msg.Split(new string[] { "FB", "ACK" }, StringSplitOptions.RemoveEmptyEntries);
                    //MessageBox.Show(msg);
                    //Regex reg = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+)");
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        //数组信息按IP提取 
                        //Match match = reg.Match(strArray[i]);
                        string ipv4 = Validator.GetIPv4(strArray[i]);
                        if (string.IsNullOrEmpty(ipv4))
                        {
                            continue;
                        }
                        ipv4 = "254"+ Regex.Match(ipv4, @"\.(\d+)\.(\d+)\.(\d+)").Value;
                        string[] strs = strArray[i].Split(';');
                        //行数
                        for (int j = 0; j < dataGridView1.Rows.Count; j++)
                        {
                            if (dataGridView1.Rows[j].Cells[2].Value.ToString() == ipv4)
                            {
                                dataGridView1.Rows[j].Cells[6].Value = getDataType(dataGridView1.Rows[j].Cells[1].Value.ToString(), strs[1]);
                                break;
                            }
                        }
                     
                    }
                }
                catch
                {
                    //报错不处理
                    //MessageBox.Show("DgvName处理信息出错869行");
                }
            });
        }

        /// <summary>
        /// 获取FB:0000 0000 :{IP Linid ID Port } 截取不同类型的数据 
        /// </summary>
        /// <param name="typeName">类型名称（中文）</param>
        /// <param name="val">数值</param>
        /// <returns>正常返回截取后的值 否则全值返回</returns>
        private string getDataType(string typeName, string val)
        {
            if (val == "ffffffff")
            {
                return "离线";
            }
            //转换为二进制的返回值
            string binVal = DataChange.HexString2BinString(val).Replace(" ", "");
            //获取types下 ini类型名称
            string type = IniHelper.findTypesIniTypebyName(typeName);
            if (string.IsNullOrEmpty(type))
            {
                return val;
            }
            string filepath = string.Format("{0}\\types\\{1}.ini", Application.StartupPath, type);
            //获取全部Section下的Key
            List<string> list = IniConfig.ReadKeys("data", filepath);

            string[] infos = null;
            string nowState = "";
            //最后返回的信息值
            string sataename = "";
            for (int i = 0; i < list.Count; i++)
            {
                //获取类型下data的数据  rw,uint8,0-1,0-1,亮度(%)
                infos = IniConfig.GetValue(filepath, "data", list[i]).Split(',');
                //读取的value的格式不规范 直接退出不作处理 排除format
                if (infos.Length != 5)
                {
                    continue;
                }
                nowState = getBinBit(binVal, infos[2]);

                sataename = sataename + string.Format("{0}:{1} ", infos[4], nowState);

            }

            return sataename;

        }

        /// <summary>
        /// 截取二进制位数  binval为二进制数值 inset为0 / 0-1 / 位置数
        /// </summary>
        /// <param name="binval"></param>
        /// <param name="inset">位置数</param>
        /// <returns>返回十进制值</returns>
        private string getBinBit(string binval, string inset)
        {
            string bin = "";
            //截取位数 组成一个新值
            if (inset.Contains("-"))
            {
                string[] infos = inset.Split('-');
                int end = Convert.ToInt32(infos[1]);
                int start = Convert.ToInt32(infos[0]);
                //反转二进制数据
                bin = DataChange.Reversal(binval).Substring(start, end - start + 1);

            }
            else
            {
                //反转二进制数据
                bin = DataChange.Reversal(binval).Substring(Convert.ToInt32(inset), 1);

            }
            //再反转复原二进制数据
            return Convert.ToInt64(DataChange.Reversal(bin), 2).ToString();
        }


        #endregion


        #region 复制 粘贴
        /// <summary>
        /// 复制点位的对象 与参数 
        /// </summary>
        public void copyData()
        {
            /*
            //获取当前选中单元格的列序号
            int colIndex = dataGridView1.CurrentRow.Cells.IndexOf(dataGridView1.CurrentCell);
            //当粘贴选中单元格为操作
            if (colIndex == 6)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
                DataJson.SceneItem info = getLogicSceneInfo(id, logicSceneContent);
                if (info == null)
                {
                    return;
                }
                //获取sceneInfo对象表中对应ID号info对象
                FileMesege.copyLogicSceneItem = info;

            }
            */

        }

        /// <summary>
        /// 粘贴点位的对象与参数
        /// </summary>
        public void pasteData()
        {
            /*
            try
            {
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;

                    //当粘贴选中单元格为操作
                    if (colIndex == 6)
                    {
                        int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[0].Value);
                        DataJson.SceneItem info = getLogicSceneInfo(id, logicSceneContent);
                        if (info == null)
                        {
                            return;
                        }

                        if (FileMesege.copyLogicSceneItem.type == "" || info.type == "" || info.type != FileMesege.copyLogicSceneItem.type)
                        {
                            continue;
                        }
                        ischange = true;
                        info.state = FileMesege.copyLogicSceneItem.state;
                      

                        dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[6].Value = (info.state).Trim();
                    }//if
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }

            }//try
            catch
            {

            }*/


        }
        #endregion

        #region 相同

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
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
                DataJson.SceneItem info = null;
                DataJson.SceneItem copyInfo = null;
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
                    info = getLogicSceneInfo(id+1, logicSceneContent);
                    if (info == null)
                    {
                        continue;
                    }
                    if (i == dataGridView1.SelectedCells.Count - 1)
                    {
                        //记录第一个选中格内容
                        FirstColumnIndex = colIndex;
                        copyInfo = info;
                        continue;
                    }
                    //当粘贴选中单元格
                    if (colIndex == 6)
                    {
                        //状态操作 
                        //获取sceneInfo对象表中对应ID号info对象
                        if (string.IsNullOrEmpty(copyInfo.type) || string.IsNullOrEmpty(info.type) || info.type != copyInfo.type)
                        {
                            //类型不一致 并且为空
                            continue;
                        }
                        ischange = true;
                        info.state = copyInfo.state;
                        dataGridView1.Rows[id].Cells[6].Value = (info.state).Trim();
                    }//if
                  


                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
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

        


    }///class
}
