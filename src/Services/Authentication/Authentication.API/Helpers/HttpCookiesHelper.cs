namespace Authentication.API.Helpers
{
    public static class HttpCookiesHelper
    {
        public static void AppendAuthCookies(HttpResponse httpResponse, string accessToken, string refreshToken)
        {
            AppendCookie(httpResponse, "access_token", accessToken, TimeSpan.FromHours(1));
            AppendCookie(httpResponse, "refresh_token", refreshToken, TimeSpan.FromDays(7));
        }

        private static void AppendCookie(HttpResponse httpResponse, string name, string value, TimeSpan expiresIn)
        {
            httpResponse.Cookies.Append(name, value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.Add(expiresIn)
            });
        }
    }
}
