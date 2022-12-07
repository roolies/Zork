namespace Zork.Common
{
    public class TrophyCase
    {
        public string Name { get; }

        public TrophyCase(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
