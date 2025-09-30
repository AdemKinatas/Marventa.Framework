# Package Icon

## Current Status

The package icon is defined in `icon.svg` but needs to be converted to `icon.png` for NuGet package display.

## Creating the Icon

### Option 1: Using SVG to PNG Converter (Recommended)

1. Open `icon.svg` in any SVG editor (Inkscape, Adobe Illustrator, or online tools)
2. Export as PNG with these settings:
   - Width: 256px
   - Height: 256px
   - DPI: 96
   - Background: Transparent or #512BD4
3. Save as `icon.png` in the root directory

### Option 2: Using Online Tools

1. Go to https://svgtopng.com or https://cloudconvert.com/svg-to-png
2. Upload `icon.svg`
3. Set output size to 256x256
4. Download as `icon.png`
5. Place in root directory

### Option 3: Using ImageMagick (Command Line)

```bash
# Install ImageMagick first
magick convert -background none -resize 256x256 icon.svg icon.png
```

### Option 4: Using Inkscape (Command Line)

```bash
# Install Inkscape first
inkscape icon.svg --export-type=png --export-width=256 --export-filename=icon.png
```

## Design Guidelines

The current icon design follows NuGet package icon best practices:

- **Size**: 256x256 pixels (required)
- **Format**: PNG with transparency
- **Colors**:
  - Primary: #512BD4 (Microsoft .NET purple)
  - Accent: #00D4FF (cyan)
  - Text: #FFFFFF (white)
- **Elements**:
  - Large "M" for Marventa
  - "FRAMEWORK" text at bottom
  - Decorative dots for visual interest

## Customization

To customize the icon, edit `icon.svg`:

```svg
<!-- Change background color -->
<rect width="256" height="256" fill="#YOUR_COLOR" rx="32"/>

<!-- Change letter -->
<text x="128" y="180">YOUR_LETTER</text>

<!-- Change accent colors -->
<circle fill="#YOUR_ACCENT_COLOR" />
```

## Verification

After creating `icon.png`:

1. Check file size: Should be < 1 MB
2. Check dimensions: Must be exactly 256x256
3. Verify in package:
   ```bash
   dotnet pack
   # Check nupkg with NuGet Package Explorer
   ```

## NuGet Package Requirements

- ✅ Format: PNG
- ✅ Size: 256x256 pixels
- ✅ File size: < 1 MB
- ✅ Transparency: Supported
- ✅ Path: Root directory of repository

## Future Improvements

Consider creating variations:
- Light mode version
- Dark mode version
- Simplified version for small displays
- Animated SVG for web use