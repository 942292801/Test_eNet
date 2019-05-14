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
    public partial class timerYYHHDD : Form
    {
        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> AddCustomDate;

        public timerYYHHDD()
        {
            InitializeComponent();
        }

        private void timerYYHHDD_Load(object sender, EventArgs e)
        {
            Calendar.DisplayMonth = DateTime.Now;
        }

        private void timerYYHHDD_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Calendar_ItemDoubleClick(object sender, MouseEventArgs e)
        {
            btnAdd_Click(this, EventArgs.Empty);
        }


    

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (Calendar.SelectedDate.ToShortDateString() == "0001/1/1")
            {
                return;
            }
            string tmp = "";
            if (cbYear.Checked)
            {
                tmp = "****";
            }
            else
            {

                tmp = Calendar.SelectedDate.Year.ToString();
            }
            if (cbMonth.Checked)
            {
                tmp = string.Format("{0}/**", tmp);
            }
            else
            {
                tmp = string.Format("{0}/{1}", tmp, Calendar.SelectedDate.Month.ToString());
            }
            tmp = string.Format("{0}/{1}", tmp, Calendar.SelectedDate.Day.ToString());
            AddCustomDate(tmp);
        }

       
    }
}
