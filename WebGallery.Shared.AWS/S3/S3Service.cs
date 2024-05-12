using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using WebGallery.Shared.AWS.IAM;

namespace WebGallery.Shared.AWS.S3;

public interface IS3Service
{
    Task PublishFile(string fileName, string path, IFormFile content);
    Task PublishFile(string fileName, string path, Stream stream, string contentType);
    Task DeleteFile(string path, string name);
}

public sealed class S3Service : IS3Service
{
    private readonly IS3Config _s3Config;
    private readonly AmazonS3Client _s3Client;

    public S3Service(IS3Config s3Config, IIamConfig iamConfig)
    {
        _s3Config = s3Config;
        _s3Client = new AmazonS3Client(
            iamConfig.AccessKeyId, iamConfig.SecretAccessKey, iamConfig.GetRegionEndpoint());
    }

    #region Implementation

    public async Task PublishFile(string fileName, string path, IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        memoryStream.Position = 0;

        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _s3Config.BucketName,
            Key = $"{path}/{fileName}",
            InputStream = memoryStream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(putObjectRequest);
    }

    public async Task PublishFile(string fileName, string path, Stream stream, string contentType)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _s3Config.BucketName,
            Key = $"{path}/{fileName}",
            InputStream = stream,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(putObjectRequest);
    }

    public async Task DeleteFile(string path, string name)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _s3Config.BucketName,
            Key = $"{path}/{name}"
        };

        await _s3Client.DeleteObjectAsync(deleteObjectRequest);
    }

    #endregion

    #region Helper Methods

    private async Task<string> ProcessFile(string folderPath)
    {
        var getObjectRequest = GetObjectRequest(folderPath);

        using var getObjectResponse = await _s3Client.GetObjectAsync(getObjectRequest);
        await using var responseStream = getObjectResponse.ResponseStream;
        using var reader = new StreamReader(responseStream);
        var content = await reader.ReadToEndAsync();

        return content;
    }

    private static bool ShouldSkipObject(S3Object obj, ref int filesProcessed, ref int offset, int limit)
    {
        if (obj.Key.EndsWith("/"))
            return true;

        if (filesProcessed >= limit)
            return true;

        if (offset > 0)
        {
            offset--;
            return true;
        }

        filesProcessed++;
        return false;
    }

    private GetObjectRequest GetObjectRequest(string path) => new() { BucketName = _s3Config.BucketName, Key = path };

    #endregion
}
