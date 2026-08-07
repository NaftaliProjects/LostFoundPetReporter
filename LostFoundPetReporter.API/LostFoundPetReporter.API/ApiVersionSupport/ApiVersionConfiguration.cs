namespace LostFoundPetReporter.API.ApiVersionSupport
{
    public static class ApiVersionConfiguration
    {

        public static IServiceCollection AddLostFoundPetReporterApiVersionConfiguration(
            this IServiceCollection services, ApiVersion defaultVersion = null)
        {
            defaultVersion ??= ApiVersion.Default;

            services.AddApiVersioning(
                options =>
                {
                    //Set Default version
                    options.DefaultApiVersion = defaultVersion;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.UseApiBehavior = true;
                    // reporting api versions will return the headers "api-supported-versions"
                    // and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                    //This combines all of the available option as well as
                    // allows for using "v" or "api-version" as options for
                    // query string, header, or media type versioning
                    options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader(), //defaults to "api-version"
                    new QueryStringApiVersionReader("v"),
                    new HeaderApiVersionReader("api-version"),
                    new HeaderApiVersionReader("v"),
                    new MediaTypeApiVersionReader(), //defaults to "v"
                    new MediaTypeApiVersionReader("api-version")
                    );
                });

         
            return services;
        }

    }
}
