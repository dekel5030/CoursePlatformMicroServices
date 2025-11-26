var builder = DistributedApplication.CreateBuilder(args);

// RabbitMq configuration
var rabbitMq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin(15672)
    .WithDataVolume();

// AuthService configuration

var authDb = builder
    .AddPostgres("authservice-db")
    .WithBindMount(@"C:\AspireVolumes\AuthService", "/var/lib/postgresql/data")
    .AddDatabase("authdb");

var authService = builder
    .AddProject<Projects.Auth_Api>("authservice")
    .WithReference(authDb)
    .WithReference(rabbitMq)
    .WithEnvironment("ConnectionStrings:Database", authDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression);

// UserService configuration

var usersDb = builder
    .AddPostgres("userservice-db")
    .WithBindMount(@"C:\AspireVolumes\UsersService", "/var/lib/postgresql/data")
    .AddDatabase("usersdb");

var usersService = builder.AddProject<Projects.User_Api>("userservice")
    .WithReference(usersDb)
    .WithReference(rabbitMq)
    .WithEnvironment("ConnectionStrings:ReadDatabase", usersDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:WriteDatabase", usersDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression);


// CourseService configuration

var coursesDb = builder
    .AddPostgres("courseservice-db")
    .WithBindMount(@"C:\AspireVolumes\CoursesService", "/var/lib/postgresql/data")
    .AddDatabase("coursesdb");

var coursesService = builder.AddProject<Projects.Course_Api>("courseservice")
    .WithReference(coursesDb)
    .WithReference(rabbitMq)
    .WithEnvironment("ConnectionStrings:ReadDatabase", coursesDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:WriteDatabase", coursesDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression);

// Frontend configuration

var frontend = builder.AddNpmApp("frontend", "../Frontend", "dev")
    .WithReference(authService)
    .WithReference(usersService)
    .WithReference(coursesService)
    .WithHttpEndpoint(port: 5067, env: "PORT")
    .WithExternalHttpEndpoints();

// EnrollmentService configuration

var enrollmentDb = builder
    .AddPostgres("enrollmentservice-db")
    .WithBindMount(@"C:\AspireVolumes\EnrollmentService", "/var/lib/postgresql/data")
    .AddDatabase("enrollmentdb");

var enrollmentService = builder.AddProject<Projects.Enrollments_Api>("enrollmentservice")
    .WithReference(enrollmentDb)
    .WithReference(rabbitMq)
    .WithEnvironment("ConnectionStrings:ReadDatabase", enrollmentDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:WriteDatabase", enrollmentDb.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings:RabbitMq", rabbitMq.Resource.ConnectionStringExpression);

builder.Build().Run();
