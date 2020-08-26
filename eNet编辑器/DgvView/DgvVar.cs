using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;
using eNet编辑器.AddForm;
using System.Threading;

namespace eNet编辑器.DgvView
{
    public partial class DgvVar : Form
    {
        public DgvVar()
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


        //临时存放旧的Name名称
        private string oldName = "";
        string ip = "";

        HashSet<DataJson.PointInfo> multipleList = new HashSet<DataJson.PointInfo>();

        private void DgvVar_Load(object sender, EventArgs e)
        {

        }


        #region 刷新dgv框相关操作

        public void dgvVarAddItem()
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
            catch
            {
            }

        }


        #endregion

         /// <summary>
        /// 加载DgV所有信息
        /// </summary>
        public void TabIni()
        {
            try
            {
                //清空并联点
                multipleList.Clear();
                this.dataGridView1.Rows.Clear();
                if(FileMesege.varSelectNode == null )
                {
                    return;
                }
                
                ip = FileMesege.varSelectNode.Text.Split(' ')[0];
                
                foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
                {
                    //加载该IP的虚拟端口
                    if (eq.ip == ip)
                    {
                        CountAddInfo(eq);
                    }

                }
                DgvMesege.RecoverDgvForm(dataGridView1, X_Value, Y_Value, rowCount, columnCount);
            }
            catch
            {
                this.dataGridView1.Rows.Clear();
                //MessageBox.Show(e.Message + "\r\n后期可以屏蔽该错误");
                //报错不作处理
            }
        }

        /// <summary>
        /// 添加每一行的信息
        /// </summary>
        /// <param name="eq"></param>
        public void CountAddInfo(DataJson.PointInfo eq)
        {
            //添加新的一行 rowNum为行号
            int rowNum = this.dataGridView1.Rows.Add();
            //序号
            this.dataGridView1.Rows[rowNum].Cells[0].Value = (rowNum + 1);
            //地址
            this.dataGridView1.Rows[rowNum].Cells[1].Value = DgvMesege.addressTransform(eq.address);
            //区域
            this.dataGridView1.Rows[rowNum].Cells[2].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();
            //名字
            this.dataGridView1.Rows[rowNum].Cells[3].Value = eq.name.Split('@')[0];
            //删除
            this.dataGridView1.Rows[rowNum].Cells[4].Value = "删除";
        }

        public void clearDgvClear()
        {
            dataGridView1.Rows.Clear();
        }
        #endregion

        #region 增加 清空 删除
        /// <summary>
        /// 增加按钮 新增一条虚拟端口信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (FileMesege.varSelectNode == null)
            {
                return;
            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            FileMesege.titleinfo = "虚拟端口";
            addVirtualport("虚拟端口");
            //按照address最后一位重新排序
            
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            
            selectLastCount();
        }

        public void selectLastCount()
        {
            DgvMesege.selectLastCount(dataGridView1);
        }

        /// <summary>
        /// 添加虚拟端口
        /// </summary>
        /// <param name="typeName"></param>
        public void addVirtualport(string typeName)
        {
            
            if (string.IsNullOrEmpty(FileMesege.titleinfo))
            {
                //AppTxtShow("请选择名称");
                //return;
                FileMesege.titleinfo = "虚拟端口";
            }
            //搜索选中区域  加载所有同区域的节点
            //区域
            string[] sect = null;
            if (string.IsNullOrEmpty(FileMesege.sectionNodeCopy))
            {
                sect = new string[] { "", "", "", "" };
            }
            else
            {
                sect = FileMesege.sectionNodeCopy.Split('\\');
            }
            if (sect[0] == "查看所有区域")
            {
                sect = new string[] { "", "", "", "" };
            }
            //计算name的排序
            DataJson.PointInfo point = new DataJson.PointInfo();
            point.pid = DataChange.randomNum();
            string name = sortName(ip);
            point.name = string.Format("{0}@{1}", sortName(ip), ip.Split('.')[3]);
            string nameID = Regex.Replace(name, @"[^\d]*", "");
            if (Convert.ToInt32(nameID) > 255)
            {
                return;
            }
            point.address = "FEFB03"+  ToolsUtil.strtohexstr(nameID);
            point.area1 = sect[0];
            point.area2 = sect[1];
            point.area3 = sect[2];
            point.area4 = sect[3];
            point.ip = ip;

            point.objType = "";//IniHelper.findObjsFileNae_ByName(typeName);
            point.type = IniHelper.findTypesIniTypebyName(typeName);
            point.value = "";


            FileMesege.PointList.virtualport.Add(point);
            CountAddInfo(point);
           
        }

        /// <summary>
        /// 名称自动添加序号 排序
        /// </summary>
        /// <param name="section">位置</param>
        /// <returns></returns>
        private string sortName(string ip)
        {
            HashSet<int> hasharry = new HashSet<int>();
            //判断当前是否有匹配
            //计算有多小个相同区域的信息
            //循环所有信息
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                if (eq.ip == ip)
                {
                    //获取序号
                    hasharry.Add(Convert.ToInt32(eq.address.Substring(6,2),16));
                    
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
        /// 排序point.virtualport的信息
        /// </summary>
        /// <param name="ip"></param>
        private void virtualportSort()
        {
            FileMesege.PointList.virtualport.Sort(delegate(DataJson.PointInfo x, DataJson.PointInfo y)
            {
                
                    return (Convert.ToInt32(x.address.Substring(6, 2), 16)).CompareTo(Convert.ToInt32(y.address.Substring(6, 2), 16));
             
               
            });
       
        }

        //清空
        private void btnClear_Click(object sender, EventArgs e)
        {

            try
            {
                if (FileMesege.varSelectNode == null )
                {
                    return;
                }
                //选中子节点
                foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
                {
                    if (eq.ip == ip)
                    {
                        //获取序号
                        multipleList.Add(eq);
                    }

                }

                //撤销 
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                foreach (DataJson.PointInfo point in multipleList)
                {
                    FileMesege.PointList.virtualport.Remove(point);
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                this.dataGridView1.Rows.Clear();
                multipleList.Clear();
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }

        //重新排序  刷新
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            virtualportSort();
            dgvVarAddItem();
        }

        #endregion



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

            if (isClick)
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
                            case "address":
                                //改变地址
                                string address = "";
                                if (dataGridView1.Rows[rowCount].Cells[1].Value != null)
                                {
                                    //原地址
                                    address = dataGridView1.Rows[rowCount].Cells[1].Value.ToString();
                                }
                                //赋值List 并添加地域 名字
                                dgvAddress(address);
                                break;
                            case "name":
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
                            case "del":
                                //删除表
                                dgvDel();
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
                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            case "del":
                                //删除表
                                dgvDel();

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

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

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
                        case "name":
                            //手动双击编辑名称
                            editName(rowNum);

                            break;

                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex + "临时调试错误信息"); }
        }


       /// <summary>
        /// 获取新的地址 刷新地域
       /// </summary>
       /// <param name="address"></param>
        private void dgvAddress(string address)
        {
            virtualportAddress varAdd = new virtualportAddress();
            //把窗口向屏幕中间刷新
            varAdd.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用  
            varAdd.Address = address;
            varAdd.ShowDialog();
            if (varAdd.DialogResult == DialogResult.OK)
            {
                //选中子节点
                oldName = dataGridView1.Rows[rowCount].Cells[3].Value.ToString();
                string tmpName = string.Format("{0}@{1}", oldName, ip.Split('.')[3]);
                //区域
                List<string> section = dataGridView1.Rows[rowCount].Cells[2].Value.ToString().Split(' ').ToList();
                while (section.Count != 4)
                {
                    section.Add("");
                }
                foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
                {
                    if (eq.ip == ip && eq.address == varAdd.RtAddress)
                    {
                        //如果该地址已经存在 则返回
                        return;
                    }
                }
                foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
                {
                    //当地域信息相同
                    if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name == tmpName)
                    {
                        //撤销 
                        DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                        eq.address = varAdd.RtAddress;
                        if(eq.name.Contains("虚拟端口"))
                        {
                            int num= Convert.ToInt32(varAdd.RtAddress.Substring(6,2),16);
                            eq.name = string.Format("虚拟端口{0}@{1}", num, ip.Split('.')[3]);
                        }
                        DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                        FileMesege.cmds.DoNewCommand(NewList, OldList);
                        dgvVarAddItem();
                        
                        break;
                    }
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
   
             //选中子节点
            string tmpName = string.Format("{0}@{1}", oldName, ip.Split('.')[3]);
            string tmpNowName = string.Format("{0}@{1}", nowName, ip.Split('.')[3]);
            //区域
            List<string> section = dataGridView1.Rows[rowNum].Cells[2].Value.ToString().Split(' ').ToList();
            while (section.Count != 4)
            {
                section.Add("");
            }

            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                //当地域信息相同
                if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name == tmpName)
                {
                    //剔除名字相同 
                    foreach (DataJson.PointInfo et in FileMesege.PointList.virtualport)
                    {
                        if (et.area1 == eq.area1 && et.area2 == eq.area2 && et.area3 == eq.area3 && et.area4 == eq.area4 && et.name == tmpNowName)
                        {
                            dataGridView1.Rows[rowNum].Cells[3].Value = oldName;
                            return;

                        }
                    }
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    eq.name = tmpNowName;
                    dataGridView1.Rows[rowNum].Cells[3].Value = nowName;
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    break;
                }
            }
        }

        private void dgvDel()
        {
            //区域
            List<string> section = dataGridView1.Rows[rowCount].Cells[2].Value.ToString().Split(' ').ToList();
            while (section.Count != 4)
            {
                section.Add("");
            }
            oldName = dataGridView1.Rows[rowCount].Cells[3].Value.ToString();
            //选中子节点
            string tmpName = string.Format("{0}@{1}", oldName, ip.Split('.')[3]);
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                //当地域信息相同
                if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3] && eq.name == tmpName)
                {
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    FileMesege.PointList.virtualport.Remove(eq);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    dgvVarAddItem();
                    break;
                }
            }
            
        }

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

       


    }
}
