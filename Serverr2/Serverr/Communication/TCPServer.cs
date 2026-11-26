using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using Serverr.Messages;

namespace Serverr.Communication;

public class TcpServer
{
    private readonly TcpListener _listener;
    private readonly MessageDispatcher _dispatcher = new MessageDispatcher();

    public TcpServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine("TCP listener started, waiting for software...");

        while (true)
        {
            TcpClient client = _listener.AcceptTcpClient();
            Console.WriteLine("Software connected ✓");
            Task.Run(() => HandleClient(client));
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            string? line;
            while ((line = reader.ReadLine()) != null)   // קורא הודעה (עד שורה חדשה)
            {
                Console.WriteLine("Received (TCP): " + line);

                Message incoming = Message.FromJson(line);
                Message response = _dispatcher.Handle(incoming);
                writer.WriteLine(response.ToJson());       // עונה (עם שורה חדשה)
            }
            Console.WriteLine("Software disconnected");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Software error: " + ex.Message);
        }
        finally
        {
            client.Close();
        }
    }
}