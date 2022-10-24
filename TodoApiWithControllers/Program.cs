﻿using TodoApiWithControllers;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

var env = app.Environment;

startup.Configure(app, env);

app.MapControllers();

app.Run();