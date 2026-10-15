namespace LostFoundPetReporter.API.ApiVersionSupport
{
    public static class ApiVersionConfiguration
    {

        public static IServiceCollection AddLostFoundPetReporterApiVersionConfiguration(this IServiceCollection services, ApiVersion defaultVersion = null)

        {
            defaultVersion ??= ApiVersion.Default;

            services.AddApiVersioning(
                options =>
                {
                    options.DefaultApiVersion = defaultVersion;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.UseApiBehavior = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new UrlSegmentApiVersionReader(),
                        new QueryStringApiVersionReader(), 
                        new QueryStringApiVersionReader("v"),
                        new HeaderApiVersionReader("api-version"),
                        new HeaderApiVersionReader("v"),
                        new MediaTypeApiVersionReader(), 
                        new MediaTypeApiVersionReader("api-version")
                    );
                });

         
            return services;
        }

    }
}
