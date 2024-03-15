using FakeXiecheng.API.Database;
using FakeXiecheng.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Identity;
using FakeXiecheng.API.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Formatters;
//using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretByte = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]);
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Authentication:Audience"],

            ValidateLifetime = true,

            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
        };
    });
builder.Services.AddControllers(Action =>
{
    Action.ReturnHttpNotAcceptable = true; //統一回覆默認的數據結構Json
}).AddNewtonsoftJson(Action =>
{
    Action.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
})
.AddXmlDataContractSerializerFormatters()
.ConfigureApiBehaviorOptions(Action =>  //驗證數據是否非法
{
    Action.InvalidModelStateResponseFactory = context =>
    {
        var problemDetail = new ValidationProblemDetails(context.ModelState)
        {
            Type = "無所謂",
            Title = "數據驗證失敗",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = "請看詳細說明",
            Instance = context.HttpContext.Request.Path
        };
        problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
        return new UnprocessableEntityObjectResult(problemDetail)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});
builder.Services.AddTransient<ITouristRouteRepository, TouristRouteRepository>(); //每次發出請求時創建一個全新倉庫，請求結束後會自動註銷這個倉庫
builder.Services.AddDbContext<AppDbContext>(option =>
{
    //option.UseMySQL("server=localhost; Database=XiechengAPI;User Id=root; Password=springboot");
    string? connectionString = builder.Configuration.GetConnectionString("MySQL");
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    option.UseMySql(connectionString, serverVersion);
});
//掃描profile文件
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<IPropertyMappingService, PropertyMappingService>();
builder.Services.Configure<MvcOptions>(config =>
{
    var outputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
    if (outputFormatter != null)
    {
        outputFormatter.SupportedMediaTypes.Add("application/vnd.aleks.hateoas+json");
    }
});
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


app.UseHttpsRedirection();
//在哪裡
app.UseRouting();
//是誰
app.UseAuthentication();
//可以幹什麼，有什麼權限
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();