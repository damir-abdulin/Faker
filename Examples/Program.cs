using System.Linq.Expressions;
using Examples;
using FakerCore;
using FakerCore.UserGenerators;

var fakerConfig = new FakerConfig();
fakerConfig.Add<A, string, CityGenerator>(a => a.City);
fakerConfig.Add<A, int, AgeGenerator>(a => a.Age);

var faker = new Faker(fakerConfig);


var a = faker.Create<A>();

Console.WriteLine($".ctor {a.Ctor};\nIntField: {a.IntField};\nFieldWithoutSet: {a.FieldWithoutSet};\n" +
                  $"FieldWithPrivateSet: {a.FieldWithPrivateSet};\nIntValue: {a.IntValue};\n" +
                  $"DecimalValue: {a.DecimalValue};\nShortValue: {a.ShortValue};\nStringValue: {a.StringValue};\n" +
                  $"City: {a.City};\nAge: {a.Age};");

