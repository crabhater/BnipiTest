using BnipiTest.CryptContext;
using BnipiTest.Models.InterfacesLib;
using System.Resources;
using System.Runtime.CompilerServices;

namespace BnipiTest.Extensions
{
    public static class CryptAuthorizationAppBuilderExtension
    {
        private static string str = "Hello, World!";//Вот это мы расшифруем 
        private static ICrypt _crypt;
        public static IApplicationBuilder UseCryptAuthentication(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                _crypt = new Crypt();
                var message = await new StreamReader(context.Request.Body).ReadToEndAsync();
                var signature = context.Request.Headers["Data"];
                var uuid = context.Request.Headers["UUID"]; //Для поиска по БД токена авторизации, в данном случае не используем, так как не сохраняем токен
                var thumbPrint = context.Request.Headers["ThumbPrint"];

                try
                {
                    var args = _crypt.GetCryptArgs(thumbPrint, signature, str);
                    var isVerified = _crypt.VerifyDetached(args);

                    if (!isVerified)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Авторизация не пройдена!");
                    }
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"Ошибка авторизации: {ex.Message}");
                }
                await next();
            });
            return app;
        }
    }
}
