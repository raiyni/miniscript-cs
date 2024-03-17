using System;
using System.Collections.Generic;
using System.Linq;
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

    [Export] public float maxExecutionTime = 0.01f;

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

    [ExportGroup("Features")]
    [Export] public bool allowMoving = true;

    protected Interpreter interpreter;
    protected HostData hostData;

    public override void _Ready()
    {
        LoadLibraries();
        AddIntrinsics();

        parent ??= GetParent();
        hostData = createDataObject();

        GD.Print(parent);
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
                var content = FileAccess.GetFileAsString(path + fileName);
                scripts.Add(content);
                fileName = dir.GetNext();
            }
        }
    }

    protected virtual void SetupInterpreter()
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
        if (parent is Node3D) return new HostData3D(parent, this);
        if (parent is Node2D) return new HostData2D(parent, this);

        return new HostData(parent, this);
    }

    public virtual void InvokeEvent(string functionName, ValMap args = null)
    {
        if (interpreter == null || !interpreter.Running()) return;

        Value handler = interpreter.GetGlobalValue(functionName);
        if (handler == null) return;

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
}