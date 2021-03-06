﻿using Ambition.Scheduler;
using System;
using System.Collections.Generic;

namespace Ambition.Pipeline
{
    public interface IPipelineProvider
    {
        void AddOrUpdatePipelines<TRequestTask>(List<Type> pipelineTypes) where TRequestTask : IRequestTask;

        void DeletePipelines<TRequestTask>() where TRequestTask : IRequestTask;

        List<IPipeline> GetPipelines(Type requestTaskType);
    }
}