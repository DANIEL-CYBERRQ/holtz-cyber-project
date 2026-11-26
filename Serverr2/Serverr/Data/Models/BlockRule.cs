using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serverr.Data.Models;

public class BlockRule
{
    public int Id { get; set; }
    public string Domain { get; set; } = "";   // הדומיין החסום, למשל "youtube.com"

    public int ChildId { get; set; }            // לאיזה ילד שייך החוק
    public Child Child { get; set; } = null!;
}
