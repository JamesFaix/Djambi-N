namespace Djambi.Model
{
    public class Faction
    {
        public int Id { get; }

        private Faction(int id)
        {
            Id = id;
        }

        public static Faction Create(int id) =>
            new Faction(id);
    }
}
