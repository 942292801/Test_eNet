using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

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
            //利用反射设置DataGridView的双缓冲
            //Type dgvType = this.treeView1.GetType();
            //PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            //BindingFlags.Instance | BindingFlags.NonPublic);
            //pi.SetValue(this.treeView1, true, null);
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
                DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "\\objs");
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
