using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace eNet编辑器.AddForm
{
    public delegate void AddDev();
    //public delegate void ChangeDev();
    public partial class tnDevice : Form
    {
        public tnDevice()
        {
            InitializeComponent();
        }
        public static event Action<string> AppTxtShow;
        public bool isNew = false;
        //回调添加设备节点
        public event AddDev adddev;

        private string oldDevNum = "";
        private string oldDevVersion = "";

        private bool isChange = false;

        public bool IsChange
        {
            get { return isChange; }
            set { isChange = value; }
        }

        private void tnDevice_Load(object sender, EventArgs e)
        {
            string[] infos = FileMesege.info.Split(' ');
            if (infos.Length > 2)
            {
                //修改模式
                cbDevice.Text = Regex.Replace(infos[2], @"[^\d]*", "");
                oldDevNum = cbDevice.Text;
                cbVersion.Text = infos[3];
                oldDevVersion = infos[3];
            }
            //网关名称
            lbip.Text = infos[0];
            //获取设备名称
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//devices");
            string[] names = null;
            //循环添加设备号
            for (int i = 0; i < 64; i++)
            {
                cbDevice.Items.Add(i);
            }
            //循环添加设备名字
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                names = file.Name.Split('.');
                cbVersion.Items.Add(names[0]);

            }



        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnDecid_Click(object sender, EventArgs e)
        {
            //设备号不为空
            if (string.IsNullOrEmpty(cbDevice.Text) || string.IsNullOrEmpty(cbVersion.Text))
            {
                AppTxtShow("设备信息错误");
                return;
            }
            //设备ID号为数字
            bool isid = Regex.IsMatch(cbDevice.Text, @"[^\d]*");
            if (!isid)
            {

                AppTxtShow("设备号输入格式错误");
                return;
            }
            bool isVersion = false;
            //确保修改的型号存在
            foreach (string version in cbVersion.Items)
            {
                if (version == cbVersion.Text)
                {
                    isVersion = true;
                }
            }
            if (!isVersion)
            {
                AppTxtShow("设备型号不存在");
                return;
            }
            
            if (isNew)
            {
                foreach (DataJson.Device dev in FileMesege.DeviceList)
                {
                    if (dev.ip == lbip.Text)
                    {
                        foreach (DataJson.Module m in dev.module)
                        {
                            if (m.id == cbDevice.Text)
                            {
                                AppTxtShow("操作失败！请检查设备号！");
                                return;
                            }
                        }
                    }
                } 
                FileMesege.info = string.Format("{0} {1} {2}", lbip.Text, cbDevice.Text, cbVersion.Text);
                //添加设备回调
                adddev();
            }
            else
            {
                //设备号改变 
                if (cbDevice.Text != oldDevNum)
                {
                    isChange = true;
                }
                if(cbVersion.Text != oldDevVersion )
                {
                    //改设备版本
                    isChange = false;
                }
                FileMesege.info = string.Format("{0} {1} {2} {3} {4}", lbip.Text, cbDevice.Text, cbVersion.Text, oldDevNum, oldDevVersion);
                //修改模式
                this.DialogResult = DialogResult.OK;
            }
            
        }

      
    }
}
