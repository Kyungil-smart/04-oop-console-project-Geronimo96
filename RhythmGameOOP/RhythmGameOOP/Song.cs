namespace RhythmGameOOP
{
    public class Song
    {
        public string Title { get; set; }     // 곡 제목 (화면에 표시될 이름)
        public string FilePath { get; set; }  // 파일 경로
        public double BPM { get; set; }       // BPM

        public Song(string title, string path, double bpm)
        {
            Title = title;
            FilePath = path;
            BPM = bpm;
        }
    }
}