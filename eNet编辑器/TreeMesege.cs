using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace eNet编辑器
{
    class TreeMesege
    {
        /// <summary>
        /// 第一级树状图添加 
        /// </summary>
        /// <param name="treeView1">树状图控件</param>
        /// <param name="name">树状图名字</param>
        /// <returns>当前节点 索引号</returns>
        public  int AddNode1(TreeView treeView1, string name)
        {
            try
            {
                TreeNode tn = new TreeNode();
                //添加根节点
                tn.Text = name;
                tn.ImageIndex = 0;
                tn.SelectedImageIndex = 2;
                tn.Expand();
                return treeView1.Nodes.Add((TreeNode)(tn.Clone()));
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
                return 0;
            }

           
          
        }

        public  int AddNode2(TreeView treeView1, string name, int index)
        {
            try
            {
                TreeNode tn = new TreeNode();
                tn.Text = name;
                tn.ImageIndex = 1;
                tn.SelectedImageIndex = 3;
                tn.Expand();
                return treeView1.Nodes[index].Nodes.Add((TreeNode)(tn.Clone()));
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
                return 0;
            }
            
        }
        public  int AddNode3(TreeView treeView1, string name, int index, int index2)
        {
            try
            {
                TreeNode tn = new TreeNode();
                tn.Text = name;
                tn.ImageIndex = 4;
                tn.SelectedImageIndex = 5;
                tn.Expand();
                return treeView1.Nodes[index].Nodes[index2].Nodes.Add((TreeNode)(tn.Clone()));
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
                return 0;
            }
           
        }

        public  int AddNode4(TreeView treeView1, string name, int index, int index2, int index3)
        {
            try
            {
                TreeNode tn = new TreeNode();
                tn.Text = name;
                tn.ImageIndex = 4;
                tn.SelectedImageIndex = 5;
                tn.Expand();
                return treeView1.Nodes[index].Nodes[index2].Nodes[index3].Nodes.Add((TreeNode)(tn.Clone()));
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
                return 0;
            }
            
        }

        /// <summary>
        /// 获取树状图索引号即==>0 0 0 0
        /// </summary>
        /// <param name="tn">当前选中节点</param>
        /// <returns></returns>
        public  string GetNodeNum(TreeNode tn)
        {
            string a = "";
            while (tn != null)
            {
                a = " " + tn.Index.ToString() + a;
                tn = tn.Parent;

            }
            return a.Trim();
        }

        /// <summary>
        /// 把树状图索引号 添加为四位 ==>0 0 “” “”
        /// </summary>
        /// <param name="a">树状图索引号</param>
        /// <returns></returns>
        public string[] NodeNumAdd(string a)
        {
            string [] strs =  a.Split(' ');
            switch (strs.Length)
            {
             
                case 1:
                    strs = new string[4] { strs[0], "", "", "" };
                    break;
                case 2:
                    strs = new string[4] { strs[0], strs[1], "", "" };
                    break;
                case 3:
                    strs = new string[4] { strs[0], strs[1], strs[2], "" };
                    break;
                default:
                    break;
            }
            return strs;
        }


        /// <summary>
        /// 获取区域信息内容 根据ID编号  返回 一\\二\\三\\四  空的区域返回 一\\ \\ \\ 
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <param name="id3"></param>
        /// <param name="id4"></param>
        /// <returns>否返回空</returns>
        public string GetSection(string id1, string id2, string id3, string id4)
        {
          
            string section = "";
            if (id1 != "" && id1 != null)
            {
                foreach (DataJson.Area1 a1 in FileMesege.AreaList)
                {
                    if (a1.id1 == id1)
                    {
                        section = a1.area;
                        if (id2 != "" && id2 != null)
                        {
                            foreach (DataJson.Area2 a2 in a1.area2)
                            {
                                if (a2.id2 == id2)
                                {
                                    section = section + "\\" + a2.area;
                                    if (id3 != "" && id3 != null)
                                    {
                                        foreach (DataJson.Area3 a3 in a2.area3)
                                        {
                                            if (a3.id3 == id3)
                                            {
                                                section = section + "\\" + a3.area;
                                                if (id4 != "" && id4 != null)
                                                {
                                                    foreach (DataJson.Area4 a4 in a3.area4)
                                                    {
                                                        if (a4.id4 == id4)
                                                        {
                                                            section = section + "\\" + a4.area;
                                                            break;
                                                        }
                                                    }//for4
                                                }//if id4 !=null""
                                                break;
                                            }

                                        }//for3
                                    }// if id3!=null""
                                    
                                    break;
                                }
                            }//for2
                        }//IF id2!= null""
                        break;
                    }


                }//for1
            }//if id1!= null""
            while (section.Split('\\').Length != 4)
            {
                section = string.Format("{0}\\", section);
            }

            return section;
        }



        /// <summary>
        /// 获取四位ID编号 根据区域名称
        /// </summary>
        /// <param name="section">字符串数组</param>
        /// <returns>必定为四位 空为“”</returns>
        public string[] GetSectionId(string[] section)
        { 
            List<string> id = new List<string>();
            try
            {
                foreach (DataJson.Area1 a1 in FileMesege.AreaList)
                {
                    if (a1.area == section[0])
                    {
                        id.Add(a1.id1);
                        foreach (DataJson.Area2 a2 in a1.area2)
                        {
                            if (a2.area == section[1])
                            {
                                id.Add(a2.id2);
                                foreach (DataJson.Area3 a3 in a2.area3)
                                {
                                    if (a3.area == section[2])
                                    {
                                        id.Add(a3.id3);
                                        foreach (DataJson.Area4 a4 in a3.area4)
                                        {
                                            if (a4.area == section[3])
                                            {
                                                id.Add(a4.id4);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                while (id.Count < 4)
                {
                    id.Add("");
                }
                return id.ToArray();
            }
            catch {
                while (id.Count < 4)
                {
                    id.Add("");
                }
                return id.ToArray();
            }
            
        }

        /// <summary>
        /// 获取四位区域 必定返回4位 1\\2\\3\\4 空的返回“”
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string GetSectionByNode(TreeNode node)
        {
            try
            {
                string section = node.FullPath;
                while (section.Split('\\').Length != 4)
                {
                    section = string.Format("{0}\\", section);
                }
                return section;
            }
            catch {
                return "";
            }
        }


        /// <summary>
        /// 获取树状图当前展开状态   返回 展开节点层
        /// </summary>
        /// <param name="treeView1"></param>
        /// <returns></returns>
        public List<string> treeIsExpandsState(TreeView treeView1)
        {
            treeView1.Visible = false;
            //记录当前节点展开状况
            List<string> isExpands = new List<string>();
            foreach (TreeNode node1 in treeView1.Nodes)
            {
                if (node1.IsExpanded)
                {
                    isExpands.Add(node1.Text);

                }
                if (node1.Nodes == null)
                {
                    continue;
                }
                foreach (TreeNode node2 in node1.Nodes)
                {
                    if (node2.IsExpanded)
                    {
                        isExpands.Add(string.Format("{0} {1}", node1.Text, node2.Text));
                    }
                    if (node2.Nodes == null)
                    {
                        continue;
                    }
                    foreach (TreeNode node3 in node2.Nodes)
                    {
                        if (node3.IsExpanded)
                        {
                            isExpands.Add(string.Format("{0} {1} {2}", node1.Text, node2.Text, node3.Text));
                        }
                        if (node3.Nodes == null)
                        {
                            continue;
                        }
                    }
                }

            }
            treeView1.Nodes.Clear();
            
            return isExpands;
        }


        /// <summary>
        /// 根据节点展开记录 重新展开节点
        /// </summary>
        /// <param name="treeView1"></param>
        /// <param name="isExpands"></param>
        public void treeIspandsStateRcv(TreeView treeView1, List<string> isExpands)
        {
            string tmpname = "";
            foreach (TreeNode node1 in treeView1.Nodes)
            {
                tmpname = node1.Text;
                foreach (string name in isExpands)
                {
                    if (node1.Text == name)
                    {
                        node1.Expand();

                    }
                }
                if (node1.Nodes == null)
                {
                    continue;
                }
                foreach (TreeNode node2 in node1.Nodes)
                {
                    tmpname = string.Format("{0} {1}", node1.Text, node2.Text);
                    foreach (string name in isExpands)
                    {
                        if (tmpname ==name)
                        {
                            node2.Expand();

                        }
                    }
                    if (node2.Nodes == null)
                    {
                        continue;
                    }
                    foreach (TreeNode node3 in node2.Nodes)
                    {
                        tmpname = string.Format("{0} {1} {2}", node1.Text, node2.Text, node3.Text);
                        foreach (string name in isExpands)
                        {
                            if (tmpname == name)
                            {
                                node3.Expand();

                            }
                        }
                        if (node3.Nodes == null)
                        {
                            continue;
                        }
                    }
                }

            }
            treeView1.Visible = true;
        }


        /// <summary>
        /// 根据节点路径 返回树状图改节点
        /// </summary>
        /// <param name="treeView1"></param>
        /// <param name="Node"></param>
        /// <returns></returns>
        public static TreeNode findNodeByName(TreeView treeView1, string Node) 
        {
            
            foreach (TreeNode node1 in treeView1.Nodes)
            {
                if (node1.FullPath == Node)
                {
                    return node1;
                }
                foreach (TreeNode node2 in node1.Nodes)
                {
                    if (node2.FullPath == Node)
                    {
                        return node2;
                    }
                    foreach (TreeNode node3 in node2.Nodes)
                    {
                        if (node3.FullPath == Node)
                        {
                            return node3;
                        }
                    }
                }

            }
            return null;
        }


        /// <summary>
        /// 根据Point信息 在树状图选中改节点
        /// </summary>
        /// <param name="tv"></param>
        /// <param name="point"></param>
        public static void SelectNodeByPoint(TreeView tv, DataJson.PointInfo point)
        {
            try
            {
                foreach (TreeNode node1 in tv.Nodes)
                {
                    //寻找第一层 IP匹配节点
                    if (node1.Text.Contains(point.ip))
                    {
                        string section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                        string pointName = string.Format("{0} {1}", section, point.name).Trim();
                        string nodeName = "";
                        foreach (TreeNode node2 in node1.Nodes)
                        {
                            nodeName = node2.Text.Replace(node2.Text.Split(' ')[0]+" ", "").Trim();
                            if (nodeName == pointName)
                            {
                                tv.SelectedNode = node2;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("窗口跳转出问题 请检查treeMessage.SelectNodeByPoint");
            }
        }


        /// <summary>
        /// 根据节点fullpath选中树状图该节点
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="fullpath"></param>
        public static void SetPrevVisitNode(TreeView treeView, string fullpath)
        {
            if (!string.IsNullOrEmpty(fullpath))
            {
                foreach (TreeNode item in treeView.Nodes)
                {
                    TreeNode tmpNode = FindNode(item, fullpath);
                    if (tmpNode != null)
                    {
                        treeView.TopNode = tmpNode;
                        treeView.SelectedNode = tmpNode;
                        break;
                    }
                }

            }
        }
        
        /// <summary>
        /// 根据fullpath递归寻找节点
        /// </summary>
        /// <param name="tnParent"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static TreeNode FindNode(TreeNode tnParent, string strValue)
        {

            if (tnParent == null) return null;
            if (tnParent.FullPath == strValue) return tnParent;


            TreeNode tnRet = null;

            foreach (TreeNode tn in tnParent.Nodes)
            {

                tnRet = FindNode(tn, strValue);

                if (tnRet != null) break;

            }

            return tnRet;

        }



    }


}
