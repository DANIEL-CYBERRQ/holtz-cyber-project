using System.Net.Sockets;

var client = new TcpClient("localhost", 6000);
using var stream = client.GetStream();
using var reader = new StreamReader(stream);
using var writer = new StreamWriter(stream) { AutoFlush = true };

// שולח הודעת בדיקה — בקשת החוקים של ילד 1 (מה שהתוכנה באמת תעשה)
writer.WriteLine("{\"type\":\"get_rules\",\"data\":{\"childId\":\"1\"}}");

// קורא את התשובה
string? response = reader.ReadLine();
Console.WriteLine("from server: " + response);

Console.WriteLine("Press Enter to close...");
Console.ReadLine();
