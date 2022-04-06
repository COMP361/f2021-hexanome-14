public enum GameMode : byte
{
    Elfenland,
    Elfengold
};


static class GameModeExtension
{
    public static byte[] Serialize(object mode)
    {
        var ph = (GameMode)mode;
        return new byte[] { (byte)ph };
    }

    public static object Deserialize(byte[] v)
    {
        return (GameMode)v[0];
    }
}