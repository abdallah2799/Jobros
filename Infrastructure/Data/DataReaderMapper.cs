// Infrastructure/Data/MapReaderToList.cs
using System.Data;
using System.Data.Common;
using System.Reflection;

public static class DataReaderMapper
{
    public static List<T> MapReaderToList<T>(DbDataReader reader) where T : new()
    {
        var results = new List<T>();
        if (reader == null) return results;

        // Build column -> ordinal map (case-insensitive)
        var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < reader.FieldCount; i++)
            columnMap[reader.GetName(i)] = i;

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.CanWrite).ToArray();

        while (reader.Read())
        {
            var item = new T();
            foreach (var prop in props)
            {
                if (!columnMap.TryGetValue(prop.Name, out var ordinal))
                {
                    // Try snake_case fallback (optional)
                    var snake = ToSnakeCase(prop.Name);
                    if (!columnMap.TryGetValue(snake, out ordinal))
                        continue;
                }

                if (reader.IsDBNull(ordinal)) continue;
                var val = reader.GetValue(ordinal);

                try
                {
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    var safeVal = Convert.ChangeType(val, targetType);
                    prop.SetValue(item, safeVal);
                }
                catch
                {
                    // ignore conversion errors — alternatively, log or throw
                }
            }
            results.Add(item);
        }

        return results;
    }

    private static string ToSnakeCase(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return name;
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            var ch = name[i];
            if (char.IsUpper(ch) && i > 0) sb.Append('_');
            sb.Append(char.ToLowerInvariant(ch));
        }
        return sb.ToString();
    }
}
