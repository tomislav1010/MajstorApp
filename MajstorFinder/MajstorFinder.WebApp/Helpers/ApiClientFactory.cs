using System.Net.Http.Headers;

namespace MajstorFinder.WebApp.Helpers
{
    public static class ApiClientFactory
    {
        public static HttpClient CreateWithJwt(IHttpClientFactory factory, string? jwt)
        {
            var client = factory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(jwt))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            return client;
        }
    }
}
