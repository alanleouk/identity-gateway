namespace Identity.Models.FindModels
{
    public class ById
    {
        public Guid Id { get; init; }

        public ById()
        {

        }

        public ById(Guid id)
        {
            Id = id;
        }
    }
}
