# Flat Epoch Method for Flat Frame Acquisition

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
