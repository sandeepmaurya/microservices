namespace Common.Configuration
{
    public interface IConfiguration
    {
        string GetValue(string key);
    }
}
