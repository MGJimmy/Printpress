using Rotativa.AspNetCore;

namespace Printpress.API;

public static class ApplicationPipelineExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler(_ => { });

        app.UseCors("AllowAll");

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();


        return app;
    }
}
