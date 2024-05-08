using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;
using WebGallery.Shared.AWS.Cognito;

namespace WebGallery.Api.Controllers.Auth;

[ApiController]
[Tags("Authorization/User")]
[Route("api/v1/auth")]
public class AuthCognitoController : Controller
{
    private readonly ICognitoService cognitoService;
    private readonly IRepository<UserProfile> userProfileRepository;

    public AuthCognitoController(
        ICognitoService cognitoService,
        IRepository<UserProfile> userProfileRepository)
    {
        this.cognitoService = cognitoService;
        this.userProfileRepository = userProfileRepository;
    }

    [HttpPost]
    [Route("sign-in")]
    [ProducesResponseType(typeof(InitiateAuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InitiateAuthResponse>> InitiateAuthAsync([FromBody] SignIn userData)
    {
        try
        {
            var cognitoResponse = await cognitoService.SignIn(userData);

            if (!await userProfileRepository.AnyAsync(new GetUserProfileByEmailSpecification(userData.Email)))
                throw new NotFoundException("User profile not found");

            return Ok(cognitoResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("sign-up")]
    [ProducesResponseType(typeof(SignUpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SignUpResponse>> SignUp([FromBody] SignUp userData)
    {
        try
        {
            var response = await cognitoService.SignUp(userData);
            return Created("User signed up successfully", response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CodeDeliveryDetailsType), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Route("sign-up/resend-code")]
    public async Task<ActionResult<CodeDeliveryDetailsType>> ResendConfirmationCode(
        [FromBody] ResendConfirmationCode resendConfirmation)
    {
        try
        {
            var resendConfirmationCodeResponse = await cognitoService.ResendConfirmationCode(resendConfirmation);
            return Ok(resendConfirmationCodeResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Route("sign-up/confirm")]
    public ActionResult<string> GetConfirmationLink([FromBody] ConfirmSignUp confirmSignUp)
    {
        try
        {
            var confirmationLink = cognitoService.GetConfirmationLink(confirmSignUp);
            return Ok(confirmationLink);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
