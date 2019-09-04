using UnityEngine;
using Entitas;

[Manage]
public class LogMessageComponent : IComponent
{
    public string message;
    public TypeLogMessage type;
    public bool toUnityLog;
}
