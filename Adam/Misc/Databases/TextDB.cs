namespace ThereMustBeAnotherWay
{
    public class TextDb
    {
        static string[] _db = new string[1];
        public static string GetText(int textId)
        {
            return _db[textId];
        }
    }
}
