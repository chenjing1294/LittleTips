namespace LittleTips.Dto
{
    public class ActivateResult
    {
        public int code { get; set; }
        public string token { get; set; }

        public bool activate { get; set; } //激活还是反激活
    }
}