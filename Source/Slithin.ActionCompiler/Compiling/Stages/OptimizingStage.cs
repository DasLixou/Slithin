﻿using Flo;
using Slithin.ActionCompiler.Compiling.Passes;
using System;
using System.Threading.Tasks;

namespace Slithin.ActionCompiler.Compiling.Stages
{
    public class OptimizingStage : IHandler<CompilerContext, CompilerContext>
    {
        private PassManager _optimization = new();
        public OptimizingStage()
        {
            _optimization.AddPass<ConstantFoldingPass>();
        }

        public async Task<CompilerContext> HandleAsync(CompilerContext context, Func<CompilerContext, Task<CompilerContext>> next)
        {
            for (int i = 0; i < context.Trees.Count; i++)
            {
                context.Trees[i] = _optimization.Process(context.Trees[i]);
            }

            return await next.Invoke(context);
        }
    }
}