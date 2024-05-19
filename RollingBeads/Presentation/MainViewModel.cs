using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using RollingBeads.Models;
using Windows.Foundation;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace RollingBeads.Presentation;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel(IOptions<AppConfig> appInfo, INavigator navigator)
    {
    }
}
