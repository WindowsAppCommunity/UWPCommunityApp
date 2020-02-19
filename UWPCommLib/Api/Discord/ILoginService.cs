using Refit;
using System.Threading.Tasks;
using UWPCommLib.Api.Discord.Models;

namespace UWPCommLib.Api.Discord
{
    public interface ILoginService
    {
        [Post("/auth/login")]
        Task<LoginResult> Login([Body] LoginRequest loginRequest);

        [Post("/auth/mfa/totp")]
        Task<LoginResult> LoginMFA([Body] LoginMFARequest loginRequest);

        [Post("/auth/mfa/sms")]
        Task<LoginResult> LoginSMS([Body] LoginMFARequest loginRequest);

        [Post("/auth/mfa/sms/send")]
        Task<SendSmsResult> SendSMS([Body] SendSmsRequest loginRequest);

        [Post("/oauth2/token")]
        Task<TokenResponse> ExchangeToken([Body(BodySerializationMethod.UrlEncoded)]TokenRequest request);
    }
}
