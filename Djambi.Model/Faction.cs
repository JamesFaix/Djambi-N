namespace Djambi.Model
{
    public class Faction
    {
        public int Id { get; }

        private Faction(int id)
        {
            Id = id;
        }

        public override string ToString() =>
            $"Id: {Id}";

        public static Faction Create(int id) =>
            new Faction(id);
    }
}
