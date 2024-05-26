using Hologram.NINA.FlatEpoch.Utility;
using Newtonsoft.Json;
using NINA.Core.Locale;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Core.Utility.Notification;
using NINA.Core.Model.Equipment;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Equipment.Model;
using NINA.Profile;
using NINA.Profile.Interfaces;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.SequenceItem.Camera;
using NINA.Sequencer.SequenceItem.FlatDevice;
using NINA.Sequencer.SequenceItem.Imaging;
using NINA.Sequencer.Validations;
using NINA.WPF.Base.Interfaces.Mediator;
using NINA.WPF.Base.Interfaces.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Settings = Hologram.NINA.FlatEpoch.Properties.Settings;
using NINA.Equipment.Interfaces;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Collections.ObjectModel;

namespace Hologram.NINA.FlatEpoch.SequenceItems {
    [ExportMetadata("Name", "Take Epoch Trained Flats")]
    [ExportMetadata("Description", "Evaluate filesystem based Light and Flat frames for the current epoch and take any Flats still needed")]
    [ExportMetadata("Icon", "BrainBulbSVG")]
    [ExportMetadata("Category", "Flat Epoch")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class TakeTrainedEpochFlats : SequenceItem, IValidatable {
        private readonly IProfileService profileService;
        private readonly ICameraMediator cameraMediator;
        private readonly IImagingMediator imagingMediator;
        private readonly IImageSaveMediator imageSaveMediator;
        private readonly IFilterWheelMediator filterWheelMediator;
        private readonly IRotatorMediator rotatorMediator;
        private readonly IFlatDeviceMediator flatDeviceMediator;
        private readonly IImageHistoryVM imageHistoryVM;

        private bool retakeFlats;
        private bool panelKeepClosed;
        private Guid radioGroupGUID = Guid.NewGuid();
        private bool stdEpochAdvance = true;
        private bool alwaysEpochAdvance;
        private bool noEpochAdvance;
        private string progressVizState;
        private string progressMessage;
        private string progressCurrent;
        private string progressTotal;
        private List<FlatFrameSetting> flatFrameSettings;

        [ImportingConstructor]
        public TakeTrainedEpochFlats(
            IProfileService profileService,
            ICameraMediator cameraMediator,
            IImagingMediator imagingMediator,
            IImageSaveMediator imageSaveMediator,
            IFilterWheelMediator filterWheelMediator,
            IRotatorMediator rotatorMediator,
            IFlatDeviceMediator flatDeviceMediator,
            IImageHistoryVM imageHistoryVM) {
            this.profileService = profileService;
            this.cameraMediator = cameraMediator;
            this.imagingMediator = imagingMediator;
            this.imageSaveMediator = imageSaveMediator;
            this.filterWheelMediator = filterWheelMediator;
            this.rotatorMediator = rotatorMediator;
            this.flatDeviceMediator = flatDeviceMediator;
            this.imageHistoryVM = imageHistoryVM;
        }
        
        public TakeTrainedEpochFlats(TakeTrainedEpochFlats cloneMe,
            IProfileService profileService,
            ICameraMediator cameraMediator,
            IImagingMediator imagingMediator,
            IImageSaveMediator  imageSaveMediator,
            IFilterWheelMediator filterWheelMediator,
            IRotatorMediator rotatorMediator,
            IFlatDeviceMediator flatDeviceMediator,
            IImageHistoryVM imageHistoryVM) {
            this.profileService = profileService;
            this.cameraMediator = cameraMediator;
            this.imagingMediator = imagingMediator;
            this.imageSaveMediator = imageSaveMediator;
            this.filterWheelMediator = filterWheelMediator;
            this.rotatorMediator = rotatorMediator;
            this.flatDeviceMediator = flatDeviceMediator;
            this.imageHistoryVM = imageHistoryVM;

            if (cloneMe != null) {
                CopyMetaData(cloneMe);
            }

        }

        private IList<string> issues = new List<string>();

        public IList<string> Issues {
            get => issues;
            set {
                issues = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public Guid RadioGroupGUID {
            get => radioGroupGUID;
            set {
                radioGroupGUID = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public bool StdEpochAdvance {
            get => stdEpochAdvance;
            set {
                stdEpochAdvance = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public bool AlwaysEpochAdvance {
            get => alwaysEpochAdvance;
            set {
                alwaysEpochAdvance = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public bool NoEpochAdvance {
            get => noEpochAdvance;
            set {
                noEpochAdvance = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public bool RetakeFlats {
            get => retakeFlats;
            set {
                retakeFlats = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public bool PanelKeepClosed {
            get => panelKeepClosed;
            set {
                panelKeepClosed = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public string ProgressVizState {
            get => progressVizState;
            set {
                progressVizState = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public string ProgressMessage {
            get => progressMessage;
            set {
                progressMessage = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public string ProgressCurrent {
            get => progressCurrent;
            set {
                progressCurrent = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public string ProgressTotal {
            get => progressTotal;
            set {
                progressTotal = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public List<FlatFrameSetting> FlatFrameSettings {
            get => flatFrameSettings;
            set {
                flatFrameSettings = value;
                RaisePropertyChanged();
            }
        }

        public override object Clone() {
            var clone = new TakeTrainedEpochFlats(this, profileService, cameraMediator, imagingMediator, imageSaveMediator, filterWheelMediator, rotatorMediator, flatDeviceMediator, imageHistoryVM) {
                RetakeFlats = RetakeFlats,
                PanelKeepClosed = PanelKeepClosed,
                RadioGroupGUID = RadioGroupGUID,
                StdEpochAdvance = StdEpochAdvance,
                AlwaysEpochAdvance = AlwaysEpochAdvance,
                NoEpochAdvance = NoEpochAdvance,
            };
            return clone;
        }

        public async override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            try {

                if (Settings.Default.outputColor == "Red") {
                    Logger.Error($"Flat Epoch not configured correctly. Message is: {Settings.Default.validationOutput}");
                    Notification.ShowInformation($"Flat Epoch not configured correctly. Check log - Skipping Instruction");
                    throw new SequenceEntityFailedException("Flat Epoch not configured correctly. Check log - Skipping Instruction");
                }

                string imagesDirectory = profileService.ActiveProfile.ImageFileSettings.FilePath;

                Logger.Debug($"Epoch KEY: {Settings.Default.labEpoch}  Defined KEYS: {Settings.Default.labFilter}, {Settings.Default.labGain}, {Settings.Default.labOffset}, {Settings.Default.labBin}, {Settings.Default.labReadOut}, {Settings.Default.labRot}");
                Logger.Debug($"Evaluating LIGHT frames in the {imagesDirectory} FilePath for Epoch {Settings.Default.currEpoch}");
                
                List<Dictionary<string, string>> lights = EvalEpochFiles.ParseImageDirectory(imagesDirectory, Settings.Default.labEpoch, Settings.Default.currEpoch, "LIGHT");
                Logger.Debug($"LIGHT frames found = {lights.Count}");

                Logger.Debug($"Evaluating FLAT frames in the {imagesDirectory} FilePath for Epoch {Settings.Default.currEpoch}");
                List<Dictionary<string, string>> flats = EvalEpochFiles.ParseImageDirectory(imagesDirectory, Settings.Default.labEpoch, Settings.Default.currEpoch, "FLAT");
                Logger.Debug($"FLAT frames found = {flats.Count}");
               
                if (RetakeFlats) {
                    flats.Clear();
                    Logger.Debug("Deduplication of existing FLAT frames skipped due to instruction setting. Retaking flats for all lights.");
                }

                List<Dictionary<string, string>> requiredFlats = new List<Dictionary<string, string>>();
                foreach (var dict in lights) {
                    if (!flats.Contains(dict, new DictionaryComparer())) {
                        requiredFlats.Add(dict);
                    }
                }
                Logger.Debug($"Missing unique combinations from FLAT = {requiredFlats.Count}");

                if (requiredFlats.Count == 0) {
                    ProgressVizState = "Visible";
                    EvalEpochAdvance(false);
                    await Task.Delay(2500);
                    ProgressMessage = " ";
                    ProgressVizState = "Collapsed";
                    return;
                }

                Logger.Info($"Flat Epoch - Required FLAT frames for epoch {Settings.Default.currEpoch}:");
                foreach (var dict in requiredFlats) {
                    string dictString = "{ ";
                    foreach (var kvp in dict) {
                        dictString += $"  {kvp.Key}: {kvp.Value}";
                    }
                    dictString += " }";

                    var singleFilter = profileService?.ActiveProfile?.FilterWheelSettings?.FilterWheelFilters.FirstOrDefault(filter => filter.Name == dict[Settings.Default.labFilter]);
                    dictString += $" at filter position {singleFilter.Position}";

                    Logger.Info(dictString);
                }

                Collection<TrainedFlatExposureSetting> tfExposureSettings = profileService.ActiveProfile.FlatDeviceSettings.TrainedFlatExposureSettings;

                int totalRequiredFrames = 0;
                FlatFrameSettings = new List<FlatFrameSetting>();
                foreach (var dict in requiredFlats) {
                    // Calculate values and store them in local variables
                    FilterInfo filter = profileService?.ActiveProfile?.FilterWheelSettings?.FilterWheelFilters
                                    .FirstOrDefault(filter => filter.Name == dict[Settings.Default.labFilter]);

                    int gain = dict.TryGetValue(Settings.Default.labGain, out string gainValue) && int.TryParse(gainValue, out int parsedGain)
                                    ? parsedGain
                                    : -1;

                    int offset = dict.TryGetValue(Settings.Default.labOffset, out string offsetValue) && int.TryParse(offsetValue, out int parsedOffset)
                                    ? parsedOffset
                                    : -1;

                    short binX = dict.TryGetValue(Settings.Default.labBin, out string binXValue) && short.TryParse(binXValue, out short parsedBinX)
                                    ? parsedBinX
                                    : (short)1;

                    short binY = dict.TryGetValue(Settings.Default.labBin, out string binYValue) && short.TryParse(binYValue, out short parsedBinY)
                                    ? parsedBinY
                                    : (short)1;

                    float? rotatorAngle = dict.TryGetValue(Settings.Default.labRot, out string rotatorAngleValue) && float.TryParse(rotatorAngleValue, out float parsedRotatorAngle)
                                            ? (float?)parsedRotatorAngle
                                            : null;

                    short? readOutMode = dict.TryGetValue(Settings.Default.labReadOut, out string readOutModeValue) && short.TryParse(readOutModeValue, out short parsedReadOutMode)
                                            ? (short?)parsedReadOutMode
                                            : null;

                    int nrRequired = Util.LookupExposures(Settings.Default.nrFlats, filter.Name);

                    Logger.Debug($"Lookup values: Filter = {filter.Position}, Gain = {gain}, Offset = {offset}, BinX = {binX}, BinY = {binY} ");
                    TrainedFlatExposureSetting tfsetting = Util.GetTrainedFlatExposureData(tfExposureSettings, filter, gain, offset, binX, binY);
                    if (tfsetting != null) {
                        Logger.Debug($"tfdata = Filter = {tfsetting.Filter}, Gain = {tfsetting.Gain}, Offset = {tfsetting.Offset}, Time = {tfsetting.Time}, Brightness = {tfsetting.Brightness}  ");
                    } else {
                        Logger.Debug("tfsetting is null");
                    }

                    // Create and add the FlatFrameSetting
                    FlatFrameSettings.Add(new FlatFrameSetting {
                        Filter = filter,
                        RotatorAngle = rotatorAngle,
                        Gain = gain,
                        Offset = offset,
                        BinX = binX,
                        BinY = binY,
                        ReadOutMode = readOutMode,
                        ExposureTime = tfsetting.Time,
                        Brightness = tfsetting.Brightness,
                        NrRequired = nrRequired
                    });
                    totalRequiredFrames += nrRequired;
                }

                // prepare for imaging
                ProgressVizState = "Visible";
                //int padMessage = 55;
                int countTotal = 0;
                int interFrameDelay = Util.LookupExposures(Settings.Default.nrFlats, "msDelay");

                ProgressMessage = "Closing flat panel cover";
                await CloseCover(progress, token);
                ProgressMessage = "Toggling flat panel light";
                await ToggleLight(true, progress, token);


                // Main image capture loop 
                foreach (var setting in FlatFrameSettings) {

                    Logger.Debug($"Changing filter to: {setting.Filter.Name}");
                    ProgressMessage = $"Changing filter to:  {setting.Filter.Name}"; //.PadRight(padMessage);
                    await filterWheelMediator.ChangeFilter(setting.Filter, token);

                    float currentRotatorAngle = rotatorMediator.GetInfo().MechanicalPosition;
                    if (setting.RotatorAngle != null) {
                        if (currentRotatorAngle != (float)setting.RotatorAngle) {
                            Logger.Debug($"Setting rotator mechanical angle to: {setting.RotatorAngle}");
                            ProgressMessage = $"Setting rotator to:  {setting.RotatorAngle}"; //.PadRight(padMessage);
                            await rotatorMediator.MoveMechanical((float)setting.RotatorAngle, token);
                        }
                    }

                    if (setting.ReadOutMode != null) {
                        Logger.Debug($"Setting Readout Mode to : {setting.ReadOutMode}");
                        ProgressMessage = $"Setting Readout Mode to:  {setting.ReadOutMode}"; //.PadRight(padMessage);
                        SetReadoutMode setReadoutMode = new SetReadoutMode(cameraMediator) { Mode = (short)setting.ReadOutMode };
                        await setReadoutMode.Execute(progress, token);
                    }

                    Logger.Debug($"Setting Flat panel brightness to : {setting.Brightness}");
                    ProgressMessage = $"Setting Flat panel brightness to:  {setting.Brightness}"; //.PadRight(padMessage);
                    SetBrightness setBrightness = new SetBrightness(flatDeviceMediator) { Brightness = setting.Brightness };
                    await setBrightness.Execute(progress, token);

                    Logger.Debug($"Taking {setting.NrRequired} FLAT frames : Gain: {setting.Gain}, Offset: {setting.Offset}, Binning: {new BinningMode(setting.BinX, setting.BinY)}, ExpTime: {setting.ExposureTime}");

                    if (interFrameDelay != 0) {
                        ProgressMessage = $"Taking set with interframe delay of {interFrameDelay}ms:  ";
                    } else {
                        ProgressMessage = "Taking set:  "; 
                    }
                    ProgressMessage += $"Filter: {setting.Filter.Name}";
                    if (setting.RotatorAngle != null) {
                        ProgressMessage += $",  Rotation: {setting.RotatorAngle}";
                    }
                    //ProgressMessage = ProgressMessage.PadRight(padMessage);

                    TakeSubframeExposure exposureSettings = new TakeSubframeExposure(profileService, cameraMediator, imagingMediator, imageSaveMediator, imageHistoryVM) {
                        ImageType = CaptureSequence.ImageTypes.FLAT,
                        Gain = setting.Gain,
                        Offset = setting.Offset,
                        Binning = new BinningMode(setting.BinX, setting.BinY),
                        ExposureTime = setting.ExposureTime,
                    };

                    for (int i = 1; i <= setting.NrRequired; i++) {

                        countTotal++;

                        ProgressCurrent = $"Count: {i} of {setting.NrRequired}";
                        ProgressTotal = $"Total Progress: {countTotal} of {totalRequiredFrames}";

                        await exposureSettings.Execute(progress, token);

                        if (interFrameDelay != 0) {
                            await Task.Delay(interFrameDelay);
                        }
                    }

                    ProgressMessage = " "; //.PadRight(padMessage);
                    ProgressCurrent = " ";

                }
                ProgressTotal = " ";

                // done with image capture, post processing

                ProgressMessage = "Toggling flat panel light";
                await ToggleLight(false, progress, token);
                if (!PanelKeepClosed) {
                    ProgressMessage = "Opening flat panel cover";
                    await OpenCover(progress, token);
                }

                EvalEpochAdvance(true);
                await Task.Delay(2500);



            } catch (Exception ex) {
                
                Logger.Error($"Exception during Execute: {ex.HResult} {ex.Message}");
                ProgressMessage = $"Error occured with message {ex.Message} - check logs.";
                await Task.Delay(2500);
                ProgressMessage = " ";
                ProgressCurrent = " ";
                ProgressTotal = " ";
                ProgressVizState = "Collapsed";

            } finally {

                ProgressMessage = " ";
                ProgressCurrent = " ";
                ProgressTotal = " ";
                ProgressVizState = "Collapsed";

            }

            return;
        }


        void EvalEpochAdvance(bool flatstaken) {
            if (stdEpochAdvance && flatstaken) {
                Logger.Debug($"The current Epoch: {Settings.Default.currEpoch}, has ended from standard");
                IncrementEpoch();
                Logger.Debug($"Flat frames acquired. Epoch advanced to: {Settings.Default.currEpoch}, Setting = 'ONLY IF'");
                ProgressMessage = $"Flats taken. Epoch advanced.  EPOCH = {Settings.Default.currEpoch}";
                return;
            }

            if (alwaysEpochAdvance) {
                Logger.Debug($"The current Epoch: {Settings.Default.currEpoch}, has ended from always");
                IncrementEpoch();
                Logger.Debug($"Epoch advanced to: {Settings.Default.currEpoch}, Setting = 'ALWAYS' Flats acquired = {flatstaken}");
                ProgressMessage = $"Setting 'AlWAYS'. Epoch advanced.  EPOCH = {Settings.Default.currEpoch}";
                return;
            }

            if (noEpochAdvance) {
                Logger.Debug($"Epoch was not Advanced. Current Epoch: {Settings.Default.currEpoch}, Setting 'NEVER'");
                ProgressMessage = $"Setting 'NEVER'. Epoch not advanced.  EPOCH = {Settings.Default.currEpoch}";
                return;
            }

            if (stdEpochAdvance) {
                Logger.Debug($"No Flats frames acquired. Epoch was not Advanced due to 'ONLY IF' setting. Epoch stays at: {Settings.Default.currEpoch}");
                ProgressMessage = $"No flats taken. Epoch not advanced.  EPOCH = {Settings.Default.currEpoch}";
                return;
            }

            Logger.Debug($"Impossible logic error: Taken={flatstaken} Std={stdEpochAdvance} Always={alwaysEpochAdvance} Never={noEpochAdvance}");
            return;
        }


        void IncrementEpoch() {
            int epochValue = int.Parse(Settings.Default.currEpoch);
            epochValue++;
            Settings.Default.currEpoch = epochValue.ToString();
            Settings.Default.Save();
        }


        protected async Task CloseCover(IProgress<ApplicationStatus> progress, CancellationToken token) {
            if (!flatDeviceMediator.GetInfo().SupportsOpenClose) {
                return;
            }

            CoverState coverState = flatDeviceMediator.GetInfo().CoverState;

            if (coverState == CoverState.Closed) {
                return;
            }

            Logger.Info("Flat Epoch: Closing flat device cover");
            await flatDeviceMediator.CloseCover(progress, token);

            coverState = flatDeviceMediator.GetInfo().CoverState;
            if (coverState != CoverState.Closed) {
                throw new SequenceEntityFailedException($"Closeing the Flat device cover FAILED");
            }
        }

        protected async Task OpenCover(IProgress<ApplicationStatus> progress, CancellationToken token) {
            if (!flatDeviceMediator.GetInfo().SupportsOpenClose) {
                return;
            }

            CoverState coverState = flatDeviceMediator.GetInfo().CoverState;

            if (coverState == CoverState.Open) {
                return;
            }

            Logger.Info("Flat Epoch: Opening flat device cover");
            await flatDeviceMediator.OpenCover(progress, token);

            coverState = flatDeviceMediator.GetInfo().CoverState;
            if (coverState != CoverState.Open) {
                throw new SequenceEntityFailedException($"Opening the Flat device cover FAILED");
            }
        }

        protected async Task ToggleLight(bool onOff, IProgress<ApplicationStatus> progress, CancellationToken token) {
            if (flatDeviceMediator.GetInfo().LightOn == onOff) {
                return;
            }

            Logger.Info($"Flat Epoch: Toggling flat device light: {onOff}");
            await flatDeviceMediator.ToggleLight(onOff, progress, token);

            if (flatDeviceMediator.GetInfo().LightOn != onOff) {
                throw new SequenceEntityFailedException($"Toggling the Flat device light FAILED");
            }
        }

        public override void AfterParentChanged() {
            Validate();
        }

        public bool Validate() {
            var i = new List<string>();

            if (Settings.Default.outputColor == "Red") {
                i.Add("Flat Epoch not configured correctly");
            }

            if (!cameraMediator.GetInfo().Connected) {
                i.Add(Loc.Instance["LblCameraNotConnected"]);
            }

            if (!filterWheelMediator.GetInfo().Connected) {
                i.Add(Loc.Instance["LblFilterWheelNotConnected"]);
            }

            if (Settings.Default.labRot.Length != 0) {
                if (!rotatorMediator.GetInfo().Connected) {
                    i.Add(Loc.Instance["LblRotatorNotConnected"]);
                }
            }

            if (!flatDeviceMediator.GetInfo().Connected) {
                i.Add(Loc.Instance["LblFlatDeviceNotConnected"]);
            } else {
                if (!flatDeviceMediator.GetInfo().SupportsOnOff) {
                    i.Add(Loc.Instance["LblFlatDeviceCannotControlBrightness"]);
                }
            }

            Issues = i;
            return i.Count == 0;
        }

    }


    public class FlatFrameSetting {
        public FilterInfo Filter { get; set; }
        public float? RotatorAngle { get; set; }
        public int Gain { get; set; }
        public int Offset { get; set; }
        public short BinX { get; set; }
        public short BinY { get; set; }
        public short? ReadOutMode { get; set; }
        public double ExposureTime { get; set; }
        public int Brightness { get; set; }
        public int NrRequired { get; set; }
    }

}