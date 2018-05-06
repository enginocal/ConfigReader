namespace ConfigReader
{
    public interface IConfigReader
    {
        T GetValue<T>(string key);
    }
}
