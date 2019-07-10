using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace eNet编辑器.ThreeView
{
    public partial class ThreeLogic : Form
    {
        public ThreeLogic()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.treeView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.treeView1, true, null);
        }
        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeLogicAddNode()
        {
            TreeMesege tm = new TreeMesege();
            treeView1.Nodes.Clear();
            if (FileMesege.DeviceList == null)
            {
                treeView1.Nodes.Clear();
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

        //选中节点高亮不消失
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

      
    }
}
