using ProtoBuf;

namespace Giphy.Persistance.Helpers;

public static class ProtoSerializer
{
    public static byte[] Serialize<T>(T data)
    {
        var ms = new MemoryStream();
        Serializer.Serialize(ms, data);
        return ms.ToArray();
    }

    public static T? Deserialize<T>(byte[]? data)
    {
        if (data == null) return default;
        var dStream = new MemoryStream(data);
        return Serializer.Deserialize<T>(dStream);
    }
}