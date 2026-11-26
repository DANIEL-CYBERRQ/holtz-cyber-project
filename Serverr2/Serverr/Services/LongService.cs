using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serverr.Data;
using Serverr.Data.Models;
using Serverr.Messages;

namespace Serverr.Services;

public class LogService
{
    public Message AddLog(Message request)
    {
        string childIdText = request.Data.GetValueOrDefault("childId", "");
        string domain = request.Data.GetValueOrDefault("domain", "");

        if (!int.TryParse(childIdText, out int childId))
        {
            return new Message { Type = "error", Data = { ["message"] = "invalid childId" } };
        }

        using var db = new AppDbContext();

        bool childExists = db.Children.Any(c => c.Id == childId);
        if (!childExists)
        {
            return new Message { Type = "error", Data = { ["message"] = "child not found" } };
        }

        var log = new BrowsingLog
        {
            ChildId = childId,
            Domain = domain,
            VisitedAt = DateTime.UtcNow   // UTC! (דרישת Npgsql, זוכר?)
        };
        db.BrowsingLogs.Add(log);
        db.SaveChanges();

        return new Message { Type = "ok", Data = { ["message"] = "log saved" } };
    }

    public Message GetLogs(Message request)
    {
        string childIdText = request.Data.GetValueOrDefault("childId", "");

        if (!int.TryParse(childIdText, out int childId))
        {
            return new Message { Type = "error", Data = { ["message"] = "invalid childId" } };
        }

        using var db = new AppDbContext();

        var logs = db.BrowsingLogs
            .Where(l => l.ChildId == childId)
            .OrderByDescending(l => l.VisitedAt)
            .Select(l => l.Domain + " @ " + l.VisitedAt.ToString("yyyy-MM-dd HH:mm"))
            .ToList();

        return new Message
        {
            Type = "ok",
            Data = { ["logs"] = string.Join(" | ", logs) }
        };
    }
}