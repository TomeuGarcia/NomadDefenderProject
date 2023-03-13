
public class GameTime
{
    static public float TimeScale { get; private set; } = 1f;

    static public void SetTimeScale(float timeScale)
    {
        TimeScale = timeScale;
    }

}
