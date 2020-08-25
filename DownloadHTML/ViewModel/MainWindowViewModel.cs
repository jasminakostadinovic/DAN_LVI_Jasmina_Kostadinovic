using DownloadHTML.Command;
using DownloadHTML.DataValidations;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DownloadHTML.ViewModel
{
	class MainWindowViewModel : ViewModelBase, IDataErrorInfo
	{
		#region Fields
		readonly MainWindow mainView;
		string url;
		string path;
		StringBuilder stringBuilderPath;
		#endregion

		#region Constructor
		internal MainWindowViewModel(MainWindow view)
		{
			this.mainView = view;
			URL = string.Empty;
			CanSave = true;
			path = string.Empty;
			stringBuilderPath = new StringBuilder();
		}
		#endregion
		#region Properties
		public bool CanSave { get; set; }
		public bool IsFileCreated { get; set; }
		public string URL
		{
			get
			{
				return url;
			}
			set
			{
				if (url == value) return;
				url = value;
				OnPropertyChanged(nameof(URL));
			}
		}
		#endregion

		#region IDataErrorInfoImplementation
		//validations

		public string Error
		{
			get
			{
				return null;
			}
		}

		public string this[string name]
		{
			get
			{
				CanSave = true;
				string validationMessage = string.Empty;
				if (name == nameof(URL))
				{
					if(!DataValidation.CheckURLValid(URL))
					{
						validationMessage = "Invalid URL format!";
						CanSave = false;
					}
				}
				if (string.IsNullOrEmpty(validationMessage))
					CanSave = true;

				return validationMessage;
			}
		}
		#endregion

		#region Commands

		//submiting the order

		private ICommand downloadHTML;
		public ICommand DownloadHTML
		{
			get
			{
				if (downloadHTML == null)
				{
					downloadHTML = new RelayCommand(param => DownloadHTMLExecute(), param => CanDownloadHTML());
				}
				return downloadHTML;
			}
		}

		private void DownloadHTMLExecute()
		{
			try
			{
				using (WebClient client = new WebClient())
				{
					path = GeneratePath(URL);
					//client.DownloadFile(URL, path);
					string htmlCode = client.DownloadString(URL);
					File.WriteAllText(path, htmlCode);
					IsFileCreated = true;
					path = string.Empty;
					stringBuilderPath = new StringBuilder();
					MessageBox.Show("You have successfully downloaded the HTML.");
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Something went wrong. HTML is not downloaded.");
			}
		}

		private string GeneratePath(string uRL)
		{
			stringBuilderPath.Append(@"..\");
			stringBuilderPath.Append(ExtractDomainNameFromURL(uRL));
			stringBuilderPath.Append(DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss-fff"));
			stringBuilderPath.Append(".html");
			return stringBuilderPath.ToString();
		}

		public string ExtractDomainNameFromURL(string Url)
		{
			if (!Url.Contains("://"))
				Url = "http://" + Url;

			return new Uri(Url).Host;
		}
		private bool CanDownloadHTML()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(URL)
					|| CanSave == false)
					return false;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		//Send SMS

		private ICommand zipFiles;
		public ICommand ZipFiles
		{
			get
			{
				if (zipFiles == null)
				{
					zipFiles = new RelayCommand(param => ZipFilesExecute(), param => CanZipFiles());
				}
				return zipFiles;
			}
		}

		private void ZipFilesExecute()
		{
			try
			{
				
			}
			catch (Exception)
			{
				MessageBox.Show("Something went wrong. File is not zipped.");
			}
		}

		private bool CanZipFiles()
		{
			if (!IsFileCreated)
				return false;
			return true;
		}
		#endregion
	}
}
