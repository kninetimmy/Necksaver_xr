namespace XRNeckSafer
{
    public class ActionPropertyGroupItem
    {
        public string Name { get; set; }
        public ActionPropertyGroup Tag { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
