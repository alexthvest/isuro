namespace Isuro.Language.Common.Diagnostics;

public readonly record struct Position(int Absolute, int Line, int Column)
{
    public static Position Zero { get; } = new(0, 1, 1);

    public Position Advance(char overChar)
    {
        if (overChar == '\n')
        {
            return new Position(Absolute + 1, Line + 1, 1);
        }

        return new Position(Absolute + 1, Line, Column + 1);
    }
}
