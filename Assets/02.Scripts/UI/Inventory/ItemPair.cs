/// <summary>
/// Item data handling structure
/// </summary>
public struct ItemPair
{
    public static ItemPair Empty = new ItemPair(-1, -1);
    public static ItemPair Error = new ItemPair(-2, -2);
    public int Code;
    public int Num;

    public ItemPair(int itemCode, int itemNum)
    {
        Code = itemCode;
        Num = itemNum;
    }   

    public static bool operator ==(ItemPair op1, ItemPair op2)
        => op1.Code == op2.Code && op1.Num == op2.Num;
    public static bool operator !=(ItemPair op1, ItemPair op2)
        => !(op1 == op2);

    public static ItemPair operator +(ItemPair op1, ItemPair op2)
    {
        if (op1.Code == op2.Code)
            return new ItemPair(op1.Code, op1.Num + op2.Num);
        return Error;
    }

    public static ItemPair operator -(ItemPair op1, ItemPair op2)
    {
        if (op1.Code == op2.Code)
            return new ItemPair(op1.Code, op1.Num - op2.Num);
        return Error;
    }
}