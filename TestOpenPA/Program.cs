
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

            PAContext context = new(MainLoop.Instance, "TestApp");
            var t = await context.ConnectAsync("tcp:10.1.10.102");
            

            Console.WriteLine("Connection succeeded: {0}", t);
            if (!t)
            {
                MainLoop.Instance.Stop();
                MainLoop.Instance.Dispose();
                return;
            }

            ServerInfo serverInfo = await context.GetServerInfoAsync();
            var sinks = await context.GetSinkInfoAsync(serverInfo.DefaultSinkName);
            Console.WriteLine("Got sink");
            
            var sources = await context.GetSourceInfoListAsync();            
            Console.WriteLine("Got Source");

            context.Disconnect();            
            context.Dispose();

            MainLoop.Instance.Stop();
            MainLoop.Instance.Dispose();

        }
    }
}
