using LostFoundPetReporter.Mobile.Services.Session;
using System.Net.Http.Headers;

namespace LostFoundPetReporter.Mobile.Services.Api
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly IUserSession _userSession;

        public JwtAuthorizationHandler(IUserSession userSession)
        {
            _userSession = userSession;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_userSession.Token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        _userSession.Token);
            }

            return await base.SendAsync(
                request,
                cancellationToken);
        }
    }
}