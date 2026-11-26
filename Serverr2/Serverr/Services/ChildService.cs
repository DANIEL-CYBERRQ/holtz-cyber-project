using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serverr.Data;
using Serverr.Data.Models;
using Serverr.Messages;

namespace Serverr.Services;

public class ChildService
{
    private static readonly Random _random = new Random();

    public Message Create(Message request)
    {
        string name = request.Data.GetValueOrDefault("name", "");
        string parentIdText = request.Data.GetValueOrDefault("parentId", "");

        // ממיר את ה-parentId למספר
        if (!int.TryParse(parentIdText, out int parentId))
        {
            return new Message
            {
                Type = "error",
                Data = { ["message"] = "invalid parentId" }
            };
        }

        using var db = new AppDbContext();

        // ודא שההורה קיים
        bool parentExists = db.Parents.Any(p => p.Id == parentId);
        if (!parentExists)
        {
            return new Message
            {
                Type = "error",
                Data = { ["message"] = "parent not found" }
            };
        }

        // מייצר PairId ייחודי
        string pairId = GeneratePairId();
        while (db.Children.Any(c => c.PairId == pairId))
            pairId = GeneratePairId();

        // יוצר ושומר את הילד
        var child = new Child
        {
            Name = name,
            PairId = pairId,
            ParentId = parentId
        };
        db.Children.Add(child);
        db.SaveChanges();

        return new Message
        {
            Type = "ok",
            Data =
            {
                ["message"] = "child created",
                ["pairId"] = pairId
            }
        };
    }

    private string GeneratePairId()
    {
        // בלי תווים מבלבלים (0/O, 1/I)
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        char[] code = new char[6];
        for (int i = 0; i < code.Length; i++)
            code[i] = chars[_random.Next(chars.Length)];
        return new string(code);
    }
}