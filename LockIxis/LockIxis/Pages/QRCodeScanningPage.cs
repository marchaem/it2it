using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace LockIxis.Pages
{
    public class QRCodeScanningPage : ZXingScannerPage
    {
        private static MobileBarcodeScanningOptions ScanningOptions = new MobileBarcodeScanningOptions
        {
            TryInverted = true,
            AutoRotate = false,
            UseFrontCameraIfAvailable = false,
            TryHarder = true,
            PossibleFormats = new List<ZXing.BarcodeFormat>
                {
                    ZXing.BarcodeFormat.QR_CODE
                }
        };

        public QRCodeScanningPage(ScanResultDelegate d) : base (ScanningOptions)
        {
            this.OnScanResult += d;
        }
    }
}
