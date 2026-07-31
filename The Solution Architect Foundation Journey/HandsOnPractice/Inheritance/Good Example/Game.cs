using System;
using System.Collections.Generic;
using System.Text;

namespace Inheritance.Good_Example;

public abstract class Game
{
    public void Start()
    {
        LoadResources();
        Play();
    }
    private void LoadResources()
    {
        Console.WriteLine("Resource Loaded...");
    }
    public abstract void Play();
}

public class Football : Game
{
    public override void Play()
    {
        Console.WriteLine("Playing Football...");
    }
}

public class Chess : Game
{
    public override void Play()
    {
        Console.WriteLine("Playing Chess...");
    }
}
