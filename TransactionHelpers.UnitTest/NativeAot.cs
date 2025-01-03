using System.Text.Json;
using System.Text.Json.Serialization;
using TransactionHelpers.Exceptions;

namespace TransactionHelpers.UnitTest;

public class NativeAot
{
    [Fact]
    public void ResultTest()
    {
        JsonSerializerOptions camelCaseOption = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        var jsonContext = new MyJsonContext(new(camelCaseOption));
        var jsonContextResult = new RestfulHelpersJsonSerializerContext(new(camelCaseOption));
        var jsonContextResult1 = new RestfulHelpersJsonSerializerContext1(new(camelCaseOption));

        var dog = new Dog()
        {
            Name = "Skippy",
            Description = "Good boy",
            DogBreed = "Golden retriever"
        };

        var cat = new Cat()
        {
            Name = "Megatron",
            Description = "Spicy furbaby",
            CatBreed = "Decepticon"
        };

        var result = new Result();
        var errorResult = new Result();
        var dogResult = new Result<Dog>();

        dogResult.WithValue(dog);

        errorResult.WithError("THIS IS ERROR", "ERROR_CODE", "THIS IS ERROR DETAIL");

        var dogSerialized = JsonSerializer.Serialize(dog, jsonContext.Dog);
        var catSerialized = JsonSerializer.Serialize(cat, jsonContext.Cat);
        var resultSerialized = JsonSerializer.Serialize(result, jsonContextResult.Result);
        var errorResultSerialized = JsonSerializer.Serialize(errorResult, jsonContextResult.Result);
        var dogResultSerialized = JsonSerializer.Serialize(dogResult, jsonContextResult1.ResultDog);

        Console.WriteLine($"Serialzied Dog: {dogSerialized}");
        Console.WriteLine($"Serialzied Cat: {catSerialized}");
        Console.WriteLine($"Serialzied result: {resultSerialized}");
        Console.WriteLine($"Serialzied error result: {errorResultSerialized}");
        Console.WriteLine($"Serialzied result Dog: {dogResultSerialized}");

        var dogDeserialized = JsonSerializer.Deserialize(dogSerialized, jsonContext.Dog) ?? throw new Exception();
        var catDeserialized = JsonSerializer.Deserialize(catSerialized, jsonContext.Cat) ?? throw new Exception();
        var resultDeserialized = JsonSerializer.Deserialize(resultSerialized, jsonContextResult.Result) ?? throw new Exception();
        var errorResultDeserialized = JsonSerializer.Deserialize(errorResultSerialized, jsonContextResult.Result) ?? throw new Exception();
        var dogResultDeserialized = JsonSerializer.Deserialize(dogResultSerialized, jsonContextResult1.ResultDog) ?? throw new Exception();

        dogResultDeserialized.ThrowIfErrorOrHasNoValue();

        Console.WriteLine($"Deserialzied Dog: {dogDeserialized}");
        Console.WriteLine($"Deserialzied Cat: {catDeserialized}");
        Console.WriteLine($"Deserialzied result: {resultDeserialized}");
        Console.WriteLine($"Deserialzied error result: {errorResultDeserialized}");
        Console.WriteLine($"Deserialzied result Dog: {dogResultDeserialized}");

        Assert.Equal(dog.Name, dogDeserialized.Name);
        Assert.Equal(dog.Description, dogDeserialized.Description);
        Assert.Equal(dog.DogBreed, dogDeserialized.DogBreed);

        Assert.Equal(cat.Name, catDeserialized.Name);
        Assert.Equal(cat.Description, catDeserialized.Description);
        Assert.Equal(cat.CatBreed, catDeserialized.CatBreed);

        Assert.Equal(result.IsSuccess, resultDeserialized.IsSuccess);
        Assert.Equal(result.IsError, resultDeserialized.IsError);
        Assert.Equal(result.Error?.Message, resultDeserialized.Error?.Message);
        Assert.Equal(result.Error?.Code, resultDeserialized.Error?.Code);
        Assert.Equal(result.Error?.Detail?.ToString(), resultDeserialized.Error?.Detail?.ToString());

        Assert.Equal(errorResult.IsSuccess, errorResultDeserialized.IsSuccess);
        Assert.Equal(errorResult.IsError, errorResultDeserialized.IsError);
        Assert.Equal(errorResult.Error?.Message, errorResultDeserialized.Error?.Message);
        Assert.Equal(errorResult.Error?.Code, errorResultDeserialized.Error?.Code);
        Assert.Equal(errorResult.Error?.Detail?.ToString(), errorResultDeserialized.Error?.Detail?.ToString());

        Assert.Equal(dogResult.IsSuccess, dogResultDeserialized.IsSuccess);
        Assert.Equal(dogResult.IsError, dogResultDeserialized.IsError);
        Assert.Equal(dogResult.Error?.Message, dogResultDeserialized.Error?.Message);
        Assert.Equal(dogResult.Error?.Code, dogResultDeserialized.Error?.Code);
        Assert.Equal(dogResult.Error?.Detail?.ToString(), dogResultDeserialized.Error?.Detail?.ToString());
        Assert.Equal(dogResult.HasValue, dogResultDeserialized.HasValue);
        Assert.Equal(dogResult.HasNoValue, dogResultDeserialized.HasNoValue);
        Assert.Equal(dogResult.Value?.Name, dogResultDeserialized.Value?.Name);
        Assert.Equal(dogResult.Value?.Description, dogResultDeserialized.Value?.Description);
        Assert.Equal(dogResult.Value?.DogBreed, dogResultDeserialized.Value?.DogBreed);
    }
}

public abstract class Animal
{
    public required string Name { get; set; }

    public required string Description { get; set; }

    public override string ToString()
    {
        return $"Name: {Name}, Description: {Description}";
    }
}

public class Dog : Animal
{
    public required string DogBreed { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, DogBreed: {DogBreed}";
    }
}

public class Cat : Animal
{
    public required string CatBreed { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, CatBreed: {CatBreed}";
    }
}

[JsonSerializable(typeof(Dog))]
[JsonSerializable(typeof(Cat))]
internal partial class MyJsonContext : JsonSerializerContext
{
}


[JsonSerializable(typeof(Result))]
[JsonSerializable(typeof(Error))]
internal partial class RestfulHelpersJsonSerializerContext : JsonSerializerContext
{
}


[JsonSerializable(typeof(Result<Dog>))]
internal partial class RestfulHelpersJsonSerializerContext1 : JsonSerializerContext
{
}
