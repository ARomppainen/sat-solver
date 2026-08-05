using SatSolver.Shared;

namespace SatSolver.Dimacs.Tests;

[Trait("Category", "Unit")]
public class DimacsParserTest
{
    [Fact]
    public void Parse_ShouldReturnFormula_WhenInputIsValid()
    {
        List<string> input = [
            "c comment",
            "",
            "p cnf 2 3",
            "",
            "1 0",
            "",
            "2 0",
            "-1 -2 0"
        ];

        Formula result = DimacsParser.Parse("test", input);

        Assert.Equal("test", result.Name);
        Assert.Equal(3, result.Clauses.Count);
        Assert.Equal(1, result.Clauses[0][0]);
        Assert.Equal(2, result.Clauses[1][0]);
        Assert.Equal(-1, result.Clauses[2][0]);
        Assert.Equal(-2, result.Clauses[2][1]);
    }

    [Fact]
    public void Parse_ShouldThrow_WhenInputIsEmpty()
    {
        Assert.Throws<DimacsParseException>(() => DimacsParser.Parse("test", []));
    }

    [Fact]
    public void Parse_ShouldThrow_WhenInputIsOnlyWhiteSpace()
    {
        Assert.Throws<DimacsParseException>(() => DimacsParser.Parse("test", ["", ""]));
    }

    [Fact]
    public void Parse_ShouldThrow_WhenProblemLineIsMissing()
    {
        List<string> input = [
            "1 0",
            "2 0",
            "-1 -2 0"
        ];

        Assert.Throws<DimacsParseException>(() => DimacsParser.Parse("test", input));
    }

    [Theory]
    [InlineData("cnf 2 3")]
    [InlineData("a cnf 2 3")]
    [InlineData("p dnf 2 3")]
    [InlineData("p cnf foo 3")]
    [InlineData("p cnf 2 bar")]
    [InlineData("p cnf -1 3")]
    [InlineData("p cnf 2 -1")]
    [InlineData("p cnf 2 3 4")]
    public void Parse_ShouldThrow_WhenProblemLineIsInvalid(string problemLine)
    {
        List<string> input = [
            problemLine,
            "1 0",
            "2 0",
            "-1 -2 0"
        ];

        Assert.Throws<DimacsParseException>(() => DimacsParser.Parse("test", input));
    }

    [Fact]
    public void Parse_ShouldThrow_WhenValueLineIsMissing()
    {
        List<string> input = [
            "p cnf 2 3",
            "1 0",
            "2 0",
        ];

        Assert.Throws<DimacsParseException>(() => DimacsParser.Parse("test", input));
    }

    [Theory]
    [InlineData("foo 0")]
    [InlineData("4 0")]
    [InlineData("-4 0")]
    public void Parse_ShouldThrow_WhenValueLineIsInvalid(string valueLine)
    {
        List<string> input = [
            "p cnf 3 1",
            valueLine
        ];

        Assert.Throws<DimacsParseException>(() => DimacsParser.Parse("test", input));
    }
}
