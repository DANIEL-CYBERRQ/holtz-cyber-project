using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serverr.Data.Models;

public class Parent
{
    public int Id { get; set; }              // מפתח ראשי, EF ייצר אוטומטית

    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    // כל הילדים של ההורה הזה
    public List<Child> Children { get; set; } = new();
}