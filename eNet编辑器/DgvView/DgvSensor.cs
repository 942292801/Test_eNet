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

namespace eNet编辑器.DgvView
{
    public partial class DgvSensor : Form
    {
        public DgvSensor()
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

        DataGridViewComboBoxColumn objType;


        private void DgvSensor_Load(object sender, EventArgs e)
        {
            //新增对象列 加载
            objType = new DataGridViewComboBoxColumn();
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

            //设置列名
            objType.HeaderText = "类型";
            //设置下拉列表的默认值 
            objType.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //或者这样设置 默认选择第一项
            objType.DefaultCellStyle.NullValue = objType.Items[0];
            objType.ReadOnly = true;
            objType.Name = "objType";   

            //插入执行对象类型
            this.dataGridView1.Columns.Insert(3, objType);
        }

        private string tmpIONum = "";
        /// <summary>
        /// 加载DgV所有信息
        /// </summary>
        public void dgvSensorAddItem()
        {
            try
            {
                cbDevNum.Items.Clear();
                cbDevNum.Text = "";
                int tmpId = -1;
                DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
                if (srs == null)
                {
                    cbIONum.Text = "";
                    this.dataGridView1.Rows.Clear();
                    return;
                }
                if (srs.sensorsInfo.Count == 0)
                {
                    //新建面板 不存在信息则让 cbIoNumCHnageText事件加载
                    tmpIONum = "";
                    cbIONum.Text = srs.ioNum.ToString();
                    return;
                }
                tmpIONum = srs.ioNum.ToString();
                cbIONum.Text = tmpIONum;

                //防止二次刷新
                this.dataGridView1.Rows.Clear();
                findKeyPanel();
                List<DataJson.sensorsInfo> delSensor = new List<DataJson.sensorsInfo>();
                string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sensorSelectNode));//16进制
                //循环加载该定时号的所有信息
                foreach (DataJson.sensorsInfo srInfo in srs.sensorsInfo)
                {
                    //新增行号
                    int dex = dataGridView1.Rows.Add();
                    if (srInfo.pid == 0)
                    {
                        //pid号为0则为空 按地址来找
                        if (srInfo.objAddress != "" && srInfo.objAddress != "FFFFFFFF")
                        {
                            DataJson.PointInfo point = DataListHelper.findPointByType_address(srInfo.objType, ip4 + srInfo.objAddress.Substring(2, 6));
                            if (point != null)
                            {
                                srInfo.pid = point.pid;
                                srInfo.objAddress = point.address;
                                srInfo.objType = point.type;
                                dataGridView1.Rows[dex].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                dataGridView1.Rows[dex].Cells[5].Value = point.name;
                            }
                        }
                    }
                    else
                    {
                        //pid号有效 需要更新address type
                        DataJson.PointInfo point = DataListHelper.findPointByPid(srInfo.pid);
                        if (point == null)
                        {
                            //pid号有无效 删除该感应
                            delSensor.Add(srInfo);
                            dataGridView1.Rows.Remove(dataGridView1.Rows[dex]);
                            continue;
                        }
                        else
                        {
                            //pid号有效
                            srInfo.objAddress = point.address;
                            //////////////////////////////////////////////////////争议地域
                            //类型不一致 在value寻找
                            if (srInfo.objType != point.type && !string.IsNullOrEmpty(point.value) && !string.IsNullOrEmpty(point.objType))
                            {
                                //根据value寻找type                        
                                point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, point.value);
                            }
                            //////////////////////////////////////////////////////到这里
                            if (srInfo.objType != point.type || srInfo.objType == "")
                            {
                                //当类型为空时候清空操作
                                srInfo.opt = "";
                                srInfo.optName = "";
                            }
                            srInfo.objType = point.type;
                            dataGridView1.Rows[dex].Cells[4].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[dex].Cells[5].Value = point.name;
                        }

                    }
                    if (tmpId == srInfo.id)
                    {
                        //同上的
                        dataGridView1.Rows[dex].Cells[0].Value = null;
                        dataGridView1.Rows[dex].Cells[8].ReadOnly = true;
                    }
                    else
                    {
                        dataGridView1.Rows[dex].Cells[0].Value = srInfo.id;
                        dataGridView1.Rows[dex].Cells[8].Value = "添加";
                    }
                    tmpId = srInfo.id;

                    dataGridView1.Rows[dex].Cells[1].Value = keyAddressTransform(srInfo.keyAddress);
                    dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(srInfo.objAddress);
                    dataGridView1.Rows[dex].Cells[3].Value = IniHelper.findTypesIniNamebyType(srInfo.objType);
                    // (操作)
                    dataGridView1.Rows[dex].Cells[6].Value = (srInfo.optName + " " + srInfo.opt).Trim();
                 
                    dataGridView1.Rows[dex].Cells[7].Value = "删除";



                }
                for (int i = 0; i < delSensor.Count; i++)
                {
                    srs.sensorsInfo.Remove(delSensor[i]);
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
        /// 寻找有io的panel
        /// </summary>
        private void findKeyPanel()
        {

            //devices 里面ini的名字
            string keyVal = "";
            string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
            string path = Application.StartupPath + "\\devices\\";
            //从设备加载网关信息
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                if (d.ip == ip)
                {
                    //加载设备
                    foreach (DataJson.Module m in d.module)
                    {
                        keyVal = IniConfig.GetValue(path + m.device + ".ini", "input", "io");
                        if (keyVal != "null")
                        {
                            cbDevNum.Items.Add(m.id);

                        }
                    }
                }

            }
        }

        /// <summary>
        /// bindinfo信息按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void sensorInfoSort(DataJson.sensors srs)
        {
            srs.sensorsInfo.Sort(delegate(DataJson.sensorsInfo x, DataJson.sensorsInfo y)
            {

                return x.id.CompareTo(y.id);
            });

        }
        /// <summary>
        /// 地址转换
        /// </summary>
        /// <returns></returns>
        public static string keyAddressTransform(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return "";
            }
            //string ip = Convert.ToInt32(address.Substring(0, 2), 16).ToString();
            string ID = Convert.ToInt32(address.Substring(2, 2), 16).ToString();
            string num = (Convert.ToInt32(address.Substring(4, 2), 16) * 256 + Convert.ToInt32(address.Substring(6, 2), 16)).ToString();

            return string.Format("{0}.{1}", ID, num);
        }

        #endregion

        #region IO键选择
        private void cbIoNum_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbIONum.Text))
            {
                return;
            }
            if (tmpIONum == cbIONum.Text)
            {
                return;
            }
            if (setKeyNum(Convert.ToInt32(cbIONum.Text)))
            {
                dgvSensorAddItem();
            }
        }

        /// <summary>
        /// 设置当前面板为 几键面板 成功返回true
        /// </summary>
        /// <param name="keyNum"></param>
        /// <returns></returns>
        private bool setKeyNum(int IONum)
        {
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return false;
            }

            //撤销 
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            srs.ioNum = IONum;
            List<DataJson.sensorsInfo> delSensor = new List<DataJson.sensorsInfo>();
            HashSet<int> numList = new HashSet<int>();
            foreach (DataJson.sensorsInfo srInfo in srs.sensorsInfo)
            {
                if (srInfo.id > IONum)
                {
                    delSensor.Add(srInfo);
                }
                numList.Add(srInfo.id);
            }
            for (int i = 0; i < delSensor.Count; i++)
            {
                //删除id大于面板按键值
                srs.sensorsInfo.Remove(delSensor[i]);
            }
            List<int> list = numList.ToList<int>();
            bool isExit = false;
            for (int i = 1; i <= IONum; i++)
            {
                isExit = false;
                for (int j = 0; j < list.Count; j++)
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
                    DataJson.sensorsInfo srInfo = new DataJson.sensorsInfo();
                    srInfo.id = i;
                    srInfo.pid = 0;
                    srInfo.keyAddress = "";
                    srInfo.objAddress = "";
                    srInfo.objType = "";
                    srInfo.opt = "";
                    srInfo.optName = "";

                    srs.sensorsInfo.Add(srInfo);
                    //排序
                    sensorInfoSort(srs);
                }
            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return true;
        }

        private void cbDevNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDevNum.Text == "")
            {
                return;
            }
            DataJson.sensors srs = DataListHelper.getSensorInfoListByNode();
            if (srs == null)
            {
                return;
            }
            string ip = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sensorSelectNode));
            string id = SocketUtil.strtohexstr(cbDevNum.Text);
            string tmpNum = "";
            foreach (DataJson.sensorsInfo srInfo in srs.sensorsInfo)
            {
                //非设备类
                tmpNum = SocketUtil.strtohexstr(srInfo.id.ToString());
                while (tmpNum.Length < 4)
                {
                    tmpNum = tmpNum.Insert(0, "0");
                }
                srInfo.keyAddress = string.Format("{0}{1}{2}", ip, id, tmpNum);
            }
            dgvSensorAddItem();
        }
        #endregion


        #region 增加 清空  清除 设置 按键
        private void btnDown_Click(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        private void btnOn_Click(object sender, EventArgs e)
        {

        }

        private void btnOff_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }

        private void btnDel_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region 表格操作

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }
        #endregion

        #region del删除快捷键
        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {

        }
        #endregion




    }
}
