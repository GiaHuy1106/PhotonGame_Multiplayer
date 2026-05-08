using System.Threading.Tasks;
using UnityEngine;

public static class Utils 
{
    public static async void DelayCall(float seconds, System.Action action)
    {
        await Task.Delay((int)(seconds * 1000));
        action?.Invoke();
    }

    public static async void Delay1Frame(System.Action action)
    {
        await Task.Yield();
        action?.Invoke();
    }
    public static Vector3 GetRandomPosition()
    {
        return new Vector3(UnityEngine.Random.Range(-10f, 10f), 1, UnityEngine.Random.Range(-10f, 10f));
    }
    public static Vector3 GetRandomArroundPoint(Vector3 point)
    {
        return new Vector3(UnityEngine.Random.Range(-5f, 5f) + point.x, point.y, UnityEngine.Random.Range(-5f, 5f) + point.z);
    }
}
