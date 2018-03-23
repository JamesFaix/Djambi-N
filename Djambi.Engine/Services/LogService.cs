using System;
using System.Collections.Generic;

namespace Djambi.Engine.Services
{
    class LogService
    {
        private readonly List<string> _messages = new List<string>();

        public IEnumerable<string> GetMessages() => _messages;

        public void Write(string message)
        {
            _messages.Add(message);
            OnWrite?.Invoke(this, message);
        }

        public event EventHandler<string> OnWrite;
    }
}
