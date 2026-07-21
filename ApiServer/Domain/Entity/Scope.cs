namespace Trustedbits.ApiServer.Domain.Entity;

public class Scope
{
    public const int MaxNameLength = 64;
    public const int MaxValueLength = 64;
    public const int MaxDescriptionLength = 128;
    
    public Guid Id
    {
        get => field;
        set
        {
            if(value == Guid.Empty)
                throw new ArgumentException("Scope ID cannot be empty or zeroed", nameof(value));
            field = value;
        }
    }

    public string Name
    {
        get => field;
        set 
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("You must assign a non-whitespace string to the scope name", nameof(value));
            if(value.Length > MaxNameLength)
                throw new ArgumentException($"Scope name length exceeded (max is {MaxNameLength})", nameof(value));
            field = value;
            NormalizedName = value.ToUpperInvariant();
        }
    }
    public string NormalizedName 
    {
        get => field;
        private set;
   }

    public string Value
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("You must assign a non-whitespace string to the scope value", nameof(value));
            if(value.Length > MaxValueLength)
                throw new ArgumentException($"Scope value length exceeded (max is {MaxValueLength})", nameof(value));
            field = value;
        }
    }

    public string Description
    {
        get => field;
        set
        {
            if (value is { Length: > MaxDescriptionLength })
                throw new ArgumentException($"Scope description length exceeded (max is {MaxDescriptionLength})", nameof(value));
            field = value;
        }
    }
    
    public Scope(Guid id, string name, string value, string description)
    {
        Id = id;
        Name = name;
        NormalizedName = name.ToUpper();
        Value = value;
        Description = description;
    }
    public Scope(string name, string value, string description)
        : this(Guid.NewGuid(), name, value, description) { }
    private Scope() { }
}