namespace Assets.Scripts.Combat
{
    public struct Attack
    {
        public int Damage;
        public ElementType Element;

        public Attack(int damage, ElementType element)
        {
            Damage = damage;
            Element = element;
        }
    }
    
    public enum ElementType
    {
        Normal,
        Fire,
        Ice,
        None
    }
}
