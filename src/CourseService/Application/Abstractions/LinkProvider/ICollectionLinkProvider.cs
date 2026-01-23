namespace Courses.Application.Abstractions.LinkProvider;

public interface ICollectionLinkProvider<TCollection, TQuery>
{
    IReadOnlyCollection<LinkDto> CreateLinks(TCollection collection, TQuery query);
}
