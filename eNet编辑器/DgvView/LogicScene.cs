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

        //存放当前操作的的对象 
        DataJson.logicsInfo tmpLogicInfo;

        //当前选中节点的IP地址
        string ip = "";


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
        public void formInfoIni(DataJson.logicsInfo logicInfo)
        {
            tmpLogicInfo = null;
            tmpLogicInfo = logicInfo;
            ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
            cbSceneGetItem(ip);
            //执行模式
            cbAttr.SelectedIndex = logicInfo.attr;
            if (tmpLogicInfo.content == null)
            {
                cbScene.Text = "";
                dataGridView1.Rows.Clear();
                return;
            }
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(tmpLogicInfo.content);
            DataJson.scenes sc = DataListHelper.getSceneInfoListByPid(ip,logicSceneContent.pid);
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
                dgvAddItem(logicSceneContent.sceneInfo, ip);

            }

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
        private void dgvAddItem(List<DataJson.sceneInfo> sceneInfo,string ip)
        {
            try
            {
                dataGridView1.Rows.Clear();
                List<DataJson.sceneInfo> delScene = new List<DataJson.sceneInfo>();
                if (sceneInfo == null)
                {
                    return;
                }
                //循环加载该场景号的所有信息
                foreach (DataJson.sceneInfo info in sceneInfo)
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
                    //dataGridView1.Rows[dex].Cells[6].Value = Convert.ToDouble(info.delay) / 10; //状态
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
        /// 设置按钮 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecid_Click(object sender, EventArgs e)
        {
            if (tmpLogicInfo == null)
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
            //主动模式
            if (cbAttr.SelectedIndex != -1)
            {
                tmpLogicInfo.attr = cbAttr.SelectedIndex;

            }
            else
            {
                cbAttr.SelectedIndex = 1;
                tmpLogicInfo.attr = 1;
            }
           
            DataJson.LogicSceneContent logicSceneContent = new DataJson.LogicSceneContent();
            logicSceneContent.pid = sc.pid;
            logicSceneContent.sceneInfo = (List<DataJson.sceneInfo>) CommandManager.CloneObject(sc.sceneInfo);
            tmpLogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(logicSceneContent.sceneInfo, ip);
            AppTxtShow("设置参数成功");
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
                                    if (!string.IsNullOrEmpty(info))
                                    {
                                        dataGridView1.Rows[rowCount].Cells[5].Value = info;
                                    }
                                    break;
                                case "del":
                                    //删除表
                                    //dgvDel(id);

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
                                case "del":
                                    //删除表
                                    //dgvDel(id);

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
        private DataJson.sceneInfo getLogicSceneInfo(int id, DataJson.LogicSceneContent logicSceneContent)
        {
            if (logicSceneContent.sceneInfo != null)
            {
                foreach (DataJson.sceneInfo sceneInfo in logicSceneContent.sceneInfo)
                {
                    if (sceneInfo.id == id)
                    {
                        return sceneInfo;
                    }
                }
            }
            return null;

        }

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
                DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(tmpLogicInfo.content);
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
                tmpLogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvAddItem(logicSceneContent.sceneInfo,ip);
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
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(tmpLogicInfo.content);
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
                tmpLogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                return dc.Ver + " " + dc.Opt;
            }
            else
            {
                return null;
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
          

        }
        #endregion


    }///class
}
