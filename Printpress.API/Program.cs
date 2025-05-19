/**************************************************************************
 * all copy right  reserved to Printpress API's developers 
 * authorized by :
 * Rashad Elhashmi , Mohamed Gamal , Mohamed Shokry
 **************************************************************************/

using NLog.Web;
using Printpress.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

// Clear default providers and use NLog
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();


app.UseApplicationPipeline();

app.Run();


/**************************************************************************
 **************************************************************************
 **************************************************************************/
