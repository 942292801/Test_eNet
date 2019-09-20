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
using System.Text.RegularExpressions;
using eNet编辑器.ThreeView;

namespace eNet编辑器.DgvView
{
    public partial class DgvPoint : Form
    {
        public event Action<string> txtAppShow;
        //更新title树状图
        public event Action updateTitleNode;

        //更新所有窗口
        public event Action updatePointView;
        public DgvPoint()
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

 
        DataGridViewComboBoxColumn cbObjType;
        DataGridViewComboBoxColumn cbValue;
        HashSet<DataJson.PointInfo> multipleList = new HashSet<DataJson.PointInfo>();
        
        private void DgvPoint_Load(object sender, EventArgs e)
        {
            //新增对象列 加载
            this.dataGridView1.Rows.Clear();
            cbObjType = new DataGridViewComboBoxColumn();
            cbValue = new DataGridViewComboBoxColumn();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//objs");
            string name = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {

                name = IniConfig.GetValue(file.FullName, "define", "name");
                if (name != "")
                {
                    cbObjType.Items.Add(name);
                }
            }
            //设置列名
            cbObjType.HeaderText = "对象";
            //设置下拉列表的默认值 
            cbObjType.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //cbObjType.DefaultCellStyle.NullValue = cbObjType.Items[0];
            cbObjType.Name = "pointObjType";
            cbObjType.ReadOnly = true;

            //设置列名
            cbValue.HeaderText = "参数";
            //设置下拉列表的默认值 
            cbValue.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            cbValue.ReadOnly = true;
            cbValue.Name = "pointValue";

            //插入执行对象
            this.dataGridView1.Columns.Insert(4, cbObjType);
            //插入执行模式
            this.dataGridView1.Columns.Insert(5, cbValue);
        }

        #region dgv的数据加载 更新combox的item值
        //根据区域和 类型TYPE（灯。空调。电视。）
        public void dgvPointAddItemByObjType()
        {
            try
            {
                //清空并联点
                multipleList.Clear();
                this.dataGridView1.Rows.Clear();
                if (string.IsNullOrEmpty(FileMesege.sectionNodeCopy))
                {
                    return;
                }
         
                //搜索选中区域  加载所有同区域的节点
                //区域
                string[] sections = FileMesege.sectionNodeCopy.Split('\\');
                if (sections[0] == "查看所有区域")
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                    {
                        if (!string.IsNullOrEmpty(FileMesege.objType) && FileMesege.objType == "所有点位")
                        {

                        }
                        else if (!string.IsNullOrEmpty(FileMesege.objType) && eq.objType != FileMesege.objType)
                        {
                            continue;
                        }
                        //加载信息
                        CountAddInfo(eq);
                    }
                }
                else
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                    {
                        if (!string.IsNullOrEmpty(FileMesege.objType) && FileMesege.objType == "所有点位")
                        {
                            
                        }else if(!string.IsNullOrEmpty(FileMesege.objType) && eq.objType != FileMesege.objType)
                        {
                            continue;
                        }
                        // 1 2 3 4 用于区分点击区域 区分加载搜索信息 例如东区 把东区所有信息加载进来 
                        if (string.IsNullOrEmpty(sections[1]) && eq.area1 == sections[0])
                        {
                            //加载信息
                            CountAddInfo(eq);
                        }
                        // 1 2 3 0
                        else if (string.IsNullOrEmpty(sections[2]) && eq.area1 == sections[0] && eq.area2 == sections[1])
                        {
                            //加载信息
                            CountAddInfo(eq);
                        }
                        // 1 2 0 0
                        else if (string.IsNullOrEmpty(sections[3]) && eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2])
                        {
                            //加载信息
                            CountAddInfo(eq);
                        }
                        //1 0 0 0
                        else if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3])
                        {
                            //加载信息
                            CountAddInfo(eq);
                        }

                    }
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //报错不作处理
            }
        }

        /// <summary>
        /// 加载信息到Dgv表中 根据区域
        /// </summary>
        public void dgvPointAddItemBySection()
        {
            try
            {
                //清空并联点
                multipleList.Clear();
                this.dataGridView1.Rows.Clear();
                if (string.IsNullOrEmpty( FileMesege.sectionNodeCopy))
                {
                    return;
                }
                //搜索选中区域  加载所有同区域的节点
                //区域
                string[] sections = FileMesege.sectionNodeCopy.Split('\\');
                if (sections[0] == "查看所有区域")
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                    {
                        //加载信息
                        CountAddInfo(eq);
                    }
                }
                else
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                    {
                        // 1 2 3 4 用于区分点击区域 区分加载搜索信息 例如东区 把东区所有信息加载进来 
                        if (string.IsNullOrEmpty(sections[1]) && eq.area1 == sections[0])
                        {
                            //加载信息
                            CountAddInfo(eq);
                        }
                        // 1 2 3 0
                        else if (string.IsNullOrEmpty(sections[2]) && eq.area1 == sections[0] && eq.area2 == sections[1])
                        {
                            //加载信息
                            CountAddInfo(eq);
                        }
                        // 1 2 0 0
                        else if (string.IsNullOrEmpty(sections[3]) && eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2])
                        {
                            //加载信息
                            CountAddInfo(eq);
                        }
                        //1 0 0 0
                        else if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3])
                        {
                            //加载信息
                            CountAddInfo(eq);
                        }

                    }
                }
                
            }
            catch {
                this.dataGridView1.Rows.Clear();
                //MessageBox.Show(e.Message + "\r\n后期可以屏蔽该错误");
                //报错不作处理
            }
        }


        /// <summary>
        /// 添加每一行的信息
        /// </summary>
        /// <param name="eq"></param>
        private void CountAddInfo(DataJson.PointInfo eq)
        {
            //添加新的一行 rowNum为行号
            int  rowNum = this.dataGridView1.Rows.Add();
            //序号
            this.dataGridView1.Rows[rowNum].Cells[0].Value = (rowNum + 1);
            //地址
            this.dataGridView1.Rows[rowNum].Cells[1].Value = DgvMesege.addressTransform( eq.address);
            //区域
            this.dataGridView1.Rows[rowNum].Cells[2].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();
            //名字
            this.dataGridView1.Rows[rowNum].Cells[3].Value = eq.name.Split('@')[0];
            //获取中文类型名
            string objTypeName = IniHelper.findObjsDefineName_ByType(eq.objType);
            //类型 objType
            this.dataGridView1.Rows[rowNum].Cells[4].Value = objTypeName;
            if (!string.IsNullOrEmpty(objTypeName))
            {
                updataValueItem(rowNum, objTypeName);
                //参数 value
                this.dataGridView1.Rows[rowNum].Cells[5].Value = IniHelper.findObjValueName_ByVal(objTypeName, eq.value);
                this.dataGridView1.Rows[rowNum].Cells[5].ReadOnly = true;
            }         
         


            this.dataGridView1.Rows[rowNum].Cells[6].Value = "删除";
        }


   
        /// <summary>
        /// 更新 表中参数项 的item值  随objType的值变化而变化
        /// </summary>
        /// <param name="rownum">行号</param>
        /// <param name="objTypeName">objTpe类型 中文的</param>
        private void updataValueItem(int rownum, string objTypeName)
        {
            
            //把中文类型 转换为英语类型
            string objType = IniHelper.findObjsFileNae_ByName(objTypeName);
            if (string.IsNullOrEmpty(objType))
            {
                return;
            }
            /**********************************************************************************
             重点  在某一列上添加新combox  重点注意 需要屏蔽出错提示dataGridView1_DataError事件
            ***********************************************************************************/
            DataGridViewComboBoxCell combox = dataGridView1.Rows[rownum].Cells["pointValue"] as DataGridViewComboBoxCell;
            combox.ReadOnly = true;
            combox.Items.Clear();
            string filepath = string.Format("{0}\\objs\\{1}.ini", Application.StartupPath, objType);
            string tmp ="";
             //修改mode的combox列表
            List<string> keys = IniConfig.ReadSections(filepath);
            for (int i = 0; i < keys.Count; i++)
            {
                if(keys[i].Contains("value"))
                {
                    tmp = IniConfig.GetValue(filepath, keys[i], "name");
                    
                        combox.Items.Add(tmp);
                        //txtAppShow(tmp);
                    
                    
                }
                
            }
            

        }

        #endregion

        #region 增加 并联按钮 删除
        /// <summary>
        /// 增加按钮 新增一条点位信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            addPoint(ThreeTitle.keys[FileMesege.cbTypeIndex]);
        }

        /// <summary>
        /// 添加点位 
        /// </summary>
        /// <param name="typeName">类型名</param>
        public void addPoint(string typeName)
        {
            if (string.IsNullOrEmpty(FileMesege.sectionNodeCopy))
            {
                txtAppShow("请选择区域");
                return;
            }
            else if (string.IsNullOrEmpty(FileMesege.titleinfo))
            {
                txtAppShow("请选择名称");
                return;
            }
            //搜索选中区域  加载所有同区域的节点
            //区域
            string[] sect = FileMesege.sectionNodeCopy.Split('\\');
            if (sect[0] == "查看所有区域")
            {
                txtAppShow("请选择区域！");
                return;
            }
            //计算name的排序
            DataJson.PointInfo point = new DataJson.PointInfo();
            point.pid = DataChange.randomNum();
            point.name = string.Format("{0}@255", sortName(sect));
            point.address = "FFFFFFFF";
            point.area1 = sect[0];
            point.area2 = sect[1];
            point.area3 = sect[2];
            point.area4 = sect[3];
            point.ip = "";
       
            point.objType = IniHelper.findObjsFileNae_ByName(typeName);

            point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, "value1");
            if (string.IsNullOrEmpty(point.type))
            {
                point.value = "";
            }
            else
            {
                point.value = "value1";
            }
            

            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            FileMesege.PointList.equipment.Add(point);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            dgvPointAddItemBySection();
        }

        /// <summary>
        /// 名称自动添加序号 排序
        /// </summary>
        /// <param name="section">位置</param>
        /// <returns></returns>
        private string sortName(string[] section)
        {
            HashSet<int> hasharry = new HashSet<int>();
            string num = "";
            //判断当前是否有匹配
            //计算有多小个相同区域的信息
            //循环所有信息
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                //当地域信息相同
                if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3])
                {
                    //名字包含该信息
                    if (!string.IsNullOrEmpty(eq.name))
                    {
                        //获取序号
                        num = eq.name.Split('@')[0].Replace(FileMesege.titleinfo, ""); //Regex.Replace(eq.name.Split('@')[0], @"[^\d]*", "");

                        if (Regex.IsMatch(num, @"^[+-]?\d*[.]?\d*$"))
                        {
                            hasharry.Add(Convert.ToInt32(num));
                        }
                    }
                }
            }
            //哈希表 同一个区域的所有序号都在里面
            List<int> arry = hasharry.ToList<int>();
            arry.Sort();
            if (arry.Count == 0)
            {
                //该区域节点前面数字不存在
                return FileMesege.titleinfo + "1";
            }
            //哈希表 不存在序号 直接返回
            for (int i = 0; i < arry.Count; i++)
            {
                if (arry[i] != i + 1)
                {
                    return FileMesege.titleinfo + (i + 1).ToString();
                }
            }
            return FileMesege.titleinfo + (arry[arry.Count - 1] + 1).ToString();
     

        }

        /// <summary>
        /// 合并point点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMultiple_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if ((bool)dataGridView1.Rows[i].Cells[7].EditedFormattedValue)
                {
                    Multiple(i);
                }
            }
            if (dataGridView1.RowCount < 1 || multipleList.Count < 2)
            {
                //没有选中数据
                return;
            }
            else
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                int i = 0;
                foreach (DataJson.PointInfo point in multipleList)
                {
                    if (i == 0)
                    {
                        i++;
                        continue;
                    }
                    DataListHelper.reMoveAllSceneByPid(point.pid);
                    FileMesege.PointList.equipment.Remove(point);
                    i++;
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                multipleList.Clear();
                updatePointView();
            }

        }


        //删除
        private void btnDel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if ((bool)dataGridView1.Rows[i].Cells[7].EditedFormattedValue)
                {
                    Multiple(i);
                }
            }
                
            if (dataGridView1.RowCount < 1 || multipleList.Count == 0)
            {
                //没有选中数据
                return;
            }
            else
            {
                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                foreach (DataJson.PointInfo point in multipleList)
                {

                    DataListHelper.reMoveAllSceneByPid(point.pid);
                    FileMesege.PointList.equipment.Remove(point);
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                multipleList.Clear();
                updatePointView();
            }
        }
        #endregion

        #region 单击双击 操作DGV表格
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
        private string oldName = "";

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
                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            case "pointName":
                                //处理双击事件操作
                                dataGridView1.Columns[3].ReadOnly = false;
                                if (dataGridView1.Rows[rowCount].Cells[3].Value != null)
                                {
                                    oldName = dataGridView1.Rows[rowCount].Cells[3].Value.ToString().Trim();
                                }
                                else
                                {
                                    oldName = "";
                                }

                                break;
                            case "pointObjType":
                                cbObjType.ReadOnly = false;
                                break;
                            case "pointValue":
                                cbValue.ReadOnly = false;
                                
                                break;
                            case "pointDel":
                                //删除当前行点位信息
                                delBtn();

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
                    //DGV的行号

                    if (rowCount >= 0 && columnCount >= 0)
                    {
                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            
                            case "pointName":
                                if(string.IsNullOrEmpty( FileMesege.titleinfo))
                                {
                                    break;
                                }
                                //添加名称
                                changeName(); 
                                break;

                            case "pointDel": 
                                //删除当前行点位信息
                                delBtn();
                                
                                break;
                            case "pointAdd":
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


        //单元格结束编辑
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
       
            //选中行号
            int rowNum = e.RowIndex;
            //选中列号
            int columnNum = e.ColumnIndex;
            DataJson.PointInfo eq = null;
            //撤销
            DataJson.totalList OldList = null;

            DataJson.totalList NewList = null;
            if (rowNum >= 0 && columnNum >= 0)
            {
                switch (dataGridView1.Columns[columnNum].Name)
                {
                    case "pointName":
                        //手动双击编辑名称
                        editName(rowNum);

                        break;

                    case "pointObjType":
                        //保存当前信息
                        eq = findNowRow(rowNum);
                        if (eq == null)
                        {
                            SocketUtil.DelayMilli(100);
                            eq = findNowRow(rowNum);
                            if (eq == null)
                            {
                                MessageBox.Show("699行信息保存失败");
                                break;
                            }
                        }
                        
                        OldList = FileMesege.cmds.getListInfos();
                        string objtype = IniHelper.findObjsFileNae_ByName(dataGridView1.Rows[rowNum].Cells[4].EditedFormattedValue.ToString());
                        if( eq.objType != objtype)
                        {
                            eq.value = "";
                            eq.objType = objtype;
                            eq.type = "";
                            eq.address = "FFFFFFFF";
                            eq.ip = "";
                            dataGridView1.Rows[rowNum].Cells[1].Value = "255.255.255.255";
                            dataGridView1.Rows[rowNum].Cells[5].Value = null;
                            NewList = FileMesege.cmds.getListInfos();
                            FileMesege.cmds.DoNewCommand(NewList, OldList);
                        }
                        
                        cbObjType.ReadOnly = true;
                        break;

                    case "pointValue":
                        //保存当前信息
                         eq = findNowRow(rowNum);
                        if (eq == null)
                        {
                            SocketUtil.DelayMilli(100);
                            eq = findNowRow(rowNum);
                            if (eq == null)
                            {
                                MessageBox.Show("699行信息保存失败");
                                break;
                            }
                        }
                        
                        if (dataGridView1.Rows[rowNum].Cells[4].Value == null || dataGridView1.Rows[rowNum].Cells[5].Value == null)
                        {
                            return;
                        }
                        OldList = FileMesege.cmds.getListInfos();
                        //类型需要 检查是否匹配一致
                        string type = IniHelper.findObjValueType_ByVal(dataGridView1.Rows[rowNum].Cells[4].Value.ToString(), dataGridView1.Rows[rowNum].Cells[5].EditedFormattedValue.ToString());
                        //解绑IP地址
                        if ( eq.address != "FFFFFFFF" &&!string.IsNullOrEmpty(eq.type) && eq.type != type)
                        {
                            eq.address = "FFFFFFFF";
                            eq.ip = "";
                            dataGridView1.Rows[rowNum].Cells[1].Value = "255.255.255.255";
                        }
                        
                        eq.value = IniHelper.findObjSectionValue_ByValName(dataGridView1.Rows[rowNum].Cells[4].Value.ToString(), dataGridView1.Rows[rowNum].Cells[5].EditedFormattedValue.ToString());
                        eq.type = type;
                        NewList = FileMesege.cmds.getListInfos();
                        cbValue.ReadOnly = true;
                        FileMesege.cmds.DoNewCommand(NewList, OldList);
                        
                        break;
                    case "pointMultiple":
                 
                        
                        break;
                    default: break;
                }
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
                    case "pointValue":
                        //更新item选项
                        string objTypeName = "";
                        if (dataGridView1.Rows[rowNum].Cells[4].Value != null)
                        {
                            objTypeName = dataGridView1.Rows[rowNum].Cells[4].Value.ToString();
                        }
                        updataValueItem(rowNum, objTypeName);
                        break;
                    case "pointMultiple":
                        dataGridView1.Rows[rowNum].Selected = true;//选中行
     
                        //multipleList.Clear();
                        for (int i = dataGridView1.SelectedRows.Count; i > 0; i--)
                        {
                    
                             dataGridView1.SelectedRows[i - 1].Cells[7].Value = true;                         
                        }
                        //提交编辑
                        dataGridView1.EndEdit();
                        break;
                    

                    default: break;
                }
            }
        }


        //重点注意 需要屏蔽出错提示dataGridView1_DataError事件
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }


        /// <summary>
        /// 展示该节点的信息到txt中
        /// </summary>
        private void dgvShowTxt()
        {
            
            //区域加名称
            DataJson.PointInfo point = findNowRow(rowCount);
            if (point == null)
            {
                return;
            }
            int id = Convert.ToInt32( point.address.Substring(4,2),16);
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == point.ip)
                {
                    foreach (DataJson.Module md in dev.module)
                    {
                        if (md.id == id)
                        {
                            string section = string.Format("{0} {1} {2} {3}", md.area1, md.area2, md.area3, md.area4).Trim().Replace(" ", "\\");
                            txtAppShow(string.Format("当前地址信息 ：{0}--->{1}--->{2}--->{3} ", dev.ip, md.device, section, md.name));//改根据地址从信息里面获取
                            break;
                        }
                    }
                    break;
                }
            }
            
        }


        /// <summary>
        /// 修改添加名称值
        /// </summary>
        private void changeName()
        {
            
            //区域
            List<string> section = dataGridView1.Rows[rowCount].Cells[2].Value.ToString().Split(' ').ToList();
            while (section.Count != 4)
            {
                section.Add("");
            }
            oldName = dataGridView1.Rows[rowCount].Cells[3].Value.ToString();
            if (oldName.Contains(FileMesege.titleinfo))
            {
                //该名称的类型已经存在
                return;
            }   
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                //当地域信息相同
                if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name.Split('@')[0] == oldName)
                {
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    string name = sortName(section.ToArray());
                    eq.name = string.Format("{0}@{1}", name, eq.name.Split('@')[1]);
                    dataGridView1.Rows[rowCount].Cells[3].Value = name;
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    break;
                }
            }
            


        }

        /// <summary>
        /// 删除本行点位信息按钮
        /// </summary>
        private void delBtn()
        {
            //区域
            List<string> section = dataGridView1.Rows[rowCount].Cells[2].Value.ToString().Split(' ').ToList();
            while (section.Count != 4)
            {
                section.Add("");
            }
            oldName = dataGridView1.Rows[rowCount].Cells[3].Value.ToString();
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                //当地域信息相同
                if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name.Split('@')[0] == oldName)
                {
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    FileMesege.PointList.equipment.Remove(eq);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    dgvPointAddItemBySection();
                    break;
                }
            }
            

        }
        
        /// <summary>
        /// 并联节点
        /// </summary>
        private void Multiple(int rowNumber)
        {
            
            //区域
            List<string> section = dataGridView1.Rows[rowNumber].Cells[2].Value.ToString().Split(' ').ToList();
            while (section.Count != 4)
            {
                section.Add("");
            }
            oldName = dataGridView1.Rows[rowNumber].Cells[3].Value.ToString();
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                //当地域信息相同
                if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name.Split('@')[0] == oldName)
                { 
                    //合并数据列表添加
                    multipleList.Add(eq);
                    if (!(bool)dataGridView1.Rows[rowNumber].Cells[7].EditedFormattedValue)
                    {
                        return;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 手动双击编辑名称
        /// </summary>
        /// <param name="rowNum">行号</param>
        private void editName(int rowNum)
        {
            //关闭可编辑
            dataGridView1.Columns[3].ReadOnly = true;
            //确保修改后的名字不会为空
            if (dataGridView1.Rows[rowNum].Cells[3].Value == null)
            {
                dataGridView1.Rows[rowNum].Cells[3].Value = oldName;
                return;
            }
            string nowName = dataGridView1.Rows[rowNum].Cells[3].Value.ToString().Trim();
            if (string.IsNullOrEmpty(nowName))
            {
                dataGridView1.Rows[rowNum].Cells[3].Value = oldName;
                return;
            }

            //区域
            List<string> section = dataGridView1.Rows[rowNum].Cells[2].Value.ToString().Split(' ').ToList();
            while (section.Count != 4)
            {
                section.Add("");
            }

            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                //当地域信息相同
                if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name.Split('@')[0] == oldName)
                {
                    //剔除名字相同 
                    foreach (DataJson.PointInfo et in FileMesege.PointList.equipment)
                    {
                        if (et.area1 == eq.area1 && et.area2 == eq.area2 && et.area3 == eq.area3 && et.area4 == eq.area4 && et.name.Split('@')[0] == nowName)
                        {

                            dataGridView1.Rows[rowNum].Cells[3].Value = oldName;
                            return;

                        }
                    }
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    eq.name = string.Format("{0}@{1}", nowName, eq.name.Split('@')[1]);
                    dataGridView1.Rows[rowNum].Cells[3].Value = nowName;
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    break;
                }
            }
        }

        /// <summary>
        /// 寻找当前选中行信息
        /// </summary>
        /// <returns>返回pointList点 否则返回null</returns>
        private DataJson.PointInfo findNowRow(int rowNum)
        {
            //区域
            List<string> section = dataGridView1.Rows[rowNum].Cells[2].Value.ToString().Split(' ').ToList();
            while (section.Count != 4)
            {
                section.Add("");
            }
            oldName = dataGridView1.Rows[rowNum].Cells[3].Value.ToString();
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                //当地域信息相同
                if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name.Split('@')[0] == oldName)
                {
                    return eq;
                }
            }

            return null;
        }

        bool isClick = false;
        //点击
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //treesection临时存放数据处
                FileMesege.sectionNode = null;
                //treetitle名字临时存放
                FileMesege.titleinfo = "";
                updateTitleNode();
                
            }
            DgvMesege.endDataViewCurrent(dataGridView1, e.Y,e.X);
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

        #endregion

        #region 复制 粘贴
        /// <summary>
        /// 复制点位的对象 与参数 
        /// </summary>
        public void copyData()
        {
            //获取当前选中单元格的列序号
            int colIndex = dataGridView1.CurrentRow.Cells.IndexOf(dataGridView1.CurrentCell);
         
            if (colIndex == 4 || colIndex == 5)
            {
                
                //区域
                List<string> section = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString().Split(' ').ToList();
                while (section.Count != 4)
                {
                    section.Add("");
                }
                foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                {
                    //当地域信息相同
                    if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name.Split('@')[0] == dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value.ToString())
                    {
                        FileMesege.copyPoint = eq;
                        break;
                    }
                }
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
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    //当粘贴选中单元格为对象和参数
                    if (colIndex == 4 || colIndex == 5)
                    {

                        //区域
                        List<string> section = dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[2].Value.ToString().Split(' ').ToList();
                        while (section.Count != 4)
                        {
                            section.Add("");
                        }
                        foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                        {
                            //当地域信息相同
                            if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name.Split('@')[0] == dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[3].Value.ToString())
                            {
                                
                                if (eq.objType == FileMesege.copyPoint.objType && eq.value == FileMesege.copyPoint.value)
                                {
                                    break;
                                }
                                eq.objType = FileMesege.copyPoint.objType;
                                eq.value = FileMesege.copyPoint.value;
                                eq.type = FileMesege.copyPoint.type;
                                //解绑IP地址
                                eq.address = "FFFFFFFF";
                                eq.ip = "";
                                ischange = true;
                                //获取中文类型名
                                string objTypeName = IniHelper.findObjsDefineName_ByType(eq.objType);
                                //类型 objType
                                this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[4].Value = objTypeName;
                                if (!string.IsNullOrEmpty(objTypeName))
                                {
                                    updataValueItem(dataGridView1.SelectedCells[i].RowIndex, objTypeName);
                                    //参数 value
                                    this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = IniHelper.findObjValueName_ByVal(objTypeName, eq.value);
                                    this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].ReadOnly = true;
                                }
                                else
                                {
                                    this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = "";
                                }
                                
                                break;
                            }
                        }
                    }//if
                }//for
                if (ischange)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
               

            }//try
            catch {

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
                bool ischange = false;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //获取当前选中单元格的列序号
                    int colIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                    //当粘贴选中单元格为对象和参数
                  
                    //区域
                    List<string> section = dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[2].Value.ToString().Split(' ').ToList();
                    while (section.Count != 4)
                    {
                        section.Add("");
                    }
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                    {
                        //当地域信息相同
                        if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name.Split('@')[0] == dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[3].Value.ToString())
                        {
                            if (colIndex == 4 )
                            {
                                eq.objType = "";
                                eq.value = "";
                                ischange = true;
                                this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[4].Value = null;
                                this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = null;
                                
                            }
                            if (colIndex == 5)
                            {
                                eq.value = "";
                                ischange = true;
                                this.dataGridView1.Rows[dataGridView1.SelectedCells[i].RowIndex].Cells[5].Value = null;
                                

                            }
                            break; 
                           
                        }
                    }
                    
                }//for
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
