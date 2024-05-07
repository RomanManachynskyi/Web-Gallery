namespace WebGallery.Shared.AWS.S3;

public interface IS3Config
{
    string BucketName { get; set; }
}

public class S3Config : IS3Config
{
    public string BucketName { get; set; } = string.Empty;
}
