﻿using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ForStatementSyntax : IStatetementSyntax
    {
        public ISyntaxBase InitExpression { get; }
        public IExpressionSyntax CondExpression { get; }
        public ISyntaxBase LoopExpression { get; }
        public List<ISyntaxBase> Body { get; }

        public ForStatementSyntax(ISyntaxBase initExpression, IExpressionSyntax condExpression, ISyntaxBase loopExpression, List<ISyntaxBase> body)
        {
            InitExpression = initExpression;
            CondExpression = condExpression;
            LoopExpression = loopExpression;
            Body = body;
        }
    }
}