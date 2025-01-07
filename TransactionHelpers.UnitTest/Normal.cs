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

        Assert.Equal(result1.IsSuccess, result2.IsSuccess);
        Assert.Equal(result1.IsError, result2.IsError);
        Assert.Equal(result1.Error?.Message, result2.Error?.Message);
        Assert.Equal(result1.Error?.Code, result2.Error?.Code);
        Assert.Equal(result1.Error?.Exception?.Message, result2.Error?.Exception?.Message);
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

        Assert.Equal(result1.HasValue, result2.HasValue);
        Assert.Equal(result1.HasNoValue, result2.HasNoValue);
        Assert.Equal(result1.IsSuccess, result2.IsSuccess);
        Assert.Equal(result1.IsError, result2.IsError);
        Assert.Equal(result1.Error?.Message, result2.Error?.Message);
        Assert.Equal(result1.Error?.Code, result2.Error?.Code);
        Assert.Equal(result1.Error?.Exception?.Message, result2.Error?.Exception?.Message);
        Assert.Equal(result1.Value, result2.Value);
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

        Assert.Equal(result1.HasValue, result2.HasValue);
        Assert.Equal(result1.HasNoValue, result2.HasNoValue);
        Assert.Equal(result1.IsSuccess, result2.IsSuccess);
        Assert.Equal(result1.IsError, result2.IsError);
        Assert.Equal(result1.Error?.Message, result2.Error?.Message);
        Assert.Equal(result1.Error?.Code, result2.Error?.Code);
        Assert.Equal(result1.Error?.Exception?.Message, result2.Error?.Exception?.Message);
        Assert.Equal(result1.Value?.Value?.Value, result2.Value);
    }

    [Fact]
    public void CloneableResultTest()
    {
        Result result1 = new();
        Result result2 = (result1.Clone() as Result)!;
        Result result3 = (result1.Clone() as Result)!;

        result1
            .WithError(new Exception());

        Result result4 = (result1.Clone() as Result)!;

        Assert.Equal(result2.IsSuccess, result3.IsSuccess);
        Assert.Equal(result2.IsError, result3.IsError);
        Assert.Equal(result2.Error?.Message, result3.Error?.Message);
        Assert.Equal(result2.Error?.Code, result3.Error?.Code);
        Assert.Equal(result2.Error?.Exception?.Message, result3.Error?.Exception?.Message);

        Assert.Equal(result1.IsSuccess, result4.IsSuccess);
        Assert.Equal(result1.IsError, result4.IsError);
        Assert.Equal(result1.Error?.Message, result4.Error?.Message);
        Assert.Equal(result1.Error?.Code, result4.Error?.Code);
        Assert.Equal(result1.Error?.Exception?.Message, result4.Error?.Exception?.Message);
    }

    [Fact]
    public void CloneableTypedResultTest()
    {
        Result<string> result1 = new();
        result1.WithValue("test");
        Result<string> result2 = (result1.Clone() as Result<string>)!;
        Result<string> result3 = (result1.Clone() as Result<string>)!;

        result1
            .WithError(new Exception());

        Result<string> result4 = (result1.Clone() as Result<string>)!;

        Assert.Equal(result2.IsSuccess, result3.IsSuccess);
        Assert.Equal(result2.IsError, result3.IsError);
        Assert.Equal(result2.Error?.Message, result3.Error?.Message);
        Assert.Equal(result2.Error?.Code, result3.Error?.Code);
        Assert.Equal(result2.Error?.Exception?.Message, result3.Error?.Exception?.Message);
        Assert.Equal(result2.HasValue, result3.HasValue);
        Assert.Equal(result2.HasNoValue, result3.HasNoValue);
        Assert.Equal(result2.Value, result3.Value);

        Assert.Equal(result1.IsSuccess, result4.IsSuccess);
        Assert.Equal(result1.IsError, result4.IsError);
        Assert.Equal(result1.Error?.Message, result4.Error?.Message);
        Assert.Equal(result1.Error?.Code, result4.Error?.Code);
        Assert.Equal(result1.Error?.Exception?.Message, result4.Error?.Exception?.Message);
        Assert.Equal(result1.HasValue, result4.HasValue);
        Assert.Equal(result1.HasNoValue, result4.HasNoValue);
        Assert.Equal(result1.Value, result4.Value);
    }
}
