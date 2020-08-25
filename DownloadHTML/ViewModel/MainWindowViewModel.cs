using DownloadHTML.Command;
using DownloadHTML.DataValidations;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DownloadHTML.ViewModel
{
	class MainWindowViewModel : ViewModelBase, IDataErrorInfo
	{
		#region Fields
		readonly MainWindow mainView;
		string url;
		string filePath;
		static string folderName = "HTML";
		string zipPath;
		string folderPath;
		#endregion

		#region Constructor
		internal MainWindowViewModel(MainWindow view)
		{
			this.mainView = view;
			URL = string.Empty;
			CanSave = true;
			filePath = string.Empty;			
			folderPath = Path.Combine(@"..\", folderName);
			Directory.CreateDirectory(folderPath);
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

		//downloading HTML

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

		private async void DownloadHTMLExecute()
		{
			try
			{
				using (WebClient client = new WebClient())
				{
					filePath = GenerateFilePath(URL);
					string htmlCode = client.DownloadString(URL);
					await Task.Run(() => File.WriteAllText(filePath, htmlCode));
					IsFileCreated = true;
					filePath = string.Empty;
					URL = string.Empty;
					MessageBox.Show("You have successfully downloaded the HTML.");
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Something went wrong. HTML is not downloaded.");
			}
		}

		private string GenerateFilePath(string uRL)
		{
			var sb = new StringBuilder();
			sb.Append(folderPath);
			sb.Append(@"\");
			sb.Append(ExtractDomainNameFromURL(uRL));
			sb.Append(DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss-fff"));
			sb.Append(".html");
			return sb.ToString();
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

		//Zip files

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
				zipPath = GenerateZipPath();

				ZipFile.CreateFromDirectory(folderPath, zipPath, CompressionLevel.Fastest, false);
		
				var files = Directory.GetFiles(folderPath);

				MessageBox.Show("You have successfuly zipped the files.");
			}
			catch (Exception)
			{
				MessageBox.Show("Something went wrong. File is not zipped.");
			}
		}

		private string GenerateZipPath()
		{
			var sb = new StringBuilder();
			sb.Append(folderName);
			sb.Append("_");
			sb.Append(DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss-fff"));
			sb.Append(".zip");
			return sb.ToString();
		}

		private bool CanZipFiles()
		{
			return Directory.GetFiles(folderPath).Any();
		}
		#endregion
	}
}
