
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser]
public class Benchmarks
{
    private ServiceProvider _scopedServices;
    private ServiceProvider _transientServices;
    private ServiceProvider _singletonServices;

    [Params(1, 10, 100)]
    public int N { get; set; }

    public Benchmarks()
    {
        
        _scopedServices = new ServiceCollection()
            .AddScoped<A>()
            .AddScoped<B>()
            .AddScoped<C>()
            .AddScoped<D>()
            .BuildServiceProvider();

        _transientServices = new ServiceCollection()
            .AddTransient<A>()
            .AddTransient<B>()
            .AddTransient<C>()
            .AddTransient<D>()
            .BuildServiceProvider();

        _singletonServices = new ServiceCollection()
            .AddSingleton<A>()
            .AddSingleton<B>()
            .AddSingleton<C>()
            .AddSingleton<D>()
            .BuildServiceProvider();
    }

    [Benchmark(Baseline = true)]
    public void Transient()
    {
        using var scope = _scopedServices.CreateScope();

        for(int i = 0; i < N; i++)
        {
            var foo = scope.ServiceProvider.GetService<D>();
        }
    }
    
    [Benchmark]
    public void Scoped()
    {
        using var scope = _transientServices.CreateScope();

        for(int i = 0; i < N; i++)
        {
            var foo = scope.ServiceProvider.GetService<D>();
        }
    }

    [Benchmark]
    public void Singleton()
    {
        using var scope = _singletonServices.CreateScope();

        for(int i = 0; i < N; i++)
        {
            var foo = scope.ServiceProvider.GetService<D>();
        }
    }
}


public class A {}

public class B
{
    private readonly A _a;
    public B(A a) => _a = a;
}

public class C
{
    private readonly B _b;
    public C(B b) => _b = b;
}

public class D
{
    private readonly C _c;
    public D(C c) => _c = c;
}