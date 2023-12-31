using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MediaMirror {
    class ASyncSocketClient {
        // The port number for the remote device.  
        private const int port = 11000;
        MediaMirrorContext ctx;
        // ManualResetEvent instances signal completion.  
        private ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        public ASyncSocketClient(MediaMirrorContext ctx){
            this.ctx = ctx;
        }
        // The response from the remote device.  
        private static Music response = null;

        public void StartClient() {
            // Connect to a remote device.  
            while (true) {
                try {
                    // Establish the remote endpoint for the socket.  
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddress = ipHostInfo.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                    ctx.serverIP = "Connecting to " + ipAddress.MapToIPv4().ToString() + ":11000";
                    Console.WriteLine("Attempting to connect to " + ipAddress.MapToIPv4().ToString() + ":11000");
                    // Create a TCP/IP socket.  
                    Socket client = new Socket(ipAddress.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                    // Connect to the remote endpoint.  
                    client.BeginConnect(remoteEP,
                        new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();

                    // Send test data to the remote device.  
                    Send(client, "yo gimme some of that gud music<EOF>");
                    sendDone.WaitOne();

                    // Receive the response from the remote device.  
                    Receive(client);
                    receiveDone.WaitOne();
                    
                    ctx.currentSong = response;
                    ctx.songUpdated = true;
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                    return;
                }
                Thread.Sleep(100);
            }
           
        }

        public Music GetMusic() {
            return ctx.currentSong;
        }

        private void ConnectCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket client) {
            try {
                // Create the state object.  
                StatefulMusic state = new StatefulMusic();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StatefulMusic.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar) {
            try {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StatefulMusic state = (StatefulMusic)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0) {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StatefulMusic.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                } else {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1) {
                        /*BinaryFormatter formatter = new BinaryFormatter();
                        using (var ms = new MemoryStream(state.buffer)) { 
                            ms.Position = 0;
                            response = (Music)formatter.Deserialize(ms);
                            
                        }*/
                        string serialized_data = state.sb.ToString();
                        Console.WriteLine(serialized_data);
                        response = JsonConvert.DeserializeObject<Music>(serialized_data);
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, String data) {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
