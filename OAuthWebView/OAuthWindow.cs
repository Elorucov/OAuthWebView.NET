using SpiderEye;

namespace OAuthWebView {
    public class OAuthWindow {
        Uri startUri;
        Uri endUri;
        string title;
        Size size;

        Uri currentUri;
        ManualResetEventSlim mres;
        Window window;

        public OAuthWindow(Uri startUri, Uri endUri, string windowTitle, double width, double height) {
            this.startUri = startUri;
            this.endUri = endUri;
            title = windowTitle;
            size = new Size(width, height);
        }

        static bool initialized = false;
        static Task task = new Task(() => Application.Run());

        public static void Initialize() {
            if (initialized) return;
            initialized = true;


#if WIN
            SpiderEye.Windows.WindowsApplication.Init();
#elif LINUX
            SpiderEye.Linux.LinuxApplication.Init();
#elif MAC
            SpiderEye.Mac.MacApplication.Init();
#endif

            task.Start();
        }

        public static void Unititialize() {
            if (!initialized) return;
            initialized = false;

            task.Dispose();
        }

        public async Task<Uri> StartAuthenticationAsync() {
            mres = new ManualResetEventSlim();
            using (window = new Window() {
#if LINUX
                CanResize = true,
#else
                CanResize = false,
#endif
                Title = title,
                Size = size
            }) {
                window.Navigating += Window_Navigating;
                window.Closed += Window_Closed;
                window.Show();
                window.LoadUrl(startUri.ToString());
                await Task.Factory.StartNew(() => {
                    mres.Wait();
                }).ConfigureAwait(true);
            }

            mres.Dispose();
            return currentUri.AbsolutePath == endUri.AbsolutePath ? currentUri : null;
        }

        private void Window_Closed(object sender, EventArgs e) {
            window.Closed -= Window_Closed;
            mres.Set();
        }

        private void Window_Navigating(object sender, NavigatingEventArgs e) {
            Console.WriteLine($"Navigating to {e.Url}");
            System.Diagnostics.Debug.WriteLine($"Navigating to {e.Url}");
            currentUri = e.Url;
            if (currentUri.AbsolutePath == endUri.AbsolutePath) {
                window.Close();
            }
        }
    }
}