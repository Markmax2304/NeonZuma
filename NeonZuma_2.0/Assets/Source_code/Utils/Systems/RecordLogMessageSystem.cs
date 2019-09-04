using System.Collections.Generic;

using NLog;
using Log = NLog.Logger;

using UnityEngine;
using Entitas;

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
        logger.Trace("\n\n\n\t\t *** START NEW FRAME *** \n\n");

        foreach (var entity in entities) 
        {
            switch (entity.logMessage.type)
            {
                case TypeLogMessage.Trace:
                    logger.Trace(entity.logMessage.message);
                    break;
                case TypeLogMessage.Error:
                    logger.Error(entity.logMessage.message);
                    break;
            }

            if (entity.logMessage.toUnityLog) 
            {
                Debug.Log(entity.logMessage.message);
            }

            entity.isDestroyed = true;
        }
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
