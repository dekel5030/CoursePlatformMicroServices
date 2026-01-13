//using System;
//using System.Collections.Generic;
//using System.Text;

namespace CoursePlatform.ServiceDefaults.Dtos;

//public interface ICollectionResponse<T>
//{
//    List<T> Items { get; init; } 
//}

//public sealed record PagnitatedResponse<T> : ICollectionResponse<T>
//{
//    public List<T> Items { get; init; }
//    public int Page { get; init; }
//    public int PageSize { get; init; }
//    public int TotalCount { get; init; }
//    public int TotalPages => (int)Math.Ceiling(TotalCount / (double) PageSize);
//    public bool HasPreviousPage => Page > 1;
//    public bool HasNextPage => Page < TotalPages;

//    public static async Task<PagnitatedResponse<T>> CreateAsync(
//        IQueryable<T> query,
//        int page,
//        int pageSize)
//    {
//        int totalCount = await query.CountAsync();
//        List<T> items = await query
//            .Skip((page - 1) * pageSize)
//            .Take(pageSize)
//            .ToListAsync();

//        return new PagnitatedResponse<T>
//        {
//            Items = items,
//            Page = page,
//            PageSize = pageSize,
//            TotalCount = totalCount
//        };
//    }
//}
public class User(string name, int age)
{
    public string Name => name;
    public int Age => age;
}

//public class Admin : User
//{
//}
