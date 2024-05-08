using Ardalis.Specification;
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
