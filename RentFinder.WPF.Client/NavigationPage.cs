using System.Windows.Controls;

namespace RentFinder.WPF.Client
{
    public abstract class NavigationPage:Page
    {
        private readonly Frame _parentFrame;
        protected NavigationPage(Frame parentFrame)
        {
            _parentFrame = parentFrame;
        }

        public Frame ParentFrame
        {
            get { return _parentFrame; }
        }
    }
}
