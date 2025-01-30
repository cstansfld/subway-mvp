using System.Reflection;
using Subway.Mvp.Apis.FreshMenu;
using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Domain.FreshMenu;
using Subway.Mvp.Infrastructure.Caching;

namespace Subway.Mvp.Tests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(MealOfTheDay).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(CacheOptions).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
