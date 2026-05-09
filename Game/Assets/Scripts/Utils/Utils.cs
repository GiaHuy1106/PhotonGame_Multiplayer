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
    public static Vector3 GetRandomAroundPoint(Vector3 point, float range = 1f)
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * range;
        return new Vector3(point.x + randomCircle.x, point.y, point.z + randomCircle.y);
    }
}
