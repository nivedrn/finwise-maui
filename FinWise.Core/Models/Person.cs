namespace FinWise.Core.Models;

/// <summary>Someone who can pay for, share in, or settle expenses.</summary>
public class Person : Entity
{
    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    /// <summary>True for the person who uses this app. At most one person should have this set.</summary>
    public bool IsCurrentUser { get; set; }
}
