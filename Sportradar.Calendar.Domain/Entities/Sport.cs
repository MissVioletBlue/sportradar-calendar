using System.ComponentModel.DataAnnotations;

namespace Sportradar.Calendar.Domain.Entities;

// sport entity is tiny but i still explain it so future me remember stuff
public sealed class Sport
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [StringLength(60)]
    public string Name { get; private set; }

    // ctor for loading existing sport with explicit id (like seeds)
    public Sport(int id, string name)
    {
        Id = id;
        Name = name.Trim();
    }

    // ctor for creating new sport from user input, db will fill id later
    public Sport(string name)
    {
        Name = name.Trim();
    }

    // ef core asks for empty ctor, so i give one but keep name safe
    private Sport()
    {
        Name = string.Empty;
    }
}