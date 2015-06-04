using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Kelner.Algorithm
{
    public class TreeAttribute
    {
        protected PossibleValueCollection _possibleValues;
        protected string _name;
        protected object _label;

        public TreeAttribute(string name, PossibleValueCollection possibleValues)
        {
            _name = name;

            if (possibleValues == null)
                _possibleValues = null;
            else
            {
                _possibleValues = possibleValues;
                _possibleValues.Sort();
            }
        }

        public string AttributeName
        {
            get
            {
                return _name;
            }
        }

        public PossibleValueCollection PossibleValues
        {
            get
            {
                return _possibleValues;
            }
        }

        public bool isValidValue(string value)
        {
            return indexValue(value) >= 0;
        }

        // Wyciąganie wartości
        public int indexValue(string value)
        {
            if (_possibleValues != null)
                return _possibleValues.BinarySearch(value);
            else
                return -1;
        }

        public override string ToString()
        {
            if (_name != string.Empty)
            {
                return _name;
            }
            else
            {
                return _label.ToString();
            }
        }
    }
}
