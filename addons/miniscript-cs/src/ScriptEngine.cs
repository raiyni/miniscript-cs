using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Miniscript;

public partial class ScriptEngine : Node
{
    static bool intrinsicsAdded = false;
    static bool librariesLoaded = false;
    static string libraries;

    [ExportGroup("Settings")]
    [Export] public Node parent;
    [Export] public bool compileOnSet = true;

    [Export] public bool discoverIntrinsics = true;

    [Export] public float maxExecutionTime = 0.02f;

    [Export(PropertyHint.Dir)]
    public string additionalLibraries { get; set; }

    private string _script;
    [Export(PropertyHint.MultilineText)]
    public string Script
    {
        get => _script;
        set
        {
            _script = value;
            if (compileOnSet && interpreter != null)
            {
                Compile();
            }
        }
    }

    protected Interpreter interpreter;
    protected HostData hostData;

    public bool Running() => interpreter.Running(); 
    public bool Done() => interpreter.done; 

    static void AddIntrinsics(bool discover)
    {
        if (intrinsicsAdded) return;
        intrinsicsAdded = true;

        if (!discover) return;

        var methods = AppDomain.CurrentDomain.GetAssemblies().ToList()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass)
            .SelectMany(x => x.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public))
            .Where(x => x.GetCustomAttributes(typeof(Discover), false).FirstOrDefault() != null);

        GD.Print($"Discovered methods: {methods.Count()}");
        foreach (var m in methods)
        {
            m.Invoke(null, new object[]{});
        }
    }

    public override void _Ready()
    {
        LoadLibraries();
        AddIntrinsics(discoverIntrinsics);

        parent ??= GetParent();
        hostData = createDataObject();

        SetupInterpreter();
    }

    string LoadLibraries()
    {
        if (!librariesLoaded)
        {
            librariesLoaded = true;
            var scripts = new List<string>();

            scanDir(scripts, "res://addons/miniscript-cs/src/lib/");
            if (!string.IsNullOrWhiteSpace(additionalLibraries))
            {
                scanDir(scripts, additionalLibraries);
            }

            libraries = string.Join("\n\n", scripts);
        }

        return libraries;
    }

    static void scanDir(List<string> scripts, string path)
    {
        if (!path.EndsWith("/"))
        {
            path += "/";
        }

        using var dir = DirAccess.Open(path);
        if (dir != null)
        {
            dir.ListDirBegin();

            string fileName = dir.GetNext();
            while (fileName != "")
            {
                GD.Print($"ScriptEngine: found library {fileName}");
                var content = FileAccess.GetFileAsString(path + fileName);
                scripts.Add(content);
                fileName = dir.GetNext();
            }
        }
    }

    public virtual void SetupInterpreter()
    {
        interpreter = new Interpreter();
        interpreter.hostData = hostData;
        interpreter.standardOutput = OnStdOut;
        interpreter.errorOutput = onErrOut;

        Compile();
    }

    public virtual void Compile()
    {
        interpreter.Reset(LoadLibraries() + "\n\n" + Script);
        interpreter.Compile();
    }

    public virtual void Run()
    {
        interpreter.RunUntilDone(maxExecutionTime);
    }

    public virtual void Restart()
    {
        interpreter.Restart();
    }

    public virtual void Stop()
    {
        interpreter.Stop();
    }

    protected virtual void OnStdOut(string s, bool eol)
    {
        GD.Print(s);
    }

    protected virtual void onErrOut(string s, bool eol)
    {
        GD.PrintErr(s);
    }

    protected virtual HostData createDataObject()
    {
        return new HostData(parent, this);
    }

    public void Publish(string functionName, ValMap args = null)
    {
        if (interpreter == null || !interpreter.Running()) return;

        Value handler = interpreter.GetGlobalValue(functionName);
        if (handler == null) 
        {
            GD.PrintErr($"ScriptEngine: {functionName} function does not exist");
            return;
        }

        var eventList = interpreter.GetGlobalValue("_events") as ValList;
        if (eventList == null)
        {
            eventList = new ValList();
            interpreter.SetGlobalValue("_events", eventList);
        }

        ValMap newEvent = new ValMap();
        newEvent["invoke"] = handler;
        newEvent["args"] = args ?? new ValMap();

        eventList.values.Add(newEvent);
    }

    public void ClearEvents()
    {
        var eventList = interpreter.GetGlobalValue("_events") as ValList;
        if (eventList != null)
        {
            eventList.values.Clear();
        }
    }
}