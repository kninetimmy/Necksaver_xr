namespace XRNeckSafer
{
    public class JoystickButton
    {
        private string _id;
        private string _joystickGuid = "-1";
        private int _button = -1;
        private int _pov = -1;

        public string JoystickGuid
        {
            get => _joystickGuid;
            set
            {
                _joystickGuid = value;
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

        public JoystickButton()
        {
            _id = GenerateId();
        }

        private string GenerateId()
        {
            return $"{JoystickGuid}-{Button}-{POV}";
        }
    }
}
