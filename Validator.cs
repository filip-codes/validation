using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Midonis.Validation
{
    public class Validator
    {
        protected bool _isFailed = false;
        protected char _divider = '|';
        protected List<KeyValuePair<string, string>> _messages = new List<KeyValuePair<string, string>>();
        protected string[] _splittedRules = new string[] {};

        /// <summary>
        /// Create a new Validator instance.
        /// </summary>
        /// 
        /// <param name="value"></param>
        /// <param name="rules"></param>
        /// <exception cref="Exception"></exception>
        public static Validator make(object value, List<KeyValuePair<string, string>> rules)
        {
            Validator instance = new Validator();

            foreach (PropertyInfo key in value.GetType().GetProperties())
            {
                foreach (KeyValuePair<string, string> rule in rules)
                {
                    if (rule.Key != key.Name) continue;

                    instance._splittedRules = rule.Value.Split(instance._divider);

                    foreach(string ruleName in instance._splittedRules)
                    {
                        switch(ruleName)
                        {
                            case ValidationRule.required:
                                if (string.IsNullOrEmpty(key.GetValue(value).ToString()))
                                {
                                    instance._isFailed = true;
                                    instance._messages.Add(new KeyValuePair<string, string>(key.Name, key.Name + " is required"));
                                }
                                break;
                            case ValidationRule.email:
                                if (! new Regex("^\\S+@\\S+\\.\\S+$").IsMatch(key.GetValue(value).ToString()))
                                {
                                    instance._isFailed = true;
                                    instance._messages.Add(new KeyValuePair<string, string>(key.Name, key.Name + " is not a valid email address"));
                                }
                                break;
                            default:
                                throw new Exception("Validation rule for '" + ruleName + "' does not exist");
                        }
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// Determine if the data fails the validation rules.
        /// </summary>
        public bool fails()
        {
            return this._isFailed;
        }

        /// <summary>
        /// An alternative more semantic shortcut to the message container.
        /// </summary>
        public List<KeyValuePair<string, string>> errors()
        {
            return this._messages;
        }
    }
}
