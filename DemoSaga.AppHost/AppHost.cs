var builder = DistributedApplication.CreateBuilder(args);

var product = builder.AddProject<Projects.DemoSaga_ProductService>("demosaga-productapi");

var order = builder.AddProject<Projects.DemoSaga_OrderService>("demosaga-orderapi");

builder.AddProject<Projects.DemoSaga_ApiGateway>("demosaga-apigateway").WaitFor(product).WaitFor(order);

builder.Build().Run();
