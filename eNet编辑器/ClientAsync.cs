﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace eNet编辑器
{
    class ClientAsync
    {
        //异步链接TCP Client
        private TcpClient client;
        /// <summary>
        /// 客户端连接完成、发送完成、连接异常或者服务端关闭触发的事件
        /// </summary>
        public event Action<TcpClient,EnSocketAction> Completed;
        /// <summary>
        /// 客户端接收消息触发的事件
        /// </summary>
        public event Action<string,string> Received;
        /// <summary>
        /// 用于控制异步接收消息
        /// </summary>
        private ManualResetEvent doReceive = new ManualResetEvent(false);
        //标识客户端是否关闭
        private bool isClose = false;
        public ClientAsync()
        {
            client = new TcpClient();
        }
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public void ConnectAsync(string ip, int port)
        {
            try
            {
                IPAddress ipAddress = null;
                try
                {
                    ipAddress = IPAddress.Parse(ip);
                }
                catch (Exception)
                {
                    OnComplete(client, EnSocketAction.Error);

                }
                IAsyncResult ar = client.BeginConnect(ipAddress, port, ConnectCallBack, client);
                if (!ar.AsyncWaitHandle.WaitOne(500))
                {
                    TcpClient cl = ar.AsyncState as TcpClient;
                    cl.Client.Close();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ConnectAsync出错:" + ex.StackTrace);
            }

        }
        /// <summary>
        /// 异步连接，连接ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接服务端的端口</param>
        public void ConnectAsync(int port)
        {
            try
            {
                ConnectAsync("127.0.0.1", port);

            }
            catch (Exception ex)
            {
                Console.WriteLine("ConnectAsync出错:" + ex.StackTrace);
            }
        }
        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void ReceiveAsync()
        {
            try
            {
                doReceive.Reset();
                StateObject obj = new StateObject();
                obj.Client = client;
                if (client != null && client.Connected)
                {
                    client.Client.BeginReceive(obj.ListData, 0, obj.ListData.Length, SocketFlags.None, ReceiveCallBack, obj);

                }
                doReceive.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReceiveAsync出错:" +ex.Message+ ex.StackTrace);
            }
            
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(string msg)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    byte[] listData = Encoding.UTF8.GetBytes(msg);
                    client.Client.BeginSend(listData, 0, listData.Length, SocketFlags.None, SendCallBack, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendAsync出错:" + ex.StackTrace);
                Close();
                OnComplete(client, EnSocketAction.Close);

            }
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendHexAsync(string msg)
        {
            try
            {
                if (client.Connected)
                {

                    byte[] listData = DataChange.strToToHexByte(msg);
                    client.Client.BeginSend(listData, 0, listData.Length, SocketFlags.None, SendCallBack, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendHexAsync出错:" + ex.StackTrace);
                Close();
                OnComplete(client, EnSocketAction.Close);

            }
        }

        /// <summary>
        /// 异步连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {

                TcpClient client = ar.AsyncState as TcpClient;
                if (!client.Connected)
                {
                    OnComplete(client, EnSocketAction.Error);
                    return;
                }
                client.EndConnect(ar);
                OnComplete(client, EnSocketAction.Connect);
            }
            catch// (Exception ex)
            {

                //Console.WriteLine("ConnectCallBack出错:" + ex.StackTrace);
                OnComplete(client, EnSocketAction.Error);
            }
        }
        /// <summary>
        /// 异步接收消息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                StateObject obj = ar.AsyncState as StateObject;
                int count = -1;
                try
                {
                    count = obj.Client.Client.EndReceive(ar);
                    doReceive.Set();
                }
                catch (Exception)
                {
                    //如果发生异常，说明客户端失去连接，触发关闭事件
                    Close();
                    OnComplete(obj.Client, EnSocketAction.Close);
                }
                if (count > 0)
                {
                    string msg = Encoding.UTF8.GetString(obj.ListData, 0, count);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (Received != null)
                        {
                            IPEndPoint iep = obj.Client.Client.RemoteEndPoint as IPEndPoint;
                            string key = string.Format("{0}:{1}", iep.Address, iep.Port);
                            Received(key, msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReceiveCallBack出错:" + ex.StackTrace);
            }
           
        }
        private void SendCallBack(IAsyncResult ar)
        {
            
            TcpClient client = ar.AsyncState as TcpClient;
            try
            {
                client.Client.EndSend(ar);
                OnComplete(client, EnSocketAction.SendMsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendCallBack出错:" + ex.StackTrace);
                //如果发生异常，说明客户端失去连接，触发关闭事件
                Close();
                OnComplete(client, EnSocketAction.Close);
            }
        }
        public virtual void OnComplete(TcpClient client, EnSocketAction enAction)
        {
            try
            {
                if (Completed != null)
                    Completed(client, enAction);
                if (enAction == EnSocketAction.Connect)//建立连接后，开始接收数据
                {
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        while (!isClose)
                        {
                            try
                            {
                                Thread.Sleep(20);
                                ReceiveAsync();
                                Thread.Sleep(20);
                            }
                            catch (Exception)
                            {
                                Close();
                                OnComplete(client, EnSocketAction.Close);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnComplete出错:"+ex.StackTrace);
            }
            
        }

        /// <summary>
        /// 断开异步连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void DisConnectCallBack(IAsyncResult ar)
        {
            try
            {
                TcpClient cl = ar.AsyncState as TcpClient;
                //cl.Client.EndConnect(ar);
                //cl.Client.EndDisconnect(ar);
                cl.Client.Close();
            }
            catch(Exception e)
            {

                OnComplete(client, EnSocketAction.Error);
                Console.WriteLine("DisConnectCallBack错误：" + e.StackTrace + e.Message);
            }
        }

        public void Close()
        {
            isClose = true;
           
        }

        /// <summary>
        /// 释放当前client对象
        /// </summary>
        public void Dispoes()
        {
            try
            {
                if (client != null && client.Client != null&&  client.Connected && client.Client.Connected)
                {
                    client.Client.BeginDisconnect(false, DisConnectCallBack, client);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("client.Dispoes()错误：" + e.StackTrace+e.Message);
            }

        }

        /// <summary>
        /// 链接状态
        /// </summary>
        /// <returns></returns>
        public bool Connected()
        {
            try
            {
                return client.Connected;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Connected出错:" + ex.StackTrace);
                return false;
            }

        }



        /// <summary>
        /// 接收socket的行为
        /// </summary>
        public enum EnSocketAction
        {
            /// <summary>
            /// socket发生连接
            /// </summary>
            Connect = 1,
            /// <summary>
            /// socket发送数据
            /// </summary>
            SendMsg = 2,
            /// <summary>
            /// 发生错误
            /// </summary>
            Error = 3,
            /// <summary>
            /// socket关闭
            /// </summary>
            Close = 4
        }
        /// <summary>
        /// 对异步接收时的对象状态的封装，将socket与接收到的数据封装在一起
        /// </summary>
        public class StateObject
        {
            public TcpClient Client { get; set; }
            private byte[] listData = new byte[2048];
            /// <summary>
            /// 接收的数据
            /// </summary>
            public byte[] ListData
            {
                get
                {
                    return listData;
                }
                set
                {
                    listData = value;
                }
            }
            }




    }
}
