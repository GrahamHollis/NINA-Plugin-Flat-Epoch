using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: Guid("3369ba5a-8c3f-46f6-bb49-9ba27afacd0f")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyTitle("Flat Epoch")]
[assembly: AssemblyDescription("An Epoch is just a chunk of time, like a period in history. Here, it's defined as all the LIGHT frames taken during that period, and that are saved in the Image file path." +
    " This plugin takes any FLAT’s needed to match the LIGHT’s, based on the criteria you define. It then advances to the next Epoch.")]

[assembly: AssemblyCompany("Graham Hollis  @Hologram")]
[assembly: AssemblyProduct("Flat Epoch")]
[assembly: AssemblyCopyright("Copyright © 2024 Graham Hollis")]

[assembly: AssemblyMetadata("MinimumApplicationVersion", "3.0.0.2017")]

// The license your plugin code is using
[assembly: AssemblyMetadata("License", "MPL-2.0")]
// The url to the license
[assembly: AssemblyMetadata("LicenseURL", "https://www.mozilla.org/en-US/MPL/2.0/")]
// The repository where your pluggin is hosted
[assembly: AssemblyMetadata("Repository", "https://github.com/GrahamHollis/NINA-Plugin-Flat-Epoch")]

// The following attributes are optional for the official manifest meta data

//[Optional] Your plugin homepage URL - omit if not applicaple
[assembly: AssemblyMetadata("Homepage", "")]

//[Optional] Common tags that quickly describe your plugin
[assembly: AssemblyMetadata("Tags", "flat,flats,flat epoch,epoch,automation")]

//[Optional] A link that will show a log of all changes in between your plugin's versions
[assembly: AssemblyMetadata("ChangelogURL", "")]

//[Optional] The url to a featured logo that will be displayed in the plugin list next to the name
[assembly: AssemblyMetadata("FeaturedImageURL", "https://raw.githubusercontent.com/GrahamHollis/NINA-Plugin-Flat-Epoch/main/assets/FElogo.png")]
//[Optional] A url to an example screenshot of your plugin in action
[assembly: AssemblyMetadata("ScreenshotURL", "")]
//[Optional] An additional url to an example example screenshot of your plugin in action
[assembly: AssemblyMetadata("AltScreenshotURL", "")]
//[Optional] An in-depth description of your plugin
[assembly: AssemblyMetadata("LongDescription", @"# Flat Epoch Method for Flat Frame Acquisition

Flat epoch is a method created to simplify the acquisition of FLAT frames in an automated workflow. Here's a breakdown of how this method operates:

## Concept of Flat Epoch

### Definition

- **Flat Frames**: Calibration frames used to correct for variations in pixel sensitivity and optical artifacts like dust motes or vignetting in the imaging system.
- **Light Frames**: The actual images of celestial objects captured through the telescope using different filters.
- **Epoch**: A period during which a specific set of light frames are captured and for which corresponding flat frames need to be acquired.

### Flat Epoch Method

- The method involves using existing light and flat frames on the filesystem to determine which additional flat frames are required.
- Flat frames are only captured for the filters that were used during the current epoch, ensuring no unnecessary flats are taken.
- To accomplish this, a structured Image File Pattern is required. Flat Epoch validates that you have the structure correct.  

### Determining Needed Flat Frames

- The system checks which flat frames already exist in the filesystem.
- It compares this with the list of filters and other defined keys used in the current epoch's light frames to identify which flat frames are missing.

### Capturing Missing Flat Frames

- Flat Epoch uses a single instruction to capture the missing flat frames.
- Once all necessary flat frames for the captured light frames are obtained, the epoch is considered complete.

### Advancing the Epoch

- The epoch advances to a new period once the flats for the previous epoch are captured.
- The process repeats for the next set of light frames from the next object or next session

## Links 

- [Documentation](https://github.com/GrahamHollis/NINA-Plugin-Flat-Epoch/blob/main/docs/README.md)
- [Source code](https://github.com/GrahamHollis/NINA-Plugin-Flat-Epoch)
- [MPL 2.0 license](https://github.com/GrahamHollis/NINA-Plugin-Flat-Epoch/blob/main/source/LICENSE.txt)

Contact me, @Hologram on the [N.I.N.A Discord server](https://discord.gg/QHG93eVz) is you have any questions or suggestions.


")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
// [Unused]
[assembly: AssemblyConfiguration("")]
// [Unused]
[assembly: AssemblyTrademark("")]
// [Unused]
[assembly: AssemblyCulture("")]