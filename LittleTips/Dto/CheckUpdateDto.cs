namespace LittleTips.Dto
{
    public class CheckUpdateDto
    {
        public CheckUpdateDto(string version)
        {
            this.Version = version;
        }

        public string Version { get; set; }

        public int MajorVersionNumber
        {
            get
            {
                string[] s = Version.Split('.');
                return int.Parse(s[0]);
            }
        }

        public int MinorVersionNumber
        {
            get
            {
                string[] s = Version.Split('.');
                return int.Parse(s[1]);
            }
        }

        public int RevisionNumber
        {
            get
            {
                string[] s = Version.Split('.');
                return int.Parse(s[2]);
            }
        }

        public int BuildNumber
        {
            get
            {
                string[] s = Version.Split('.');
                return int.Parse(s[3]);
            }
        }
    }
}