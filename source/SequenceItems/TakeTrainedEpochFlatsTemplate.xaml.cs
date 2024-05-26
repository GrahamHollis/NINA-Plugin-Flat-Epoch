using System.ComponentModel.Composition;
using System.Windows;

namespace Hologram.NINA.FlatEpoch.SequenceItems {

    [Export(typeof(ResourceDictionary))]
    public partial class TakeTrainedEpochFlatsTemplate {

        public TakeTrainedEpochFlatsTemplate() {
            InitializeComponent();
        }
    }
}