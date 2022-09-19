using System;
using System.Collections.Generic;

namespace XRNeckSafer
{
    public class ActionPropertyProcessor : IDisposable
    {
        private JoystickKeyboardScanner _scanner;
        private readonly List<ActionProperty> _properties;

        public ActionPropertyProcessor(List<ActionProperty> properties)
        {
            _scanner = new JoystickKeyboardScanner(2);
            _scanner.OnCurrentlyPressedChanged += OnCurrentlyPressedChanged;
            _properties = properties ?? new List<ActionProperty>();
        }

        private void OnCurrentlyPressedChanged(JoystickKeyboardInput input, bool sameKeys)
        {
            // Console.WriteLine($"Accessor: same - {sameKeys} {input}");
            foreach (ActionProperty prop in _properties)
            {
                foreach (var propEvent in prop.Events)
                {
                    prop.DispatchEvent(propEvent, sameKeys, propEvent.InputCombination.Match(input));
                }
            }
        }

        public void Dispose()
        {
            if (_scanner != null)
            {
                _scanner.Dispose();
                _scanner = null;
            }
        }
    }
}
