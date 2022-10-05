using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XRNeckSafer
{
    public class ActionPropertyProcessor : IDisposable
    {
        private static readonly ILogger _logger = LogManager.GetLogger(nameof(ActionPropertyProcessor));
        private JoystickKeyboardScanner _scanner;
        private readonly List<ActionProperty> _properties;

        public ActionPropertyProcessor(List<ActionProperty> properties)
        {
            _scanner = new JoystickKeyboardScanner();
            _scanner.OnCurrentlyPressedChanged += OnCurrentlyPressedChanged;
            _properties = properties ?? new List<ActionProperty>();
        }

        private void OnCurrentlyPressedChanged(JoystickKeyboardInput input, bool sameKeys)
        {
            _logger.Trace(input.ToString());
            foreach (ActionProperty prop in _properties)
            {
                foreach (ActionPropertyEvent propEvent in prop.Events)
                {
                    var matched = propEvent.InputCombinations.FirstOrDefault(i => i.Match(input));
                    if (matched != null)
                    {
                        prop.DispatchEvent(propEvent, matched, sameKeys, true);
                        continue;
                    }
                    var firstEvent = propEvent.InputCombinations.FirstOrDefault();
                    if (firstEvent != null)
                    {
                        prop.DispatchEvent(propEvent, firstEvent, sameKeys, false);
                    }
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
