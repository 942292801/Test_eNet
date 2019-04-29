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
    public partial class ThreeOperation : Form
    {
        public ThreeOperation()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeOperationAddNode()
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
                        /*
                        foreach (DataJson.Scene sc in FileMesege.sceneList)
                        {
                            //  添加该网关IP的子节点
                            if (sc.IP == d.ip)
                            {
                                foreach (DataJson.scenes scs in sc.scenes)
                                {
                                    int index2 = tm.AddNode2(treeView1, Resources.Scene + scs.id + " " + scs.location.Replace(" ", "") + " " + scs.sceneName, index);
                                }

                            }
                        }*/
                    }

                }
            }
            catch
            {
                //错误处理
                MessageBox.Show("场景添加节点初始化失败,请检查scene.json文件");
            }
        }

        //选中字体高亮不消失
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
    }
}
