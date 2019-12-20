using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using eNet编辑器.AddForm;
using Newtonsoft.Json;
using System.Reflection;
using eNet编辑器.LogicForm;
using System.Threading;

namespace eNet编辑器.DgvView
{
    public partial class LogicVoice : UserControl
    {
        public LogicVoice()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
            doubleBuffered(dataGridView1);
            doubleBuffered(dataGridView2);
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

        string[] letter = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        public static event Action<string> AppTxtShow;

        //当前选中节点的IP地址
        string ip = "";

        private void LogicVoice_Load(object sender, EventArgs e)
        {
            dgvAddColumn();
        }

        private void dgvAddColumn()
        {
           
            //新增对象列 加载
            this.dataGridView1.Rows.Clear();
            //新增对象列 加载
            this.dataGridView2.Rows.Clear();

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
            this.dataGridView1.Columns.Insert(1, dgvc);
            this.dataGridView2.Columns.Insert(2, dgvc2);
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
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            txtVoice.Text = "";
            txtAssignment.Text = "";
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            //执行模式
            cbAttr.SelectedIndex = LogicInfo.attr;
            if (string.IsNullOrEmpty(LogicInfo.content))
            {
                return;
            }
            ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];

            DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
            //表达式和赋值
            txtVoice.Text = logicVoiceContent.voice;
            txtAssignment.Text = DgvMesege.addressTransform(logicVoiceContent.voiceGive);
            //加载对象比较表格
            dgvVoiceAddItem(dataGridView1, logicVoiceContent.voiceItem, ip);
            //加载确定 否 表格
            dgvAddItem(dataGridView2, logicVoiceContent.voiceIfItem, ip);
            DgvMesege.RecoverDgvForm(dataGridView1, X_Value1, Y_Value1, ScrollRowCount1, ScrollColumnCount1);
            DgvMesege.RecoverDgvForm(dataGridView2, X_Value2, Y_Value2, ScrollRowCount2, ScrollColumnCount2);
        }

        #endregion

        /// <summary>
        /// 表格判断条件 展现数据
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="conditionInfo"></param>
        /// <param name="ip"></param>
        private void dgvVoiceAddItem(DataGridView dataGridView, List<DataJson.VoiceItem> voiceIf, string ip)
        {
            try
            {
                dataGridView.Rows.Clear();
                List<DataJson.VoiceItem> delVoice = new List<DataJson.VoiceItem>();
                if (voiceIf == null)
                {
                    return;
                }
                //循环加载该场景号的所有信息
                foreach (DataJson.VoiceItem info in voiceIf)
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
                            delVoice.Add(info);
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
                            info.type = point.type;
                            dataGridView.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView.Rows[dex].Cells[4].Value = point.name;
                        }

                    }

                    dataGridView.Rows[dex].Cells[0].Value = info.letter;
                    dataGridView.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView.Rows[dex].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView.Rows[dex].Cells[5].Value = "删除";


                }
                for (int i = 0; i < delVoice.Count; i++)
                {
                    voiceIf.Remove(delVoice[i]);
                }


            }
            catch (Exception ex)
            {
                dataGridView.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }
        }


        /// <summary>
        /// dgv表格2 展现数据
        /// </summary>
        /// <param name="sceneInfo"></param>
        /// <param name="ip"></param>
        private void dgvAddItem(DataGridView dataGridView, List<DataJson.VoiceIfItem> voiceInfo, string ip)
        {
            try
            {
                
                dataGridView.Rows.Clear();
                List<DataJson.VoiceIfItem> delScene = new List<DataJson.VoiceIfItem>();
                if (voiceInfo == null)
                {
                    return;
                }
                //循环加载该场景号的所有信息
                foreach (DataJson.VoiceIfItem info in voiceInfo)
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
                                dataGridView.Rows[dex].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                dataGridView.Rows[dex].Cells[5].Value = point.name;
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
                            dataGridView.Rows[dex].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView.Rows[dex].Cells[5].Value = point.name;
                        }

                    }

                    dataGridView.Rows[dex].Cells[0].Value = info.id;
                    dataGridView.Rows[dex].Cells[1].Value = info.result;
                    dataGridView.Rows[dex].Cells[3].Value = DgvMesege.addressTransform(info.address);
                    dataGridView.Rows[dex].Cells[2].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView.Rows[dex].Cells[6].Value = (info.optName + " " + info.opt).Trim();
                    dataGridView.Rows[dex].Cells[7].Value = Convert.ToDouble(info.delay) / 10;
                    dataGridView.Rows[dex].Cells[8].Value = "删除";


                }
                for (int i = 0; i < delScene.Count; i++)
                {
                    voiceInfo.Remove(delScene[i]);
                }

            }
            catch (Exception ex)
            {
                dataGridView.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }
        }

        #endregion

        #region 执行模式
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

        #region 表达式
        private void txtVoice_DoubleClick(object sender, EventArgs e)
        {
            Voice dc = new Voice();
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            DataJson.VoiceContent logicVoiceContent;
            if (string.IsNullOrEmpty(LogicInfo.content))
            {
                logicVoiceContent = new DataJson.VoiceContent();
            }
            else
            {
                logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

            }
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            dc.Result = txtVoice.Text;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                logicVoiceContent.voice = dc.ReturnResult;
                LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                AppTxtShow("设置表达式成功");
                txtVoice.Text = dc.ReturnResult;
            }
        }

        private void txtVoice_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
           
        }

        #endregion

        #region 更改赋值地址
        private void txtAssignment_DoubleClick(object sender, EventArgs e)
        {
            LogicVoiceAddress dc = new LogicVoiceAddress();
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            DataJson.VoiceContent logicVoiceContent ;
            if (string.IsNullOrEmpty(LogicInfo.content))
            {
                logicVoiceContent = new DataJson.VoiceContent();
            }
            else
            {
                logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
            
            }
            //把窗口向屏幕中间刷新
            dc.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            dc.Obj = DgvMesege.addressTransform(logicVoiceContent.voiceGive);
            if (string.IsNullOrEmpty(logicVoiceContent.voiceGive))
            {
                dc.ObjType = "";
            }
            else
            {
                dc.ObjType = logicVoiceContent.voiceGive.Substring(2, 2);
            }
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                logicVoiceContent.voiceGive = dc.Obj;
                LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                AppTxtShow("设置赋值地址成功");
                txtAssignment.Text = DgvMesege.addressTransform(dc.Obj);
            }
        }

        private void txtAssignment_KeyPress(object sender, KeyPressEventArgs e)
        {
            //禁止输入
            e.Handled = true;
        }

        #endregion

        #region 添加选择项
        /// <summary>
        /// 添加表达项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnElseIfAdd_Click(object sender, EventArgs e)
        {
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            DataJson.VoiceContent logicVoiceContent;
            if (string.IsNullOrEmpty(LogicInfo.content))
            {
                logicVoiceContent = new DataJson.VoiceContent();
            }
            else
            {
                logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

            }
            if (logicVoiceContent.voiceItem.Count > 25)
            {
                return;
            }
            DataJson.VoiceItem voiceItem = new DataJson.VoiceItem();
            voiceItem.letter = letter[logicVoiceContent.voiceItem.Count];
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            logicVoiceContent.voiceItem.Add(voiceItem);
            LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvVoiceAddItem(dataGridView1, logicVoiceContent.voiceItem, ip);
            DgvMesege.selectLastCount(dataGridView1); 
        }

        /// <summary>
        /// 添加表达式判断项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnElseAdd_Click(object sender, EventArgs e)
        {
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            DataJson.VoiceContent logicVoiceContent;
            if (string.IsNullOrEmpty(LogicInfo.content))
            {
                logicVoiceContent = new DataJson.VoiceContent();
            }
            else
            {
                logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

            }
            DataJson.VoiceIfItem voiceIfItem = new DataJson.VoiceIfItem();
            voiceIfItem.id = logicVoiceContent.voiceIfItem.Count + 1;
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            logicVoiceContent.voiceIfItem.Add(voiceIfItem);
            LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(dataGridView2, logicVoiceContent.voiceIfItem, ip);
            DgvMesege.selectLastCount(dataGridView2); 
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

        #region 表达项表格 dataGridView1

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

                            string id = dataGridView1.Rows[rowCount].Cells[0].Value.ToString();
                            switch (dataGridView1.Columns[columnCount].Name)
                            {
                                case "address1":
                                    
                                    //改变地址
                                    string obj = "";
                                    if (dataGridView1.Rows[rowCount].Cells[2].Value != null)
                                    {
                                        //原地址
                                        obj = dataGridView1.Rows[rowCount].Cells[2].Value.ToString();
                                    }
                                    string objType = dataGridView1.Rows[rowCount].Cells[1].EditedFormattedValue.ToString();
                                    //赋值List 并添加地域 名字
                                    dgvAddress1(id, objType, obj);
                                    
                                    break;
 
                                case "del1":
                                    //删除表
                                    dgvDel1(id);

                                    break;
                                case "num1":
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
                            string id = dataGridView1.Rows[rowCount].Cells[0].Value.ToString();
                            switch (dataGridView1.Columns[columnCount].Name)
                            {
                                case "address1":
                                    setTitleAddress1();
                                    break;
                                case "section1":
                                    setTitleAddress1();
                                    break;
                                case "name1":
                                    setTitleAddress1();
                                    break;
                                case "del1":
                                    //删除表
                                    dgvDel1(id);

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

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            CellMouseDown(dataGridView1, e, doubleClickTimer1);
            ScrollRowCount1 = e.RowIndex;
            ScrollColumnCount1 = e.ColumnIndex;
        }

        private void dataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            CellMouseMove(dataGridView1, e);
        }


        /// <summary>
        /// 获取新的地址 刷新地域 名字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objType">当前对象的类型</param>
        /// <param name="obj">当前对象的地址值</param>
        /// <returns></returns>
        private void dgvAddress1(string id, string objType, string obj)
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
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

                DataJson.VoiceItem info = DataListHelper.getLogicVoiceItem(id, logicVoiceContent.voiceItem);
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
 
                            info.type = point.type;
                        }
                    }
                    else
                    {
                        //搜索一次dev表 
                        info.pid = 0;
                        if (info.type != type)
                        {

                            info.type = type;
                        }

                    }


                }
                LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvVoiceAddItem(dataGridView1, logicVoiceContent.voiceItem, ip);
                DgvMesege.RecoverDgvForm(dataGridView1, X_Value1, Y_Value1, ScrollRowCount1, ScrollColumnCount1);
            }//ok


        }

        private void dgvDel1(string id)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

            DataJson.VoiceItem info = DataListHelper.getLogicVoiceItem(id, logicVoiceContent.voiceItem);
            if (info == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (!logicVoiceContent.voiceItem.Remove(info))
            {
                return;
            }
            //排序
            VoiceItemSort(logicVoiceContent.voiceItem);
            LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvVoiceAddItem(dataGridView1, logicVoiceContent.voiceItem, ip);
        }

        private void VoiceItemSort(List<DataJson.VoiceItem> infos)
        {
            int i = 0;
            foreach (DataJson.VoiceItem info in infos)
            {
                info.letter = letter[i];
                i++;
            }
        }


        /// <summary>
        /// 复制title选中的节点 赋地址给ObjAddress
        /// </summary>
        private void setTitleAddress1()
        {
            try
            {
                int colIndex = dataGridView1.SelectedCells[0].ColumnIndex;
                //string ip = FileMesege.panelSelectNode.Parent.Text.Split(' ')[0];
                int id = dataGridView1.CurrentCell.RowIndex;

                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null || string.IsNullOrEmpty(LogicInfo.content))
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

                DataJson.VoiceItem info = DataListHelper.getLogicVoiceItem(dataGridView1.Rows[id].Cells[0].Value.ToString(), logicVoiceContent.voiceItem);
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

                    dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView1.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView1.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView1.Rows[id].Cells[4].Value = eq.name;
              
              

                }
                LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            catch
            {

            }

        }

        #endregion


        #region 表达判断项 表格dataGridView2
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
                                    if (dataGridView2.Rows[rowCount].Cells[3].Value != null)
                                    {
                                        //原地址
                                        obj = dataGridView2.Rows[rowCount].Cells[3].Value.ToString();
                                    }
                                    string objType = dataGridView2.Rows[rowCount].Cells[2].EditedFormattedValue.ToString();
                                    //赋值List 并添加地域 名字
                                    dgvAddress2(id, objType, obj);
                                    
                                    break;
                                case "operation2":
                                    
                                    //操作
                                    string info = dgvOperation2(id, dataGridView2.Rows[rowCount].Cells[2].EditedFormattedValue.ToString());
                                    if (!string.IsNullOrEmpty(info))
                                    {
                                        dataGridView2.Rows[rowCount].Cells[6].Value = info;
                                    }
                                    break;
                                case "del2":
                                    //删除表
                                    dgvDel2(id);

                                    break;
                                case "result":
                                    //结果
                                    info = dgvResult(id, dataGridView2.Rows[rowCount].Cells[1].EditedFormattedValue.ToString());
                                    if (!string.IsNullOrEmpty(info))
                                    {
                                        dataGridView2.Rows[rowCount].Cells[1].Value = info;
                                    }
                                  

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
                             
                                    dataGridView2.Columns[7].ReadOnly = false;
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


        string tmpDelay = "";
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
                            if (dataGridView2.Rows[rowNum].Cells[7].Value != null )
                            {
                                tmpDelay = dataGridView2.Rows[rowNum].Cells[7].Value.ToString() ;
                            }
                            dataGridView2.Columns[7].ReadOnly = true;
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
                            if (dataGridView2.Rows[rowNum].Cells[7].Value != null && Validator.IsNumber(dataGridView2.Rows[rowNum].Cells[7].Value.ToString()))
                            {
                                //改变延时
                                dgvDelay2(Convert.ToInt32(dataGridView2.Rows[rowNum].Cells[0].Value), Convert.ToDouble(dataGridView2.Rows[rowNum].Cells[7].Value));
                            }
                            else
                            {
                                AppTxtShow("延时格式错误，请正确填写！");
                                dataGridView2.Rows[rowNum].Cells[7].Value = tmpDelay;
                            }
                            break;

                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        private string dgvResult(int id, string result)
        {
            LogicResult dc = new LogicResult();
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return null;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

            DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id, logicVoiceContent.voiceIfItem);
            if (info == null)
            {
                return null;
            }
            dc.StartPosition = FormStartPosition.CenterParent;
            dc.Result = result;
            dc.ShowDialog();
            if (dc.DialogResult == DialogResult.OK)
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                info.result = dc.ReturnResult;
                LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                return dc.ReturnResult;
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
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

                DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id, logicVoiceContent.voiceIfItem);
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
                LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                dgvAddItem(dataGridView2, logicVoiceContent.voiceIfItem, ip);
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
            DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

            DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id, logicVoiceContent.voiceIfItem);
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
                LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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
            DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

            DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id, logicVoiceContent.voiceIfItem);
            if (info == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            info.delay = Convert.ToInt32(time * 10);
            LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);

        }

        private void dgvDel2(int id)
        {
            //找到当前操作tab对象
            DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
            if (LogicInfo == null)
            {
                return;
            }
            //把tab对象JSON字符串转换为 操作对象
            DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

            DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id, logicVoiceContent.voiceIfItem);
            if (info == null)
            {
                return;
            }
            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (!logicVoiceContent.voiceIfItem.Remove(info))
            {
                return;
            }
            //排序
            voiceIfItemSort(logicVoiceContent.voiceIfItem);
            LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvAddItem(dataGridView2, logicVoiceContent.voiceIfItem, ip);

        }

        private void voiceIfItemSort(List<DataJson.VoiceIfItem> infos)
        {
            int i = 1;
            foreach (DataJson.VoiceIfItem info in infos)
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
                //string ip = FileMesege.panelSelectNode.Parent.Text.Split(' ')[0];
                int id = dataGridView2.CurrentCell.RowIndex ;

                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

                DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id + 1, logicVoiceContent.voiceIfItem);
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

                    dataGridView2.Rows[id].Cells[3].Value = DgvMesege.addressTransform(info.address);
                    dataGridView2.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView2.Rows[id].Cells[5].Value = eq.name;

                }
                else
                {
                    info.pid = eq.pid;
                    info.address = eq.address;
                    info.type = eq.type;
                    info.opt = "";
                    info.optName = "";
                    dataGridView2.Rows[id].Cells[3].Value = DgvMesege.addressTransform(info.address);
                    dataGridView2.Rows[id].Cells[2].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView2.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();//改根据地址从信息里面获取
                    dataGridView2.Rows[id].Cells[5].Value = eq.name;
                    dataGridView2.Rows[id].Cells[6].Value = null;


                }
                LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

                
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                for (int i = 0; i < dataGridView2.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView2.SelectedCells[i].ColumnIndex;

                    //当粘贴选中单元格为操作
                    if (colIndex == 6)
                    {
                        int id = Convert.ToInt32(dataGridView2.Rows[dataGridView2.SelectedCells[i].RowIndex].Cells[0].Value);
         
                        DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id, logicVoiceContent.voiceIfItem);
                        if (info == null)
                        {
                            continue;
                        }

                        ischange = true;
                        info.opt = "";
                        info.optName = "";
                        dataGridView2.Rows[dataGridView2.SelectedCells[i].RowIndex].Cells[6].Value = null;

                    }//if
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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
            /*
            //获取当前选中单元格的列序号
            int colIndex = dataGridView2.CurrentRow.Cells.IndexOf(dataGridView2.CurrentCell);
            //当粘贴选中单元格为操作
            if (colIndex == 6)
            {
                int id = Convert.ToInt32(dataGridView2.CurrentRow.Cells[0].Value);
                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);

                DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id, logicVoiceContent.voiceIfItem);
                if (info == null)
                {
                    return;
                }
                //获取sceneInfo对象表中对应ID号info对象
                FileMesege.copyLogicScene = info;

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
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                for (int i = 0; i < dataGridView2.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView2.SelectedCells[i].ColumnIndex;

                    //当粘贴选中单元格为操作
                    if (colIndex == 6)
                    {
                        int id = Convert.ToInt32(dataGridView2.Rows[dataGridView2.SelectedCells[i].RowIndex].Cells[0].Value);
                        DataJson.VoiceIfItem info = DataListHelper.getLogicVoiceIfItem(id, logicVoiceContent.voiceIfItem);
                        if (info == null)
                        {
                            return;
                        }

                        if (FileMesege.copyLogicScene.type == "" || info.type == "" || info.type != FileMesege.copyLogicScene.type)
                        {
                            continue;
                        }
                        ischange = true;
                        info.opt = FileMesege.copyLogicScene.opt;
                        info.optName = FileMesege.copyLogicScene.optName;

                        dataGridView2.Rows[dataGridView2.SelectedCells[i].RowIndex].Cells[6].Value = (info.optName + " " + info.opt).Trim();
                    }//if
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                  
                }

            }//try
            catch
            {

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

                //选中行 序号
                int id = 0;
                //列号
                int colIndex = 0;
                //记录第一个选中格的列号
                int FirstColumnIndex = -1;
                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
                DataJson.VoiceItem info = null;
                DataJson.VoiceItem copyInfo = null;
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

                    info = DataListHelper.getLogicVoiceItem(dataGridView1.Rows[id].Cells[0].Value.ToString(), logicVoiceContent.voiceItem);
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
                    if (colIndex == 2)
                    {
                        //选中单元格为地址
                        info.address = copyInfo.address;
             
                        info.type = copyInfo.type ;
                        info.pid = copyInfo.pid;
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByPid(info.pid);
                        if (point != null)
                        {
                            info.type = point.type;
                            dataGridView1.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView1.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id].Cells[4].Value = string.Empty;
                        }
                        dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView1.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        ischange = true;

                    }
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
                DataJson.VoiceIfItem info = null;
                DataJson.VoiceIfItem copyInfo = null;
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
                    info = DataListHelper.getLogicVoiceIfItem(id + 1, logicVoiceContent.voiceIfItem);
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
                    if (colIndex == 6)
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
                        dataGridView2.Rows[id].Cells[6].Value = (info.optName + " " + info.opt).Trim();
                    }//if
                    else if (colIndex == 3)
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
                            dataGridView2.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView2.Rows[id].Cells[5].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView2.Rows[id ].Cells[4].Value = string.Empty;
                            dataGridView2.Rows[id ].Cells[5].Value = string.Empty;
                        }
                        dataGridView2.Rows[id ].Cells[2].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView2.Rows[id ].Cells[3].Value = DgvMesege.addressTransform(info.address);
                        dataGridView2.Rows[id ].Cells[6].Value = (info.optName + " " + info.opt).Trim();

                        ischange = true;

                    }
                    else if (colIndex == 7)
                    {
                        //延时相同
                        ischange = true;
                        info.delay = copyInfo.delay;
                        dataGridView2.Rows[id ].Cells[7].Value = Convert.ToDouble(info.delay) / 10;
                    }
                    else if (colIndex == 1)
                    {
                        //结果相同
                        ischange = true;
                        info.result = copyInfo.result;
                        dataGridView2.Rows[id].Cells[1].Value = info.result;
                    }

                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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
                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
                DataJson.VoiceItem info = null;
                DataJson.VoiceItem copyInfo = null;
                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = dataGridView1.SelectedCells[i].RowIndex;
                        info = DataListHelper.getLogicVoiceItem(dataGridView1.Rows[id].Cells[0].Value.ToString(), logicVoiceContent.voiceItem);
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
                    info = DataListHelper.getLogicVoiceItem(dataGridView1.Rows[id].Cells[0].Value.ToString(), logicVoiceContent.voiceItem);
                    if (info == null)
                    {
                        return;
                    }
                    if (addCount == 0)
                    {
                        continue;
                    }
                    if (colIndex == 2)
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

                        info.address = DgvMesege.addressAdd(copyInfo.address, addCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        info.type = IniHelper.findIniTypesByAddress(ip, info.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                        if (point != null)
                        {
                            info.pid = point.pid;
                            info.type = point.type;
                            dataGridView1.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView1.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id].Cells[4].Value = string.Empty;
                        }
                        dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView1.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        ischange = true;

                    }
                    addCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
                DataJson.VoiceIfItem info = null;
                DataJson.VoiceIfItem copyInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView2.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                        id = dataGridView2.SelectedCells[i].RowIndex;
                        info = DataListHelper.getLogicVoiceIfItem(id + 1, logicVoiceContent.voiceIfItem);
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
                    info = DataListHelper.getLogicVoiceIfItem(id + 1, logicVoiceContent.voiceIfItem);
                    if (info == null)
                    {
                        return;
                    }

                    //延时递增
                    if (colIndex == 7)
                    {

                        ischange = true;
                        info.delay = copyInfo.delay + addCount * Convert.ToInt32(FileMesege.AsDesCendingNum * 10);
                        dataGridView2.Rows[id].Cells[7].Value = Convert.ToDouble(info.delay) / 10;
                    }//if
                    else if (colIndex == 3)
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
                            dataGridView2.Rows[id ].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView2.Rows[id ].Cells[5].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView2.Rows[id ].Cells[4].Value = string.Empty;
                            dataGridView2.Rows[id ].Cells[5].Value = string.Empty;
                        }
                        info.opt = string.Empty;
                        info.optName = string.Empty;
                        dataGridView2.Rows[id].Cells[2].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView2.Rows[id].Cells[3].Value = DgvMesege.addressTransform(info.address);
                        dataGridView2.Rows[id].Cells[6].Value = (info.optName + " " + info.opt).Trim();
                       
                        ischange = true;

                    }
                    addCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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
                //找到当前操作tab对象
                DataJson.logicsInfo LogicInfo = DataListHelper.findLogicInfoByTabName(FileMesege.LogicTabName);
                if (LogicInfo == null)
                {
                    return;
                }
                //把tab对象JSON字符串转换为 操作对象
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
                DataJson.VoiceItem info = null;
                DataJson.VoiceItem copyInfo = null;
                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView1.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                        id = dataGridView1.SelectedCells[i].RowIndex;
                        info = DataListHelper.getLogicVoiceItem(dataGridView1.Rows[id].Cells[0].Value.ToString(), logicVoiceContent.voiceItem);
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
                    info = DataListHelper.getLogicVoiceItem(dataGridView1.Rows[id].Cells[0].Value.ToString(), logicVoiceContent.voiceItem);
                    if (info == null)
                    {
                        return;
                    }
                    if (reduceCount == 0)
                    {
                        continue;
                    }
                    if (colIndex == 2)
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

                        info.address = DgvMesege.addressReduce(copyInfo.address, reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum));
                        info.type = IniHelper.findIniTypesByAddress(ip, info.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                        if (point != null)
                        {
                            info.pid = point.pid;
                            info.type = point.type;
                            dataGridView1.Rows[id].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[id].Cells[4].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView1.Rows[id].Cells[3].Value = string.Empty;
                            dataGridView1.Rows[id].Cells[4].Value = string.Empty;
                        }
                        dataGridView1.Rows[id].Cells[2].Value = DgvMesege.addressTransform(info.address);
                        dataGridView1.Rows[id].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                        ischange = true;

                    }
                    reduceCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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
                DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
                DataJson.VoiceIfItem info = null;
                DataJson.VoiceIfItem copyInfo = null;

                //把第一行的数目 和 列数记录起来
                for (int i = dataGridView2.SelectedCells.Count - 1; i >= 0; i--)
                {
                    colIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                    if (FirstColumnIndex == -1 && FirstColumnIndex != colIndex)
                    {
                        //只操作单选的列
                        FirstColumnIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                        id = dataGridView2.SelectedCells[i].RowIndex;
                        info = DataListHelper.getLogicVoiceIfItem(id + 1, logicVoiceContent.voiceIfItem);
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
                    info = DataListHelper.getLogicVoiceIfItem(id + 1, logicVoiceContent.voiceIfItem);
                    if (info == null)
                    {
                        return;
                    }

                    //延时递减
                    if (colIndex == 7)
                    {

                        ischange = true;
                        info.delay = copyInfo.delay - reduceCount * Convert.ToInt32(FileMesege.AsDesCendingNum * 10);
                        if (info.delay < 0)
                        {
                            info.delay = 0;
                        }
                        dataGridView2.Rows[id].Cells[7].Value = Convert.ToDouble(info.delay) / 10;
                    }//if
                    else if (colIndex == 3)
                    {
                        //地址递减
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
                        info.type = IniHelper.findIniTypesByAddress(ip, info.address).Split(',')[0];
                        //添加地域和名称 在sceneInfo表中
                        DataJson.PointInfo point = DataListHelper.findPointByType_address("", info.address, ip);
                        if (point != null)
                        {
                            info.pid = point.pid;
                            info.type = point.type;
                            dataGridView2.Rows[id].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView2.Rows[id].Cells[5].Value = point.name;
                        }
                        else
                        {
                            info.pid = 0;
                            dataGridView2.Rows[id].Cells[4].Value = string.Empty;
                            dataGridView2.Rows[id].Cells[5].Value = string.Empty;
                        }
                        info.opt = string.Empty;
                        info.optName = string.Empty;
                        dataGridView2.Rows[id].Cells[2].Value = IniHelper.findTypesIniNamebyType(info.type);
                        dataGridView2.Rows[id].Cells[3].Value = DgvMesege.addressTransform(info.address);
                        dataGridView2.Rows[id].Cells[6].Value = (info.optName + " " + info.opt).Trim();

                        ischange = true;

                    }
                    reduceCount--;
                }
                if (ischange)
                {
                    LogicInfo.content = JsonConvert.SerializeObject(logicVoiceContent);
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

        #endregion


    }
}
