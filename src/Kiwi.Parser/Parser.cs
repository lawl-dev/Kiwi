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
        public Parser(List<Token> token) : base(new TransactableTokenStream(RemoveUnnecessaryToken(token)))
        {
        }

        private static string[] OperatorFunctionNames => new[]
                                                         {
                                                             "Add",
                                                             "Sub",
                                                             "Mult",
                                                             "Div",
                                                             "Pow",
                                                             "Range",
                                                             "In",
                                                             "CompareTo"
                                                         };

        private static TokenType[] BinaryOperators => new[]
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
                                                          TokenType.InKeyword,
                                                          TokenType.NotInKeyword,
                                                          TokenType.IsKeyword,
                                                          TokenType.AndKeyword,
                                                          TokenType.NotEqual
                                                      };

        private static TokenType[] BuildInTypes => new[]
                                                   {
                                                       TokenType.IntKeyword,
                                                       TokenType.FloatKeyword,
                                                       TokenType.StringKeyword
                                                   };

        private static TokenType[] AssignOperators => new[]
                                                      {
                                                          TokenType.Colon,
                                                          TokenType.ColonDiv,
                                                          TokenType.ColonMult,
                                                          TokenType.ColonAdd,
                                                          TokenType.ColonPow,
                                                          TokenType.ColonSub
                                                      };

        private static TokenType[] PostfixOperators => new[]
                                                       {
                                                           TokenType.Dot,
                                                           TokenType.OpenParenth,
                                                           TokenType.OpenBracket
                                                       };

        private static TokenType[] PrefixOperators => new[]
                                                      {
                                                          TokenType.Add,
                                                          TokenType.Sub,
                                                          TokenType.NotKeyword
                                                      };

        public CompilationUnitSyntax Parse()
        {
            var usings = new List<UsingSyntax>();
            var namespaces = new List<NamespaceSyntax>();

            while (TokenStream.Current != null)
            {
                switch (TokenStream.Current.Type)
                {
                    case TokenType.UsingKeyword:
                        usings.Add(ParseUsing());
                        break;
                    case TokenType.NamespaceKeyword:
                        namespaces.Add(ParseNamespace());
                        break;
                    default:
                        throw new KiwiSyntaxException("Unexpected Token. Expected Using or Namespace");
                }
            }
            return new CompilationUnitSyntax(usings, namespaces);
        }

        private NamespaceSyntax ParseNamespace()
        {
            ParseExpected(TokenType.NamespaceKeyword);
            var namespaceName = ParseExpected(TokenType.Identifier);
            var body = ParseInner(ParseNamespaceBody);
            return new NamespaceSyntax(
                namespaceName,
                body.OfType<ClassSyntax>().ToList(),
                body.OfType<DataSyntax>().ToList(),
                body.OfType<EnumSyntax>().ToList(),
                body.OfType<FunctionSyntax>().ToList());
        }

        private ISyntaxBase ParseNamespaceBody()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.ClassKeyword:
                    return ParseClass();
                case TokenType.DataKeyword:
                    return ParseDataClass();
                case TokenType.EnumKeyword:
                    return ParseEnum();
                case TokenType.FuncKeyword:
                    return ParseFunction();
                case TokenType.InfixKeyword:
                    return ParseInfixFunction();
                default:
                    throw new KiwiSyntaxException("Unexpected Token. Expected Class, Data or Enum");
            }
        }

        private UsingSyntax ParseUsing()
        {
            ParseExpected(TokenType.UsingKeyword);
            var namespaceName = ParseExpected(TokenType.Identifier);
            return new UsingSyntax(namespaceName);
        }

        private ClassSyntax ParseClass()
        {
            ParseExpected(TokenType.ClassKeyword);
            var className = ParseExpected(TokenType.Identifier);

            Token descriptorName = null;
            if (ParseOptional(TokenType.IsKeyword))
            {
                descriptorName = ParseExpected(TokenType.Identifier);
            }

            var inner = ParseInner(ParseClassBody);
            return new ClassSyntax(
                className,
                descriptorName,
                inner.OfType<ConstructorSyntax>().ToList(),
                inner.OfType<FunctionSyntax>().ToList(),
                inner.OfType<FieldSyntax>().ToList());
        }

        private ISyntaxBase ParseClassBody()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.ConstructorKeyword:
                    return ParseConstructor();
                case TokenType.FuncKeyword:
                    return ParseFunction();
                case TokenType.Operator:
                    return ParseOperator();
                case TokenType.InfixKeyword:
                    return ParseInfixFunction();
                case TokenType.VarKeyword:
                case TokenType.ImmutKeyword:
                    return ParseField();
                default:
                    throw new KiwiSyntaxException("Unexpected Token. Expected Constructor, Func, Var or Const");
            }
        }

        private FieldSyntax ParseField()
        {
            var isImmutable = TokenStream.Current.Type == TokenType.ImmutKeyword;
            var fieldTypeQualifier = ParseExpected(isImmutable ? TokenType.ImmutKeyword : TokenType.VarKeyword);
            var fieldName = ParseExpected(TokenType.Identifier);
            ParseExpected(TokenType.Colon);
            var fieldInitializer = ParseExpression();

            return new FieldSyntax(fieldTypeQualifier, fieldName, fieldInitializer);
        }

        private IExpressionSyntax ParseExpression()
        {
            var firstExpression = ParseTermOrPrefixExpression();

            if (!BinaryOperators.Contains(TokenStream.Current.Type))
            {
                return firstExpression;
            }

            var expressionOperatorChain = new ExpressionOperatorChain();
            expressionOperatorChain.Add(firstExpression);

            while (BinaryOperators.Contains(TokenStream.Current.Type))
            {
                var @operator = TokenStream.Current;
                expressionOperatorChain.Add(@operator);
                TokenStream.Consume();
                var nExpression = ParseTermOrPrefixExpression();
                expressionOperatorChain.Add(nExpression);
            }

            return expressionOperatorChain.SolveOperatorPrecendence(BinaryOperators);
        }

        private IExpressionSyntax ParseTermOrPrefixExpression()
        {
            if (IsPrefixOperator(TokenStream.Current))
            {
                return ParsePrefixExpression();
            }

            var expression = ParseTermExpression();

            while (IsPossibleInfix())
            {
                expression = new InfixFunctionInvocationExpressionSyntax(expression, ParseMemberExpression(), ParseExpression());
            }

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
                    expression = ParseTrueExpression();
                    break;
                case TokenType.FalseKeyword:
                    expression = ParseFalseExpression();
                    break;
                case TokenType.Int:
                    expression = ParseIntExpression();
                    break;
                case TokenType.Float:
                    expression = ParseFloatExpression();
                    break;
                case TokenType.String:
                    expression = ParseStringExpression();
                    break;
                case TokenType.Identifier:
                    expression = ParseMemberExpression();
                    break;
                case TokenType.OpenParenth:
                    expression = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression).Single();
                    break;
                case TokenType.NewKeyword:
                    expression = ParseNewExpression();
                    break;
                case TokenType.IfKeyword:
                    expression = ParseIfElseExpression();
                    break;
                case TokenType.FuncKeyword:
                    expression = ParseFunctionExpression();
                    break;
                default:
                    throw new KiwiSyntaxException(
                        $"Unexpected Token {current}. Expected Sign Operator, New, Int, Float, String or Identifier Expression.");
            }
            return expression;
        }

        private IdentifierExpressionSyntax ParseMemberExpression()
        {
            var expression = new IdentifierExpressionSyntax(ParseExpected(TokenType.Identifier));
            return expression;
        }

        private IExpressionSyntax ParseStringExpression()
        {
            IExpressionSyntax expression = new StringExpressionSyntax(ParseExpected(TokenType.String));
            return expression;
        }

        private IExpressionSyntax ParseFloatExpression()
        {
            IExpressionSyntax expression = new FloatExpressionSyntax(ParseExpected(TokenType.Float));
            return expression;
        }

        private IExpressionSyntax ParseIntExpression()
        {
            IExpressionSyntax expression = new IntExpressionSyntax(ParseExpected(TokenType.Int));
            return expression;
        }

        private IExpressionSyntax ParseFalseExpression()
        {
            IExpressionSyntax expression = new BooleanExpressionSyntax(ParseExpected(TokenType.FalseKeyword));
            return expression;
        }

        private IExpressionSyntax ParseTrueExpression()
        {
            var trueToken = ParseExpected(TokenType.TrueKeyword);
            IExpressionSyntax expression = new BooleanExpressionSyntax(trueToken);
            return expression;
        }

        private IExpressionSyntax ParseFunctionExpression()
        {
            ParseExpected(TokenType.FuncKeyword);
            TokenStream.TakeSnapshot();
            try
            {
                var anonymousFunctionExpression = ParseAnonymousFunctionExpression();
                TokenStream.CommitSnapshot();
                return anonymousFunctionExpression;
            }
            catch
            {
                TokenStream.RollbackSnapshot();
            }

            var implicitParameterList = ParseExpressionList();
            ParseExpected(TokenType.HypenGreater);
            var statement = ParseScopeOrSingleStatement();
            return new ImplicitParameterTypeAnonymousFunctionExpressionSyntax(implicitParameterList, statement);
        }

        private IExpressionSyntax ParseAnonymousFunctionExpression()
        {
            var parameter = ParseParameterList();
            ParseExpected(TokenType.HypenGreater);
            var statement = ParseScopeOrSingleStatement();
            return new AnonymousFunctionExpressionSyntax(parameter, statement);
        }

        private List<IExpressionSyntax> ParseExpressionList()
        {
            return ParseInnerCommmaSeperated(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression);
        }

        private List<ParameterSyntax> ParseParameterList()
        {
            return ParseInnerCommmaSeperated(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                ParseParameter);
        }

        private IExpressionSyntax ParsePrefixExpression()
        {
            var @operator = TokenStream.Current;
            if (!IsPrefixOperator(@operator))
            {
                throw new KiwiSyntaxException("Expected prefix operator");
            }

            TokenStream.Consume();
            return new SignExpressionSyntax(@operator, ParseTermOrPrefixExpression());
        }

        private IExpressionSyntax ParsePostfixExpression(IExpressionSyntax expression)
        {
            while (IsPostfixOperator(TokenStream.Current))
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
                ParseExpected(TokenType.OpenBracket);
                arrayParameter.Add(ParseExpression());
                ParseExpected(TokenType.ClosingBracket);
            }
            expression = new ArrayAccessExpressionSyntax(expression, arrayParameter);
            return expression;
        }

        private IExpressionSyntax ParseInvocationExpression(IExpressionSyntax expression)
        {
            var invokeParameter = ParseExpressionList();
            expression = new InvocationExpressionSyntax(expression, invokeParameter);
            return expression;
        }

        private IExpressionSyntax ParseMemberAccessExpression(IExpressionSyntax ownerExpression)
        {
            ParseExpected(TokenType.Dot);
            var memberName = ParseExpected(TokenType.Identifier);
            ownerExpression = new MemberAccessExpressionSyntax(ownerExpression, memberName);
            return ownerExpression;
        }

        private IfElseExpressionSyntax ParseIfElseExpression()
        {
            ParseExpected(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression).Single();
            var ifTrueExpression = ParseExpression();
            ParseExpected(TokenType.ElseKeyword);
            var ifFalseExpression = ParseExpression();
            return new IfElseExpressionSyntax(condition, ifTrueExpression, ifFalseExpression);
        }

        private IExpressionSyntax ParseNewExpression()
        {
            ParseExpected(TokenType.NewKeyword);
            var typeName = ParseSymbolOrBuildInType();
            if (TokenStream.Current.Type == TokenType.OpenBracket)
            {
                var dimension = 0;
                var sizes = new List<IExpressionSyntax>();
                for (; TokenStream.Current.Type == TokenType.OpenBracket; dimension++)
                {
                    ParseExpected(TokenType.OpenBracket);
                    sizes.Add(ParseExpression());
                    ParseExpected(TokenType.ClosingBracket);
                }
                return new ArrayCreationExpressionSyntax(typeName, sizes);
            }

            var parameter = ParseInner(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                ParseExpression,
                true).ToList();
            return new ObjectCreationExpressionSyntax(typeName, parameter);
        }

        private FunctionSyntax ParseFunction()
        {
            ParseExpected(TokenType.FuncKeyword);
            var functionName = ParseExpected(TokenType.Identifier);
            var functionParameter = ParseParameterList();

            IStatementSyntax statements;
            TypeSyntax returnType = null;
            if (ParseOptional(TokenType.HypenGreater))
            {
                switch (TokenStream.Current.Type)
                {
                    case TokenType.ReturnKeyword:
                        statements = ParseReturnStatement();
                        break;
                    default:
                        returnType = ParseSimpleTypeOrArrayType();
                        statements = ParseScope(ParseStatement);
                        break;
                }
            }
            else
            {
                statements = ParseScope(ParseStatement);
            }

            return new FunctionSyntax(functionName, functionParameter, statements, returnType);
        }

        private FunctionSyntax ParseOperator()
        {
            ParseExpected(TokenType.Operator);
            var functionName = ParseExpected(TokenType.Identifier);
            Ensure(() => IsOperatorFunctionName(functionName.Value), $"Invalid operator function name '{functionName.Value}'");
            var functionParameter = ParseParameterList();
            
            IStatementSyntax statements;
            TypeSyntax returnType = null;
            if (ParseOptional(TokenType.HypenGreater))
            {
                switch (TokenStream.Current.Type)
                {
                    case TokenType.ReturnKeyword:
                        statements = ParseReturnStatement();
                        break;
                    default:
                        returnType = ParseSimpleTypeOrArrayType();
                        statements = ParseScope(ParseStatement);
                        break;
                }
            }
            else
            {
                statements = ParseScope(ParseStatement);
            }

            return new OperatorFunctionSyntax(functionName, functionParameter, statements, returnType);
        }

        private InfixFunctionSyntax ParseInfixFunction()
        {
            ParseExpected(TokenType.InfixKeyword);
            ParseExpected(TokenType.FuncKeyword);
            var functionName = ParseExpected(TokenType.Identifier);
            var functionParameter = ParseParameterList();
            Ensure(() => functionParameter.Count == 2, "infix' modifier is inapplicable on this function. Parameter count != 2");
            ParseExpected(TokenType.HypenGreater);

            IStatementSyntax statements;
            TypeSyntax returnType = null;
            switch (TokenStream.Current.Type)
            {
                case TokenType.ReturnKeyword:
                    statements = ParseReturnStatement();
                    break;
                default:
                    returnType = ParseSimpleTypeOrArrayType();
                    statements = ParseScope(ParseStatement);
                    break;
            }

            return new InfixFunctionSyntax(functionName, functionParameter, statements, returnType);
        }

        private IStatementSyntax ParseStatement()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.IfKeyword:
                    return ParseIfStatement();
                case TokenType.ReturnKeyword:
                    return ParseReturnStatement();
                case TokenType.MatchKeyword:
                    return ParseWhenStatement();
                case TokenType.SwitchKeyword:
                    return ParseSwitchStatement();
                case TokenType.VarKeyword:
                case TokenType.ImmutKeyword:
                    return ParseVariablesDeclarationStatement();
                case TokenType.Identifier:
                    return ParseFunctionCallOrAssignStatement();
                case TokenType.ForKeyword:
                    return ParseForInOrForStatement();
                case TokenType.ForReverseKeyword:
                    return ParseForReverseStatement();
                case TokenType.OpenBrace:
                    return ParseScope(ParseStatement);
                default:
                    throw new KiwiSyntaxException(
                        $"Unexpected Token \"{TokenStream.Current.Value}\". Expected If, Return, When, Switch, Var, Const, Identifier, For or Forr");
            }
        }

        private IStatementSyntax ParseVariablesDeclarationStatement()
        {
            var declarations = ParseVariableDeclarations();
            if (declarations.Count == 1)
            {
                return declarations[0];
            }

            return new VariablesDeclarationStatementSyntax(declarations);
        }

        private IStatementSyntax ParseForReverseStatement()
        {
            ParseExpected(TokenType.ForReverseKeyword);
            ParseExpected(TokenType.OpenParenth);
            if (TokenStream.Current.Type == TokenType.VarKeyword)
            {
                var variableDeclaration = ParseVariablesDeclarationStatement();
                var variableDeclarationStatement = variableDeclaration as VariableDeclarationStatementSyntax;
                if (variableDeclarationStatement?.InitExpression == null)
                {
                    ParseExpected(TokenType.InKeyword);
                    var collExpression = ParseExpression();
                    ParseExpected(TokenType.ClosingParenth);
                    var statement = ParseScope(ParseStatement);
                    return new ReverseForInStatementSyntax(variableDeclarationStatement, collExpression, statement);
                }
                throw new KiwiSyntaxException("Expected Variable declaration without init");
            }

            if (TokenStream.Peek(1).Type == TokenType.InKeyword)
            {
                var expression = ParseTermExpression();
                ParseExpected(TokenType.InKeyword);
                var collExpression = ParseExpression();
                ParseExpected(TokenType.ClosingParenth);
                var statement = ParseScope(ParseStatement);
                return new ReverseForInStatementSyntax(expression, collExpression, statement);
            }

            throw new KiwiSyntaxException("Unexpected Statement. Expected AssignmentStatement");
        }

        private IStatementSyntax ParseForInOrForStatement()
        {
            ParseExpected(TokenType.ForKeyword);
            ParseExpected(TokenType.OpenParenth);
            if (TokenStream.Current.Type == TokenType.VarKeyword)
            {
                var variableDeclaration = ParseVariablesDeclarationStatement();
                var variableDeclarationStatement = variableDeclaration as VariableDeclarationStatementSyntax;
                if (variableDeclaration is VariablesDeclarationStatementSyntax
                    || variableDeclarationStatement?.InitExpression != null)
                {
                    ParseExpected(TokenType.Semicolon);
                    var condExpression = ParseExpression();
                    ParseExpected(TokenType.Semicolon);
                    var incExpression = ParseFunctionCallOrAssignStatement();
                    ParseExpected(TokenType.ClosingParenth);
                    var statement = ParseScope(ParseStatement);
                    return new ForStatementSyntax(variableDeclaration, condExpression, incExpression, statement);
                }
                if (variableDeclarationStatement?.InitExpression == null)
                {
                    ParseExpected(TokenType.InKeyword);
                    var collExpression = ParseExpression();
                    ParseExpected(TokenType.ClosingParenth);
                    var statement = ParseScope(ParseStatement);
                    return new ForInStatementSyntax(variableDeclarationStatement, collExpression, statement);
                }
            }

            if (TokenStream.Peek(1).Type == TokenType.InKeyword)
            {
                var expression = ParseTermExpression();
                ParseExpected(TokenType.InKeyword);
                var collExpression = ParseExpression();
                ParseExpected(TokenType.ClosingParenth);
                var statement = ParseScope(ParseStatement);
                return new ForInStatementSyntax(expression, collExpression, statement);
            }

            var assignmentStatement = ParseFunctionCallOrAssignStatement() as AssignmentStatementSyntax;
            if (assignmentStatement != null)
            {
                ParseExpected(TokenType.Semicolon);
                var condExpression = ParseExpression();
                ParseExpected(TokenType.Semicolon);
                var incrementStatement = ParseStatement();
                ParseExpected(TokenType.ClosingParenth);
                var statement = ParseScope(ParseStatement);
                return new ForStatementSyntax(assignmentStatement, condExpression, incrementStatement, statement);
            }

            throw new KiwiSyntaxException("Unexpected Statement. Expected AssignmentStatement");
        }

        private IStatementSyntax ParseFunctionCallOrAssignStatement()
        {
            var expression = ParseExpression();

            if (IsMemberExpression(expression))
            {
                var current = TokenStream.Current;
                if (!IsAssignOperator(current))
                {
                    throw new KiwiSyntaxException(
                        $"Unexpected assign operator {current}. Expected :, :+, :-, :/, :* or :^.");
                }
                ParseExpected(current.Type);
                var intializer = ParseExpression();
                return new AssignmentStatementSyntax(expression, current, intializer);
            }

            var invocationExpression = expression as InvocationExpressionSyntax;
            if (invocationExpression != null)
            {
                return new InvocationStatementSyntax(invocationExpression);
            }

            throw new KiwiSyntaxException(
                "Unexpected Syntax. Expected MemberAccessExpressionSyntax, ArrayAccessExpression, IdentifierExpressionSyntax or InvocationExpressionSyntax");
        }

        private static bool IsMemberExpression(IExpressionSyntax expression)
        {
            return expression is MemberAccessExpressionSyntax || expression is ArrayAccessExpressionSyntax
                   || expression is IdentifierExpressionSyntax;
        }

        private ParameterSyntax ParseParameter()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.TwoDots:
                    return ParseParamsParameter();
                case TokenType.IntKeyword:
                case TokenType.FloatKeyword:
                case TokenType.StringKeyword:
                case TokenType.Identifier:
                    return ParseSimpleParameter();
                default:
                    throw new KiwiSyntaxException("Unexpected Token. Expected .., Int, Float, String or Identifier");
            }
        }


        private TypeSyntax ParseSimpleTypeOrArrayType()
        {
            var type = ParseSymbolOrBuildInType();
            if (TokenStream.Current.Type == TokenType.OpenBracket)
            {
                var dimension = 0;
                for (; TokenStream.Current.Type == TokenType.OpenBracket; dimension++)
                {
                    ParseExpected(TokenType.OpenBracket);
                    ParseExpected(TokenType.ClosingBracket);
                }
                return new ArrayTypeSyntax(type.TypeName, dimension);
            }
            return type;
        }

        private List<VariableDeclarationStatementSyntax> ParseVariableDeclarations()
        {
            var current = TokenStream.Current;
            Token variableQualifier;
            switch (current.Type)
            {
                case TokenType.VarKeyword:
                case TokenType.ImmutKeyword:
                    ParseExpected(current.Type);
                    variableQualifier = current;
                    break;
                default:
                    throw new KiwiSyntaxException("Expected qualifier");
            }
            var variableDeclarationStatements = new List<VariableDeclarationStatementSyntax>();
            while (TokenStream.Current.Type == TokenType.Identifier)
            {
                var identifier = ParseExpected(TokenType.Identifier);
                IExpressionSyntax initExpression = null;
                if (ParseOptional(TokenType.Colon))
                {
                    initExpression = ParseExpression();
                }
                variableDeclarationStatements.Add(
                    new VariableDeclarationStatementSyntax(variableQualifier, identifier, initExpression));
                ParseOptional(TokenType.Comma);
            }
            return variableDeclarationStatements;
        }

        private ReturnStatementSyntax ParseReturnStatement()
        {
            ParseExpected(TokenType.ReturnKeyword);
            var returnExpression = ParseExpression();
            return new ReturnStatementSyntax(returnExpression);
        }

        private DataSyntax ParseDataClass()
        {
            ParseExpected(TokenType.DataKeyword);
            var typeName = ParseExpected(TokenType.Identifier);
            var parameter = ParseParameterList();
            return new DataSyntax(typeName, parameter.ToList());
        }

        private ConstructorSyntax ParseConstructor()
        {
            ParseExpected(TokenType.ConstructorKeyword);

            var parameter = ParseParameterList();
            var bodySyntax = ParseScope(ParseStatement);
            return new ConstructorSyntax(parameter, bodySyntax);
        }

        private IStatementSyntax ParseScopeOrSingleStatement()
        {
            var hasScope = TokenStream.Current.Type == TokenType.OpenBrace;
            var statements = hasScope ? ParseScope(ParseStatement) : ParseStatement();
            return statements;
        }

        private SwitchStatementSyntax ParseSwitchStatement()
        {
            ParseExpected(TokenType.SwitchKeyword);
            ParseExpected(TokenType.OpenParenth);
            var condition = ParseExpression();
            ParseExpected(TokenType.ClosingParenth);
            var caseAndDefaultSyntax = ParseInner(ParseCaseOrElse);

            ElseSyntax elseSyntax;
            try
            {
                elseSyntax = caseAndDefaultSyntax.SingleOrDefault(x => x is ElseSyntax) as ElseSyntax;
            }
            catch
            {
                throw new KiwiSyntaxException("Duplicate else label");
            }

            return new SwitchStatementSyntax(condition, caseAndDefaultSyntax.OfType<CaseSyntax>().ToList(), elseSyntax);
        }

        private ISyntaxBase ParseCaseOrElse()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.CaseKeyword:
                    return ParseCase();
                case TokenType.ElseKeyword:
                    return ParseElse();
                default:
                    throw new KiwiSyntaxException("Unexpected Token. Expected Case or Else");
            }
        }

        private ElseSyntax ParseElse()
        {
            ParseExpected(TokenType.ElseKeyword);
            ParseExpected(TokenType.HypenGreater);

            var statements = ParseScopeOrSingleStatement();
            return new ElseSyntax(statements);
        }

        private ISyntaxBase ParseCase()
        {
            ParseExpected(TokenType.CaseKeyword);
            var expression = ParseExpression();
            ParseExpected(TokenType.HypenGreater);

            var statements = ParseScopeOrSingleStatement();
            return new CaseSyntax(expression, statements);
        }

        private IStatementSyntax ParseWhenStatement()
        {
            ParseExpected(TokenType.MatchKeyword);

            if (TokenStream.Current.Type == TokenType.OpenParenth)
            {
                return ParseConditionalWhenStatement();
            }
            return ParseSimpleWhenStatement();
        }

        private SimpleMatchStatementSyntax ParseSimpleWhenStatement()
        {
            var whenEntries = ParseInner(TokenType.OpenBrace, TokenType.ClosingBrace, ParseWhenSimpleWhenEntry);
            return new SimpleMatchStatementSyntax(whenEntries);
        }

        private WhenEntry ParseWhenSimpleWhenEntry()
        {
            ParseExpected(TokenType.CaseKeyword);
            var condition = ParseExpression();
            ParseExpected(TokenType.HypenGreater);
            var statements = ParseScopeOrSingleStatement();
            return new WhenEntry(condition, statements);
        }

        private ConditionalMatchStatementSyntax ParseConditionalWhenStatement()
        {
            ParseExpected(TokenType.OpenParenth);
            var condition = ParseExpression();
            ParseExpected(TokenType.ClosingParenth);

            var whenEntries = ParseInner(TokenType.OpenBrace, TokenType.ClosingBrace, ParseConditionalWhenEntry);
            return new ConditionalMatchStatementSyntax(condition, whenEntries);
        }

        private ConditionalWhenEntry ParseConditionalWhenEntry()
        {
            ParseExpected(TokenType.CaseKeyword);
            var binaryOperators = BinaryOperators;
            if (!binaryOperators.Contains(TokenStream.Current.Type))
            {
                throw new KiwiSyntaxException("Expected a binary operator");
            }

            var op = TokenStream.Current;
            TokenStream.Consume();

            var condition1 = ParseExpression();

            if (op.Type == TokenType.InKeyword)
            {
                condition1 = ParseWhenInExpression(condition1);
            }

            ParseExpected(TokenType.HypenGreater);
            var statements = ParseScopeOrSingleStatement();
            return new ConditionalWhenEntry(op, condition1, statements);
        }

        private WhenInExpressionSyntax ParseWhenInExpression(IExpressionSyntax condition1)
        {
            var inExpressionList = new List<IExpressionSyntax>();
            while (ParseOptional(TokenType.Comma))
            {
                var conditionN = ParseExpression();
                inExpressionList.Add(conditionN);
            }
            inExpressionList.Insert(0, condition1);
            return new WhenInExpressionSyntax(inExpressionList);
        }

        private IfStatementSyntax ParseIfStatement()
        {
            ParseExpected(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression).Single();
            var statements = ParseScope(ParseStatement);
            if (TokenStream.Current.Type == TokenType.ElseKeyword)
            {
                return ParseIfElseStatement(condition, statements);
            }
            return new IfStatementSyntax(condition, statements);
        }

        private IfElseStatementSyntax ParseIfElseStatement(
            IExpressionSyntax condition,
            ScopeStatementSyntax statements)
        {
            ParseExpected(TokenType.ElseKeyword);
            var elseBody = ParseScope(ParseStatement);
            return new IfElseStatementSyntax(condition, statements, elseBody);
        }

        private ParameterSyntax ParseSimpleParameter()
        {
            var typeName = ParseSimpleTypeOrArrayType();

            var parameterName = ParseExpected(TokenType.Identifier);
            return new ParameterSyntax(typeName, parameterName);
        }

        private TypeSyntax ParseSymbolOrBuildInType()
        {
            var current = TokenStream.Current;
            Token typeName;
            if (BuildInTypes.Contains(current.Type))
            {
                TokenStream.Consume();
                typeName = current;
            }
            else
            {
                typeName = ParseExpected(TokenType.Identifier);
            }

            return new TypeSyntax(typeName);
        }

        private ParameterSyntax ParseParamsParameter()
        {
            ParseExpected(TokenType.TwoDots);
            var typeName = ParseSimpleTypeOrArrayType();
            var parameterName = ParseExpected(TokenType.Identifier);
            return new ParameterSyntax(typeName, parameterName);
        }

        private EnumSyntax ParseEnum()
        {
            ParseExpected(TokenType.EnumKeyword);
            var enumName = ParseExpected(TokenType.Identifier);
            var member = ParseInnerCommmaSeperated(
                TokenType.OpenBrace,
                TokenType.ClosingBrace,
                ParseEnumMember);
            return new EnumSyntax(enumName, member);
        }

        private EnumMemberSyntax ParseEnumMember()
        {
            var memberName = ParseExpected(TokenType.Identifier);

            IExpressionSyntax initializer = null;
            if (ParseOptional(TokenType.Colon))
            {
                ParseExpected(TokenType.Colon);
                initializer = ParseExpression();
            }
            return new EnumMemberSyntax(memberName, initializer);
        }

        private List<TSyntax> ParseInner<TSyntax>(Func<TSyntax> parser) where TSyntax : ISyntaxBase
        {
            return ParseInner(TokenType.OpenBrace, TokenType.ClosingBrace, parser);
        }

        private ScopeStatementSyntax ParseScope(Func<IStatementSyntax> parser)
        {
            return new ScopeStatementSyntax(ParseInner(TokenType.OpenBrace, TokenType.ClosingBrace, parser).ToList());
        }

        private static List<Token> RemoveUnnecessaryToken(List<Token> token)
        {
            return token.Where(x => x.Type != TokenType.Whitespace)
                        .Where(x => x.Type != TokenType.Tab)
                        .Where(x => x.Type != TokenType.NewLine)
                        .Where(x => x.Type != TokenType.Comment)
                        .ToList();
        }

        private static bool IsAssignOperator(Token token)
        {
            return AssignOperators.Contains(token.Type);
        }

        private static bool IsPostfixOperator(Token token)
        {
            return PostfixOperators.Contains(token.Type);
        }

        private static bool IsPrefixOperator(Token token)
        {
            return PrefixOperators.Contains(token.Type);
        }

        private bool IsPossibleInfix()
        {
            return TokenStream.Current.Type == TokenType.Identifier;
        }

        private static void Ensure(Func<bool> func, string message)
        {
            if (!func())
            {
                throw new KiwiSyntaxException(message);
            }
        }

        private static bool IsOperatorFunctionName(string functionName)
        {
            return OperatorFunctionNames.Contains(functionName);
        }
    }
}