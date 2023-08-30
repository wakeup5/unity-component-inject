using System;

namespace Waker.Injection
{
    public enum InjectFrom : byte
    {
        Self,
        Children,
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAttribute : System.Attribute
    {
        private InjectFrom _injectFrom;
        private string _selector;

        public InjectAttribute(string selector)
        {
            _injectFrom = InjectFrom.Children;
            _selector = selector;
        }

        public InjectAttribute(InjectFrom injectFrom)
        {
            _injectFrom = injectFrom;
            _selector = null;
        }

        [Obsolete("Use Inject(InjectFrom injectFrom) or Inject(string selector) instead")]
        public InjectAttribute(string selector = null, InjectFrom injectFrom = InjectFrom.Children)
        {
            this._injectFrom = !string.IsNullOrEmpty(selector) ? InjectFrom.Children : injectFrom;
            this._selector = selector;
        }

        public InjectFrom InjectFrom => _injectFrom;

        public string Selector => _selector;
    }
}