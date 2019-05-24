using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace eNet编辑器.AddForm
{
    public partial class sceneAddress : Form
    {
        public sceneAddress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 传入地址
        /// </summary>
        private string obj;

        public string Obj
        {
            get { return obj; }
            set { obj = value; }
        }

        private string objType;

        /// <summary>
        /// 对象类型（中文）
        /// </summary>
        public string ObjType
        {
            get { return objType; }
            set { objType = value; }
        }

        /// <summary>
        /// 返回 类型（英文4.0_scene）
        /// </summary>
        private string rtType;

        public string RtType
        {
            get { return rtType; }
            set { rtType = value; }
        }

        /// <summary>
        /// 存放当前dev表中读取到的 ID+设备
        /// </summary>
        List<string> devName = null;

        private string ip = "";
        //是否为设备类型
        private bool isDev = false;

        private void sceneAddress_Load(object sender, EventArgs e)
        {
            cb2.Items.Add("设备");
            cb2.Items.Add("场景");
            cb2.Items.Add("定时");
            cb2.Items.Add("编组");
            cb2.Items.Add("面板");
            cb2.Items.Add("传感");
            //加载默认的类型到cb中
            iniInfo(objType);
            //初始默认设备列表信息到cb框
            addIp();


            //加载地址信息
            if (obj != "" && obj != null)
            {
                string[] infos = obj.Split('.');
                if (!isDev)
                {
                    //不为设备类型
                    cb1.Text = infos[0];
                    cb4.Text = (Convert.ToInt32(infos[2]) * 256 + Convert.ToInt32(infos[3])).ToString();
                    findNum();
                }
                else
                {
                    cb1.Text = infos[0];
                    cb3.Text = infos[2];
                    cb4.Text = infos[3];
                    findPort(infos[2]);
                }
                   
            }
            
            
        }

        /// <summary>
        /// 根据类型type（中文名） 加载Ini的信息
        /// </summary>
        private void iniInfo(string findType)
        {
            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type = "";
            isDev = true;
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "define", "name");
                //找到类型一致
                if (type == findType)
                {
                    Label[] lbs = { lb1, lb2, lb3, lb4 };
                    ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
                    //读取【address】1-4信息
                    for (int i = 0; i < 4; i++)
                    {
                        string[] infos = IniConfig.GetValue(file.FullName, "address", (i + 1).ToString()).Split(',');
                        if (infos.Length == 2)
                        {
                            lbs[i].Text = infos[0];
                            cbTextChange(cbs[i], infos[1]);
                        }
                        else
                        {
                            //场景 定时 面板 传感 即cb3关闭
                            lbs[i].Text = "";
                            cbs[i].Enabled = false;
                            isDev = false;
                        }
                    }
                    break;
                }
                else if (string.IsNullOrEmpty(findType))
                {
                    Label[] lbs = { lb1, lb2, lb3, lb4 };
                    ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
                    //读取【address】1-4信息
                    for (int i = 0; i < 4; i++)
                    {
                        string[] infos = IniConfig.GetValue(file.FullName, "address", (i + 1).ToString()).Split(',');
                        lbs[i].Text = infos[0];
                        cbTextChange(cbs[i], infos[1]);
                        
                    }
                    break;
                }

            }///foreach 
        }

        /// <summary>
        /// 加载网关Ip到cb1的框里面 
        /// </summary>
        /// <param name="selectSumNode">选中子节点</param>
        private void addIp()
        {

            try
            {
                devName = new List<string>();
                switch (FileMesege.formType)
                {
                    ////////////////////////////////////还需要后续添加选中节点参数
                    case "name":

                        break;

                    case "point":
                        break;
                    case "scene":
                        ip = FileMesege.sceneSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "timer":
                        ip = FileMesege.timerSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "panel":
                        ip = FileMesege.panelSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "logic":

                        break;
                    case "operation":

                        break;
                    default: break;
                }

                cb1.Enabled = false;
                cb1.Text = ip.Split('.')[3];
                addNumForDevList();
                
            }
            catch
            {

            }
           
        }

        /// <summary>
        /// 加载网关的设备到cb3里面
        /// </summary>
        private void addNumForDevList()
        {
            foreach (DataJson.Device devip in FileMesege.DeviceList)
            {
                if (devip.ip == ip)
                {
                    cb3.Items.Clear();
                    foreach (DataJson.Module md in devip.module)
                    {
                        devName.Add(md.id + " " + md.device);
                        cb3.Items.Add(md.id);
                    }
                    break;
                }
            }
        }

       

        /// <summary>
        /// cb信息内容的判断1-9 或 1,2,3  或数字（链路类型）
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void cbTextChange(ComboBox cb, string info)
        {
            
            if (info.Contains("-"))
            {
                cb.Items.Clear();
                string[] infos = info.Split('-');
                int j = Convert.ToInt32(infos[1]);
                for (int i = Convert.ToInt32(infos[0]); i <= j; i++)
                {
                    cb.Items.Add(i.ToString());
                }
                cb.Enabled = true;
            }
            else
            {
                if (cb == cb2)
                {
                    cb.Text = info;
                    cb.Enabled = true;
                }
                else
                {
                    cb.Items.Clear();
                    cb.Items.Add(info);
                    if (cb == cb4)
                    {
                        cb.Enabled = true;
                    }
                    else
                    {
                        cb.Enabled = false;
                    }
                    
                }
                
            }
           
        }

        private void btnDecid_Click(object sender, EventArgs e)
        {

            string newobj = "";
            if (cb1.Text != "" && cb2.Text != "")
            {
                switch (cb2.Text)
                { 
                    case "设备":
                        newobj = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr("0");
                        break;
                    case "场景":
                        newobj = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr("16");
                        break;
                    case "定时":
                        newobj = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr("32");
                        break;
                    case "人感":
                        newobj = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr("48");
                        break;
                    case "面板":
                        newobj = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr("48");
                        break;
                    case "编组":
                        newobj = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr("48");
                        break;
                    default:
                        newobj = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr("0");
                        break;
                }
                
                if (cb3.Text != "" && cb4.Text != "")
                {
                    newobj = newobj + SocketUtil.strtohexstr(cb3.Text) + SocketUtil.strtohexstr(cb4.Text);
                }
                else if (cb3.Text == "" && cb4.Text != "")
                {
                    string tmp = SocketUtil.strtohexstr(cb4.Text);
                    while (tmp.Length < 4)
                    {
                        tmp = tmp.Insert(0, "0");
                    }
                    newobj = newobj + tmp;
                }
            }
            if (newobj.Length == 8)
            {
                this.obj = newobj;
                this.DialogResult = DialogResult.OK;
            }
            this.DialogResult = DialogResult.OK;
        }



        private void cb2_TextChanged(object sender, EventArgs e)
        {
            
            typeChange(cb2.Text);
            if (isDev)
            {
                addIp();
            }
            else
            {
                findNum();
            }
            
        }

        /// <summary>
        /// 当cb2类型改变cb3 cb4的格式  设备 场景 定时 面板 人感
        /// </summary>
        /// <param name="cbType"></param>
        private void typeChange(string cbType)
        {
            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type = "";
            cb3.Text = "";
            cb4.Text = "";
            isDev = true;
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "address", "2").Split(',')[1];
                //找到类型一致
                if (type == cbType)
                {
                    //记录当前类型 场景 定时 面板 人感 编组
                    rtType = file.Name.Replace(".ini","");
                    Label[] lbs = { lb1, lb2, lb3, lb4 };
                    ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
                    //读取【address】1-4信息
                    for (int i = 0; i < 4; i++)
                    {
                        string[] infos = IniConfig.GetValue(file.FullName, "address", (i + 1).ToString()).Split(',');
                        if (infos.Length == 2)
                        {
                            lbs[i].Text = infos[0];
                            cbTextChange(cbs[i], infos[1]);
                        }
                        else
                        {
                            //场景 定时 面板 传感 即cb3关闭
                            lbs[i].Text = "";
                            cbs[i].Enabled = false;
                            isDev = false;
                        }
                    }
                    break;
                }
                //不存在该个类型
                rtType = "";
            }///foreach 
             
        }

        private void cb3_TextChanged(object sender, EventArgs e)
        {
            if (cb3.Enabled)
            {
                findPort(cb3.Text);
            }
 
            
        }

        /// <summary>
        /// 寻找该设备的端口数目
        /// </summary>
        /// <param name="id"></param>
        private void findPort(string id)
        {
            if (devName == null)
            {
                return;
            }
            for (int i = 0; i < devName.Count; i++)
            {
                if (devName[i].Split(' ')[0] == id)
                {
                    string filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, devName[i].Split(' ')[1]);
                    //获取全部Section下的Key
                    List<string> list = IniConfig.ReadKeys("ports", filepath);
                    cb4.Items.Clear();
                    //循环添加行信息
                    for (int j = 0; j < list.Count; j++)
                    {

                        cb4.Items.Add(list[j]);
                    }
                    break;
                }
            }

        }

        /// <summary>
        /// 查找当前ip的存在场景 定时 面板 号
        /// </summary>
        private void findNum()
        {
            try
            {
                
                cb4.Items.Clear();
                switch (cb2.Text)
                {
                    case "场景":
                        DataJson.Scene scene = DataListHelper.getSceneList(ip);

                        foreach (DataJson.scenes scenes in scene.scenes)
                        {
                            cb4.Items.Add(scenes.id);
                        }
                        break;
                    case "定时":
                        DataJson.Timer timer = DataListHelper.getTimerList(ip);

                        foreach (DataJson.timers tms in timer.timers)
                        {
                            cb4.Items.Add(tms.id);
                        }
                        break;
                    case "面板":
                        DataJson.Panel panel = DataListHelper.getPanelList(ip);

                        foreach (DataJson.panels pls in panel.panels)
                        {
                            cb4.Items.Add(pls.id);
                        }
                        break;
                    case "人感":
                        break;
                    case "":
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }
        

        private void sceneAddress_Paint(object sender, PaintEventArgs e)
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


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void plInfoTitle_MouseDown_1(object sender, MouseEventArgs e)
        {
            mPoint = new Point(e.X, e.Y);
        }

        private void plInfoTitle_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

       

      


        


    }
}
