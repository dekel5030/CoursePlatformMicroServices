var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis").WithDataVolume();

// RabbitMq configuration
var rabbitMq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin(15672)
    .WithDataVolume();

// Keycloak configuration

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.4.7")
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
    .WithArgs("start-dev", "--health-enabled=true", "--metrics-enabled=true")
    .WithHttpHealthCheck("/health/ready");

// AuthService configuration

var authDb = builder
    .AddPostgres("authservice-db")
    .WithPgAdmin()
    .WithBindMount(@"C:\AspireVolumes\AuthService", "/var/lib/postgresql/data")
    .AddDatabase("authdb");

var authService = builder
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

var usersDb = builder
    .AddPostgres("userservice-db")
    .WithBindMount(@"C:\AspireVolumes\UsersService", "/var/lib/postgresql/data")
    .AddDatabase("usersdb");

var usersService = builder.AddProject<Projects.User_Api>("userservice")
    .WithReference(usersDb)
    .WithReference(rabbitMq)
    .WaitFor(usersDb)
    .WaitFor(rabbitMq)
    .WithEnvironment("ConnectionStrings:ReadDatabase", usersDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:WriteDatabase", usersDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression)
    .WithHttpHealthCheck("/health");

// CourseService configuration

var coursesDb = builder
    .AddPostgres("courseservice-db")
    .WithBindMount(@"C:\AspireVolumes\CoursesService", "/var/lib/postgresql/data")
    .AddDatabase("coursesdb");

var coursesService = builder.AddProject<Projects.Course_Api>("courseservice")
    .WithReference(coursesDb)
    .WithReference(rabbitMq)
    .WaitFor(coursesDb)
    .WaitFor(rabbitMq)
    .WithEnvironment("ConnectionStrings:ReadDatabase", coursesDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:WriteDatabase", coursesDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression)
    .WithHttpHealthCheck("/health");

// Gateway configuration

var gateway = builder.AddProject<Projects.Gateway_Api>("gateway")
    .WithReference(authDb)
    .WithReference(authService)
    .WithReference(usersService)
    .WithReference(coursesService)
    .WithReference(redis)
    .WaitFor(authDb)
    .WaitFor(authService)
    .WaitFor(usersService)
    .WaitFor(coursesService);

// Frontend configuration
var frontend = builder.AddJavaScriptApp("frontend", "../Frontend", "dev")
    .WithReference(gateway)
    .WaitFor(gateway)
    .WithHttpEndpoint(port: 5067, env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
