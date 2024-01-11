namespace ShroomCity.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ConfigureApiMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShroomCity API v1"));
        }

        app.UseRouting();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        return app;
    }
}
