namespace ProjectDonut.UI.DialogueSystem
{
    public class Dialogue
    {
        public bool IsActive { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Text { get; set; }
        public float ShowTime { get; set; }
        public float ShowTimer { get; set; }
        public int CharCounter { get; set; }
        public float CharInterval { get; set; }
        public float CharTimer { get; set; }
    }
}
