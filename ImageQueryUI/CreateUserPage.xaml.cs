using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Maui.Graphics.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ImageQueryUI;

public partial class CreateUserPage : ContentPage
{
	private ImageDatabaseObject IDO_DataWrangler;

	public CreateUserPage(ImageDatabaseObject imageDatabaseObject)
	{
		this.IDO_DataWrangler = imageDatabaseObject;
		InitializeComponent();
    }

    private async void ValidateUniqueUsername(object sender, TextChangedEventArgs e)
	{
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            StatusText.TextColor = Colors.Black;
            StatusText.Text = "Enter a username please.";
            CreateUserButton.IsEnabled = false;
        }

        if ("Development Account".Equals(e.NewTextValue))
        {
            var t = "";
        }

        var query = IDO_DataWrangler.DatabaseConnection.CreateCommand();
        query.CommandText =
            "SELECT count(*) FROM users WHERE name = $inputText";

        query.Parameters.AddWithValue("$inputText", e.NewTextValue);
        
        var reader = await query.ExecuteReaderAsync();

        while (reader.Read())
        {
            var numRecords = reader.GetInt32(0);

            if (numRecords == 0)
            {
                StatusText.TextColor = Colors.Black;
                StatusText.Text = "Username available!";
                CreateUserButton.IsEnabled = true;
                return;
            }
            else
            {
                
                StatusText.TextColor = Colors.Red;
                StatusText.Text = "Username already in use. Please select another option.";
                CreateUserButton.IsEnabled = false;
                return;
            }
        }
        
        throw new IOException("SQLite Error: Something broke about the connection.");
    }

    private async void AddNewUser(Object sender, EventArgs e) 
    {
        var query = IDO_DataWrangler.DatabaseConnection.CreateCommand();
        query.CommandText =
            @"
                    INSERT INTO users 
                        (name)
                    VALUES 
                        ($inputText)
            ";

        query.Parameters.AddWithValue("$inputText", NewUserName.Text);

        var result = await query.ExecuteNonQueryAsync();

        #region Load user accounts
        #endregion
    }
}