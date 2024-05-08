namespace WebGallery.Core.Dtos;

public sealed class HashtagRequest
{
    public string Search { get; set; }
    public uint? TotalUsesFrom { get; set; }
    public uint? TotalUsesTo { get; set; }
    public SortParameters SortParameters { get; set; }
    public PaginationParameters PaginationParameters { get; set; }
}

public sealed class HashtagResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public uint TotalUses { get; set; }
}
