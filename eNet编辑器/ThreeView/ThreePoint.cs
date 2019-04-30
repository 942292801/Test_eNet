using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace eNet编辑器.ThreeView
{
    public partial class ThreePoint : Form
    {
        public ThreePoint()
        {
            InitializeComponent();
        }

        public event Action updateDgvPoint;
        private void ThreePoint_Load(object sender, EventArgs e)
        {

        }

        public void ThreePointAddNode()
        {
            try
            {
                TreeMesege tm = new TreeMesege();
                //记录当前节点展开状况
                // List<string> isExpands = tm.treeIsExpandsState(treeView1);
                treeView1.Nodes.Clear();
                tm.AddNode1(treeView1, "所有点位");
                DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//objs");
                string name = "";
                foreach (FileInfo file in folder.GetFiles("*.ini"))
                {

                    name = IniConfig.GetValue(file.FullName, "define", "name");
                    if (name != "")
                    {
                        tm.AddNode1(treeView1, name);
                    }

                }
            }
            catch { 
            
            }
            //展开记录的节点
            //tm.treeIspandsStateRcv(treeView1, isExpands);

        }

        /// <summary>
        /// 高亮显示选中项 重绘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 点击选中后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (string.IsNullOrEmpty(FileMesege.sectionNodeCopy))
            {
                return;
            }
            FileMesege.objType = IniHelper.findObjsFileNae_ByName(treeView1.SelectedNode.Text);
            if (treeView1.SelectedNode.Text == "所有点位")
            {
                FileMesege.objType = "所有点位";
            }
            updateDgvPoint();
        }

      

      
    }
}
