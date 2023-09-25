namespace partycli
{
    public interface IStorage
    {
        void StoreValue(string name, string value, bool writeToConsole = true);
    }
}