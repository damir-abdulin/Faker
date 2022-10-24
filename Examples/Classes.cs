namespace Examples;

public class A
{
    public string Ctor { get; }
    public B B { get; set; }

    public int IntField { get; set; }
    public int FieldWithoutSet { get; }
    public int FieldWithPrivateSet { get; private set; }
    
    public int IntValue;
    public decimal DecimalValue; 
    public short ShortValue;
    public string StringValue;

    private A()
    {
        Ctor = "A()";
    }
}

public class B
{
    public C C { get; set; }
}

public class C
{
    public A A { get; set; }
}