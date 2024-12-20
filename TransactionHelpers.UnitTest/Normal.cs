using TransactionHelpers.Exceptions;

namespace TransactionHelpers.UnitTest;

public class Normal
{
    [Fact]
    public void ResultTest()
    {
        Result result1 = new();

        Assert.True(result1.IsSuccess);
        Assert.False(result1.IsError);
        Assert.Null(result1.Error);

        result1
            .WithResult(result1)
            .WithError(new Exception());

        Assert.False(result1.IsSuccess);
        Assert.True(result1.IsError);
        Assert.NotNull(result1.Error);
        Assert.Throws<Exception>(result1.ThrowIfError);

        Result result2 = new();

        result2
            .WithResult(result1);

        Assert.False(result2.IsSuccess);
        Assert.True(result2.IsError);
        Assert.NotNull(result2.Error);
        Assert.Throws<Exception>(result2.ThrowIfError);
    }

    [Fact]
    public void TypedResultTest()
    {
        Result<string> result1 = new();

        Assert.True(result1.IsSuccess);
        Assert.False(result1.IsError);
        Assert.Null(result1.Error);
        Assert.Throws<EmptyResultException>(result1.ThrowIfErrorOrHasNoValue);

        result1
            .WithResult(result1)
            .WithValue("test");

        Assert.True(result1.IsSuccess);
        Assert.False(result1.IsError);
        Assert.Null(result1.Error);

        Result<string> result2 = new();

        result2
            .WithResult(result1);

        Assert.True(result2.IsSuccess);
        Assert.False(result2.IsError);
        Assert.Null(result2.Error);
    }

    [Fact]
    public void CascadeTypedResultTest()
    {
        Result<Result<Result<string>>> result1 = new();

        Assert.True(result1.IsSuccess);
        Assert.False(result1.IsError);
        Assert.Null(result1.Error);
        Assert.Throws<EmptyResultException>(result1.ThrowIfErrorOrHasNoValue);

        result1
            .WithResult(result1)
            .WithValue(new Result<Result<string>>()
                .WithValue(new Result<string>()
                    .WithValue("test")));

        Assert.True(result1.IsSuccess);
        Assert.False(result1.IsError);
        Assert.Null(result1.Error);

        Result<string> result2 = new();

        result2
            .WithResult(result1);

        Assert.True(result2.IsSuccess);
        Assert.False(result2.IsError);
        Assert.Null(result2.Error);
        Assert.Equal(result1.Value?.Value?.Value, result2.Value);
        Assert.Equal("test", result1.Value?.Value?.Value);
        Assert.Equal("test", result2.Value);
    }
}

