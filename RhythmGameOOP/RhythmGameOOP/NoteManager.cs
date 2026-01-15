using System;
using System.Collections.Generic;

namespace RhythmGameOOP
{
    // [노트 관리자 클래스]
    // 화면에 떨어지는 모든 노트들의 생성, 이동, 삭제, 그리고 판정 검색을 담당합니다.
    // ★중요★: 멀티스레드 환경(게임루프 vs 키입력)에서 안전하게 작동하도록 'lock'을 사용합니다.
    public class NoteManager
    {
        // 화면에 존재하는 모든 노트를 담아두는 리스트
        private List<Note> notes;

        // 무작위 레인 선택을 위한 난수 생성기
        private Random rand;

        // 노트 생성 타이밍 계산을 위한 변수들
        private double lastSpawnTime; // 마지막으로 노트가 생성된 시간
        private double spawnInterval; // 다음 노트가 나올 때까지의 간격 (초)

        // [핵심: 스레드 보호용 자물쇠]
        // 리스트(notes)를 수정하거나 읽을 때, 이 객체를 사용해 문을 잠급니다.
        // 누군가 사용 중일 때 다른 스레드는 기다리게 만들어 충돌을 방지합니다.
        private object lockObj = new object();

        public NoteManager()
        {
            notes = new List<Note>();
            rand = new Random();

            // 박자 계산: 60초 / BPM = 1박자의 시간
            // 예: 120 BPM이면 0.5초마다 노트 생성
            spawnInterval = 60.0 / GlobalSettings.BPM;
        }

        // [로직 1: 노트 생성]
        // 시간이 되면 새로운 노트를 만들어 리스트에 넣습니다.
        public void SpawnLogic(double currentTime)
        {
            // 1. 노래 시작 후 싱크 조절 시간(SyncDelay)이 지났는지 확인
            if (currentTime > GlobalSettings.SyncDelay)
            {
                // 2. 마지막 생성 시간보다 '생성 주기'만큼 시간이 흘렀는지 확인
                if (currentTime - lastSpawnTime > spawnInterval)
                {
                    // [자물쇠 잠금]
                    // 리스트에 'Add'를 하는 도중에 다른 스레드가 리스트를 건드리면 에러가 납니다.
                    // 따라서 추가하는 동안에는 아무도 못 건드리게 잠급니다.
                    lock (lockObj)
                    {
                        // 현재 모드(4키/8키)에 맞춰 랜덤한 레인 번호 생성
                        int lane = rand.Next(0, GlobalSettings.LaneCount);

                        // 새 노트 추가
                        notes.Add(new Note(lane));
                    }

                    // 마지막 생성 시간을 갱신 (다음 타이밍 계산용)
                    lastSpawnTime = currentTime;
                }
            }
        }

        // [로직 2: 노트 이동 및 삭제]
        // 매 프레임마다 노트를 아래로 내리고, 화면 밖으로 나간 노트를 지웁니다.
        public void UpdateNotes(ScoreManager scoreManager)
        {
            // [자물쇠 잠금]
            // 리스트를 순회(for문)하거나 삭제(RemoveAt)하는 도중엔 절대 리스트가 변경되면 안 됩니다.
            lock (lockObj)
            {
                // 리스트 요소를 삭제할 때는 반드시 '뒤에서부터' 반복해야 인덱스 에러가 안 납니다.
                for (int i = notes.Count - 1; i >= 0; i--)
                {
                    // 노트의 Y 좌표를 속도만큼 증가 (아래로 이동)
                    notes[i].Y += GlobalSettings.NoteSpeed;

                    // 노트가 화면 바닥(TrackHeight)을 넘어갔다면? (놓침)
                    if (notes[i].Y >= GlobalSettings.TrackHeight)
                    {
                        // 플레이어가 맞춘 노트가 아니라면 Miss 처리
                        if (!notes[i].IsHit)
                        {
                            scoreManager.ResetCombo(); // 콤보 초기화 및 생명력 감소
                        }

                        // 화면 밖으로 나간 노트는 리스트에서 영구 삭제
                        // (이때 다른 스레드가 리스트를 읽고 있으면 충돌하므로 lock 필수)
                        notes.RemoveAt(i);
                    }
                }
            }
        }

        // [로직 3: 렌더링용 리스트 반환]
        // Renderer가 그림을 그릴 때 노트 리스트를 가져가기 위해 사용합니다.
        public List<Note> GetNotes()
        {
            // [자물쇠 잠금]
            lock (lockObj)
            {
                // ★핵심 기법★
                // 원본 리스트(notes)를 그냥 주면, 렌더링하는 도중에 UpdateNotes가 노트를 지워버릴 수 있습니다.
                // 그러면 "그리려던 노트가 사라졌습니다"라는 에러가 발생합니다.
                // 그래서 현재 상태를 복사한 '새로운 리스트'를 만들어서 전달합니다. (스냅샷)
                return new List<Note>(notes);
            }
        }

        // [로직 4: 판정 검색]
        // 사용자가 키를 눌렀을 때, 해당 레인에 있는 판정 가능한 노트를 찾습니다.
        public Note GetClosestNote(int laneIndex)
        {
            // [자물쇠 잠금]
            // 검색(foreach) 도중에 리스트 내용이 바뀌면 에러가 나므로 잠급니다.
            lock (lockObj)
            {
                Note target = null;
                float maxY = -1; // 판정선(아래쪽)에 가장 가까운 노트를 찾기 위한 변수

                foreach (var note in notes)
                {
                    // 1. 눌린 키(laneIndex)와 같은 줄에 있고
                    // 2. 아직 처리되지 않은(IsHit == false) 노트 중에서
                    if (note.LaneIndex == laneIndex && !note.IsHit)
                    {
                        // 가장 Y값이 큰(화면 아래쪽에 있는) 노트를 찾습니다.
                        if (note.Y > maxY)
                        {
                            maxY = note.Y;
                            target = note;
                        }
                    }
                }
                // 찾은 노트(또는 없으면 null)를 반환
                return target;
            }
        }
    }
}