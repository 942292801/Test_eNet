using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eNet编辑器.AddForm
{
    public partial class tsChange : Form
    {
        public tsChange()
        {
            InitializeComponent();
        }

        private void btnDecid_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("节点名称不能为空");
                return;
            }
            FileMesege.info = txtName.Text;
            this.DialogResult = DialogResult.OK;
        }


        private void tsChange_Load(object sender, EventArgs e)
        {
            txtName.Text = FileMesege.info;
        }

   
    }
}
