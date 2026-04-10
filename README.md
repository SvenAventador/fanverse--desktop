# FanVerse--desktop
A native Windows desktop admin dashboard for FanVerse — manage users, stories, reports, and platform moderation. Built with WPF and .NET 10 (code-behind style).

## 🚀 Overview

**FanVerse Admin Desktop** is a dedicated moderation tool for platform administrators.  
This WPF application connects to the FanVerse backend API, providing a powerful interface for content moderation, user management, and platform analytics.

Built with classic WPF approach — XAML + code-behind. No MVVM complexity.

---

## ✨ Core Features

### 📊 Dashboard & Analytics
- Real-time platform statistics (users, stories, chapters, comments)
- Moderation queue counters with visual indicators
- Recent reports and flagged content feed

### 👥 User Management
- Browse, search, and filter all users
- View user profiles, stories, and activity history
- Suspend/ban/unban users with reason logging
- Promote/demote moderators
- Delete problematic accounts

### 📝 Content Moderation
- **Stories** — view all, filter by status/reports, edit, hide, or delete
- **Chapters** — preview flagged chapters, edit sensitive content, soft-delete
- **Comments** — review reported comments, approve, edit, or remove
- **Bulk actions** — select multiple items for moderation

### 🏷 Genre & Tag Management
- Create, edit, or disable genres
- Merge duplicate tags
- Review tag suggestions from users

### ⚠️ Reports System
- Queue of pending reports (stories, chapters, comments)
- Resolve reports with decision notes
- Ban users after repeated violations
- Report history per user/content item

### 🔧 System Configuration
- Platform settings (registration open/close, content filters)
- Rate limits adjustment
- Announcement broadcasting

### 📜 Audit Log
- Complete action history with admin username, timestamp, and details
- Filter by action type, target user, or content ID
- Export logs to CSV

---

## 🛠 Built With

- **Framework**: WPF (.NET 10)
- **Language**: C# 13
- **UI Library**: MaterialDesignInXamlToolkit
- **Architecture**: Classic WPF (XAML + code-behind)
- **API Client**: HttpClient + Newtonsoft.Json
- **Scaffolding**: EF Core DbScaffold (optional, for local models)

---

## 📥 Prerequisites

- Windows 10/11 (x64)
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with "Desktop development with .NET" workload
- Running [FanVerse Backend Server](https://github.com/SvenAventador/fanverse-server) with admin API enabled

---

## ⚙️ Setup & Configuration

### 1. Clone the repository

```bash
git clone https://github.com/SvenAventador/FanVerse--desktop-admin.git
cd FanVerse--desktop-admin
```

### 2. Restore NuGet packages
```bash
dotnet restore
```

### 3. Configure API endpoint
Edit ```App.config```:
```xml
<configuration>
  <appSettings>
    <add key="ApiBaseUrl" value="http://localhost:5000/api/admin"/>
    <add key="ApiTimeoutSeconds" value="30"/>
    <add key="AutoRefreshSeconds" value="15"/>
  </appSettings>
</configuration>
```

### 4. (Optional) Scaffold local database models
```bash
dotnet ef dbcontext scaffold "Host=localhost;Database=fanverse_admin;Username=postgres;Password=yourpassword" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Models --force
```
### 4.1 Needed dependencies
Install the following dependencies via NuGet:
``` bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### 5. Build and run
```bash
dotnet build
dotnet run
```
Or open ```FanVerseAdmin.sln``` in Visual Studio → ```F5```

### 🏗 Approximate Project Structure
```bash
FanVerse--desktop/
├── MainWindow.xaml               # Main dashboard
├── MainWindow.xaml.cs            # Code-behind logic
├── LoginWindow.xaml
├── LoginWindow.xaml.cs
├── Windows/                      # Additional windows
│   ├── UserManagementWindow.xaml
│   ├── UserManagementWindow.xaml.cs
│   ├── ModerationQueueWindow.xaml
│   ├── ModerationQueueWindow.xaml.cs
│   ├── ReportsWindow.xaml
│   ├── ReportsWindow.xaml.cs
│   ├── AuditLogWindow.xaml
│   ├── AuditLogWindow.xaml.cs
│   ├── SettingsWindow.xaml
│   └── SettingsWindow.xaml.cs
├── Controls/                     # Custom user controls
│   ├── UserCard.xaml
│   ├── UserCard.xaml.cs
│   ├── StoryPreview.xaml
│   └── StoryPreview.xaml.cs
├── Dialogs/                      # Modal dialogs
│   ├── BanUserDialog.xaml
│   ├── BanUserDialog.xaml.cs
│   ├── ResolveReportDialog.xaml
│   └── ResolveReportDialog.xaml.cs
├── Models/                       # Data models
│   ├── User.cs
│   ├── Story.cs
│   ├── Chapter.cs
│   ├── Report.cs
│   └── AuditEntry.cs
├── Services/                     # Helper classes
│   ├── ApiService.cs             # HTTP requests to backend
│   ├── AuthService.cs            # JWT token management
│   ├── DialogService.cs          # Show dialogs
│   ├── DataCache.cs              # Local SQLite caching
│   └── ExportService.cs          # CSV/Excel export
├── Utils/                        # Helpers
│   ├── StringExtensions.cs
│   ├── DateTimeHelper.cs
│   └── ImageLoader.cs
├── Assets/                       # Icons, fonts, styles
│   ├── Styles.xaml
│   ├── Icons/
│   └── Fonts/
├── App.xaml
├── App.xaml.cs
└── FanVerseAdmin.csproj
```

### 💻 Code-Behind Example
```cs
// MainWindow.xaml.cs
public partial class MainWindow : Window
{
    private ApiService _api;
    private Timer _refreshTimer;
    
    public MainWindow()
    {
        InitializeComponent();
        _api = new ApiService();
        LoadDashboard();
        StartAutoRefresh();
    }
    
    private async void LoadDashboard()
    {
        try
        {
            var stats = await _api.GetStatsAsync();
            TotalUsersText.Text = stats.TotalUsers.ToString();
            PendingReportsText.Text = stats.PendingReports.ToString();
            PendingStoriesText.Text = stats.PendingStories.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", 
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void UsersButton_Click(object sender, RoutedEventArgs e)
    {
        var userWindow = new UserManagementWindow();
        userWindow.Owner = this;
        userWindow.ShowDialog();
    }
}
```
