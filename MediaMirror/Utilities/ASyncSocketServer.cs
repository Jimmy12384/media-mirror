using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MediaMirror {
    public class StatefulMusic {
        //client socket
        public Socket workSocket = null;
        //size of receiving buffer
        public const int BufferSize = 8192;
        //receiving buffer
        public byte[] buffer = new byte[BufferSize];
        //received data
        public StringBuilder sb = new StringBuilder();
    }

    class ASyncSocketListener {
        static MediaMirrorContext ctx;
        //Thread signal
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static Music music = null;
        private readonly IPAddress ip;

        public ASyncSocketListener(MediaMirrorContext ctx, IPAddress ip) {
            ASyncSocketListener.ctx = ctx;
            this.ip = ip;
        }

        public void StartListening() {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer    
            int port = 11000;

            IPEndPoint localEndPoint = new IPEndPoint(ip, port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ip.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true) {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    ctx.serverIP = ip.MapToIPv4().ToString() + ":" + port;
                    Console.WriteLine("Waiting for a connection at " + ip.MapToIPv4().ToString() + ":" + port + "...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
        public static void AcceptCallback(IAsyncResult ar) {    //called whenever a client connects to the server socket
                                                                // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StatefulMusic state = new StatefulMusic();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StatefulMusic.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        public static void ReadCallback(IAsyncResult ar) {  //called whenever a request is made
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StatefulMusic state = (StatefulMusic)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for eof tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1) {
                    // All the data has been read from the
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client. 
                    if (ctx.currentSong == null) {
                        Console.WriteLine("host currently inactive, waiting...");
                        Send(handler, new Music());
                    } else {
                        ctx.currentSong.packet = SerializeAndTrim(ctx.currentSong.uri);
                        Send(handler, ctx.currentSong);
                        music = ctx.currentSong;
                    }
                } else {
                    // Not all data received. Get more data  
                    handler.BeginReceive(state.buffer, 0, StatefulMusic.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
        public static byte[] SerializeAndTrim(string filename) {
            byte[] packet = new byte[ctx.currentSong.bitrate * 1000];
            using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                int offset = Convert.ToInt32(ctx.currentSong.bitrate * (ctx.currentSong.current_elapsed_time));
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                reader.Read(packet, 0, ctx.currentSong.bitrate * 1000);
                Console.WriteLine("Bitrate: " + ctx.currentSong.bitrate + "; Elapsed Time(secs): " + ctx.currentSong.current_elapsed_time + "Index: " + ctx.currentSong.bitrate * (ctx.currentSong.current_elapsed_time / 1000));
                Console.WriteLine("packet len: " + packet.Length);
                Console.WriteLine("Finishing Serializing music to byte array");
            }
            return packet;
        }
        private static void Send(Socket handler, Music data) {
            byte[] byteData;
            string serialized_data;
            serialized_data = JsonConvert.SerializeObject(data, Formatting.Indented);
            byteData = Encoding.ASCII.GetBytes(serialized_data);
            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }
        private static void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
                handler.Close();

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public Music getMusic() {
            return music;
        }
    }
}
