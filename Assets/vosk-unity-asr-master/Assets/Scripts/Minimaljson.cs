using System.Collections.Generic;
using System.Text;
public class JSONString
{
    private readonly string _value;
    public JSONString(string value) => _value = value;
    public override string ToString() => "\"" + _value.Replace("\"", "\\\"") + "\"";
}

public class JSONArray
{
    private readonly List<object> _items = new List<object>();
    public void Add(object item) => _items.Add(item);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i < _items.Count; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append(_items[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }
}