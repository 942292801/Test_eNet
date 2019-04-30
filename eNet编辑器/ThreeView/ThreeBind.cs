using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using eNet编辑器.Properties;

namespace eNet编辑器.ThreeView
{
    public delegate void DgvBindAddItem();
    public partial class ThreeBind : Form
    {
        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> TxtShow;

        public event DgvBindAddItem dgvbindAddItem;
        public ThreeBind()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeBindAddNode()
        {
            TreeMesege tm = new TreeMesege();
            if (FileMesege.DeviceList == null)
            {
               
                return;
            }
            try
            {
                //devices 里面ini的名字
                string keyVal = "";
                string path = Application.StartupPath + "\\devices\\";
                //从设备加载网关信息
                foreach (DataJson.Device d in FileMesege.DeviceList)
                {
                    //新建网关节点
                    int index = tm.AddNode1(treeView1, d.ip + " " + d.master);
                    
                    //加载设备
                    foreach (DataJson.Module m in d.module)
                    {
                        keyVal = IniConfig.GetValue(path + m.device + ".ini", "input", "key");
                        if (keyVal != "null" )
                        {
                            //判断加英文
                            int index2 = tm.AddNode2(treeView1, Resources.Device + m.id + " " + m.device, index);
                            //节点加载的时候 判断Binglist里面是否存在该节点设备信息 不存在则写入新的
                            bindListMesage(d.ip,d.master,Convert.ToInt32(m.id),m.device,keyVal);

                        }
                            
                    }
                    
                }
            }
            catch
            {
                //错误处理
                MessageBox.Show("场景添加节点初始化失败,请检查scene.json文件");
            }
        }

        /// <summary>
        /// 判断IP信息 设备 信息 按键信息在 List是否存在  按键信息没有就自动添加ini表中key数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="master">网关名称</param>
        /// <param name="id">ID号</param>
        /// <param name="device">设备名称</param>
        /// <param name="keyVal">按键数量</param>
        private void bindListMesage(string ip, string master,int id,string device,string keyVal)
        {
            
            if (FileMesege.bindList == null)
            {
                FileMesege.bindList = new List<DataJson.Bind>();
            }
            //判断是否存在在IP的信息
            DataJson.Bind B = FileMesege.bindList.Find(delegate(DataJson.Bind b) { return b.IP == ip; });
            if (B == null)
            {
                //不存在IP信息
                B = new DataJson.Bind();
                B.Dev = master;
                B.IP = ip;
                B.Binds = new List<DataJson.binds>();
                //列表添加新IP的信息
                FileMesege.bindList.Add(B);

            }
            //判断设备信息
            DataJson.binds bs = B.Binds.Find(delegate(DataJson.binds b) { return b.id == id; });
            if (bs == null)
            {
                bs = new DataJson.binds();
                bs.id = id;
                bs.bindName = device;
                bs.bindInfo = new List<DataJson.bindInfo>();
                //IP信息 添加新的设备信息
                B.Binds.Add(bs);
            }
            //判断按键信息
            if (bs.bindInfo.Count < 1)
            { 
                DataJson.bindInfo binfo;
                //解析KeyVal  -->ini读取的key = keyVal
                //数据格式为num - num
                if (keyVal.Contains("-"))
                {
                   
                    string[] infos = keyVal.Split('-');
                    int j = Convert.ToInt32(infos[1]);
                    for (int i = Convert.ToInt32(infos[0]); i <= j; i++)
                    {
                        binfo = new DataJson.bindInfo();
                        binfo.groupId = id;
                        binfo.objType = "开关";
                        //binfo.Address
                        binfo.showType = "开关";
                        
                        binfo.showMode = "无";
                        binfo.keyId = i;
                        bs.bindInfo.Add(binfo);
                    }
                   
                }
                else
                {
                    int j = Convert.ToInt32(keyVal);
                    //纯数字 添加
                    for(int i =1;i<=j;i++)
                    {
                        binfo = new DataJson.bindInfo();
                        binfo.groupId = id;
                        binfo.objType = "开关";
                        //binfo.Address
                        binfo.showType = "开关";
                        binfo.showMode = "无";
                        binfo.keyId = i;
                        
                        bs.bindInfo.Add(binfo);
                    }
                }
               
            }


        }


        #region 节点点击 点击后事件 树状图重绘
        /// <summary>
        /// 选中节点  选择显示的DGV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FileMesege.bindSelectNode = treeView1.SelectedNode;
            //txtTest.Text = new TreeMesege().GetTopNode(treeView1.SelectedNode);
            //判断 ini文件存在不  再判断是父子节点 
            string[] arr = treeView1.SelectedNode.Text.Split(' ');
            string filename = arr[1] + ".ini";
            string filepath = Application.StartupPath + "\\devices\\" + filename;
            TreeMesege tm = new TreeMesege();
            string[] num = tm.GetNodeNum(treeView1.SelectedNode).Split(' ');
            //判断ini文件存在不存在
            if (File.Exists(filepath))
            {

                //显示设备node
                TxtShow(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note") + "\r\n");
                //调用dgv的ini配置
                dgvbindAddItem();
                //BindList添加信息


            }
            else
            {
                //不存在文件
                //sendFormContrl("");
                return;
                //MessageBox.Show("bu存在文件夹");

            }
        }

        

        /// <summary>
        /// 鼠标右击选中节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);

                if (CurrentNode != null)
                {
                    treeView1.SelectedNode = CurrentNode;//选中这个节点
                    if (CurrentNode.Parent != null)
                    {

                    }
                    else
                    {


                    }
                }
            }
        }

        //选中字体高亮
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(204, 235, 248)), e.Bounds);
                e.Graphics.DrawString(e.Node.Text, treeView1.Font, new SolidBrush(Color.Black), e.Bounds.Location);
            }
            else
            {
                e.DrawDefault = true;
            }
        }


        #endregion













    }//class
}
