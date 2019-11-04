using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace eNet编辑器.LogicForm
{
    public partial class LogicState : Form
    {
        public LogicState()
        {
            InitializeComponent();
        }

        ClientAsync client = new ClientAsync();

        /// <summary>
        /// types里 format的所有信息 发送格式
        /// </summary>
        private string format = "";

        /// <summary>
        /// types里 data的数据段 1-8所有信息
        /// </summary>
        private List<string[]> dataInfos = new List<string[]>();

        /// <summary>
        /// types data的key
        /// </summary>
        List<string> Keylist = new List<string>();

        /// <summary>
        /// 类型名称
        /// </summary>
        private string typeName;

        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        /// <summary>
        /// 状态值
        /// </summary>
        private string stateValue;
        public string StateValue
        {
            get { return stateValue; }
            set { stateValue = value; }
        }


        /// <summary>
        /// 十进制的地址
        /// </summary>
        private string addressDecimal;
        public string AddressDecimal
        {
            get { return addressDecimal; }
            set { addressDecimal = value; }
        }



        /// <summary>
        /// 返回信息
        /// </summary>
        private string returnVal;
        public string ReturnVal
        {
            get { return returnVal; }
            set { returnVal = value; }
        }


        //标志不是第一次加载信息
        bool isLoad = false;

        private string ip;
        private void LogicState_Load(object sender, EventArgs e)
        {
            try
            {
                ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
                //游标条
                DevComponents.DotNetBar.Controls.Slider[] slds = { sld1, sld2, sld3, sld4, sld5, sld6, sld7, sld8 };
                //左栏信息说明
                DevComponents.DotNetBar.LabelX[] lbs = { lb1, lb2, lb3, lb4, lb5, lb6, lb7, lb8 };
            
                //获取types下 ini类型名称
                string type = IniHelper.findTypesIniTypebyName(typeName);
                if (string.IsNullOrEmpty(type))
                {
                    return ;
                }
                string filepath = string.Format("{0}\\types\\{1}.ini", Application.StartupPath, type);
                //获取全部Section下的Key
                Keylist = IniConfig.ReadKeys("data", filepath);
                //顺便 读取format 数据格式
                format = IniConfig.GetValue(filepath, "data", "format");
                //inikey里面的序号
                int keyIndex;
                //data下的KEY值 rw,uint8,0-1,0-1,亮度(%)
                string[] infos = null;
                //拉度条值范围
                string[] sldValue = null;
                //转换为二进制的返回值
                string binVal = DataChange.HexString2BinString(stateValue).Replace(" ", "");
                for (int i = 0; i < Keylist.Count; i++)
                {
                    
                    //获取类型下data的数据  rw,uint8,0-1,0-1,亮度(%)
                    infos = IniConfig.GetValue(filepath, "data", Keylist[i]).Split(',');
                    //读取的value的格式不规范 直接退出不作处理 排除format
                    if (infos.Length != 5)
                    {
                        continue;
                    }
                    keyIndex = Convert.ToInt32(Keylist[i]);
                    //添加到data 信息列表里面去
                    dataInfos.Add(infos);
                    //sld控件处理
                    if (infos[3].Contains("-"))
                    {
                        sldValue = infos[3].Split('-');
                        slds[keyIndex - 1].Minimum = Convert.ToInt32(sldValue[0]);
                        slds[keyIndex - 1].Maximum = Convert.ToInt32(sldValue[1]);
                        if (infos[0].Contains("w"))
                        {
                            string val = DataChange.getBinBit(binVal, infos[2]);
                            //该项是可以进行写操作
                            slds[keyIndex - 1].Value =Convert.ToInt32( val);
                            slds[keyIndex - 1].Text =val;    
                            //开启Enable
                            slds[keyIndex - 1].Visible = true;
                            lbs[keyIndex - 1].Visible = true;
                            
                            //显示大小
                            //this.Size = new Size(254, this.Size.Height + 38);
                        }
                        //slds[keyIndex - 1].TextColor = Color.Red;
                    }



                    //lbs控件处理
                    lbs[keyIndex - 1].Text = infos[4]+"：";
                    //lbs[keyIndex - 1].ForeColor = Color.Red;

                }
                //连接tcp
                
                Init();
                isLoad = true;
            }
            catch
            {
                this.DialogResult = System.Windows.Forms.DialogResult.No;
                this.Close();
                //MessageBox.Show("界面初始化出错");
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

                        //MessageBox.Show("连接发生错误,请检查网络连接");

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
            
            //异步连接
            client.ConnectAsync(ip, 6003);

        }

        private void LogicState_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (string.IsNullOrEmpty(format))
            {
                //组合信息为空
                return;
            }
            DevComponents.DotNetBar.Controls.Slider[] slds = { sld1, sld2, sld3, sld4, sld5, sld6, sld7, sld8 };
            string content = format.Replace(" ","");
            string val = "";
            for (int i = 0; i < dataInfos.Count; i++)
            {
                if (dataInfos[i][0].Contains("w"))
                {
                    //替换值
                    val = slds[Convert.ToInt32(Keylist[i + 1]) - 1].Text;
                    //拉读条值 替换 相对应的位置
                    content = DataChange.replaceStr(content, val, dataInfos[i][2]);
                    returnVal = content;
                }
            }
            
            
        }

        private void LogicState_FormClosed(object sender, FormClosedEventArgs e)
        {
            //断开tcp连接
            if (client != null)
            {
                client.Dispoes();
            }
            
        }

        private void sld1_ValueChanged(object sender, EventArgs e)
        {
            sld1.Text = sld1.Value.ToString();
            sendMsg();
        }

        private void sld2_ValueChanged(object sender, EventArgs e)
        {
            sld2.Text = sld2.Value.ToString();
            sendMsg();
        }

        private void sld3_ValueChanged(object sender, EventArgs e)
        {
            sld3.Text = sld3.Value.ToString();
            sendMsg();
        }

        private void sld4_ValueChanged(object sender, EventArgs e)
        {
            sld4.Text = sld4.Value.ToString();
            sendMsg();
        }

        private void sld5_ValueChanged(object sender, EventArgs e)
        {
            sld5.Text = sld5.Value.ToString();
            sendMsg();
        }

        private void sld6_ValueChanged(object sender, EventArgs e)
        {
            sld6.Text = sld6.Value.ToString();
            sendMsg();
        }

        private void sld7_ValueChanged(object sender, EventArgs e)
        {
            sld7.Text = sld7.Value.ToString();
            sendMsg();
        }

        private void sld8_ValueChanged(object sender, EventArgs e)
        {
            sld8.Text = sld8.Value.ToString();
            sendMsg();
        }

        private void sendMsg()
        {
            if (isLoad && client.Connected())
            {
                if (string.IsNullOrEmpty(format))
                {
                    //组合信息为空
                    return;
                }
                DevComponents.DotNetBar.Controls.Slider[] slds = { sld1, sld2, sld3, sld4, sld5, sld6, sld7, sld8 };

                string content = format.Replace(" ", "");
                string val = "";
                for (int i = 0; i < dataInfos.Count; i++)
                {
                    if (dataInfos[i][0].Contains("w"))
                    {
                        //替换值
                        val = slds[Convert.ToInt32(Keylist[i + 1]) - 1].Text;
                        //拉读条值 替换 相对应的位置
                        content = DataChange.replaceStr(content, val, dataInfos[i][2]);
                        //returnVal = content;
                    }
                }
                //1.筛选当前发送内容
                //获取obj发送的指令    SET; 指令;对象信息  \r\n
                string msg = ToolsUtil.getObjSet(content,addressDecimal ,ip);
                //客户端发送数据
                client.SendAsync(msg);
                //MessageBox.Show(msg);
                
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

        private void LogicState_Deactivate(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void LogicState_Paint(object sender, PaintEventArgs e)
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
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        #endregion

      

 

       



    }
}
