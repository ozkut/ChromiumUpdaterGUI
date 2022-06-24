namespace ChromiumUpdater
{
    internal class SerializeableVariables
    {
        public bool StartOnBoot { get; set; }
        public bool CheckUpdateOnClick { get; set; }
        public bool HideConfig { get; set; }
        public bool CheckForSelfUpdate { get; set; }
        public int DownloadTimeout { get; set; }
        public bool CheckUpdateRegularly { get; set; }
        public int UpdateCheckInterval { get; set; }
        public bool ShowNotifWhenUpToDate { get; set; }
    }
}