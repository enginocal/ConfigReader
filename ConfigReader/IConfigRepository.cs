using System.Collections.Generic;

namespace ConfigReader
{
    public interface IConfigRepository
    {
        IEnumerable<Config> GetValues(string domain);
    }
}
