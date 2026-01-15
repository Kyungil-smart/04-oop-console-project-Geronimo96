using System;
using System.Collections.Generic;

namespace RhythmGameOOP
{
    public class NoteManager
    {
        // 화면에 존재하는 모든 노트를 담아두는 리스트
        private List<Note> notes;

        // 무작위 생성을 위한 난수 생성기 (랜덤)
        private Random rand;

        // 타이밍 계산을 위한 변수들
        private double lastSpawnTime; // 마지막으로 노트가 나온 시간
        private double spawnInterval; // 노트가 나오는 간격 (초 단위)

        public NoteManager()
        {
            notes = new List<Note>();
            rand = new Random();

            // [박자 계산]
            // 1분은 60초입니다. BPM(Beats Per Minute)이 120이라면
            // 60 / 120 = 0.5초마다 한 번씩 노트가 나와야 박자가 맞습니다.
            spawnInterval = 60.0 / GlobalSettings.BPM;
        }

        // [핵심 로직 1] 시간이 되면 노트를 생성합니다.
        public void SpawnLogic(double currentTime)
        {
            // 1. 노래 시작 후 '싱크 조절 시간(SyncDelay)'이 지났는지 확인
            // (노래 시작하자마자 노트가 쏟아지면 당황스러우니 텀을 줍니다)
            if (currentTime > GlobalSettings.SyncDelay)
            {
                // 2. 마지막 생성 시간보다 '생성 주기(interval)'만큼 시간이 흘렀는지 확인
                if (currentTime - lastSpawnTime > spawnInterval)
                {
                    // [★중요] 현재 모드(4키/8키)에 맞춰서 랜덤한 레인을 뽑습니다.
                    // LaneCount가 4면 -> 0, 1, 2, 3 중에서 랜덤
                    // LaneCount가 8이면 -> 0, 1, 2, 3, 4, 5, 6, 7 중에서 랜덤
                    int lane = rand.Next(0, GlobalSettings.LaneCount);

                    // 뽑힌 레인에 새 노트 생성 후 리스트에 추가
                    notes.Add(new Note(lane));

                    // 마지막 생성 시간을 현재 시간으로 갱신 (다음 노트 타이밍을 위해)
                    lastSpawnTime = currentTime;
                }
            }
        }

        // [핵심 로직 2] 노트를 아래로 이동시키고, 화면 밖으로 나가면 삭제합니다.
        public void UpdateNotes(ScoreManager scoreManager)
        {
            // [주의] 리스트에서 요소를 삭제할 때는 반드시 '뒤에서부터' 반복문을 돌려야 안전합니다.
            // 앞에서부터 지우면 인덱스가 밀려서 에러가 나거나 노트를 건너뛰게 됩니다.
            for (int i = notes.Count - 1; i >= 0; i--)
            {
                // 노트의 Y 좌표를 속도만큼 증가 (아래로 내려옴)
                notes[i].Y += GlobalSettings.NoteSpeed;

                // 노트가 화면 바닥(TrackHeight)을 뚫고 지나갔다면? (놓친 경우)
                if (notes[i].Y >= GlobalSettings.TrackHeight)
                {
                    // 플레이어가 이미 맞춘 노트가 아니라면? (진짜 놓친 것)
                    if (!notes[i].IsHit)
                    {
                        // 콤보 초기화 및 목숨 감소 등의 처리를 수행
                        scoreManager.ResetCombo();
                    }

                    // 맞췄든 못 맞췄든, 화면 밖으로 나간 노트는 무조건 리스트에서 삭제!
                    // (메모리 절약 및 다음 프레임 중복 처리 방지)
                    notes.RemoveAt(i);
                }
            }
        }

        // 렌더링(그리기) 클래스가 노트 목록을 읽어갈 수 있게 해주는 함수
        // IReadOnlyList를 사용하여 외부에서 함부로 리스트를 수정하지 못하게 보호합니다.
        public IReadOnlyList<Note> GetNotes()
        {
            return notes;
        }

        // [판정 로직] 특정 키를 눌렀을 때, 그 라인에서 가장 가까운 노트를 찾아줍니다.
        public Note GetClosestNote(int laneIndex)
        {
            Note target = null;
            float maxY = -1; // Y값이 클수록 아래에 있다는 뜻 (화면 좌표계)

            foreach (var note in notes)
            {
                // 1. 내가 누른 키(laneIndex)와 같은 줄에 있고
                // 2. 아직 맞추지 않은(IsHit == false) 노트 중에서
                if (note.LaneIndex == laneIndex && !note.IsHit)
                {
                    // 가장 아래쪽(판정선에 가까운) 노트를 타겟으로 잡습니다.
                    if (note.Y > maxY)
                    {
                        maxY = note.Y;
                        target = note;
                    }
                }
            }
            // 찾았으면 노트 객체를, 못 찾았으면 null을 반환
            return target;
        }
    }
}