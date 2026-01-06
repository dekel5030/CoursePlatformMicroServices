namespace Kernel.ResourceActions;

public record ResourceAction(string Href, string Method, string? Rel = null);

public interface ICanUploadImage
{
    ResourceAction? UploadImage { get; }
}

public interface ICanUploadVideo
{
    ResourceAction? UploadVideo { get; }
}

public interface IHasStorageActions<T> where T : class
{
    T? StorageActions { get; init; }
}