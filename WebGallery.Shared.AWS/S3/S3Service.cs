using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using WebGallery.Shared.AWS.IAM;

namespace WebGallery.Shared.AWS.S3;

public interface IS3Service
{
    Task<string> ProcessFile(string folderPath, string fileName);
    Task<List<string>> ProcessFolder(string path, int limit, int offset);
    Task PublishFile(string fileName, string path, IFormFile content);
    Task PublishFile(string fileName, string path, Stream stream, string contentType);
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

    public async Task<string> ProcessFile(string folderPath, string fileName)
    {
        var filePath = $"{folderPath}/{fileName}";
        var content = await ProcessFile(filePath);

        return content;
    }

    public async Task<List<string>> ProcessFolder(string path, int limit, int offset)
    {
        var request = new ListObjectsV2Request { BucketName = _s3Config.BucketName, Prefix = $"{path}/" };

        var response = await _s3Client.ListObjectsV2Async(request);
        var jsonFiles = new List<string>();
        var filesProcessed = 0;

        var objectsToProcess = response.S3Objects
            .Where(s3object => !ShouldSkipObject(s3object, ref filesProcessed, ref offset, limit));

        foreach (var s3Object in objectsToProcess)
        {
            var content = await ProcessFile(s3Object.Key);
            jsonFiles.Add(content);
        }

        return jsonFiles;
    }

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
