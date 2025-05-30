using System.Text;
using System.Text.Json;

namespace Connection.Shared;

public class ConnectionPackage
{
    public static ConnectionPackage FromObject(object obj)
    {
        if (obj == null)
            return null;
        try
        {
            var json = JsonSerializer.Serialize(obj);
            if (!string.IsNullOrEmpty(json))
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                return new ConnectionPackage(bytes);
            }
        }
        catch (Exception ex)
        {
            
            Console.WriteLine(ex.Message);
        }

        return null;

    }

    private DateTime timestamp = DateTime.Now;
    private Guid id = Guid.NewGuid();
    private byte[] content;

    public int GetContentSize()
    {
        if (content == null)
            return 0;

        return content.Length;
    }

    public DateTime GetTimestamp()
    {
        return timestamp;
    }

    public Guid GetId()
    {
        return id;
    }

    public string GetShortId()
    {
        return GetId().ToString().Substring(0, 8);
    }

    public byte[] GetContent() => content;

    public T GetContentAs<T>()
    {
        if (content == null)
            return default(T);

        var stringContent = GetContentAsString();
        if (!string.IsNullOrEmpty(stringContent))
        {
            return JsonSerializer.Deserialize<T>(stringContent);
        }

        return default(T);
    }

    public string GetContentAsString()
    {
        var stringResult = string.Empty;
        if (content == null)
            return string.Empty;

        try
        {
            stringResult = Encoding.UTF8.GetString(content);
        }
        catch (Exception ex)
        {
        }

        return stringResult;
    }

    public ConnectionPackage(byte[] byteBase)
    {
        this.content = byteBase;
    }

    public ConnectionPackage()
    {
    }
}