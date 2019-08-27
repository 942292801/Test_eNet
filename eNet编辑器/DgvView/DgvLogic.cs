using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace eNet编辑器.DgvView
{
    public partial class DgvLogic : Form
    {
        public DgvLogic()
        {
            
            //设置窗体双缓存
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            InitializeComponent();
            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.dataGridView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        private void DgvLogic_Load(object sender, EventArgs e)
        {

        }

        public void dgvLogicAddItem()
        { 
        
        }

        private void btnAddDay_Click(object sender, EventArgs e)
        {
            FileMesege.PointList = new DataJson.Point();
            while(true)
            {
                int randomNum = DataChange.randomNum();
                if (FileMesege.PointList.equipment.Exists(x => x.pid == randomNum))
                {

                    MessageBox.Show(string.Format("第{0}个存在相同pid", FileMesege.PointList.equipment.Count));
                    break;
                }
                else
                {
                    DataJson.PointInfo point = new DataJson.PointInfo();
                    point.pid = randomNum;
                    FileMesege.PointList.equipment.Add(point);

                }
            }
            
        }

    }
}
