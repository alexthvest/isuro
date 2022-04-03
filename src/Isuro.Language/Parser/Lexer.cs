using System.Collections.Immutable;
using Isuro.Language.Common.Diagnostics;
using Isuro.Language.Common.Syntax;

namespace Isuro.Language.Parser;

public sealed class Lexer
{
    private readonly string _source;
    private Position _position = Position.Zero;

    public Lexer(string source)
    {
        _source = source;
    }
    
    public ImmutableArray<SyntaxToken> Tokenize()
    {
        var syntaxTokens = ImmutableArray.CreateBuilder<SyntaxToken>();
        
        while (_position.Absolute < _source.Length)
        {
            var syntaxToken = TokenizeNext();
            syntaxTokens.Add(syntaxToken);
        }

        var eosToken = new SyntaxToken(SyntaxKind.EndOfStreamToken, '\0', _position);
        syntaxTokens.Add(eosToken);

        return syntaxTokens.ToImmutable();
    }

    private SyntaxToken TokenizeNext()
    {
        var @char = Peek();

        if (@char == '\n')
        {
            return TokenizeNewLine();
        }

        if (char.IsWhiteSpace(@char))
        {
            return TokenizeWhitespace();
        }

        if (char.IsLetter(@char))
        {
            return TokenizeIdentifier();
        }

        if (char.IsDigit(@char))
        {
            return TokenizeNumber();
        }

        if (@char == '-' && char.IsDigit(Peek(1)))
        {
            return TokenizeNumber();
        }

        return TokenizeCharacter();
    }

    private SyntaxToken TokenizeNewLine()
    {
        var position = _position;
        var @char = Consume();

        return new SyntaxToken(SyntaxKind.NewLineToken, @char, position);
    }

    private SyntaxToken TokenizeWhitespace()
    {
        var position = _position;
        var value = Consume(char.IsWhiteSpace);

        return new SyntaxToken(SyntaxKind.WhitespaceToken, value, position);
    }

    private SyntaxToken TokenizeIdentifier()
    {
        var position = _position;
        var value = Consume(c => char.IsLetterOrDigit(c) || "_-".Contains(c));

        return new SyntaxToken(SyntaxKind.IdentifierToken, value, position);
    }

    private SyntaxToken TokenizeNumber()
    {
        var position = _position;
        var hasPoint = false;

        var value = Consume(c =>
        {
            if (c == '.')
            {
                if (hasPoint) return false;
                return hasPoint = true;
            }

            return char.IsDigit(c) || c == '-';
        });

        return new SyntaxToken(SyntaxKind.NumberToken, value, position);
    }

    private SyntaxToken TokenizeCharacter()
    {
        var position = _position;
        var @char = Consume();

        return new SyntaxToken(SyntaxKind.CharacterToken, @char, position);
    }
    
    private char Consume()
    {
        var @char = Peek();
        _position = _position.Advance(@char);
        return @char;
    }
    
    private string Consume(Func<char, bool> predicate)
    {
        var startPosition = _position;
        var @char = Peek();

        while (@char != '\0' && predicate(@char))
        {
            Consume();
            @char = Peek();
        }

        return _source[startPosition.Absolute.._position.Absolute];
    }

    private char Peek(int offset = 0)
    {
        var index = _position.Absolute + offset;
        return index < _source.Length ? _source[index] : '\0';
    }
}
