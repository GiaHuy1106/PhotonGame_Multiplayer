using System;
using UnityEngine;

public static class StartUp 
{
    public static string IDtoken;
    [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnLoadMethod()
    {
        var token = TokenUtils.NewToken();
        IDtoken = TokenUtils.TokenToString(token);
        
    }

}
