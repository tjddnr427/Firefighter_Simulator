---
name: agentic-ai-generator
description: >
  어떤 프로젝트에든 즉시 적용 가능한 AI 개발 환경 세팅 메타 스킬.
  프로젝트 설명 파일 하나를 주면 PRD(제품명세서) · TechSpec · 티켓(TiDD) ·
  하네스 구조(에이전트/스킬) · GitHub 워크플로우 · CI/CD 까지 전부 세팅한다.
  "개발 세팅", "환경 구성", "프로젝트 초기화", "PRD 만들어줘", "티켓 뽑아줘",
  "하네스 구성해줘", "에이전트 팀 만들어줘" 등의 요청 시 반드시 이 스킬을 먼저 사용한다.
  재실행, 업데이트, 일부 재구성 요청도 이 스킬로 처리한다.
---

# Agentic AI Generator

> 이 파일 하나를 프로젝트 루트에 넣으면, Claude가 전체 개발 환경을 자동으로 세팅한다.
> 기반 방법론: Speckit(SDD) · GSD · PRD/TechSpec/Tasks/TiDD 4단계 · harness-lab 7요소

---

## 사용법

```
# 방법 1 — 이 파일을 직접 읽어서 실행
"Agentic_AI_Generator.md를 읽고 [프로젝트 설명 파일]을 기반으로 개발 환경 세팅해줘"

# 방법 2 — 스킬로 설치 후 호출
/agentic-ai-generator [프로젝트 설명 파일 경로 또는 프로젝트 설명]

# 방법 3 — 단계별 실행
/agentic-ai-generator prd       → PRD만 생성
/agentic-ai-generator tickets   → 티켓만 생성
/agentic-ai-generator harness   → 하네스 구조만 생성
/agentic-ai-generator github    → GitHub 워크플로우만 설정
```

---

## 역할

이 스킬은 다음을 자동으로 처리한다.

| 단계 | 방법론 | 산출물 |
|------|--------|--------|
| 정의 | Speckit Constitution + PRD | `artifacts/PRD.md` |
| 설계 | TechSpec + Speckit Plan | `artifacts/TechSpec.md` |
| 분해 | GSD + Speckit Tasks | `artifacts/Tasks.md` |
| 티켓 | TiDD | `artifacts/Tickets.md` + GitHub 이슈 |
| 하네스 | harness-lab 7요소 | `.claude/agents/`, `.claude/skills/` |
| 자동화 | CI/CD | `.github/workflows/ci.yml`, `cd.yml` |

---

## 기본 원칙

1. **청사진 먼저** — 파일을 만들기 전, 반드시 청사진을 보여주고 사용자 승인을 받는다.
2. **산출물 우선** — "무엇이 나오면 성공인가"를 먼저 정하고 역방향으로 설계한다.
3. **컨텍스트 분리** — 각 에이전트는 독립된 컨텍스트에서 실행한다(GSD Fresh Context).
4. **티켓 주도** — 모든 구현 작업은 이슈 번호에 연결된 브랜치에서 시작한다(TiDD).
5. **작게 자주** — Phase 단위로 나누어 배칭 문제를 예방한다.
6. **헌법 우선** — Constitution에 어긋나는 구현은 진행하지 않는다.
7. **기록 필수** — 모든 중간 산출물은 `artifacts/`에 파일로 남긴다.

---

## 실행 흐름 (8 Phases)

### Phase 0 — 현황 파악 & 헌법 수립 (Constitution)

**목적**: 프로젝트의 불변 제약 조건을 먼저 확정한다.

**수행 순서**:
1. 입력 파일(또는 사용자 설명)을 읽는다.
2. 기존 `.claude/`, `CLAUDE.md`, `artifacts/`가 있으면 읽고 신규/확장/유지보수 중 하나로 분류한다.
3. 아래 Constitution 항목을 사용자와 확인한다.

**Constitution 확인 항목**:
```
- 프로젝트 이름 및 한 줄 설명
- 기술 스택 제약 (사용할/사용 금지 기술)
- 대상 사용자 (페르소나)
- 배포 환경 (웹, 앱, 서버, 게임 엔진 등)
- 팀 규모 및 역할
- 마감 기한
- 보안/법적 제약
- 정의상 성공 기준 (North Star Metric)
```

**산출물**: `artifacts/constitution.md`

---

### Phase 1 — PRD 생성 (Specify + Clarify)

**목적**: "무엇을, 왜 만드는가"를 문서화한다.

**수행 순서**:
1. Constitution을 기반으로 PRD 초안을 작성한다.
2. 모호한 항목은 `확인 필요` 태그로 표시하고 사용자에게 질문한다(최대 3개).
3. 답변을 반영해 PRD를 완성한다.

**PRD 섹션 (Speckit Specify 기반)**:
```
0. Document Information (작성자, 버전, 날짜)
1. Project Overview (배경, 목적)
2. User Story ([누가]로서 [무엇]을 원한다, 왜냐하면 [이유] 때문이다)
3. Target Users (페르소나 정의)
4. KPI / North Star Metric
5. Functional Requirements (MoSCoW: Must/Should/Could/Won't)
6. UI/UX (User Flow, Wireframe 설명, Interaction Rules)
7. Technical Constraints (기술 제약, NFR — 비기능 요구사항)
8. Acceptance Criteria (EARS 표기법: "~할 때 시스템은 ~해야 한다")
9. Out of Scope
```

**산출물**: `artifacts/PRD.md`

---

### Phase 2 — TechSpec 생성 (Plan)

**목적**: "어떻게 만드는가"를 기술로 번역한다.

**수행 순서**:
1. PRD의 Functional Requirements를 기술 명세로 변환한다.
2. 아키텍처 패턴을 선택한다 (아래 5종 중).
3. 데이터 모델, API 명세, 성능/보안 기준을 작성한다.

**아키텍처 선택 기준**:
```
클라이언트-서버  → 단순 MVP, 빠른 개발
계층형(Layered)  → 팀 협업, 관심사 분리 필요 시
마이크로서비스   → 독립 스케일링, 복잡한 도메인
이벤트 기반      → 실시간 처리, 느슨한 결합 필요
헥사고날         → 도메인 순수성, 외부 교체 가능성
```

**TechSpec 섹션**:
```
1. Architecture Decision (선택 이유 포함)
2. System Components & Responsibilities
3. Data Model / ERD
4. API Specification (Endpoint, Method, Request, Response, Status Code)
5. Security Design (인증·인가, 암호화, Rate Limiting)
6. Performance Targets (SLI/SLO: p99 latency, throughput)
7. Infrastructure & Deployment
8. Tech Stack (확정 목록)
9. Risks & Mitigations
```

**산출물**: `artifacts/TechSpec.md`

---

### Phase 3 — 작업 분해 & 티켓 생성 (Tasks → TiDD)

**목적**: TechSpec을 구현 가능한 최소 작업 단위로 쪼개고 GitHub 이슈로 발행한다.

**수행 순서**:
1. TechSpec의 기능을 Phase 단위로 묶는다.
2. 각 Phase를 이슈(티켓)로 분해한다 (배칭 문제 예방).
3. 이슈 목록을 작성한다.
4. 사용자 승인 후 GitHub 이슈 생성 명령어를 제공한다.

**이슈 작성 형식**:
```markdown
## #[번호] [타입]: [제목]

**Phase**: Phase [N] — [단계명]
**라벨**: [feature | bug | test | docs | ci | refactor]
**우선순위**: [P0 Must | P1 Should | P2 Could]
**예상 소요**: [시간/일]
**브랜치**: [feature/N-short-description]

### 사용자 스토리
[누가]로서 [무엇]을 원한다, 왜냐하면 [이유] 때문이다.

### 완료 기준 (Acceptance Criteria)
- [ ] [EARS: ~할 때 시스템은 ~해야 한다]
- [ ] [테스트 통과 조건]
- [ ] [성능 기준 — 해당 시]

### 기술 힌트
[구현 참고사항]

### 의존 이슈
[#번호 또는 없음]
```

**라벨 색상 표준**:
```
feature  #16a34a (녹색)  — 새 기능
bug      #ef4444 (빨강)  — 버그 수정
test     #3b82f6 (파랑)  — 테스트 코드
docs     #f59e0b (노랑)  — 문서
ci       #8b5cf6 (보라)  — CI/CD
refactor #06b6d4 (청록)  — 리팩터링
```

**Kanban 컬럼**: `📋 Todo` → `🔧 In Progress` → `👀 Review` → `✅ Done`

**브랜치 네이밍**: `[타입]/[이슈번호]-[짧은-설명]`
예) `feature/3-user-login`, `fix/7-null-pointer`

**산출물**: `artifacts/Tasks.md`, `artifacts/Tickets.md`

---

### Phase 4 — 하네스 설계 (Harness Blueprint)

**목적**: 프로젝트에 맞는 에이전트 팀과 스킬 구조를 설계한다.

**수행 순서**:
1. 프로젝트 규모에 따라 에이전트를 선택한다.
2. 팀 패턴을 선택한다.
3. 청사진을 사용자에게 보여주고 승인을 받는다.

**에이전트 풀 (필요한 것만 선택)**:
```
researcher.md     — 기술 조사, 레퍼런스 수집
architect.md      — 아키텍처 결정, 기술 스택 선택
developer.md      — 구현 코드 작성
reviewer.md       — 코드 리뷰, PR 검토
tester.md         — 테스트 케이스 작성, 품질 검증
pm.md             — 일정 관리, 이슈 추적, 칸반 업데이트
devops.md         — CI/CD, 인프라, 배포
security.md       — 보안 검토, 취약점 분석
```

**팀 패턴 선택 기준**:
```
파이프라인     → 순차 의존 (PRD→TechSpec→구현)
팬아웃/팬인    → 독립 병렬 (모듈별 동시 개발)
생성-검증      → 코드 작성 → 리뷰 → 승인
감독자         → 복잡한 조율이 필요한 대형 프로젝트
전문가 풀      → 상황별 전문 에이전트 선택 호출
계층적 위임    → 단계별 재귀 위임
```

**팀 규모 가이드**:
```
소규모 (이슈 10개 미만) → 2~3명
중규모 (10~20개)       → 3~5명
대규모 (20개+)         → 5~7명 (절대 초과 금지)
```

**산출물**: 청사진 문서 (파일 생성 전 승인 대기)

---

### Phase 5 — 하네스 구현 (Harness Build)

**목적**: 승인된 청사진을 실제 파일로 생성한다.

**생성 파일 구조**:
```
[프로젝트 루트]/
├── CLAUDE.md                          ← 하네스 포인터 + 라우팅 규칙
├── Agentic_AI_Generator.md            ← 이 파일 (보존)
├── artifacts/
│   ├── README.md                      ← 산출물 지도
│   ├── constitution.md
│   ├── PRD.md
│   ├── TechSpec.md
│   ├── Tasks.md
│   ├── Tickets.md
│   └── improvement-log.md
├── .claude/
│   ├── agents/
│   │   ├── [선택된 에이전트].md (각 파일)
│   │   └── ...
│   └── skills/
│       ├── [프로젝트명]-orchestrator/
│       │   └── SKILL.md              ← 전체 진행 관리
│       ├── prd-generator/
│       │   └── SKILL.md
│       ├── ticket-creator/
│       │   └── SKILL.md
│       └── github-setup/
│           └── SKILL.md
└── .github/
    └── workflows/
        ├── ci.yml
        └── cd.yml
```

**CLAUDE.md 구조**:
```markdown
# [프로젝트명] — 하네스 포인터

## 자연어 라우팅
- "[프로젝트] 관련 작업" → `[프로젝트명]-orchestrator` 스킬 사용
- "PRD 업데이트" → `prd-generator` 스킬 사용
- "티켓 생성" → `ticket-creator` 스킬 사용
- "GitHub 설정" → `github-setup` 스킬 사용

## 하네스 구조
- 에이전트: `.claude/agents/`
- 스킬: `.claude/skills/`
- 산출물: `artifacts/`

## 변경 이력
| 날짜 | 변경 내용 | 대상 | 사유 |
|------|----------|------|------|
| [날짜] | 초기 구성 | 전체 | Agentic_AI_Generator |
```

**에이전트 파일 구조**:
```markdown
---
name: [에이전트명]
description: [적극적 트리거 설명 — 구체적 상황 명시]
---

## 핵심 역할
[한 줄 역할 정의]

## 책임 범위
- 담당: [목록]
- 비담당: [다른 에이전트에게 위임할 것]

## 입력
- 받는 것: [파일/메시지]
- 기대 형식: [형식]

## 출력
- 만드는 것: [산출물]
- 저장 위치: `artifacts/[파일명]`

## 팀 통신
- `SendMessage` 수신 대상: [에이전트 목록]
- `SendMessage` 발신 대상: [에이전트 목록]
- `TaskUpdate` 시점: [완료/차단 시]

## 품질 기준
- [검증 체크리스트]
```

**오케스트레이터 스킬 구조**:
```markdown
---
name: [프로젝트명]-orchestrator
description: >
  [프로젝트명] 전체 개발 흐름 관리. PRD→TechSpec→티켓→구현→배포까지.
  "개발 시작", "다음 단계", "재실행", "업데이트" 요청 시 이 스킬 사용.
---

## Phase 1: 컨텍스트 확인
artifacts/ 존재 여부 확인 →
  있음 + 부분 수정 → 해당 Phase만 재실행
  있음 + 새 요청 → 기존 백업 후 새 실행
  없음 → 초기 실행 (Phase 0부터)

## Phase 2: 팀 구성
TeamCreate → 필요 에이전트 선택
TaskCreate → Phase별 작업 등록 (의존 관계 포함)

## Phase 3~N: 작업 실행
각 Phase마다:
1. 담당 에이전트에게 TaskCreate
2. SendMessage로 컨텍스트 전달
3. TaskGet으로 진행 상태 확인
4. 완료 시 artifacts/ 파일 확인
5. 사람 승인 필요 지점에서 대기

## 산출물 계약
[단계] → [파일 경로] → [다음 단계가 읽는 파일]

## 팀 정리
TeamDelete (모든 작업 완료 후)
```

---

### Phase 6 — GitHub 워크플로우 설정 (GitHub Setup)

**목적**: 이슈, 라벨, 칸반, CI/CD를 자동 설정한다.

**수행 순서**:
1. GitHub CLI 명령어 목록을 생성한다.
2. 사용자 승인 후 실행하거나 실행 가이드를 제공한다.

**자동 생성 명령어 (gh CLI)**:

```bash
# 1. 라벨 생성
gh label create "feature" --color "#16a34a" --description "새로운 기능"
gh label create "bug"     --color "#ef4444" --description "버그 수정"
gh label create "test"    --color "#3b82f6" --description "테스트 코드"
gh label create "docs"    --color "#f59e0b" --description "문서"
gh label create "ci"      --color "#8b5cf6" --description "CI/CD"
gh label create "P0"      --color "#dc2626" --description "Must Have"
gh label create "P1"      --color "#ea580c" --description "Should Have"
gh label create "P2"      --color "#65a30d" --description "Could Have"

# 2. 이슈 생성 (artifacts/Tickets.md 기반)
gh issue create --title "[타입]: [제목]" \
  --body "[이슈 본문]" \
  --label "feature,P0"

# 3. GitHub Project (칸반) 생성
gh project create --title "[프로젝트명] Sprint 1" --owner "@me"

# 4. 마일스톤 생성
gh api repos/{owner}/{repo}/milestones \
  -f title="Phase 1 — [단계명]" \
  -f due_on="[날짜]T00:00:00Z"
```

**CI 워크플로우** (`.github/workflows/ci.yml`):
```yaml
name: CI
on:
  pull_request:
    branches: [main, develop]
  push:
    branches: [main]

jobs:
  lint:
    name: 1) 정적 분석 (Lint)
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: "20", cache: "npm" }
      - run: npm ci
      - run: npm run lint

  security:
    name: 2) 보안 검사
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: "20", cache: "npm" }
      - run: npm ci
      - run: npm audit --audit-level=high

  unit-test:
    name: 3) 단위 테스트
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: "20", cache: "npm" }
      - run: npm ci
      - run: npm run test:unit

  integration-test:
    name: 4) 통합 테스트
    needs: unit-test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: "20", cache: "npm" }
      - run: npm ci
      - run: npm run test:integration
```

**CD 워크플로우** (`.github/workflows/cd.yml`):
```yaml
name: CD
on:
  push:
    branches: [main]
    tags: ['v*']

jobs:
  build-and-push:
    name: Docker 빌드 & 푸시
    runs-on: ubuntu-latest
    needs: []  # CI jobs reference here
    steps:
      - uses: actions/checkout@v4
      - name: Docker 빌드
        run: docker build -t ghcr.io/${{ github.repository }}:${{ github.sha }} .
      - name: GHCR 로그인
        run: echo "${{ secrets.GITHUB_TOKEN }}" | docker login ghcr.io -u $ --password-stdin
      - name: 이미지 푸시
        run: docker push ghcr.io/${{ github.repository }}:${{ github.sha }}

  e2e-test:
    name: E2E 테스트
    needs: build-and-push
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - run: npm ci
      - run: npm run test:e2e

  smoke-test:
    name: Smoke 테스트 (배포 후)
    needs: e2e-test
    runs-on: ubuntu-latest
    steps:
      - run: curl -f ${{ secrets.DEPLOY_URL }}/health || exit 1
```

---

### Phase 7 — 검증 & 진화 (Analyze + Evolution)

**목적**: 생성된 모든 것을 검증하고 개선 루프를 설정한다.

**검증 체크리스트**:
```
구조 검증
- [ ] .claude/agents/ — 모든 에이전트 파일 존재
- [ ] .claude/skills/ — 오케스트레이터 포함 모든 스킬 존재
- [ ] CLAUDE.md — 라우팅 규칙 + 변경 이력 존재
- [ ] artifacts/ — PRD, TechSpec, Tasks, Tickets 존재
- [ ] .github/workflows/ — ci.yml, cd.yml 존재

내용 검증
- [ ] PRD — 모든 7섹션 작성, 모호한 항목 없음
- [ ] TechSpec — 아키텍처 선택 이유 명시
- [ ] Tickets — 모든 이슈에 AC(완료 기준) 포함
- [ ] 에이전트 — 각 파일에 책임/입력/출력/통신 명시
- [ ] 오케스트레이터 — 재실행 분기 존재

테스트 프롬프트 (최소 3개)
- 정상: "[정상적인 작업 요청]"
- 애매: "[모호한 표현의 요청]"
- 실패: "[잘못된 입력이나 엣지 케이스]"
```

**개선 루프 (harness-lab 진화 메커니즘)**:
```
실행 완료 → 피드백 수집 → artifacts/improvement-log.md 기록
                        → .claude/agents/ 업데이트 (해당 시)
                        → .claude/skills/ 업데이트 (해당 시)
                        → CLAUDE.md 변경 이력 기록
```

**`artifacts/improvement-log.md` 형식**:
```markdown
# 개선 기록

## [날짜] — [변경 제목]
- **대상**: [파일명]
- **변경 내용**: [설명]
- **사유**: [피드백 또는 실패 원인]
- **효과**: [개선 결과]
```

---

## 산출물 완성 기준

### PRD 완성 기준
- [ ] 사용자 스토리가 "[누가]/[무엇]/[왜]" 3단 공식으로 작성됨
- [ ] 기능 요건에 MoSCoW 우선순위 표시
- [ ] AC가 EARS 표기법으로 측정 가능하게 작성됨
- [ ] KPI에 North Star Metric 포함
- [ ] Out of Scope 명시

### TechSpec 완성 기준
- [ ] 아키텍처 선택 이유 명시
- [ ] API 명세: 엔드포인트/메서드/요청/응답/상태코드
- [ ] SLO 수치 명시 (p99 latency 등)
- [ ] 보안: 인증·인가 방식 결정

### 티켓 완성 기준
- [ ] 모든 티켓에 고유 번호 부여
- [ ] Phase별 그룹화
- [ ] 브랜치명 패턴 포함
- [ ] 의존 이슈 표시
- [ ] 예상 소요 시간 포함

### 하네스 완성 기준
- [ ] 오케스트레이터 스킬에 재실행 분기 포함
- [ ] 모든 에이전트에 팀 통신 프로토콜 명시
- [ ] CLAUDE.md에 자연어 라우팅 규칙 포함
- [ ] 테스트 프롬프트 최소 3개

---

## 피해야 할 것

- 청사진 승인 전에 파일 생성 금지
- 단일 에이전트로 처리 가능한 작업에 팀 구성 금지
- `CLAUDE.md`에 에이전트/스킬 상세 규칙 복사 금지 (포인터만)
- 사람 승인 없이 외부 발송·배포·삭제 자동 완료 금지
- 티켓 없이 브랜치 생성 금지 (TiDD 원칙)
- `artifacts/`를 거치지 않는 중간 산출물 금지

---

## 자가 설치 (Optional)

이 파일을 `.claude/skills/agentic-ai-generator/SKILL.md`로 복사하면
`/agentic-ai-generator` 슬래시 명령으로 직접 호출할 수 있다.

```bash
mkdir -p .claude/skills/agentic-ai-generator
cp Agentic_AI_Generator.md .claude/skills/agentic-ai-generator/SKILL.md
```

---

## 버전 정보

| 항목 | 내용 |
|------|------|
| 버전 | v1.0.0 |
| 기반 방법론 | Speckit(SDD) · GSD · TiDD · harness-lab |
| 기반 자료 | 한성대 소프트웨어공학 강의 (10~14강) |
| 적용 범위 | 어떤 언어/프레임워크/프로젝트 규모에도 적용 |
| 최초 생성 | 2026-06-16 |
