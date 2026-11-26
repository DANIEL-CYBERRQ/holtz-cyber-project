using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Serverr.Messages;

namespace Serverr.Communication;

public class WebSocketListener
{
    private readonly TcpListener _listener;
    private readonly MessageDispatcher _dispatcher = new MessageDispatcher();

    public WebSocketListener(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine("WebSocket listener started, waiting for browsers...");

        while (true)                                   // ממשיך לקבל לקוחות חדשים
        {
            TcpClient client = _listener.AcceptTcpClient();
            Console.WriteLine("Client connected ✓");
            Task.Run(() => HandleClient(client));      // כל לקוח בנפרד, כדי שלא יחסום אחרים
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            DoHandshake(stream);
            Console.WriteLine("Handshake done ✓");

            while (true)
            {
                string? raw = ReadMessage(stream);
                if (raw == null)
                {
                    Console.WriteLine("Client disconnected");
                    break;
                }
                Console.WriteLine("Received: " + raw);

                Message incoming = Message.FromJson(raw);
                Message response = _dispatcher.Handle(incoming);
                SendMessage(stream, response.ToJson());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Client error: " + ex.Message);   // לא מפיל את השרת
        }
        finally
        {
            client.Close();
        }
    }

    private void DoHandshake(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        string key = Regex.Match(request, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();

        string magic = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        byte[] hash = SHA1.HashData(Encoding.UTF8.GetBytes(key + magic));
        string acceptKey = Convert.ToBase64String(hash);

        string response =
            "HTTP/1.1 101 Switching Protocols\r\n" +
            "Upgrade: websocket\r\n" +
            "Connection: Upgrade\r\n" +
            "Sec-WebSocket-Accept: " + acceptKey + "\r\n\r\n";
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        stream.Write(responseBytes, 0, responseBytes.Length);
    }

    private string? ReadMessage(NetworkStream stream)
    {
        byte[] header = new byte[2];
        int read = stream.Read(header, 0, 2);
        if (read == 0) return null;

        int opcode = header[0] & 0b0000_1111;
        if (opcode == 0x8) return null;              // פריים סגירה → התנתקות

        int payloadLength = header[1] & 0b0111_1111;

        if (payloadLength == 126)
        {
            byte[] ext = new byte[2];
            stream.Read(ext, 0, 2);
            payloadLength = (ext[0] << 8) | ext[1];
        }

        byte[] mask = new byte[4];
        stream.Read(mask, 0, 4);

        byte[] payload = new byte[payloadLength];
        stream.Read(payload, 0, payloadLength);

        for (int i = 0; i < payloadLength; i++)
            payload[i] = (byte)(payload[i] ^ mask[i % 4]);

        return Encoding.UTF8.GetString(payload);
    }

    private void SendMessage(NetworkStream stream, string message)
    {
        byte[] payload = Encoding.UTF8.GetBytes(message);
        byte[] frame;

        if (payload.Length < 126)
        {
            frame = new byte[2 + payload.Length];
            frame[0] = 0x81;
            frame[1] = (byte)payload.Length;
            Array.Copy(payload, 0, frame, 2, payload.Length);
        }
        else
        {
            frame = new byte[4 + payload.Length];
            frame[0] = 0x81;
            frame[1] = 126;
            frame[2] = (byte)((payload.Length >> 8) & 0xFF);
            frame[3] = (byte)(payload.Length & 0xFF);
            Array.Copy(payload, 0, frame, 4, payload.Length);
        }

        stream.Write(frame, 0, frame.Length);
    }
}