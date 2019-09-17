using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using eNet编辑器.Properties;

namespace eNet编辑器.DgvView
{
    public partial class LogicScene : UserControl
    {
        public LogicScene()
        {
            InitializeComponent();
        }

        //DataJson.logicsInfo tmp

        private void LogicScene_Load(object sender, EventArgs e)
        {
            //新增对象列 加载
            this.dataGridView1.Rows.Clear();
            DataGridViewComboBoxColumn dgvc = new DataGridViewComboBoxColumn();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                dgvc.Items.Add(IniConfig.GetValue(file.FullName, "define", "name"));
            }
            //设置列名
            dgvc.HeaderText = "类型";
            //设置下拉列表的默认值 
            dgvc.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //或者这样设置 默认选择第一项
            dgvc.DefaultCellStyle.NullValue = dgvc.Items[0];
            dgvc.ReadOnly = true;
            dgvc.Name = "type";
            //插入
            this.dataGridView1.Columns.Insert(2, dgvc);
          
        }

        /// <summary>
        /// 窗体信息初始化
        /// </summary>
        public void formInfoIni(DataJson.logicsInfo logicInfo)
        {
            cbSceneGetItem();
            cbAttr.SelectedIndex = logicInfo.attr;
            
        }

        /// <summary>
        /// 获取当前IP存在的场景信息
        /// </summary>
        private void cbSceneGetItem()
        {
            cbScene.Items.Clear();
            string ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
            string section = "";

                if (FileMesege.sceneList != null)
                {
                    foreach (DataJson.Scene sc in FileMesege.sceneList)
                    {
                        //  添加该网关IP的子节点
                        if (sc.IP == ip)
                        {
                            foreach (DataJson.scenes scs in sc.scenes)
                            {
                                DataJson.PointInfo point = DataListHelper.findPointByPid(scs.pid, FileMesege.PointList.scene);
                                if (point != null)
                                {
                                    section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                    cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));

                                }

                            }

                        }
                    }
                }

            
        }

        private void cbAttr_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



    }
}
