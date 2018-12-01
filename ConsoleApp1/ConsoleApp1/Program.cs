using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


namespace chairbot
{
    public class Chairbot
    {
        // Incoming data from the client.  
        public static string data = null;
        public static string helloworld = "hello world";

        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 3636);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                        byte[] msg = Encoding.ASCII.GetBytes("response");
                        handler.Send(msg);
                        //Console.WriteLine("Text received : {0}", data);
                        Deliver(data);
                    }

                    // Echo the data back to the client.  

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static int Main(String[] args)
        {
            //StartListening();
            Arduino();
            StartListening();
            return 0;
        }
        public static void Deliver(String msg)
        {
            port.WriteLine(msg);
            Console.WriteLine(port.ReadLine());
        }
        static SerialPort port;
        static void Arduino()
        {
            try
            {
                int baud;
                string name;
                Console.WriteLine("Available ports:");
                if (SerialPort.GetPortNames().Count() >= 0)
                {
                    foreach (string p in SerialPort.GetPortNames())
                    {
                        Console.WriteLine(p);
                    }
                }
                else
                {
                    Console.WriteLine("No Ports available, press any key to exit.");
                    Console.ReadLine();
                    // Quit
                    return;
                }
                Console.WriteLine("Port Name:");
                name = Console.ReadLine();
                Console.WriteLine("Baud rate:");
                baud = GetBaudRate();
                BeginSerial(baud, name);
                Console.WriteLine("serial has begun");
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                port.Open();
                Console.WriteLine("serial has been opened");
                //port.WriteLine(Console.ReadLine());
                //Console.WriteLine(port.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            Console.Write(port.ReadExisting());
        }

        static void BeginSerial(int baud, string name)
        {
            port = new SerialPort(name, baud);
        }

        static int GetBaudRate()
        {
            try
            {
                return int.Parse(Console.ReadLine());
            }
            catch
            {
                return GetBaudRate();
            }
        }
    }
}
