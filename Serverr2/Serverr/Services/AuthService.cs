using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serverr.Data;
using Serverr.Data.Models;
using Serverr.Messages;

namespace Serverr.Services;

public class AuthService
{
    public Message Register(Message request)
    {
        string name = request.Data.GetValueOrDefault("name", "");
        string email = request.Data.GetValueOrDefault("email", "");
        string password = request.Data.GetValueOrDefault("password", "");

        using var db = new AppDbContext();

        bool exists = db.Parents.Any(p => p.Email == email);
        if (exists)
            return new Message { Type = "error", Data = { ["message"] = "email already registered" } };

        string hashed = BCrypt.Net.BCrypt.HashPassword(password);

        var parent = new Parent { Name = name, Email = email, Password = hashed };
        db.Parents.Add(parent);
        db.SaveChanges();

        return new Message
        {
            Type = "ok",
            Data =
        {
            ["message"] = "registered successfully",
            ["name"] = parent.Name,
            ["parentId"] = parent.Id.ToString()
        }
        };
    }

    public Message Login(Message request)
    {
        string email = request.Data.GetValueOrDefault("email", "");
        string password = request.Data.GetValueOrDefault("password", "");

        using var db = new AppDbContext();

        // מוצאים לפי אימייל, ואז מאמתים את הגיבוב
        Parent? parent = db.Parents.FirstOrDefault(p => p.Email == email);
        if (parent == null || !BCrypt.Net.BCrypt.Verify(password, parent.Password))
            return new Message { Type = "error", Data = { ["message"] = "invalid email or password" } };

        return new Message
        {
            Type = "ok",
            Data =
            {
                ["message"] = "login successful",
                ["name"] = parent.Name,
                ["parentId"] = parent.Id.ToString()
            }
        };
    }

    public Message PairDevice(Message request)
    {
        string pairId = request.Data.GetValueOrDefault("pairId", "");
        string email = request.Data.GetValueOrDefault("email", "");
        string password = request.Data.GetValueOrDefault("password", "");

        using var db = new AppDbContext();

        Parent? parent = db.Parents.FirstOrDefault(p => p.Email == email);
        if (parent == null || !BCrypt.Net.BCrypt.Verify(password, parent.Password))
            return new Message { Type = "error", Data = { ["message"] = "invalid email or password" } };

        Child? child = db.Children.FirstOrDefault(c => c.PairId == pairId && c.ParentId == parent.Id);
        if (child == null)
            return new Message { Type = "error", Data = { ["message"] = "invalid pairId for this parent" } };

        string token = Guid.NewGuid().ToString("N");
        child.Token = token;
        db.SaveChanges();

        return new Message
        {
            Type = "ok",
            Data = { ["message"] = "paired successfully", ["token"] = token, ["childId"] = child.Id.ToString() }
        };
    }
}