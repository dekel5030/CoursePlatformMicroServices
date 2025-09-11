namespace Kernel;
public interface IVersionedEntity
{
    long EntityVersion { get; }
}
