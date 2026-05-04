using System;

public static class TokenUtils 
{
    public static int HashToken(byte[] token)
    {
        return new Guid(token).GetHashCode();
    }
    public static byte[] NewToken()
    {
        return Guid.NewGuid().ToByteArray();
    }
    public static string TokenToString(byte[] token)
    {
        return new Guid(token).ToString();
    }
}
