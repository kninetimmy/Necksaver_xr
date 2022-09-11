namespace XRNeckSafer
{
    public class JoyBut
    {
        private string _id;
        private int _joyIndex = -1;
        private int _button = -1;
        private int _pov = -1;

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

        public JoyBut()
        {
            _id = GenerateId();
        }

        private string GenerateId()
        {
            return $"{JoyIndex}-{Button}-{POV}";
        }
    }
}
