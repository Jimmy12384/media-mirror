using System;
using System.Threading;

namespace MediaMirror {
    public class MusicFetcher {
        readonly MediaMirrorContext ctx;
        Thread client_thread;
        public MusicFetcher(MediaMirrorContext ctx, Thread thread) {
            Console.WriteLine("Fetcher created");
            client_thread = thread;
            this.ctx = ctx;
        }

        public Music fetchMusic() {
            Console.WriteLine("Creating client");
            ASyncSocketClient client = new ASyncSocketClient(ctx);
            client_thread = new Thread(new ThreadStart(client.StartClient));
            client_thread.Start();
            while(client.GetMusic() == null);
            
            return client.GetMusic();
        }
    }
    
}
