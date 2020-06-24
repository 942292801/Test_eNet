using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;

namespace eNet编辑器
{
    class UdpSocket
    {

        private UdpClient m_UdpClient;
        
        /// <summary>
        /// 本机IP 和Port
        /// </summary>
        IPEndPoint m_LocalIPEndPoint;
        /// <summary>
        /// 客户端接收消息触发的事件
        /// </summary>
        public event Action<string, string> Received;

        private bool isClose = false;
        public bool isbing =false;

        /// <summary>
        /// UDP绑定IP与端口  线程池开启接收数据
        /// </summary>
        /// <param name="IP">本地IP</param>
        /// <param name="port">本地端口</param>
        public void udpBing(string IP,string port)
        {
            try
            {
                IPAddress LocalIP = IPAddress.Parse(IP);//本地IP
                int LocalPort = Convert.ToInt32(port);//本地Port
                m_LocalIPEndPoint = new IPEndPoint(LocalIP, LocalPort);//本地IP和Port

                m_UdpClient = new UdpClient(m_LocalIPEndPoint);
                isbing = true;
                ThreadPool.QueueUserWorkItem(x =>
                {
                    while (!isClose)
                    {
                        try
                        {
                            //Thread.Sleep(20);
                            udpReceive();
                            //Thread.Sleep(20);
                        }
                        catch (Exception)
                        {
                            Close();
                        }
                    }
                });
                
            }
            catch (Exception ex)
            {
                isbing = false;
                Console.WriteLine(ex.Message);
 
            }
        }


        /// <summary>
        /// UDP发送信息
        /// </summary>
        /// <param name="IP">远程目标IP地址</param>
        /// <param name="Port">远程目标Port</param>
        /// <param name="msg">发送的信息</param>
        public void udpSend(string IP,string Port,string msg)
        {
            try
            {


                if (isbing)
                {
                    IPAddress RemoteIP;   //远端 IP                
                    int RemotePort;      //远端 Port
                    IPEndPoint RemoteIPEndPoint; //远端 IP&Port
                    if (IPAddress.TryParse(IP, out RemoteIP) == false)//远端 IP
                    {
                        MessageBox.Show("IP地址错误！Remote IP is Wrong!", "Wrong");
                        return;
                    }


                    RemotePort = Convert.ToInt32(Port);//远端 Port
                    RemoteIPEndPoint = new IPEndPoint(RemoteIP, RemotePort);//远端 IP和Port
                                                                            //Get Data
                    byte[] sendBytes = System.Text.Encoding.Default.GetBytes(msg);
                    int cnt = sendBytes.Length;

                    if (0 == cnt)
                    {
                        return;
                    }
                    //Send
                    m_UdpClient.Send(sendBytes, cnt, RemoteIPEndPoint);
                }
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }
        }

        /// <summary>
        /// UDP接收线程
        /// </summary>
        private void udpReceive()
        {
            int cnt = 0;
            //接收到信息回调主窗口处理
            //定义IPENDPOINT，装载远程IP地址和端口 
            IPEndPoint remoteIpAndPort = new IPEndPoint(IPAddress.Any, 0);
            byte[] ReceiveBytes = m_UdpClient.Receive(ref remoteIpAndPort);
            //当前接收信息的长度
            cnt = ReceiveBytes.Length;
            string str =System.Text.Encoding.Default.GetString(ReceiveBytes, 0, cnt);
            //信息回调给event
            Received(remoteIpAndPort.Address.ToString(), str);
        }

        /// <summary>
        /// 释放udp资源
        /// </summary>
        public void udpClose()
        {
            if (isbing)
            {
                //关闭 UDP
                m_UdpClient.Close();
            }           
            Close();
            isClose = false;
            isbing =false;
        }

        /// <summary>
        /// 标记接收线程退出
        /// </summary>
        private void Close()
        {
            isClose = true;
        }
    }
}
