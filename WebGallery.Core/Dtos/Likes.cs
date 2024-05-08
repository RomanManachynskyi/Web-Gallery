using NodaTime;

namespace WebGallery.Core.Dtos;

public sealed class LikesRequest
{
    public string Search { get; set; }
    public LocalDate? PublishedFrom { get; set; }
    public LocalDate? PublishedTo { get; set; }
    public SortParameters SortParameters { get; set; }
    public PaginationParameters PaginationParameters { get; set; }
}
