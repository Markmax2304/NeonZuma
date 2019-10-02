using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

/// <summary>
/// Логика записи сообщений в лог
/// </summary>
public class RecordLogMessageSystem : ReactiveSystem<ManageEntity>
{
    private static Log logger = LogManager.GetCurrentClassLogger();

    private Contexts _contexts;

    public RecordLogMessageSystem(Contexts contexts) : base(contexts.manage)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<ManageEntity> entities)
    {
#if UNITY_EDITOR
        logger.Trace("\n\n\n\t\t *** START NEW FRAME *** \n\n");

        foreach (var entity in entities) 
        {
            switch (entity.logMessage.type)
            {
                case TypeLogMessage.Trace:
                    logger.Trace(string.Concat(entity.logMessage.sourceType.ToString(), entity.logMessage.message));
                    break;
                case TypeLogMessage.Error:
                    logger.Error(string.Concat(entity.logMessage.sourceType.ToString(), entity.logMessage.message));
                    break;
            }

            if (entity.logMessage.toUnityLog) 
            {
                Debug.Log(entity.logMessage.message);
            }

            entity.isDestroyed = true;
        }
#endif
    }

    protected override bool Filter(ManageEntity entity)
    {
        return entity.hasLogMessage;
    }

    protected override ICollector<ManageEntity> GetTrigger(IContext<ManageEntity> context)
    {
        return context.CreateCollector(ManageMatcher.LogMessage);
    }
}
