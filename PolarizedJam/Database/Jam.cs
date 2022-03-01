using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PolarizedJam.Database;

public class Jam
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = "jsonb")] 
    public JamData Data { get; set; } = new();
}

public class JamData
{
    public IList<Shape> Shapes { get; set; } = new List<Shape>();
}

public class Shape
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public int X { get; set; }
    public int Y { get; set; }
}
