using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eNet编辑器.DgvView
{
    public partial class LogicScene : UserControl
    {
        public LogicScene(int indexs)
        {
            InitializeComponent();
            for (int i = 0; i < indexs; i++)
            {
                
                int dex = dataGridView1.Rows.Add();
                dataGridView1.Rows[dex].Cells[0].Value = dex;
            }
        }
    }
}
