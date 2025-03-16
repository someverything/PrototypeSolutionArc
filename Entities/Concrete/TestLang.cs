using Entities.Common;

namespace Entities.Concrete;

public class TestLang : BaseEntity
{
    public string Name { get; set; }
    public string LangCode { get; set; }
    public Test Test { get; set; }
    public Guid TestId { get; set; }
}