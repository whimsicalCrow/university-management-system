using Microsoft.AspNetCore.Builder;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using University.Application.Common.Behaviors;
using University.Application.ThesisProjects.Commands.CreateThesisProject;
using University.Domain.Interfaces;
using University.Infrastructure.Persistence;
using University.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// EF Core InMemory for demo (swap for SQL Server in production)
builder.Services.AddDbContext<UniversityDbContext>(opt => opt.UseInMemoryDatabase("UniversityDb"));

// Application services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateThesisProjectCommand).Assembly));
AssemblyScanner.FindValidatorsInAssembly(typeof(CreateThesisProjectCommand).Assembly)
    .ForEach(result => builder.Services.AddScoped(result.InterfaceType, result.ValidatorType));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IProfessorRepository, ProfessorRepository>();
builder.Services.AddScoped<IThesisProjectRepository, ThesisProjectRepository>();
builder.Services.AddScoped<IThesisUpdateRepository, ThesisUpdateRepository>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
