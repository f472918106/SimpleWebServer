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
        /// <summary>
        /// SimpleWebServer
        /// 向请求的浏览器返回一个静态HTML页面
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //获取本机IP地址,即127.0.0.1
            IPAddress localaddress = IPAddress.Loopback;

            //创建可以访问的断点,49155表示端口号,如果此时端口号设置为零,系统会默认分配一个空闲端口号
            IPEndPoint endpoint = new IPEndPoint(localaddress, 65000);

            //创建Socket对象,使用IPv4地址,数据通信类型为数据流,传输控制协议为TCP
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //将Socket绑定到断点上
            socket.Bind(endpoint);
            //设置连接队列长度
            socket.Listen(10);

            while(true)
            {
                Console.WriteLine("[" + DateTime.Now + "] Wait an connect Request...");
                //开始监听,阻塞程序线程的执行,直到接收到一个客户端的连接请求
                Socket clientsocket = socket.Accept();

                //输出客户端地址
                Console.WriteLine("Client Address is {0}", clientsocket.RemoteEndPoint);
                //把客户端请求数据读入保存到一个数组中
                byte[] buffer = new byte[2048];

                int receivelength = clientsocket.Receive(buffer, 2048, SocketFlags.None);
                string requeststring = Encoding.UTF8.GetString(buffer, 0, receivelength);

                //在客户端输出请求的消息
                Console.WriteLine(requeststring);

                //服务器做出相应内容
                //响应的状态行
                string statusLine = "HTTP/1.1 200 OK\r\n";
                byte[] responseStatusLineBytes = Encoding.UTF8.GetBytes(statusLine);
                string responseBody = "<html><head><title>Hello World</title></head><body><b>Web Server Running...</b></body></html";
                string responseHeader = string.Format("Content-Type: text/html; charest=UTF-8\r\nContent-Length: {0}\r\n", responseBody.Length);
                byte[] responseHeaderBytes = Encoding.UTF8.GetBytes(responseHeader);
                byte[] responseBodyBytes = Encoding.UTF8.GetBytes(responseBody);

                //向客户端发送状态行
                clientsocket.Send(responseStatusLineBytes);

                //向客户端发送回应头信息
                clientsocket.Send(responseHeaderBytes);

                //向客户端发送头部和内容的空行
                clientsocket.Send(new byte[] { 13, 10 });

                //向客户端发送主体部分
                clientsocket.Send(responseBodyBytes);

                //断开连接
                clientsocket.Close();
                Console.ReadKey();
                break;
            }

            //关闭服务器
            socket.Close();
        }
    }
}
