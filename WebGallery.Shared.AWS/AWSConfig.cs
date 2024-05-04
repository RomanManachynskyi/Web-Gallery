using Amazon;

namespace WebGallery.Shared.AWS;

public interface IAWSConfig
{
    string RegionEndpoint { get; }
}

public static class AwsConfig
{
    public static RegionEndpoint GetRegionEndpoint(this IAWSConfig config)
    {
        if (string.IsNullOrEmpty(config.RegionEndpoint))
            return RegionEndpoint.EUNorth1; // Stockholm

        return config.RegionEndpoint.ToLower().Trim() switch
        {
            "af-south-1" => RegionEndpoint.AFSouth1,
            "me-south-1" => RegionEndpoint.MESouth1,
            "ca-central-1" => RegionEndpoint.CACentral1,
            "cn-north-1" => RegionEndpoint.CNNorth1,
            "us-gov-west-1" => RegionEndpoint.USGovCloudWest1,
            "us-gov-east-1" => RegionEndpoint.USGovCloudEast1,
            "ap-southeast-1" => RegionEndpoint.APSoutheast1,
            "ap-southeast-2" => RegionEndpoint.APSoutheast2,
            "ap-south-1" => RegionEndpoint.APSouth1,
            "ap-northeast-3" => RegionEndpoint.APNortheast3,
            "sa-east-1" => RegionEndpoint.SAEast1,
            "ap-northeast-1" => RegionEndpoint.APNortheast1,
            "ap-northeast-2" => RegionEndpoint.APNortheast2,
            "us-west-1" => RegionEndpoint.USWest1,
            "us-west-2" => RegionEndpoint.USWest2,
            "eu-north-1" => RegionEndpoint.EUNorth1,
            "eu-west-1" => RegionEndpoint.EUWest1,
            "us-east-2" => RegionEndpoint.USEast2,
            "eu-west-3" => RegionEndpoint.EUWest3,
            "eu-central-1" => RegionEndpoint.EUCentral1,
            "eu-south-1" => RegionEndpoint.EUSouth1,
            "ap-east-1" => RegionEndpoint.APEast1,
            "eu-west-2" => RegionEndpoint.EUWest2,
            _ => RegionEndpoint.USEast1
        };
    }
}
