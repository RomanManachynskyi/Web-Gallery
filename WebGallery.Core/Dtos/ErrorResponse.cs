namespace WebGallery.Core.Dtos;

public sealed class ErrorResponse
{ 
    public string Key { get; set; }

    public string Message { get; set; }

    public string Description { get; set; }

    public ErrorResponse(string key, string message, string description)
    {
        Key = key;
        Message = message;
        Description = description;
    }
};
