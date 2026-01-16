using Aspire.Hosting.JavaScript;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// RabbitMq configuration
IResourceBuilder<RabbitMQServerResource> rabbitMq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin(15672)
    .WithDataVolume();

string garageConfigPath = Path.Combine(builder.AppHostDirectory, "garage.toml");
IResourceBuilder<ContainerResource> garage = builder.AddContainer("garage", "dxflrs/garage", "v2.1.0")
    .WithBindMount(garageConfigPath, "/etc/garage.toml")
    .WithBindMount(@"C:\AspireVolumes\Garage\meta", "/var/lib/garage/meta")
    .WithBindMount(@"C:\AspireVolumes\Garage\data", "/var/lib/garage/data")
    .WithHttpEndpoint(port: 3900, targetPort: 3900, name: "s3")
    .WithHttpEndpoint(port: 3902, targetPort: 3902, name: "web")
    .WithHttpEndpoint(port: 3903, targetPort: 3903, name: "admin")
    .WithEntrypoint("/garage")
    .WithArgs("server");

IResourceBuilder<ProjectResource> storageService = builder.AddProject<Projects.StorageService>("storageservice")
    .WaitFor(garage)
    .WaitFor(rabbitMq)
    .WithEnvironment("S3__ServiceUrl", garage.GetEndpoint("s3"))
    .WithEnvironment("S3__PublicUrl", garage.GetEndpoint("s3"))
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression);

IResourceBuilder<RedisResource> redis = builder.AddRedis("redis").WithDataVolume().WithRedisInsight();

// Keycloak configuration

IResourceBuilder<ContainerResource> keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.4.7")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")
    .WithEnvironment("KC_HTTP_MANAGEMENT_HEALTH_ENABLED", "false")
    .WithEnvironment("KC_TRACING_ENABLED", "true")
    .WithEnvironment("OTEL_SERVICE_NAME", "keycloak")
    .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
    .WithEnvironment("OTEL_TRACES_SAMPLER", "always_on")
    .WithEnvironment("KC_SPI_EVENTS_LISTENER_HTTP_WEBHOOK_EVENTS", "REGISTER")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
    .WithBindMount(@"C:\AspireVolumes\keycloak-data", "/opt/keycloak/data")
    .WithBindMount(Path.Combine(builder.AppHostDirectory, "..", "infrastructure", "keycloak", "themes"), "/opt/keycloak/themes")
    .WithEnvironment("KC_THEME_CACHE_THEMES", "false")
    .WithEnvironment("KC_THEME_CACHE_TEMPLATES", "false")
    .WithArgs("start-dev", "--health-enabled=true", "--metrics-enabled=true")
    .WithHttpHealthCheck("/health/ready");

// AuthService configuration

IResourceBuilder<PostgresDatabaseResource> authDb = builder
    .AddPostgres("authservice-db")
    .WithPgAdmin()
    .WithBindMount(@"C:\AspireVolumes\AuthService", "/var/lib/postgresql/data")
    .AddDatabase("authdb");

IResourceBuilder<ProjectResource> authService = builder
    .AddProject<Projects.Auth_Api>("authservice")
    .WithReference(authDb)
    .WithReference(rabbitMq)
    .WithReference(redis)
    .WaitFor(keycloak)
    .WaitFor(authDb)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WithEnvironment("ConnectionStrings:ReadDatabase", authDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:WriteDatabase", authDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression)
    //.WithEnvironment("Keycloak:Authority", $"{keycloak.GetEndpoint("http")}/realms/course-platform")
    .WithHttpHealthCheck("/health");


// UserService configuration

IResourceBuilder<PostgresDatabaseResource> usersDb = builder
    .AddPostgres("userservice-db")
    .WithBindMount(@"C:\AspireVolumes\UsersService", "/var/lib/postgresql/data")
    .AddDatabase("usersdb");

IResourceBuilder<ProjectResource> usersService = builder.AddProject<Projects.Users_Api>("userservice")
    .WithReference(usersDb)
    .WithReference(rabbitMq)
    .WaitFor(usersDb)
    .WaitFor(rabbitMq)
    .WithEnvironment("ConnectionStrings:ReadDatabase", usersDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:WriteDatabase", usersDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression)
    .WithHttpHealthCheck("/health");

// CourseService configuration

IResourceBuilder<PostgresDatabaseResource> coursesDb = builder
    .AddPostgres("courseservice-db")
    .WithBindMount(@"C:\AspireVolumes\CoursesService", "/var/lib/postgresql/data")
    .AddDatabase("coursesdb");

IResourceBuilder<ProjectResource> coursesService = builder.AddProject<Projects.Courses_Api>("courseservice")
    .WithReference(coursesDb)
    .WithReference(rabbitMq)
    .WithReference(authService)
    .WaitFor(coursesDb)
    .WaitFor(rabbitMq)
    .WithEnvironment("ConnectionStrings:ReadDatabase", coursesDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:WriteDatabase", coursesDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression)
    .WithEnvironment("Storage:BaseUrl", $"{storageService.GetEndpoint("https")}/storage")
    .WithHttpHealthCheck("/health");
// Gateway configuration

IResourceBuilder<ProjectResource> gateway = builder.AddProject<Projects.Gateway_Api>("gateway")
    .WithEnvironment(
        "ReverseProxy__Clusters__garage-website-cluster__Destinations__destination1__Address",
        garage.GetEndpoint("web")
    )
    .WithReference(authDb)
    .WithReference(authService)
    .WithReference(usersService)
    .WithReference(coursesService)
    .WithReference(redis);

EndpointReference gatewayEndpoint = gateway.GetEndpoint("https");
coursesService.WithEnvironment("s3__Endpoint", gatewayEndpoint);

// Frontend configuration
IResourceBuilder<JavaScriptAppResource> _ = builder.AddJavaScriptApp("frontend", "../Frontend", "dev")
    .WithReference(gateway)
    .WaitFor(gateway)
    .WithHttpEndpoint(port: 5067, env: "PORT")
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
