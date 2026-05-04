using System;
using UnityEngine;

public static class StartUp 
{
    public static byte[] token;
    [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnLoadMethod()
    {
        token = TokenUtils.NewToken();
    }

}
