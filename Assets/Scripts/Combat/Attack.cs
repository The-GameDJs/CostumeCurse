namespace Assets.Scripts.Combat
{
    public struct Attack
    {
        public int Damage;
        public ElementType Element;
        public AttackStyle Style;

        public Attack(int damage, ElementType element, AttackStyle style)
        {
            Damage = damage;
            Element = element;
            Style = style;
        }
    }
    
    public enum ElementType
    {
        Normal,
        Fire,
        Ice,
        None
    }

    public enum AttackStyle
    {
        Melee,
        Ranged,
        Magic
    }
}
