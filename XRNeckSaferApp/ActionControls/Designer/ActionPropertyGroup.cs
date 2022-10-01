namespace XRNeckSafer
{
    public class ActionPropertyGroup
    {
        public string Name { get; set; }
        public int Order { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
