# NINA Plugin Flat Epoch
 Flat Epoch Plugin for N.I.N.A.


# Documentation

## Flat Epoch

"An Epoch is just a chunk of time, like a period in history. Here, it's
defined as all the LIGHT frames taken during that period, and that are
saved in the Image file path. This plugin takes any FLAT’s needed to
match the LIGHT’s, based on the criteria you define. It then advances to
the next Epoch."

### Overview

Flat Epoch is a utility for acquiring FLAT frames based on your captured
LIGHT frames, tailored to unique combinations of filters, camera
settings (gain, offset, binning, readout mode), and rotation that are
important to your workflow. The current epoch progresses once all
necessary FLATs for the existing LIGHTs are captured.

When to capture the flats and advance the epoch depends on your
equipment and workflow. Flat Epoch is designed for automated workflows
with a permanent setup and motorized flat panels but can also benefit
manual workflows.

For instance, with an imprecise rotator, capture FLATs before rotating.
Without a rotator or with a precise rotator, capture FLATs daily at the
end of each session. There can also be a benefit in workflows that use a
hands-on approach to manually capture flats, either at the start or end
of a session, or even less frequently, such as every few days.

### Requirements

To run Flat Epoch successfully, the following requirements should be
met:

- Image File Pattern: This must be setup to ensure your image files
  conform to the Flat Epoch file naming rules.

- ASCOM Compliant Flat Panel: Have the capability to take trained
  flats using an ASCOM compliant flat panel and have trained exposure
  values for all combinations of filter, binning gain and offset used
  in your workflow.

- Image File Path: You leave all acquired images in the N.I.N.A
  "Image File Path" for the duration of the current epoch before
  moving then to another location. This image data for the current
  epoch is used to determine which flats are to be acquired.

### User Interface

<img src="https://github.com/GrahamHollis/NINA-Plugin-Flat-Epoch/blob/main/assets/FElanding.png" width="800">

The landing page shown above is where Flat Epoch is configured. To ensure
everything is set up correctly, click the ```Check Flat Epoch
Configuration``` button. This action will verify your settings. The Flat
Epoch instruction ‘Take Epoch Trained Flats’ will only execute if the
green ‘PASSED’ message is displayed.

### Specific configuration settings

**Definitions**

- IFP = Image File Pattern found in N.I.N.A. Options -\> Imaging
- FQFN = Fully qualified file name of saved images

**Current Flat Epoch nr.**

This is a simple sequence number that will be substituted in the it
FQFN using the IFP pattern name ```$$FLAT_EPOCH$$```

**Frames per Flat**

In its simplest form, this is the default number of frames that will
be captured per unique combination of filter, gain. Offset, binning,
readout mode and rotation that exists in the set of light frames
captured in the epoch. This must be in the range of 3 to 200

Example: ```30```

**Optionally**, a millisecond delay between flat frames captured by Flat
Epoch can be specified in the form :200. This delay, if specified, must
be in the range 1 to 2000.

Example: ```30:200```

**Optionally**, filter specific overrides can be specified in the form
```<filter name>=<20>``` by way of a comma separated list. The override
value can be either 0 or the range 3 to 200. A value of 0 will cause any
flat for this filter to be skipped.

Example ```30, Ha=25, Sii=25, Oiii=25```

The filter override for can also be used with a delay

Example ```30:100, L=0, Ha=20```

If a filter name used does not match an override name, the default
value will be used. The filter names must be 5 characters or less. There
can be up to 10 filter overrides.

**Keys**

Each key defined must appear in the IFP, and each key can only appear once.
The mandatory keys that need to be defined are EPOCH and FILTER. The remaining
keys—CAMERA GAIN, CAMERA OFFSET, BINNING, READOUT MODE, and ROTATION—depend on
your workflow to determine if they are needed. Any key that represents a value
that could potentially change during the imaging session should be defined in
the Flat Epoch configuration. This allows them to be tracked, and matching
flats can be taken for those changing values.

The keys defined need to appear in the IFP along with its matching
pattern name to form the key_value pair. The associated patterns are:


| Key Name | Key_Value Pair |
| ---------| -------------- |
| **Epoch** | ```<key>_$$FLAT_EPOCH$$``` |
| **Filter** | ```<key>_$$FILTER$$``` |
| **Camera Gain** | ```<key>_$$GAIN$$``` |
| **Camera Offset** | ```<key>_$$OFFSET$$``` |
| **Binning** | ```<key>_$$BINNING$$``` |
| **Readout Mode** | ```<key>_$$READOUTMODE$$``` |
| **Rotator Angle** | ```<key>_$$OTATORANGLE$$``` |


The keys defined need to comply with the rules that follow.

**Image File Pattern RULES**

- IFP = ‘Image File Pattern’ as defined in N.I.N.A. Options-\>Imaging
- KVP = Key Value Pair as in KEY_VALUE or KEY-VALUE**
- Supported KVP prefix and suffix are \_ - \\

1.  The IFP must start with the pattern name ```$$IMAGETYPE$$```

2.  The key’s defined must be unique from each other and occur only
    once in the IFP

3.  The Epoch and Filter keys along with their patterns are mandatory,
    the rest are optional

4.  Each key defined must be included in the IFP along with their
    associated pattern

5.  The Epoch ```<key>_$$FLAT_EPOCH$$``` must be a directory level above all
    the other keywords

6.  If a Flat IFP is defined, it must comply with all the rules above

An example of a conforming IFP, and the one I use is:

```$$IMAGETYPE$$\$$TELESCOPE$$\$$TARGETNAME$$\SESSION_$$DATEMINUS12$$_EPOCH_$$FLAT_EPOCH$$\$$IMAGETYPE$$_$$TARGETNAME$$_$$DATETIME$$_FILTER_$$FILTER$$_ROT_$$ROTATORANGLE$$_$$EXPOSURETIME$$s_$$FRAMENR$$```

### Sequencer Instruction Options

<img src="https://github.com/GrahamHollis/NINA-Plugin-Flat-Epoch/blob/main/assets/FEinstruction.png" width="800">

**Epoch Advance**

**Only if Flats Taken**: This is the default and normal setting. If flats
are taken such that all lights have matching flats, the epoch will be
advanced. If no frames were captured by the instruction the epoch will
remain unchanged. In most automated workflows this will be the setting
to use.

**Never**: There are not many use cases to set never. Apart from for
testing purposes, if its desired to take early flats when using Target
Scheduler, the Flat Epoch instruction could be placed both in ‘After New
Target Instruction’ and in ‘After Each Target Instruction’. The one
placed in the latter should be set to ‘Never’. And the former to
‘Always’ The effect of this will be to take early flats and when the
object is complete, to advance the Epoch.

**Always**: This can be set at the end of an imaging session to make sure,
first of all that all lights have matching flats and then to make sure
the next session starts with a new Epoch. Especially true if taking
manual trained flats at the start of a session and need an automated way
to increment the Epoch. Although it cannot harm anything, using this
setting could potentially leave holes in the Epoch sequence number by
skipping an Epoch.

**Retake existing**

Flat Epoch evaluates existing Light frames as well as existing Flat
frames for unique sets of defined keys. It then deduplicates the
resulting sets so as not to retake existing flats. If this is set to
‘ON’ then it takes flats for every unique combination of keys found in
the light frames without first deduplicating the list.

**Keep closed**

Default is to open the flat panel cover after taking flats. Typically
want to switch this ‘ON’ when taking flats in the unsafe condition or at
end of session and leave it ‘OFF’ at other times.

### Sequencer Instruction Execution

During execution, the instruction will provide details of the current
flat set in progress, including a count of flats taken. Additionally,
it will provide a milestone count of flats taken out of the total being
processed.

<img src="https://github.com/GrahamHollis/NINA-Plugin-Flat-Epoch/blob/main/assets/FEexec2.png" width="800">


## Workflow Examples


### End of Session FLATS

**Suitable for**: Workflows without a rotator or with a precision repeatable
rotator, using an ASCOM-compatible flat panel with trained flats.

**With a DSO Instruction Set**: At the end of an imaging session comprising
one or many targets, place a single 'Flat Epoch - Take Epoch Trained
Flats' instruction in the sequence after imaging completes but before
warming the camera or disconnecting equipment. When the instruction
executes, it will evaluate all LIGHT frames captured during the session,
take matching FLATs, and then advance the epoch ready for the next
session.

**With Target Scheduler**: Disable the project setting ‘Flats Handling’ for
each of the active projects and then place the Flat Epoch instruction as
detailed above.


### During Session FLATS (Immediate Flats)

**Suitable for**: Workflows using a rotator that cannot reacquire a precise
mechanical angle, using an ASCOM-compatible flat panel with trained
flats.

**With a DSO Instruction Set**: At the end of imaging a target, place a
'Flat Epoch - Take Epoch Trained Flats' instruction in the sequence
before moving the rotator. This ensures each target has its own epoch
during the session.

**With Target Scheduler**: Disable the project setting ‘Flats Handling’ for
each of the active projects and place the Flat Epoch instruction in the
‘After New Target Instructions’ container within the ‘Target Scheduler
Container’. This differs from the Target Scheduler recommendation to
place the ‘Target Scheduler Immediate Flats’ in the ‘After Each Target
Instructions’ container.

**Additional Considerations**: The Flat Epoch instruction should also be
placed in the Unsafe Event workflow to capture any flats for the LIGHTs
taken immediately prior to the unsafe event.
For additional security, place a Flat Epoch instruction before imaging
begins and after imaging ends. This ensures that flats are captured even
if the workflow is interrupted and restarted, or if an error causes imaging
to terminate prematurely.


### Start or End of Session Manual FLATS

**Suitable for**: Workflows that use a manually positioned flat panel with
trained flats.

**Start of Session FLATS**: Capture FLATs at the start of the session
using the standard N.I.N.A. Trained Flat Instruction and the manually
positioned flat panel. Place the Flat Epoch instruction at end of
imaging session with the ‘Always’ radio button selected. This will serve
to increment the Epoch but also as a safety to check for any required
flats.

**End of Session FLATS**: Capture FLATs at the end of the session using a
Sequencer Powerups ‘Wait Indefinitely’ to pause the sequence and allow for
the manual positioning of the flat panel. Once in place. proceed with the
sequence to execute the Flat Epoch instruction.




## Future enhancements: (the wish list)

- During deduplication of existing flats and lights, take the delta
  flats, if the number of flats that exist for the unique combination
  are less that the defined Frames per Flat.
- If a combination of filter,gain,offset and binning does not exist in
  the trained flat table, auto train for that combination and add it to
  the table for future. 


If you have any question or suggestions, I can be reached as @Hologram
on the N.I.N.A. discord server.

 