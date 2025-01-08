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

        app.UseCors("AllowAll");

        app.UseStaticFiles();

        app.UseRotativa();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapDefaultControllerRoute();




        return app;
    }


}
