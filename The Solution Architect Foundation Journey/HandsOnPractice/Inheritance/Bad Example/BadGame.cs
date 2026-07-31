namespace Inheritance.Bad_Example;

public abstract class BadGame
{
    public void Start()
    {
        Play();
    }
    protected void LoadResources()
    {
        Console.WriteLine("Resource Loaded...");
    }
    public abstract void Play();
}

public class BadFootball : BadGame
{
    public override void Play()
    {
        LoadResources(); // developer need to remember
        Console.WriteLine("Playing Football...");
    }
}

public class BadChess : BadGame
{
    public override void Play()
    {
        // delveloper miss to call LoadResources() here
        Console.WriteLine("Playing Chess...");
    }
}
// what's wrong in this code:
// code duplication here
