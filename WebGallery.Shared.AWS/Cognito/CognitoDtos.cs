using System.ComponentModel.DataAnnotations;

namespace WebGallery.Shared.AWS.Cognito;

public sealed record SignUp(
    string Email,
    [MinLength(8)]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one number.")]
    string Password);

public sealed record SignIn(string Email, string Password);

public sealed record ResendConfirmationCode(string Email);

public sealed record ConfirmSignUp(string Email, string ConfirmationCode);
