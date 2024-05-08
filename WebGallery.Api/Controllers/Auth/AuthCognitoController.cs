using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Shared.AWS.Cognito;

namespace WebGallery.Api.Controllers.Auth;

[Route("api/v1/auth")]
public class AuthCognitoController : Controller
{
    private readonly ICognitoService _cognitoService;

    public AuthCognitoController(ICognitoService cognitoService) => _cognitoService = cognitoService;

    [HttpPost]
    [Route("sign-in")]
    [ProducesResponseType(typeof(InitiateAuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InitiateAuthResponse>> InitiateAuthAsync([FromBody] SignIn userData)
    {
        try
        {
            var response = await _cognitoService.SignIn(userData);
            return Ok(response);
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
    public async Task<ActionResult<SignUpResponse>> SignUp([FromBody] SignUp user)
    {
        try
        {
            var response = await _cognitoService.SignUp(user);
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
            var resendConfirmationCodeResponse = await _cognitoService.ResendConfirmationCode(resendConfirmation);
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
            var confirmationLink = _cognitoService.GetConfirmationLink(confirmSignUp);
            return Ok(confirmationLink);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
