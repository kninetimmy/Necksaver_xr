using System;
using System.Linq;

namespace XRNeckSafer
{
    public class ActionPropertyProcessor : IDisposable
    {
        private JoystickKeyboardScanner _scanner;
        private readonly ActionProperty[] _properties;

        public ActionPropertyProcessor(ActionProperty[] properties)
        {
            _scanner = new JoystickKeyboardScanner(2);
            _scanner.OnCurrentlyPressedChanged += OnCurrentlyPressedChanged;
            _scanner.OnScanningComplete += OnScanningComplete;
            _properties = properties;
        }

        private void OnScanningComplete(JoystickKeyboardInput input)
        {
            foreach (var prop in _properties)
            {
                var actionEvent = prop.Events.FirstOrDefault(p => p.InputCombination.IsEqual(input));
                if (actionEvent != null && !actionEvent.Toggle)
                {
                    // Console.WriteLine("Process trigger - OnScanningComplete!");
                    prop.DispatchEvent(actionEvent, false, true);
                }
            }
        }

        private void OnCurrentlyPressedChanged(JoystickKeyboardInput input, bool sameKeys)
        {
            foreach (var prop in _properties)
            {
                var actionEvent = prop.Events.FirstOrDefault(p => p.InputCombination.IsEqual(input));
                if (actionEvent != null)
                {
                    // Console.WriteLine("Process trigger - OnCurrentlyPressedChanged!");
                    prop.DispatchEvent(actionEvent, sameKeys, false);
                }
            }
        }

        public void Dispose()
        {
            if (_scanner != null)
            {
                _scanner.Dispose();
                foreach(var prop in _properties)
                {
                    prop.UnsubscribeTriggerHandlers();
                }
                _scanner = null;
            }
        }
    }
}
