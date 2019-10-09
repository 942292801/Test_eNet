using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace eNet编辑器.DgvView
{
    public partial class LogicCondition : UserControl
    {
        public LogicCondition()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
            doubleBuffered(dataGridView1);
            doubleBuffered(dataGridView2);
            doubleBuffered(dataGridView3);
        }

        #region 利用反射设置DataGridView的双缓冲
        private void doubleBuffered(DataGridView dataGridView)
        {
            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.dataGridView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }
        #endregion

        private void LogicCondition_Load(object sender, EventArgs e)
        {
            dgvAddColumn();
        }

        private void dgvAddColumn()
        {
            //新增对象列 加载
            this.dataGridView1.Rows.Clear();
            DataGridViewComboBoxColumn cbOperation = new DataGridViewComboBoxColumn();
            cbOperation.Items.Add("等于");
            cbOperation.Items.Add("大于");
            cbOperation.Items.Add("小于");
            cbOperation.Items.Add("不等于");
            cbOperation.Items.Add("大于等于");
            cbOperation.Items.Add("小于等于");
            //设置列名
            cbOperation.HeaderText = "比较式";
            //设置下拉列表的默认值 
            cbOperation.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //cbObjType.DefaultCellStyle.NullValue = cbObjType.Items[0];
            cbOperation.Name = "pointObjType";
            //cbOperation.ReadOnly = true;
            //插入执行对象
            this.dataGridView1.Columns.Insert(4, cbOperation);
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

        private void btnIfAdd_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        private void btnElseIfAdd_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Add();
        }

        private void btnElseAdd_Click(object sender, EventArgs e)
        {
            dataGridView3.Rows.Add();
        }




    }
}
