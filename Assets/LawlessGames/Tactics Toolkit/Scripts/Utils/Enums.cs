
namespace TacticsToolkit
{
    public enum Stats
    {
        Health,
        Mana,
        Strenght,
        Endurance,
        Speed,
        Intelligence,
        MoveRange,
        AttackRange,
        CurrentHealth,
        CurrentMana
    }

    public enum Operation
    {
        Add,
        Minus,
        Multiply,
        Divide,
        AddByPercentage,
        MinusByPercentage
    }

    public enum TileTypes
    {
        Traversable,
        NonTraversable,
        Effect
    }
}