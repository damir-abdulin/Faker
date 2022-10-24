using System.Linq.Expressions;
using Examples;
using FakerCore;

var faker = new Faker();

var a = faker.Create<A>();

Console.WriteLine($".ctor {a.Ctor};\nIntField: {a.IntField};\nFieldWithoutSet: {a.FieldWithoutSet};\n" +
                  $"FieldWithPrivateSet: {a.FieldWithPrivateSet};\nIntValue: {a.IntValue};\n" +
                  $"DecimalValue: {a.DecimalValue};\n ShortValue: {a.ShortValue};\n StringValue: {a.StringValue};");

