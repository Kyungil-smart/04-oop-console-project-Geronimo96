namespace RhythmGameOOP
{
    public class Note
    {
        public int LaneIndex { get; private set; } // 몇 번째 줄인지
        public float Y { get; set; }               // 현재 높이 (실수형으로 부드럽게 이동)
        public bool IsHit { get; set; }            // 이미 맞춘 노트인지 확인 (중복 점수 방지)

        public Note(int laneIndex)
        {
            LaneIndex = laneIndex;
            Y = 0; // 맨 위에서 시작
            IsHit = false;
        }
    }
}