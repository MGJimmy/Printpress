/**************************************************************************
 * all copy right  reserved to Printpress API's developers 
 * authorized by :
 * Rashad Elhashmi , Mohamed Gamal , Mohamed Shokry
 **************************************************************************/

using Printpress.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);


var app = builder.Build();


app.UseApplicationPipeline();

app.Run();


/**************************************************************************
 **************************************************************************
 **************************************************************************/
