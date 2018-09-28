namespace AppName.Web.Models
{
    public class Airport
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public Airport()
        {

        }

        public Airport(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
