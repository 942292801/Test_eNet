using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eNet编辑器;
using eNet编辑器.DgvView;
using System.IO;
using eNet编辑器.AddForm;
using System.Text.RegularExpressions;
using eNet编辑器.Properties;
using System.Reflection;
namespace eNet编辑器.ThreeView
{
    public delegate void SendFormContrl(string msg);
    public delegate void ShowDeviceDgv(bool flag);
    public delegate void DgvNameAddItem();
    public delegate void DgvDeviceAddItem();
    public partial class ThreeName : Form
    {
        //主窗口显示信息
        public event SendFormContrl sendFormContrl;
        //dgv设备框添加信息
        public event DgvNameAddItem dgvNameAddItem;
        //dgv网关框添加信息
        public event DgvDeviceAddItem dgvDeviceAddItem;
        //显示那个加载网关框或者设备框
        public event ShowDeviceDgv showDevice;

        public event Action addTitleNode;

        //添加新网关框定义
        tnGateway tng;
        //添加新的设备框定义
        tnDevice tnd;
        
        bool newitemflag = false;
        
        public ThreeName()
        {
            InitializeComponent();

        }




        private void ThreeName_Load(object sender, EventArgs e)
        {
            //添加新网关的窗口初始化
            tng = new tnGateway();
            //回调添加新网关
            tng.addgw += new AddGW(newgwDelegate);
            //添加新设备的窗口初始化
            tnd = new tnDevice();
            //回调添加新设备
            tnd.adddev +=new AddDev(newdevDelegate);
            
        }
        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载
        /// </summary>
        public void ThreeNameAddNode()
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
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                //添加网关
                index = tm.AddNode1(treeView1, d.ip + " " + d.master);
                foreach (DataJson.Module m in d.module)
                {
                    //判断加英文         Resources.treeName为资源文件的KEY值
                    index2 = tm.AddNode2(treeView1, Resources.Device + m.id + " " + m.device , index);
                }
            }
            //展开记录的节点
            tm.treeIspandsStateRcv(treeView1, isExpands);

            
        }

        #region 对树状图网关增 删 改和  设备增

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            //右击树状图外面区域
            if (newitemflag != true)
            {
                //新建网关
                newTnGateway();  
                   
            }
            else//右击树状图区域
            {
                //新建设备
                newTnDevice();
                   
            }
 
        }


        /// <summary>
        /// 新建设备节点弹框
        /// </summary>
        /// <returns></returns>
        private void newTnDevice()
        {

            //把窗口向屏幕中间刷新
            tnd.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用           
            FileMesege.info = treeView1.SelectedNode.Text;
            tnd.isNew = true;
            tnd.Title = "添加";
            tnd.ShowDialog();

        }

        //新建设备
        public void newdevDelegate()
        {
            //新建设备
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //把树状图名字分割成IP +设备型号
            string[] strs = FileMesege.info.Split(' ');
            //网关IP
            string ip = strs[0];
            //设备号
            string id = strs[1];
            //设备型号
            string version = strs[2];
            //展开新增的节点的信息
            string parentNodePath = "";
            if (treeView1.SelectedNode != null)
            {
                parentNodePath = treeView1.SelectedNode.FullPath;
            }
            DataListHelper.newDevice(ip,id,version);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            if (FileMesege.tnselectNode != null)
            {
                try
                {
                    //展开新增的节点的
                    TreeMesege.findNodeByName(treeView1, parentNodePath).Expand();
                }
                catch
                {

                }

            }
            dgvNameAddItem();
            dgvDeviceAddItem();
            
        }

        /// <summary>
        /// 新建网关的弹框
        /// </summary>
        /// <returns></returns>
        private void newTnGateway()
        {
            //新建

            //把窗口向屏幕中间刷新
            tng.StartPosition = FormStartPosition.CenterParent;
            tng.isNew = true;
            tng.ShowDialog();


        }

        /// <summary>
        /// 新建网关回调函数
        /// </summary>
        public void newgwDelegate()
        {
            //新建网关
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //把树状图名字分割成IP +设备型号
            string[] strs = FileMesege.info.Split(' ');
            //网关IP
            string ip = strs[0];
            //设备型号
            string master = strs[1];
            DataListHelper.newGateway(ip,master);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);


        }


        
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
            //右击树状图外面区域
            if (newitemflag != true)
            {
                return;
            }
            else//右击树状图区域
            {
                    
                //把窗口向屏幕中间刷新
                tng.StartPosition = FormStartPosition.CenterParent;
                tng.isNew = false;
                tng.ShowDialog();
                if (tng.DialogResult == DialogResult.OK)
                {

                    //修改网关
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    //把树状图名字分割成IP +设备型号 + 旧的信息型号 
                    string[] strs = FileMesege.info.Split(' ');
                    //网关IP
                    string ip = strs[0];
                    //设备型号
                    string master = strs[1];
                    //网关IP
                    string oldip = strs[2];
                    DataListHelper.changeGateway(ip,master,oldip);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }

            }
            
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
           
            //右击树状图外面区域
            if (newitemflag != true)
            {
                return;
            }
            else//右击树状图区域
            {
                newitemflag = false;
                int Nodeindex = treeView1.SelectedNode.Index;
                //删除网关
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //把树状图名字分割成IP +设备型号
                string[] strs = treeView1.SelectedNode.Text.Split(' ');
                //网关IP
                string ip = strs[0];
                //设备型号
                string master = strs[1];
                DataListHelper.delGateway(ip,master);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                FileMesege.tnselectNode = null;
                ////选中删除节点的下一个节点
                if (treeView1.Nodes.Count > 0 )
                {
                    if (Nodeindex < treeView1.Nodes.Count)
                    {
                        treeView1.SelectedNode = treeView1.Nodes[Nodeindex];
                    }
                    else
                    {
                        treeView1.SelectedNode = treeView1.Nodes[0];
                    }
                    
                }
            }
            
             
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

        #region 对树状图设备的删 改
        private void 修改ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tnDevice tnd = new tnDevice();
            //把窗口向屏幕中间刷新
            tnd.StartPosition = FormStartPosition.CenterParent;
            //把当前选仲树状图网关传递到info里面 给新建设备框网关使用
            FileMesege.info = treeView1.SelectedNode.Parent.Text + " " + treeView1.SelectedNode.Text;
            tnd.isNew = false;
            tnd.Title = "修改";
            tnd.ShowDialog();
            if (tnd.DialogResult == DialogResult.OK)
            {


                //修改网关
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //把树状图名字分割成IP +设备型号 + 旧的信息型号 
                string[] strs = FileMesege.info.Split(' ');
                //网关IP
                string ip = strs[0];
                //设备号
                string id = strs[1];
                //设备型号
                string version = strs[2];
                //设备号
                string oldid = strs[3];
                //设备型号
                string oldVersion = strs[4];
                //修改IP
                if (tnd.IsChange)
                {
                    //修改NameList信息
                    DataListHelper.changePointID(ip, id, oldid);
                }
                else
                {
                    //删除NameList信息
                    DataListHelper.delPointID(ip, oldid);
                }
                DataListHelper.changeDevice(ip, id, version, oldid, oldVersion);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                //刷新窗体清空窗体信息
                dgvNameAddItem();
                dgvDeviceAddItem();

            }



        }


        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
                 //删除设备
                string ip = treeView1.SelectedNode.Parent.Text.Split(' ')[0];
                bool isip = Regex.IsMatch(ip, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
                if (!isip)
                {
                    return;
                }
                int Nodeindex = treeView1.SelectedNode.Index;
                int pNodeindex = treeView1.SelectedNode.Parent.Index;
                string id = Regex.Replace(treeView1.SelectedNode.Text.Split(' ')[0], @"[^\d]*", "");
                string version = treeView1.SelectedNode.Text.Split(' ')[1];
                //修改网关
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                DataListHelper.delDevice(ip, id, version);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                FileMesege.tnselectNode = null;
                //刷新窗体清空窗体信息
                dgvNameAddItem();
                dgvDeviceAddItem();

                //选中删除节点的下一个节点 没有节点就直接选中父节点
                if (treeView1.Nodes[pNodeindex].Nodes.Count > 0)
                {
                    if (Nodeindex < treeView1.Nodes[pNodeindex].Nodes.Count)
                    {
                        treeView1.SelectedNode = treeView1.Nodes[pNodeindex].Nodes[Nodeindex];
                    }
                    else
                    {
                        treeView1.SelectedNode = treeView1.Nodes[pNodeindex].Nodes[0];
                    }

                }
                else
                {
                    treeView1.SelectedNode = treeView1.Nodes[pNodeindex];
                }
        }

        #endregion


        #region 节点点击 点击后事件 树状图重绘
        /// <summary>
        /// 选中节点 显示DGV的信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            //判断 ini文件存在不  再判断是父子节点 
            string filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, treeView1.SelectedNode.Text.Split(' ')[1]); 
            TreeMesege tm = new TreeMesege();
            FileMesege.tnselectNode = treeView1.SelectedNode;
            addTitleNode();
            //判断ini文件存在不存在
            if (File.Exists(filepath))
            {

                //显示设备node
                sendFormContrl(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note") + "\r\n");                
                //调用dgv的ini配置
                if (treeView1.SelectedNode.Parent == null)
                {
                    //显示网关设备DGV框                
                    showDevice(true);
                    //选中为父节点显示DGVDevice  
                    dgvDeviceAddItem();
                    

                }
                else
                {
                    
                    //显示端口DGV框
                    showDevice(false);
                    //处理点击名字函数
                    dgvNameAddItem();
                }
            }
            else
            {
                //不存在文件
                sendFormContrl("");
                return;
                //MessageBox.Show("bu存在文件夹");

            }
        }

        /// <summary>
        /// 右击选中节点 显示菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
                newitemflag = false;
                if (CurrentNode != null)
                {
                    treeView1.SelectedNode = CurrentNode;//选中这个节点
                    if (CurrentNode.Parent != null)
                    {
                        //右击选择为子节点 显示菜单2
                        CurrentNode.ContextMenuStrip = contextMenuStrip2;
                        //newitemflag = true;
                    }
                    else
                    {
                        
                        //右击选择为父节点 显示示菜单1
                        CurrentNode.ContextMenuStrip = contextMenuStrip1;
                        newitemflag = true;
                    }
                }

            }

        }


    
      
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
                return;
                //foreColor = Color.Lime;//鼠标经过时文字颜色
                //backColor = Color.Gray;//鼠标经过时背景颜色
            }
            else
            {
                foreColor = this.treeView1.ForeColor;
                backColor = this.treeView1.BackColor;
            }
            
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
            e.Graphics.DrawString(e.Node.Text, this.treeView1.Font, new SolidBrush(foreColor), e.Bounds.X, e.Bounds.Y + 4);



        }
        #endregion

        #region 新建网关 新建设备 删除节点（按键）

        //新建网关
        private void btnAddGw_Click(object sender, EventArgs e)
        {
            newTnGateway();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }
            if (treeView1.SelectedNode.Parent == null)
            {
                //添加设备
                newTnDevice();
            }
          
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }
            if (treeView1.SelectedNode.Parent != null)
            {
                //删除设备
                删除ToolStripMenuItem1_Click(this, EventArgs.Empty);


            }
            else
            {
                newitemflag = true;
                //删除网关              
                删除ToolStripMenuItem_Click(this, EventArgs.Empty);
               
            }
        }
        #endregion






    }
}
