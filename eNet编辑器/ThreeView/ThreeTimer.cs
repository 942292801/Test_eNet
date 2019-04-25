using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eNet编辑器.ThreeView
{
    public partial class ThreeTimer : Form
    {
        public ThreeTimer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeTimerAddNode()
        {
            TreeMesege tm = new TreeMesege();
            treeView1.Nodes.Clear();
            if (FileMesege.DeviceList == null)
            {
                return;
            }
            try
            {
                //从设备加载网关信息
                foreach (DataJson.Device d in FileMesege.DeviceList)
                {
                    int index = tm.AddNode1(treeView1, d.ip + " " + d.master);
                    if (FileMesege.sceneList != null)
                    {
                        
                    }

                }
            }
            catch
            {
                //错误处理
                MessageBox.Show("场景添加节点初始化失败,请检查scene.json文件");
            }
        }

        //选中节点高亮不消失
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), e.Bounds);
                e.Graphics.DrawString(e.Node.Text, treeView1.Font, new SolidBrush(Color.Black), e.Bounds.Location);
            }
            else
            {
                e.DrawDefault = true;
            }
        }
    }
}
