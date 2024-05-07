using System.ComponentModel.DataAnnotations;

namespace WebGallery.Core.Attributes;

public sealed class ListRequestLimitAttribute : RangeAttribute
{
    private const int MaxLimit = 100;
    private const int MinLimit = 1;

    public ListRequestLimitAttribute() : base(MinLimit, MaxLimit) { }
}

public sealed class ListRequestOffsetAttribute : RangeAttribute
{
    private const int MinOffset = 0;

    public ListRequestOffsetAttribute() : base(MinOffset, int.MaxValue) { }
}
