using SpiderEye;

namespace OAuthWebView {
    public class OAuthWindow {
        Uri startUri;
        Uri endUri;
        string title;
        Size size;

        Uri currentUri;

        public OAuthWindow(Uri startUri, Uri endUri, string windowTitle, double width, double height) {
            this.startUri = startUri;
            this.endUri = endUri;
            title = windowTitle;
            size = new Size(width, height);
        }

        public Uri StartAuthentication() {
#if WIN
            SpiderEye.Windows.WindowsApplication.Init();
#elif LINUX
            SpiderEye.Linux.LinuxApplication.Init();
#elif MAC
            SpiderEye.Mac.MacApplication.Init();
#endif

            using (var window = new Window() {
#if LINUX
                CanResize = true,
#else
                CanResize = false,
#endif
                Title = title,
                Size = size
            }) {
                window.Navigating += Window_Navigating;
                Application.Run(window, startUri.ToString());
            }

            return currentUri.AbsolutePath == endUri.AbsolutePath ? currentUri : null;
        }

        private void Window_Navigating(object sender, NavigatingEventArgs e) {
            currentUri = e.Url;
            if (currentUri.AbsolutePath == endUri.AbsolutePath) Application.Exit();
        }
    }
}
