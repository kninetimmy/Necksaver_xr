namespace XRNeckSafer
{
    public class JoyBut
    {
        private string _id;
        private int _joyIndex;
        private int _button;
        private int _pov;

        public int JoyIndex
        {
            get => _joyIndex;
            set
            {
                _joyIndex = value;
                _id = GenerateId();
            }
        }

        public int Button
        {
            get => _button;
            set
            {
                _button = value;
                _id = GenerateId();
            }
        }
        public int POV
        {
            get => _pov;
            set
            {
                _pov = value;
                _id = GenerateId();
            }
        }

        public string GetId()
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = GenerateId();
            }
            return _id;
        }

        private JoyBut()
        {

        }

        public static JoyBut CreateEmpty()
        {
            return new JoyBut { JoyIndex = -1, Button = -1, POV = -1 };
        }

        private string GenerateId()
        {
            return $"{JoyIndex}-{Button}-{POV}";
        }


    }
}
