using Microsoft.EntityFrameworkCore;
using Serverr.Data;

using Serverr.Communication;

var ws = new WebSocketListener(5000);
var tcp = new TcpServer(6000);

Task.Run(() => tcp.Start());   // TCP רץ ברקע (פורט 6000)
ws.Start();                     // WebSocket על ה-thread הראשי (פורט 5000)