namespace RhythmGameOOP
{
    public class Song
    {
        public string Title { get; set; }     // 화면에 표시될 제목
        public string FilePath { get; set; }  // 실제 파일 위치
        public double BPM { get; set; }       // 곡의 빠르기

        public Song(string title, string path, double bpm)
        {
            Title = title;
            FilePath = path;
            BPM = bpm;
        }
    }
}