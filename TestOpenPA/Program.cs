
using OpenPA;
using OpenPA.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestOpenPA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task t = program();
            t.Wait();

            Console.WriteLine("Good-bye");
        }

        static async Task program()
        {
            PulseAudio.Init();
            MainLoop mainLoop = new MainLoop();
            mainLoop.Start();

            PAContext context = new(mainLoop, "TestApp");
            var t = await context.ConnectAsync("tcp:10.1.10.12");

            Console.WriteLine("Connection succeeded: {0}", t);


            string server = await context.GetServerNameAsync();
            uint server_ver = await context.GetServerProtocolVersionAsync();

            Console.WriteLine("Server: {0}, proto ver {1}", server, server_ver);

            context.Disconnect();
            context.Dispose();

            mainLoop.Stop();
            mainLoop.Dispose();

        }
    }
}
