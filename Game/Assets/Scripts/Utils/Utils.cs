using System.Threading.Tasks;

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
}
