using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using eNet编辑器.AddForm;
using eNet编辑器.LogicForm;
using eNet编辑器.Properties;
using System.Threading;

namespace eNet编辑器.DgvView
{
    public partial class LogicCondition : UserControl
    {
        public LogicCondition()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
            doubleBuffered(dataGridView1);
            doubleBuffered(dataGridView2);
            doubleBuffered(dataGridView3);
        }

        #region 利用反射设置DataGridView的双缓冲
        private void doubleBuffered(DataGridView dataGridView)
        {
            //利用反射设置DataGridView的双缓冲
            Type dgvType = dataGridView.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView, true, null);
        }
        #endregion

        public static event Action<string> AppTxtShow;

        //当前选中节点的IP地址
        string ip = "";
        /// <summary>
        /// 临时记录上一次延时参数 填入错误后返回值
        /// </summary>
        string tmpDelay = "";
        DataGridViewComboBoxColumn cbOperation;

        private void LogicCondition_Load(object sender, EventArgs e)
        {
            dgvAddColumn();
            //设置自动换行

            this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            //设置自动调整高度

            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; 
        }


        #region 窗体添加信息
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

                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                dataGridView3.Rows.Clear();
                return;
            }
            //执行模式
            cbAttr.SelectedIndex = LogicInfo.attr;
            if (string.IsNullOrEmpty(LogicInfo.content))
            {
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                dataGridView3.Rows.Clear();
                return;
            }
            ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];

            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
            //加载对象比较表格
            dgvConditionAddItem(dataGridView1, logicConditionContent.conditionInfo, ip);
            //加载确定 否 表格
            dgvAddItem(dataGridView2, logicConditionContent.trueDo, ip);
            dgvAddItem(dataGridView3, logicConditionContent.falseDo, ip);
            DgvMesege.RecoverDgvForm(dataGridView1, X_Value1, Y_Value1, ScrollRowCount1, ScrollColumnCount1);
            DgvMesege.RecoverDgvForm(dataGridView2, X_Value2, Y_Value2, ScrollRowCount2, ScrollColumnCount2);
            DgvMesege.RecoverDgvForm(dataGridView3, X_Value3, Y_Value3, ScrollRowCount3, ScrollColumnCount3);
        }

        #endregion

        /// <summary>
        /// 表格判断条件 展现数据
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="conditionInfo"></param>
        /// <param name="ip"></param>
        private void dgvConditionAddItem(DataGridView dataGridView, List<DataJson.ConditionInfo> conditionInfo, string ip)
        {
            try
            {
                dataGridView.Rows.Clear();
                if (conditionInfo == null)
                {
                    return;
                }
                //循环加载该场景号的所有信息
                foreach (DataJson.ConditionInfo info in conditionInfo)
                {
                    int dex = dataGridView.Rows.Add();
                    /* 关闭非匹配变红
                    if (info.compareobjType != info.objType)
                    {
                        this.dataGridView1.Rows[dex].Cells[6].Style.ForeColor = Color.Red;
                    }*/
                    dataGridView.Rows[dex].Cells[0].Value = info.id;
                    dataGridView.Rows[dex].Cells[1].Value = info.a;
                    dataGridView.Rows[dex].Cells[2].Value = getObj(info);
                    dataGridView.Rows[dex].Cells[3].Value = info.b;
                    dataGridView.Rows[dex].Cells[4].Value = info.operation;
                    dataGridView.Rows[dex].Cells[5].Value = info.c;
                    dataGridView.Rows[dex].Cells[6].Value = getCompareObj(info);
                    dataGridView.Rows[dex].Cells[7].Value = info.d;
                    dataGridView.Rows[dex].Cells[8].Value = "删除";


                }
              


            }
            catch (Exception ex)
            {
                dataGridView.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }
        }

        
        /// <summary>
        /// 获取判断 对象信息
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="address"></param>
        private string getObj(DataJson.ConditionInfo info)
        {
            if (info.objPid == 0)
            {
                //pid号为0则为空 按地址来找
                if (!string.IsNullOrEmpty(info.objAddress) && info.objAddress != "FFFFFFFF")
                {

                    DataJson.PointInfo point = DataListHelper.findPointByType_address(info.objType, info.objAddress, ip);
                    if (point != null)
                    {
                        info.objPid = point.pid;
                        info.objAddress = point.address;
                        info.objType = point.type;
                        return DataListHelper.dealSection(point) + string.Format(" ({1})", DgvMesege.addressTransform(info.objAddress));
                    }
                    
                    else
                    {
                        string tmpType = IniHelper.findIniLinkTypeByAddress(info.objAddress);

                        return string.Format("{0} ({1})", tmpType, DgvMesege.addressTransform(info.objAddress));
                    }
                }
                //pid为0 返回时间
                return DgvMesege.addressTransform(info.objAddress);
            }
            else
            {
                //pid号有效 需要更新address type
                DataJson.PointInfo point = DataListHelper.findPointByPid(info.objPid);
                if (point == null)
                {
                    //pid号有无效 删除该场景
                    return DgvMesege.addressTransform(info.objAddress);
                }
                else
                {
                    //pid号有效
                    try
                    {
                        if (info.objAddress.Substring(2, 6) != point.address.Substring(2, 6))
                        {
                            info.objAddress = point.address;

                        }
                    }
                    catch
                    {
                        info.objAddress = point.address;
                    }
                    //////////////////////////////////////////////////////争议地域
                    //类型不一致 在value寻找
                    if (info.objType != point.type && !string.IsNullOrEmpty(point.value) && !string.IsNullOrEmpty(point.objType))
                    {
                        //根据value寻找type                        
                        point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, point.value);
                    }

                    info.objType = point.type;
                    return DataListHelper.dealSection(point) + string.Format(" ({0})", DgvMesege.addressTransform(info.objAddress));
                }

            }
        }

        /// <summary>
        /// 获取判断 比较对象信息
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="address"></param>
        private string getCompareObj(DataJson.ConditionInfo info)
        {
            if (info.comparePid == 0)
            {
                //pid号为0则为空 按地址来找
                if (!string.IsNullOrEmpty(info.compareobjAddress)  )
                {

                    DataJson.PointInfo point = DataListHelper.findPointByType_address(info.compareobjType, info.compareobjAddress, ip);
                    if (point != null)
                    {
                        info.comparePid = point.pid;
                        info.compareobjAddress = point.address;
                        info.compareobjType = point.type;
                        return DataListHelper.dealSection(point) + string.Format(" ({1})", DgvMesege.addressTransform(info.compareobjAddress));
                    }
                    else
                    {
                        
                        string tmpType = IniHelper.findIniLinkTypeByAddress(info.compareobjAddress);
                        if (info.compareobjAddress.Substring(0,2)!= "FE" && string.IsNullOrEmpty(info.compareobjType))
                        { 
                            //状态操作
                            return string.Format("{0} ({1})", Resources.StateControl, DgvMesege.addressTransform(info.compareobjAddress));
                        }
                        return string.Format("{0} ({1})", tmpType, DgvMesege.addressTransform(info.compareobjAddress));
                    }
                }
                //pid为0 返回时间
                return DgvMesege.addressTransform(info.compareobjAddress);
            }
            else
            {
                //pid号有效 需要更新address type
                DataJson.PointInfo point = DataListHelper.findPointByPid(info.comparePid);
                if (point == null)
                {
                    //pid号有无效 删除该场景
                    return DgvMesege.addressTransform(info.compareobjAddress);
                }
                else
                {
                    //pid号有效
                    try
                    {
                        if (info.compareobjAddress.Substring(2, 6) != point.address.Substring(2, 6))
                        {
                            info.compareobjAddress = point.address;

                        }
                    }
                    catch
                    {
                        info.compareobjAddress = point.address;
                    }
                    //////////////////////////////////////////////////////争议地域
                    //类型不一致 在value寻找
                    if (info.compareobjType != point.type && !string.IsNullOrEmpty(point.value) && !string.IsNullOrEmpty(point.objType))
                    {
                        //根据value寻找type                        
                        point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, point.value);
                    }

                    info.compareobjType = point.type;
                    return DataListHelper.dealSection(point) + string.Format(" ({0})", DgvMesege.addressTransform(info.compareobjAddress));
                }

            }
        }

        /// <summary>
        /// dgv表格2 3展现数据
        /// </summary>
        /// <param name="sceneInfo"></param>
        /// <param name="ip"></param>
        private void dgvAddItem(DataGridView dataGridView, List<DataJson.sceneInfo> sceneInfo, string ip)
        {
            try
            {
                dataGridView.Rows.Clear();
                List<DataJson.sceneInfo> delScene = new List<DataJson.sceneInfo>();
                if (sceneInfo == null)
                {
                    return;
                }
                //循环加载该场景号的所有信息
                foreach (DataJson.sceneInfo info in sceneInfo)
                {
                    int dex = dataGridView.Rows.Add();

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
                                dataGridView.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                dataGridView.Rows[dex].Cells[4].Value = point.name;
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
                            dataGridView.Rows.Remove(dataGridView.Rows[dex]);
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
                            dataGridView.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView.Rows[dex].Cells[4].Value = point.name;
                        }

                    }

                    dataGridView.Rows[dex].Cells[0].Value = info.id;
                    dataGridView.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView.Rows[dex].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView.Rows[dex].Cells[5].Value = (info.optName + " " + info.opt).Trim();
                    dataGridView.Rows[dex].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
                    dataGridView.Rows[dex].Cells[7].Value = "删除";


                }
                for (int i = 0; i < delScene.Count; i++)
                {
                    sceneInfo.Remove(delScene[i]);
                }

            }
            catch (Exception ex)
            {
                dataGridView.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }
        }

        #endregion

        #region dgv加载比较式和类型框
        /// <summary>
        /// dgv加载比较式和类型框
        /// </summary>
        private void dgvAddColumn()
        {
            //新增对象列 加载
            this.dataGridView1.Rows.Clear();
            cbOperation = new DataGridViewComboBoxColumn();
            
            /*cbOperation.Items.Add("等于");
            cbOperation.Items.Add("大于");
            cbOperation.Items.Add("小于");
            cbOperation.Items.Add("不等于");
            cbOperation.Items.Add("大于等于");
            cbOperation.Items.Add("小于等于");*/
             cbOperation.Items.Add("==");
            cbOperation.Items.Add(">");
            cbOperation.Items.Add("<");
            cbOperation.Items.Add("!=");
            cbOperation.Items.Add(">=");
            cbOperation.Items.Add("<=");
            //设置列名
            cbOperation.HeaderText = "比较式";
            //设置下拉列表的默认值 
            cbOperation.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //cbObjType.DefaultCellStyle.NullValue = cbObjType.Items[0];
            cbOperation.Name = "operation";
            cbOperation.ReadOnly = true;
            //插入执行对象
            this.dataGridView1.Columns.Insert(4, cbOperation);
            //新增对象列 加载
            this.dataGridView2.Rows.Clear();
            //新增对象列 加载
            this.dataGridView3.Rows.Clear();

            DataGridViewComboBoxColumn dgvc = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn dgvc2 = new DataGridViewComboBoxColumn();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "define", "name");
                if (!string.IsNullOrEmpty(type))
                {
                    dgvc.Items.Add(type);
                    dgvc2.Items.Add(type);
                
                }
            }
            //设置列名
            dgvc.HeaderText = "类型";
            //设置下拉列表的默认值 
            dgvc.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //或者这样设置 默认选择第一项
            dgvc.DefaultCellStyle.NullValue = dgvc.Items[0];
            dgvc.ReadOnly = true;
            dgvc.Name = "type";


            //设置列名
            dgvc2.HeaderText = "类型";
            //设置下拉列表的默认值 
            dgvc2.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //或者这样设置 默认选择第一项
            dgvc2.DefaultCellStyle.NullValue = dgvc.Items[0];
            dgvc2.ReadOnly = true;
            dgvc2.Name = "type";

            //插入
            this.dataGridView2.Columns.Insert(1, dgvc);
            this.dataGridView3.Columns.Insert(1, dgvc2);
        }
        #endregion


        #region   设置执行模式
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

        #endregion


        #region 表格新增一行
        private void btnIfAdd_Click(object sender, EventArgs e)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            DataJson.ConditionContent logicConditionContent = new DataJson.ConditionContent();
            if (!string.IsNullOrEmpty(LogicInfo.content) )
            {
                //把tab对象JSON字符串转换为 操作对象
                logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
            }
            //新建表
            DataJson.ConditionInfo info = new DataJson.ConditionInfo();
            info.id = logicConditionContent.conditionInfo.Count + 1;

            //info.type = "";
            //info.opt = "";
            //info.optName = "";
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            logicConditionContent.conditionInfo.Add(info);
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvConditionAddItem(dataGridView1, logicConditionContent.conditionInfo, ip);
            DgvMesege.selectLastCount(dataGridView1); 
        }

        private void btnElseIfAdd_Click(object sender, EventArgs e)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            DataJson.ConditionContent logicConditionContent = new DataJson.ConditionContent();
            if (!string.IsNullOrEmpty(LogicInfo.content))
            {
                //把tab对象JSON字符串转换为 操作对象
                logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
            }
            //新建表
            DataJson.sceneInfo info = new DataJson.sceneInfo();
            info.id = logicConditionContent.trueDo.Count + 1;

            //info.type = "";
            //info.opt = "";
            //info.optName = "";
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            logicConditionContent.trueDo.Add(info);
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(dataGridView2, logicConditionContent.trueDo, ip);
            DgvMesege.selectLastCount(dataGridView2); 
        }

        private void btnElseAdd_Click(object sender, EventArgs e)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            DataJson.ConditionContent logicConditionContent = new DataJson.ConditionContent();
            if (!string.IsNullOrEmpty(LogicInfo.content))
            {
                //把tab对象JSON字符串转换为 操作对象
                logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
            }
            //新建表
            DataJson.sceneInfo info = new DataJson.sceneInfo();
            info.id = logicConditionContent.falseDo.Count + 1;

            //info.type = "";
            //info.opt = "";
            //info.optName = "";
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            logicConditionContent.falseDo.Add(info);
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(dataGridView3, logicConditionContent.falseDo, ip);
            DgvMesege.selectLastCount(dataGridView3); 
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

        private int ScrollRowCount1 = 0;
        private int ScrollColumnCount1 = 0;
        private int ScrollRowCount2 = 0;
        private int ScrollColumnCount2 = 0;
        private int ScrollRowCount3 = 0;
        private int ScrollColumnCount3 = 0;
        private void CellMouseDown(DataGridView dataGridView, DataGridViewCellMouseEventArgs e, System.Windows.Forms.Timer doubleClickTimer)
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
                    if (dataGridView.SelectedCells.Count == 1 && rowCount == oldrowCount && columnCount == oldcolumnCount)
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

        private void CellMouseMove(DataGridView dataGridView, DataGridViewCellMouseEventArgs e)
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
                    dataGridView.ClearSelection();
                    dataGridView.Rows[rowNum].Selected = true;//选中行
                }
            }
        }

        #region 判断表格 

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            DgvMesege.endDataViewCurrent(dataGridView1, e.Y, e.X);
        }


        private void doubleClickTimer1_Tick(object sender, EventArgs e)
        {
            try
            {
                milliseconds += 100;
                // 第二次鼠标点击超出双击事件间隔
                if (milliseconds >= SystemInformation.DoubleClickTime)
                {
                    doubleClickTimer1.Stop();


                    if (isDoubleClick)
                    {

                        if (rowCount >= 0 && columnCount >= 0)
                        {

                            int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
                            //改变地址
                            string obj = "";
                            //string objType = "";
                            switch (dataGridView1.Columns[columnCount].Name)
                            {
                                case "objAddress":
                                   
                                    if (dataGridView1.Rows[rowCount].Cells[2].Value != null)
                                    {  
                                        obj = Validator.GetParenthesis(dataGridView1.Rows[rowCount].Cells[2].Value.ToString());
                                    }
                                    //赋值List 并添加地域 名字
                                    dgvobjAddress(id,obj);
                                    break;
                                case "compareAddress":
                                    //赋值List 并添加地域 名字
                                     dgvcompareAddress(id);
                                    break;
                                case "operation":
                                    cbOperation.ReadOnly = false;
                                    break;
                     
                                case "del1":
                                    //删除表
                                    dgvIfDel(id);
                                    
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
                                case "a":
                                    dataGridView1.Columns[1].ReadOnly = false;
                                    break;
                                case "b":
                                    dataGridView1.Columns[3].ReadOnly = false;
                                    break;
                                case "c":
                                    dataGridView1.Columns[5].ReadOnly = false;
                                    break;
                                case "d":
                                    dataGridView1.Columns[7].ReadOnly = false;
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
                        #region //处理单击事件操作

                        if (rowCount >= 0 && columnCount >= 0)
                        {
                            //DGV的行号
                            int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
                            switch (dataGridView1.Columns[columnCount].Name)
                            {
                                case "objAddress":
                                    setTitleObjAddress();
                                    break;
                                case "compareAddress":
                                    setTitleCompareAddress();
                                    break;
                                case "name":
                                    //setTitleAddress();
                                    break;
                                case "del1":
                                    //删除表
                                    dgvIfDel(id);

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
                        #endregion
                    }

                    isFirstClick = true;
                    isDoubleClick = false;
                    milliseconds = 0;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            CellMouseDown(dataGridView1,e,doubleClickTimer1);
            ScrollRowCount1 = e.RowIndex;
            ScrollColumnCount1 = e.ColumnIndex;
        }

        private void dataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            CellMouseMove(dataGridView1,e);
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
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
                        case "a":
                            if (dataGridView1.Rows[rowNum].Cells[1].Value != null )
                            {

                                tmpDelay = dataGridView1.Rows[rowNum].Cells[1].Value.ToString();
                            }
                         
                            break;
                        case "b":
                            if (dataGridView1.Rows[rowNum].Cells[3].Value != null )
                            {
                                tmpDelay = dataGridView1.Rows[rowNum].Cells[3].Value.ToString();
                            }
                  
                            break;
                        case "c":
                            if (dataGridView1.Rows[rowNum].Cells[5].Value != null)
                            {
                                tmpDelay = dataGridView1.Rows[rowNum].Cells[5].Value.ToString();
                            }
                            break;
                        case "d":
                            if (dataGridView1.Rows[rowNum].Cells[7].Value != null)
                            {
                                tmpDelay = dataGridView1.Rows[rowNum].Cells[7].Value.ToString();
                            }
                            break;
                    
                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
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
                        case "a":
                            dataGridView1.Columns[1].ReadOnly = true;
                            if (dataGridView1.Rows[rowNum].Cells[1].Value != null && Validator.IsInteger(dataGridView1.Rows[rowNum].Cells[1].Value.ToString()))
                            {

                                //改变延时
                                dgvABCD(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[1].Value),1);
                            }
                            else
                            {
                                AppTxtShow("延时格式错误，请正确填写！");
                                dataGridView1.Rows[rowNum].Cells[1].Value = tmpDelay;
                            }
                            break;
                        case "b":
                            dataGridView1.Columns[3].ReadOnly = true;
                            if (dataGridView1.Rows[rowNum].Cells[3].Value != null && Validator.IsInteger(dataGridView1.Rows[rowNum].Cells[3].Value.ToString()))
                            {
                                //改变延时
                                dgvABCD(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[3].Value), 3);
                            }
                            else
                            {
                                AppTxtShow("延时格式错误，请正确填写！");
                                dataGridView1.Rows[rowNum].Cells[3].Value = tmpDelay;
                            }
                            break;
                        case "c":
                            dataGridView1.Columns[5].ReadOnly = true;
                            if (dataGridView1.Rows[rowNum].Cells[5].Value != null && Validator.IsInteger(dataGridView1.Rows[rowNum].Cells[5].Value.ToString()))
                            {
                                //改变延时
                                dgvABCD(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[5].Value), 5);
                            }
                            else
                            {
                                AppTxtShow("延时格式错误，请正确填写！");
                                dataGridView1.Rows[rowNum].Cells[5].Value = tmpDelay;
                            }
                            break;
                        case "d":
                            dataGridView1.Columns[7].ReadOnly = true;
                            if (dataGridView1.Rows[rowNum].Cells[7].Value != null && Validator.IsInteger(dataGridView1.Rows[rowNum].Cells[7].Value.ToString()))
                            {
                                //改变延时
                                dgvABCD(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[7].Value), 7);
                            }
                            else
                            {
                                AppTxtShow("延时格式错误，请正确填写！");
                                dataGridView1.Rows[rowNum].Cells[7].Value = tmpDelay;
                            }
                            break;
                        case "operation":
                            dgvIfOperation(Convert.ToInt32(dataGridView1.Rows[rowNum].Cells[0].Value), dataGridView1.Rows[rowNum].Cells[4].EditedFormattedValue.ToString());
                            cbOperation.ReadOnly = true;
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="obj">当前对象的地址值</param>
        /// <returns></returns>
        private void dgvobjAddress(int id, string obj)
        {
            LogicConditionAddress dc = new LogicConditionAddress();
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            DataJson.ConditionInfo info = DataListHelper.getLogicConditionInfo(id, logicConditionContent.conditionInfo);
            if (info == null)
            {
                return;
            }
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            dc.ObjType = info.objType;
            dc.Obj = obj;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //地址
                info.objAddress = dc.Obj;
 
                //按照地址查找type的类型 只限制于设备
                string type = IniHelper.findIniTypesByAddress(ip, info.objAddress).Split(',')[0];
                if (string.IsNullOrEmpty(type))
                {
                    type = dc.RtType;

                }
                //区域加名称
                DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.objAddress, ip);
                if (point != null)
                {
                    info.objPid = point.pid;
                    if (info.objType != point.type)
                    {
                        info.objType = point.type;
                    }
                }
                else
                {
                    info.objPid = 0;
                    if (info.objType != type)
                    {
                        info.objType = type;
                    }

                }
                if (info.objType != info.compareobjType)
                {
                    info.comparePid = 0;
                    info.compareobjAddress = string.Empty;
                    info.compareobjType = string.Empty;
                    //dataGridView1.Rows[id].Cells[6].Value = null;
                    //dataGridView1.Rows[id].Cells[6].Value = getCompareObj(info);
                }

                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvConditionAddItem(dataGridView1, logicConditionContent.conditionInfo, ip);
                DgvMesege.RecoverDgvForm(dataGridView1, X_Value1, Y_Value1, ScrollRowCount1, ScrollColumnCount1);
            }//ok


        }

        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void dgvcompareAddress(int id)
        {
            LogicCompareAddress dc = new LogicCompareAddress();
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            DataJson.ConditionInfo info = DataListHelper.getLogicConditionInfo(id, logicConditionContent.conditionInfo);
            if (info == null || string.IsNullOrEmpty(info.objType) || string.IsNullOrEmpty(info.objAddress))
            {
                return;
            }
            
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            dc.ObjType = info.objType;
            dc.Address = info.compareobjAddress;
            dc.CompareType = info.compareobjType;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                info.compareobjAddress = dc.Address;
                if (dc.Address.Substring(0, 2) == "FE")
                {
                    //按照地址查找type的类型 只限制于设备
                    string type = IniHelper.findIniTypesByAddress(ip, info.compareobjAddress).Split(',')[0];
                    if (string.IsNullOrEmpty(type))
                    {
                        type = dc.RtType;

                    }
                    //区域加名称
                    DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.compareobjAddress, ip);
                    if (point != null)
                    {
                        info.comparePid = point.pid;
                        if (info.compareobjType != point.type)
                        {
                            info.compareobjType = point.type;
                        }
                    }
                    else
                    {
                        //搜索一次dev表 
                        info.comparePid = 0;

                        if (info.compareobjType != type)
                        {
                            info.compareobjType = type;
                        }

                    }
                }
                else
                {
                    //状态操作
                    info.compareobjType = "";
                    info.comparePid = 0;
                }
               
                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvConditionAddItem(dataGridView1, logicConditionContent.conditionInfo, ip);
                DgvMesege.RecoverDgvForm(dataGridView1, X_Value1, Y_Value1, ScrollRowCount1, ScrollColumnCount1);
            }//ok


        }

        private void dgvABCD(int id, int val,int abcd)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content) )
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
            
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            DataJson.ConditionInfo info = DataListHelper.getLogicConditionInfo(id, logicConditionContent.conditionInfo);
            if (info == null)
            {
                return;
            }
            switch (abcd)
            { 
                case 1:
                    info.a = val;
                    break;
                case 3:
                    info.b = val;
                    break;
                case 5:
                    info.c = val;
                    break;
                case 7:
                    info.d = val;
                    break;
                default:
                    break;
            }
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
           

        }

        private void dgvIfOperation(int id, string val)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            DataJson.ConditionInfo info = DataListHelper.getLogicConditionInfo(id, logicConditionContent.conditionInfo);
            if (info == null)
            {
                return;
            }
            info.operation = val;
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);


        }

        private void dgvIfDel(int id)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            
            DataJson.ConditionInfo info = DataListHelper.getLogicConditionInfo(id, logicConditionContent.conditionInfo);
            if (info == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (!logicConditionContent.conditionInfo.Remove(info))
            {
                return;
            }
            //排序
            conditionInfoSort(logicConditionContent.conditionInfo);
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvConditionAddItem(dataGridView1, logicConditionContent.conditionInfo,ip);
        }

        private void conditionInfoSort(List<DataJson.ConditionInfo> infos)
        {
            int i = 1;
            foreach (DataJson.ConditionInfo info in infos)
            {
                info.id = i;
                i++;
            }
        }

        /// <summary>
        /// 复制title选中的节点 赋地址给ObjAddress
        /// </summary>
        private void setTitleObjAddress()
        {
            try
            {
                int colIndex = dataGridView1.SelectedCells[0].ColumnIndex;
                int id = dataGridView1.CurrentCell.RowIndex;

                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);


                DataJson.ConditionInfo info = DataListHelper.getLogicConditionInfo(id+1, logicConditionContent.conditionInfo);
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

                info.objPid = eq.pid;
                info.objAddress = eq.address;
                info.objType = eq.type;
                dataGridView1.Rows[id].Cells[2].Value = getObj(info);

                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            catch
            {

            }

        }

        /// <summary>
        /// 复制title选中的节点 赋地址给ObjAddress
        /// </summary>
        private void setTitleCompareAddress()
        {
            try
            {
                int colIndex = dataGridView1.SelectedCells[0].ColumnIndex;
                int id = dataGridView1.CurrentCell.RowIndex;

                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);


                DataJson.ConditionInfo info = DataListHelper.getLogicConditionInfo(id + 1, logicConditionContent.conditionInfo);
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

                info.comparePid = eq.pid;
                info.compareobjAddress = eq.address;
                info.compareobjType = eq.type;
                dataGridView1.Rows[id].Cells[6].Value = getCompareObj(info);

                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            catch
            {

            }

        }

        #endregion


        #region trueDo表格 符合执行

        private void dataGridView2_MouseDown(object sender, MouseEventArgs e)
        {
            DgvMesege.endDataViewCurrent(dataGridView2, e.Y, e.X);
        }
   

        private void doubleClickTimer2_Tick(object sender, EventArgs e)
        {
            try
            {
                milliseconds += 100;
                // 第二次鼠标点击超出双击事件间隔
                if (milliseconds >= SystemInformation.DoubleClickTime)
                {
                    doubleClickTimer2.Stop();


                    if (isDoubleClick)
                    {

                        if (rowCount >= 0 && columnCount >= 0)
                        {

                            int id = Convert.ToInt32(dataGridView2.Rows[rowCount].Cells[0].Value);
                            switch (dataGridView2.Columns[columnCount].Name)
                            {
                                case "address2":
                                    //改变地址
                                    string obj = "";
                                    if (dataGridView2.Rows[rowCount].Cells[2].Value != null)
                                    {
                                        //原地址
                                        obj = dataGridView2.Rows[rowCount].Cells[2].Value.ToString();
                                    }
                                    string objType = dataGridView2.Rows[rowCount].Cells[1].EditedFormattedValue.ToString();
                                    //赋值List 并添加地域 名字
                                    dgvAddress2(id, objType, obj);
                                    
                                    break;
                                case "operation2":
                                    
                                    //操作
                                    string info = dgvOperation2(id, dataGridView2.Rows[rowCount].Cells[1].EditedFormattedValue.ToString());
                                    if (!string.IsNullOrEmpty(info))
                                    {
                                        dataGridView2.Rows[rowCount].Cells[5].Value = info;
                                    }
                                    break;
                                case "del2":
                                    //删除表
                                    dgvDel2(id);

                                    break;
                                case "num2":
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
                                case "delay2":
                                    dataGridView2.Columns[6].ReadOnly = false;
                      
                                    break;
                                default: break;
                            }
                            try
                            {
                                //更改内容回自动刷新到第一行
                                dataGridView2.CurrentCell = dataGridView2.Rows[rowCount].Cells[columnCount];
                            }
                            catch
                            {
                                if (dataGridView2.Rows.Count > 0)
                                {
                                    dataGridView2.CurrentCell = dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[columnCount];
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
                            int id = Convert.ToInt32(dataGridView2.Rows[rowCount].Cells[0].Value);
                            switch (dataGridView2.Columns[columnCount].Name)
                            {
                                case "address2":
                                    setTitleAddress2();
                                    break;
                                case "section2":
                                    setTitleAddress2();
                                    break;
                                case "name2":
                                    setTitleAddress2();
                                    break;
                                case "del2":
                                    //删除表
                                    dgvDel2(id);

                                    break;
                                default: break;



                            }
                            try
                            {
                                //更改内容回自动刷新到第一行
                                dataGridView2.CurrentCell = dataGridView2.Rows[rowCount].Cells[columnCount];
                            }
                            catch
                            {
                                if (dataGridView2.Rows.Count > 0)
                                {
                                    dataGridView2.CurrentCell = dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[columnCount];
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

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            CellMouseDown(dataGridView2, e, doubleClickTimer2);
            ScrollRowCount2 = e.RowIndex;
            ScrollColumnCount2 = e.ColumnIndex;
        }

        private void dataGridView2_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            CellMouseMove(dataGridView2, e);
        }

        private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                //选中行号
                int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    switch (dataGridView2.Columns[columnNum].Name)
                    {
                        case "delay2":
                            if (dataGridView2.Rows[rowNum].Cells[6].Value != null )
                            {
                                //改变延时
                                tmpDelay = dataGridView2.Rows[rowNum].Cells[6].Value.ToString();
                            }
                   
                            break;

                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //选中行号
                int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    switch (dataGridView2.Columns[columnNum].Name)
                    {
                        case "delay2":
                            dataGridView2.Columns[6].ReadOnly = true;
                            if (dataGridView2.Rows[rowNum].Cells[6].Value != null && Validator.IsNumber(dataGridView2.Rows[rowNum].Cells[6].Value.ToString()))
                            {
                                //改变延时
                                dgvDelay2(Convert.ToInt32(dataGridView2.Rows[rowNum].Cells[0].Value), Convert.ToDouble(dataGridView2.Rows[rowNum].Cells[6].Value));
                            }
                            else
                            {
                                AppTxtShow("延时格式错误，请正确填写！");
                                dataGridView2.Rows[rowNum].Cells[6].Value = tmpDelay;
                            }
                            break;
                 
                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }




        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="obj">当前对象的地址值</param>
        /// <returns></returns>
        private void dgvAddress2(int id, string objType, string obj)
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
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

                DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.trueDo);
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
                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvAddItem(dataGridView2, logicConditionContent.trueDo,ip);
                DgvMesege.RecoverDgvForm(dataGridView2, X_Value2, Y_Value2, ScrollRowCount2, ScrollColumnCount2);
            }//ok


        }


        /// <summary>
        /// DGV表 操作栏 返回操作信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string dgvOperation2(int id, string type)
        {


            LogicConcrol dc = new LogicConcrol();
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return null;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.trueDo);
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
                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
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
        /// 修改DGV表中 延时时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time">延时时间</param>
        private void dgvDelay2(int id, Double time)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.trueDo);
            if (info == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            info.delay = Convert.ToInt32(time * 10);
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);

        }

        private void dgvDel2(int id)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.trueDo);
            if (info == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (!logicConditionContent.trueDo.Remove(info))
            {
                return;
            }
            //排序
            sceneInfoSort(logicConditionContent.trueDo);
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(dataGridView2, logicConditionContent.trueDo, ip);
        }

        private void sceneInfoSort(List<DataJson.sceneInfo> infos)
        {
            int i = 1;
            foreach (DataJson.sceneInfo info in infos)
            {
                info.id = i;
                i++;
            }
        }

        /// <summary>
        /// 复制title选中的节点 赋地址给ObjAddress
        /// </summary>
        private void setTitleAddress2()
        {
            try
            {
                int colIndex = dataGridView2.SelectedCells[0].ColumnIndex;
                int id = dataGridView2.CurrentCell.RowIndex;

                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

                DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id+1, logicConditionContent.trueDo);
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

                    dataGridView2.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView2.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView2.Rows[id].Cells[4].Value = eq.name;

                }
                else
                {
                    info.pid = eq.pid;
                    info.address = eq.address;
                    info.type = eq.type;
                    info.opt = "";
                    info.optName = "";
                    dataGridView2.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView2.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView2.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView2.Rows[id].Cells[4].Value = eq.name;
                    dataGridView2.Rows[id].Cells[5].Value = null;


                }
                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            catch
            {

            }

        }

        #endregion


        #region falseDo 不符合执行
        private void dataGridView3_MouseDown(object sender, MouseEventArgs e)
        {
            DgvMesege.endDataViewCurrent(dataGridView3, e.Y, e.X);
        }

        private void doubleClickTimer3_Tick(object sender, EventArgs e)
        {
            try
            {
                milliseconds += 100;
                // 第二次鼠标点击超出双击事件间隔
                if (milliseconds >= SystemInformation.DoubleClickTime)
                {
                    doubleClickTimer3.Stop();


                    if (isDoubleClick)
                    {

                        if (rowCount >= 0 && columnCount >= 0)
                        {

                            int id = Convert.ToInt32(dataGridView3.Rows[rowCount].Cells[0].Value);
                            switch (dataGridView3.Columns[columnCount].Name)
                            {
                                case "address3":
                                    //改变地址
                                    string obj = "";
                                    if (dataGridView3.Rows[rowCount].Cells[2].Value != null)
                                    {
                                        //原地址
                                        obj = dataGridView3.Rows[rowCount].Cells[2].Value.ToString();
                                    }
                                    string objType = dataGridView3.Rows[rowCount].Cells[1].EditedFormattedValue.ToString();
                                    //赋值List 并添加地域 名字
                                    dgvAddress3(id, objType, obj);
                                    
                                    break;
                                case "operation3":
                                    
                                    //操作
                                    string info = dgvOperation3(id, dataGridView3.Rows[rowCount].Cells[1].EditedFormattedValue.ToString());
                                    if (!string.IsNullOrEmpty(info))
                                    {
                                        dataGridView3.Rows[rowCount].Cells[5].Value = info;
                                    }
                                    break;
                                case "del3":
                                    //删除表
                                    dgvDel3(id);

                                    break;
                                case "num3":
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
                                case "delay3":
                                    dataGridView3.Columns[6].ReadOnly = false;
                            
                                    break;
                                default: break;
                            }
                            try
                            {
                                //更改内容回自动刷新到第一行
                                dataGridView3.CurrentCell = dataGridView3.Rows[rowCount].Cells[columnCount];
                            }
                            catch
                            {
                                if (dataGridView3.Rows.Count > 0)
                                {
                                    dataGridView3.CurrentCell = dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[columnCount];
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
                            int id = Convert.ToInt32(dataGridView3.Rows[rowCount].Cells[0].Value);
                            switch (dataGridView3.Columns[columnCount].Name)
                            {
                                case "address3":
                                    setTitleAddress3();
                                    break;
                                case "section3":
                                    setTitleAddress3();
                                    break;
                                case "name3":
                                    setTitleAddress3();
                                    break;
                                case "del3":
                                    //删除表
                                    dgvDel3(id);

                                    break;
                                default: break;



                            }
                            try
                            {
                                //更改内容回自动刷新到第一行
                                dataGridView3.CurrentCell = dataGridView3.Rows[rowCount].Cells[columnCount];
                            }
                            catch
                            {
                                if (dataGridView3.Rows.Count > 0)
                                {
                                    dataGridView3.CurrentCell = dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[columnCount];
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

        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView3_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            CellMouseDown(dataGridView3, e, doubleClickTimer3);
            ScrollRowCount3= e.RowIndex;
            ScrollColumnCount3 = e.ColumnIndex;
        }

        private void dataGridView3_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            CellMouseMove(dataGridView3, e);
        }

        
        private void dataGridView3_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                //选中行号
                int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    switch (dataGridView3.Columns[columnNum].Name)
                    {
                        case "delay3":
                            if (dataGridView3.Rows[rowNum].Cells[6].Value != null)
                            {
                                //记录定时
                                tmpDelay = dataGridView3.Rows[rowNum].Cells[6].Value.ToString();
                              
                            }
                           
                            break;

                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //选中行号
                int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    switch (dataGridView3.Columns[columnNum].Name)
                    {
                        case "delay3":
                            dataGridView3.Columns[6].ReadOnly = true;
                            if (dataGridView3.Rows[rowNum].Cells[6].Value != null && Validator.IsNumber(dataGridView3.Rows[rowNum].Cells[6].Value.ToString()))
                            {
                                //改变延时
                                dgvDelay3(Convert.ToInt32(dataGridView3.Rows[rowNum].Cells[0].Value), Convert.ToDouble(dataGridView3.Rows[rowNum].Cells[6].Value));
                            }
                            else
                            {
                                dataGridView3.Rows[rowNum].Cells[6].Value = tmpDelay;
                                AppTxtShow("延时格式错误，请正确填写！");
                            }
                            break;

                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="obj">当前对象的地址值</param>
        /// <returns></returns>
        private void dgvAddress3(int id, string objType, string obj)
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
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

                DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.falseDo);
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
                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvAddItem(dataGridView3, logicConditionContent.falseDo, ip);
                DgvMesege.RecoverDgvForm(dataGridView3, X_Value3, Y_Value3, ScrollRowCount3, ScrollColumnCount3);
            }//ok


        }


        /// <summary>
        /// DGV表 操作栏 返回操作信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string dgvOperation3(int id, string type)
        {


            LogicConcrol dc = new LogicConcrol();
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return null;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.falseDo);
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
                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
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
        /// 修改DGV表中 延时时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time">延时时间</param>
        private void dgvDelay3(int id, Double time)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.falseDo);
            if (info == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            info.delay = Convert.ToInt32(time * 10);
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);

        }

        private void dgvDel3(int id)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

            DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.falseDo);
            if (info == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (!logicConditionContent.falseDo.Remove(info))
            {
                return;
            }
            //排序
            sceneInfoSort(logicConditionContent.falseDo);
            LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(dataGridView3, logicConditionContent.falseDo, ip);
        }

        /// <summary>
        /// 复制title选中的节点 赋地址给ObjAddress
        /// </summary>
        private void setTitleAddress3()
        {
            try
            {
                int colIndex = dataGridView3.SelectedCells[0].ColumnIndex;
                int id = dataGridView3.CurrentCell.RowIndex;

                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

                DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id+1, logicConditionContent.falseDo);
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

                    dataGridView3.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView3.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView3.Rows[id].Cells[4].Value = eq.name;

                }
                else
                {
                    info.pid = eq.pid;
                    info.address = eq.address;
                    info.type = eq.type;
                    info.opt = "";
                    info.optName = "";
                    dataGridView3.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView3.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView3.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView3.Rows[id].Cells[4].Value = eq.name;
                    dataGridView3.Rows[id].Cells[5].Value = null;


                }
                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            catch
            {

            }

        }

        #endregion

   

        #endregion

        #region Del按键处理
        private void dataGridView2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Delete)
                {

                    DelKeyOpt2();

                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        //删除操作
        private void DelKeyOpt2()
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
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                for (int i = 0; i < dataGridView2.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView2.SelectedCells[i].ColumnIndex;

                    //当粘贴选中单元格为操作
                    if (colIndex == 5)
                    {
                        int id = Convert.ToInt32(dataGridView2.Rows[dataGridView2.SelectedCells[i].RowIndex].Cells[0].Value);
                        DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.trueDo);
                        if (info == null)
                        {
                            continue;
                        }
      

                        ischange = true;
                        info.opt = "";
                        info.optName = "";
                        dataGridView2.Rows[dataGridView2.SelectedCells[i].RowIndex].Cells[5].Value = null;

                    }//if
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }

        }

        private void dataGridView3_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Delete)
                {

                    DelKeyOpt3();

                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        //删除操作
        private void DelKeyOpt3()
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
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);

                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                for (int i = 0; i < dataGridView3.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView3.SelectedCells[i].ColumnIndex;

                    //当粘贴选中单元格为操作
                    if (colIndex == 5)
                    {
                        int id = Convert.ToInt32(dataGridView3.Rows[dataGridView3.SelectedCells[i].RowIndex].Cells[0].Value);
                        DataJson.sceneInfo info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.falseDo);
                        if (info == null)
                        {
                            continue;
                        }
          

                        ischange = true;
                        info.opt = "";
                        info.optName = "";
                        dataGridView3.Rows[dataGridView3.SelectedCells[i].RowIndex].Cells[5].Value = null;

                    }//if
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
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
            try
            {
                if (dataGridView2.Focused)
                {
                    copy(dataGridView2, true);
                }
                else if (dataGridView3.Focused)
                {
                    copy(dataGridView3, false);
                }
            }
            catch { }
            
            

        }

        private void copy(DataGridView dataGridView,bool flag)
        {
            /*
            //获取当前选中单元格的列序号
            int colIndex = dataGridView.CurrentRow.Cells.IndexOf(dataGridView.CurrentCell);
            //当粘贴选中单元格为操作
            if (colIndex == 5)
            {
                int id = Convert.ToInt32(dataGridView.CurrentRow.Cells[0].Value);
                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.sceneInfo info = null;
                if (flag)
                {
                    info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.trueDo);
                    if (info == null)
                    {
                        return;
                    }
                }
                else
                {
                    info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.falseDo);
                    if (info == null)
                    {
                        return;
                    }
                }
                //获取sceneInfo对象表中对应ID号info对象
                FileMesege.copyLogicScene = info;

            }*/
        }

        /// <summary>
        /// 粘贴点位的对象与参数
        /// </summary>
        public void pasteData()
        {
            
            try
            {
                if (dataGridView2.Focused)
                {
                    past(dataGridView2, true);
                }
                else if (dataGridView3.Focused)
                {
                    past(dataGridView3, false);
                }

            }//try
            catch
            {

            }
        }

        private void past(DataGridView dataGridView, bool flag)
        {
            /*
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
            bool ischange = false;
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            for (int i = 0; i < dataGridView.SelectedCells.Count; i++)
            {
                //获取当前选中单元格的列序号
                int colIndex = dataGridView.SelectedCells[i].ColumnIndex;

                //当粘贴选中单元格为操作
                if (colIndex == 5)
                {
                    int id = Convert.ToInt32(dataGridView.Rows[dataGridView.SelectedCells[i].RowIndex].Cells[0].Value);
                    DataJson.sceneInfo info = null;
                    if (flag)
                    {
                        info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.trueDo);
                        if (info == null)
                        {
                            return;
                        }
                    }
                    else
                    {
                        info = DataListHelper.getLogicSceneInfo(id, logicConditionContent.falseDo);
                        if (info == null)
                        {
                            return;
                        }
                    }
                    if (FileMesege.copyLogicScene.type == "" || info.type == "" || info.type != FileMesege.copyLogicScene.type)
                    {
                        continue;
                    }
                    ischange = true;
                    info.opt = FileMesege.copyLogicScene.opt;
                    info.optName = FileMesege.copyLogicScene.optName;

                    dataGridView.Rows[dataGridView.SelectedCells[i].RowIndex].Cells[5].Value = (info.optName + " " + info.opt).Trim();
                }//if
            }
            if (ischange)
            {
                LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }*/
        }

        #endregion

        #region 升序 相同 降序

        #region 相同
        public void Same()
        {
            try
            {
                if (dataGridView1.Focused)
                {
                    SameDgv1();
                }
                else if (dataGridView2.Focused)
                {
                    SameDgv2();
                }
                else if (dataGridView3.Focused)
                {
                    SameDgv3();
                }
            }
            catch { }
        }

        private void SameDgv1()
        {
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();

                //选中行号
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
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.ConditionInfo info = null;
                DataJson.ConditionInfo copyInfo = null;
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
                    info = DataListHelper.getLogicConditionInfo(id +1 , logicConditionContent.conditionInfo);
                    if (info == null)
                    {
                        return;
                    }

                    if (i == dataGridView1.SelectedCells.Count - 1)
                    {
                        //记录第一个选中格内容
                        FirstColumnIndex = colIndex;
                        copyInfo = info;
                        continue;
                    }
                    //当粘贴选中单元格为操作
                    if (colIndex == 2)
                    {
                        //选中单元格为对象
                        info.objAddress = copyInfo.objAddress;
                        info.objType = copyInfo.objType;
                        info.objPid = copyInfo.objPid;
                        if (info.objType != info.compareobjType)
                        {
                            info.comparePid = 0;
                            info.compareobjAddress = string.Empty;
                            info.compareobjType = string.Empty;
                            dataGridView1.Rows[id].Cells[6].Value = null;
                            //dataGridView1.Rows[id].Cells[6].Value = getCompareObj(info);
                        }
                        dataGridView1.Rows[id].Cells[2].Value = getObj(info);

                        ischange = true;

                    }
                    else if (colIndex == 6)
                    {
                        //选中单元格为对象
                        if (dataGridView1.Rows[id].Cells[2].Value != null &&  info.objType == copyInfo.objType)
                        {
                            info.compareobjAddress = copyInfo.compareobjAddress;
                            info.compareobjType = copyInfo.compareobjType;
                            info.comparePid = copyInfo.comparePid;
                            dataGridView1.Rows[id].Cells[6].Value = getCompareObj(info);
                            ischange = true;
                        }

                       
                    }
                    else if (colIndex == 1)
                    {
                        info.a = copyInfo.a;
                        dataGridView1.Rows[id].Cells[1].Value = info.a;
                        ischange = true;
                    }
                    else if (colIndex == 3)
                    {
                        info.b = copyInfo.b;
                        dataGridView1.Rows[id].Cells[3].Value = info.b;
                        ischange = true;
                    }
                    else if (colIndex == 5)
                    {
                        info.c = copyInfo.c;
                        dataGridView1.Rows[id].Cells[5].Value = info.c;
                        ischange = true;
                        
                    }
                    else if (colIndex == 7)
                    {
                        info.d = copyInfo.d;
                        dataGridView1.Rows[id].Cells[7].Value = info.d;
                        ischange = true;

                    }
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }



            }//try
            catch
            {

            }
        }

        private void SameDgv2()
        {
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();

                //选中行号
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
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.sceneInfo info = null;
                DataJson.sceneInfo copyInfo = null;
                for (int i = dataGridView2.SelectedCells.Count - 1; i >= 0; i--)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作 第一个选中格的列号 
                        continue;
                    }
                    id = dataGridView2.SelectedCells[i].RowIndex;
                    info = DataListHelper.getLogicSceneInfo(id+1, logicConditionContent.trueDo);
                    if (info == null)
                    {
                        return;
                    }

                    if (i == dataGridView2.SelectedCells.Count - 1)
                    {
                        //记录第一个选中格内容
                        FirstColumnIndex = colIndex;
                        copyInfo = info;
                        continue;
                    }
                    //当粘贴选中单元格为操作
                    if (colIndex == 5)
                    {

                        //获取sceneInfo对象表中对应ID号info对象
                        if (string.IsNullOrEmpty(copyInfo.type) || string.IsNullOrEmpty(info.type) || info.type != copyInfo.type)
                        {
                            //类型不一致 并且为空
                            continue;
                        }
                        ischange = true;
                        info.opt = copyInfo.opt;
                        info.optName = copyInfo.optName;
                        dataGridView2.Rows[id].Cells[5].Value = (info.optName + " " + info.opt).Trim();
                    }//if
                    else if (colIndex == 2)
                    {
                        //选中单元格为地址
                        info.address = copyInfo.address;
                        if (copyInfo.type != info.type)
                        {
                            //类型不一样清空类型
                            info.opt = string.Empty;
                            info.optName = string.Empty;
                        }
                        info.type = copyInfo.type;
                        info.pid = copyInfo.pid;
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByPid(info.pid);
                        if (point != null)
                        {
                            info.type = point.type;
                            dataGridView2.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView2.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView2.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView2.Rows[id].Cells[4].Value = string.Empty;
                        }
                        dataGridView2.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView2.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView2.Rows[id].Cells[5].Value = (info.optName + " " + info.opt).Trim();

                        ischange = true;

                    }
                    else if (colIndex == 6)
                    {
                        //延时相同
                        ischange = true;
                        info.delay = copyInfo.delay;
                        dataGridView2.Rows[id].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
                    }
                

                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }



            }//try
            catch
            {

            }
        }

        private void SameDgv3()
        {
            try
            {
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();

                //选中行号
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
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.sceneInfo info = null;
                DataJson.sceneInfo copyInfo = null;
                for (int i = dataGridView3.SelectedCells.Count - 1; i >= 0; i--)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView3.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作 第一个选中格的列号 
                        continue;
                    }
                    id = dataGridView3.SelectedCells[i].RowIndex;
                    info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.falseDo);
                    if (info == null)
                    {
                        return;
                    }

                    if (i == dataGridView3.SelectedCells.Count - 1)
                    {
                        //记录第一个选中格内容
                        FirstColumnIndex = colIndex;
                        copyInfo = info;
                        continue;
                    }
                    //当粘贴选中单元格为操作
                    if (colIndex == 5)
                    {

                        //获取sceneInfo对象表中对应ID号info对象
                        if (string.IsNullOrEmpty(copyInfo.type) || string.IsNullOrEmpty(info.type) || info.type != copyInfo.type)
                        {
                            //类型不一致 并且为空
                            continue;
                        }
                        ischange = true;
                        info.opt = copyInfo.opt;
                        info.optName = copyInfo.optName;
                        dataGridView3.Rows[id].Cells[5].Value = (info.optName + " " + info.opt).Trim();
                    }//if
                    else if (colIndex == 2)
                    {
                        //选中单元格为地址
                        info.address = copyInfo.address;
                        if (copyInfo.type != info.type)
                        {
                            //类型不一样清空类型
                            info.opt = string.Empty;
                            info.optName = string.Empty;
                        }
                        info.type = copyInfo.type;
                        info.pid = copyInfo.pid;
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByPid(info.pid);
                        if (point != null)
                        {
                            info.type = point.type;
                            dataGridView3.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView3.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView3.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView3.Rows[id].Cells[4].Value = string.Empty;
                        }
                        dataGridView3.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView3.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView3.Rows[id].Cells[5].Value = (info.optName + " " + info.opt).Trim();

                        ischange = true;

                    }
                    else if (colIndex == 6)
                    {
                        //延时相同
                        ischange = true;
                        info.delay = copyInfo.delay;
                        dataGridView3.Rows[id].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
                    }


                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }



            }//try
            catch
            {

            }
        }

        #endregion

        #region 升序
        public void Ascending()
        {
            try
            {
                if (dataGridView1.Focused)
                {
                    AscendingDgv1();
                }
                else if (dataGridView2.Focused)
                {
                    AscendingDgv2();
                }
                else if (dataGridView3.Focused)
                {
                    AscendingDgv3();
                }
            }
            catch { }

        }

        private void AscendingDgv1()
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
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.ConditionInfo info = null;
                DataJson.ConditionInfo copyInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = dataGridView1.SelectedCells[i].RowIndex;

                        info = DataListHelper.getLogicConditionInfo(id + 1, logicConditionContent.conditionInfo);
                        if (info == null)
                        {
                            return;
                        }
                        copyInfo = info;
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
                    info = DataListHelper.getLogicConditionInfo(id + 1, logicConditionContent.conditionInfo);
                    if (info == null)
                    {
                        return;
                    }

                    if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(copyInfo.objAddress) || copyInfo.objAddress == "FFFFFFFF" || copyInfo.objAddress.Substring(0, 2) != "FE")
                        {
                            continue;
                        }
                        if (!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }
                        if (addCount == 0)
                        {
                            continue;
                        }
                        info.objAddress = DgvMesege.addressAdd(copyInfo.objAddress, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        info.objType = IniHelper.findIniTypesByAddress(ip, info.objAddress).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.objAddress, ip);
                        if (point != null)
                        {
                            info.objPid = point.pid;
                            info.objType = point.type;
                        }
                        else
                        {
                            info.objPid = 0;
                  
                        }
                        if (info.objType != info.compareobjType)
                        {
                            info.comparePid = 0;
                            info.compareobjAddress = string.Empty;
                            info.compareobjType = string.Empty;
                            dataGridView1.Rows[id].Cells[6].Value = null;
                            //dataGridView1.Rows[id].Cells[6].Value = getCompareObj(info);
                        }
                        dataGridView1.Rows[id].Cells[2].Value = getObj(info);

                        ischange = true;

                    }
                    else if (colIndex == 6)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(copyInfo.compareobjAddress) || copyInfo.compareobjAddress == "FFFFFFFF" || copyInfo.compareobjAddress.Substring(0, 2) != "FE")
                        {
                            continue;
                        }
                        if (!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }
                        if (addCount == 0)
                        {
                            continue;
                        }
                        info.compareobjAddress = DgvMesege.addressAdd(copyInfo.compareobjAddress, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        info.compareobjType = IniHelper.findIniTypesByAddress(ip, info.compareobjAddress).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.compareobjAddress, ip);
                        if (point != null)
                        {
                            info.comparePid = point.pid;
                            info.compareobjType = point.type;
                        }
                        else
                        {
                            info.comparePid = 0;

                        }
                        if (info.objType != info.compareobjType)
                        {
                            info.comparePid = 0;
                            info.compareobjAddress = string.Empty;
                            info.compareobjType = string.Empty;
                          
                            
                        }
                        dataGridView1.Rows[id].Cells[6].Value = getCompareObj(info);

                        ischange = true;
                    }
                    else if (colIndex == 1)
                    {
                        info.a = copyInfo.a + addCount * Convert.ToInt32(FileMesege.AsDesCendingNum);
                        dataGridView1.Rows[id].Cells[1].Value = info.a;
                        ischange = true;
                    }
                    else if (colIndex == 3)
                    {
                        info.b = copyInfo.b + addCount * Convert.ToInt32(FileMesege.AsDesCendingNum);
                        dataGridView1.Rows[id].Cells[3].Value = info.b;
                        ischange = true;
                    }
                    else if (colIndex == 5)
                    {
                        info.c = copyInfo.c + addCount * Convert.ToInt32(FileMesege.AsDesCendingNum);
                        dataGridView1.Rows[id].Cells[5].Value = info.c;
                        ischange = true;

                    }
                    else if (colIndex == 7)
                    {
                        info.d = copyInfo.d + addCount * Convert.ToInt32(FileMesege.AsDesCendingNum);
                        dataGridView1.Rows[id].Cells[7].Value = info.d;
                        ischange = true;

                    }
                    addCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }
        }

        private void AscendingDgv2()
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
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.sceneInfo info = null;
                DataJson.sceneInfo copyInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView2.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                        id = dataGridView2.SelectedCells[i].RowIndex;
           
                        info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.trueDo);
                        if (info == null)
                        {
                            return;
                        }
                        copyInfo = info;
                        continue;
                    }
                    if (colIndex == FirstColumnIndex)
                    {
                        addCount++;
                    }
                }

                for (int i = 0; i < dataGridView2.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        continue;
                    }
                    id = dataGridView2.SelectedCells[i].RowIndex;
                    info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.trueDo);
                    if (info == null)
                    {
                        return;
                    }

                    //延时递增
                    if (colIndex == 6)
                    {

                        ischange = true;
                        info.delay = copyInfo.delay + addCount * Convert.ToInt32(FileMesege.AsDesCendingNum * 10);
                        dataGridView2.Rows[id].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
                    }//if
                    else if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(copyInfo.address) || copyInfo.address == "FFFFFFFF")
                        {
                            continue;
                        }
                        if (!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }
                        if (addCount == 0)
                        {
                            continue;
                        }
                        info.address = DgvMesege.addressAdd(copyInfo.address, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        info.type = IniHelper.findIniTypesByAddress(ip, info.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                        if (point != null)
                        {
                            info.pid = point.pid;
                            info.type = point.type;
                            dataGridView2.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView2.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView2.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView2.Rows[id].Cells[4].Value = string.Empty;
                        }
                        info.opt = string.Empty;
                        info.optName = string.Empty;
                        dataGridView2.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView2.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView2.Rows[id].Cells[5].Value = (info.optName + " " + info.opt).Trim();

                        ischange = true;

                    }
                    addCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }
        }

        private void AscendingDgv3()
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
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.sceneInfo info = null;
                DataJson.sceneInfo copyInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView3.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView3.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView3.SelectedCells[i].ColumnIndex;
                        id = dataGridView3.SelectedCells[i].RowIndex;

                        info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.falseDo);
                        if (info == null)
                        {
                            return;
                        }
                        copyInfo = info;
                        continue;
                    }
                    if (colIndex == FirstColumnIndex)
                    {
                        addCount++;
                    }
                }

                for (int i = 0; i < dataGridView3.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView3.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        continue;
                    }
                    id = dataGridView3.SelectedCells[i].RowIndex;
                    info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.falseDo);
                    if (info == null)
                    {
                        return;
                    }

                    //延时递增
                    if (colIndex == 6)
                    {

                        ischange = true;
                        info.delay = copyInfo.delay + addCount * Convert.ToInt32(FileMesege.AsDesCendingNum * 10);
                        dataGridView3.Rows[id].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
                    }//if
                    else if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(copyInfo.address) || copyInfo.address == "FFFFFFFF")
                        {
                            continue;
                        }
                        if (!Validator.IsInteger(FileMesege.AsDesCendingNum.ToString()))
                        {
                            FileMesege.AsDesCendingNum = 1;
                        }
                        if (addCount == 0)
                        {
                            continue;
                        }
                        info.address = DgvMesege.addressAdd(copyInfo.address, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        info.type = IniHelper.findIniTypesByAddress(ip, info.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                        if (point != null)
                        {
                            info.pid = point.pid;
                            info.type = point.type;
                            dataGridView3.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView3.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView3.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView3.Rows[id].Cells[4].Value = string.Empty;
                        }
                        info.opt = string.Empty;
                        info.optName = string.Empty;
                        dataGridView3.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView3.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView3.Rows[id].Cells[5].Value = (info.optName + " " + info.opt).Trim();

                        ischange = true;

                    }
                    addCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }
        }
        #endregion

        #region 降序
        public void Descending()
        {
            try
            {
                if (dataGridView1.Focused)
                {
                    DescendingDgv1();
                }
                else if (dataGridView2.Focused)
                {
                    DescendingDgv2();
                }
                else if (dataGridView3.Focused)
                {
                    DescendingDgv3();
                }
            }
            catch { }
        }

  

        private void DescendingDgv1()
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
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.ConditionInfo info = null;
                DataJson.ConditionInfo copyInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = dataGridView1.SelectedCells[i].RowIndex;

                        info = DataListHelper.getLogicConditionInfo(id + 1, logicConditionContent.conditionInfo);
                        if (info == null)
                        {
                            return;
                        }
                        copyInfo = info;
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
                    info = DataListHelper.getLogicConditionInfo(id + 1, logicConditionContent.conditionInfo);
                    if (info == null)
                    {
                        return;
                    }

                    if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(copyInfo.objAddress) || copyInfo.objAddress == "FFFFFFFF" || copyInfo.objAddress.Substring(0, 2) != "FE")
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
                        info.objAddress = DgvMesege.addressReduce(copyInfo.objAddress, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        info.objType = IniHelper.findIniTypesByAddress(ip, info.objAddress).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.objAddress, ip);
                        if (point != null)
                        {
                            info.objPid = point.pid;
                            info.objType = point.type;
                        }
                        else
                        {
                            info.objPid = 0;

                        }
                        if (info.objType != info.compareobjType)
                        {
                            info.comparePid = 0;
                            info.compareobjAddress = string.Empty;
                            info.compareobjType = string.Empty;
                            dataGridView1.Rows[id].Cells[6].Value = null;
                            //dataGridView1.Rows[id].Cells[6].Value = getCompareObj(info);
                        }
                        dataGridView1.Rows[id].Cells[2].Value = getObj(info);

                        ischange = true;

                    }
                    else if (colIndex == 6)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(copyInfo.compareobjAddress) || copyInfo.compareobjAddress == "FFFFFFFF" || copyInfo.compareobjAddress.Substring(0, 2) != "FE")
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
                        info.compareobjAddress = DgvMesege.addressReduce(copyInfo.compareobjAddress, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        info.compareobjType = IniHelper.findIniTypesByAddress(ip, info.compareobjAddress).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.compareobjAddress, ip);
                        if (point != null)
                        {
                            info.comparePid = point.pid;
                            info.compareobjType = point.type;
                        }
                        else
                        {
                            info.comparePid = 0;

                        }
                        if (info.objType != info.compareobjType)
                        {
                            info.comparePid = 0;
                            info.compareobjAddress = string.Empty;
                            info.compareobjType = string.Empty;


                        }
                        dataGridView1.Rows[id].Cells[6].Value = getCompareObj(info);

                        ischange = true;
                    }
                    else if (colIndex == 1)
                    {
                        info.a = copyInfo.a - reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum);
                        if (info.a < 0)
                        {
                            info.a = 0;
                        }
                        dataGridView1.Rows[id].Cells[1].Value = info.a;
                        ischange = true;
                    }
                    else if (colIndex == 3)
                    {
                        info.b = copyInfo.b - reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum);
                        if (info.b < 0)
                        {
                            info.b = 0;
                        }
                        dataGridView1.Rows[id].Cells[3].Value = info.b;
                        ischange = true;
                    }
                    else if (colIndex == 5)
                    {
                        info.c = copyInfo.c - reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum);
                        if (info.c < 0)
                        {
                            info.c = 0;
                        }
                        dataGridView1.Rows[id].Cells[5].Value = info.c;
                        ischange = true;

                    }
                    else if (colIndex == 7)
                    {
                        info.d = copyInfo.d - reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum);
                        if (info.d < 0)
                        {
                            info.d = 0;
                        }
                        dataGridView1.Rows[id].Cells[7].Value = info.d;
                        ischange = true;

                    }
                    reduceCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }
        }

        private void DescendingDgv2()
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
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.sceneInfo info = null;
                DataJson.sceneInfo copyInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView2.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                        id = dataGridView2.SelectedCells[i].RowIndex;

                        info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.trueDo);
                        if (info == null)
                        {
                            return;
                        }
                        copyInfo = info;
                        continue;
                    }
                    if (colIndex == FirstColumnIndex)
                    {
                        reduceCount++;
                    }
                }

                for (int i = 0; i < dataGridView2.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        continue;
                    }
                    id = dataGridView2.SelectedCells[i].RowIndex;
                    info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.trueDo);
                    if (info == null)
                    {
                        return;
                    }

                    //延时递增
                    if (colIndex == 6)
                    {

                        ischange = true;
                        info.delay = copyInfo.delay - reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum * 10);
                        if (info.delay < 0)
                        {
                            info.delay = 0;
                        }
                        dataGridView2.Rows[id].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
                    }//if
                    else if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(copyInfo.address) || copyInfo.address == "FFFFFFFF")
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
                        info.address = DgvMesege.addressReduce(copyInfo.address, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        info.type = IniHelper.findIniTypesByAddress(ip, info.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                        if (point != null)
                        {
                            info.pid = point.pid;
                            info.type = point.type;
                            dataGridView2.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView2.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView2.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView2.Rows[id].Cells[4].Value = string.Empty;
                        }
                        info.opt = string.Empty;
                        info.optName = string.Empty;
                        dataGridView2.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView2.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView2.Rows[id].Cells[5].Value = (info.optName + " " + info.opt).Trim();

                        ischange = true;

                    }
                    reduceCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }
        }

        private void DescendingDgv3()
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
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);
                DataJson.sceneInfo info = null;
                DataJson.sceneInfo copyInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView3.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView3.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView3.SelectedCells[i].ColumnIndex;
                        id = dataGridView3.SelectedCells[i].RowIndex;

                        info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.falseDo);
                        if (info == null)
                        {
                            return;
                        }
                        copyInfo = info;
                        continue;
                    }
                    if (colIndex == FirstColumnIndex)
                    {
                        reduceCount++;
                    }
                }

                for (int i = 0; i < dataGridView3.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    colIndex = dataGridView3.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        continue;
                    }
                    id = dataGridView3.SelectedCells[i].RowIndex;
                    info = DataListHelper.getLogicSceneInfo(id + 1, logicConditionContent.falseDo);
                    if (info == null)
                    {
                        return;
                    }

                    //延时递增
                    if (colIndex == 6)
                    {

                        ischange = true;
                        info.delay = copyInfo.delay - reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum * 10);
                        if (info.delay < 0)
                        {
                            info.delay = 0;
                        }
                        dataGridView3.Rows[id].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
                    }//if
                    else if (colIndex == 2)
                    {
                        //地址递增
                        if (string.IsNullOrEmpty(copyInfo.address) || copyInfo.address == "FFFFFFFF")
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
                        info.address = DgvMesege.addressReduce(copyInfo.address, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        //AppTxtShow(id +"   "+sceneInfo.address+ "   "+addCount);
                        info.type = IniHelper.findIniTypesByAddress(ip, info.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                        if (point != null)
                        {
                            info.pid = point.pid;
                            info.type = point.type;
                            dataGridView3.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView3.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView3.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView3.Rows[id].Cells[4].Value = string.Empty;
                        }
                        info.opt = string.Empty;
                        info.optName = string.Empty;
                        dataGridView3.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView3.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView3.Rows[id].Cells[5].Value = (info.optName + " " + info.opt).Trim();

                        ischange = true;

                    }
                    reduceCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicConditionContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }//try
            catch
            {

            }
        }

        #endregion



        #endregion

        #region 记录滑动条位置
        //滑动条位置
        int X_Value1; // Stores position of Horizontal scroll bar
        int Y_Value1; // Stores position of Vertical scroll bar
        int X_Value2; // Stores position of Horizontal scroll bar
        int Y_Value2; // Stores position of Vertical scroll bar
        int X_Value3; // Stores position of Horizontal scroll bar
        int Y_Value3; // Stores position of Vertical scroll bar
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                X_Value1 = e.NewValue;
            }

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                Y_Value1 = e.NewValue;
            }
        }

        private void dataGridView2_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                X_Value2 = e.NewValue;
            }

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                Y_Value2 = e.NewValue;
            }
        }

        private void dataGridView3_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                X_Value3 = e.NewValue;
            }

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                Y_Value3 = e.NewValue;
            }
        }




        #endregion






    }//class
}
