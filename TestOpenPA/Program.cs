
using OpenPA;
using OpenPA.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenPA.Native;

namespace TestOpenPA
{
    class Program
    {
#if DEBUG
        static string? addr = "tcp:10.1.10.102";
#else
        //static string addr = "unix:/run/user/1000/pulse/native";
        static string? addr = null;
#endif
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Connecting to {0}", addr);

            Console.WriteLine("PULSE_SERVER: {0}", Environment.GetEnvironmentVariable("PULSE_SERVER"));

            Task t = program();
            t.Wait();

            Console.WriteLine("Good-bye");
        }

        static async Task program()
        {
            PulseAudio.Init();

            PAContext context = new(MainLoop.Instance, "TestApp");
            var t = await context.ConnectAsync(addr);


            Console.WriteLine("Connection succeeded: {0}", t);
            if (!t)
            {
                MainLoop.Instance.Stop();
                MainLoop.Instance.Dispose();
                return;
            }

            ServerInfo? serverInfo = await context.GetServerInfoAsync();
            if (serverInfo != null)
            {
//#if DEBUG
//#else
//                if (context.SetCardProfile(0, "output:hdmi-stereo"))
//                {
//                    Console.WriteLine("Setting profile success.");
//                }
//                else
//                {
//                    Console.WriteLine("Setting profile failed.");
//                }
//#endif

                Console.WriteLine("Host Name: {0}", serverInfo.HostName);
                Console.WriteLine("User Name: {0}", serverInfo.UserName);
                Console.WriteLine("Server Name: {0}", serverInfo.ServerName);
                Console.WriteLine("Server Version: {0}", serverInfo.ServerVersion);
                Console.WriteLine("Default Source Name: {0}", serverInfo.DefaultSourceName);
                Console.WriteLine("Default Sink Name: {0}", serverInfo.DefaultSinkName);
                Console.WriteLine("Sample Spec: {0} {1} {2}", serverInfo.SampleSpec?.Format, serverInfo.SampleSpec?.Channels, serverInfo.SampleSpec?.Rate);

                if (serverInfo.ChannelMap != null && serverInfo.ChannelMap.Map != null)
                {
                    Console.Write("Channels:");
                    foreach (ChannelPosition channelPosition in serverInfo.ChannelMap.Map)
                    {
                        Console.Write(" {0}", channelPosition);
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("Sinks:");
                var sinks = await context.GetSinkInfoListAsync();
                if (sinks != null)
                {
                    foreach (var sink in sinks)
                    {
                        Console.WriteLine("{0}", sink?.Name);
                    }
                }

                Console.WriteLine();

                Console.WriteLine("Modules:");
                var modules = await context.GetModuleInfoListAsync();
                if (modules != null)
                {
                    foreach (var module in modules)
                    {
                        Console.WriteLine("{0}", module?.Name);
                    }
                }

                Console.WriteLine();

                Console.WriteLine("Clients:");
                var clients = await context.GetClientInfoListAsync();
                if (clients != null)
                {
                    foreach (var client in clients)
                    {
                        Console.WriteLine("{0}", client?.Name);
                    }
                }

                Console.WriteLine();

                Console.WriteLine("Cards:");
                var cards = await context.GetCardInfoListAsync();
                if (cards != null)
                {
                    foreach (var card in cards)
                    {
                        Console.WriteLine("{0}", card?.Name);
                    }
                }

                if (!String.IsNullOrWhiteSpace(serverInfo.DefaultSinkName))
                {
                    SampleSpec sampleSpec = new()
                    {
                        Channels = 1,
                        Format = SampleFormat.S16LE,
                        Rate = 44100,
                    };

                    AudioStream audioStream = AudioStream.CreateMonoStream(context, "MyAudioStream", sampleSpec);
                    audioStream.SetStateCallback((stream) => Console.WriteLine("Stream created."));
                    audioStream.SetStartedCallback((stream) => Console.WriteLine("started"));
                    //await audioStream.ConnectPlaybackAsync(serverInfo.DefaultSinkName, null, StreamFlags.NOFLAGS);
                    await audioStream.ConnectPlaybackAsync(sinks?[0]?.Name ?? serverInfo.DefaultSinkName, null, StreamFlags.NOFLAGS);

                    await Task.Delay(500);
#if DEBUG
                    byte[] buffer = System.IO.File.ReadAllBytes(".\\Assets\\save.snd");
#else
                byte[] buffer = System.IO.File.ReadAllBytes("/usr/share/Assets/save.snd");
#endif
                    for (int i = 0; i < 5; i++)
                    {
                        AudioBuffer? audioBuffer = audioStream.BeginWrite((uint)buffer.Length);
                        if (audioBuffer != null)
                        {
                            audioBuffer.Copy(buffer);
                            audioStream.Write(audioBuffer);
                            await audioStream.DrainAsync();
                        }
                    }

                    audioStream.Disconnect();
                    audioStream.Dispose();
                }
            }
            context.Disconnect();
            context.Dispose();

            MainLoop.Instance.Stop();
            MainLoop.Instance.Dispose();

        }

    }
}
