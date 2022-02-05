namespace ChromiumUpdaterGUI
{
    internal class SerializeableVariables
    {
        public bool StartOnBoot { get; set; }
        public bool CheckUpdateOnClick { get; set; }
        public bool HideConfig { get; set; }
        public bool CheckForSelfUpdate { get; set; }
        public int DownloadTimeout { get; set; }
    }
}