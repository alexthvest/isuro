using Isuro.Language.Common.Diagnostics;

namespace Isuro.Language.Common.Syntax;

public record SyntaxToken(SyntaxKind Kind, object? Value, Position Position)
{
    public override string ToString() => $"({Position.Line}:{Position.Column}) {Kind} = {Value}";
}
