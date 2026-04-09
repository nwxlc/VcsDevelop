namespace VcsDevelop.Core.Application;

public interface IHandler<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IHandler<in TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}
