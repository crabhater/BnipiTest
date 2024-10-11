using BnipiTest.Extensions;
using BnipiTest.Models;
using BnipiTest.Models.InterfacesLib;
using Microsoft.EntityFrameworkCore;
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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            //На собесе было недопонимание как происходит аутентификация в моем приложении, для Дениса накидал
            //пример как можно проводить аутентификацию при помощи DI конвейера компонентов и криптографических функций
            //Это только пример и лучше его не использовать. Довольно сложно добиться нормальной работоспособности в winapi,а это вообще на коленке написано
            //Если есть желание посмотреть как работает - оставил эндпоинт для импорта сертификата с закрытым ключом, можно развернуть на виртуалке и экспортировать сертификат
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseCryptAuthentication();
            //}



            app.Run();
        }
    }
}
