using Microsoft.Data.Sqlite;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using static System.TimeZoneInfo;

namespace ImageQueryUI;

public partial class MainPage : ContentPage
{
    ImageDatabaseObject IDO_DataWrangler = new ImageDatabaseObject();

	public MainPage()
	{
		InitializeComponent();
    }

	private async void ConnectToOak(object sender, EventArgs e)
	{
        try
        {

            #region Say Hi!
            // Cast sender to Button to access properties and methods
            LoginButton.IsVisible = false;
            uint transitionTime = 1000;

            StatusText.Opacity = 0;
            StatusText.Text = "Welcome!";
            await StatusText.FadeTo(1, transitionTime);

            await UpdateStatusTextAsync("Let's get started organizing your images", transitionTime);
            #endregion

            #region Verify Operating System
            ApplicationProgressBar.IsVisible = true;

            await UpdateStatusTextAsync("Checking system requirements", transitionTime);
            ApplicationProgressBar.Progress = 0.15;
            
            var pathToOak = "";
            if (DeviceInfo.Platform.Equals(DevicePlatform.WinUI))
            {
                pathToOak = @"\\smb-ruthh.oak.stanford.edu\groups\ruthh";

                await UpdateStatusTextAsync("Windows OS Detected", transitionTime);
            }
            else if (DeviceInfo.Platform.Equals(DevicePlatform.macOS))
            {
                pathToOak = @"smb://smb-ruthh.oak.stanford.edu/groups/ruthh";

                await UpdateStatusTextAsync("MacOS Detected", transitionTime);
            }
            else
            {
                throw new IOException(string.Format("Apologies, this application does not currently support the platform \"{0}\".\n\nThis application will now terminate", DeviceInfo.Platform.ToString()));
            }

            ApplicationProgressBar.Progress = 0.45;
            #endregion

            #region Verify Oak Connection

            await UpdateStatusTextAsync("Validating Connection to Oak", transitionTime);
            ApplicationProgressBar.Progress = 0.625;

            if (Directory.Exists(pathToOak))
            {
                await UpdateStatusTextAsync("Oak Storage Found. Connecting to database", transitionTime);

                ApplicationProgressBar.Progress = 0.8;
            }
            else
            {
                throw new DirectoryNotFoundException("Connection to Oak Storage failed. Oak path expected to be:\n\n -  " + pathToOak + "\n\nMake sure that Oak Storage is networked and is accessible. If you are off-campus, make sure the VPN connection is active.");
            }
            #endregion

            #region Connect to SQLite Database
            var imageDatabasePath = Path.Combine(pathToOak, "Microscope Images", "Database", "CompiledImages.sqlite");

            if (!File.Exists(imageDatabasePath)) 
            {
                throw new IOException($"Could not find database file on Oak Storge at filepath `{imageDatabasePath}`. This database is required to use this application and should NOT be missing.");
            }

            var connectionBuilder = new SqliteConnectionStringBuilder();
            connectionBuilder.DataSource = imageDatabasePath;

            SqliteConnection connection = new SqliteConnection(connectionBuilder.ConnectionString);
            connection.Open();

            // save the open connection to the IDO_DataObject
            IDO_DataWrangler.DatabaseConnection = connection;

            await UpdateStatusTextAsync("Database connection successful", transitionTime);
            ApplicationProgressBar.Progress = 1;
            ApplicationProgressBar.ProgressColor = Colors.DarkOliveGreen;

            #endregion

            await UpdateStatusTextAsync("Retrieving existing image metadata", transitionTime);

            ApplicationProgressBar.IsVisible = false;
            ApplicationProgressBar.Progress = 0;
            ApplicationProgressBar.ProgressColor = Colors.CornflowerBlue;

            #region Read in image metadata
            // grab image channel
            // metadata enum
            var query = connection.CreateCommand();
            query.CommandText =
                @"
                    SELECT id, sensor_type, channel_type
                    FROM image_channels
                ";

            using (var reader = query.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var sensorType = reader.GetString(1);
                    var channelType = reader.GetString(2);

                    IDO_DataWrangler.imageChannels.Add(id, new ImageChannel(id, sensorType, channelType));
                }
            }

            query = connection.CreateCommand();
            query.CommandText =
                @"
                    SELECT id, name
                    FROM image_types
                ";

            using (var reader = query.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var imageType = reader.GetString(1);

                    IDO_DataWrangler.imageTypes.Add(id, new ImageType(id, imageType));
                }
            }

            #endregion

            await StatusText.FadeTo(0, transitionTime);
            StatusText.Text = "Loading user data.";
            await StatusText.FadeTo(1, transitionTime);

            #region Load user accounts
            query = connection.CreateCommand();
            query.CommandText =
                @"
                    SELECT id, name
                    FROM users
                ";

            using (var reader = query.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);

                    IDO_DataWrangler.users.Add(id, new User(id, name));
                }
            }
            #endregion

            await UpdateStatusTextAsync("Application Ready!", transitionTime);

            UserPicker.ItemsSource = IDO_DataWrangler.users.Values.ToList();
            UserPicker.IsVisible = true;
            SelectUserButton.IsVisible = true;
            CreateUserButton.IsVisible = true;
            await UpdateStatusTextAsync("Please select your user account", transitionTime);
        }
        catch (IOException exception)
        {
            // Handle the exception
            await DisplayAlert("Error", "An error occurred:\n\n" + exception.Message, "OK");
            Microsoft.Maui.Controls.Application.Current.Quit();
        }
    }

    private async void SelectUser(object sender, EventArgs e)
    { 
    
    }

    private async void CreateUser(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateUserPage(IDO_DataWrangler));
    }

    private async Task UpdateStatusTextAsync(string newText, uint transitionTime, int sleepTime = 0)
    {
        await StatusText.FadeTo(0, transitionTime);
        StatusText.Text = newText;
        await StatusText.FadeTo(1, transitionTime);
        Thread.Sleep(sleepTime);
    }
    
}

