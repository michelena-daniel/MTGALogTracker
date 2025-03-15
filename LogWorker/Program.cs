using Domain.Interfaces;
using Domain.Models.Settings;
using Infrastructure.Data;
using Infrastructure.Maps;
using Infrastructure.Repositories;
using LogWorker;
using LogWorker.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.Configure<LogPathOptions>(
    builder.Configuration.GetSection("LogPath"));
builder.Services.Configure<LogPathOptions>(
    builder.Configuration.GetSection("LogPath"));

builder.Services.AddTransient<ILogReaderService, LogReaderService>();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();
builder.Services.AddScoped<IPlayerRankRepository, PlayerRankRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

var host = builder.Build();
host.Run();
