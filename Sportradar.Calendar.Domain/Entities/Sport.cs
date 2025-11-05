using System.ComponentModel.DataAnnotations;

namespace Sportradar.Calendar.Domain.Entities;

public sealed class Sport
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [StringLength(60)]
    public string Name { get; private set; }

    public Sport(int id, string name)
    {
        Id = id;
        Name = name.Trim();
    }

    public Sport(string name)
    {
        Name = name.Trim();
    }

    private Sport()
    {
        Name = string.Empty;
    }
}