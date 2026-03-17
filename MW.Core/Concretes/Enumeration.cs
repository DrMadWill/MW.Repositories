using System.Reflection;

namespace MW.Core.Concretes;

public abstract class Enumeration(int id, string name) : IComparable
{

    public string Name { get; set; } = name;
    public int Id { get; set; } = id;

    public override string ToString() => Name;

    private static readonly Dictionary<Type, IEnumerable<Enumeration>> _cache = new();

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var type = typeof(T);
        if (_cache.TryGetValue(type, out var cached))
            return cached.Cast<T>();

        var fields = type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f.FieldType == type)
            .Select(f => f.GetValue(null))
            .Cast<Enumeration>()
            .ToList();

        _cache.TryAdd(type, fields); 
        return fields.Cast<T>();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
            return false;

        var typeMatches = GetType().Equals(otherValue.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }
    

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        => Math.Abs(firstValue.Id - secondValue.Id);

    public static T FromValue<T>(int id) where T : Enumeration
        => Parse<T, int>(id, "display name", s => s.Id == id);
    
    public static T FromDisplayName<T>(string displayName) where T : Enumeration
        => Parse<T, string>(displayName, "display name", s => s.Name == displayName);

    protected static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        var matchItem = GetAll<T>().FirstOrDefault(predicate);
        if (matchItem == null)
            throw new InvalidOperationException($"'{value}' is not valid {description} in {typeof(T)}");
        return matchItem;
    }
    
    public int CompareTo(object? other)
    {
        if (other is not Enumeration otherEnum)
            throw new ArgumentException($"Object must be of type {nameof(Enumeration)}");
        return Id.CompareTo(otherEnum.Id);
    }
}