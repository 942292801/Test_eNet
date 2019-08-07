using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace eNet编辑器.AddForm
{
    public partial class DGVconcrol : Form
    {
        public DGVconcrol()
        {
            InitializeComponent();
        }
        ClientAsync client = new ClientAsync();
        //private string type;
        /// <summary>
        /// 保存信息存回dgv 开关（0,0,0,1）
        /// </summary>
        private string dgvshow;
        public string Dgvshow
        {
            get { return dgvshow; }
            set { dgvshow = value; }
        }
        /// <summary>
        /// 存放type.ini里面的commod信息
        /// </summary>
        private List<data> comdsList =new List<data>();
        class data {
            public string[] list { get;set;}
        }
        /// <summary>
        /// 软件运行路径
        /// </summary>
        private string path = Application.StartupPath;
        /// <summary>
        /// 当前行数 0开始
        /// </summary>
        private int rowindex;
        public int Rowindex
        {
            get { return rowindex; }
            set { rowindex = value; }
        }

        private DataJson.PointInfo point = null;

        internal DataJson.PointInfo Point
        {
            get { return point; }
            set { point = value; }
        }

        private void DGVconcrol_Load(object sender, EventArgs e)
        {
            try
            {
                //1.获取当前设备的名字信息 读一次device .ini 该行的类型 
                string[] names = FileMesege.tnselectNode.Text.Split(' ');
                string[] type = IniConfig.GetValue(path + "//devices//" + names[1] + ".ini", "ports", (rowindex + 1).ToString()).Split(',');

                //2.打开对应类型的type.ini文件加载
                string tmp = "";
                //循环读取command 1= 2= 3= 信息
                for (int i = 1; i < 30; i++)
                {
                    tmp = IniConfig.GetValue(path + "//types//" + type[0] + ".ini", "command", i.ToString());
                    if (tmp == "")
                    {
                        break;
                    }
                    data a = new data();

                    a.list = tmp.Split(',');

                    //把命令存放到信息
                    comdsList.Add(a);
                }

                //2.1 cb1 加载 type.ini信息的头一个信息 
                foreach (data comd in comdsList)
                {
                    cbVersion.Items.Add(comd.list[0]);
                }
                //默认加载cbversion第一个选项信息
                cbVersion.SelectedIndex = 0;
                cbAndlb(0);
                Init();
            }
            catch
            { 
            
            }
             
        }

        /// <summary>
        /// 当Version选项改变时 lb信息 和cb信息更新
        /// </summary>
        /// <param name="indexs">version索引</param>
        private void cbAndlb(int indexs)
        {
            //2.2 cb2-3 lb1-4按读取分割的顺序添加 信息 
            lb1.Text = comdsList[indexs].list[1];
            lb2.Text = comdsList[indexs].list[3];
            lb3.Text = comdsList[indexs].list[5];
            lb4.Text = comdsList[indexs].list[7];
            //2.3 cb2-3 的内容需要判断 1-100 表示有1-100个内容选择 enable打开
            cbTextChange(cb1, comdsList[indexs].list[2]);
            cbTextChange(cb2, comdsList[indexs].list[4]);
            cbTextChange(cb3, comdsList[indexs].list[6]);
            cbTextChange(cb4, comdsList[indexs].list[8]);
        }

        /// <summary>
        /// cb信息内容的判断1-9 或 1,2,3  或数字
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void cbTextChange(ComboBox cb,string info)
        {
            cb.Items.Clear();
            cb.Enabled = true;
            if (info.Contains("\\"))
            {
                dealXiegan(cb, info);
            }
            else if (info.Contains(":"))
            {
                dealMaohao(cb, info);
            }
            else if (info.Contains("-"))
            {
                dealHenggan(cb, info);
            }
            else if (info == "getObj")
            {
                if (point != null && !string.IsNullOrEmpty(point.objType) && !string.IsNullOrEmpty(point.value))
                {
                    dealGetObj(cb, info);
                    
                }
                else
                {
                    cb.Items.Add("点位：对象和参数未赋值 ");
                    cb.Enabled = false;
                }

            }
            else
            {
                cb.Items.Add(info);
                cb.Enabled = false;
            }
            //初始化索引
            cb.SelectedIndex = 0;
            
        }

        private void dealXiegan(ComboBox cb,string info)
        {
            string[] parents = info.Split('\\');
            for (int l = 0; l < parents.Length; l++)
            {
                if (parents[l].Contains(':'))
                {
                    dealMaohao(cb, parents[l]);
                }
                else if(parents[l].Contains('-'))
                {
                    dealHenggan(cb, parents[l]);
                }
                else
                {
                    cb.Items.Add(parents[l]);
                }
            }
        }

        private void dealMaohao(ComboBox cb, string info)
        {
            string[] parents = info.Split(':');
            if (parents[0].Contains("-"))
            {
                string[] child = parents[0].Split('-');
                int j = Convert.ToInt32(child[1]);
                for (int i = Convert.ToInt32(child[0]); i <= j; i++)
                {
                    cb.Items.Add(i.ToString()+parents[1]);
                }
            }
            else
            {
                cb.Items.Add(info);
            }
            
        }

        private void dealHenggan(ComboBox cb, string info)
        {
            string[] child = info.Split('-');
            int j = Convert.ToInt32(child[1]);
            if (j > 100)
            {
                j = 100;
            }
            for (int i = Convert.ToInt32(child[0]); i <= j; i++)
            {
                cb.Items.Add(i.ToString());
            }
        }

        private void dealGetObj(ComboBox cb, string info)
        {
            string filepath = string.Format("{0}//objs//{1}.ini", path, point.objType);
            List<string> keys = IniConfig.ReadKeys(point.value, filepath);
            
            for (int i = 2; i < keys.Count; i++)
            {
                cbTextChange(cb, IniConfig.GetValue(filepath, point.value, keys[i]));
            }
        }

        //改变cbversion窗口值改变
        private void cbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbAndlb(cbVersion.SelectedIndex);
        }

        
        //确认 发送代码操作 或把值传输回DGV框内
        private void btnDecid_Click(object sender, EventArgs e)
        {
            if (client.Connected())
            {
                string cb1Num = dealNum(cb1.Text);
                string cb2Num = dealNum(cb2.Text);
                string cb3Num = dealNum(cb3.Text);
                string cb4Num = dealNum(cb4.Text);
                if (string.IsNullOrEmpty(cb1Num) || string.IsNullOrEmpty(cb2Num) || string.IsNullOrEmpty(cb3Num) || string.IsNullOrEmpty(cb4Num))
                {
                    return;
                }
                //1.筛选当前发送内容
                //获取obj发送的指令    SET; 指令;对象信息  \r\n
                string content = SocketUtil.strtohexstr(cb1Num) + SocketUtil.strtohexstr(cb2Num) + SocketUtil.strtohexstr(cb3Num) + SocketUtil.strtohexstr(cb4Num);
                string msg = SocketUtil.getObjSet(content, rowindex, FileMesege.tnselectNode);
                //客户端发送数据
                client.SendAsync(msg);
            }
        }

        /// <summary>
        /// 提取 21秒 或 255 ：0.1秒 前面的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string dealNum(string str) 
        {
            string tmp = "";
            if (str.Contains(":"))
            { 
                tmp = Regex.Replace(str.Split(':')[0], @"[^\d]*", "");
            }else
            {
                tmp = Regex.Replace(str, @"[^\d]*", "");
            }
            return tmp;
        }

 
        private void DGVconcrol_FormClosed(object sender, FormClosedEventArgs e)
        {
            //断开tcp连接
            if ( client!= null)
            {
                client.Dispoes();
            }

        }

        /// <summary>
        /// 异步连接TCP信息回调初始化
        /// </summary>
        private void Init()
        {
            
            client.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync.EnSocketAction>((c, enAction) =>
            {
                /*string key = "";

                try
                {
                    if ( c.Client.Connected)
                    {
                        IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                        key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                    }
                }
                catch {}*/

                switch (enAction)
                {
                    case ClientAsync.EnSocketAction.Connect:
                        //MessageBox.Show("已经与" + key + "建立连接");
                        break;
                    case ClientAsync.EnSocketAction.SendMsg:

                        //MessageBox.Show(DateTime.Now + "：向" + key + "发送了一条消息");
                        break;
                    case ClientAsync.EnSocketAction.Close:

                        //MessageBox.Show("服务端连接关闭");
                        break;
                    case ClientAsync.EnSocketAction.Error:

                        MessageBox.Show("连接发生错误,请检查网络连接");

                        break;
                    default:
                        break;
                }
            });
            //信息接收处理
            client.Received += new Action<string, string>((key, msg) =>
            {
                //MessageBox.Show(key + msg);
            });
            string[] strip = FileMesege.tnselectNode.Parent.Text.Split(' ');
            //异步连接
            client.ConnectAsync(strip[0], 6003);
            
        }

       

        private void lbName_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

         #region 窗体样色


        #region 窗体样色2
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
         );

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:                        // box shadow
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

        }
        #endregion
        private void DGVconcrol_Paint(object sender, PaintEventArgs e)
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
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void lbName_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint = new Point(e.X, e.Y);
        }

      
        #endregion



    }
}
