using BnipiTest.Extensions;
using BnipiTest.Models;
using BnipiTest.Models.InterfacesLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace BnipiTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ProjectContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("BnipiTestConString")));

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            //На собесе было недопонимание как происходит аутентификация в моем приложении, для Дениса накидал
            //пример как можно проводить аутентификацию при помощи DI конвейера компонентов и криптографических функций
            //app.UseCryptAuthentication();

            app.Run();
        }
    }
}
