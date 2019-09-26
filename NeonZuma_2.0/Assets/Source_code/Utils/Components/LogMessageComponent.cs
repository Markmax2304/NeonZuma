using System;

using UnityEngine;
using Entitas;

/// <summary>
/// Данные о сообщение, которое будет записано в Лог
/// Имеет само сообщение, тип записи, следует ли писать в Юнити лог тоже, а также источник вызова лога
/// </summary>
[Manage]
public class LogMessageComponent : IComponent
{
    public string message;
    public TypeLogMessage type;
    public bool toUnityLog;
    public Type sourceType;
}
