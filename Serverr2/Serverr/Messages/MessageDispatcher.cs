using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serverr.Services;

namespace Serverr.Messages;

public class MessageDispatcher
{
    private readonly AuthService _auth = new AuthService();
    private readonly ChildService _childService = new ChildService();
    private readonly RuleService _ruleService = new RuleService();
    private readonly LogService _logService = new LogService();

    public Message Handle(Message message)
    {
        switch (message.Type)
        {
            case "register":
                return _auth.Register(message);

            case "login":
                return _auth.Login(message);

            case "create_child":
                return _childService.Create(message);

            case "add_rule":
                return _ruleService.AddRule(message);

            case "get_rules":
                return _ruleService.GetRules(message);

            case "log":
                return _logService.AddLog(message);

            case "get_logs":
                return _logService.GetLogs(message);

            case "pair":
                return _auth.PairDevice(message);
            default:
                return new Message
                {
                    Type = "error",
                    Data = { ["message"] = "unknown type: " + message.Type }
                };
        }
    }
}