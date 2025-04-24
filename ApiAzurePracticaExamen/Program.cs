using ApiAzurePracticaExamen.Data;
using ApiAzurePracticaExamen.Helpers;
using ApiAzurePracticaExamen.Repositories;
using ApiAzurePracticaExamen.Services;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});
SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();

HelperCryptography.Initialize(builder.Configuration, secretClient);
builder.Services.AddTransient<HelperUsuarioToken>();
builder.Services.AddHttpContextAccessor();

HelperActionServicesOAuth helper = new HelperActionServicesOAuth(builder.Configuration, secretClient);
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);
builder.Services.AddAuthentication(helper.GetAuthenticationSchema()).AddJwtBearer(helper.GetJwtBearerOptions());

// Add services to the container.
//string connectionString = builder.Configuration.GetConnectionString("SqlAzure");
KeyVaultSecret secret = await secretClient.GetSecretAsync("SqlAzure");
string connectionString = secret.Value;
builder.Services.AddTransient<RepositoryCubos>();
builder.Services.AddDbContext<CuboContext>(options => options.UseSqlServer(connectionString));


//string azureKeys = builder.Configuration.GetValue<string>("AzureKeys:StorageAccount");
KeyVaultSecret storageSecret = await secretClient.GetSecretAsync("StorageAccount");
string storage = storageSecret.Value;
BlobServiceClient blobServiceClient = new BlobServiceClient(storage);
builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);
builder.Services.AddTransient<ServiceStorageBlob>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
}
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
