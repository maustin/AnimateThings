using System;

namespace LinkedMovement.UI.Utils {
    public static class LMEase {
        public enum Values {
            Linear,
            InSine,
            OutSine,
            InOutSine,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InElastic,
            OutElastic,
            InOutElastic,
            InBack,
            OutBack,
            InOutBack,
            InBounce,
            OutBounce,
            InOutBounce,
        }

        private static string[] _names;

        public static string[] Names {
            get {
                if (_names == null) {
                    _names = Enum.GetNames(typeof(Values));
                }
                return _names;
            }
        }
    }
}
