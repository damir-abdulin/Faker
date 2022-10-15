using System;
using NUnit.Framework;

using FakerCore;
using FakerCore.Generators;

namespace FakerTests;

public class Tests
{
    private readonly Faker _faker = new Faker();
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void Create_DateTimeGenerator_ReturnDateTime()
    {
        var generator = new DateTimeGenerator();
        var dateTime = _faker.Create<DateTime>();
        var statement = dateTime.CompareTo(generator.MinDate) >= 0 && dateTime.CompareTo(generator.MaxDate) <= 0;
        
        
        Assert.IsTrue(statement, $"Invalid time: {dateTime}");
    }
}