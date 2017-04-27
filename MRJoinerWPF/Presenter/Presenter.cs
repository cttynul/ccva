using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MRJoinerWPF.Presenter
{
    public abstract class Presenter
    {
        private Control _element;

        public Presenter(Control element)
        {
            Element = element;
        }

        protected Control Element
        {
            get
            {
                return _element;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Control");
                }
                _element = value;
            }
        }
    }
}
