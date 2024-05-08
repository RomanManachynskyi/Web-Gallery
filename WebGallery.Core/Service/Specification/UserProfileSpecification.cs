using Ardalis.Specification;
using WebGallery.Core.Dtos;
using WebGallery.Core.Extentions;
using WebGallery.Data.Entities;

namespace WebGallery.Core.Service.Specification;

public sealed class GetUserProfileByCognitoUserIdSpecification : Specification<UserProfile>
{
    public GetUserProfileByCognitoUserIdSpecification(Guid cognitoUserId)
    {
        Query
            .Where(userProfile => userProfile.CognitoUserId == cognitoUserId);
    }
}

public sealed class GetUserProfileByEmailSpecification : Specification<UserProfile>
{
    public GetUserProfileByEmailSpecification(string email)
    {
        Query
            .Where(userProfile => userProfile.Email == email);
    }
}
public sealed class ListUserProfilesSpecification : Specification<UserProfile>
{
    public ListUserProfilesSpecification(UserProfilesRequest request)
    {
        Query
            .Where(userProfile => string.IsNullOrEmpty(request.Search)
                            || userProfile.Username.Contains(request.Search))
            .Where(userProfile => string.IsNullOrEmpty(request.Search)
                            || userProfile.Occupation.Contains(request.Search));

        Query.ApplyListRequest(request.SortParameters, request.PaginationParameters);
    }
}
