namespace WebGallery.Core.Exceptions;

public class SpecificationBuilderException : Exception
{
    public string Description { get; set; }

    public SpecificationBuilderException(string message, string description) : base(message)
    {
        Description = description;
    }
}
