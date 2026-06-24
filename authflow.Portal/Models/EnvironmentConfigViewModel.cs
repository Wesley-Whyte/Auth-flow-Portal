namespace authflow.Portal.Models;

/// <summary>Holds configuration values sourced from environment variables for display on the home page.</summary>
/// <param name="Environment">The current hosting environment (e.g. Development, Production).</param>
/// <param name="AuthApiBaseUrl">Base URL of the external authentication API.</param>
/// <param name="JwtIssuer">The expected issuer claim used when validating JWT tokens.</param>
/// <param name="JwtSigninKey">The symmetric key used to sign and validate JWT tokens.</param>
/// <param name="GoogleClientId">OAuth 2.0 client ID for Google authentication.</param>
/// <param name="GoogleClientSecret">OAuth 2.0 client secret for Google authentication.</param>
/// <param name="MicrosoftClientId">OAuth 2.0 client ID for Microsoft authentication.</param>
/// <param name="MicrosoftClientSecret">OAuth 2.0 client secret for Microsoft authentication.</param>
public record EnvironmentConfigViewModel(
    string Environment,
    string AuthApiBaseUrl,
    string JwtIssuer,
    string JwtSigninKey,
    string GoogleClientId,
    string GoogleClientSecret,
    string MicrosoftClientId,
    string MicrosoftClientSecret
);
