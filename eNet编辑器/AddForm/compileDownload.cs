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
    public partial class compileDownload : Form
    {
        public compileDownload()
        {
            InitializeComponent();
        }

        public event Action<string> AppTxtShow;

        private void compileDownload_Load(object sender, EventArgs e)
        {
            
            if (FileMesege.DeviceList == null)
            {
                return;
            }
 
                //从设备加载网关信息
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                cbIP.Items.Add(d.ip);
            }
            if (cbIP.Items.Count > 0)
            {
                cbIP.SelectedIndex = 0;
            }
        }

        #region 重绘
        private void compileDownload_Paint(object sender, PaintEventArgs e)
        {
            Rectangle myRectangle = new Rectangle(0, 0, this.Width, this.Height);
            //ControlPaint.DrawBorder(e.Graphics, myRectangle, Color.Blue, ButtonBorderStyle.Solid);//画个边框 
            ControlPaint.DrawBorder(e.Graphics, myRectangle,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 2, ButtonBorderStyle.Solid,
                Color.DarkGray, 2, ButtonBorderStyle.Solid
            );
        }

        private Point mPoint;
        private void plInfoTitle_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint = new Point(e.X, e.Y);
        }

        private void plInfoTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion


        //编译
        private void btnCompile_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i < cbIP.Items.Count; i++)
            {
                if (cbIP.Items[i].ToString() == cbIP.Text)
                {
                    //存在该Ip的信息 可以进行编译
                    FileMesege fm = new FileMesege();
                    if (fm.ObjDirClearByIP(cbIP.Text))
                    {
                        AppTxtShow(string.Format("({0})工程文件初始化成功！", cbIP.Text));

                    }
                    else
                    {
                        AppTxtShow(string.Format("({0})工程文件初始化失败！", cbIP.Text));
                        return;
                    }
                    
                    //抽离point 信息
                    if (fm.getPointJsonByIP(cbIP.Text))
                    {
                        AppTxtShow("point.json编译通过！");
                    }
                    else
                    {
                        AppTxtShow("point.json编译失败！");
                        return;
                    }

                    //获取area 信息
                    if (fm.getAreaJsonByIP(cbIP.Text))
                    {
                        AppTxtShow("area.json编译通过！");
                    }
                    else
                    {
                        AppTxtShow("area.json编译失败！");
                        return;
                    }

                    //获取device 信息
                    if (fm.getDeviceJsonByIP(cbIP.Text))
                    {
                        AppTxtShow("device.json编译通过！");
                    }
                    else
                    {
                        AppTxtShow("device.json编译失败！");
                        return;
                    }
                    
                    //编译场景
                    if (fm.getSceneJsonByIP(cbIP.Text))
                    {
                        AppTxtShow("scene.json编译通过！");
                    }
                    else
                    {
                        AppTxtShow("scene.json编译失败！");
                        
                    }
                    //编译定时
                    if (fm.getTimerJsonByIP(cbIP.Text))
                    {
                        AppTxtShow("timer.json编译通过！");
                    }
                    else
                    {
                        AppTxtShow("timer.json编译失败！");

                    }
                    //编译面板
                    if (fm.getPanelJsonByIP(cbIP.Text))
                    {
                        AppTxtShow("panel.json编译通过！");
                    }
                    else
                    {
                        AppTxtShow("panel.json编译失败！");

                    }
                    //编译感应
                    if (fm.getSensorJsonByIP(cbIP.Text))
                    {
                        AppTxtShow("sensor.json编译通过！");
                    }
                    else
                    {
                        AppTxtShow("sensor.json编译失败！");

                    }
                    ZipHelper zipHelp = new ZipHelper();
                    string file = string.Format("{0}\\objs\\{1}",FileMesege.TmpFilePath,cbIP.Text);
                    try
                    {
                        //压缩到选中路径
                        //zipHelp.ZipManyFilesOrDictorys();
                        zipHelp.ZipDir(file, string.Format("{0}.zip", file), 9);
                    }
                    catch
                    {
                        
                    }
                    break;
                }
            }
            
        }

        //下载到主机
        private void btnSend_Click(object sender, EventArgs e)
        {
            //编译 然后通过就可以发送到各个主机
        }




    }
}
