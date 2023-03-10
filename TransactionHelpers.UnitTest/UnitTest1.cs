using TransactionHelpers.Exceptions;

namespace TransactionHelpers.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void ResponseTest()
        {
            Response response1 = new();

            Assert.True(response1.IsSuccess);
            Assert.False(response1.IsError);
            Assert.Null(response1.Error);

            response1.Append(new Exception());

            Assert.False(response1.IsSuccess);
            Assert.True(response1.IsError);
            Assert.NotNull(response1.Error);
            Assert.Throws<Exception>(response1.ThrowIfError);

            Response response2 = new();

            response2.Append(response1);

            Assert.False(response2.IsSuccess);
            Assert.True(response2.IsError);
            Assert.NotNull(response2.Error);
            Assert.Throws<Exception>(response2.ThrowIfError);
        }
        [Fact]
        public void TypedResponseTest()
        {
            Response<string> response1 = new();

            Assert.False(response1.IsSuccess);
            Assert.True(response1.IsError);
            Assert.NotNull(response1.Error);
            Assert.Throws<EmptyResultException>(response1.ThrowIfError);

            response1.Append("test");

            Assert.True(response1.IsSuccess);
            Assert.False(response1.IsError);
            Assert.Null(response1.Error);

            Response<string> response2 = new();

            response2.Append(response1);

            Assert.True(response2.IsSuccess);
            Assert.False(response2.IsError);
            Assert.Null(response2.Error);
        }
    }
}