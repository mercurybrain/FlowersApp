using CommunityToolkit.Mvvm.ComponentModel;
using Flowers.Services;

namespace Flowers.ViewModel
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        public ProfileViewModel(DatabaseService databaseService) { 
            _databaseService = databaseService;
        }
    }
}
