using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using eNet编辑器.Properties;

namespace eNet编辑器.ThreeView
{
    public partial class ThreePoint : Form
    {
        public ThreePoint()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
        
        }

        public event Action updateDgvPoint;
        public event Action<string> AppTxtShow;
        //树状图节点
        string fullpath = "";


        private void ThreePoint_Load(object sender, EventArgs e)
        {

        }

        public void ThreePointAddNode()
        {
            if (FileMesege.DeviceList == null)
            {
                treeView1.Nodes.Clear();
                return;
            }

            TreeMesege tm = new TreeMesege();
            List<string> isExpands = tm.treeIsExpandsState(treeView1);
            //记录当前节点展开状况
            int index = 0;
            int index2 = 0;
            int index3 = 0;
            string filepath = "";
            string device = "";
            string gwdevice = "";
            string portVal = "";
            string address = "";
            string section = "";
            foreach (DataJson.Device master in FileMesege.DeviceList)
            {
                //添加网关
                filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, master.master);
                gwdevice = IniConfig.GetValue(filepath, "define", "display");
                index = tm.AddNode1(treeView1, master.ip + " " + gwdevice);
                foreach (DataJson.Module m in master.module)
                {
                    filepath = filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, m.device);
                    device = IniConfig.GetValue(filepath, "define", "display");
                    if (string.IsNullOrEmpty(device))
                    {
                        //未识别设备
                        //判断加英文         Resources.treeName为资源文件的KEY值
                        index2 = tm.AddNode2(treeView1, Resources.UnrecognizedDev + m.id + " " + m.device, index);
                    }
                    else
                    {
                        //判断加英文         Resources.treeName为资源文件的KEY值
                        index2 = tm.AddNode2(treeView1, Resources.Device + m.id + " " + device, index);

                        #region 端口添加区域和名称
                        //读取端口数量
                        List<string> list = IniConfig.ReadKeys("ports", filepath);
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i] == "0")
                            {
                                continue;
                            }
                            //获取类型版本类型版本
                            portVal = IniConfig.GetValue(filepath, "ports", list[i]);
                            if (string.IsNullOrEmpty(portVal))
                            {
                                break;
                            }
                            address = "FE00" + m.id.ToString("X2") + Convert.ToInt32(list[i]).ToString("X2");
                            DataJson.PointInfo point = DataListHelper.findPointByType_address(portVal.Split(',')[0], address, master.ip);
                            if (point == null)
                            {
                                index3 = tm.AddNode3(treeView1, Resources.Port + list[i], index, index2);
                            }
                            else
                            {
                                section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                index3 = tm.AddNode3(treeView1,string.Format("{0}{1} {2} {3}", Resources.Port, list[i],section,point.name), index, index2);
                            }
                        }

                    
                        #endregion
                    }
                }
            }

            //展开记录的节点
            tm.treeIspandsStateRcv(treeView1, isExpands);
            TreeMesege.SetPrevVisitNode(treeView1, fullpath);
        }

        #region 点击树状图 树状图重绘
        /// <summary>
        /// 高亮显示选中项 重绘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Color foreColor;
            Color backColor;
            if ((e.State & TreeNodeStates.Selected) > 0)
            {
                foreColor = Color.Black;//鼠标点击节点时文字颜色
                backColor = Color.FromArgb(204, 235, 248);//鼠标点击节点时背景颜色
            }
            else if ((e.State & TreeNodeStates.Hot) > 0)
            {
                foreColor = Color.Lime;//鼠标经过时文字颜色
                backColor = Color.Gray;//鼠标经过时背景颜色
            }
            else
            {
                foreColor = this.treeView1.ForeColor;
                backColor = this.treeView1.BackColor;
            }
            //e.Graphics.FillRectangle(new SolidBrush(backColor), new Rectangle(e.Bounds.Location, new Size(this.treeView1.Width - e.Bounds.X, e.Bounds.Height)));
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
            e.Graphics.DrawString(e.Node.Text, this.treeView1.Font, new SolidBrush(foreColor), e.Bounds.X, e.Bounds.Y + 4);

        }

        /// <summary>
        /// 选中复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode node in e.Node.Nodes)
            {
                node.Checked = e.Node.Checked;
            }
        }



        #endregion

        #region 按钮 ：添加或删除  点位

        private void BtnAddPoint_Click(object sender, EventArgs e)
        {
            AddPoint();
        }

        private void BtnDelPoint_Click(object sender, EventArgs e)
        {
            DelPoint();
        }

        /// <summary>
        /// 添加点位
        /// </summary>
        private void AddPoint()
        {
            try
            {
                if (FileMesege.sectionNode == null)
                {
                    AppTxtShow("请选择区域");
                    return;
                }
                if (FileMesege.sectionNode.FullPath.Contains("查看所有区域"))
                {
                    AppTxtShow("选择区域不能为：查看所有区域");
                    return;
                }
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();

                //位置
                TreeMesege tm = new TreeMesege();
                string[] tmID = tm.GetSectionId(FileMesege.sectionNode.FullPath.Split('\\'));
                string[] sections = tm.GetSection(tmID[0], tmID[1], tmID[2], tmID[3]).Split('\\');
                string ip = "";
                string id = "";
                string port = "";
                string address = "";
                string filepath = "";
                string type = "";
                string[] portNodeTxt ;
                string title = "";
                string titleNum = "";
                string tmpName = "";
                bool isExit = false;
                //循环获取选中的节点
                foreach (TreeNode MasterNode in treeView1.Nodes)
                {
                    ip = MasterNode.Text.Split(' ')[0];
                    foreach (TreeNode DevNode in MasterNode.Nodes)
                    {
                        foreach (TreeNode PortNode in DevNode.Nodes)
                        {
                            //非选中节点退出
                            if (!PortNode.Checked)
                            {
                                continue;
                            }
                            portNodeTxt = PortNode.Text.Split(' ');
                            if (portNodeTxt.Length == 3)
                            {
                                if (portNodeTxt[1].Contains(FileMesege.sectionNode.FullPath)
                                && portNodeTxt[2].Contains(FileMesege.titleinfo)
                                && portNodeTxt[2].Substring(0, FileMesege.titleinfo.Length) == FileMesege.titleinfo)
                                {
                                    //遇到同一个区域和同一个名称 则不处理
                                    continue;
                                }
                            }
                            filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, DevNode.Text.Split(' ')[1].Trim());
                            port = Regex.Replace(PortNode.Text.Split(' ')[0], @"[^\d]*", "");
                            type = IniConfig.GetValue(filepath, "ports", port);
                            if (string.IsNullOrEmpty(type))
                            {
                                continue;
                            }
                            type = type.Trim().Split(',')[0];
                            id = Regex.Replace(DevNode.Text.Split(' ')[0], @"[^\d]*", "");
                            address = "FE00" + Convert.ToInt32(id).ToString("X2") + Convert.ToInt32(port).ToString("X2");
                            if (address.Length != 8)
                            {
                                continue;
                            }

                            //新建点位
                            //DataListHelper.newPoint(address, ip, sections, type, FileMesege.PointList.equipment);

                          



                            //添加 点位名称
                            title = FileMesege.titleinfo;
                            //名称栏为空 或者区域栏 返回空值 
                            if (string.IsNullOrEmpty(title))
                            {
                                title = "未定义";
                            }
                            else
                            {
                                //把@去掉
                                title = title.Split('@')[0];
                            }
                            //纯文字title
                            title = Regex.Replace(title, @"[\d]$", "");
                            HashSet<int> hasharry = new HashSet<int>();
                            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                            {
                                //当地域信息相同
                                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3])
                                {
                                    if (eq.name != null && eq.name.Substring(0, title.Length) == title)
                                    {
                                        //获取序号
                                        titleNum = eq.name.Split('@')[0].Replace(title, "");
                                        if (!string.IsNullOrEmpty(titleNum))
                                        {
                                            hasharry.Add(Convert.ToInt32(titleNum));

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
                                title = title + "1";
                            }
                            else
                            {
                                isExit = false;
                                //哈希表 不存在序号 直接返回
                                for (int i = 0; i < arry.Count; i++)
                                {
                                    if (arry[i] != i + 1)
                                    {
                                        title = title + (i + 1).ToString();
                                        isExit = true;
                                        break;
                                    }
                                }
                                if (!isExit)
                                {
                                    title = title + (arry[arry.Count - 1] + 1).ToString();

                                }
                            }
                            tmpName = string.Format("{0}@{1}", title, ip.Split('.')[3]);

                            /*foreach (DataJson.PointInfo e in FileMesege.PointList.equipment)
                            {
                                if (address == e.address && e.ip == ip)
                                {
                                    if (string.IsNullOrWhiteSpace(e.name) || e.name != tmpName)
                                    {
                                        e.name = tmpName;
                                    }
                                    else if (e.name.Contains("未定义"))
                                    {
                                        e.name = tmpName;
                                    }
                                    else
                                    {
                                        //非空有内容
                                    }
                                    break;
                                }
                            }*/
                            isExit = false;
                            foreach (DataJson.PointInfo e in FileMesege.PointList.equipment)
                            {
                                //循环判断 NameList中是否存在该节点
                                if (address == e.address && e.ip == ip)
                                {

                                    e.area1 = sections[0];
                                    e.area2 = sections[1];
                                    e.area3 = sections[2];
                                    e.area4 = sections[3];
                                    e.name = tmpName;
                                    //存在
                                    //更新NameList里面的类型信息
                                    e.type = type;
                                    isExit = true;
                                    break;

                                }

                            }
                            if (!isExit)
                            {
                                //不存在 插入新信息
                                DataJson.PointInfo eq = new DataJson.PointInfo();
                                eq.pid = DataChange.randomNum();
                                eq.area1 = sections[0];
                                eq.area2 = sections[1];
                                eq.area3 = sections[2];
                                eq.area4 = sections[3];
                                //eq.name = "";
                                eq.ip = ip;
                                eq.address = address;
                                eq.objType = "";
                                eq.value = "";
                                eq.type = type;
                                eq.name = tmpName;
                                FileMesege.PointList.equipment.Add(eq);
                            }
                           


                        }

                    }
                }
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                ThreePointAddNode();
                updateDgvPoint();
            }//try
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }

        }

        private void DelPoint()
        {
            try
            {
                //string ip = "";
                string[] portNodeTxt;
                List<string> sections = new List<string>(); 
                List<DataJson.PointInfo> pointInfos = new List<DataJson.PointInfo>();
                foreach (TreeNode MasterNode in treeView1.Nodes)
                {
                    //ip = MasterNode.Text.Split(' ')[0];
                    foreach (TreeNode DevNode in MasterNode.Nodes)
                    {
                        foreach (TreeNode PortNode in DevNode.Nodes)
                        {
                            //非选中节点退出
                            if (!PortNode.Checked)
                            {
                                continue;
                            }
                            portNodeTxt = PortNode.Text.Split(' ');
                            if (portNodeTxt.Length != 3)
                            {
                                continue;
                            }
                            sections.Clear();
                            sections = portNodeTxt[1].Split('\\').ToList();
                            while (sections.Count != 4)
                            {
                                sections.Add("");
                            }
                            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                            {
                               
                                //当地域信息相同
                                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3] && eq.name == portNodeTxt[2])
                                {
                                    pointInfos.Add(eq);
                                    break;
                                }
                            }

                        }
                    }
                }
                if (pointInfos.Count > 0)
                {
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    for (int i = 0; i < pointInfos.Count; i++)
                    {
                        FileMesege.PointList.equipment.Remove(pointInfos[i]);
                    }
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    ThreePointAddNode();
                    updateDgvPoint();
                }

            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }

           


        }


        #endregion


        #region 菜单栏： 
        private void 添加点位ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPoint();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DelPoint();
        }

        private void 展开所有节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void 收起所有节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        #endregion

    }
}
