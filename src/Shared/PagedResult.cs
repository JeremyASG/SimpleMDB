using System.ComponentModel;

namespace SimpleMDB;

public class PagedResult<T>
{
    public List<T> Values { get; }
    public int Totalcount { get; }

    public PagedResult(List<T> values, int totalCount)
    {
        Values = values;
        Totalcount = totalCount;
    }
}

