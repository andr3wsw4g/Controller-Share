using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using vJoyInterfaceWrap;

namespace Controller_Share
{
    
    public struct Message
    {
        public Message(DateTime a, int b)
        {
            data = b;
            time = a;
        }
        public int data { get; private set; }
        public DateTime time { get; private set; }
    }
    public class ControllerConnection
    {
        //If you are connecting to an external port
        public ControllerConnection(string ip, Int32 port, SocketType a)
        {
            switch (a)
            {
                case SocketType.Dgram:
                    iType = ProtocolType.Udp;
                    break;
                case SocketType.Raw:
                    iType = ProtocolType.Icmp;
                    break;
                case SocketType.Rdm:
                    iType = ProtocolType.Udp;
                    break;
                case SocketType.Seqpacket:
                    iType = ProtocolType.Udp;
                    break;
                case SocketType.Stream:
                    iType = ProtocolType.Tcp;
                    break;
                case SocketType.Unknown:
                    break;
                default:
                    break;

            }
            iSHistory = new List<Message>();
            iRHistory = new List<Message>();
            try
            {
                iAddress = IPAddress.Parse(ip);
                Console.WriteLine(this.iAddress.ToString());
                this.iEP = new IPEndPoint(iAddress, port);
                Console.WriteLine(this.iEP.ToString());
                Console.WriteLine(iAddress.AddressFamily);
                Console.WriteLine(SocketType.Stream);
                Console.WriteLine(ProtocolType.Tcp);
                this.iSocket = new Socket(iEP.AddressFamily, a, iType);
                try
                {
                    iSocket.Connect(iEP);
                    Console.WriteLine("Socket Connected to {0}", iSocket.RemoteEndPoint.ToString());                    
                    //
                    sMessage = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    Message s = new Message(System.DateTime.Now, iSocket.Send(sMessage));
                    iSHistory.Add(s);
                    Message r = new Message(System.DateTime.Now, iSocket.Receive(rMessage));

                    iRHistory.Add(r);
                    Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(rMessage, 0, iRHistory[0].data));

                    // Release the socket.  
                    iSocket.Shutdown(SocketShutdown.Both);
                    iSocket.Close();
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e) {  
            Console.WriteLine(e.ToString());  
            }
        }
        
        public byte[] sMessage { get; private set; }
        public byte[] rMessage { get; private set; }
        public Int32 iPort { get; private set; }
        public List<Message> iSHistory { get; private set; }
        public List<Message> iRHistory { get; private set; }
        public IPHostEntry iHost { get; private set; }
        public IPAddress iAddress { get; private set; }
        public Socket iSocket { get; private set; }
        public IPEndPoint iEP { get; private set; }
        public ProtocolType iType { get; private set; }

    }
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    public class ControllerHost
    {
        public IPAddress getIPv4(IPAddress[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].AddressFamily == AddressFamily.InterNetwork)
                    return a[i];
            }
            return IPAddress.Any;
        }
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public byte[] sMessage { get; private set; }
        public byte[] rMessage { get; private set; }
        public Int32 iPort { get; private set; }
        public List<Message> iSHistory { get;  private set; }
        public List<Message> iRHistory { get;  private set; }
        public IPHostEntry iHost { get; private set; }
        public IPAddress iAddress { get; private set; }
        public Socket iSocket { get; private set; }
        public IPEndPoint iEP { get; private set; }
        public ProtocolType iType { get; private set; }
        public Dictionary<IPEndPoint,vJoy> controllers= new Dictionary<IPEndPoint, vJoy>();
        public void Listen(Int32 port, SocketType a)
        {
            switch (a)
            {
                case SocketType.Dgram:
                    iType = ProtocolType.Udp;
                    break;
                case SocketType.Raw:
                    iType = ProtocolType.Icmp;
                    break;
                case SocketType.Rdm:
                    iType = ProtocolType.Udp;
                    break;
                case SocketType.Seqpacket:
                    iType = ProtocolType.Udp;
                    break;
                case SocketType.Stream:
                    iType = ProtocolType.Tcp;
                    break;
                case SocketType.Unknown:
                    break;
                default:
                    break;

            }
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            iHost = Dns.GetHostEntry(string.Empty);
            iAddress = getIPv4(iHost.AddressList);
            iEP = new IPEndPoint(iAddress, port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(iAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(iEP);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }
        
        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            //create the XInput controller
            controllers.Add(((IPEndPoint)handler.RemoteEndPoint),new vJoy());
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf(((char)26)) > -1)
                {
                    //grab the vJoy by the IPEndpoint
                    vJoy current_controller = controllers[(IPEndPoint)handler.RemoteEndPoint];
                    
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.  
                    Send(handler, "You sent: " + content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
