# TRM Application - Login & Dashboard Setup Guide

## Overview
Your Tool Request Management (TRM) application is now fully configured with:
- ✅ Professional login page with PRIMEPACK branding
- ✅ SQL Server database authentication
- ✅ Modern dashboard with sidebar navigation
- ✅ Session management and user tracking
- ✅ Responsive design for all devices

---

## Architecture

### Authentication Flow
```
User starts app
	↓
Program.cs checks "/" route
	↓
Is user authenticated?
	├─ NO → Redirect to /Auth/Login
	└─ YES → Redirect to /Index (Dashboard)

Login page submission
	↓
AuthenticationService queries TRMS_Users table
	↓
Credentials valid?
	├─ NO → Show error message
	└─ YES → Set session & redirect to Dashboard
```

### Database Structure
The application expects the following table in your SQL Server database:

**[PTI_ERP].[dbo].[TRMS_Users]**

Columns (required):
- `UserId` (INT, PK, Auto-increment)
- `Username` (NVARCHAR(100), Unique, Required)
- `Password` (NVARCHAR(255), Required) - Store as hashed password
- `EmployeeName` (NVARCHAR(150), Optional)
- `Department` (NVARCHAR(100), Optional)
- `Email` (NVARCHAR(100), Optional)
- `Role` (NVARCHAR(50), Optional)
- `IsActive` (BIT, Default: 1)
- `CreatedDate` (DATETIME, Optional)
- `LastLogin` (DATETIME, Optional)

---

## File Structure

### New Files Created

#### Authentication & Login
- `TRM/Pages/Auth/Login.cshtml` - Login page view
- `TRM/Pages/Auth/Login.cshtml.cs` - Login page handler
- `TRM/Pages/Auth/Logout.cshtml` - Logout page with redirect
- `TRM/Pages/Auth/Logout.cshtml.cs` - Logout handler

#### Database Layer
- `TRM/Models/User.cs` - User entity model
- `TRM/Data/ApplicationDbContext.cs` - Entity Framework Core context
- `TRM/Services/AuthenticationService.cs` - Authentication business logic

#### Dashboard
- `TRM/Pages/Index.cshtml` - Main dashboard (Tooling Requests list)
- `TRM/Pages/Index.cshtml.cs` - Dashboard page model
- `TRM/Pages/Shared/_LayoutDashboard.cshtml` - Dashboard layout template

#### Styling
- `TRM/wwwroot/css/features/auth/login.css` - Professional login page styles
- `TRM/wwwroot/css/dashboard.css` - Dashboard & sidebar styles

#### Configuration
- `TRM/appsettings.json` - Updated with database connection string
- `TRM/TRM.csproj` - Updated with Entity Framework Core packages
- `TRM/Program.cs` - Updated with DI, middleware, and routing

---

## Key Features

### 1. Login Page
- **Professional Design**: Two-column layout with PRIMEPACK branding
- **Security Features**:
  - Password visibility toggle (👁️ icon)
  - HttpOnly secure cookies
  - Session timeout (1 hour)
  - Last login tracking

- **User Experience**:
  - Smooth animations and transitions
  - Error messaging with icons
  - "Remember Me" functionality (30-day persistent cookie)
  - Responsive design (mobile, tablet, desktop)
  - Form focus indicators

### 2. Dashboard
- **Sidebar Navigation**:
  - App branding with logo
  - Menu items (Tooling Requests, Prerequisites)
  - User profile section with name and email
  - Sign Out button

- **Main Content**:
  - Header with title and "New Request" button
  - Data table with TRF numbers, customers, status, and dates
  - Status badges with color coding:
	- DRAFT: Yellow
	- SUBMITTED: Green
	- APPROVED: Blue
	- PENDING: Purple
  - Empty state with call-to-action

- **Responsive Design**:
  - Fixed sidebar on desktop
  - Collapsible sidebar on mobile
  - Touch-friendly buttons and links

### 3. Security
- **Authentication**:
  - Database-driven authentication
  - LastLogin timestamp updates
  - Logging of all login attempts (success & failure with IP address)
  - User active status checking

- **Session Management**:
  - Session timeout: 1 hour
  - HttpOnly cookies (prevents XSS attacks)
  - Secure flag for HTTPS
  - SameSite: Strict (prevents CSRF)

- **Password Security**:
  - Passwords stored securely (configure hashing)
  - Cleared from memory after failed login
  - Password visibility toggle

---

## Startup & Configuration

### 1. Update Database
Add test users to the TRMS_Users table:
```sql
INSERT INTO [PTI_ERP].[dbo].[TRMS_Users] (Username, Password, EmployeeName, Department, Email, Role, IsActive, CreatedDate)
VALUES 
('0271', '12345', 'Admin User', 'Management', 'admin@company.com', 'Admin', 1, GETDATE()),
('0272', 'password', 'John Smith', 'Operations', 'john.smith@company.com', 'User', 1, GETDATE());
```

### 2. Verify Connection String
In `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "SERVER=(local)\\SQLEXPRESS;DATABASE=PTI_ERP;USER=SA;PWD=12345;Encrypt=false;TrustServerCertificate=true;"
}
```

### 3. Run Application
```powershell
dotnet run
```

The app will automatically:
1. Start at the root URL (`/`)
2. Redirect to `/Auth/Login` if not authenticated
3. Show dashboard at `/Index` after successful login

---

## Testing

### Test Scenarios

#### 1. First Login
- URL: `http://localhost:5000/`
- Expected: Redirect to login page
- Action: Enter username (0271) and password (12345)
- Expected: Redirect to dashboard showing sample data

#### 2. Dashboard Access
- Session variables set:
  - UserId: "1"
  - Username: "0271"
  - EmployeeName: "Admin User"
  - Department: "Management"
  - Email: "admin@company.com"
  - Role: "Admin"

- User info displays in sidebar

#### 3. Remember Me
- Check "Keep me signed in" during login
- Close and reopen browser
- Should remain logged in (30-day cookie)

#### 4. Logout
- Click "Sign Out" button
- Should redirect to login page
- Session cleared
- Cookie removed

#### 5. Responsive Design
- Resize browser to test mobile/tablet views
- Sidebar becomes collapsible on small screens
- Table remains readable with proper scrolling

#### 6. Error Handling
- Invalid credentials → Error message displayed
- Empty fields → Validation message
- Failed authentication → Logged with IP address

---

## Next Steps

### 1. Implement Password Hashing
Update `AuthenticationService.cs` to use BCrypt:
```csharp
// Install: dotnet add package BCrypt.Net-Next
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(inputPassword);
if (!BCrypt.Net.BCrypt.Verify(inputPassword, user.Password))
{
	// Invalid password
}
```

### 2. Add Data Access Layer
Create repositories for TRMS_Users and implement actual CRUD operations for tooling requests.

### 3. Implement Tooling Request Pages
- Create: `/Pages/ToolingRequest/Create.cshtml`
- Edit: `/Pages/ToolingRequest/Edit.cshtml`
- Detail: `/Pages/ToolingRequest/Detail.cshtml`

### 4. Add Role-Based Authorization
Implement authorization attributes to restrict pages by user role:
```csharp
if (userRole != "Admin") 
{
	return Forbid();
}
```

### 5. Enable HTTPS
Update `appsettings.json` to enforce HTTPS in production.

### 6. Add Forgot Password Feature
Implement password reset flow with email verification.

### 7. Database Migrations
Consider adding Entity Framework Core migrations for production deployments:
```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Customization

### Colors & Branding
All colors are defined in CSS custom properties. Update in:
- `TRM/wwwroot/css/features/auth/login.css` (`:root` section)
- `TRM/wwwroot/css/dashboard.css` (`:root` section)

### Company Logo
Place your company logo at: `TRM/wwwroot/img/PTI_nb.png`
The app will automatically use it.

### company Name
Update in Login.cshtml:
```html
<h1 class="company-tagline"><strong>Your Company Name</strong></h1>
```

### Session Timeout
In `Program.cs`:
```csharp
options.IdleTimeout = TimeSpan.FromMinutes(30); // Change from 1 hour
```

---

## Troubleshooting

### "Connection to database failed"
- Verify SQL Server is running: `(local)\SQLEXPRESS`
- Check connection string in `appsettings.json`
- Verify SA user credentials and permissions
- Ensure PTI_ERP database exists

### "Login always fails"
- Check if TRMS_Users table exists and has data
- Verify column names match the model
- Check password storage format (plain text vs hashed)
- Review application logs for detailed error messages

### "Session not persisting"
- Ensure session middleware is added to Program.cs
- Check if cookies are enabled in browser
- Verify session.Cookie settings in Program.cs

### "Sidebar not showing on mobile"
- Clear browser cache
- Check media queries in `dashboard.css`
- Verify Bootstrap is loading correctly

---

## Security Checklist

- [ ] Change SA password to strong password
- [ ] Implement password hashing (BCrypt)
- [ ] Enable HTTPS in production
- [ ] Implement CORS if needed
- [ ] Add rate limiting to login attempts
- [ ] Configure SQL Server encryption
- [ ] Review and update connection string security settings
- [ ] Implement proper logging and monitoring
- [ ] Add two-factor authentication (optional)
- [ ] Regular security updates for dependencies

---

## Deployment Considerations

### Development
- Connection string with `Encrypt=false`
- Detailed logging enabled
- SQL Server running locally

### Production
- Secure connection string with encryption
- Minimal logging (only errors and security events)
- SQL Server on separate secure server
- HTTPS enforced
- Strong authentication credentials
- Database backups configured
- Monitoring and alerting enabled

---

## Support & Documentation

For more information:
- [ASP.NET Core Razor Pages](https://learn.microsoft.com/aspnet/core/razor-pages)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [SQL Server Documentation](https://learn.microsoft.com/sql/sql-server)
- [Bootstrap Documentation](https://getbootstrap.com/docs)

---

**Created:** January 2026  
**Application:** Tool Request Management System (TRM)  
**Version:** 1.0.0  
**Framework:** .NET 10 with ASP.NET Core Razor Pages
