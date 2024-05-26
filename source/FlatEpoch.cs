using Hologram.NINA.FlatEpoch.Utility;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using NINA.WPF.Base.Interfaces.ViewModel;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Settings = Hologram.NINA.FlatEpoch.Properties.Settings;


namespace Hologram.NINA.FlatEpoch {

    [Export(typeof(IPluginManifest))]
    public class FlatEpoch : PluginBase, INotifyPropertyChanged {
        private readonly IPluginOptionsAccessor pluginSettings;
        private readonly IProfileService profileService;
        private readonly IImageSaveMediator imageSaveMediator;

        // Implementing a file pattern
        private static readonly ImagePattern EpochImagePattern = new ImagePattern("$$FLAT_EPOCH$$", "The current Flat Epoch sequence number", "Flat Epoch");

        [ImportingConstructor]
        public FlatEpoch(IProfileService profileService, IImageSaveMediator imageSaveMediator,
            IOptionsVM options) {
            if (Settings.Default.UpdateSettings) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                CoreUtil.SaveSettings(Settings.Default);
            }

            this.profileService = profileService;
            this.imageSaveMediator = imageSaveMediator;
            
            // React on a changed profile
            profileService.ProfileChanged += ProfileService_ProfileChanged;

            // for updating file pattern with current epoch number
            imageSaveMediator.BeforeFinalizeImageSaved += BeforeFinalizeImageSaved;

            // Register a new image file pattern for the Options > Imaging > File Patterns area
            options.AddImagePattern(EpochImagePattern);

            PropertyChanged += FlatEpoch_PropertyChanged;

            ValidateImagePattern = new RelayCommand(RunValidatePattern);

            RunValidatePattern(EventArgs.Empty);

        }

        public ICommand ValidateImagePattern { get; private set; }

        private void RunValidatePattern(object obj) {

            string filepattern = profileService.ActiveProfile.ImageFileSettings.FilePattern;
            string filepatternflat = profileService.ActiveProfile.ImageFileSettings.FilePatternFLAT;

            (bool Passed, string validationMessage) = Validate.CheckFilePattern(filepattern, LabEpoch, LabFilter, LabGain, LabOffset, LabBin, LabReadOut, LabRot, NrFlats, CurrEpoch);
            if (!Passed) {
                ValidationOutput = validationMessage;
                OutputColor = "Red";
            } else {
                if (filepatternflat.Length > 0) {
                    (Passed, validationMessage) = Validate.CheckFilePattern(filepatternflat, LabEpoch, LabFilter, LabGain, LabOffset, LabBin, LabReadOut, LabRot, NrFlats, CurrEpoch);
                    if (!Passed) {
                        validationMessage = "FLAT IFP " + validationMessage;
                        OutputColor = "Red";
                    } else {
                        OutputColor = "Green";
                    }
                } else {
                    OutputColor = "Green";
                }
            }
            ValidationOutput = validationMessage;
        }

        protected Task BeforeFinalizeImageSaved(object sender, BeforeFinalizeImageSavedEventArgs args) {

            //string epoch = Settings.Default.currEpoch.ToString();
            ImagePattern proto = EpochImagePattern;
            args.AddImagePattern(new ImagePattern(proto.Key, proto.Description) { Value = CurrEpoch });

            Logger.Trace($"Imagepattern value set to {CurrEpoch}");

            return Task.CompletedTask;

        }

        public string CurrEpoch {
            get => Settings.Default.currEpoch;
            set {
                Settings.Default.currEpoch = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string NrFlats {
            get => Settings.Default.nrFlats;
            set {
                Settings.Default.nrFlats = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string LabEpoch {
            get => Settings.Default.labEpoch;
            set {
                Settings.Default.labEpoch = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string LabFilter {
            get => Settings.Default.labFilter;
            set {
                Settings.Default.labFilter = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string LabGain {
            get => Settings.Default.labGain;
            set {
                Settings.Default.labGain = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string LabOffset {
            get => Settings.Default.labOffset;
            set {
                Settings.Default.labOffset = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string LabBin {
            get => Settings.Default.labBin;
            set {
                Settings.Default.labBin = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string LabReadOut {
            get => Settings.Default.labReadOut;
            set {
                Settings.Default.labReadOut = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string LabRot {
            get => Settings.Default.labRot;
            set {
                Settings.Default.labRot = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string OutputColor {
            get => Settings.Default.outputColor;
            set {
                Settings.Default.outputColor = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public string ValidationOutput {
            get => Settings.Default.validationOutput;
            set {
                Settings.Default.validationOutput = value;
                Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public override Task Teardown() {
            imageSaveMediator.BeforeFinalizeImageSaved -= BeforeFinalizeImageSaved;
            return base.Teardown();
        }
        
        private void ProfileService_ProfileChanged(object sender, EventArgs e) {
            // Rase the event that this profile specific value has been changed due to the profile switch
            //RaisePropertyChanged(nameof(ProfileSpecificNotificationMessage));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Event handler for PropertyChanged event
        private void FlatEpoch_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(LabEpoch) ||
                e.PropertyName == nameof(LabFilter) ||
                e.PropertyName == nameof(LabGain) ||
                e.PropertyName == nameof(LabOffset) ||
                e.PropertyName == nameof(LabBin) ||
                e.PropertyName == nameof(LabReadOut) ||
                e.PropertyName == nameof(LabRot) ||
                e.PropertyName == nameof(NrFlats)) {
                RunValidatePattern(EventArgs.Empty);
            }
        }
    }
}
