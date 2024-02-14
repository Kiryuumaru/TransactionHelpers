using TransactionHelpers.Exceptions;

namespace TransactionHelpers.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void ResponseTest()
        {
            Result response1 = new();

            Assert.True(response1.IsSuccess);
            Assert.False(response1.IsError);
            Assert.Null(response1.Error);

            response1 = new()
            {
                AppendResponse = response1,
                AppendException = new Exception()
            };

            Assert.False(response1.IsSuccess);
            Assert.True(response1.IsError);
            Assert.NotNull(response1.Error);
            Assert.Throws<Exception>(response1.ThrowIfError);

            Result response2 = new();

            response2 = new()
            {
                AppendResponse = response1
            };

            Assert.False(response2.IsSuccess);
            Assert.True(response2.IsError);
            Assert.NotNull(response2.Error);
            Assert.Throws<Exception>(response2.ThrowIfError);
        }

        [Fact]
        public void TypedResponseTest()
        {
            Result<string> response1 = new();

            Assert.True(response1.IsSuccess);
            Assert.False(response1.IsError);
            Assert.Null(response1.Error);
            Assert.Throws<EmptyResultException>(response1.ThrowIfErrorOrHasNoResult);

            response1 = new()
            {
                AppendResponse = response1,
                Value = "test"
            };

            Assert.True(response1.IsSuccess);
            Assert.False(response1.IsError);
            Assert.Null(response1.Error);

            Result<string> response2 = new();

            response2 = new()
            {
                AppendResponse = response1,
            };

            Assert.True(response2.IsSuccess);
            Assert.False(response2.IsError);
            Assert.Null(response2.Error);
        }

        [Fact]
        public void CascadeTypedResponseTest()
        {
            Result<Result<Result<string>>> response1 = new();

            Assert.True(response1.IsSuccess);
            Assert.False(response1.IsError);
            Assert.Null(response1.Error);
            Assert.Throws<EmptyResultException>(response1.ThrowIfErrorOrHasNoResult);

            response1 = new()
            {
                AppendResponse = response1,
                Value = new()
                {
                    Value = new()
                    {
                        Value = "test"
                    }
                }
            };

            Assert.True(response1.IsSuccess);
            Assert.False(response1.IsError);
            Assert.Null(response1.Error);

            Result<string> response2 = new();

            response2 = new()
            {
                AppendResponse = response1,
            };

            Assert.True(response2.IsSuccess);
            Assert.False(response2.IsError);
            Assert.Null(response2.Error);
            Assert.Equal(response1.Value.Value.Value, response2.Value);
        }
    }
}