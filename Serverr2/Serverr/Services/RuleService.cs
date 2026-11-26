using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serverr.Data;
using Serverr.Data.Models;
using Serverr.Messages;

namespace Serverr.Services;

public class RuleService
{
    public Message AddRule(Message request)
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

        bool ruleExists = db.BlockRules.Any(r => r.ChildId == childId && r.Domain == domain);
        if (ruleExists)
        {
            return new Message { Type = "error", Data = { ["message"] = "rule already exists" } };
        }

        var rule = new BlockRule { ChildId = childId, Domain = domain };
        db.BlockRules.Add(rule);
        db.SaveChanges();

        return new Message { Type = "ok", Data = { ["message"] = "rule added" } };
    }

    public Message GetRules(Message request)
    {
        string childIdText = request.Data.GetValueOrDefault("childId", "");

        if (!int.TryParse(childIdText, out int childId))
        {
            return new Message { Type = "error", Data = { ["message"] = "invalid childId" } };
        }

        using var db = new AppDbContext();

        // שולף את כל הדומיינים החסומים של הילד
        var domains = db.BlockRules
            .Where(r => r.ChildId == childId)
            .Select(r => r.Domain)
            .ToList();

        return new Message
        {
            Type = "ok",
            Data = { ["domains"] = string.Join(",", domains) }
        };
    }
}
