using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomExceptons
{
    public class InvalidGameObjectNameException : System.Exception
    {
        public InvalidGameObjectNameException() { }
        public InvalidGameObjectNameException(string message) : base(message) { }
        public InvalidGameObjectNameException(string message, System.Exception inner) : base(message, inner) {}
    }
}
