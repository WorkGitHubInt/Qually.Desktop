using System.ComponentModel;

namespace QuallyFlash
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
