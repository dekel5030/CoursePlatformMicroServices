# CoursePlatform Keycloak Custom Theme

This directory contains the custom Keycloak login theme for CoursePlatform, designed to provide a seamless brand experience that matches the React frontend.

## ?? Directory Structure

```
infrastructure/keycloak/themes/courseplatform/
??? login/
    ??? theme.properties              # Theme configuration
    ??? login.ftl                     # Login page template
    ??? register.ftl                  # Registration page template
    ??? login-reset-password.ftl      # Password reset page template
    ??? error.ftl                     # Error page template
    ??? messages/
    ?   ??? messages_en.properties    # Custom text labels (English)
    ??? resources/
        ??? css/
        ?   ??? login.css             # Custom styles matching React frontend
        ??? js/                        # JavaScript files (if needed)
        ??? img/                       # Images and logos (if needed)
```

## ?? Theme Features

### Design System Alignment
- **Color Palette**: Matches the React frontend Tailwind CSS v4 theme with oklch colors
- **Typography**: Uses system fonts consistent with the main application
- **Layout**: Modern card-based design with gradient background
- **Responsive**: Mobile-first design that works on all screen sizes
- **Accessibility**: WCAG compliant with proper ARIA labels and keyboard navigation

### Supported Pages
- ? Login page (`login.ftl`)
- ? Registration page (`register.ftl`)
- ? Password reset page (`login-reset-password.ftl`)
- ? Error page (`error.ftl`)

### Features
- Password visibility toggle
- Remember me checkbox
- Social provider login support
- Form validation with error messages
- Loading states
- Custom branding (CoursePlatform title)

## ?? Activation

### 1. Enable the Theme in Keycloak Admin Console

After starting the application with .NET Aspire:

1. Navigate to Keycloak Admin Console: `http://localhost:8080`
2. Login with credentials:
   - Username: `admin`
   - Password: `admin`
3. Select your realm (e.g., `course-platform`)
4. Go to **Realm Settings** ? **Themes** tab
5. Under **Login Theme**, select `courseplatform` from the dropdown
6. Click **Save**

### 2. Verify Theme is Loaded

To verify the theme is properly mounted:

```bash
# Check if theme files are accessible in the container
docker ps | grep keycloak
docker exec -it <keycloak-container-id> ls -la /opt/keycloak/themes/courseplatform/login
```

Expected output should show:
- `theme.properties`
- `login.ftl`
- `register.ftl`
- `login-reset-password.ftl`
- `error.ftl`
- `resources/` directory

### 3. Test the Theme

1. Navigate to the login URL: `http://localhost:8080/realms/course-platform/protocol/openid-connect/auth?client_id=<your-client>&redirect_uri=<redirect>&response_type=code`
2. You should see the custom CoursePlatform branded login page
3. Test registration by clicking "Create Account"
4. Test password reset by clicking "Forgot password?"

## ?? Customization Guide

### Changing Colors

Edit `resources/css/login.css` and modify the CSS variables:

```css
:root {
    --cp-primary: oklch(48% 0.12 265);        /* Primary brand color */
    --cp-primary-foreground: oklch(98% 0 0);  /* Text on primary */
    --cp-background: oklch(100% 0 0);         /* Page background */
    --cp-foreground: oklch(20% 0.02 265);     /* Main text color */
    /* ... other variables ... */
}
```

### Adding a Logo

1. Place your logo image in `resources/img/` directory:
   ```
   resources/img/logo.svg
   ```

2. Update `login.ftl`, `register.ftl`, etc. header section:
   ```html
   <div class="cp-header">
       <img src="${url.resourcesPath}/img/logo.svg" alt="CoursePlatform" class="cp-logo" />
       <h1 class="cp-title">CoursePlatform</h1>
       <p class="cp-subtitle">${msg("loginTitleHtml",(realm.displayNameHtml!''))}</p>
   </div>
   ```

3. Add logo styles to `login.css`:
   ```css
   .cp-logo {
       width: 48px;
       height: 48px;
       margin: 0 auto 1rem;
       display: block;
   }
   ```

### Changing Text

Edit `messages/messages_en.properties`:

```properties
loginTitleHtml=Sign in to your account
registerTitle=Create your account
# ... add or modify messages ...
```

For additional languages, create new files:
- `messages_es.properties` (Spanish)
- `messages_fr.properties` (French)
- etc.

Update `theme.properties` to include new locales:
```properties
locales=en,es,fr
```

### Adding Custom JavaScript

1. Create a JavaScript file in `resources/js/`:
   ```
   resources/js/custom.js
   ```

2. Update `theme.properties` to include it:
   ```properties
   scripts=js/custom.js
   ```

### Creating Additional Templates

Keycloak supports many other templates:
- `login-update-password.ftl` - Update password page
- `login-update-profile.ftl` - Update profile page
- `login-verify-email.ftl` - Email verification page
- `login-otp.ftl` - Two-factor authentication page
- `info.ftl` - Info page
- etc.

Copy the structure from existing templates and customize as needed.

## ?? Development Workflow

### Live Reload (Manual)

Keycloak caches themes. To see changes:

1. **Clear Keycloak cache**: Restart the Keycloak container
   ```bash
   # Using .NET Aspire dashboard
   # Stop and start the Keycloak resource
   ```

2. **Disable caching during development**:
   
   Update `AppHost.cs` to add environment variable:
   ```csharp
   .WithEnvironment("KC_SPI_THEME_STATIC_MAX_AGE", "-1")
   .WithEnvironment("KC_SPI_THEME_CACHE_THEMES", "false")
   .WithEnvironment("KC_SPI_THEME_CACHE_TEMPLATES", "false")
   ```

3. **Hard refresh browser**: Ctrl+F5 or Cmd+Shift+R

### Testing Locally

You can test the theme before deploying:

1. Make changes to FreeMarker templates or CSS
2. Restart Keycloak container
3. Navigate to login page
4. Test all flows:
   - Login
   - Registration
   - Password reset
   - Error handling
   - Social login (if configured)

## ?? Theme Configuration Reference

### theme.properties Options

```properties
# Inherit from parent theme (base Keycloak theme)
parent=keycloak

# Stylesheets to include (comma-separated)
styles=css/login.css,css/custom.css

# Scripts to include (comma-separated)
scripts=js/custom.js

# Import base theme styles
import=common/keycloak

# Supported locales
locales=en,es,fr

# Default locale
defaultLocale=en
```

## ?? Best Practices

1. **Keep it Simple**: Avoid over-customizing; maintain usability
2. **Test Accessibility**: Use screen readers and keyboard navigation
3. **Mobile First**: Test on mobile devices early
4. **Performance**: Optimize images and minimize CSS/JS
5. **Consistency**: Match your main application's design system
6. **Security**: Don't expose sensitive information in client-side code
7. **Version Control**: Track theme changes in Git
8. **Documentation**: Update this README when making significant changes

## ?? Troubleshooting

### Theme Not Showing Up

**Problem**: Custom theme not appearing in Keycloak admin dropdown

**Solutions**:
1. Verify theme files are in correct directory structure
2. Check `theme.properties` exists and is properly formatted
3. Restart Keycloak container
4. Check Keycloak logs for theme loading errors

### Styles Not Applying

**Problem**: CSS changes not visible

**Solutions**:
1. Clear browser cache (Ctrl+F5)
2. Disable theme caching (see Development Workflow)
3. Check CSS file path in `theme.properties`
4. Inspect browser developer tools for CSS loading errors

### FreeMarker Template Errors

**Problem**: White page or error message

**Solutions**:
1. Check Keycloak container logs: `docker logs <container-id>`
2. Validate FreeMarker syntax
3. Ensure all required variables are defined
4. Compare with parent theme template

### Custom Messages Not Displaying

**Problem**: Default messages showing instead of custom ones

**Solutions**:
1. Verify `messages_en.properties` file exists in `messages/` directory
2. Check property keys match exactly (case-sensitive)
3. Ensure locale is configured in `theme.properties`
4. Restart Keycloak container

## ?? Resources

- [Keycloak Server Developer Guide - Themes](https://www.keycloak.org/docs/latest/server_development/#_themes)
- [FreeMarker Template Language](https://freemarker.apache.org/docs/)
- [Keycloak Theme Properties Reference](https://www.keycloak.org/docs/latest/server_development/#theme-properties)
- [CoursePlatform Frontend Theme](../../Frontend/src/index.css)

## ?? Maintenance Notes

- **Theme Version**: 1.0.0
- **Compatible Keycloak Version**: 26.4.7
- **Last Updated**: 2024
- **Maintainer**: CoursePlatform Team

---

For questions or issues with the theme, please contact the development team or create an issue in the repository.
