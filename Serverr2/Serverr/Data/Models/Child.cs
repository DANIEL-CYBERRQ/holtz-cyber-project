using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serverr.Data.Models;

public class Child
{
    public int Id { get; set; }                 // מפתח ראשי פנימי
    public string PairId { get; set; } = "";    // ה-ID שההורה מקבל ומקליד בתוכנה
    public string Name { get; set; } = "";      // כדי להבדיל בין ילדים
    public string? Token { get; set; }          // ניתן אחרי צימוד; ריק עד אז

    public int ParentId { get; set; }           // לאיזה הורה שייך
    public Parent Parent { get; set; } = null!;

    public List<BlockRule> BlockRules { get; set; } = new();
    public List<BrowsingLog> BrowsingLogs { get; set; } = new();
}
