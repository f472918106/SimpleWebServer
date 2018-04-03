using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SimpleWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //获取本机IP地址,即127.0.0.1
            IPAddress Localaddress = IPAddress.Loopback;

            //创建可以访问的断点
            IPEndPoint endpoint = new IPEndPoint(Localaddress, 65000);

            //创建Tcp监听器
            TcpListener tcplistener = new TcpListener(endpoint);

            //启动监听
            tcplistener.Start();
            Console.WriteLine("[" + DateTime.Now + "]" + "Wait an connect Request...");
            while(true)
            {
                TcpClient client = tcplistener.AcceptTcpClient();
                if(client.Connected == true)
                {
                    Console.WriteLine("[" + DateTime.Now + "]" + "Created connection");
                }

                //获得一个网络流对象
                //该网络流对象封装了Socket的输入和输出操作
                //此时通过对网络流对象进行写入来返回响应消息
                //通过对网络流对象进行读取来获得请求消息
                NetworkStream netstream = client.GetStream();
                //把客户端请求读入保存到一个数组中
                byte[] buffer = new byte[2048];

                int receivelength = netstream.Read(buffer, 0, 2048);
                string requeststring = Encoding.UTF8.GetString(buffer, 0, receivelength);

                //在服务器输出请求消息
                Console.WriteLine(requeststring);

                //服务器做出响应内容
                //响应的状态行
                string statusLine = "HTTP/1.1 200 OK\r\n";
                byte[] responseStatusLineBytes = Encoding.UTF8.GetBytes(statusLine);
                string responseBody = "<html><head><title>Hello World</title></head><body><b>Web Server Running...</b></body></html";
                string responseHeader = string.Format("Content-Type: text/html; charest=UTF-8\r\nContent-Length: {0}\r\n", responseBody.Length);
                byte[] responseHeaderBytes = Encoding.UTF8.GetBytes(responseHeader);
                byte[] responseBodyBytes = Encoding.UTF8.GetBytes(responseBody);

                //写入状态行信息
                netstream.Write(responseStatusLineBytes, 0, responseStatusLineBytes.Length);
                //写入回应的头部
                netstream.Write(responseHeaderBytes, 0, responseHeaderBytes.Length);
                //写入回应头部和内容之间的空行
                netstream.Write(new byte[] { 13, 10 }, 0, 2);

                //写入回应的内容
                netstream.Write(responseBodyBytes, 0, responseBodyBytes.Length);

                //关闭客户端连接
                client.Close();
                Console.ReadKey();
                break;
            }

            //关闭服务器
            tcplistener.Stop();
        }
    }
}
