using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace eNet编辑器.DgvView
{
    public partial class LogicVoice : UserControl
    {
        public LogicVoice()
        {
            InitializeComponent();
        }

        private void LogicVoice_Load(object sender, EventArgs e)
        {
            dgvAddColumn();
        }

        private void dgvAddColumn()
        {
           
            //新增对象列 加载
            this.dataGridView2.Rows.Clear();
            //新增对象列 加载
            this.dataGridView3.Rows.Clear();

            DataGridViewComboBoxColumn dgvc = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn dgvc2 = new DataGridViewComboBoxColumn();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "define", "name");
                if (!string.IsNullOrEmpty(type))
                {
                    dgvc.Items.Add(type);
                    dgvc2.Items.Add(type);

                }
            }
            //设置列名
            dgvc.HeaderText = "类型";
            //设置下拉列表的默认值 
            dgvc.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //或者这样设置 默认选择第一项
            dgvc.DefaultCellStyle.NullValue = dgvc.Items[0];
            dgvc.ReadOnly = true;
            dgvc.Name = "type";


            //设置列名
            dgvc2.HeaderText = "类型";
            //设置下拉列表的默认值 
            dgvc2.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //或者这样设置 默认选择第一项
            dgvc2.DefaultCellStyle.NullValue = dgvc.Items[0];
            dgvc2.ReadOnly = true;
            dgvc2.Name = "type";

            //插入
            this.dataGridView2.Columns.Insert(1, dgvc);
            this.dataGridView3.Columns.Insert(1, dgvc2);
        }
    }
}
