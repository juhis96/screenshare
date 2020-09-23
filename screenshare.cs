using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Specialized;
using System.Net;

class screenshare {
	[StructLayout(LayoutKind.Sequential)]
	struct CURSORINFO {
		public Int32 cbSize;
		public Int32 flags;
		public IntPtr hCursor;
		public POINTAPI ptScreenPos;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct POINTAPI {
		public int x;
		public int y;
	}

	[DllImport("user32.dll")]
	static extern bool GetCursorInfo(out CURSORINFO pci);

	[DllImport("user32.dll")]
	static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

	const Int32 CURSOR_SHOWING = 0x00000001;

	public static Bitmap CaptureScreen(bool CaptureMouse) {
		Bitmap result = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format24bppRgb);

		try {
			using (Graphics g = Graphics.FromImage(result)) {
				g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

				if (CaptureMouse) {
					CURSORINFO pci;
					pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

					if (GetCursorInfo(out pci)) {
						if (pci.flags == CURSOR_SHOWING) {
							DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
							g.ReleaseHdc();
						}
					}
				}
			}
		}
		catch {
			result = null;
		}

		return result;
	}
	
	static void Main(string[] args) {
		int resize = 2;
		long quality = 50;
		string url = "http://localhost:8000/screenshare.php";
		int refreshrate = 2500;
		
		while(true){
			Bitmap bmp = CaptureScreen(true);
			bmp = new Bitmap(bmp,new Size(bmp.Width/resize, bmp.Height/resize));
			
			ImageCodecInfo imgCodecInfo;
			Encoder enc;
			EncoderParameter encParameter;
			EncoderParameters encParameters;

			imgCodecInfo = GetEncoderInfo("image/jpeg");

			enc = Encoder.Quality;

			encParameters = new EncoderParameters(1);

			encParameter = new EncoderParameter(enc, quality);
			encParameters.Param[0] = encParameter;
			
			MemoryStream ms = new MemoryStream();
			bmp.Save(ms, imgCodecInfo, encParameters);
			byte[] bmpBytes = ms.ToArray();
			
			string ImageData = Convert.ToBase64String(bmpBytes);

			using (WebClient client = new WebClient()) {
				byte[] response = client.UploadValues(url, new NameValueCollection() {
					{ "imgData", ImageData }
				});

				Console.WriteLine(System.Text.Encoding.Default.GetString(response));
			}
			
			Thread.Sleep(refreshrate);
			bmp.Dispose();
		}	

	}

	private static ImageCodecInfo GetEncoderInfo(String mimeType) {
		int i;
		ImageCodecInfo[] imgEncoders;
		imgEncoders = ImageCodecInfo.GetImageEncoders();
		for(i = 0; i < imgEncoders.Length; ++i) {
			if(imgEncoders[i].MimeType == mimeType)
				return imgEncoders[i];
		}
		return null;
	}

}  