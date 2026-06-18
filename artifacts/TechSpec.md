# TechSpec — 소방관 시뮬레이터 기본 규칙

**작성자**: 2271333 권성욱  
**버전**: v0.3  
**작성일**: 2026-06-17  
**엔진**: Unity 6 (URP) / C#  
**기반 코드**: PlayerController.cs, PlayerHealth.cs, WaterShooter.cs, FireManager.cs, GameManager.cs

---

## 0. 핵심 전제 규칙 — 모달 우선순위

> **모든 오브젝트, 입력, 이벤트, 상호작용 구현 전에 반드시 이 규칙을 확인한다.**

### 규칙: 모달이 열려 있으면 게임 세계는 완전히 정지한다

모달(PauseModal / GameOverModal / GameClearModal) 이 `SetActive(true)` 된 순간부터,  
아래 항목은 **코드 레벨에서 명시적으로 차단**되어야 한다.

| 차단 대상 | 차단 방법 | 담당 위치 |
|----------|---------|---------|
| 플레이어 이동 (WASD) | `Time.timeScale = 0` → FixedUpdate 미실행 | GameManager |
| 물 발사 (마우스 클릭) | `GameManager.IsPlaying` 체크 후 return | WaterShooter.Update() |
| 화재 피해 (OnTriggerStay) | `Time.timeScale = 0` → deltaTime = 0 | 자동 차단됨 |
| 점수 증가 | `GameManager.IsPlaying` 체크 | GameManager.AddScore() |
| 타이머 감소 | `isGameOver` or `isPaused` 체크 후 return | GameManager.Update() |
| ESC 입력 (중복 열기) | 이미 모달이 열려있으면 무시 | GameManager |
| 새 오브젝트 생성 (Instantiate) | 모달 열림 중 호출 금지 | 각 스크립트 |

### 구현 방법: IsPlaying 프로퍼티

모든 스크립트가 공유하는 단일 진실 소스(Single Source of Truth).

```csharp
// GameManager.cs
public bool IsPlaying => !isGameOver && !isPaused;
```

각 스크립트 Update() 첫 줄에 반드시 추가:

```csharp
// WaterShooter.cs, PlayerController.cs 등
void Update()
{
    if (!GameManager.Instance.IsPlaying) return;
    // 이후 로직...
}
```

### 모달 활성화 순서 (반드시 이 순서로)

```
1. isGameOver = true  또는  isPaused = true   ← 상태 먼저 변경
2. Time.timeScale = 0                          ← 물리/시간 정지
3. modal.SetActive(true)                       ← UI 표시
```

```
모달 닫을 때 (반드시 이 순서로)
1. modal.SetActive(false)                      ← UI 숨김
2. isPaused = false                            ← 상태 복원
3. Time.timeScale = 1                          ← 시간 재개
```

> **순서를 바꾸면 안 되는 이유**: SetActive 전에 timeScale을 복원하면  
> 모달이 닫히는 0.3초 애니메이션 중에도 게임이 진행되어 버그 발생.

---

## 1. 히트박스(Collider) 크기 규칙

> "판정이 공정하게 느껴지는" 핵심 원칙.  
> 위험 요소는 작게, 플레이어 공격/아이템은 크게.

| 오브젝트 | 컴포넌트 | 수치 | 시각 크기 대비 | 이유 |
|----------|---------|------|--------------|------|
| **Player** | CapsuleCollider | Height 1.6 / Radius 0.3 | ~80% | 불꽃 끝에서 억울하게 맞지 않게 |
| **FireZone** | SphereCollider (Trigger) | Radius 0.7 | ~70% | 시각적 불꽃보다 판정이 안쪽에 있어야 공정함 |
| **Water 발사체** | SphereCollider (Trigger) | Radius 0.25 | ~125% | 조준이 넉넉해야 쾌감, 잘 맞는 느낌 |
| **벽 / 바닥** | BoxCollider | 메시와 동일 | 100% | 지형은 눈에 보이는 대로 |

---

## 2. 이동 & 물리 수치

현재 코드(`PlayerController.cs`) 기준 확정값.

| 항목 | 값 | 코드 위치 |
|------|----|---------  |
| 기본 이동속도 | 5.0 m/s | `m_moveSpeed = 5.0f` |
| 달리기 (Shift) | 10.0 m/s | `moveDir *= 2.0f` |
| 점프력 | 5.0 (Impulse) | `m_jumpForce = 5.0f` |
| 점프 쿨타임 | 0.25초 | `m_minJumpInterval = 0.25f` |
| 중력 | -9.81 (Unity 기본값) | Project Settings |
| Rigidbody Freeze Rotation | X, Z 고정 | Inspector |

### 이동 방식 주의사항

현재 코드가 `transform.Translate` + `Rigidbody` 혼용 중.  
→ **물리 충돌이 씹히는 버그 발생 가능**.  
TechSpec 결정: 이동은 `Rigidbody.MovePosition()` 또는 `velocity`로 통일.

```csharp
// 권장 방식 (FixedUpdate에서)
void FixedUpdate()
{
    Vector3 worldMove = transform.forward * m_move.y + transform.right * m_move.x;
    m_rigidBody.MovePosition(m_rigidBody.position + worldMove * m_moveSpeed * Time.fixedDeltaTime);
}
```

---

## 3. 입력(Input) 정의

Unity Input System 사용. 레거시 `Input.GetAxis` 혼용 금지.

| 키 | 행동 | 현재 상태 |
|----|------|---------|
| WASD | 이동 | 구현됨 (`Move` Action) |
| Shift (홀드) | 달리기 2배 | 구현됨 |
| Space | 점프 | 구현됨 |
| 마우스 좌클릭 (홀드) | 물 발사 | 구현됨 (`Fire` Action) |
| ESC | 일시정지 | **미구현** → GameManager에 추가 필요 |
| F | 상호작용 | Debug.Log만 있음 (현재 미사용) |
| E | 인벤토리 | Debug.Log만 있음 (현재 미사용) |
| 1 / 2 / 3 / 4 | 퀵슬롯 | Debug.Log만 있음 (현재 미사용) |

**규칙**: 게임 상태가 `Playing`이 아닐 때는 WASD / 발사 입력을 무시한다.  
→ `GameManager.IsPlaying` 프로퍼티를 만들고 각 스크립트에서 체크.

---

## 4. 물 발사(Water) 수치

`WaterShooter.cs` 기준 확정값.

| 항목 | 값 |
|------|----|
| 발사 속도 (연사) | 0.1초 간격 (10발/초) |
| 발사체 수명 | 1.5초 |
| 발사체 이동속도 | **미정** → 10 m/s 권장 |
| 발사 방향 | `firePoint.rotation` (플레이어 전방) |

---

## 5. 화재(Fire) & 피해 수치

`FireZone.cs` 기준.

| 상황 | 피해량 |
|------|--------|
| 화재 구역 진입 시 (OnTriggerEnter) | 즉시 -10 HP |
| 화재 구역 안에 있는 동안 (OnTriggerStay) | -5 HP/초 |
| 물 발사체 명중 시 | 화재 오브젝트 즉시 제거 |

### HP 수치

| 항목 | 값 |
|------|----|
| 최대 HP | 100 |
| HP 0 도달 시 | `GameManager.Instance.GameOver()` 호출 |

---

## 6. 점수 공식

| 항목 | 점수 |
|------|------|
| 화재 1개 진압 | +100점 |
| 게임 클리어 시 시간 보너스 | 남은 초 × 5점 |
| **최고 점수 (화재 3개, 90초 클리어)** | 300 + 450 = **750점** |

```csharp
// GameManager.cs 클리어 시 계산 예시
int finalScore = score + Mathf.FloorToInt(remainingTime) * 5;
```

---

## 7. 게임 상태(State) 규칙

```
[Playing]
    │
    ├── ESC 키         → [Paused]  ←── ESC 또는 "계속하기"
    ├── 타이머 0        → [GameOver]
    ├── HP 0           → [GameOver]
    └── FireZone 0개   → [GameClear]
```

| 상태 | Time.timeScale | 플레이어 입력 | HUD |
|------|---------------|-------------|-----|
| Playing | 1 | 전체 허용 | 표시 |
| Paused | 0 | UI만 | 표시 |
| GameOver | 0 | UI만 | 숨김 |
| GameClear | 0 | UI만 | 숨김 |

**규칙**: `isGameOver = true` 이면 `Update()` 첫 줄에서 즉시 `return`.

---

## 8. UI 레이어 구조

```
Canvas  (Screen Space - Overlay, Scale With Screen Size 1920×1080)
├── HUD_Layer          ← Playing 중 항상 표시
│   ├── TimerPanel     (상단 중앙)
│   ├── HPPanel        (좌하단)
│   └── ScorePanel     (우상단)
└── Modal_Layer        ← 기본 SetActive(false)
    ├── PauseModal
    ├── GameClearModal
    └── GameOverModal
```

---

## 9. 타이머 UI 규칙

| 남은 시간 | 색상 | 효과 |
|---------|------|------|
| 31초 이상 | 흰색 `#FFFFFF` | 없음 |
| 10~30초 | 주황 `#FF8C00` | 없음 |
| 10초 이하 | 빨강 `#FF2222` | 0.5초 주기 Scale 펄스 (1.0 → 1.2 → 1.0) |

---

## 10. HP 바 색상 규칙

| HP 비율 | Fill 색상 |
|---------|----------|
| 70~100% | 초록 `#22DD44` |
| 30~69% | 주황 `#FFAA00` |
| 0~29% | 빨강 `#FF2222` + 화면 가장자리 빨간 비네트 |

---

## 11. 모달창 공통 규칙

| 항목 | 값 |
|------|----|
| 배경 오버레이 | 검정 반투명 alpha 0.7 (전체화면) |
| 패널 크기 | 600 × 400 px 고정 |
| 출현 애니메이션 | Scale 0 → 1, 0.3초 |
| 활성화 시 | `Time.timeScale = 0` |
| 닫힐 때 | `Time.timeScale = 1` |
| 기본 상태 | `SetActive(false)` |

### 모달별 내용

| 모달 | 표시 내용 | 버튼 |
|------|---------|------|
| PauseModal | "일시정지" | 계속하기 / 메인메뉴 |
| GameClearModal | "임무 완료!" + 최종 점수 + 남은 시간 | 다시하기 / 메인메뉴 |
| GameOverModal | "임무 실패" + 실패 원인 + 획득 점수 | 다시하기 / 메인메뉴 |

---

## 12. 피드백(Game Feel) 규칙

| 이벤트 | 시각 | 청각 |
|--------|------|------|
| 물 발사 | 파티클 생성 | 물 소리 |
| 화재 진압 | 오브젝트 제거 + `+100` 팝업 0.5초 | 진압음 |
| HP 피해 | 화면 가장자리 빨간 플래시 0.2초 | 피해음 |
| 게임 클리어 | 클리어 모달 0.3초 등장 | 승리음 |
| 게임 오버 | 오버 모달 0.3초 등장 | 실패음 |

---

## 13. 씬 구성

| 씬 이름 | Build Index | 역할 |
|---------|-----------|------|
| TitleScene | 0 | 메인메뉴 |
| SampleScene | 1 | 인게임 |

**씬 전환 규칙**:
- 다시하기 → `Time.timeScale = 1` 복원 후 `LoadScene` (순서 필수)
- 메인메뉴 이동 → 동일

```csharp
public void OnRetry()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}

public void OnMainMenu()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("TitleScene");
}
```

---

## 14. 미결 항목

| # | 항목 | 옵션 |
|---|------|------|
| 1 | 카메라 방식 | 3인칭 고정 오프셋 vs 마우스로 회전 |
| 2 | 물 발사체 이동속도 | 10 m/s 권장, 확정 필요 |
| 3 | 화재 재생성 여부 | 없음 (1회 진압으로 종료) 권장 |
| 4 | 사운드 에셋 | Unity 기본 or 외부 무료 에셋 |
| 5 | 발사체 이동방식 | Rigidbody AddForce vs transform.Translate |
