namespace LegacySql.Domain.ValueObjects
{
    public class EntityId
    {
        public int Value { get; }

        public EntityId(int value)
        {
            Value = value;
        }
    }
}
