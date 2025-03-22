using Domain.Interfaces;
using Domain.Models.Settings;
using Infrastructure.Data;
using Infrastructure.Maps;
using Infrastructure.Repositories;
using LogWorker;
using LogWorker.Services;
using LogWorker.Services.CoreServices;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHostedService<Worker>();
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.Configure<LogPathOptions>(
    builder.Configuration.GetSection("LogPath"));
builder.Services.Configure<LogPathOptions>(
    builder.Configuration.GetSection("LogPath"));

builder.Services.AddTransient<ILogReaderService, LogReaderService>();
builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();
builder.Services.AddScoped<IPlayerRankRepository, PlayerRankRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddTransient<IUserInfoService, UserInfoService>();
builder.Services.AddTransient<IRankService, RankService>();
builder.Services.AddTransient<IMatchService, MatchService>();
builder.Services.AddTransient<IDeckService, DeckService>();
builder.Services.AddTransient<IEventService, EventService>();

var host = builder.Build();
host.Run();
