using GoMrDevice.MVVM.Core;
using GoMrDevice.Services;
using Microsoft.Azure.Devices.Shared;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GoMrDevice.MVVM.ViewModels;

public class SettingsViewModel : ObservableObject
{
	private readonly NavigationStore _navigationStore;
	private readonly DateTimeService _dateTimeService;
	public DeviceListViewModel DeviceListViewModel { get; }

	public SettingsViewModel(NavigationStore navigationStore, DateTimeService dateTimeService)
	{
		_navigationStore = navigationStore;
		_dateTimeService = dateTimeService;
		DeviceListViewModel = new DeviceListViewModel();
	}
	
	// Navigation
	public ICommand NavigateToHomeCommand =>
		new RelayCommand(() => _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore, _dateTimeService));

	public ICommand CloseApplicationCommand =>
		new RelayCommand(() => ApplicationService.CloseApplication());


	private string? _title = "Settings";
	public string? Title { get => _title; set => SetValue(ref _title, value); }




}