using Amazon.DynamoDBv2;
using Amazon.Textract;
using Canais25.Adapters.Outbound.Classification;
using Canais25.Adapters.Outbound.DynamoDb;
using Canais25.Adapters.Outbound.Textract;
using Canais25.Application.UseCases;
using Canais25.Core.Ports.In;
using Canais25.Core.Ports.Out;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAWSService<IAmazonTextract>();
builder.Services.AddAWSService<IAmazonDynamoDB>();

builder.Services.AddScoped<IDocumentTextExtractor, TextractDocumentTextExtractor>();
builder.Services.AddScoped<ICategoryProvider, InMemoryCategoryProvider>();
builder.Services.AddScoped<IComplaintRepository, DynamoDbComplaintRepository>();
builder.Services.AddScoped<IProcessDocumentCommand, ProcessDocumentUseCase>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
