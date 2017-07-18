namespace Core.Models
{
    public class Property
    {
        public string Name { get; set; }
        public string Statement { get; set; }

        public static implicit operator string(Property property)
            => property.Statement;
    }
}