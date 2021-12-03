using ProtoBuf;

namespace Giphy.Domain.Entities;

[ProtoContract]
public class Gif
{
    [ProtoMember(1)] public string Id { get; set; } = null!;
    [ProtoMember(2)] public string Type { get; set; } = null!;
    [ProtoMember(3)] public string Url { get; set; } = null!;
    [ProtoMember(4)] public string Slug { get; set; } = null!;
    [ProtoMember(5)] public string Title { get; set; } = null!;
}