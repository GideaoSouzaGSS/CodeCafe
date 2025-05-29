using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.CodeCafe_ApiService>("apiservice");

builder.Build().Run();
