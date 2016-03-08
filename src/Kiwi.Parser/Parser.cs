using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Common;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class Parser : ParserBase
    {
        private Func<ISyntaxBase> _namespaceBodyParser;
        private Func<ISyntaxBase> _classBodyParser;
        private Func<ISyntaxBase> _functionParameterParser;
        private Func<ISyntaxBase> _functionBodyParser;

        public Parser(List<Token> token) : base(new TransactableTokenStream(PrepareTokenSource(token)))
        {
            InitParser();
        }
        
        public CompilationUnitSyntax Parse()
        {
            var syntax = new List<ISyntaxBase>();
            while (TokenStream.Current != null)
            {
                var result = Parse(ParseUsing, ParseNamespace);
                if (result == null)
                {
                    throw new KiwiSyntaxException("Unexpected Syntax. Expected UsingSyntax or NamespaceSyntax");
                }
                syntax.Add(result);
            }
            return new CompilationUnitSyntax(syntax);
        }

        private NamespaceSyntax ParseNamespace()
        {
            if (TokenStream.Current.Type != TokenType.NamespaceKeyword)
            {
                return null;
            }

            Consume(TokenType.NamespaceKeyword);
            var namespaceName = Consume(TokenType.Symbol);
            
            var bodySyntax = ParseScope(_namespaceBodyParser);
            return new NamespaceSyntax(namespaceName, bodySyntax);
        }

        private UsingSyntax ParseUsing()
        {
            if (TokenStream.Current.Type != TokenType.UsingKeyword)
            {
                return null;
            }

            Consume(TokenType.UsingKeyword);
            var namespaceName = Consume(TokenType.Symbol);
            return new UsingSyntax(namespaceName);
        }

        private ClassSyntax ParseClass()
        {
            if (TokenStream.Current.Type != TokenType.ClassKeyword)
            {
                return null;
            }

            Consume(TokenType.ClassKeyword);
            var className = Consume(TokenType.Symbol);
            var inherit = TokenStream.Current.Type == TokenType.IsKeyword;
            Token descriptorName = null;

            if (inherit)
            {
                Consume(TokenType.IsKeyword);
                descriptorName = Consume(TokenType.Symbol);
            }
            
            var inner = ParseScope(_classBodyParser);
            return new ClassSyntax(className, descriptorName, inner);
        }

        private FieldSyntax ParseField()
        {
            if (TokenStream.Current.Type != TokenType.ConstKeyword && TokenStream.Current.Type != TokenType.VarKeyword)
            {
                return null;
            }

            var isConst = TokenStream.Current.Type == TokenType.ConstKeyword;
            var fieldTypeQualifier = Consume(isConst ? TokenType.ConstKeyword : TokenType.VarKeyword);
            var fieldType = Consume(TokenType.Symbol);
            var fieldName = Consume(TokenType.Symbol);
            IExpressionSyntax fieldInitializer = null;

            if (isConst)
            {
                Consume(TokenType.Colon);
                fieldInitializer = ParseExpression();
            }

            return new FieldSyntax(fieldTypeQualifier, fieldType, fieldName, fieldInitializer);
        }

        private IExpressionSyntax ParseExpression()
        {
            var binaryOperators = new[]
                            {
                                TokenType.Equal,
                                TokenType.Mult,
                                TokenType.Div,
                                TokenType.Add,
                                TokenType.Sub,
                                TokenType.Pow,
                                TokenType.Less,
                                TokenType.Greater,
                                TokenType.Or,
                                TokenType.TwoDots, 
                            };

            var firstExpression = ParseTermOrPrefixExpression();
            
            if (!binaryOperators.Contains(TokenStream.Current.Type))
            {
                return firstExpression;
            }

            var expressionOperatorChain = new ExpressionOperatorChain();
            expressionOperatorChain.Add(firstExpression);

            while (binaryOperators.Contains(TokenStream.Current.Type))
            {
                var @operator = TokenStream.Current;
                expressionOperatorChain.Add(@operator);
                TokenStream.Consume();
                var nExpression = ParseTermOrPrefixExpression();
                expressionOperatorChain.Add(nExpression);
            }

            return expressionOperatorChain.SolveOperatorPrecendence(binaryOperators);
        }

        private IExpressionSyntax ParseTermOrPrefixExpression()
        {
            if (IsValidPrefixOperator(TokenStream.Current))
            {
                return ParsePrefixExpression();
            }

            var expression = ParseTermExpression();

            expression = ParsePostfixExpression(expression);
            return expression;
        }

        private IExpressionSyntax ParseTermExpression()
        {
            IExpressionSyntax expression;
            var current = TokenStream.Current;
            switch (current.Type)
            {
                case TokenType.TrueKeyword:
                    var trueToken = Consume(TokenType.FalseKeyword);
                    expression = new BooleanExpressionSyntax(trueToken);
                    break;
                case TokenType.FalseKeyword:
                    var falseToken = Consume(TokenType.FalseKeyword);
                    expression = new BooleanExpressionSyntax(falseToken);
                    break;
                case TokenType.Int:
                    Consume(TokenType.Int);
                    expression = new IntExpressionSyntax(current);
                    break;
                case TokenType.Float:
                    Consume(TokenType.Float);
                    expression = new FloatExpressionSyntax(current);
                    break;
                case TokenType.String:
                    Consume(TokenType.String);
                    expression = new StringExpressionSyntax(current);
                    break;
                case TokenType.Symbol:
                    Consume(TokenType.Symbol);
                    expression = new MemberExpressionSyntax(current);
                    break;
                case TokenType.OpenParenth:
                    expression =
                        (IExpressionSyntax)
                        ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression).Single();
                    break;
                case TokenType.NewKeyword:
                    expression = ParseNewExpression();
                    break;
                case TokenType.IfKeyword:
                    expression = ParseIfElseExpression();
                    break;
                default:
                    throw new KiwiSyntaxException(
                        $"Unexpected Token {current}. Expected Sign Operator, New, Int, Float, String or Symbol Expression.");
            }
            return expression;
        }

        private IExpressionSyntax ParsePrefixExpression()
        {
            var @operator = TokenStream.Current;
            if (!IsValidPrefixOperator(@operator))
            {
                throw new KiwiSyntaxException("Expected prefix operator");
            }

            TokenStream.Consume();
            return new SignExpressionSyntax(@operator, ParseTermOrPrefixExpression());
        }

        private IExpressionSyntax ParsePostfixExpression(IExpressionSyntax expression)
        {
            while (IsValidPostfixOperator(TokenStream.Current))
            {
                switch (TokenStream.Current.Type)
                {
                    case TokenType.Dot:
                        expression = ParseMemberAccessExpression(expression);
                        break;
                    case TokenType.OpenParenth:
                        expression = ParseInvocationExpression(expression);
                        break;
                    case TokenType.OpenBracket:
                        expression = ParseArrayAccessExpression(expression);
                        break;
                }
            }
            return expression;
        }
        
        private IExpressionSyntax ParseArrayAccessExpression(IExpressionSyntax expression)
        {
            var arrayParameter = new List<IExpressionSyntax>();
            while (TokenStream.Current.Type == TokenType.OpenBracket)
            {
                Consume(TokenType.OpenBracket);
                arrayParameter.Add(ParseExpression());
                Consume(TokenType.ClosingBracket);
            }
            expression = new ArrayAccessExpression(expression, arrayParameter);
            return expression;
        }

        private IExpressionSyntax ParseInvocationExpression(IExpressionSyntax expression)
        {
            var invokeParameter = ParseInnerCommmaSeperated(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                ParseExpression).Cast<IExpressionSyntax>().ToList();
            expression = new InvocationExpressionSyntax(expression, invokeParameter);
            return expression;
        }

        private IExpressionSyntax ParseMemberAccessExpression(IExpressionSyntax ownerExpression)
        {
            Consume(TokenType.Dot);
            var memberName = Consume(TokenType.Symbol);
            ownerExpression = new MemberAccessExpressionSyntax(ownerExpression, memberName);
            return ownerExpression;
        }

        private IfElseExpressionSyntax ParseIfElseExpression()
        {
            Consume(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression).Single();
            var ifTrueExpression = ParseExpression();
            Consume(TokenType.ElseKeyword);
            var ifFalseExpression = ParseExpression();
            return new IfElseExpressionSyntax((IExpressionSyntax)condition, ifTrueExpression, ifFalseExpression);
        }

        private IExpressionSyntax ParseNewExpression()
        {
            if (TokenStream.Current.Type != TokenType.NewKeyword)
            {
                return null;
            }

            Consume(TokenType.NewKeyword);
            var typeName = ParseSymbolOrBuildInType();
            if (TokenStream.Current.Type == TokenType.OpenBracket)
            {
                int dimension = 0;
                var sizes = new List<IExpressionSyntax>();
                for (; TokenStream.Current.Type == TokenType.OpenBracket; dimension++)
                {
                    Consume(TokenType.OpenBracket);
                    sizes.Add(ParseExpression());
                    Consume(TokenType.ClosingBracket);
                }
                return new ArrayCreationExpressionSyntax(typeName, sizes);
            }
            
            var parameter = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth,
                ParseExpression, true).Cast<IExpressionSyntax>().ToList();
            return new ObjectCreationExpressionSyntax(typeName, parameter);
        }

        private FunctionSyntax ParseFunction()
        {
            if (TokenStream.Current.Type != TokenType.FuncKeyword)
            {
                return null;
            }

            Consume(TokenType.FuncKeyword);
            var functionName = Consume(TokenType.Symbol);
            var functionParameter = ParseInnerCommmaSeperated(TokenType.OpenParenth, TokenType.ClosingParenth, _functionParameterParser);

            var hasReturnValue = TokenStream.Current.Type == TokenType.HypenGreater;
            if (hasReturnValue)
            {
                return ParseFunctionThatReturnValue(functionName, functionParameter);
            }

            var body = ParseScope(_functionBodyParser);

            return new FunctionSyntax(functionName, functionParameter.Cast<ParameterSyntax>().ToList(), body);
        }

        private FunctionSyntax ParseFunctionThatReturnValue(Token functionName, List<ISyntaxBase> functionParameter)
        {
            Consume(TokenType.HypenGreater);

            if (TokenStream.Current.Type == TokenType.DataKeyword)
            {
                return ParseDataFunction(functionName, functionParameter);
            }

            var returnType = ParseSymbolOrBuildInType();
            var body = ParseScope(_functionBodyParser);
            return new ReturnFunctionSyntax(functionName, functionParameter.Cast<ParameterSyntax>().ToList(), body, returnType);
        }

        private FunctionSyntax ParseDataFunction(Token functionName, List<ISyntaxBase> functionParameter)
        {
            var dataClass = ParseDataClass();
            var body = ParseScope(_functionBodyParser);
            return new DataClassFunctionSyntax(functionName, functionParameter.Cast<ParameterSyntax>().ToList(), body, dataClass);
        }

        private VariableDeclarationStatementSyntax ParseVariableDeclaration()
        {
            var current = TokenStream.Current;
            Token variableQualifier;
            switch (current.Type)
            {
                case TokenType.VarKeyword:
                case TokenType.ConstKeyword:
                    Consume(current.Type);
                    variableQualifier = current;
                    break;
                default:
                    return null;
            }
            var variableNames = new List<Token>();
            for (variableNames.Add(Consume(TokenType.Symbol));
                 TokenStream.Current.Type == TokenType.Comma;
                 variableNames.Add(Consume(TokenType.Symbol)))
            {
                Consume(TokenType.Comma);
            }
            Consume(TokenType.Colon);
            var initializer = ParseExpression();
            return new VariableDeclarationStatementSyntax(variableQualifier, variableNames, initializer);
        }

        private ReturnStatementSyntax ParseReturnStatement()
        {
            if (TokenStream.Current.Type != TokenType.ReturnKeyword)
            {
                return null;
            }

            Consume(TokenType.ReturnKeyword);
            var returnExpression = ParseExpression();
            return new ReturnStatementSyntax(returnExpression);
        }

        private DataSyntax ParseDataClass()
        {
            if (TokenStream.Current.Type != TokenType.DataKeyword)
            {
                return null;
            }

            Consume(TokenType.DataKeyword);
            var typeName = Consume(TokenType.Symbol);
            var parameter = ParseInnerCommmaSeperated(TokenType.OpenParenth, TokenType.ClosingParenth, _functionParameterParser);
            return new DataSyntax(typeName, parameter.Cast<ParameterSyntax>().ToList());
        }

        private ConstructorSyntax ParseConstructor()
        {
            if (TokenStream.Current.Type != TokenType.ConstructorKeyword)
            {
                return null;
            }

            Consume(TokenType.ConstructorKeyword);

            var argList = ParseInnerCommmaSeperated(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                _functionParameterParser);

            var bodySyntax = ParseScope(_functionBodyParser);

            return new ConstructorSyntax(argList, bodySyntax);
        }

        private ForInStatementSyntax ParseForInStatement()
        {
            if (TokenStream.Current.Type != TokenType.ForKeyword)
            {
                return null;
            }
            TokenStream.TakeSnapshot();

            Consume(TokenType.ForKeyword);
            IExpressionSyntax itemExpression;
            IExpressionSyntax collectionExpression;
            List<ISyntaxBase> body;
            bool declareItemInnerScope;
            if (!TryParseForIn(out itemExpression, out declareItemInnerScope, out collectionExpression, out body))
            {
                TokenStream.RollbackSnapshot();
                return null;
            }
            return new ForInStatementSyntax(itemExpression, declareItemInnerScope, collectionExpression, body);
        }

        private ForInStatementSyntax ParseReverseForInStatement()
        {
            if (TokenStream.Current.Type != TokenType.ForReverseKeyword)
            {
                return null;
            }
            TokenStream.TakeSnapshot();

            Consume(TokenType.ForReverseKeyword);
            IExpressionSyntax itemExpression;
            IExpressionSyntax collectionExpression;
            List<ISyntaxBase> body;
            bool declareItemInnerScope;
            if (!TryParseForIn(out itemExpression, out declareItemInnerScope, out collectionExpression, out body))
            {
                TokenStream.RollbackSnapshot();
                return null;
            }
            return new ReverseForInStatementSyntax(itemExpression, declareItemInnerScope, collectionExpression, body);
        }

        private bool TryParseForIn(out IExpressionSyntax itemExpression, out bool declareItemInnerScope, out IExpressionSyntax collectionExpression, out List<ISyntaxBase> body)
        {
            Consume(TokenType.OpenParenth);

            declareItemInnerScope = TokenStream.Current.Type == TokenType.VarKeyword;
            if (declareItemInnerScope)
            {
                Consume(TokenType.VarKeyword);
            }

            itemExpression = ParseExpression();

            if (TokenStream.Current.Type != TokenType.InKeyword)
            {
                collectionExpression = null;
                body = null;
                return false;
            }

            Consume(TokenType.InKeyword);
            collectionExpression = ParseExpression();
            Consume(TokenType.ClosingParenth);
            body = ParseScope(_functionBodyParser);
            return true;
        }

        private ForStatementSyntax ParseForStatement()
        {
            if (TokenStream.Current.Type != TokenType.ForKeyword)
            {
                return null;
            }
            
            Consume(TokenType.ForKeyword);
            Consume(TokenType.OpenParenth);

            var initExpression = ParseForInitExpression();
            Consume(TokenType.Semicolon);
            var condExpression = ParseExpression();
            Consume(TokenType.Semicolon);
            var loopExpression = ParseForInitExpression();
            Consume(TokenType.ClosingParenth);

            var hasScope = TokenStream.Current.Type == TokenType.OpenBrace;
            List<ISyntaxBase> body;
            if (hasScope)
            {
                body = ParseScope(_functionBodyParser);
            }
            else
            {
                body = new List<ISyntaxBase>() { _functionBodyParser() };
            }

            return new ForStatementSyntax(initExpression, condExpression, loopExpression, body);
        }

        private ISyntaxBase ParseForInitExpression()
        {
            var result = Parse(
                ParseVariableDeclaration,
                ParseAssignmentStatement,
                ParseExpression);

            
            if (!IsValidForInitExpression(result))
            {
                throw new KiwiSyntaxException(
                    "Only assignment call increment decrement and new object expressions can be used.");
            }

            return result;
        }

        private AssignmentStatementSyntax ParseAssignmentStatement()
        {
            if (TokenStream.Current.Type != TokenType.Symbol)
            {
                return null;
            }

            TokenStream.TakeSnapshot();
            var member = ParseExpression();

            if (!(member is MemberAccessExpressionSyntax || member is ArrayAccessExpression
                 || member is MemberExpressionSyntax))
            {
                TokenStream.RollbackSnapshot();
                return null;
            }

            var current = TokenStream.Current;
            if (!IsValidAssignOperator(current))
            {
                throw new KiwiSyntaxException("Unexpected assign operator {0}. Expected :, :+, :-, :/, :* or :^.");
            }
            Consume(current.Type);
            var intializer = ParseExpression();
            return new AssignmentStatementSyntax(member, current, intializer);
        }


        private SwitchStatementSyntax ParseSwitchStatement()
        {
            if (TokenStream.Current.Type != TokenType.SwitchKeyword)
            {
                return null;
            }

            Consume(TokenType.SwitchKeyword);
            Consume(TokenType.OpenParenth);
            var condition = ParseExpression();
            Consume(TokenType.ClosingParenth);
            var caseAndDefaultSyntax = ParseScope(() => Parse(ParseCase, ParseDefault));

            DefaultSyntax defaultSyntax;
            try
            {
                defaultSyntax = caseAndDefaultSyntax.SingleOrDefault(x => x is DefaultSyntax) as DefaultSyntax;
            }
            catch
            {
                throw new KiwiSyntaxException("Duplicate default label");
            }

            return new SwitchStatementSyntax(condition, caseAndDefaultSyntax.OfType<CaseSyntax>().ToList(), defaultSyntax);
        }

        private DefaultSyntax ParseDefault()
        {
            if (TokenStream.Current.Type != TokenType.DefaultKeyword)
            {
                return null;
            }

            Consume(TokenType.DefaultKeyword);
            Consume(TokenType.HypenGreater);

            var hasScope = TokenStream.Current.Type == TokenType.OpenBrace;
            List<ISyntaxBase> body;
            if (hasScope)
            {
                body = ParseScope(_functionBodyParser);
            }
            else
            {
                body = new List<ISyntaxBase>() { _functionBodyParser() };
            }

            return new DefaultSyntax(body);
        }

        private ISyntaxBase ParseCase()
        {
            if (TokenStream.Current.Type != TokenType.CaseKeyword)
            {
                return null;
            }

            Consume(TokenType.CaseKeyword);
            var expression = ParseExpression();
            Consume(TokenType.HypenGreater);

            var hasScope = TokenStream.Current.Type == TokenType.OpenBrace;
            List<ISyntaxBase> body;
            if (hasScope)
            {
                body = ParseScope(_functionBodyParser);
            }
            else
            {
                body = new List<ISyntaxBase>() { _functionBodyParser() };
            }

            return new CaseSyntax(expression, body);
        }

        private ISyntaxBase ParseWhenStatement()
        {
            if (TokenStream.Current.Type != TokenType.WhenKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private IfStatementSyntax ParseIfStatement()
        {
            if (TokenStream.Current.Type != TokenType.IfKeyword)
            {
                return null;
            }

            Consume(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression);
            var body = ParseScope(_functionBodyParser);
            if (TokenStream.Current.Type == TokenType.ElseKeyword)
            {
                return ParseIfElseStatement(condition, body);
            }
            return new IfStatementSyntax(condition, body.Cast<IStatetementSyntax>().ToList());
        }

        private IfElseStatementSyntax ParseIfElseStatement(List<ISyntaxBase> condition, List<ISyntaxBase> body)
        {
            Consume(TokenType.ElseKeyword);
            var elseBody = ParseScope(_functionBodyParser);
            return new IfElseStatementSyntax(condition, body.Cast<IStatetementSyntax>().ToList(), elseBody.Cast<IStatetementSyntax>().ToList());
        }

        private ParameterSyntax ParseParameter()
        {
            var typeName = ParseSymbolOrBuildInType();

            var parameterName = Consume(TokenType.Symbol);
            return new ParameterSyntax(typeName, parameterName);
        }

        private TypeSyntax ParseSymbolOrBuildInType()
        {
            var buildInTypeNames = new[]
                                   {
                                       TokenType.IntKeyword,
                                       TokenType.FloatKeyword,
                                   };

            var current = TokenStream.Current;
            Token typeName;
            if (buildInTypeNames.Contains(current.Type))
            {
                TokenStream.Consume();
                typeName = current;
            }
            else
            {
                typeName = Consume(TokenType.Symbol);
            }

            return new TypeSyntax(typeName);
        }

        private ParameterSyntax ParseParamsParameter()
        {
            if (TokenStream.Current.Type != TokenType.TwoDots)
            {
                return null;
            }

            Consume(TokenType.TwoDots);
            var typeName = ParseSymbolOrBuildInType();
            var parameterName = Consume(TokenType.Symbol);
            return new ParameterSyntax(typeName, parameterName);
        }

        private EnumSyntax ParseEnum()
        {
            if (TokenStream.Current.Type != TokenType.EnumKeyword)
            {
                return null;
            }

            Consume(TokenType.EnumKeyword);
            var enumName = Consume(TokenType.Symbol);
            var memberSyntax = ParseInnerCommmaSeperated(
                TokenType.OpenBrace,
                TokenType.ClosingBrace,
                ParseEnumMember);
            return new EnumSyntax(enumName, memberSyntax);
        }

        private ISyntaxBase ParseEnumMember()
        {
            var memberName = Consume(TokenType.Symbol);
            var hasExplicitValue = TokenStream.Current.Type == TokenType.Colon;
            IExpressionSyntax initializer = null;
            if (hasExplicitValue)
            {
                Consume(TokenType.Colon);
                initializer = ParseExpression();
            }
            return new EnumMemberSyntax(memberName, initializer);
        }

        private List<ISyntaxBase> ParseScope(Func<ISyntaxBase> parser)
        {
            return ParseInner(TokenType.OpenBrace, TokenType.ClosingBrace, parser);
        }

        private static List<Token> PrepareTokenSource(List<Token> token)
        {
            return token.Where(x => x.Type != TokenType.Whitespace)
                        .Where(x => x.Type != TokenType.Tab)
                        .Where(x => x.Type != TokenType.NewLine)
                        .Where(x => x.Type != TokenType.Comment)
                        .ToList();
        }

        private void InitParser()
        {
            _namespaceBodyParser = () => Parse(ParseClass, ParseDataClass, ParseEnum);
            _classBodyParser = () => Parse(ParseConstructor, ParseFunction, ParseField);
            _functionParameterParser = () => Parse(ParseParameter, ParseParamsParameter);
            _functionBodyParser = () => Parse(
                ParseIfStatement,
                ParseReturnStatement,
                ParseIfStatement,
                ParseWhenStatement,
                ParseSwitchStatement,
                ParseVariableDeclaration,
                ParseAssignmentStatement,
                ParseForInStatement,
                ParseReverseForInStatement,
                ParseForStatement);
        }

        private static bool IsValidForInitExpression(ISyntaxBase result)
        {
            return new[]
                   {
                       typeof(AssignmentStatementSyntax),
                       typeof(VariableDeclarationStatementSyntax),
                       typeof(CallExpressionSyntax),
                       typeof(ObjectCreationExpressionSyntax)
                   }.Contains(result.GetType());
        }

        private static bool IsValidAssignOperator(Token token)
        {
            return new[]
                   {
                       TokenType.Colon,
                       TokenType.ColonDiv,
                       TokenType.ColonMult,
                       TokenType.ColonAdd,
                       TokenType.ColonPow,
                       TokenType.ColonSub
                   }.Contains(token.Type);
        }

        private static bool IsValidPostfixOperator(Token token)
        {
            return new[] { TokenType.Dot, TokenType.OpenParenth, TokenType.OpenBracket }.Contains(token.Type);
        }
        
        private static bool IsValidPrefixOperator(Token token)
        {
            return new[]
                   {
                       TokenType.Add,
                       TokenType.Sub
                   }.Contains(token.Type);
        }
    }
}