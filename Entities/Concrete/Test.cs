using Entities.Common;

namespace Entities.Concrete;

public class Test : BaseEntity
{
    public IQueryable<TestLang> TestLangs { get; set; }
}